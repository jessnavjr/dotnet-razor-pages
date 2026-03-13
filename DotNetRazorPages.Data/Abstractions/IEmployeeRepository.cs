using DotNetRazorPages.Data.Entities;

namespace DotNetRazorPages.Data.Abstractions;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}