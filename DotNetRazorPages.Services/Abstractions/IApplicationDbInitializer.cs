namespace DotNetRazorPages.Services.Abstractions;

public interface IApplicationDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}