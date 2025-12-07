using ClosedXML.Excel;
using System.Globalization;
using System.Text.Json;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Services;

public class ExportService : IExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public Task<byte[]> ExportMembersAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Members");

        // Localized column headers
        ws.Cell(1, 1).Value = Strings.Grid_MemberNumber;
        ws.Cell(1, 2).Value = Strings.Grid_FirstName;
        ws.Cell(1, 3).Value = Strings.Grid_LastName;
        ws.Cell(1, 4).Value = Strings.Grid_BusinessRank;
        ws.Cell(1, 5).Value = Strings.Grid_Office;
        ws.Cell(1, 6).Value = Strings.Grid_MemberType;
        ws.Cell(1, 7).Value = Strings.Grid_IsPermanent;
        ws.Cell(1, 8).Value = Strings.Grid_PersonalIdNumber;
        ws.Cell(1, 9).Value = Strings.Grid_BusinessIdNumber;
        ws.Cell(1, 10).Value = Strings.Grid_DateOfBirth;
        ws.Cell(1, 11).Value = Strings.Grid_BadgeIssueDate;
        ws.Cell(1, 12).Value = Strings.Grid_BadgeExpiryDate;
        ws.Cell(1, 13).Value = Strings.Grid_Phone;
        ws.Cell(1, 14).Value = Strings.Grid_Email;
        ws.Cell(1, 15).Value = Strings.Grid_Notes;
        ws.Cell(1, 16).Value = Strings.Grid_IsActive;
        ws.Cell(1, 17).Value = Strings.Grid_CreatedAt;
        ws.Cell(1, 18).Value = Strings.Grid_UpdatedAt;

        var culture = CultureInfo.CurrentCulture;
        var dateFormat = culture.DateTimeFormat.ShortDatePattern;
        var dateTimeFormat = $"{dateFormat} {culture.DateTimeFormat.ShortTimePattern}";

        var row = 2;
        foreach (var m in members)
        {
            ws.Cell(row, 1).Value = m.MemberNumber;
            ws.Cell(row, 2).Value = m.FirstName;
            ws.Cell(row, 3).Value = m.LastName;
            ws.Cell(row, 4).Value = m.BusinessRank;
            ws.Cell(row, 5).Value = m.Office;
            ws.Cell(row, 6).Value = m.MemberTypeName;
            ws.Cell(row, 7).Value = m.IsPermanentStaff ? Strings.Common_Yes : Strings.Common_No;
            ws.Cell(row, 8).Value = m.PersonalIdNumber;
            ws.Cell(row, 9).Value = m.BusinessIdNumber;
            ws.Cell(row, 10).Value = m.DateOfBirth?.ToString(dateFormat, culture);
            ws.Cell(row, 11).Value = m.BadgeIssueDate?.ToString(dateFormat, culture);
            ws.Cell(row, 12).Value = m.BadgeExpiryDate?.ToString(dateFormat, culture);
            ws.Cell(row, 13).Value = m.Phone;
            ws.Cell(row, 14).Value = m.Email;
            ws.Cell(row, 15).Value = m.Notes;
            ws.Cell(row, 16).Value = m.IsActive ? Strings.Common_Yes : Strings.Common_No;
            ws.Cell(row, 17).Value = m.CreatedAtUtc.ToString(dateTimeFormat, culture);
            ws.Cell(row, 18).Value = m.UpdatedAtUtc?.ToString(dateTimeFormat, culture);
            row++;
        }

        return Task.FromResult(SaveToBytes(workbook));
    }

    public Task<byte[]> ExportVisitsAsync(IEnumerable<Visit> visits, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Visits");

        // Localized column headers
        ws.Cell(1, 1).Value = Strings.Grid_Timestamp;
        ws.Cell(1, 2).Value = Strings.Grid_MemberNumber;
        ws.Cell(1, 3).Value = Strings.Grid_BusinessRank;
        ws.Cell(1, 4).Value = Strings.Grid_FirstName;
        ws.Cell(1, 5).Value = Strings.Grid_LastName;
        ws.Cell(1, 6).Value = Strings.Grid_MemberType;
        ws.Cell(1, 7).Value = Strings.Grid_VisitType;
        ws.Cell(1, 8).Value = Strings.Grid_SiteCode;
        ws.Cell(1, 9).Value = Strings.Grid_LoggedBy;
        ws.Cell(1, 10).Value = Strings.Grid_Notes;

        var culture = CultureInfo.CurrentCulture;
        var dateTimeFormat = $"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}";

        var row = 2;
        foreach (var v in visits)
        {
            ws.Cell(row, 1).Value = v.TimestampLocal.ToString(dateTimeFormat, culture);
            ws.Cell(row, 2).Value = v.MemberNumber;
            ws.Cell(row, 3).Value = v.MemberBusinessRank;
            ws.Cell(row, 4).Value = v.MemberFirstName;
            ws.Cell(row, 5).Value = v.MemberLastName;
            ws.Cell(row, 6).Value = v.MemberTypeName;
            ws.Cell(row, 7).Value = v.Direction == Core.Enums.VisitDirection.Entry ? Strings.Gate_Entry : Strings.Gate_Exit;
            ws.Cell(row, 8).Value = v.SiteCode;
            ws.Cell(row, 9).Value = v.UserDisplayName;
            ws.Cell(row, 10).Value = v.Notes;
            row++;
        }

        return Task.FromResult(SaveToBytes(workbook));
    }

    public Task<byte[]> ExportAuditAsync(IEnumerable<AuditEntry> entries, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Audit");
        ws.Cell(1, 1).Value = "TimestampUtc";
        ws.Cell(1, 2).Value = "ActionType";
        ws.Cell(1, 3).Value = "TargetType";
        ws.Cell(1, 4).Value = "TargetId";
        ws.Cell(1, 5).Value = "User";
        ws.Cell(1, 6).Value = "Details";

        var row = 2;
        foreach (var a in entries)
        {
            ws.Cell(row, 1).Value = a.TimestampUtc;
            ws.Cell(row, 2).Value = a.ActionType;
            ws.Cell(row, 3).Value = a.TargetType;
            ws.Cell(row, 4).Value = a.TargetId;
            ws.Cell(row, 5).Value = a.Username;
            ws.Cell(row, 6).Value = a.DetailsJson;
            row++;
        }

        return Task.FromResult(SaveToBytes(workbook));
    }

    public Task<byte[]> ExportMembersJsonAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(members, JsonOptions));
    }

    public Task<byte[]> ExportVisitsJsonAsync(IEnumerable<Visit> visits, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(visits, JsonOptions));
    }

    public Task<byte[]> ExportAuditJsonAsync(IEnumerable<AuditEntry> entries, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(entries, JsonOptions));
    }

    private static byte[] SaveToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
