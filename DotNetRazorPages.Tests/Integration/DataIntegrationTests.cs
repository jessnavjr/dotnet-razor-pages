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
    public async Task EmployeeRepository_GetPagedAsync_ReturnsOrderedEmployeesAndTotalCount()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        context.Employees.AddRange(
            new Employee { FirstName = "Emma", LastName = "Reed", Email = "e@x.com", JobTitle = "HR", HireDate = DateTime.UtcNow, IsActive = true },
            new Employee { FirstName = "Noah", LastName = "Mitchell", Email = "n@x.com", JobTitle = "Dev", HireDate = DateTime.UtcNow, IsActive = true },
            new Employee { FirstName = "Ava", LastName = "Carter", Email = "a@x.com", JobTitle = "Mgr", HireDate = DateTime.UtcNow, IsActive = true }
        );
        await context.SaveChangesAsync();

        var repo = new EmployeeRepository(context);

        var result = await repo.GetPagedAsync(1, 2, "name", false);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Carter", result.Items[0].LastName);
        Assert.Equal("Mitchell", result.Items[1].LastName);
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

        Assert.Equal(200, firstCount);
        Assert.Equal(200, secondCount);
    }

    [Fact]
    public async Task ApplicationDbInitializer_TopsUpToMinimum_WhenEmployeesAlreadyExist()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();

        var existingEmployees = Enumerable.Range(1, 25)
            .Select(i => new Employee
            {
                FirstName = $"Existing{i}",
                LastName = "Employee",
                Email = $"existing{i:000}@company.com",
                JobTitle = "Engineer",
                HireDate = new DateTime(2020, 1, 1).AddDays(i),
                IsActive = true
            });

        await context.Employees.AddRangeAsync(existingEmployees);
        await context.SaveChangesAsync();

        var initializer = new ApplicationDbInitializer(context);
        await initializer.InitializeAsync();

        var totalCount = await context.Employees.CountAsync();

        Assert.Equal(200, totalCount);
    }

    [Fact]
    public async Task ApplicationDbInitializer_SeedsWithUniqueFirstAndLastNameCombinations()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using var context = CreateContext(connection);
        var initializer = new ApplicationDbInitializer(context);

        await initializer.InitializeAsync();

        var combinations = await context.Employees
            .Select(e => e.FirstName + "|" + e.LastName)
            .ToListAsync();

        Assert.Equal(combinations.Count, combinations.Distinct(StringComparer.OrdinalIgnoreCase).Count());
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

        var uniqueNameIndex = entity
            .GetIndexes()
            .Single(i => i.Properties.Select(p => p.Name).SequenceEqual([nameof(Employee.FirstName), nameof(Employee.LastName)]));

        Assert.True(uniqueNameIndex.IsUnique);
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