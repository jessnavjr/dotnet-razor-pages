# Deployment Plan: Azure DevOps YAML to IIS and SQL Server (Dev, Test, Prod)

## 1. Objective
Deploy the DotNet Razor Pages solution through Azure DevOps multi-stage YAML pipelines to:
- Dev
- Test
- Prod

Hosting model:
- Web tier: Windows Server with IIS
- Data tier: Microsoft SQL Server

## 2. Deployment Model
- Build once, deploy many:
  - Build, test, and publish artifacts once in CI.
  - Promote the same artifact to Dev, then Test, then Prod.
- Stage-based release gates:
  - Environment approvals and checks managed in Azure DevOps Environments.
- Secrets management:
  - SQL credentials and connection strings provided by Azure DevOps variable groups (secret variables).

## 3. Pipeline Files
- Root Azure DevOps pipeline YAML: [azure-pipelines.yml](../azure-pipelines.yml)
- Reusable build stage template: [.azuredevops/templates/stage-build.yml](../.azuredevops/templates/stage-build.yml)
- Reusable deploy stage template (IIS + SQL): [.azuredevops/templates/stage-deploy-iis-sql.yml](../.azuredevops/templates/stage-deploy-iis-sql.yml)

Template ownership guidance:
- Edit stage ordering, trigger settings, and environment promotion flow in [azure-pipelines.yml](../azure-pipelines.yml).
- Edit shared build/test/publish logic once in [.azuredevops/templates/stage-build.yml](../.azuredevops/templates/stage-build.yml).
- Edit shared database and IIS deployment logic once in [.azuredevops/templates/stage-deploy-iis-sql.yml](../.azuredevops/templates/stage-deploy-iis-sql.yml).

## 4. Prerequisites

### 4.1 Azure DevOps
- Project with pipeline permissions enabled
- Environments created:
  - `drp-dev`
  - `drp-test`
  - `drp-prod`
- VM resources registered in each environment (Windows deployment targets)
- Approvals/checks configured on Test/Prod environments

### 4.2 Agent and Target Server Requirements
- Build agent:
  - Microsoft-hosted or self-hosted Windows agent
  - .NET SDK 10.x installed (or installed by task)
- Target IIS servers:
  - IIS role installed
  - WebAdministration PowerShell module available
  - SQL deployment tooling installed:
    - `sqlcmd` and/or
    - `SqlPackage.exe` (if DACPAC mode is used)

### 4.3 SQL Server Requirements
- SQL login or integrated principal with schema deployment rights
- Target database created or creation rights provided
- Network connectivity from target server to SQL Server

## 5. Variable Strategy
Use one variable group per environment:
- `drp-dev`
- `drp-test`
- `drp-prod`

Recommended variables:
- Web/IIS:
  - `IisSiteName`
  - `IisAppPoolName`
  - `WebDeployPath`
  - `IisBindingProtocol` (example: `http` or `https`)
  - `IisBindingPort` (example: `80` or `443`)
  - `IisBindingHost` (optional host header)
  - `AspNetCoreEnvironment` (Development, Test, Production)
- DB:
  - `DbDeployMode` (`SqlScripts` or `DacPac`)
  - `SqlServerInstance`
  - `SqlDatabaseName`
  - `SqlDeployUsername` (secret)
  - `SqlDeployPassword` (secret)
- Optional app setting injection:
  - `ConnectionStrings__DefaultConnection` (secret)

## 6. Artifact Contents
Build stage publishes one artifact named `drop` containing:
- `web.zip` (published web app package)
- optional SQL assets from `database` folder:
  - `deploy.dacpac` and/or
  - one or more `.sql` scripts

## 7. Stage Flow
1. BuildAndTest
- Restore, build, test
- Publish web app
- Package web app as `web.zip`
- Publish pipeline artifact

2. Deploy_Dev
- Deploy database changes
- Deploy IIS web content
- Start site and app pool

3. Deploy_Test
- Same as Dev, using Test variable group and environment checks

4. Deploy_Prod
- Same as Test, using Prod variables and stricter approvals

## 8. Deployment Steps (Per Environment)

### 8.1 Database Deployment
- If `DbDeployMode = DacPac` and a dacpac exists:
  - Run `SqlPackage.exe /Action:Publish`
- Else:
  - Execute all `.sql` files in sorted order via `sqlcmd`

### 8.2 IIS Web Deployment
- Ensure deployment path exists
- Backup current deployment directory before overwrite
- Extract `web.zip` to deployment path
- Create or update app pool and site
- Optionally set app pool environment variables:
  - `ASPNETCORE_ENVIRONMENT`
  - `ConnectionStrings__DefaultConnection`
- Start app pool and site

## 9. Rollback Plan
- Web rollback:
  - Restore from latest backup folder under `WebDeployPath\_backups`
  - Restart app pool/site
- Database rollback:
  - Prefer forward-fix scripts for minor issues
  - Use pre-deployment backup restore for major failures
- Process:
  - Stop pipeline progression after failed validation
  - Execute rollback runbook per environment

## 10. Post-Deployment Validation
Minimum checks after each stage:
- Site responds with successful health/basic request
- Key routes load:
  - `/`
  - `/Employees`
- Authentication flow is reachable:
  - `/Login`
- DB connectivity verified by page load and query-backed screen
- Export smoke checks:
  - CSV/JSON/PDF route checks as applicable

## 11. Security Controls
- Keep all credentials in secret variables
- Avoid hardcoding connection strings in pipeline YAML
- Restrict variable group access by environment
- Use environment approvals for Test and Prod
- Run deployment identity with least privilege

## 12. Operational Recommendations
- Add deployment logs retention and artifact retention policies
- Add synthetic smoke tests post-deploy in each stage
- Add tagging/annotation of deployed build numbers
- Add change ticket reference enforcement for Prod stage

## 13. How to Add a New Environment Stage (Short Guide)
1. Create a new Azure DevOps Environment, for example `drp-uat`, and register its deployment target VM(s).
2. Create a matching variable group, for example `drp-uat`, with the same keys used by Dev/Test/Prod.
3. Add a new stage template reference in [azure-pipelines.yml](../azure-pipelines.yml) after the stage it should depend on.

Example stage entry:

    - template: .azuredevops/templates/stage-deploy-iis-sql.yml
      parameters:
        stageName: Deploy_UAT
        displayName: Deploy to UAT
        dependsOn: Deploy_Test
        variableGroup: drp-uat
        environmentName: drp-uat
        deploymentName: DeployUat
        deploymentDisplayName: Deploy UAT (IIS + SQL)
        artifactName: $(ArtifactName)

4. Add approvals/checks to the new environment as needed.
5. Run the pipeline and verify routing, authentication, and DB-backed pages in that environment.

## 14. Future Improvements
- Move to declarative database migrations pipeline with drift detection
- Add blue/green or IIS slot-like cutover strategy
- Integrate App Insights deployment annotations
