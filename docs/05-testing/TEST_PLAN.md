# DotNet Razor Pages Test Plan

## Document Control
- Document ID: DRP-TEST-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Owner: QA / Engineering

## 1. Purpose
This test plan defines the approach, scope, environments, test types, and acceptance criteria for validating the DotNet Razor Pages solution.

## 2. Objectives
- Verify functional correctness of employee management workflows.
- Validate security behavior for authentication and authorization boundaries.
- Confirm data integrity across create/update/delete and export features.
- Ensure regression safety through automated unit, integration, and functional tests.

## 3. Scope
### In Scope
- Employee CRUD behavior
- List search, sorting, and pagination
- Export flows (CSV, JSON, PDF)
- Admin access policy enforcement and access-denied behavior
- Active Directory lookup behavior and error handling (where applicable)
- Startup initialization and seed behavior

### Out of Scope
- Load/performance benchmarking at production scale
- Full penetration testing and infrastructure vulnerability scanning
- Browser matrix testing beyond current supported baseline

## 4. Test Strategy
A multi-layer testing strategy is used:
- Unit tests: page model behavior, service mapping/logic, pagination utilities
- Integration tests: repository/data behavior and persistence interactions
- Functional tests: end-to-end endpoint behavior with test host

Primary test project:
- `DotNetRazorPages.Tests`

## 5. Test Items
### Web Layer
- Page models in `DotNetRazorPages.Web/Pages/*`
- Authentication and authorization behavior
- Export handlers and file response metadata

### Services Layer
- Employee service mapping and orchestration
- PDF generation service contract behavior
- Active Directory service behavior and configuration validation

### Data Layer
- EF Core context mappings and repository methods
- Query behavior: filtering, sorting, pagination

## 6. Environments and Data
### 6.1 Local Development Test Environment
- Runtime: .NET 10
- Test host: ASP.NET Core `WebApplicationFactory`
- Datastores:
  - Functional tests: in-memory DB via factory setup
  - Integration tests: in-memory/SQLite depending on fixture

### 6.2 Test Data
- Deterministic fake employees in tests for stable assertions
- Seeded employee dataset for startup and list behavior
- Known role claims for admin/non-admin authorization paths

## 7. Entry and Exit Criteria
### Entry Criteria
- Solution builds successfully
- Required configuration keys are available for test host startup
- Test dependencies restored

### Exit Criteria
- All critical and high-priority test cases pass
- No unresolved high-severity defects for release scope
- Automated suite pass threshold met in CI

## 8. Test Types and Coverage Areas

### 8.1 Unit Testing
Coverage targets:
- Employees page model query normalization and pagination behavior
- Employee detail create/update/delete and not-found handling
- Service DTO/entity mapping correctness
- Export content generation basics (CSV/JSON)

### 8.2 Integration Testing
Coverage targets:
- Repository CRUD and query operations
- Sort/filter/page behavior against data layer
- Data constraints and entity mappings

### 8.3 Functional Testing
Coverage targets:
- Route availability and expected response codes
- Login/access-denied/admin policy flow
- Employee list rendering and detail retrieval
- Export endpoint responses and file metadata
- PDF export response type/signature verification

## 9. Non-Functional Validation
### Security-Focused Validation
- Unauthenticated users are redirected from protected routes
- Non-admin users cannot access admin routes
- Admin users can access admin routes
- Invalid IDs do not leak data and return not-found behavior

### Reliability-Focused Validation
- Startup initialization executes without blocking app boot
- Invalid operations fail gracefully with deterministic responses

## 10. Defect Management
- Severity levels: Critical, High, Medium, Low
- Defects tracked in repository issue management workflow
- Minimum defect metadata:
  - Repro steps
  - Expected vs actual behavior
  - Environment and build
  - Evidence (logs/screenshots/test output)

## 11. Automation and Execution
### Primary Commands
- Build:
  - `dotnet build DotNetRazorPages.sln -c Debug`
- Run all tests:
  - `dotnet test DotNetRazorPages.Tests/DotNetRazorPages.Tests.csproj -c Debug`
- Run focused tests:
  - `dotnet test DotNetRazorPages.Tests/DotNetRazorPages.Tests.csproj -c Debug --filter "FullyQualifiedName~PageModelTests"`

### Suggested CI Stages
1. Restore and build
2. Unit tests
3. Integration tests
4. Functional tests
5. Publish test results and coverage artifacts

## 12. Roles and Responsibilities
- QA Engineer: owns test case design, execution strategy, and release sign-off input
- Developer: writes/updates automated tests with feature changes
- Engineering Lead: enforces quality gates and test completeness
- Product Owner: validates UAT outcomes against business expectations

## 13. Risks and Mitigations
- Risk: test auth model differs from production identity approach
  - Mitigation: add production-auth integration tests once IdP integration is introduced
- Risk: environment-sensitive AD behavior (Windows-only dependency)
  - Mitigation: isolate AD tests with platform-aware execution strategy/mocks
- Risk: regressions in exports due to format changes
  - Mitigation: keep file metadata and content signature assertions in functional suite

## 14. Traceability (High-Level)
| Requirement Area | Planned Test Type |
|---|---|
| Employee CRUD | Unit + Integration + Functional |
| Search/Sort/Pagination | Unit + Integration + Functional |
| CSV/JSON Exports | Unit + Functional |
| PDF Export | Functional + Unit contract tests |
| Admin Authorization | Functional |
| Startup Initialization | Integration + startup verification |

## 15. Release Readiness Checklist
- [ ] Build is green on target branch
- [ ] All automated test suites pass
- [ ] No open critical/high defects
- [ ] Test evidence attached to release artifact
- [ ] Security-sensitive flows verified (authz, export access, error behavior)

## 16. Related Documents
- `docs/REQUIREMENTS.md`
- `docs/ARCHITECTURE.md`
- `docs/SYSTEMS_ARCHITECTURE.md`
- `docs/USER_FLOWS.md`
- `docs/DATA_FLOW_CHART.md`
- `docs/SECURITY_SUMMARY_AND_REVIEW.md`
