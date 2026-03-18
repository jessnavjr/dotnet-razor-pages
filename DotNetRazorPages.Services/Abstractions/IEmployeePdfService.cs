using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services.Abstractions;

public interface IEmployeePdfService
{
    byte[] GenerateEmployeeReport(EmployeeDto employee);
}
