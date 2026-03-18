using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Services;
using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Tests.Unit;

public class ServiceTests
{
    [Fact]
    public void GreetingService_ComposesMessage()
    {
        var service = new GreetingService(new FakeGreetingRepository("Hi from repo."));

        var result = service.GetGreeting();

        Assert.Equal("Hello from the Services layer. Hi from repo.", result);
    }

    [Fact]
    public async Task EmployeeService_MapsEntitiesToDtos()
    {
        var entities = new List<Employee>
        {
            new() { Id = 1, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = new DateTime(2020, 1, 1), IsActive = true },
            new() { Id = 2, FirstName = "Noah", LastName = "Mitchell", Email = "n@x.com", JobTitle = "Dev", HireDate = new DateTime(2021, 1, 1), IsActive = false }
        };
        var service = new EmployeeService(new FakeEmployeeRepository(entities));

        var result = await service.GetEmployeesAsync(1, 10, "name", false);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Ava", result.Items[0].FirstName);
        Assert.Equal("Mitchell", result.Items[1].LastName);
        Assert.False(result.Items[1].IsActive);
    }

    [Fact]
    public async Task EmployeeService_GetAllEmployeesAsync_MapsAllEntitiesToDtos()
    {
        var entities = new List<Employee>
        {
            new() { Id = 1, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = new DateTime(2020, 1, 1), IsActive = true },
            new() { Id = 2, FirstName = "Noah", LastName = "Mitchell", Email = "n@x.com", JobTitle = "Dev", HireDate = new DateTime(2021, 1, 1), IsActive = false }
        };
        var service = new EmployeeService(new FakeEmployeeRepository(entities));

        var result = await service.GetAllEmployeesAsync("name", false);

        Assert.Equal(2, result.Count);
        Assert.Equal("Ava", result[0].FirstName);
        Assert.Equal("Mitchell", result[1].LastName);
    }

    [Fact]
    public async Task EmployeeService_GetEmployeeByIdAsync_ReturnsMappedDto()
    {
        var entities = new List<Employee>
        {
            new() { Id = 10, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = new DateTime(2020, 1, 1), IsActive = true }
        };
        var service = new EmployeeService(new FakeEmployeeRepository(entities));

        var result = await service.GetEmployeeByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal("Ava", result!.FirstName);
    }

    [Fact]
    public async Task EmployeeService_CreateUpdateDelete_WorksAcrossRepository()
    {
        var entities = new List<Employee>
        {
            new() { Id = 1, FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = new DateTime(2020, 1, 1), IsActive = true }
        };
        var service = new EmployeeService(new FakeEmployeeRepository(entities));

        var created = await service.CreateEmployeeAsync(new EmployeeDto
        {
            FirstName = "Noah",
            LastName = "Mitchell",
            Email = "n@x.com",
            JobTitle = "Dev",
            HireDate = new DateTime(2021, 1, 1),
            IsActive = false
        });

        var updated = await service.UpdateEmployeeAsync(new EmployeeDto
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Email = created.Email,
            JobTitle = "Senior Dev",
            HireDate = created.HireDate,
            IsActive = true
        });

        var deleted = await service.DeleteEmployeeAsync(created.Id);

        Assert.True(created.Id > 0);
        Assert.NotNull(updated);
        Assert.Equal("Senior Dev", updated!.JobTitle);
        Assert.True(deleted);
    }

    [Fact]
    public void EmployeeDto_DefaultValues_AreInitialized()
    {
        var dto = new EmployeeDto();

        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.JobTitle);
    }

    [Fact]
    public void Employee_DefaultValues_AreInitialized()
    {
        var employee = new Employee();

        Assert.Equal(string.Empty, employee.FirstName);
        Assert.Equal(string.Empty, employee.LastName);
        Assert.Equal(string.Empty, employee.Email);
        Assert.Equal(string.Empty, employee.JobTitle);
    }

    private sealed class FakeGreetingRepository(string message) : IGreetingRepository
    {
        public string GetGreeting() => message;
    }

    private sealed class FakeEmployeeRepository(IReadOnlyList<Employee> seedEmployees) : IEmployeeRepository
    {
        private readonly List<Employee> _employees = seedEmployees.ToList();

        public Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            var nextId = _employees.Count == 0 ? 1 : _employees.Max(e => e.Id) + 1;
            employee.Id = nextId;
            _employees.Add(employee);
            return Task.FromResult(employee);
        }

        public Task<Employee?> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existing is null)
            {
                return Task.FromResult<Employee?>(null);
            }

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.JobTitle = employee.JobTitle;
            existing.HireDate = employee.HireDate;
            existing.IsActive = employee.IsActive;

            return Task.FromResult<Employee?>(existing);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == id);
            if (existing is null)
            {
                return Task.FromResult(false);
            }

            _employees.Remove(existing);
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<Employee>> GetAllAsync(
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Employee> query = _employees;

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

            return Task.FromResult<IReadOnlyList<Employee>>(query.ToList());
        }

        public Task<DotNetRazorPages.Data.Models.PagedResult<Employee>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string sortBy,
            bool descending,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Employee> query = _employees;

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

            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult(new DotNetRazorPages.Data.Models.PagedResult<Employee>
            {
                Items = items,
                TotalCount = _employees.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_employees.FirstOrDefault(e => e.Id == id));
    }
}