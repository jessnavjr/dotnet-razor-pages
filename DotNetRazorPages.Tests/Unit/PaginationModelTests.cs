using DotNetRazorPages.Web.Models;

namespace DotNetRazorPages.Tests.Unit;

public class PaginationModelTests
{
    [Fact]
    public void NormalizeRequestedValues_UsesDefaultPageSize_WhenRequestedSizeNotAllowed()
    {
        var model = new PaginationModel("/Employees", defaultPageSize: 5, pageSizeOptions: [5, 10, 25]);

        model.NormalizeRequestedValues(requestedPageNumber: -10, requestedPageSize: 3);

        Assert.Equal(1, model.PageNumber);
        Assert.Equal(5, model.PageSize);
    }

    [Fact]
    public void NormalizeRequestedValues_AcceptsAllowedPageSize()
    {
        var model = new PaginationModel("/Employees", defaultPageSize: 5, pageSizeOptions: [5, 10, 25]);

        model.NormalizeRequestedValues(requestedPageNumber: 2, requestedPageSize: 10);

        Assert.Equal(2, model.PageNumber);
        Assert.Equal(10, model.PageSize);
    }

    [Fact]
    public void ApplyTotalCount_ComputesTotalPages_AndClampsPageNumber()
    {
        var model = new PaginationModel("/Employees", defaultPageSize: 5, pageSizeOptions: [5, 10, 25]);
        model.NormalizeRequestedValues(requestedPageNumber: 10, requestedPageSize: 5);

        model.ApplyTotalCount(totalCount: 12);

        Assert.Equal(12, model.TotalCount);
        Assert.Equal(3, model.TotalPages);
        Assert.Equal(3, model.PageNumber);
        Assert.True(model.HasPreviousPage);
        Assert.False(model.HasNextPage);
    }

    [Fact]
    public void BuildPageUrl_IncludesPageNumberAndPageSize()
    {
        var model = new PaginationModel("/Employees", defaultPageSize: 5, pageSizeOptions: [5, 10, 25]);
        model.NormalizeRequestedValues(requestedPageNumber: 1, requestedPageSize: 10);

        var url = model.BuildPageUrl(2);

        Assert.Contains("/Employees", url);
        Assert.Contains("pageNumber=2", url);
        Assert.Contains("pageSize=10", url);
    }
}
