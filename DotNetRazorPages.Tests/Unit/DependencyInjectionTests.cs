using DotNetRazorPages.Data;
using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Services;
using DotNetRazorPages.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetRazorPages.Tests.Unit;

public class DependencyInjectionTests
{
    [Fact]
    public void AddDataLayer_Throws_WhenConnectionStringIsMissing()
    {
        var services = new ServiceCollection();

        var ex = Assert.Throws<ArgumentException>(() => services.AddDataLayer("   "));

        Assert.Contains("connection string", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddDataLayer_RegistersRepositories()
    {
        var services = new ServiceCollection();

        var returned = services.AddDataLayer("Server=localhost;Database=TestDb;User Id=sa;Password=Pass@word1;Encrypt=False;");

        Assert.Same(services, returned);
        Assert.Contains(services, d => d.ServiceType == typeof(IGreetingRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(IEmployeeRepository));
        Assert.Contains(services, d => d.ServiceType == typeof(ApplicationDbContext));
    }

    [Fact]
    public void AddApplicationServices_RegistersServiceLayer()
    {
        var services = new ServiceCollection();

        var returned = services.AddApplicationServices("Server=localhost;Database=TestDb;User Id=sa;Password=Pass@word1;Encrypt=False;");

        Assert.Same(services, returned);
        Assert.Contains(services, d => d.ServiceType == typeof(IGreetingService));
        Assert.Contains(services, d => d.ServiceType == typeof(IEmployeeService));
        Assert.Contains(services, d => d.ServiceType == typeof(IApplicationDbInitializer));
    }
}