using System.Diagnostics;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using DotNetRazorPages.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetRazorPages.Tests.Unit;

public class PageModelTests
{
    [Fact]
    public void IndexModel_OnGet_SetsGreetingMessage()
    {
        var model = new IndexModel(new FakeGreetingService());

        model.OnGet();

        Assert.Equal("hello-test", model.GreetingMessage);
    }

    [Fact]
    public async Task EmployeesModel_OnGetAsync_LoadsEmployees()
    {
        var model = new EmployeesModel(new FakeEmployeeService());

        await model.OnGetAsync(CancellationToken.None);

        Assert.Single(model.Employees);
        Assert.Equal("Ava", model.Employees[0].FirstName);
        Assert.Equal(1, model.PageNumber);
        Assert.Equal(2, model.TotalCount);
        Assert.Equal(1, model.TotalPages);
        Assert.False(model.HasNextPage);
    }

    [Fact]
    public async Task EmployeesModel_OnGetExportCsvAsync_ReturnsCsvWithAllRows()
    {
        var model = new EmployeesModel(new FakeEmployeeService())
        {
            SortBy = "name",
            SortDirection = "asc"
        };

        var result = await model.OnGetExportCsvAsync(CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Contains("Id,FirstName,LastName,Age,Email,JobTitle,HireDate,IsActive", content);
        Assert.Contains("Ava,Carter", content);
        Assert.Contains("Noah,Mitchell", content);
    }

    [Fact]
    public async Task EmployeeDetailModel_OnGetAsync_LoadsEmployee_WhenFound()
    {
        var model = new EmployeeDetailModel(new FakeEmployeeService());

        var result = await model.OnGetAsync(1, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.Equal(1, model.Input.Id);
        Assert.Equal("Ava", model.Input.FirstName);
    }

    [Fact]
    public async Task EmployeeDetailModel_OnGetAsync_ReturnsNotFound_WhenMissing()
    {
        var model = new EmployeeDetailModel(new FakeEmployeeService());

        var result = await model.OnGetAsync(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal(0, model.Input.Id);
    }

    [Fact]
    public async Task EmployeeDetailModel_OnGetAsync_WithoutId_InitializesCreateMode()
    {
        var model = new EmployeeDetailModel(new FakeEmployeeService());

        var result = await model.OnGetAsync(null, CancellationToken.None);

        Assert.IsType<PageResult>(result);
        Assert.True(model.IsCreateMode);
        Assert.True(model.Input.IsActive);
    }

    [Fact]
    public async Task EmployeeDetailModel_OnPostCreateAsync_CreatesAndRedirects()
    {
        var model = new EmployeeDetailModel(new FakeEmployeeService())
        {
            Input = new EmployeeDetailModel.InputModel
            {
                FirstName = "Mia",
                LastName = "Stone",
                Email = "mia.stone@test.local",
                JobTitle = "QA Engineer",
                HireDate = DateTime.UtcNow.Date,
                IsActive = true
            }
        };

        var result = await model.OnPostCreateAsync(CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/EmployeeDetail", redirect.PageName);
    }

    [Fact]
    public async Task EmployeeDetailModel_OnPostDeleteAsync_DeletesAndRedirects()
    {
        var model = new EmployeeDetailModel(new FakeEmployeeService())
        {
            Input = new EmployeeDetailModel.InputModel
            {
                Id = 1,
                FirstName = "Ava",
                LastName = "Carter",
                Email = "a@x.com",
                JobTitle = "Mgr",
                HireDate = DateTime.UtcNow.Date,
                IsActive = true
            }
        };

        var result = await model.OnPostDeleteAsync(CancellationToken.None);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Employees", redirect.PageName);
    }

    [Fact]
    public void PrivacyModel_OnGet_DoesNotThrow()
    {
        var model = new PrivacyModel();

        model.OnGet();
    }

    [Fact]
    public void ErrorModel_OnGet_UsesCurrentActivityId_WhenAvailable()
    {
        using var activity = new Activity("test");
        activity.Start();

        var model = new ErrorModel
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext { TraceIdentifier = "trace-id" }
            }
        };

        model.OnGet();

        Assert.NotNull(model.RequestId);
        Assert.True(model.ShowRequestId);
        Assert.Equal(activity.Id, model.RequestId);
    }

    [Fact]
    public void ErrorModel_OnGet_UsesTraceIdentifier_WhenNoActivity()
    {
        Activity.Current = null;
        var httpContext = new DefaultHttpContext { TraceIdentifier = "trace-id-fallback" };

        var model = new ErrorModel
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        model.OnGet();

        Assert.Equal("trace-id-fallback", model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ErrorModel_ShowRequestId_IsFalse_WhenRequestIdMissing()
    {
        var model = new ErrorModel { RequestId = null };

        Assert.False(model.ShowRequestId);
    }

    private sealed class FakeGreetingService : IGreetingService
    {
        public string GetGreeting() => "hello-test";
    }

    private sealed class FakeEmployeeService : IEmployeeService
    {
        private readonly Dictionary<int, EmployeeDto> _employees = new()
        {
            [1] = new EmployeeDto
            {
                Id = 1,
                FirstName = "Ava",
                LastName = "Carter",
                Email = "a@x.com",
                JobTitle = "Mgr",
                HireDate = DateTime.UtcNow.AddYears(-3),
                IsActive = true
            },
            [2] = new EmployeeDto
            {
                Id = 2,
                FirstName = "Noah",
                LastName = "Mitchell",
                Email = "n@x.com",
                JobTitle = "Dev",
                HireDate = DateTime.UtcNow.AddYears(-2),
                IsActive = false
            }
        };

        public Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
        {
            var nextId = _employees.Keys.Max() + 1;
            var created = new EmployeeDto
            {
                Id = nextId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                JobTitle = employee.JobTitle,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive
            };

            _employees[nextId] = created;
            return Task.FromResult(created);
        }

        public Task<EmployeeDto?> UpdateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
        {
            if (!_employees.ContainsKey(employee.Id))
            {
                return Task.FromResult<EmployeeDto?>(null);
            }

            _employees[employee.Id] = employee;
            return Task.FromResult<EmployeeDto?>(employee);
        }

        public Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_employees.Remove(id));

        public Task<EmployeeDto?> GetEmployeeByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            _employees.TryGetValue(id, out var employee);
            return Task.FromResult(employee);
        }

        public Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<EmployeeDto>>(_employees.Values.ToList());
        }

        public Task<PagedResult<EmployeeDto>> GetEmployeesAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<EmployeeDto> items =
            [
                new EmployeeDto { Id = 1, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = DateTime.UtcNow, IsActive = true }
            ];

            return Task.FromResult(new PagedResult<EmployeeDto>
            {
                Items = items,
                TotalCount = 2,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
