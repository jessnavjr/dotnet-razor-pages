using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Data.Models;

namespace DotNetRazorPages.Data.Abstractions;

public interface IEmployeeRepository
{
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<Employee?> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetAllAsync(
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Employee>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}