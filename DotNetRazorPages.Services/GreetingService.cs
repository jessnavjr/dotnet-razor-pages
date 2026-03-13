using DotNetRazorPages.Data.Abstractions;
using DotNetRazorPages.Services.Abstractions;

namespace DotNetRazorPages.Services;

public class GreetingService(IGreetingRepository greetingRepository) : IGreetingService
{
    public string GetGreeting() => $"Hello from the Services layer. {greetingRepository.GetGreeting()}";
}