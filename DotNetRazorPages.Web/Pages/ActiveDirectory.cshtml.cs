using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace DotNetRazorPages.Web.Pages;

[Authorize(Policy = "AdminOnly")]
public class ActiveDirectoryModel(IActiveDirectoryService activeDirectoryService) : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    public ActiveDirectoryUserResult? UserResult { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public bool SearchAttempted { get; private set; }

    public async Task OnPostAsync(CancellationToken cancellationToken)
    {
        SearchAttempted = true;

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Please enter a username to search.";
            return;
        }

        try
        {
            UserResult = await activeDirectoryService.FindUserAsync(Username, cancellationToken);

            if (UserResult is null)
            {
                ErrorMessage = "No Active Directory user was found for that username.";
            }
        }
        catch (PrincipalServerDownException ex)
        {
            ErrorMessage = $"Active Directory server is unavailable: {ex.Message}";
        }
        catch (DirectoryServicesCOMException ex)
        {
            ErrorMessage = $"Active Directory query failed: {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (PlatformNotSupportedException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
