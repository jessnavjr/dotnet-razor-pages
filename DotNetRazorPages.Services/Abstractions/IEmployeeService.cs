using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services.Abstractions;

public interface IEmployeeService
{
    Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default);

    Task<EmployeeDto?> UpdateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default);

    Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default);

    Task<EmployeeDto?> GetEmployeeByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}