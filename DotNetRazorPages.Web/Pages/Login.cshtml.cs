using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DotNetRazorPages.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? SelectedRole { get; set; }

    public string[]? AvailableRoles { get; private set; }
    
    public bool ShowLogoutSuccess { get; set; }

    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnGet()
    {
        // Check if this was a logout redirect
        ShowLogoutSuccess = !string.IsNullOrEmpty(Request.Query["loggedout"]);
        
        // Get available roles from configuration
        try
        {
            AvailableRoles = _configuration.GetSection("Authorization:AdminRoles").Get<string[]>() ?? ["Admin", "User"];
        }
        catch
        {
            AvailableRoles = ["Admin", "User"];
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Username))
        {
            ModelState.AddModelError("", "Username is required.");
            OnGet();
            return Page();
        }

        try
        {
            // Get allowed admin roles from configuration
            var adminRoles = _configuration.GetSection("Authorization:AdminRoles").Get<string[]>() ?? ["Admin"];
            var selectedRole = SelectedRole ?? _configuration.GetValue<string>("Authorization:TestUserRole") ?? "User";

            // Create claims for the user
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, Username),
                new(ClaimTypes.Role, selectedRole)
            };

            // If user is in an admin role, add additional claims
            if (adminRoles.Contains(selectedRole))
            {
                claims.Add(new("admin", "true"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An error occurred during login. Please try again.");
            OnGet();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login", new { loggedout = true });
        }
        catch (Exception)
        {
            // Even if there's an error, clear the session and redirect
            return RedirectToPage("/Index");
        }
    }
}
