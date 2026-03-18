using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Data.Repositories;

public class EmployeeRepository(ApplicationDbContext dbContext) : IEmployeeRepository
{
    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employee.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.Email = employee.Email;
        existing.JobTitle = employee.JobTitle;
        existing.HireDate = employee.HireDate;
        existing.IsActive = employee.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Employees.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = ApplySearch(dbContext.Employees.AsNoTracking(), searchTerm);
        var orderedQuery = ApplyOrdering(baseQuery, sortBy, descending);

        return await orderedQuery.ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Employee>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        var baseQuery = ApplySearch(dbContext.Employees.AsNoTracking(), searchTerm);

        var orderedQuery = ApplyOrdering(baseQuery, sortBy, descending);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var items = await orderedQuery
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Employee>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = safePageNumber,
            PageSize = safePageSize
        };
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    private static IQueryable<Employee> ApplyOrdering(IQueryable<Employee> baseQuery, string sortBy, bool descending)
    {
        var sortKey = (sortBy ?? string.Empty).Trim().ToLowerInvariant();

        return (sortKey, descending) switch
        {
            ("id", false) => baseQuery.OrderBy(e => e.Id),
            ("id", true) => baseQuery.OrderByDescending(e => e.Id),

            ("email", false) => baseQuery.OrderBy(e => e.Email).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("email", true) => baseQuery.OrderByDescending(e => e.Email).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),

            ("jobtitle", false) => baseQuery.OrderBy(e => e.JobTitle).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("jobtitle", true) => baseQuery.OrderByDescending(e => e.JobTitle).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),

            ("hiredate", false) => baseQuery.OrderBy(e => e.HireDate).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("hiredate", true) => baseQuery.OrderByDescending(e => e.HireDate).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),

            // Age is derived from HireDate on the web page, so use HireDate as the sort source.
            ("age", false) => baseQuery.OrderBy(e => e.HireDate).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("age", true) => baseQuery.OrderByDescending(e => e.HireDate).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),

            ("status", false) => baseQuery.OrderBy(e => e.IsActive).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("status", true) => baseQuery.OrderByDescending(e => e.IsActive).ThenBy(e => e.LastName).ThenBy(e => e.FirstName),

            ("name", true) => baseQuery.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName),
            _ => baseQuery.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
        };
    }

    private static IQueryable<Employee> ApplySearch(IQueryable<Employee> baseQuery, string? searchTerm)
    {
        var normalizedSearch = (searchTerm ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedSearch))
        {
            return baseQuery;
        }

        return baseQuery.Where(e =>
            EF.Functions.Like(e.FirstName, $"%{normalizedSearch}%") ||
            EF.Functions.Like(e.LastName, $"%{normalizedSearch}%") ||
            EF.Functions.Like(e.JobTitle, $"%{normalizedSearch}%"));
    }
}