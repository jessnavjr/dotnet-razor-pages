# DotNetRazorPages.Web

ASP.NET Core Razor Pages web application for the DotNet Razor Pages solution.

## What This Project Contains

- Razor Pages UI and request pipeline
- Cookie authentication and admin authorization policy
- Employees pages backed by the Services and Data layers
- Active Directory lookup page for internal Windows environments
- TypeScript browser behavior under `wwwroot/ts`
- Optional Elastic Stack and Azure Key Vault configuration hooks

## Main Areas

- `Pages/Home` - landing page
- `Pages/Employees` - employee listing and related flows
- `Pages/Auth` - login and access denied pages
- `Pages/Admin` - admin-only pages, including Active Directory lookup
- `wwwroot/ts` - browser-side TypeScript modules and Jest tests
- `Program.cs` - startup, middleware, auth policy, and config validation

## Local Development

### Prerequisites

- .NET SDK 10.x
- Node.js for TypeScript test tooling
- Local SQL Server reachable on `localhost:1433`

### Required Local Secret

The app does not store the real DB connection string in tracked `appsettings*.json` files.

Set the local development connection string with ASP.NET Core User Secrets:

```bash
dotnet user-secrets set --project DotNetRazorPages.Web \
  "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=<local-password>;Encrypt=False;TrustServerCertificate=True;"
```

### Run The App

From the repository root:

```bash
dotnet build DotNetRazorPages.Web/DotNetRazorPages.Web.csproj -c Debug
dotnet run --project DotNetRazorPages.Web --no-build
```

Default local URLs come from `Properties/launchSettings.json`.

## Configuration

Base tracked settings live in:

- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Example.json`

### Secret Sources

Supported secret sources:

1. ASP.NET Core User Secrets for local development
2. Environment variables for deployed environments
3. Azure Key Vault when `AzureKeyVault:Enabled=true`

### Key Settings

- `ConnectionStrings:DefaultConnection` - required
- `Authorization:AdminRoles` - roles allowed to access admin pages
- `ActiveDirectory:Domain` - required if the internal AD page is used
- `ActiveDirectory:Container` - optional LDAP container
- `ElasticStack:*` - optional Elastic configuration
- `AzureKeyVault:*` - optional runtime secret provider configuration

### Active Directory Behavior

The Active Directory feature is configured for internal use and uses the current Windows identity of the app process. No bind username or bind password is required.

## TypeScript Test Tooling

The web project uses Jest for TypeScript tests.

Install dependencies:

```bash
cd DotNetRazorPages.Web
npm install
```

Run tests:

```bash
npm run test:ts
```

Watch mode:

```bash
npm run test:ts:watch
```

CI-style run:

```bash
npm run test:ts:ci
```

## Notes

- Database initialization runs at startup through `IApplicationDbInitializer`
- In non-development environments the app enables HSTS and the exception handler page
- Elastic configuration is validated only when `ElasticStack:Enabled=true`

## Related Docs

- `../README.md` - solution-level local setup
- `../docs/readme-organization.md` - documentation index
- `../docs/06-deployment/deployment-plan-azdo-iis-sql.md` - deployment model
- `../docs/08-reference/admin-role-implementation.md` - admin auth notes