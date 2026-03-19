# Documentation Index

This folder contains enterprise-style architecture and requirements documentation for the solution.

## Documents
- [ASP.NET Razor Pages Enterprise Template](ASP_NET_RAZOR_PAGES_ENTERPRISE_TEMPLATE.md)
- [Deployment Plan (Azure DevOps + IIS + SQL Server)](DEPLOYMENT_PLAN_AZDO_IIS_SQL.md)
- [Dependency Lifecycle and Upgrade Plan (5-Year)](DEPENDENCY_LIFECYCLE_AND_UPGRADE_PLAN.md)
- [Dependency Inventory (Machine-readable JSON)](DEPENDENCY_INVENTORY.json)
- [One-Page Solution Summary](ONE_PAGE_SUMMARY.md)
- [Security Summary and Review](SECURITY_SUMMARY_AND_REVIEW.md)
- [Test Plan](TEST_PLAN.md)
- [Business Requirements Document](BUSINESS_REQUIREMENTS.md)
- [Use Cases](USE_CASES.md)
- [Entity Relationship Diagram](ENTITY_RELATIONSHIP_DIAGRAM.md)
- [Cloud Migration Roadmap (Microsoft Standards)](CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md)
- [Requirements Specification](REQUIREMENTS.md)
- [Architecture Overview and Diagrams](ARCHITECTURE.md)
- [Systems Architecture](SYSTEMS_ARCHITECTURE.md)
- [Wireframes](WIREFRAMES.md)
- [User Flows](USER_FLOWS.md)
- [Data Flow Chart](DATA_FLOW_CHART.md)

## Intended Use
- Product and business stakeholders: review requirements, scope, and acceptance criteria.
- Engineering and QA: use traceability and architecture sections for implementation and testing alignment.
- Security and operations: review risks, constraints, and operational considerations.

## Deployment Pipeline Templates
- Root pipeline orchestration: [../azure-pipelines.yml](../azure-pipelines.yml)
- Shared build stage template: [../.azuredevops/templates/stage-build.yml](../.azuredevops/templates/stage-build.yml)
- Shared deploy stage template: [../.azuredevops/templates/stage-deploy-iis-sql.yml](../.azuredevops/templates/stage-deploy-iis-sql.yml)
