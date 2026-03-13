using DotNetRazorPages.Data.Abstractions;

namespace DotNetRazorPages.Data.Repositories;

public class GreetingRepository : IGreetingRepository
{
    public string GetGreeting() => "Hello from the Data layer.";
}