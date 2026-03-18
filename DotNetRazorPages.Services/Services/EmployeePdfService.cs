using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DotNetRazorPages.Services.Services;

public class EmployeePdfService : IEmployeePdfService
{
    public byte[] GenerateEmployeeReport(EmployeeDto employee)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.5f, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontFamily("Segoe UI").FontColor("#1f2552"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, employee));
                page.Footer().Element(content => ComposeFooter(content, employee));
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container
            .BorderBottom(2)
            .BorderColor("#4f46e5")
            .PaddingBottom(12)
            .Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item()
                        .Text(".NETRazorPages")
                        .FontSize(14)
                        .Bold()
                        .FontColor("#4f46e5");
                    col.Item()
                        .Text("Employee Record")
                        .FontSize(8)
                        .FontColor("#5f6792")
                        .LetterSpacing(0.07f);
                });

                row.AutoItem()
                    .AlignRight()
                    .AlignBottom()
                    .Text($"Generated: {DateTime.UtcNow:MMMM d, yyyy}")
                    .FontSize(8)
                    .FontColor("#5f6792");
            });
    }

    private static void ComposeContent(IContainer container, EmployeeDto employee)
    {
        container.Column(col =>
        {
            // Hero section
            col.Item()
                .PaddingTop(24)
                .PaddingBottom(16)
                .Row(row =>
                {
                    row.RelativeItem().Column(nameCol =>
                    {
                        nameCol.Item()
                            .Text($"{employee.FirstName} {employee.LastName}")
                            .FontSize(26)
                            .Bold()
                            .FontColor("#1f2552");

                        nameCol.Item()
                            .PaddingTop(4)
                            .Text(employee.JobTitle)
                            .FontSize(12)
                            .FontColor("#5f6792");

                        nameCol.Item()
                            .PaddingTop(4)
                            .Text($"Employee #{employee.Id}")
                            .FontSize(9)
                            .FontColor("#9ca3af");
                    });

                    row.AutoItem()
                        .AlignBottom()
                        .Element(badge => ComposeBadge(badge, employee.IsActive));
                });

            // Divider
            col.Item()
                .BorderBottom(1)
                .BorderColor("#dde2ea")
                .PaddingBottom(20);

            // Two-column field layout
            col.Item()
                .PaddingTop(24)
                .Column(fields =>
                {
                    fields.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingRight(24).Element(c => AddField(c, "First Name", employee.FirstName));
                        row.RelativeItem().Element(c => AddField(c, "Last Name", employee.LastName));
                    });
                    fields.Item().PaddingTop(20).Element(c => AddField(c, "Email Address", employee.Email));
                    fields.Item().PaddingTop(20).Element(c => AddField(c, "Job Title", employee.JobTitle));
                    fields.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem().PaddingRight(24).Element(c => AddField(c, "Hire Date", employee.HireDate.ToString("MMMM d, yyyy")));
                        row.RelativeItem().Element(c => AddField(c, "Employment Status", employee.IsActive ? "Active" : "Inactive"));
                    });
                });
        });
    }

    private static void AddField(IContainer container, string label, string value)
    {
        container.Column(col =>
        {
            col.Item()
                .Text(label.ToUpperInvariant())
                .FontSize(7)
                .FontColor("#5f6792")
                .LetterSpacing(0.08f);

            col.Item()
                .PaddingTop(4)
                .PaddingBottom(8)
                .BorderBottom(1)
                .BorderColor("#e5e7eb")
                .Text(value)
                .FontSize(11)
                .Bold()
                .FontColor("#1f2552");
        });
    }

    private static void ComposeBadge(IContainer container, bool isActive)
    {
        var bgColor = isActive ? "#dcfce7" : "#fee2e2";
        var textColor = isActive ? "#166534" : "#991b1b";
        var label = isActive ? "\u25cf Active" : "\u25cf Inactive";

        container
            .Background(bgColor)
            .Border(1)
            .BorderColor(isActive ? "#bbf7d0" : "#fecaca")
            .PaddingVertical(5)
            .PaddingHorizontal(12)
            .Text(label)
            .FontSize(9)
            .Bold()
            .FontColor(textColor);
    }

    private static void ComposeFooter(IContainer container, EmployeeDto employee)
    {
        container
            .BorderTop(1)
            .BorderColor("#dde2ea")
            .PaddingTop(8)
            .Row(row =>
            {
                row.RelativeItem()
                    .Text("Confidential \u2013 Internal Use Only")
                    .FontSize(8)
                    .FontColor("#9ca3af");

                row.AutoItem()
                    .Text($"Employee ID: {employee.Id}")
                    .FontSize(8)
                    .FontColor("#9ca3af");
            });
    }
}
