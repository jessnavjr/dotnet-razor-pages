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
        Assert.Contains("Ava Carter", employeeContent);
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
    public async Task ProductionEnvironment_PrivacyPage_ReturnsSuccess()
    {
        await using var factory = new TestWebApplicationFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Privacy");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Privacy", content);
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
}