# Dependency Lifecycle and Upgrade Plan (5-Year)

## Document Control
- Document ID: DRP-UPG-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Owners: Engineering Lead, Operations Lead, Security Lead

## 1. Purpose
This document defines a 5-year upgrade plan for platform and runtime dependencies used by the solution, with a focus on avoiding end-of-life (EOL) risk and maintaining Microsoft-aligned supportability.

## 2. Scope
In scope:
- .NET runtime and SDK
- ASP.NET Core and EF Core major/minor alignment
- SQL Server engine version
- Windows Server host/runtime version (where applicable)
- Node/TypeScript/Vitest toolchain for frontend tests
- Critical NuGet and test framework dependencies

Out of scope:
- Non-production developer workstation tooling preferences
- One-off experimental dependencies

## 3. Current Dependency Baseline (From Repository)
- Target framework: `net10.0` (Web, Services, Data, Tests)
- EF Core SQL Server: `Microsoft.EntityFrameworkCore.SqlServer 10.0.5`
- ASP.NET Core test host: `Microsoft.AspNetCore.Mvc.Testing 10.0.5`
- PDF library: `QuestPDF 2026.2.3`
- AD libraries:
  - `System.DirectoryServices 10.0.0`
  - `System.DirectoryServices.AccountManagement 10.0.0`
- Test stack:
  - `Microsoft.NET.Test.Sdk 17.14.1`
  - `xunit 2.9.3`
  - `xunit.runner.visualstudio 3.1.4`
  - `coverlet.collector 6.0.4`
  - `coverlet.msbuild 8.0.0`
- Frontend dev dependencies:
  - `typescript 5.8.2`
  - `vitest 3.1.1`
  - `jsdom 26.1.0`
- Local SQL container image currently referenced in README: `mcr.microsoft.com/azure-sql-edge:latest`

Machine-readable companion inventory:
- `docs/DEPENDENCY_INVENTORY.json`
- Used by CI lifecycle policy step in `.github/workflows/ci.yml`

## 4. Lifecycle Policy (Standards)
1. Do not run production workloads on dependencies in End-of-Support state.
2. For Microsoft runtimes/platforms, complete upgrades at least 6 months before EOS.
3. Prefer LTS channels for server-side runtime/platform dependencies.
4. Keep framework families aligned by major version (for example .NET/EF Core/ASP.NET test packages).
5. Enforce monthly dependency review and quarterly lifecycle audit.
6. Pin production-critical container tags (no `latest`) and define upgrade cadence.

## 5. 5-Year Upgrade Horizon (2026-2031)

Important:
- Dates below are planning targets and must be verified quarterly against official vendor lifecycle announcements.

| Dependency Family | Current Baseline | 2026-2027 Plan | 2028-2029 Plan | 2030-2031 Plan | Risk if Not Upgraded |
|---|---|---|---|---|---|
| .NET / ASP.NET Core | .NET 10 | Move to next LTS track before .NET 10 EOS window | Adopt next LTS | Adopt next LTS | Security patch gaps, unsupported runtime |
| EF Core | 10.0.x | Stay aligned with app runtime major | Upgrade with runtime major | Upgrade with runtime major | Provider/runtime incompatibility |
| SQL Server Engine | Local SQL edge/dev SQL + SQL Server | Standardize supported SQL Server/Azure SQL target; remove ambiguous image tags | Upgrade to supported SQL major before EOS milestones | Revalidate HA/DR + engine support | Data platform support/compliance risk |
| Windows Server (if hosting on Windows) | TBD by environment | Inventory host OS versions; eliminate near-EOS images | Upgrade pre-EOS per Microsoft lifecycle | Continue N-1 supported baseline | OS security and patching exposure |
| Node + TS test toolchain | TS 5.8 / Vitest 3.x / jsdom 26 | Keep within actively supported Node LTS and package majors | Perform annual major review | Perform annual major review | Build/test instability, supply-chain risk |
| NuGet test/security tooling | Mixed versions | Quarterly package updates (minor/patch) | Annual major version uplift with compatibility pass | Continue quarterly cadence | CVE exposure and CI drift |

## 6. Target Upgrade Cadence

### Monthly
- Run dependency update scan (NuGet + npm).
- Triage CVEs and transitive vulnerabilities.
- Apply safe patch updates.

### Quarterly
- Perform lifecycle review against official Microsoft and package support pages.
- Review all runtime/platform EOL dates and update this plan.
- Execute one non-prod upgrade rehearsal (if major upgrade is upcoming within 12 months).

### Annual
- Complete one planned major platform review:
  - .NET LTS alignment decision
  - SQL platform support validation
  - Windows Server support validation
  - CI/toolchain major updates

## 7. Roadmap by Phase

### Phase A: Inventory and Policy Enforcement (0-30 days)
- Create authoritative dependency inventory in CI artifact.
- Add policy check for unsupported runtimes/dependencies.
- Pin all container images to explicit versions/tags.

### Phase B: Runtime and Data Platform Stabilization (30-90 days)
- Confirm production target for SQL platform (SQL Server version or Azure SQL tier).
- Define .NET LTS migration target and cutover date.
- Implement pre-production upgrade rehearsal environment.

### Phase C: Automation and Governance (90-180 days)
- Add automated dependency PR tooling (Dependabot/Renovate).
- Add CI gate for known critical vulnerabilities.
- Add release checklist item for lifecycle compliance.

### Phase D: Continuous Lifecycle Operations (180 days onward)
- Operate monthly/quarterly cadence.
- Track lifecycle KPIs and remediation SLAs.
- Keep this document current with actual version state.

## 8. Dependency-Specific Upgrade Guidance

### 8.1 .NET and ASP.NET Core
- Prefer LTS runtime for production.
- Keep SDK and runtime compatibility matrix documented.
- Validate breaking changes in preview branches before adoption windows.

### 8.2 EF Core and Data Access
- Upgrade EF Core in lockstep with .NET major where feasible.
- Run integration tests against representative SQL version before production rollout.
- Validate query plans for paging/sorting and export-heavy scenarios.

### 8.3 SQL Server
- Define supported production SQL version(s) and publish a compatibility matrix.
- Validate backup/restore and migration scripts during every major version upgrade.
- Avoid floating image tags in dev/test examples to reduce drift.

### 8.4 Windows Server (Host OS)
- Maintain a host OS inventory (environment, version, EOS date, owner).
- Enforce patch baseline and maintenance windows.
- Schedule OS version upgrades at least 12 months before EOS.

### 8.5 Frontend Tooling (Node/TypeScript/Vitest)
- Standardize supported Node LTS version in CI.
- Review major upgrades annually; apply minor/patch quarterly.
- Keep test snapshots and DOM behavior checks stable across jsdom/vitest upgrades.

## 9. Governance and Responsibilities
- Engineering Lead:
  - Own runtime and package alignment decisions
  - Approve major upgrade cutovers
- Operations Lead:
  - Own SQL/OS supportability and maintenance windows
  - Own host lifecycle compliance
- Security Lead:
  - Own vulnerability triage SLAs and risk acceptance workflow
- QA Lead:
  - Own regression validation plan for major upgrades

## 10. Risk Matrix

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Runtime reaches EOS before migration | Medium | High | 12-month forecast + enforced gate |
| SQL version drift across environments | Medium | High | Version pinning + environment matrix |
| OS lifecycle not tracked | Medium | High | CMDB-style inventory + quarterly review |
| Breaking changes in major upgrades | High | Medium | Staged rehearsals + regression test suites |
| Vulnerability backlog grows | Medium | High | Monthly triage SLA + auto-update tooling |

## 11. KPIs
- % dependencies in supported state (target: 100%)
- Mean time to remediate critical dependency CVEs
- Number of environments with pinned runtime/DB versions
- Major upgrade rehearsal success rate
- Upgrade lead time before official EOS

## 12. Immediate Actions for This Repository
1. Decide and document production SQL platform/version standard.
2. Replace floating SQL container image tag in README with pinned version.
3. Define target LTS migration window after current .NET 10 baseline.
4. Add dependency automation (Dependabot/Renovate) for NuGet and npm.
5. Add lifecycle compliance checkpoint to release process.

## 13. Review Schedule
- Next review date: 2026-06-30
- Review cadence: Quarterly
- Change trigger events:
  - New major .NET release
  - Microsoft lifecycle bulletin changes
  - SQL/OS support policy changes
  - Critical CVE affecting stack dependencies

## 14. Related Documents
- `docs/CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md`
- `docs/SECURITY_SUMMARY_AND_REVIEW.md`
- `docs/TEST_PLAN.md`
- `docs/SYSTEMS_ARCHITECTURE.md`
- `docs/ASP_NET_RAZOR_PAGES_ENTERPRISE_TEMPLATE.md`
