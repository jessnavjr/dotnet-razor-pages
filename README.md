# DotNet Razor Pages

## Solution Documentation

Enterprise-style documentation is available in the `docs` folder:

- Requirements specification: `docs/REQUIREMENTS.md`
- Architecture and diagrams: `docs/ARCHITECTURE.md`
- Documentation index: `docs/README.md`

## Local SQL Server (Docker)

This project is configured to use a local SQL Server container for development.

### Prerequisites

1. Install container tooling (one-time):

```bash
brew install colima docker
```

2. Start local container runtime:

```bash
colima start --cpu 4 --memory 8 --disk 60
```

### Start SQL Server

Run the local SQL container used by this solution:

```bash
export SQL_SA_PASSWORD='replace-with-a-local-dev-password'

docker rm -f dotnet-razor-sql >/dev/null 2>&1 || true
docker run -d --name dotnet-razor-sql \
  -e ACCEPT_EULA=1 \
  -e MSSQL_SA_PASSWORD="$SQL_SA_PASSWORD" \
  -p 1433:1433 \
  mcr.microsoft.com/azure-sql-edge:latest
```

### Verify SQL container

```bash
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'
```

You should see `dotnet-razor-sql` with `0.0.0.0:1433->1433/tcp`.

### Connection string configuration

Do not store the real connection string in tracked `appsettings*.json` files.

For local development, store it with ASP.NET Core User Secrets:

```bash
dotnet user-secrets set --project DotNetRazorPages.Web \
  "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=$SQL_SA_PASSWORD;Encrypt=False;TrustServerCertificate=True;"
```

For deployed environments, use the `ConnectionStrings__DefaultConnection` environment variable.

Example local development connection string:

```text
Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=<from-user-secrets>;Encrypt=False;TrustServerCertificate=True;
```

### Other secrets

Store Elastic credentials outside tracked configuration as well.

Local development with User Secrets:

```bash
dotnet user-secrets set --project DotNetRazorPages.Web "ElasticStack:Username" "<elastic-username>"
dotnet user-secrets set --project DotNetRazorPages.Web "ElasticStack:Password" "<elastic-password>"
dotnet user-secrets set --project DotNetRazorPages.Web "ElasticStack:ApiKey" "<elastic-api-key>"
dotnet user-secrets set --project DotNetRazorPages.Web "ElasticStack:CloudId" "<elastic-cloud-id>"
```

Equivalent deployed environment variables:

```text
ElasticStack__Username
ElasticStack__Password
ElasticStack__ApiKey
ElasticStack__CloudId
```

The Active Directory feature now uses the current Windows identity for internal directory queries and no longer requires a configured bind username or password.

Optional Azure Key Vault integration:

```bash
dotnet user-secrets set --project DotNetRazorPages.Web "AzureKeyVault:Enabled" "true"
dotnet user-secrets set --project DotNetRazorPages.Web "AzureKeyVault:VaultUri" "https://<your-vault-name>.vault.azure.net/"
```

When Azure Key Vault is enabled, the app uses `DefaultAzureCredential` and loads secrets before application startup validation runs.

Use Azure Key Vault secret names with `--` in place of `:` for hierarchical keys, for example:

```text
ConnectionStrings--DefaultConnection
ElasticStack--ApiKey
ElasticStack--CloudId
ElasticStack--Username
ElasticStack--Password
```

See the checked-in example configuration in `DotNetRazorPages.Web/appsettings.Example.json` for the expected key structure.

### Run the solution

```bash
dotnet build DotNetRazorPages.sln
dotnet run --project DotNetRazorPages.Web --no-build
```

On first run, code-first initialization will:

1. Create `DotNetRazorPagesDb` if it does not exist.
2. Create the `Employees` table.
3. Seed default employee records.

### Stop local SQL Server

```bash
docker stop dotnet-razor-sql
```

### Remove local SQL Server container

```bash
docker rm -f dotnet-razor-sql
```

### Stop container runtime (optional)

```bash
colima stop
```
