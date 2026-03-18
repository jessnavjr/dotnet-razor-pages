using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using DotNetRazorPages.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DotNetRazorPages.Web.Pages;

public class EmployeesModel(IEmployeeService employeeService) : PageModel
{
    private static readonly int[] AllowedPageSizes = [10, 25, 50, 100];
    private static readonly HashSet<string> AllowedSortColumns =
    [
        "id",
        "name",
        "age",
        "email",
        "jobtitle",
        "hiredate",
        "status"
    ];

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "name";

    [BindProperty(SupportsGet = true)]
    public string SortDirection { get; set; } = "asc";

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public IReadOnlyList<EmployeeDto> Employees { get; private set; } = [];
    public PaginationModel Pagination { get; } = new("/Employees", defaultPageSize: 10, pageSizeOptions: AllowedPageSizes);
    public IReadOnlyList<int> PageSizeOptions => Pagination.PageSizeOptions;
    public int TotalCount => Pagination.TotalCount;
    public int TotalPages => Pagination.TotalPages;
    public bool HasPreviousPage => Pagination.HasPreviousPage;
    public bool HasNextPage => Pagination.HasNextPage;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        SortBy = NormalizeSortBy(SortBy);
        SortDirection = NormalizeSortDirection(SortDirection);
        SearchTerm = NormalizeSearchTerm(SearchTerm);

        Pagination.NormalizeRequestedValues(PageNumber, PageSize);
        var queryParameters = new Dictionary<string, string>
        {
            ["sortBy"] = SortBy,
            ["sortDirection"] = SortDirection
        };

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            queryParameters["searchTerm"] = SearchTerm;
        }

        Pagination.SetAdditionalQueryParameters(queryParameters);

        var pagedResult = await employeeService.GetEmployeesAsync(
            Pagination.PageNumber,
            Pagination.PageSize,
            SortBy,
            IsSortDescending(),
            SearchTerm,
            cancellationToken);

        Employees = pagedResult.Items;
        Pagination.ApplyTotalCount(pagedResult.TotalCount);

        PageNumber = Pagination.PageNumber;
        PageSize = Pagination.PageSize;
    }

    public async Task<IActionResult> OnGetExportCsvAsync(CancellationToken cancellationToken)
    {
        SortBy = NormalizeSortBy(SortBy);
        SortDirection = NormalizeSortDirection(SortDirection);
        SearchTerm = NormalizeSearchTerm(SearchTerm);

        var employees = await employeeService.GetAllEmployeesAsync(
            SortBy,
            IsSortDescending(),
            SearchTerm,
            cancellationToken);

        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,FirstName,LastName,Age,Email,JobTitle,HireDate,IsActive");

        foreach (var employee in employees)
        {
            var age = CalculateAge(employee.HireDate);
            csvBuilder
                .Append(employee.Id).Append(',')
                .Append(EscapeCsv(employee.FirstName)).Append(',')
                .Append(EscapeCsv(employee.LastName)).Append(',')
                .Append(age).Append(',')
                .Append(EscapeCsv(employee.Email)).Append(',')
                .Append(EscapeCsv(employee.JobTitle)).Append(',')
                .Append(employee.HireDate.ToString("yyyy-MM-dd")).Append(',')
                .Append(employee.IsActive ? "true" : "false")
                .AppendLine();
        }

        var fileName = $"employees-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(csvBuilder.ToString());

        return File(bytes, "text/csv", fileName);
    }

    public async Task<IActionResult> OnGetExportJsonAsync(CancellationToken cancellationToken)
    {
        SortBy = NormalizeSortBy(SortBy);
        SortDirection = NormalizeSortDirection(SortDirection);
        SearchTerm = NormalizeSearchTerm(SearchTerm);

        var employees = await employeeService.GetAllEmployeesAsync(
            SortBy,
            IsSortDescending(),
            SearchTerm,
            cancellationToken);

        var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        var fileName = $"employees-{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(json);

        return File(bytes, "application/json", fileName);
    }

    public string GetSortLink(string column)
    {
        var normalizedColumn = NormalizeSortBy(column);
        var nextDirection =
            string.Equals(SortBy, normalizedColumn, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(SortDirection, "asc", StringComparison.OrdinalIgnoreCase)
                ? "desc"
                : "asc";

        var query = new Dictionary<string, string?>
        {
            ["pageNumber"] = "1",
            ["pageSize"] = Pagination.PageSize.ToString(),
            ["sortBy"] = normalizedColumn,
            ["sortDirection"] = nextDirection
        };

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query["searchTerm"] = SearchTerm;
        }

        return QueryHelpers.AddQueryString(Pagination.PagePath, query);
    }

    public string GetSortIndicator(string column)
    {
        var normalizedColumn = NormalizeSortBy(column);

        if (!string.Equals(SortBy, normalizedColumn, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return IsSortDescending() ? " ▼" : " ▲";
    }

    public string GetDisplayName(string propertyName)
    {
        var property = typeof(EmployeeDto).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is null)
        {
            return propertyName;
        }

        return property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
    }

    private bool IsSortDescending() =>
        string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeSortBy(string? sortBy)
    {
        var normalized = (sortBy ?? string.Empty).Trim().ToLowerInvariant();
        return AllowedSortColumns.Contains(normalized) ? normalized : "name";
    }

    private static string NormalizeSortDirection(string? direction)
    {
        return string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase)
            ? "desc"
            : "asc";
    }

    private static string? NormalizeSearchTerm(string? searchTerm)
    {
        var normalized = (searchTerm ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private static int CalculateAge(DateTime hireDate)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - hireDate.Year;

        if (today < hireDate.Date.AddYears(age))
        {
            age--;
        }

        return Math.Max(age, 0);
    }
}