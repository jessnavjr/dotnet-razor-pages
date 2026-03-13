using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default) =>
        (await employeeRepository.GetAllAsync(cancellationToken))
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                JobTitle = e.JobTitle,
                HireDate = e.HireDate,
                IsActive = e.IsActive
            })
            .ToList();
}