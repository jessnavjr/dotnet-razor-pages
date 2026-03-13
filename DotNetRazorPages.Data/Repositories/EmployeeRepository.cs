using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Data.Repositories;

public class EmployeeRepository(ApplicationDbContext dbContext) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
}