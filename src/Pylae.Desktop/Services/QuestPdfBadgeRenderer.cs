using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Pylae.Desktop.Services;

public class QuestPdfBadgeRenderer : IBadgeRenderer
{
    public Task<byte[]> RenderBadgeAsync(Member member, CancellationToken cancellationToken = default)
    {
        var qrBytes = GenerateQr(member.MemberNumber.ToString());

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6.Landscape());
                page.Margin(10);
                page.Content().Row(row =>
                {
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().Text("Pylae").SemiBold().FontSize(16);
                        col.Item().Text($"{member.FirstName} {member.LastName}").FontSize(14);
                        col.Item().Text($"#{member.MemberNumber}").FontSize(12);
                        col.Item().Text(member.MemberType?.DisplayName ?? string.Empty).FontSize(10);
                        col.Item().Text(member.Office?.Name ?? string.Empty).FontSize(10);
                        if (member.BadgeExpiryDate.HasValue)
                        {
                            col.Item().Text($"Expires: {member.BadgeExpiryDate:yyyy-MM-dd}").FontSize(9);
                        }
                    });

                    row.RelativeItem(1).Column(col =>
                    {
                        col.Item().Image(qrBytes, ImageScaling.FitArea);
                    });
                });
            });
        });

        var stream = new MemoryStream();
        doc.GeneratePdf(stream);
        return Task.FromResult(stream.ToArray());
    }

    private static byte[] GenerateQr(string payload)
    {
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(data);
        return pngQr.GetGraphic(10);
    }
}
