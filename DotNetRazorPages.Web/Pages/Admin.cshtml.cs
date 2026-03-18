using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DotNetRazorPages.Web.Pages;

[Authorize(Policy = "AdminOnly")]
public class AdminModel : PageModel
{
    private readonly IConfiguration _configuration;

    public string[]? AllowedRoles { get; private set; }
    public string? CurrentUser { get; private set; }
    public IEnumerable<string>? UserRoles { get; private set; }
    public bool IsAuthorizedAdmin { get; private set; }

    public AdminModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnGet()
    {
        // Get the configured admin roles
        var allowedRoles = _configuration.GetSection("Authorization:AdminRoles").Get<string[]>() ?? ["Admin"];
        AllowedRoles = allowedRoles;
        
        // Get current user information
        CurrentUser = User?.Identity?.Name ?? "Unknown";

        var principal = User ?? new ClaimsPrincipal();
        
        // Read standard role claims first, then legacy custom role claims.
        var roleClaims = principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Concat(principal.FindAll("role").Select(c => c.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        UserRoles = roleClaims;
        
        // Check if user has at least one admin role
        IsAuthorizedAdmin = roleClaims.Any(role => allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }
}
