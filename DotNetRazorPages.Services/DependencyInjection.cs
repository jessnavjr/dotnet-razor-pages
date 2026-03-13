using DotNetRazorPages.Data;
using DotNetRazorPages.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetRazorPages.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        services.AddDataLayer(connectionString);
        services.AddScoped<IGreetingService, GreetingService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IApplicationDbInitializer, ApplicationDbInitializer>();

        return services;
    }
}