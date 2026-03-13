using DotNetRazorPages.Data;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetRazorPages.Tests.Functional;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=Str0ngPassw0rd!Dev2026;Encrypt=False;TrustServerCertificate=True;";

    private readonly string _environment;
    private readonly bool _returnEmployees;

    public TestWebApplicationFactory(string environment = "Development", bool returnEmployees = true)
    {
        _environment = environment;
        _returnEmployees = returnEmployees;
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", TestConnectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environment);
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=Str0ngPassw0rd!Dev2026;Encrypt=False;TrustServerCertificate=True;"
            };

            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IApplicationDbInitializer>();
            services.RemoveAll<IGreetingService>();
            services.RemoveAll<IEmployeeService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase($"functional-tests-{Guid.NewGuid()}"));

            services.AddScoped<IApplicationDbInitializer, NoOpInitializer>();
            services.AddScoped<IGreetingService, FakeGreetingService>();
            services.AddScoped<IEmployeeService>(_ => new FakeEmployeeService(_returnEmployees));
        });
    }

    private sealed class NoOpInitializer : IApplicationDbInitializer
    {
        public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeGreetingService : IGreetingService
    {
        public string GetGreeting() => "Hello from functional tests.";
    }

    private sealed class FakeEmployeeService(bool returnEmployees) : IEmployeeService
    {
        public Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        {
            if (!returnEmployees)
            {
                return Task.FromResult<IReadOnlyList<EmployeeDto>>([]);
            }

            IReadOnlyList<EmployeeDto> employees =
            [
                new EmployeeDto
                {
                    Id = 101,
                    FirstName = "Ava",
                    LastName = "Carter",
                    Email = "ava.carter@test.local",
                    JobTitle = "Engineering Manager",
                    HireDate = new DateTime(2020, 3, 15),
                    IsActive = true
                },
                new EmployeeDto
                {
                    Id = 102,
                    FirstName = "Noah",
                    LastName = "Nguyen",
                    Email = "noah.nguyen@test.local",
                    JobTitle = "SRE",
                    HireDate = new DateTime(2021, 7, 8),
                    IsActive = false
                }
            ];

            return Task.FromResult(employees);
        }
    }
}