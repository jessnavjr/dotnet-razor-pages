using System.Diagnostics;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using DotNetRazorPages.Web.Pages;
using Microsoft.AspNetCore.Http;
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
        public Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<EmployeeDto> items =
            [
                new EmployeeDto { Id = 1, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = DateTime.UtcNow, IsActive = true }
            ];

            return Task.FromResult(items);
        }
    }
}
