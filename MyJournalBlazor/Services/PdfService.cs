using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MyJournalBlazor.Models;
using Colors = QuestPDF.Helpers.Colors;

using System.Net;
using System.Text.RegularExpressions;

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

                    // HEADER
                    page.Header()
                        .PaddingBottom(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("My Journal").Bold().FontSize(24).FontColor(Colors.Blue.Darken2);
                                c.Item().Text($"Export Date: {DateTime.Now:MMMM dd, yyyy}")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });

                    // CONTENT
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            foreach (var entry in entries.OrderBy(e => e.Date))
                            {
                                column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                                {
                                    // Title + Date row
                                    c.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text(entry.Title).Bold().FontSize(14);
                                        row.AutoItem().AlignRight().Text($"{entry.Date:MMM dd, yyyy}").FontSize(12);
                                    });

                                    c.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Metadata
                                    c.Item().PaddingTop(5).Text(text =>
                                    {
                                        text.Span("Mood: ").SemiBold();
                                        text.Span($"{entry.Mood}   ");

                                        if (!string.IsNullOrWhiteSpace(entry.SecondaryMoods))
                                            text.Span($"({entry.SecondaryMoods})   ").FontColor(Colors.Grey.Darken1);

                                        if (!string.IsNullOrWhiteSpace(entry.Tags))
                                            text.Span($"Tags: {entry.Tags}").FontColor(Colors.Blue.Medium);
                                    });

                                    // Body (HTML -> readable plain text)
                                    var body = HtmlToPlainText(entry.Content);

                                    // Soft-wrap long “words” so PDF can break lines
                                    body = InsertSoftBreaks(body, 25);

                                    c.Item().PaddingTop(10)
                                        .Text(body)
                                        .LineHeight(1.35f);
                                });

                                column.Item().Height(15);
                            }
                        });

                    // FOOTER
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

        // Converts Quill HTML into readable text with bullets and line breaks.
        private string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "";

            // Decode HTML entities (&nbsp; etc.)
            html = WebUtility.HtmlDecode(html);

            // Normalize line-break tags
            html = html.Replace("<br>", "\n")
                       .Replace("<br/>", "\n")
                       .Replace("<br />", "\n");

            // Block elements -> newline spacing
            html = Regex.Replace(html, @"</(p|div|h1|h2|h3|h4|h5|h6)>", "\n\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(p|div)>", "", RegexOptions.IgnoreCase);

            // Headings: keep content but ensure spacing
            html = Regex.Replace(html, @"<(h1|h2|h3|h4|h5|h6)[^>]*>", "\n", RegexOptions.IgnoreCase);

            // Lists: turn <li> into bullet lines
            html = Regex.Replace(html, @"<li[^>]*>", "\n• ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"</li>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"</(ul|ol)>", "\n\n", RegexOptions.IgnoreCase);

            // Remove all remaining tags
            html = Regex.Replace(html, @"<.*?>", "");

            // Cleanup whitespace
            html = html.Replace("\r", "");
            html = Regex.Replace(html, @"\n{3,}", "\n\n");        // max 2 blank lines
            html = Regex.Replace(html, @"[ \t]{2,}", " ");        // collapse spaces
            html = Regex.Replace(html, @"\n +", "\n");            // trim start of lines

            return html.Trim();
        }

        // Inserts zero-width spaces into long sequences so PDF can wrap them.
        private string InsertSoftBreaks(string text, int maxRun)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return Regex.Replace(text, $@"(\S{{{maxRun}}})(?=\S)", "$1\u200B");
        }
    }
}