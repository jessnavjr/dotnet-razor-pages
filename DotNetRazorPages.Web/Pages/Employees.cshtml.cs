using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetRazorPages.Web.Pages;

public class EmployeesModel(IEmployeeService employeeService) : PageModel
{

    public IReadOnlyList<EmployeeDto> Employees { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Employees = await employeeService.GetEmployeesAsync(cancellationToken);
    }
}