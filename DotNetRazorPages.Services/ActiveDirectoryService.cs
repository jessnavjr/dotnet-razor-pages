using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace DotNetRazorPages.Services;

public class ActiveDirectoryService(IOptions<ActiveDirectoryOptions> options) : IActiveDirectoryService
{
    private readonly ActiveDirectoryOptions _options = options.Value;

    public Task<ActiveDirectoryUserResult?> FindUserAsync(string username, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidateConfiguration();
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Active Directory lookup with System.DirectoryServices is supported on Windows only.");
        }

        return Task.FromResult(FindUserInternal(username));
    }

    [SupportedOSPlatform("windows")]
    private ActiveDirectoryUserResult? FindUserInternal(string username)
    {

        var contextOptions = ContextOptions.Negotiate;
        if (_options.UseSecureSocketLayer)
        {
            contextOptions |= ContextOptions.SecureSocketLayer;
        }

        using var context = new PrincipalContext(
            ContextType.Domain,
            _options.Domain,
            _options.Container,
            contextOptions,
            _options.BindUsername,
            _options.BindPassword);

        using var userPrincipal = new UserPrincipal(context)
        {
            SamAccountName = username.Trim()
        };

        using var searcher = new PrincipalSearcher(userPrincipal);
        var principal = searcher.FindOne() as UserPrincipal;

        if (principal is null)
        {
            return null;
        }

        var directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
        var distinguishedName = GetProperty(directoryEntry, "distinguishedName");
        var securityGroups = GetSecurityGroups(principal);
        var enabled = principal.Enabled ?? false;

        var result = new ActiveDirectoryUserResult
        {
            Username = principal.SamAccountName ?? string.Empty,
            UserPrincipalName = principal.UserPrincipalName ?? string.Empty,
            DisplayName = principal.DisplayName ?? string.Empty,
            GivenName = principal.GivenName ?? string.Empty,
            Surname = principal.Surname ?? string.Empty,
            Email = principal.EmailAddress ?? string.Empty,
            DistinguishedName = distinguishedName,
            Enabled = enabled,
            SecurityGroups = securityGroups
        };

        return result;
    }

    [SupportedOSPlatform("windows")]
    private static IReadOnlyList<ActiveDirectoryGroupResult> GetSecurityGroups(UserPrincipal user)
    {
        var groups = new List<ActiveDirectoryGroupResult>();

        foreach (var principal in user.GetAuthorizationGroups())
        {
            if (principal is not GroupPrincipal group)
            {
                continue;
            }

            var directoryEntry = group.GetUnderlyingObject() as DirectoryEntry;
            groups.Add(new ActiveDirectoryGroupResult
            {
                Name = group.SamAccountName ?? group.Name ?? string.Empty,
                DistinguishedName = GetProperty(directoryEntry, "distinguishedName")
            });
        }

        return groups.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.Domain) ||
            string.IsNullOrWhiteSpace(_options.BindUsername) ||
            string.IsNullOrWhiteSpace(_options.BindPassword))
        {
            throw new InvalidOperationException("Active Directory settings are incomplete. Configure ActiveDirectory:Domain, ActiveDirectory:BindUsername, and ActiveDirectory:BindPassword.");
        }
    }

    [SupportedOSPlatform("windows")]
    private static string GetProperty(DirectoryEntry? directoryEntry, string propertyName)
    {
        if (directoryEntry is null || !directoryEntry.Properties.Contains(propertyName))
        {
            return string.Empty;
        }

        return directoryEntry.Properties[propertyName].Value?.ToString() ?? string.Empty;
    }
}
