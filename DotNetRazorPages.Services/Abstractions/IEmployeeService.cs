using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services.Abstractions;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default);
}