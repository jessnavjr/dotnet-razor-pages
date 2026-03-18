using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
    {
        var created = await employeeRepository.AddAsync(MapToEntity(employee), cancellationToken);
        return MapToDto(created);
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(EmployeeDto employee, CancellationToken cancellationToken = default)
    {
        var updated = await employeeRepository.UpdateAsync(MapToEntity(employee), cancellationToken);
        return updated is null ? null : MapToDto(updated);
    }

    public Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
        => employeeRepository.DeleteAsync(id, cancellationToken);

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : MapToDto(employee);
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllEmployeesAsync(
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(sortBy, descending, searchTerm, cancellationToken);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool descending,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var pagedEmployees = await employeeRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            sortBy,
            descending,
            searchTerm,
            cancellationToken);

        var items = pagedEmployees.Items
            .Select(MapToDto)
            .ToList();

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = pagedEmployees.TotalCount,
            PageNumber = pagedEmployees.PageNumber,
            PageSize = pagedEmployees.PageSize
        };
    }

    private static EmployeeDto MapToDto(Data.Entities.Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        JobTitle = e.JobTitle,
        HireDate = e.HireDate,
        IsActive = e.IsActive
    };

    private static Data.Entities.Employee MapToEntity(EmployeeDto dto) => new()
    {
        Id = dto.Id,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        JobTitle = dto.JobTitle,
        HireDate = dto.HireDate,
        IsActive = dto.IsActive
    };
}