using DotNetRazorPages.Data;
using DotNetRazorPages.Services.Abstractions;
using DotNetRazorPages.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DotNetRazorPages.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        var emptyConfiguration = new ConfigurationBuilder().Build();
        return services.AddApplicationServices(connectionString, emptyConfiguration);
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        services.AddDataLayer(connectionString);
        services.Configure<ActiveDirectoryOptions>(configuration.GetSection(ActiveDirectoryOptions.SectionName));
        services.AddScoped<IGreetingService, GreetingService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
        services.AddScoped<IApplicationDbInitializer, ApplicationDbInitializer>();

        return services;
    }
}