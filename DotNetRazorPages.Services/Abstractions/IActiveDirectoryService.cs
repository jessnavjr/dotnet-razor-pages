using DotNetRazorPages.Services.Models;

namespace DotNetRazorPages.Services.Abstractions;

public interface IActiveDirectoryService
{
    Task<ActiveDirectoryUserResult?> FindUserAsync(string username, CancellationToken cancellationToken = default);
}
