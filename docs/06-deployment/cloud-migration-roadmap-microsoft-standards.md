# Cloud Migration Roadmap (Microsoft Standards)

## Document Control
- Document ID: DRP-CLOUD-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Audience: Engineering, Security, Operations, Architecture, Product

## 1. Purpose
This roadmap defines how to migrate DotNet Razor Pages to Azure using Microsoft-recommended standards and practices.

Primary reference standards:
- Microsoft Cloud Adoption Framework (CAF)
- Azure Well-Architected Framework
- Azure Landing Zone design principles
- Microsoft Security Baselines and Zero Trust principles

## 2. Current-State Summary (Starting Point)
- Runtime: ASP.NET Core Razor Pages (.NET 10)
- Architecture: Layered monolith (Web -> Services -> Data)
- Data store: SQL Server
- Auth model: cookie-based, test-oriented login workflow
- Security gaps (from existing review): plaintext/default credentials, transport hardening needed, test auth not production-ready

## 3. Target-State Architecture (Azure-Aligned)
- Application hosting: Azure App Service (or Azure Container Apps if containerized roadmap is preferred)
- Database: Azure SQL Database (production tier with HA and backups)
- Identity: Microsoft Entra ID (OIDC) for user authentication
- Secrets: Azure Key Vault + Managed Identity
- Observability: Azure Monitor + Application Insights + Log Analytics
- CI/CD: GitHub Actions or Azure DevOps with environment gates
- Governance: Azure Policy, RBAC, Defender for Cloud, budget/cost controls

## 4. Migration Principles
1. Security first: no production workload before identity and secret hardening.
2. Automate everything: infrastructure and deployment via IaC.
3. Incremental delivery: migrate in controlled waves with rollback.
4. Evidence-driven readiness: each phase requires test/security/ops gates.
5. Standards alignment: map decisions to CAF + Well-Architected pillars.

## 5. Phased Roadmap

### Phase 0: Strategy and Governance Foundation (Weeks 1-2)
Objectives:
- Define business case, migration scope, and success metrics.
- Establish cloud governance baseline.

Activities:
- Confirm hosting option (App Service vs Container Apps).
- Define RACI and operating model.
- Create Azure subscription/resource group strategy.
- Align environments (dev/test/prod) and naming/tagging standards.
- Define compliance and data classification requirements.

Deliverables:
- Cloud migration charter
- Environment strategy
- Governance baseline and tagging policy

Exit Criteria:
- Stakeholder sign-off on target architecture and governance model.

### Phase 1: Landing Zone and Platform Setup (Weeks 2-4)
Objectives:
- Build secure Azure foundation before app migration.

Activities:
- Implement landing zone controls (network, RBAC, policy assignments).
- Configure Azure Policy for secure defaults.
- Enable Defender for Cloud recommendations and alerts.
- Set up Key Vault and managed identities.
- Set up Log Analytics workspace and App Insights instance.

Deliverables:
- Landing zone resources
- Policy and RBAC baseline
- Observability foundation

Exit Criteria:
- Platform passes security baseline checklist.

### Phase 2: Application Security and Code Readiness (Weeks 3-6)
Objectives:
- Remove blockers preventing safe production deployment.

Activities:
- Replace test-style login with Microsoft Entra ID integration.
- Remove plaintext credentials from source-controlled config.
- Enforce secure connection settings for database and integrations.
- Add explicit authorization strategy for employee and export routes.
- Sanitize user-facing error messages and strengthen logging.
- Explicitly configure cookie/security headers for production.

Deliverables:
- Production-ready auth/authz implementation
- Secret management integration
- Security remediation evidence

Exit Criteria:
- High-severity security findings closed.

### Phase 3: Infrastructure as Code and Environments (Weeks 5-8)
Objectives:
- Standardize repeatable environment provisioning.

Activities:
- Author IaC (Bicep or Terraform) for all Azure resources.
- Create environment parameter sets (dev/test/prod).
- Add database provisioning/migration automation.
- Define backup, restore, and DR configuration.

Deliverables:
- IaC repo/modules
- Automated environment provisioning pipeline

Exit Criteria:
- Non-prod environments rebuilt from code only.

### Phase 4: CI/CD and DevSecOps Controls (Weeks 6-9)
Objectives:
- Build reliable release process with quality/security gates.

Activities:
- Create CI pipeline stages: build, unit, integration, functional tests.
- Add SAST/dependency scanning and policy checks.
- Add artifact signing and provenance where feasible.
- Create CD pipeline with approvals and deployment strategies (blue/green or staged slots).

Deliverables:
- End-to-end CI/CD pipeline
- Security and quality gates

Exit Criteria:
- Reproducible deployments to non-prod with full gates.

### Phase 5: Data Migration and Cutover Preparation (Weeks 8-10)
Objectives:
- Prepare and validate database migration to Azure SQL.

Activities:
- Select migration method (offline/near-zero downtime).
- Validate schema, indexes, and seed/init behavior in cloud.
- Execute performance baseline and query tuning.
- Rehearse rollback and restore procedures.

Deliverables:
- Migration runbook
- Data validation report
- Cutover readiness checklist

Exit Criteria:
- Dry-run migration passes with accepted RTO/RPO.

### Phase 6: Production Rollout and Hypercare (Weeks 10-12)
Objectives:
- Safely release to production and stabilize operations.

Activities:
- Perform production deployment using approved release process.
- Monitor key SLOs and security signals.
- Run hypercare with rapid incident response.
- Capture lessons learned and backlog improvements.

Deliverables:
- Production deployment report
- Hypercare report
- Post-implementation review

Exit Criteria:
- Stable operations and handoff to BAU support.

## 6. Microsoft Standards Mapping

### Cloud Adoption Framework (CAF)
- Strategy: business outcomes, financial model, stakeholder alignment
- Plan: digital estate rationalization and migration wave planning
- Ready: landing zone, identity, policy, networking, governance
- Adopt: migrate app/data with controlled releases
- Govern: policy, compliance posture, cost controls
- Manage: operations baseline, monitoring, incident and change management

### Well-Architected Pillars
- Reliability: health checks, failover, backups, DR testing
- Security: Entra ID, Key Vault, least privilege RBAC, secure defaults
- Cost Optimization: right-sizing, budgets, reserved capacity where appropriate
- Operational Excellence: IaC, CI/CD, runbooks, observability
- Performance Efficiency: app/database tuning, autoscale strategy

## 7. Workstreams and Ownership
- Architecture: target-state design and standards mapping
- Security: IAM, secrets, policy controls, threat model updates
- Application Engineering: auth refactor, config hardening, feature compatibility
- Data Engineering: Azure SQL migration and validation
- Platform/DevOps: landing zone, IaC, CI/CD, monitoring
- QA: regression and release validation

## 8. Milestones and Gates
1. M1: Cloud governance baseline approved
2. M2: Landing zone and observability ready
3. M3: Security hardening complete (critical findings closed)
4. M4: IaC and CI/CD production-grade
5. M5: Data migration dry-run success
6. M6: Production go-live and hypercare completion

Gate criteria examples:
- No open critical security findings
- Test suite pass threshold met
- Rollback rehearsed and approved
- SLO dashboards and alerts operational

## 9. Risks and Mitigations
- Risk: security debt delays production readiness
  - Mitigation: front-load auth/secrets remediation in Phase 2
- Risk: data migration performance regressions
  - Mitigation: dry-runs, index tuning, rollback runbook
- Risk: operational blind spots post-go-live
  - Mitigation: mandatory observability baseline and hypercare
- Risk: uncontrolled cloud cost growth
  - Mitigation: budgets, tagging, and monthly FinOps review

## 10. KPIs and Success Metrics
- Deployment frequency and lead time
- Change failure rate and MTTR
- Production availability (SLO)
- Security finding closure time
- Test pass rate in CI/CD
- Cloud spend vs budget

## 11. Immediate Next Actions (First 30 Days)
1. Approve target hosting option and architecture decision record.
2. Start Entra ID integration design and remove test auth path from production.
3. Implement Key Vault + managed identity for all sensitive settings.
4. Stand up non-prod landing zone resources via IaC.
5. Add CI quality/security gates for build and test plan compliance.

## 12. Related Documents
- docs/security-summary-and-review.md
- docs/test-plan.md
- docs/SYSTEMS_architecture.md
- docs/architecture.md
- docs/requirements.md
- docs/business-requirements.md
