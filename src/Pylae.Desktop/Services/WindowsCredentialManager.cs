using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pylae.Desktop.Services;

/// <summary>
/// Helper class to interact with Windows Credential Manager for secure password storage.
/// </summary>
internal static class WindowsCredentialManager
{
    /// <summary>
    /// Saves a password to Windows Credential Manager.
    /// </summary>
    /// <param name="target">Unique identifier for this credential (e.g., "Pylae_Database_SiteCode")</param>
    /// <param name="username">Username (can be application name)</param>
    /// <param name="password">Password to store</param>
    public static bool SaveCredential(string target, string username, string password)
    {
        try
        {
            var credential = new CREDENTIAL
            {
                Type = CRED_TYPE.GENERIC,
                TargetName = target,
                UserName = username,
                CredentialBlob = Marshal.StringToCoTaskMemUni(password),
                CredentialBlobSize = (uint)(password.Length * 2), // Unicode = 2 bytes per char
                Persist = CRED_PERSIST.LOCAL_MACHINE,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                Comment = "Pylae database encryption password",
                TargetAlias = IntPtr.Zero,
            };

            var result = CredWrite(ref credential, 0);

            // Clean up password from memory
            if (credential.CredentialBlob != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(credential.CredentialBlob);
            }

            return result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves a password from Windows Credential Manager.
    /// </summary>
    /// <param name="target">Unique identifier for this credential</param>
    /// <returns>Password if found, null otherwise</returns>
    public static string? GetCredential(string target)
    {
        try
        {
            if (!CredRead(target, CRED_TYPE.GENERIC, 0, out var credPtr))
            {
                return null;
            }

            try
            {
                var credential = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
                if (credential.CredentialBlob == IntPtr.Zero)
                {
                    return null;
                }

                var password = Marshal.PtrToStringUni(
                    credential.CredentialBlob,
                    (int)credential.CredentialBlobSize / 2); // Unicode = 2 bytes per char

                return password;
            }
            finally
            {
                CredFree(credPtr);
            }
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Deletes a credential from Windows Credential Manager.
    /// </summary>
    public static bool DeleteCredential(string target)
    {
        try
        {
            return CredDelete(target, CRED_TYPE.GENERIC, 0);
        }
        catch
        {
            return false;
        }
    }

    #region P/Invoke Declarations

    [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredWrite([In] ref CREDENTIAL credential, [In] uint flags);

    [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredRead(
        string target,
        CRED_TYPE type,
        int flags,
        out IntPtr credential);

    [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredDelete(string target, CRED_TYPE type, int flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CredFree([In] IntPtr buffer);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public uint Flags;
        public CRED_TYPE Type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public CRED_PERSIST Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
    }

    private enum CRED_TYPE : uint
    {
        GENERIC = 1,
        DOMAIN_PASSWORD = 2,
        DOMAIN_CERTIFICATE = 3,
        DOMAIN_VISIBLE_PASSWORD = 4,
        GENERIC_CERTIFICATE = 5,
        DOMAIN_EXTENDED = 6,
        MAXIMUM = 7,
        MAXIMUM_EX = 1007
    }

    private enum CRED_PERSIST : uint
    {
        SESSION = 1,
        LOCAL_MACHINE = 2,
        ENTERPRISE = 3
    }

    #endregion
}
