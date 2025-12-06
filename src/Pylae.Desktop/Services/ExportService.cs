using ClosedXML.Excel;
using System.Text.Json;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

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
        ws.Cell(1, 1).Value = "MemberNumber";
        ws.Cell(1, 2).Value = "FirstName";
        ws.Cell(1, 3).Value = "LastName";
        ws.Cell(1, 4).Value = "Office";
        ws.Cell(1, 5).Value = "MemberType";
        ws.Cell(1, 6).Value = "BadgeExpiryDate";

        var row = 2;
        foreach (var m in members)
        {
            ws.Cell(row, 1).Value = m.MemberNumber;
            ws.Cell(row, 2).Value = m.FirstName;
            ws.Cell(row, 3).Value = m.LastName;
            ws.Cell(row, 4).Value = m.Office;
            ws.Cell(row, 5).Value = m.MemberType?.DisplayName;
            ws.Cell(row, 6).Value = m.BadgeExpiryDate?.ToString("yyyy-MM-dd");
            row++;
        }

        return Task.FromResult(SaveToBytes(workbook));
    }

    public Task<byte[]> ExportVisitsAsync(IEnumerable<Visit> visits, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Visits");
        ws.Cell(1, 1).Value = "TimestampUtc";
        ws.Cell(1, 2).Value = "MemberNumber";
        ws.Cell(1, 3).Value = "Name";
        ws.Cell(1, 4).Value = "Direction";
        ws.Cell(1, 5).Value = "SiteCode";
        ws.Cell(1, 6).Value = "User";
        ws.Cell(1, 7).Value = "Notes";

        var row = 2;
        foreach (var v in visits)
        {
            ws.Cell(row, 1).Value = v.TimestampUtc;
            ws.Cell(row, 2).Value = v.MemberNumber;
            ws.Cell(row, 3).Value = $"{v.MemberFirstName} {v.MemberLastName}";
            ws.Cell(row, 4).Value = v.Direction.ToString();
            ws.Cell(row, 5).Value = v.SiteCode;
            ws.Cell(row, 6).Value = v.Username;
            ws.Cell(row, 7).Value = v.Notes;
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
