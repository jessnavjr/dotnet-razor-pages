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

        var result = await service.GetEmployeesAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Ava", result[0].FirstName);
        Assert.Equal("Mitchell", result[1].LastName);
        Assert.False(result[1].IsActive);
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

    private sealed class FakeEmployeeRepository(IReadOnlyList<Employee> employees) : IEmployeeRepository
    {
        public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(employees);

        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(employees.FirstOrDefault(e => e.Id == id));
    }
}