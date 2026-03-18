# DotNet Razor Pages

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
docker rm -f dotnet-razor-sql >/dev/null 2>&1 || true
docker run -d --name dotnet-razor-sql \
  -e ACCEPT_EULA=1 \
  -e MSSQL_SA_PASSWORD='Str0ngPassw0rd!Dev2026' \
  -p 1433:1433 \
  mcr.microsoft.com/azure-sql-edge:latest
```

### Verify SQL container

```bash
docker ps --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'
```

You should see `dotnet-razor-sql` with `0.0.0.0:1433->1433/tcp`.

### Connection string used by the app

Configured in `DotNetRazorPages.Web/appsettings.json` as `ConnectionStrings:DefaultConnection`:

```text
Server=localhost,1433;Database=DotNetRazorPagesDb;User Id=sa;Password=Str0ngPassw0rd!Dev2026;Encrypt=False;TrustServerCertificate=True;
```

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
