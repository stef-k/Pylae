using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Data.Context;
using Pylae.Data.Seed;
using Pylae.Data.Services;
using Pylae.Desktop.Forms;
using Pylae.Desktop.Resources;
using Pylae.Desktop.Services;
using Pylae.Desktop.ViewModels;
using WindowsCredentialManager = Pylae.Desktop.Services.WindowsCredentialManager;
using Pylae.Sync;
using Serilog;
using System.Windows.Forms;
using System.Globalization;
using Pylae.Core.Models;
namespace Pylae.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetColorMode(SystemColorMode.System);

        var configuration = BuildConfiguration();
        var databaseOptions = BuildDatabaseOptions(configuration, promptForPassword: false);

        // Check if database exists for the configured site code
        var isFirstRun = !File.Exists(databaseOptions.GetMasterDbPath());

        // If not found, check if there's an existing database in ProgramData
        if (isFirstRun)
        {
            var discoveredSiteCode = DiscoverExistingDatabase(databaseOptions.RootPath);
            if (discoveredSiteCode != null)
            {
                var result = MessageBox.Show(
                    string.Format(Strings.DbPassword_DiscoveryMessage, discoveredSiteCode),
                    Strings.DbPassword_DiscoveryTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    databaseOptions.SiteCode = discoveredSiteCode;

                    // Prompt for password with retry on wrong password
                    if (PromptForPasswordWithRetry(databaseOptions, out var password))
                    {
                        databaseOptions.EncryptionPassword = password;
                        isFirstRun = false; // Use existing database
                    }
                    else
                    {
                        // User cancelled or failed too many times, proceed to first run
                        isFirstRun = true;
                    }
                }
            }
        }

        string? adminPassword = null;
        string? primaryLanguage = null;

        // IMPORTANT: Set encryption password BEFORE building service provider
        if (isFirstRun)
        {
            using var form = new FirstRunForm();
            if (form.ShowDialog() != DialogResult.OK)
            {
                throw new InvalidOperationException(Strings.FirstRun_Required);
            }

            adminPassword = form.AdminPassword;
            primaryLanguage = form.PrimaryLanguage;
            databaseOptions.SiteCode = form.SiteCode;
            databaseOptions.SiteDisplayName = form.SiteDisplayName;
            databaseOptions.EncryptionPassword = string.IsNullOrWhiteSpace(form.EncryptionPassword)
                ? PromptForPassword()
                : form.EncryptionPassword;

            if (string.IsNullOrWhiteSpace(databaseOptions.SiteCode) ||
                string.IsNullOrWhiteSpace(databaseOptions.SiteDisplayName) ||
                string.IsNullOrWhiteSpace(adminPassword) ||
                string.IsNullOrWhiteSpace(databaseOptions.EncryptionPassword))
            {
                throw new InvalidOperationException("First run configuration is incomplete.");
            }

            // Save site code to appsettings.json for future runs
            SaveSiteCodeToConfig(databaseOptions.SiteCode, databaseOptions.SiteDisplayName);

            // Save encryption password to Windows Credential Manager (delete any old one first)
            var credentialTarget = GetCredentialTargetName(databaseOptions.SiteCode);
            WindowsCredentialManager.DeleteCredential(credentialTarget);
            WindowsCredentialManager.SaveCredential(credentialTarget, "Pylae", databaseOptions.EncryptionPassword);

            // Create directories only after we know the actual site code
            DatabaseConfig.EnsureDirectories(databaseOptions);
        }
        else if (string.IsNullOrWhiteSpace(databaseOptions.EncryptionPassword))
        {
            // Database exists but no password provided, prompt with validation
            if (!PromptForPasswordWithRetry(databaseOptions, out var password))
            {
                // Max attempts reached or user cancelled - offer first-run setup
                var startFresh = MessageBox.Show(
                    Strings.DbPassword_MaxAttemptsReached,
                    Strings.DbPassword_MaxAttemptsTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (startFresh == DialogResult.Yes)
                {
                    isFirstRun = true; // Proceed to first-run setup
                }
                else
                {
                    // User chose not to continue
                    return;
                }
            }
            else
            {
                databaseOptions.EncryptionPassword = password;
            }
        }

        // Build services AFTER encryption password is set
        var services = ConfigureServices(configuration, databaseOptions);
        var provider = services.BuildServiceProvider();

        try
        {
            using var scope = provider.CreateScope();

            InitializeFirstRun(scope.ServiceProvider, databaseOptions, isFirstRun, adminPassword, primaryLanguage);
            if (!isFirstRun)
            {
                SeedDatabase(scope.ServiceProvider, databaseOptions);
            }
            else
            {
                MessageBox.Show(Strings.FirstRun_AdminLoginHint, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ApplyCulture(scope.ServiceProvider);
            StartHealthMonitor(scope.ServiceProvider);
            StartScheduledBackups(scope.ServiceProvider);
            StartAuditLogCleanup(scope.ServiceProvider);
            StartVisitCleanup(scope.ServiceProvider);
            scope.ServiceProvider.GetRequiredService<ThemeService>();

            using var idleLock = ConfigureIdleLock(scope.ServiceProvider);

#if DEBUG
            // Dev bypass: skip login when PYLAE_DEV_AUTO_LOGIN=true
            if (Environment.GetEnvironmentVariable("PYLAE_DEV_AUTO_LOGIN") == "true")
            {
                var devUser = GetDevAutoLoginUser(scope.ServiceProvider);
                if (devUser != null)
                {
                    var currentUserService = scope.ServiceProvider.GetRequiredService<CurrentUserService>();
                    currentUserService.CurrentUser = devUser;
                    var mainForm = scope.ServiceProvider.GetRequiredService<MainForm>();
                    mainForm.SetCurrentUser(devUser);
                    Application.Run(mainForm);
                }
                else
                {
                    // Fall back to normal login if no admin user found
                    var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
                    Application.Run(loginForm);
                }
            }
            else
#endif
            {
                var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
                Application.Run(loginForm);
            }

            // Perform shutdown tasks before disposal
            ShutdownServicesAsync(scope.ServiceProvider).GetAwaiter().GetResult();
        }
        finally
        {
            if (provider is IAsyncDisposable asyncProvider)
            {
                asyncProvider.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            else
            {
                provider.Dispose();
            }
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static DatabaseOptions BuildDatabaseOptions(IConfiguration configuration, bool promptForPassword = true)
    {
        var siteCode = configuration["Site:Code"] ?? DefaultSettings.All[SettingKeys.SiteCode];
        var displayName = configuration["Site:DisplayName"] ?? DefaultSettings.All[SettingKeys.SiteDisplayName];
        var rootPath = configuration["Database:RootPath"];

        // Try to get password from: 1) Environment variable, 2) Windows Credential Manager
        var encryptionPassword = Environment.GetEnvironmentVariable("PYLAE_DB_PASSWORD");
        if (string.IsNullOrWhiteSpace(encryptionPassword))
        {
            encryptionPassword = WindowsCredentialManager.GetCredential(GetCredentialTargetName(siteCode));
        }

        if (promptForPassword && string.IsNullOrWhiteSpace(encryptionPassword))
        {
            encryptionPassword = PromptForPassword();
        }

        return new DatabaseOptions
        {
            SiteCode = siteCode,
            SiteDisplayName = displayName,
            RootPath = string.IsNullOrWhiteSpace(rootPath)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Pylae")
                : Environment.ExpandEnvironmentVariables(rootPath),
            EncryptionPassword = encryptionPassword ?? string.Empty
        };
    }

    private static ServiceCollection ConfigureServices(IConfiguration configuration, DatabaseOptions databaseOptions)
    {
        var services = new ServiceCollection();

        services.AddSingleton(configuration);

        var loggerFactory = LoggingConfigurator.ConfigureSerilog(databaseOptions, DefaultSettings.All);
        services.AddSingleton<ILoggerFactory>(loggerFactory);
        services.AddLogging(builder => builder.AddSerilog());

        services.AddPylaeData(databaseOptions);
        services.AddPylaeSync();

        services.AddTransient<LoginForm>();
        services.AddTransient<MainForm>();
        services.AddTransient<RemoteSitesForm>();
        services.AddTransient<CatalogsForm>();
        services.AddTransient<MemberTypeEditorForm>();
        services.AddTransient<LockForm>();
        services.AddSingleton<CurrentUserService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<GateViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<MembersViewModel>();
        services.AddTransient<VisitsViewModel>();
        services.AddTransient<AuditLogViewModel>();
        services.AddTransient<CatalogsViewModel>();
        services.AddTransient<UsersViewModel>();
        services.AddTransient<IExportService, ExportService>();
        services.AddTransient<IBadgeRenderer, QuestPdfBadgeRenderer>();
        services.AddTransient<IPhotoValidator, PhotoValidator>();
        services.AddSingleton<IBackupService, BackupService>();
        services.AddSingleton<ScheduledBackupService>();
        services.AddSingleton<AuditLogCleanupService>();
        services.AddSingleton<VisitCleanupService>();
        services.AddSingleton<HealthMonitorService>();
        services.AddSingleton<MasterMergeService>();
        services.AddSingleton<SyncHistoryService>();
        services.AddSingleton<ThemeService>();

        return services;
    }

    private static void SeedDatabase(IServiceProvider provider, DatabaseOptions databaseOptions)
    {
        using var scope = provider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        seeder.SeedAsync(databaseOptions.EncryptionPassword).GetAwaiter().GetResult();
    }

    private static void ApplyCulture(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var settings = settingsService.GetAllAsync().GetAwaiter().GetResult();

        if (settings.TryGetValue(SettingKeys.PrimaryLanguage, out var culture))
        {
            try
            {
                var cultureInfo = new System.Globalization.CultureInfo(culture);
                System.Globalization.CultureInfo.CurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.CurrentUICulture = cultureInfo;
                Strings.Culture = cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                // Fallback to default culture
            }
        }
    }

    private static string PromptForPassword()
    {
        using var dialog = new Form
        {
            Width = 360,
            Height = 180,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = Strings.DbPassword_Title,
            StartPosition = FormStartPosition.CenterScreen
        };

        var label = new Label { Left = 20, Top = 20, Text = Strings.DbPassword_Prompt, AutoSize = true };
        var textBox = new TextBox { Left = 20, Top = 50, Width = 300, UseSystemPasswordChar = true };
        var okButton = new Button { Text = Strings.Button_OK, Left = 160, Width = 75, Top = 90, DialogResult = DialogResult.OK };
        var cancelButton = new Button { Text = Strings.Button_Cancel, Left = 245, Width = 75, Top = 90, DialogResult = DialogResult.Cancel };

        okButton.Click += (_, _) => dialog.Close();
        cancelButton.Click += (_, _) => dialog.Close();

        dialog.Controls.Add(label);
        dialog.Controls.Add(textBox);
        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = okButton;
        dialog.CancelButton = cancelButton;

        return dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text)
            ? textBox.Text
            : throw new InvalidOperationException("Database password is required.");
    }

    private static void StartHealthMonitor(IServiceProvider provider)
    {
        var monitor = provider.GetRequiredService<HealthMonitorService>();
        monitor.StartAsync().GetAwaiter().GetResult();
    }

    private static void StartScheduledBackups(IServiceProvider provider)
    {
        var backupService = provider.GetRequiredService<ScheduledBackupService>();
        backupService.StartAsync().GetAwaiter().GetResult();
    }

    private static void StartAuditLogCleanup(IServiceProvider provider)
    {
        var cleanupService = provider.GetRequiredService<AuditLogCleanupService>();
        cleanupService.StartAsync().GetAwaiter().GetResult();
    }

    private static void StartVisitCleanup(IServiceProvider provider)
    {
        var cleanupService = provider.GetRequiredService<VisitCleanupService>();
        cleanupService.StartAsync().GetAwaiter().GetResult();
    }

    private static void InitializeFirstRun(IServiceProvider provider, DatabaseOptions options, bool isFirstRun, string? adminPassword, string? primaryLanguage)
    {
        if (!isFirstRun)
        {
            return;
        }

        using var scope = provider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(Strings.FirstRun_AdminPassword);
        }

        // Validate encryption password is set
        if (string.IsNullOrWhiteSpace(options.EncryptionPassword))
        {
            throw new InvalidOperationException("Encryption password is not set during first run initialization.");
        }

        seeder.SeedAsync(adminPassword).GetAwaiter().GetResult();

        var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var defaults = new[]
        {
            new Setting { Key = SettingKeys.SiteCode, Value = options.SiteCode },
            new Setting { Key = SettingKeys.SiteDisplayName, Value = options.SiteDisplayName },
            new Setting { Key = SettingKeys.PrimaryLanguage, Value = primaryLanguage ?? "el-GR" }
        };
        settingsService.UpsertAsync(defaults).GetAwaiter().GetResult();
    }

    private static IdleLockService? ConfigureIdleLock(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
        var settings = settingsService.GetAllAsync().GetAwaiter().GetResult();
        var timeout = settings.TryGetValue(SettingKeys.IdleTimeoutMinutes, out var v) && int.TryParse(v, out var parsed)
            ? parsed
            : 0;

        if (timeout <= 0)
        {
            return null;
        }

        var currentUserService = provider.GetRequiredService<CurrentUserService>();
        return new IdleLockService(timeout, currentUserService, provider);
    }

    private static async Task ShutdownServicesAsync(IServiceProvider provider)
    {
        try
        {
            // Perform shutdown backup if enabled
            var backupService = provider.GetRequiredService<ScheduledBackupService>();
            await backupService.StopAsync();
        }
        catch (Exception ex)
        {
            // Log shutdown errors but don't block application exit
            var logger = provider.GetService<ILogger<ScheduledBackupService>>();
            logger?.LogError(ex, "Error during shutdown");
        }
    }

    private static bool PromptForPasswordWithRetry(DatabaseOptions options, out string password)
    {
        const int maxAttempts = 3;
        password = string.Empty;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // Prompt for password
                var enteredPassword = PromptForPasswordDialog();
                if (string.IsNullOrWhiteSpace(enteredPassword))
                {
                    // User cancelled
                    MessageBox.Show(Strings.DbPassword_Cancelled, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Validate password by attempting to connect
                if (ValidateEncryptionPassword(options, enteredPassword))
                {
                    password = enteredPassword;

                    // Save password to Windows Credential Manager for future use (delete any old one first)
                    var credTarget = GetCredentialTargetName(options.SiteCode);
                    WindowsCredentialManager.DeleteCredential(credTarget);
                    WindowsCredentialManager.SaveCredential(credTarget, "Pylae", enteredPassword);

                    return true;
                }

                // Wrong password
                if (attempt < maxAttempts)
                {
                    var retry = MessageBox.Show(
                        Strings.DbPassword_WrongPassword + $"\n\nAttempt {attempt} of {maxAttempts}.\n\n" + Strings.DbPassword_RetryPrompt,
                        Strings.App_Title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (retry != DialogResult.Yes)
                    {
                        return false;
                    }
                }
                else
                {
                    // Max attempts reached - caller will handle the message
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating password: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        return false;
    }

    private static bool ValidateEncryptionPassword(DatabaseOptions options, string password)
    {
        try
        {
            var connectionString = DatabaseConfig.BuildConnectionString(options.GetMasterDbPath(), password);
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            connection.Open();

            // Try a simple query to ensure the database is valid and decrypted
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' LIMIT 1";
            command.ExecuteScalar();

            return true;
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 26)
        {
            // Error 26: "file is not a database" - wrong password
            return false;
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 11)
        {
            // Error 11: "database disk image is malformed" - wrong password
            return false;
        }
        catch
        {
            // Other errors also indicate wrong password or corrupted DB
            return false;
        }
    }

    private static string? PromptForPasswordDialog()
    {
        using var dialog = new Form
        {
            Width = 360,
            Height = 180,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = Strings.DbPassword_Title,
            StartPosition = FormStartPosition.CenterScreen
        };

        var label = new Label { Left = 20, Top = 20, Text = Strings.DbPassword_Prompt, AutoSize = true };
        var textBox = new TextBox { Left = 20, Top = 50, Width = 300, UseSystemPasswordChar = true };
        var okButton = new Button { Text = Strings.Button_OK, Left = 160, Width = 75, Top = 90, DialogResult = DialogResult.OK };
        var cancelButton = new Button { Text = Strings.Button_Cancel, Left = 245, Width = 75, Top = 90, DialogResult = DialogResult.Cancel };

        okButton.Click += (_, _) => dialog.Close();
        cancelButton.Click += (_, _) => dialog.Close();

        dialog.Controls.Add(label);
        dialog.Controls.Add(textBox);
        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = okButton;
        dialog.CancelButton = cancelButton;

        return dialog.ShowDialog() == DialogResult.OK ? textBox.Text : null;
    }

    private static string? DiscoverExistingDatabase(string rootPath)
    {
        try
        {
            if (!Directory.Exists(rootPath))
            {
                return null;
            }

            // Search for ALL subdirectories that contain Data/master.db
            var foundDatabases = new List<string>();
            var subdirectories = Directory.GetDirectories(rootPath);

            foreach (var subdir in subdirectories)
            {
                var siteCode = Path.GetFileName(subdir);
                var masterDbPath = Path.Combine(subdir, "Data", "master.db");

                if (File.Exists(masterDbPath))
                {
                    foundDatabases.Add(siteCode);
                }
            }

            // Handle based on how many databases were found
            if (foundDatabases.Count == 0)
            {
                return null; // No databases found
            }
            else if (foundDatabases.Count == 1)
            {
                return foundDatabases[0]; // Only one database, use it
            }
            else
            {
                // Multiple databases found - let user choose
                return ShowDatabaseSelectionDialog(foundDatabases);
            }
        }
        catch (Exception)
        {
            // If we can't discover, fall back to config value
        }

        return null;
    }

    private static string? ShowDatabaseSelectionDialog(List<string> siteCodes)
    {
        using var dialog = new Form
        {
            Width = 450,
            Height = 300,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = "Select Database",
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var label = new Label
        {
            Left = 20,
            Top = 20,
            Width = 400,
            Height = 40,
            Text = "Multiple databases found. Please select which one to use:"
        };

        var listBox = new ListBox
        {
            Left = 20,
            Top = 70,
            Width = 400,
            Height = 120
        };

        foreach (var siteCode in siteCodes.OrderBy(s => s))
        {
            listBox.Items.Add(siteCode);
        }

        if (listBox.Items.Count > 0)
        {
            listBox.SelectedIndex = 0;
        }

        var okButton = new Button
        {
            Text = Strings.Button_OK,
            Left = 250,
            Width = 80,
            Top = 210,
            DialogResult = DialogResult.OK
        };

        var cancelButton = new Button
        {
            Text = Strings.Button_Cancel,
            Left = 340,
            Width = 80,
            Top = 210,
            DialogResult = DialogResult.Cancel
        };

        dialog.Controls.Add(label);
        dialog.Controls.Add(listBox);
        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = okButton;
        dialog.CancelButton = cancelButton;

        if (dialog.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
        {
            return listBox.SelectedItem.ToString();
        }

        return null; // User cancelled
    }

    private static string GetCredentialTargetName(string siteCode)
    {
        return $"Pylae_Database_{siteCode}";
    }

#if DEBUG
    private static User? GetDevAutoLoginUser(IServiceProvider provider)
    {
        try
        {
            var userService = provider.GetRequiredService<IUserService>();
            // Try to get "admin" user first, fall back to any admin role user
            var adminUser = userService.GetByUsernameAsync("admin").GetAwaiter().GetResult();
            if (adminUser != null)
            {
                return adminUser;
            }

            // Fall back to first user with Admin role
            var users = userService.GetAllAsync().GetAwaiter().GetResult();
            return users.FirstOrDefault(u => u.Role == UserRole.Admin);
        }
        catch
        {
            return null;
        }
    }
#endif

    private static void SaveSiteCodeToConfig(string siteCode, string siteDisplayName)
    {
        try
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var json = File.ReadAllText(configPath);

            // Parse JSON
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { Indented = true });

            writer.WriteStartObject();

            // Write Site section with updated values
            writer.WriteStartObject("Site");
            writer.WriteString("Code", siteCode);
            writer.WriteString("DisplayName", siteDisplayName);
            writer.WriteEndObject();

            // Copy other sections as-is
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                if (property.Name != "Site")
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
            writer.Flush();

            // Write back to file
            var updatedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            File.WriteAllText(configPath, updatedJson);
        }
        catch (Exception ex)
        {
            // Log but don't fail - the app can still work, just needs manual config update
            Console.WriteLine($"Warning: Could not update appsettings.json: {ex.Message}");
        }
    }
}
