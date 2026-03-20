# Documentation Index

This folder contains enterprise-style architecture and requirements documentation for the solution.

## Documents
- [ASP.NET Razor Pages Enterprise Template](asp-net-razor-pages-enterprise-template.md)
- [Deployment Plan (Azure DevOps + IIS + SQL Server)](deployment-plan-azdo-iis-sql.md)
- [Dependency Lifecycle and Upgrade Plan (5-Year)](dependency-lifecycle-and-upgrade-plan.md)
- [Dependency Inventory (Machine-readable JSON)](DEPENDENCY_INVENTORY.json)
- [One-Page Solution Summary](one-page-summary.md)
- [Security Summary and Review](security-summary-and-review.md)
- [Test Plan](test-plan.md)
- [Business Requirements Document](business-requirements.md)
- [Use Cases](use-cases.md)
- [Entity Relationship Diagram](entity-relationship-diagram.md)
- [Cloud Migration Roadmap (Microsoft Standards)](cloud-migration-roadmap-microsoft-standards.md)
- [Requirements Specification](requirements.md)
- [Architecture Overview and Diagrams](architecture.md)
- [Systems Architecture](SYSTEMS_architecture.md)
- [Wireframes](wireframes.md)
- [User Flows](user-flows.md)
- [Data Flow Chart](data-flow-chart.md)

## Intended Use
- Product and business stakeholders: review requirements, scope, and acceptance criteria.
- Engineering and QA: use traceability and architecture sections for implementation and testing alignment.
- Security and operations: review risks, constraints, and operational considerations.

## Deployment Pipeline Templates
- Root pipeline orchestration: [../azure-pipelines.yml](../azure-pipelines.yml)
- Shared build stage template: [../.azuredevops/templates/stage-build.yml](../.azuredevops/templates/stage-build.yml)
- Shared deploy stage template: [../.azuredevops/templates/stage-deploy-iis-sql.yml](../.azuredevops/templates/stage-deploy-iis-sql.yml)
