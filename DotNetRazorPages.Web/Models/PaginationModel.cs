using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace DotNetRazorPages.Web.Models;

public class PaginationModel
{
    public PaginationModel(string pagePath, int defaultPageSize, IReadOnlyList<int> pageSizeOptions)
    {
        PagePath = pagePath;
        PageSizeOptions = pageSizeOptions;
        PageSize = defaultPageSize;
    }

    public string PagePath { get; }
    public IReadOnlyList<int> PageSizeOptions { get; }
    public IReadOnlyDictionary<string, string> AdditionalQueryParameters { get; private set; } =
        new Dictionary<string, string>();
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public void NormalizeRequestedValues(int requestedPageNumber, int requestedPageSize)
    {
        PageNumber = Math.Max(1, requestedPageNumber);
        PageSize = PageSizeOptions.Contains(requestedPageSize) ? requestedPageSize : PageSize;
    }

    public void ApplyTotalCount(int totalCount)
    {
        TotalCount = Math.Max(0, totalCount);

        if (PageNumber > TotalPages)
        {
            PageNumber = TotalPages;
        }
    }

    public void SetAdditionalQueryParameters(IReadOnlyDictionary<string, string>? queryParameters)
    {
        AdditionalQueryParameters = queryParameters ?? new Dictionary<string, string>();
    }

    public string BuildPageUrl(int pageNumber)
    {
        var query = new Dictionary<string, string?>
        {
            ["pageNumber"] = pageNumber.ToString(CultureInfo.InvariantCulture),
            ["pageSize"] = PageSize.ToString(CultureInfo.InvariantCulture)
        };

        foreach (var (key, value) in AdditionalQueryParameters)
        {
            if (!string.Equals(key, "pageNumber", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(key, "pageSize", StringComparison.OrdinalIgnoreCase))
            {
                query[key] = value;
            }
        }

        return QueryHelpers.AddQueryString(PagePath, query);
    }
}