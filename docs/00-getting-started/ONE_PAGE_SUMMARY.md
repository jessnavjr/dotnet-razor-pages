# DotNet Razor Pages - One-Page Solution Summary

## Purpose
DotNet Razor Pages is an internal employee management application built on .NET 10. It supports employee lifecycle workflows (create, read, update, delete), secure role-based administration, and export/reporting features for operational use.

## Business Value
- Centralized employee record management in a simple web UI
- Faster operations via search, sorting, and pagination
- Data portability through CSV, JSON, and PDF exports
- Role-based admin controls for sensitive functionality

## Scope (Current)
In scope:
- Employee list and detail workflows
- Employee CRUD operations
- List exports (CSV/JSON)
- Detail export (PDF via QuestPDF)
- Admin and Active Directory pages with role checks
- Local SQL-backed startup initialization and seed data

Out of scope:
- Public APIs
- Self-service external user onboarding
- Production enterprise identity provider integration

## Architecture at a Glance
- Web: ASP.NET Core Razor Pages (`DotNetRazorPages.Web`)
- Services: business logic and integration abstractions (`DotNetRazorPages.Services`)
- Data: EF Core SQL Server repositories (`DotNetRazorPages.Data`)
- Tests: unit, integration, and functional suites (`DotNetRazorPages.Tests`)

Layering rule:
- Web -> Services -> Data

## Key Capabilities
- Employee list with server-side paging, sorting, and search
- Employee detail create/edit/delete flows
- Export endpoints:
  - List: CSV and JSON
  - Detail: PDF
- Cookie authentication + role-based admin authorization policy
- Startup database initialization with minimum employee seed baseline

## Stakeholder Snapshot
| Stakeholder | What Matters Most | Current State |
|---|---|---|
| Product/Business | Employee workflows and exports | Implemented |
| End Users | Fast list browsing and editing | Implemented |
| Engineering | Layered architecture and abstractions | Implemented |
| QA | Automated test coverage across layers | Implemented (unit/integration/functional) |
| Security | AuthN/AuthZ controls and protected admin routes | Implemented (dev-focused auth model) |
| Operations | Repeatable local setup and startup initialization | Implemented |

## Security and Compliance Notes
- Authentication: cookie-based
- Authorization: role policy (`AdminOnly`) from configuration
- Production-only hardening concerns:
  - Replace development credentials and AD bind values with managed secrets
  - Integrate enterprise IdP (for example OIDC/Entra ID)
  - Define formal retention/export governance policy

## Quality and Readiness
- Solution includes automated tests for:
  - Page model behavior
  - Service and data behavior
  - Functional endpoint and authorization flows
- Current working baseline builds and tests successfully in local development.

## Top Risks and Next Steps
1. Replace development-oriented authentication with enterprise SSO.
2. Move sensitive configuration to secure secret management.
3. Formalize SLAs/SLOs and observability standards.
4. Expand traceability between requirements and test cases for release governance.

## Reference Documents
- Requirements: `docs/REQUIREMENTS.md`
- Architecture and diagrams: `docs/ARCHITECTURE.md`
- Admin implementation notes: `docs/08-reference/ADMIN_ROLE_IMPLEMENTATION.md`
