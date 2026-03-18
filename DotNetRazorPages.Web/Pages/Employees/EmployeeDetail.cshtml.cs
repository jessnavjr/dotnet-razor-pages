using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetRazorPages.Web.Pages;

public class EmployeeDetailModel(IEmployeeService employeeService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsCreateMode => Input.Id == 0;

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
    {
        if (!id.HasValue)
        {
            Input = new InputModel
            {
                HireDate = DateTime.UtcNow.Date,
                IsActive = true
            };

            return Page();
        }

        if (id <= 0)
        {
            return NotFound();
        }

        var employee = await employeeService.GetEmployeeByIdAsync(id.Value, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        Input = MapToInput(employee);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Input.Id = 0;
            return Page();
        }

        var created = await employeeService.CreateEmployeeAsync(MapToDto(Input), cancellationToken);
        SetStatusMessage("Employee created successfully.");
        return RedirectToPage("/Employees/EmployeeDetail", new { id = created.Id });
    }

    public async Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        if (Input.Id <= 0)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updated = await employeeService.UpdateEmployeeAsync(MapToDto(Input), cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        SetStatusMessage("Employee updated successfully.");
        return RedirectToPage("/Employees/EmployeeDetail", new { id = updated.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
    {
        if (Input.Id <= 0)
        {
            return NotFound();
        }

        var deleted = await employeeService.DeleteEmployeeAsync(Input.Id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        SetStatusMessage("Employee deleted successfully.");
        return RedirectToPage("/Employees/Employees");
    }

    private static EmployeeDto MapToDto(InputModel input) => new()
    {
        Id = input.Id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Email = input.Email,
        JobTitle = input.JobTitle,
        HireDate = input.HireDate,
        IsActive = input.IsActive
    };

    private static InputModel MapToInput(EmployeeDto dto) => new()
    {
        Id = dto.Id,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        JobTitle = dto.JobTitle,
        HireDate = dto.HireDate,
        IsActive = dto.IsActive
    };

    public sealed class InputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(256, ErrorMessage = "Email must be 256 characters or fewer.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string JobTitle { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.UtcNow.Date;

        public bool IsActive { get; set; }
    }

    public string? StatusMessage => TempData is null ? null : TempData["StatusMessage"] as string;

    private void SetStatusMessage(string message)
    {
        if (TempData is not null)
        {
            TempData["StatusMessage"] = message;
        }
    }
}
