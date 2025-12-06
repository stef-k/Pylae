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
                        col.Item().Text(member.Office ?? string.Empty).FontSize(10);
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

    public Task<byte[]> RenderBatchBadgesAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default)
    {
        var memberList = members.ToList();

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(15);
                page.Content().Column(col =>
                {
                    // A4 portrait can fit 4 badges (2x2 layout, A6 landscape = half of A5 portrait)
                    // Group members into rows of 2
                    for (int i = 0; i < memberList.Count; i += 2)
                    {
                        col.Item().Row(row =>
                        {
                            // First badge in row
                            row.RelativeItem().Border(0.5f).Padding(5).Element(e =>
                                RenderBadgeContent(e, memberList[i]));

                            // Second badge in row (if exists)
                            if (i + 1 < memberList.Count)
                            {
                                row.RelativeItem().Border(0.5f).Padding(5).Element(e =>
                                    RenderBadgeContent(e, memberList[i + 1]));
                            }
                            else
                            {
                                row.RelativeItem();
                            }
                        });

                        // Add spacing between rows
                        col.Item().Height(10);

                        // Page break after every 4 badges (2 rows)
                        if ((i + 2) % 4 == 0 && i + 2 < memberList.Count)
                        {
                            col.Item().PageBreak();
                        }
                    }
                });
            });
        });

        var stream = new MemoryStream();
        doc.GeneratePdf(stream);
        return Task.FromResult(stream.ToArray());
    }

    private void RenderBadgeContent(IContainer container, Member member)
    {
        var qrBytes = GenerateQr(member.MemberNumber.ToString());

        container.MinHeight(140).Row(row =>
        {
            row.RelativeItem(2).Column(col =>
            {
                col.Item().Text("Pylae").SemiBold().FontSize(14);
                col.Item().Text($"{member.FirstName} {member.LastName}").FontSize(12);
                col.Item().Text($"#{member.MemberNumber}").FontSize(10);
                col.Item().Text(member.MemberType?.DisplayName ?? string.Empty).FontSize(9);
                col.Item().Text(member.Office ?? string.Empty).FontSize(9);
                if (member.BadgeExpiryDate.HasValue)
                {
                    col.Item().Text($"Exp: {member.BadgeExpiryDate:yyyy-MM-dd}").FontSize(8);
                }
            });

            row.RelativeItem(1).AlignMiddle().Image(qrBytes, ImageScaling.FitArea);
        });
    }

    private static byte[] GenerateQr(string payload)
    {
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(data);
        return pngQr.GetGraphic(10);
    }
}
