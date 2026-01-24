using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MyJournalBlazor.Models;
using Colors = QuestPDF.Helpers.Colors;

namespace MyJournalBlazor.Services
{
    public class PdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePdf(List<JournalEntry> entries)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // --- HEADER ---
                    page.Header()
                        .PaddingBottom(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("My Journal").Bold().FontSize(24).FontColor(Colors.Blue.Darken2);
                                c.Item().Text($"Export Date: {DateTime.Now:MMMM dd, yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });

                    // --- CONTENT ---
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            foreach (var entry in entries)
                            {
                                column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                                {
                                    // Title and Date Row
                                    c.Item().Row(row =>
                                    {
                                        // Title
                                        row.RelativeItem().Text(entry.Title).Bold().FontSize(14);
                                        row.AutoItem().AlignRight().Text($"{entry.Date:MMM dd, yyyy}").FontSize(12);
                                    });

                                    c.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Metadata
                                    c.Item().PaddingTop(5).Text(text =>
                                    {
                                        text.Span("Mood: ").SemiBold();
                                        text.Span($"{entry.Mood}   ");

                                        if (!string.IsNullOrEmpty(entry.SecondaryMoods))
                                            text.Span($"({entry.SecondaryMoods})   ").FontColor(Colors.Grey.Darken1);

                                        if (!string.IsNullOrEmpty(entry.Tags))
                                            text.Span($"Tags: {entry.Tags}").FontColor(Colors.Blue.Medium);
                                    });

                                    // Content Body
                                    c.Item().PaddingTop(10).Text(CleanText(entry.Content));
                                });

                                // Spacer
                                column.Item().Height(15);
                            }
                        });

                    // --- FOOTER ---
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();
        }

        private string CleanText(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var text = html.Replace("<br>", "\n").Replace("<p>", "\n");
            return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", " ").Trim();
        }
    }
}