using Microsoft.EntityFrameworkCore;
using Pylae.Data.Context;
using OfficeEntity = Pylae.Data.Entities.Master.Office;
using MemberTypeEntity = Pylae.Data.Entities.Master.MemberType;
using MemberEntity = Pylae.Data.Entities.Master.Member;

namespace Pylae.Desktop.Services;

/// <summary>
/// Merges remote master data into local database using UpdatedAtUtc where available.
/// </summary>
public record MergeCounts(int Added, int Updated, int Skipped, List<string> SkippedItems);

public record MergeResult(MergeCounts Offices, MergeCounts MemberTypes, MergeCounts Members)
{
    public bool HasConflicts => Offices.Skipped > 0 || MemberTypes.Skipped > 0 || Members.Skipped > 0;
}

public class MasterMergeService
{
    private readonly DatabaseOptions _options;

    public MasterMergeService(DatabaseOptions options)
    {
        _options = options;
    }

    public async Task<MergeResult> MergeAsync(byte[] remoteMasterDb, CancellationToken cancellationToken = default)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"pylae_remote_{Guid.NewGuid():N}.db");
        await File.WriteAllBytesAsync(tempPath, remoteMasterDb, cancellationToken);

        var localOpts = BuildOptions(_options.GetMasterDbPath());
        var remoteOpts = BuildOptions(tempPath);

        await using var local = new PylaeMasterDbContext(localOpts);
        await using var remote = new PylaeMasterDbContext(remoteOpts);

        var offices = await MergeOfficesAsync(local, remote, cancellationToken);
        var types = await MergeMemberTypesAsync(local, remote, cancellationToken);
        var members = await MergeMembersAsync(local, remote, cancellationToken);
        await MergeSettingsAsync(local, remote, cancellationToken);

        await local.SaveChangesAsync(cancellationToken);

        try
        {
            File.Delete(tempPath);
        }
        catch
        {
            // ignore
        }

        return new MergeResult(offices, types, members);
    }

    private DbContextOptions<PylaeMasterDbContext> BuildOptions(string path)
    {
        var builder = new DbContextOptionsBuilder<PylaeMasterDbContext>();
        DatabaseConfig.ConfigureMaster(builder, path, _options.EncryptionPassword);
        return builder.Options;
    }

    private static async Task<MergeCounts> MergeOfficesAsync(PylaeMasterDbContext local, PylaeMasterDbContext remote, CancellationToken ct)
    {
        var localOffices = await local.Offices.ToDictionaryAsync(x => x.Id, ct);
        var remoteOffices = await remote.Offices.AsNoTracking().ToListAsync(ct);
        int added = 0, updated = 0, skipped = 0;
        var skippedItems = new List<string>();

        foreach (var remoteOffice in remoteOffices)
        {
            if (localOffices.TryGetValue(remoteOffice.Id, out var existing))
            {
                if (remoteOffice.UpdatedAtUtc > existing.UpdatedAtUtc)
                {
                    CopyOffice(existing, remoteOffice);
                    updated++;
                }
                else
                {
                    skipped++;
                    skippedItems.Add($"{remoteOffice.Code ?? remoteOffice.Id.ToString()}");
                }
            }
            else
            {
                local.Offices.Add(remoteOffice);
                added++;
            }
        }

        return new MergeCounts(added, updated, skipped, skippedItems);
    }

    private static async Task<MergeCounts> MergeMemberTypesAsync(PylaeMasterDbContext local, PylaeMasterDbContext remote, CancellationToken ct)
    {
        var localTypes = await local.MemberTypes.ToDictionaryAsync(x => x.Id, ct);
        var remoteTypes = await remote.MemberTypes.AsNoTracking().ToListAsync(ct);
        int added = 0, updated = 0, skipped = 0;
        var skippedItems = new List<string>();

        foreach (var remoteType in remoteTypes)
        {
            if (localTypes.TryGetValue(remoteType.Id, out var existing))
            {
                if (remoteType.UpdatedAtUtc > existing.UpdatedAtUtc)
                {
                    CopyMemberType(existing, remoteType);
                    updated++;
                }
                else
                {
                    skipped++;
                    skippedItems.Add($"{remoteType.Code ?? remoteType.Id.ToString()}");
                }
            }
            else
            {
                local.MemberTypes.Add(remoteType);
                added++;
            }
        }

        return new MergeCounts(added, updated, skipped, skippedItems);
    }

    private static async Task<MergeCounts> MergeMembersAsync(PylaeMasterDbContext local, PylaeMasterDbContext remote, CancellationToken ct)
    {
        var localMembers = await local.Members.ToDictionaryAsync(x => x.Id, ct);
        var remoteMembers = await remote.Members.AsNoTracking().ToListAsync(ct);
        int added = 0, updated = 0, skipped = 0;
        var skippedItems = new List<string>();

        foreach (var remoteMember in remoteMembers)
        {
            if (localMembers.TryGetValue(remoteMember.Id, out var existing))
            {
                if (remoteMember.UpdatedAtUtc > existing.UpdatedAtUtc)
                {
                    CopyMember(existing, remoteMember);
                    updated++;
                }
                else
                {
                    skipped++;
                    skippedItems.Add($"{remoteMember.MemberNumber}:{remoteMember.LastName}");
                }
            }
            else
            {
                local.Members.Add(remoteMember);
                added++;
            }
        }

        return new MergeCounts(added, updated, skipped, skippedItems);
    }

    private static async Task MergeSettingsAsync(PylaeMasterDbContext local, PylaeMasterDbContext remote, CancellationToken ct)
    {
        var remoteSettings = await remote.Settings.AsNoTracking().ToListAsync(ct);
        foreach (var setting in remoteSettings)
        {
            var existing = await local.Settings.FirstOrDefaultAsync(s => s.Key == setting.Key, ct);
            if (existing is null)
            {
                local.Settings.Add(setting);
            }
            else
            {
                existing.Value = setting.Value;
            }
        }
    }

    private static void CopyOffice(OfficeEntity target, OfficeEntity source)
    {
        target.Code = source.Code;
        target.Name = source.Name;
        target.Phone = source.Phone;
        target.HeadFullName = source.HeadFullName;
        target.HeadBusinessTitle = source.HeadBusinessTitle;
        target.HeadBusinessRank = source.HeadBusinessRank;
        target.Notes = source.Notes;
        target.IsActive = source.IsActive;
        target.DisplayOrder = source.DisplayOrder;
        target.UpdatedAtUtc = source.UpdatedAtUtc;
    }

    private static void CopyMemberType(MemberTypeEntity target, MemberTypeEntity source)
    {
        target.Code = source.Code;
        target.DisplayName = source.DisplayName;
        target.Description = source.Description;
        target.IsActive = source.IsActive;
        target.DisplayOrder = source.DisplayOrder;
        target.UpdatedAtUtc = source.UpdatedAtUtc;
    }

    private static void CopyMember(MemberEntity target, MemberEntity source)
    {
        target.MemberNumber = source.MemberNumber;
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
        target.BusinessRank = source.BusinessRank;
        target.OfficeId = source.OfficeId;
        target.MemberTypeId = source.MemberTypeId;
        target.PersonalIdNumber = source.PersonalIdNumber;
        target.BusinessIdNumber = source.BusinessIdNumber;
        target.IsPermanentStaff = source.IsPermanentStaff;
        target.PhotoFileName = source.PhotoFileName;
        target.BadgeIssueDate = source.BadgeIssueDate;
        target.BadgeExpiryDate = source.BadgeExpiryDate;
        target.DateOfBirth = source.DateOfBirth;
        target.Phone = source.Phone;
        target.Email = source.Email;
        target.Notes = source.Notes;
        target.IsActive = source.IsActive;
        target.UpdatedAtUtc = source.UpdatedAtUtc;
    }
}
