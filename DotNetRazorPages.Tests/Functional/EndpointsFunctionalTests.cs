using Microsoft.AspNetCore.Mvc.Testing;

namespace DotNetRazorPages.Tests.Functional;

public class EndpointsFunctionalTests
{
    [Fact]
    public async Task DevelopmentEnvironment_HomeAndEmployeesPages_ReturnSuccessAndContent()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var home = await client.GetAsync("/");
        var employees = await client.GetAsync("/Employees");

        home.EnsureSuccessStatusCode();
        employees.EnsureSuccessStatusCode();

        var homeContent = await home.Content.ReadAsStringAsync();
        var employeeContent = await employees.Content.ReadAsStringAsync();

        Assert.Contains("Hello from functional tests.", homeContent);
        Assert.Contains("Ava", employeeContent);
        Assert.Contains("Carter", employeeContent);
        Assert.Contains("Engineering Manager", employeeContent);
        Assert.Contains("Inactive", employeeContent);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesPage_WhenNoEmployees_ShowsEmptyState()
    {
        await using var factory = new TestWebApplicationFactory("Development", returnEmployees: false);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Employees");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("No employees found.", content);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesPage_RendersSortableHeaderLinks()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Employees?pageNumber=1&pageSize=10&sortBy=name&sortDirection=asc");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("/Employees?pageNumber=1&amp;pageSize=10&amp;sortBy=id&amp;sortDirection=asc", content);
        Assert.Contains("/Employees?pageNumber=1&amp;pageSize=10&amp;sortBy=name&amp;sortDirection=desc", content);
        Assert.Contains("/Employees?pageNumber=1&amp;pageSize=10&amp;sortBy=hiredate&amp;sortDirection=asc", content);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesPage_SearchTerm_FiltersByFirstNameLastNameOrJobTitle()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var byFirstName = await client.GetAsync("/Employees?searchTerm=Ava");
        var byLastName = await client.GetAsync("/Employees?searchTerm=Nguyen");
        var byJobTitle = await client.GetAsync("/Employees?searchTerm=Engineering");

        byFirstName.EnsureSuccessStatusCode();
        byLastName.EnsureSuccessStatusCode();
        byJobTitle.EnsureSuccessStatusCode();

        var firstNameContent = await byFirstName.Content.ReadAsStringAsync();
        var lastNameContent = await byLastName.Content.ReadAsStringAsync();
        var jobTitleContent = await byJobTitle.Content.ReadAsStringAsync();

        Assert.Contains(">Ava</a>", firstNameContent);
        Assert.DoesNotContain(">Noah</a>", firstNameContent);

        Assert.Contains(">Noah</a>", lastNameContent);
        Assert.DoesNotContain(">Ava</a>", lastNameContent);

        Assert.Contains("Engineering Manager", jobTitleContent);
        Assert.DoesNotContain("SRE", jobTitleContent);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesPage_NameSort_ChangesRenderedRowOrder()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var ascendingResponse = await client.GetAsync("/Employees?pageNumber=1&pageSize=10&sortBy=name&sortDirection=asc");
        var descendingResponse = await client.GetAsync("/Employees?pageNumber=1&pageSize=10&sortBy=name&sortDirection=desc");

        ascendingResponse.EnsureSuccessStatusCode();
        descendingResponse.EnsureSuccessStatusCode();

        var ascendingContent = await ascendingResponse.Content.ReadAsStringAsync();
        var descendingContent = await descendingResponse.Content.ReadAsStringAsync();

        var avaAscendingIndex = ascendingContent.IndexOf(">Ava</a>", StringComparison.Ordinal);
        var noahAscendingIndex = ascendingContent.IndexOf(">Noah</a>", StringComparison.Ordinal);
        var avaDescendingIndex = descendingContent.IndexOf(">Ava</a>", StringComparison.Ordinal);
        var noahDescendingIndex = descendingContent.IndexOf(">Noah</a>", StringComparison.Ordinal);

        Assert.True(avaAscendingIndex >= 0 && noahAscendingIndex >= 0);
        Assert.True(avaDescendingIndex >= 0 && noahDescendingIndex >= 0);
        Assert.True(avaAscendingIndex < noahAscendingIndex);
        Assert.True(noahDescendingIndex < avaDescendingIndex);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesExportCsv_ReturnsCsvFileWithAllEmployees()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Employees?handler=ExportCsv&sortBy=name&sortDirection=asc");

        response.EnsureSuccessStatusCode();

        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(response.Content.Headers.ContentDisposition);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Id,FirstName,LastName,Age,Email,JobTitle,HireDate,IsActive", content);
        Assert.Contains("Ava,Carter", content);
        Assert.Contains("Noah,Nguyen", content);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesExportJson_ReturnsJsonFileWithAllEmployees()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Employees?handler=ExportJson&sortBy=name&sortDirection=asc");

        response.EnsureSuccessStatusCode();

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(response.Content.Headers.ContentDisposition);

        var contentDisposition = response.Content.Headers.ContentDisposition;
        var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName;
        Assert.False(string.IsNullOrWhiteSpace(fileName));
        Assert.EndsWith(".json", fileName!.Trim('"'), StringComparison.OrdinalIgnoreCase);

        var content = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("[", content.TrimStart());
        Assert.Contains("\"firstName\": \"Ava\"", content);
        Assert.Contains("\"lastName\": \"Carter\"", content);
        Assert.Contains("\"email\": \"ava.carter@test.local\"", content);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeesPage_ContainsDetailsLink()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Employees");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("/EmployeeDetail/101", content);
    }

    [Fact]
    public async Task DevelopmentEnvironment_EmployeeDetailsPage_ReturnsEmployeeRecord()
    {
        await using var factory = new TestWebApplicationFactory("Development");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/EmployeeDetail/101");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Employee Details", content);
        Assert.Contains("Ava", content);
        Assert.Contains("Carter", content);
        Assert.Contains("Engineering Manager", content);
    }

    [Fact]
    public async Task ProductionEnvironment_PrivacyPage_ReturnsNotFound()
    {
        await using var factory = new TestWebApplicationFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Privacy");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsNotFound()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/not-a-real-route");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ErrorRoute_ReturnsErrorPageAndRequestId()
    {
        await using var factory = new TestWebApplicationFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Error");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("An error occurred while processing your request.", content);
        Assert.Contains("Request ID:", content);
    }

    [Fact]
    public async Task AdminPage_UnauthenticatedAccess_RedirectsToLogin()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Admin");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.True(response.Headers.Location?.ToString().Contains("/Login") ?? false);
    }

    [Fact]
    public async Task LoginPage_DisplaysCorrectly()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Login");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Login", content);
        Assert.Contains("Username", content);
        Assert.Contains("Select Role", content);
        Assert.Contains("Admin", content);
    }

    [Fact]
    public async Task AccessDeniedPage_ReturnsAccessDeniedMessage()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/AccessDenied");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Access Denied", content);
        Assert.Contains("You do not have permission", content);
    }
}