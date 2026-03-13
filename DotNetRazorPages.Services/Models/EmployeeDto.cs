namespace DotNetRazorPages.Services.Models;

public class EmployeeDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public DateTime HireDate { get; init; }
    public bool IsActive { get; init; }
}