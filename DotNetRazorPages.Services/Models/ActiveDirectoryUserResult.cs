namespace DotNetRazorPages.Services.Models;

public sealed class ActiveDirectoryUserResult
{
    public string Username { get; init; } = string.Empty;
    public string UserPrincipalName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string GivenName { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string DistinguishedName { get; init; } = string.Empty;
    public bool Enabled { get; init; }
    public IReadOnlyList<ActiveDirectoryGroupResult> SecurityGroups { get; init; } = [];
}

public sealed class ActiveDirectoryGroupResult
{
    public string Name { get; init; } = string.Empty;
    public string DistinguishedName { get; init; } = string.Empty;
}
