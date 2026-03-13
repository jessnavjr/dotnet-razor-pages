using DotNetRazorPages.Data;
using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Services;

public class ApplicationDbInitializer(ApplicationDbContext dbContext) : IApplicationDbInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Employees.AnyAsync(cancellationToken))
        {
            return;
        }

        List<Employee> employees =
        [
            new() { FirstName = "Ava", LastName = "Carter", Email = "ava.carter@company.com", JobTitle = "Engineering Manager", HireDate = new DateTime(2020, 3, 15), IsActive = true },
            new() { FirstName = "Noah", LastName = "Mitchell", Email = "noah.mitchell@company.com", JobTitle = "Senior Software Engineer", HireDate = new DateTime(2021, 6, 7), IsActive = true },
            new() { FirstName = "Mia", LastName = "Nguyen", Email = "mia.nguyen@company.com", JobTitle = "Product Designer", HireDate = new DateTime(2022, 2, 1), IsActive = true },
            new() { FirstName = "Liam", LastName = "Patel", Email = "liam.patel@company.com", JobTitle = "Data Analyst", HireDate = new DateTime(2023, 9, 20), IsActive = true },
            new() { FirstName = "Emma", LastName = "Reed", Email = "emma.reed@company.com", JobTitle = "HR Specialist", HireDate = new DateTime(2019, 11, 11), IsActive = true }
        ];

        await dbContext.Employees.AddRangeAsync(employees, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}