# Production Support Runbook

## DotNet Razor Pages

## 1. Purpose

This runbook provides operational procedures for supporting the DotNet Razor Pages solution in production. It is intended for on-call engineers, support engineers, and release managers.

Objectives:
- Restore service quickly during incidents
- Standardize triage and recovery actions
- Reduce mean time to detect and resolve (MTTD/MTTR)
- Provide clear escalation and rollback paths

## 2. System Overview

### Application Components

- Web app: ASP.NET Core Razor Pages (target framework `net10.0`)
- Hosting: IIS on Windows Server
- Data store: SQL Server
- Pipeline: Azure DevOps multi-stage YAML
- Auth: Cookie authentication with role-based admin policy
- PDF exports: QuestPDF

### Main Runtime Behaviors

- Uses `ConnectionStrings:DefaultConnection`
- Runs startup initializer via `IApplicationDbInitializer.InitializeAsync()`
- Exposes key routes through Razor Pages
- Uses IIS app pool env vars for runtime settings in deployment
- Can optionally load secrets from Azure Key Vault when `AzureKeyVault__Enabled=true`

### Primary Routes for Validation

- `/`
- `/Employees`
- `/Auth/Login` or `/Login` (configured login path)
- `/Auth/AccessDenied` or `/AccessDenied`
- `/Admin/Admin`

## 3. Environments and Promotion Path

Pipeline stages:
1. `BuildAndTest`
2. `Deploy_Dev`
3. `Deploy_Test`
4. `Deploy_Prod`

Promotion model:
- Build once, deploy many
- Same artifact (`drop`) promoted through environments

Primary deployment assets:
- `web.zip` for IIS deploy
- optional `sql` folder for `.sql` scripts or `.dacpac`

## 4. Ownership and On-Call Roles

### Roles

- L1 Support: initial triage, gather evidence, execute standard checks
- L2 Application Engineer: app-level troubleshooting, config fixes, restarts, rollback decisions
- L3 Platform/DB Engineer: IIS host issues, SQL incidents, deployment pipeline failures
- Incident Commander (for Sev 1/2): drives timeline, comms, and decision points

### Responsibilities

L1:
- Detect and acknowledge alerts
- Confirm impact and severity
- Execute quick health checks
- Escalate with incident bundle

L2:
- Analyze application logs and deployment changes
- Execute remediation and rollback as needed
- Coordinate with DB/platform teams

L3:
- Resolve infra/network/SQL-level issues
- Assist with advanced recovery (restore/failover)

## 5. Incident Severity Matrix

- Sev 1: Full production outage, critical business impact
- Sev 2: Major feature unavailable or severe degradation
- Sev 3: Partial degradation, workaround exists
- Sev 4: Low impact issue or cosmetic defect

Target response guidance:
- Sev 1: acknowledge within 5 minutes
- Sev 2: acknowledge within 15 minutes
- Sev 3: acknowledge within 30 minutes
- Sev 4: next business cycle

## 6. Golden Signals and Alert Triggers

Monitor these signals:
- Availability: route success rate for `/` and `/Employees`
- Latency: p95 and p99 response times
- Errors: HTTP 5xx rate and unhandled exceptions
- Saturation: IIS worker CPU/memory and SQL resource pressure

Common alert thresholds:
- 5xx error rate > 2% for 5 minutes
- p95 latency > 2s for 10 minutes
- App pool crash/recycle loop
- SQL connectivity failures > threshold

## 7. Access and Tooling Prerequisites

Required access:
- Azure DevOps pipeline and environments
- IIS server (RDP or remote PowerShell)
- SQL Server access for diagnostics
- Application log store access

Useful tools:
- Azure DevOps pipeline logs
- Windows Event Viewer
- IIS Manager and `appcmd`
- SQL client tools (`sqlcmd`)
- Browser and HTTP client (`curl`/PowerShell)

## 8. Quick Triage Checklist (First 10 Minutes)

1. Confirm current impact:
- Which routes fail?
- All users or subset?
- Single environment or multiple?

2. Check latest deployment:
- Was `Deploy_Prod` executed recently?
- Did DB deployment run and succeed?

3. Verify service health:
- Browse `/`
- Browse `/Employees`
- Attempt auth flow `/Login`

4. Verify configuration source:
- If Azure Key Vault is enabled, confirm the app host can authenticate with `DefaultAzureCredential`
- Otherwise confirm IIS app pool environment variables are present for required secrets

5. Check host status:
- IIS site state
- App pool state
- Disk and memory headroom

6. Check database connectivity:
- Connection/login errors in logs
- Basic DB connectivity test

7. Determine severity and escalate if needed

## 9. Standard Operational Commands

Run from host shell/PowerShell as appropriate.

### IIS and App Pool

List sites:

```powershell
Import-Module WebAdministration
Get-Website | Select-Object Name, State, PhysicalPath
```

List app pools:

```powershell
Get-WebAppPoolState -Name "<AppPoolName>"
```

Start app pool:

```powershell
Start-WebAppPool -Name "<AppPoolName>"
```

Recycle app pool:

```powershell
Restart-WebAppPool -Name "<AppPoolName>"
```

Start site:

```powershell
Start-Website -Name "<SiteName>"
```

Stop site:

```powershell
Stop-Website -Name "<SiteName>"
```

Check app pool environment vars:

```powershell
$pool = "<AppPoolName>"
& $env:windir\System32\inetsrv\appcmd.exe list apppool "$pool" /text:*
```

### Route Health Check

```powershell
Invoke-WebRequest -Uri "https://<prod-host>/" -UseBasicParsing
Invoke-WebRequest -Uri "https://<prod-host>/Employees" -UseBasicParsing
Invoke-WebRequest -Uri "https://<prod-host>/Login" -UseBasicParsing
```

### SQL Connectivity Check

```powershell
sqlcmd -S "<SqlServerInstance>" -d "<SqlDatabaseName>" -U "<User>" -P "<Password>" -Q "SELECT 1"
```

## 10. Common Incident Playbooks

## Playbook A: Site Down (HTTP 500 or no response)

Symptoms:
- Home route fails
- IIS returns 500/503 or site unavailable

Actions:
1. Validate IIS site and app pool are running
2. Check latest deployment logs for failed unzip/config
3. Verify app pool env vars:
- `ASPNETCORE_ENVIRONMENT`
- `ConnectionStrings__DefaultConnection` (if used)
 - `AzureKeyVault__Enabled`
 - `AzureKeyVault__VaultUri`
4. Recycle app pool
5. Re-test `/` and `/Employees`
6. If still failing, roll back web content (see rollback section)
7. Escalate Sev 1 if outage persists > 10 minutes

## Playbook B: Database Connectivity Failure

Symptoms:
- Errors indicating SQL login timeout or connection refused
- Employees pages fail while static pages load

Actions:
1. Run SQL connectivity check (`SELECT 1`)
2. Validate connection string configured in IIS app pool environment vars
3. If Azure Key Vault is enabled, validate Key Vault reachability and app host identity permissions
4. Confirm SQL instance and DB name match expected env group values
5. Check SQL server availability and network route from web host
6. If deployment introduced DB change, verify migration/script completion
7. If needed, re-run deploy stage or perform DB rollback/restore per DBA guidance
8. Keep app in degraded mode only if safe; otherwise fail closed with error page

## 10.1 Secret Source Validation

When investigating startup or configuration failures, verify one of these supported secret delivery modes is in effect.

### Mode A: IIS App Pool Environment Variables

Expected environment variables may include:
- `ConnectionStrings__DefaultConnection`
- `ActiveDirectory__BindUsername`
- `ActiveDirectory__BindPassword`
- `ElasticStack__Enabled`
- `ElasticStack__UseElasticCloud`
- `ElasticStack__CloudId`
- `ElasticStack__ApiKey`
- `ElasticStack__Username`
- `ElasticStack__Password`
- `AzureKeyVault__Enabled`
- `AzureKeyVault__VaultUri`

### Mode B: Azure Key Vault Runtime Loading

Expected host requirements:
- `AzureKeyVault__Enabled=true`
- `AzureKeyVault__VaultUri` set correctly
- The IIS host identity or assigned managed identity has `Get` and `List` permissions for required secrets
- Matching secret names exist in Key Vault using `--` separators instead of `:`

Common startup failure patterns:
- Missing `ConnectionStrings--DefaultConnection` in Key Vault
- `AzureKeyVault__VaultUri` typo or wrong vault
- Host identity missing permission to read secrets
- Elastic enabled without `ApiKey` or username/password secret values

## Playbook C: Post-Deployment Regression

Symptoms:
- Incident starts immediately after successful `Deploy_Prod`
- Specific route/feature broken

Actions:
1. Compare previous successful release vs current release
2. Identify if failure is web-only, DB-only, or both
3. Execute smoke tests:
- `/`
- `/Employees`
- auth flow
- exports if applicable
4. For web-only regression: restore from IIS backup folder
5. For DB regression: apply forward fix or execute approved rollback strategy
6. Document exact build number and rollback timestamp

## Playbook D: Authentication/Authorization Failure

Symptoms:
- Login loop or access denied for expected roles
- Admin pages blocked unexpectedly

Actions:
1. Validate cookie auth endpoints and middleware sequence
2. Confirm role configuration (`Authorization:AdminRoles`)
3. Verify claims are present (`ClaimTypes.Role` or `role`)
4. Test non-admin and admin flow separately
5. If config-related, restore last known-good config and recycle app pool

## Playbook E: Performance Degradation

Symptoms:
- Elevated p95 latency
- Timeouts on employees or export pages

Actions:
1. Check IIS worker CPU/memory
2. Check SQL wait/locks and expensive queries
3. Identify hot route(s) by logs
4. Recycle app pool if memory leak suspected
5. Temporarily disable non-critical heavy operations if possible
6. Scale up/out infra if saturation confirmed
7. Open problem record for permanent fix

## 11. Deployment Verification Checklist (After Any Change)

Run after production deploy and after incident recovery:

1. Pipeline
- `BuildAndTest` passed
- `Deploy_Prod` passed

2. Routes
- `/` returns 200
- `/Employees` renders expected data
- `/Login` reachable

3. Security
- Protected admin route requires proper role
- Access denied behavior works

4. Data
- Employees list loads
- CRUD path sanity check (non-destructive where possible)

5. Exports (if enabled)
- CSV/JSON/PDF endpoints respond correctly

6. Logs and metrics
- No sustained 5xx spike
- Latency back to normal range

## 12. Rollback Procedures

## Web Rollback (IIS)

The deployment template moves existing content into:
- `<WebDeployPath>\_backups\<yyyyMMdd-HHmmss>`

Rollback steps:
1. Stop IIS site/app pool
2. Move current deploy files out of `<WebDeployPath>` (excluding `_backups`)
3. Restore desired backup snapshot from `_backups`
4. Start app pool and site
5. Validate key routes
6. Announce rollback completion

## Database Rollback

Preferred strategy:
- Forward-fix for minor issues
- Restore from pre-deploy backup for major integrity issues

Do not run ad hoc destructive SQL in production without DBA approval.

## 13. Escalation Matrix

Escalate immediately when:
- Sev 1 criteria met
- Data integrity concern exists
- Security/auth anomaly indicates possible breach
- No recovery within 30 minutes for Sev 2

Escalation path:
1. L1 -> L2 app engineer
2. L2 -> DBA/platform engineer
3. Incident Commander + engineering manager for Sev 1/2
4. Security lead for auth/data anomaly

## 14. Communications Templates

## Initial Incident Message

- Incident ID: <id>
- Severity: <Sev>
- Start Time: <UTC>
- Impact: <who/what>
- Symptoms: <routes/errors>
- Current Action: <triage/restart/rollback>
- Next Update: <time>

## Recovery Message

- Incident ID: <id>
- Resolved Time: <UTC>
- Root Cause (preliminary): <summary>
- Action Taken: <restart/rollback/fix>
- Validation: <checks passed>
- Follow-ups: <postmortem/tasks>

## 15. Evidence Collection Checklist

Collect and attach:
- Azure DevOps run URL and stage logs
- IIS site/app pool state snapshot
- Relevant app and system log excerpts
- SQL error snippets if DB-related
- Timeline of actions and timestamps
- Recovery validation results

## 16. Known High-Risk Areas

- Startup initializer failures can block app boot
- Incorrect `ConnectionStrings__DefaultConnection` in app pool env vars
- SQL deployment mode mismatch (`SqlScripts` vs `DacPac`)
- Role configuration mismatches affecting admin access
- Post-deploy config drift in IIS bindings/app pool settings

## 17. Preventive Maintenance

Daily:
- Review previous 24h error rate and latency
- Verify backup job success (DB and web host)

Weekly:
- Validate environment variable group correctness
- Confirm deployment pipeline health and agent availability
- Review top exceptions and recurring alerts

Monthly:
- Disaster recovery rehearsal (tabletop or practical)
- Access review for production support permissions
- Dependency and security patch review

## 18. Operational Readiness Checks (Pre-Release)

Before production promotion:
1. All tests pass in CI (`dotnet test` and TypeScript tests if changed)
2. Deployment approval gates confirmed
3. Rollback point verified:
- IIS backup path available
- DB rollback strategy defined
4. On-call coverage confirmed for release window
5. Smoke test plan prepared

## 19. Appendix: Reference Commands

Build and test locally:

```bash
dotnet build DotNetRazorPages.sln -c Debug
dotnet test DotNetRazorPages.sln -c Debug
```

Run web app locally:

```bash
dotnet run --project DotNetRazorPages.Web --no-build
```

Run TypeScript tests (web project):

```bash
cd DotNetRazorPages.Web
npm run test:ts
```

## 20. Related Documents

- [Deployment Plan](DEPLOYMENT_PLAN_AZDO_IIS_SQL.md)
- [Cloud Migration Roadmap](CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md)
- [Test Plan](../05-testing/TEST_PLAN.md)
- [Security Summary](../08-reference/SECURITY_SUMMARY_AND_REVIEW.md)
- [Systems Architecture](../01-architecture/SYSTEMS_ARCHITECTURE.md)

## 21. Revision History

| Version | Date | Author | Summary |
|---|---|---|---|
| 1.0 | 2026-03-18 | Engineering | Initial production support runbook |
