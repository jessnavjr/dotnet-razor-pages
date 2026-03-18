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
        private readonly List<EmployeeDto> _employees =
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

        public Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
        {
            var nextId = _employees.Max(e => e.Id) + 1;
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

            _employees.Add(created);
            return Task.FromResult(created);
        }

        public Task<EmployeeDto?> UpdateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
        {
            var existingIndex = _employees.FindIndex(e => e.Id == employee.Id);
            if (existingIndex < 0)
            {
                return Task.FromResult<EmployeeDto?>(null);
            }

            _employees[existingIndex] = employee;
            return Task.FromResult<EmployeeDto?>(employee);
        }

        public Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == id);
            if (existing is null)
            {
                return Task.FromResult(false);
            }

            _employees.Remove(existing);
            return Task.FromResult(true);
        }

        public Task<EmployeeDto?> GetEmployeeByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (!returnEmployees)
            {
                return Task.FromResult<EmployeeDto?>(null);
            }

            var employee = _employees.FirstOrDefault(e => e.Id == id);

            return Task.FromResult(employee);
        }

        public Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            if (!returnEmployees)
            {
                return Task.FromResult<IReadOnlyList<EmployeeDto>>([]);
            }

            IEnumerable<EmployeeDto> query = _employees;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.JobTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
            {
                query = descending
                    ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
            }

            return Task.FromResult<IReadOnlyList<EmployeeDto>>(query.ToList());
        }

        public Task<PagedResult<EmployeeDto>> GetEmployeesAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            if (!returnEmployees)
            {
                return Task.FromResult(new PagedResult<EmployeeDto>
                {
                    Items = [],
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }

            IEnumerable<EmployeeDto> query = _employees;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.JobTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
            {
                query = descending
                    ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
            }

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = query.Count();

            return Task.FromResult(new PagedResult<EmployeeDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}