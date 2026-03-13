using DotNetRazorPages.Data;
using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Data.Repositories;
using DotNetRazorPages.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Tests.Integration;

public class DataIntegrationTests
{
    [Fact]
    public async Task EmployeeRepository_GetAllAsync_ReturnsOrderedEmployees()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Employees.AddRange(
            new Employee { FirstName = "Noah", LastName = "Mitchell", Email = "n@x.com", JobTitle = "Dev", HireDate = DateTime.UtcNow, IsActive = true },
            new Employee { FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = DateTime.UtcNow, IsActive = true }
        );
        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Carter", result[0].LastName);
        Assert.Equal("Mitchell", result[1].LastName);
    }

    [Fact]
    public async Task EmployeeRepository_GetByIdAsync_ReturnsEmployeeOrNull()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();

        context.Employees.Add(new Employee { FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = DateTime.UtcNow, IsActive = true });
        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context);
        var existingId = context.Employees.Single().Id;

        var existing = await repo.GetByIdAsync(existingId);
        var missing = await repo.GetByIdAsync(-1);

        Assert.NotNull(existing);
        Assert.Equal("Ava", existing!.FirstName);
        Assert.Null(missing);
    }

    [Fact]
    public async Task ApplicationDbInitializer_SeedsOnce_AndIsIdempotent()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        var initializer = new ApplicationDbInitializer(context);

        await initializer.InitializeAsync();
        var firstCount = await context.Employees.CountAsync();

        await initializer.InitializeAsync();
        var secondCount = await context.Employees.CountAsync();

        Assert.Equal(5, firstCount);
        Assert.Equal(5, secondCount);
    }

    [Fact]
    public async Task ApplicationDbContext_ModelConfiguration_IsApplied()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();

        var entity = context.Model.FindEntityType(typeof(Employee));

        Assert.NotNull(entity);
        Assert.Equal("Employees", entity!.GetTableName());
        Assert.Equal(100, entity.FindProperty(nameof(Employee.FirstName))!.GetMaxLength());
        Assert.Equal(100, entity.FindProperty(nameof(Employee.LastName))!.GetMaxLength());
        Assert.Equal(256, entity.FindProperty(nameof(Employee.Email))!.GetMaxLength());
        Assert.Equal(150, entity.FindProperty(nameof(Employee.JobTitle))!.GetMaxLength());
    }

    [Fact]
    public void GreetingRepository_ReturnsExpectedMessage()
    {
        var repo = new GreetingRepository();

        var greeting = repo.GetGreeting();

        Assert.Equal("Hello from the Data layer.", greeting);
    }

    private static ApplicationDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        return new ApplicationDbContext(options);
    }
}