using System.ComponentModel.DataAnnotations;

namespace DotNetRazorPages.Services.Models;

public class EmployeeDto
{
    [Display(Name = "Employee ID")]
    public int Id { get; init; }

    [Display(Name = "First Name")]
    public string FirstName { get; init; } = string.Empty;

    [Display(Name = "Last Name")]
    public string LastName { get; init; } = string.Empty;

    [Display(Name = "Email Address")]
    public string Email { get; init; } = string.Empty;

    [Display(Name = "Job Title")]
    public string JobTitle { get; init; } = string.Empty;

    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; init; }

    [Display(Name = "Active Status")]
    public bool IsActive { get; init; }
}