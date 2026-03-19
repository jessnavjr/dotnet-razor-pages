using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using DotNetRazorPages.Services;
using DotNetRazorPages.Services.Abstractions;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
AddAzureKeyVaultIfEnabled(builder);
ValidateSecretBackedConfiguration(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found. Configure it with ASP.NET Core User Secrets for local development or the ConnectionStrings__DefaultConnection environment variable for deployed environments.");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is empty. Configure it with ASP.NET Core User Secrets for local development or the ConnectionStrings__DefaultConnection environment variable for deployed environments.");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddApplicationServices(connectionString, builder.Configuration);

// Add authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
    {
        var adminRoles = builder.Configuration.GetSection("Authorization:AdminRoles").Get<string[]>() ?? ["Admin"];
        policy.RequireAssertion(context =>
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value);

            return userRoles.Any(role => adminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        });
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IApplicationDbInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static void AddAzureKeyVaultIfEnabled(WebApplicationBuilder builder)
{
    var keyVaultSection = builder.Configuration.GetSection("AzureKeyVault");
    if (!keyVaultSection.GetValue<bool>("Enabled"))
    {
        return;
    }

    var vaultUri = keyVaultSection["VaultUri"];
    if (string.IsNullOrWhiteSpace(vaultUri))
    {
        throw new InvalidOperationException(
            "Azure Key Vault is enabled but AzureKeyVault:VaultUri is not configured.");
    }

    builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
}

static void ValidateSecretBackedConfiguration(IConfiguration configuration)
{
    var elasticSection = configuration.GetSection("ElasticStack");
    if (elasticSection.GetValue<bool>("Enabled"))
    {
        var useElasticCloud = elasticSection.GetValue<bool>("UseElasticCloud");
        var apiKey = elasticSection["ApiKey"];
        var username = elasticSection["Username"];
        var password = elasticSection["Password"];

        if (useElasticCloud)
        {
            var cloudId = elasticSection["CloudId"];
            if (string.IsNullOrWhiteSpace(cloudId))
            {
                throw new InvalidOperationException(
                    "ElasticStack is enabled for Elastic Cloud, but ElasticStack:CloudId is not configured.");
            }
        }
        else
        {
            var uris = elasticSection.GetSection("Uris").Get<string[]>();
            if (uris is null || uris.Length == 0 || uris.All(string.IsNullOrWhiteSpace))
            {
                throw new InvalidOperationException(
                    "ElasticStack is enabled, but ElasticStack:Uris does not contain any Elasticsearch endpoints.");
            }
        }

        var hasApiKey = !string.IsNullOrWhiteSpace(apiKey);
        var hasBasicAuth = !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
        if (!hasApiKey && !hasBasicAuth)
        {
            throw new InvalidOperationException(
                "ElasticStack is enabled, but no authentication is configured. Set ElasticStack:ApiKey or ElasticStack:Username and ElasticStack:Password using User Secrets, environment variables, or Azure Key Vault.");
        }
    }

    var activeDirectorySection = configuration.GetSection("ActiveDirectory");
    var bindUsername = activeDirectorySection["BindUsername"];
    var bindPassword = activeDirectorySection["BindPassword"];
    if (!string.IsNullOrWhiteSpace(bindUsername) && string.IsNullOrWhiteSpace(bindPassword))
    {
        throw new InvalidOperationException(
            "ActiveDirectory:BindUsername is configured, but ActiveDirectory:BindPassword is missing. Store both values in User Secrets, environment variables, or Azure Key Vault.");
    }
}

public partial class Program
{
}
