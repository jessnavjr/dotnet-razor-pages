# Developer Skillset Guide for Maintaining This Solution

## Purpose

This document defines the skills a developer should have to effectively maintain the DotNet Razor Pages solution in development, testing, deployment, and production support contexts.

It is designed for:
- New team members onboarding to the codebase
- Engineering managers defining hiring/interview criteria
- Tech leads planning capability development
- Teams building a maintainers-on-call rotation

---

## 1. Solution Stack Overview (What You Are Maintaining)

To maintain this solution well, developers need practical ability across:

- ASP.NET Core Razor Pages (`net10.0`)
- C# application/service architecture
- SQL Server data model and query behavior
- Entity Framework Core patterns
- Authentication/authorization (cookie + role/claim policy)
- TypeScript for frontend enhancements and tests
- Test automation (unit, integration, functional)
- Azure DevOps pipeline operations
- IIS deployment and runtime troubleshooting
- Production support runbook execution

---

## 2. Skill Proficiency Levels

Use this scale when assessing maintainers:

- **L1 - Foundational**: Can contribute with guidance
- **L2 - Working**: Can implement and troubleshoot independently for common tasks
- **L3 - Advanced**: Can design, optimize, and mentor others

Recommended baseline:
- Mid-level maintainer: mostly L2
- Senior maintainer: L2-L3 across core areas
- On-call primary: L2 minimum in operations + L3 in at least one critical domain

---

## 3. Core Skill Matrix

| Skill Area | Why It Matters Here | Target Level |
|---|---|---|
| ASP.NET Core Razor Pages | Main web framework and request/page lifecycle | L2 |
| C# and .NET Runtime | Business logic, services, dependency injection, async patterns | L2 |
| SQL Server | Data correctness, troubleshooting, performance basics | L2 |
| EF Core | Data access behavior, query shape, mapping and migrations awareness | L2 |
| AuthN/AuthZ | Cookie auth, role policy enforcement, route protection | L2 |
| Testing in .NET | Regression prevention and safe refactoring | L2 |
| TypeScript | Frontend behavior and support scripts | L1-L2 |
| Frontend test tooling (Vitest) | Maintaining web-side tests | L1-L2 |
| Azure DevOps YAML | Build/deploy pipeline troubleshooting and release confidence | L2 |
| IIS Operations | Production deploy, app pool/site diagnosis, rollback | L2 |
| Incident Handling | Fast triage and recovery in production | L2 |
| Security Basics | Secure coding and operational hardening | L2 |
| Documentation and communication | Change traceability, support handoff quality | L2 |

---

## 4. Detailed Skill Expectations

## 4.1 ASP.NET Core Razor Pages (Core)

A maintainer should be able to:
- Understand page model lifecycle and handler methods (`OnGet`, `OnPost`, etc.)
- Read and modify Razor Pages under `Pages/`
- Work with model binding, validation, and anti-forgery behavior
- Trace route-to-page and page-to-service execution paths
- Diagnose common runtime issues in middleware/request pipeline

Minimum competency target: **L2**

---

## 4.2 C# Application and Service Layer

A maintainer should be able to:
- Navigate service abstractions and implementations
- Use dependency injection correctly and safely
- Apply async/await patterns to avoid deadlocks and blocking
- Handle exceptions with meaningful logging and user-safe behavior
- Make safe refactors while preserving public contracts

Minimum competency target: **L2**

---

## 4.3 SQL Server and Data Layer

A maintainer should be able to:
- Understand core schema relationships used by business features
- Write and review basic-to-intermediate SQL queries
- Validate data changes and troubleshoot data-related defects
- Analyze query behavior for correctness and basic performance concerns
- Collaborate with DBAs on deployment/rollback strategy

Minimum competency target: **L2**

---

## 4.4 EF Core and Data Access Patterns

A maintainer should be able to:
- Understand repository/service-level data access flow
- Identify common query pitfalls (over-fetching, inefficient filtering)
- Troubleshoot data mapping and lifecycle issues
- Validate persistence behavior with tests

Minimum competency target: **L2**

---

## 4.5 Authentication and Authorization

A maintainer should be able to:
- Understand cookie authentication flow
- Work with role/claim-based authorization policy behavior
- Troubleshoot login/access denied behavior safely
- Ensure protected routes remain protected after code changes

Minimum competency target: **L2**

---

## 4.6 Frontend Skills (TypeScript + HTML/CSS)

A maintainer should be able to:
- Read and update TypeScript used in web UI behavior
- Debug browser-side issues from network to DOM behavior
- Maintain basic frontend patterns and avoid regressions
- Keep UI code aligned with Razor-rendered architecture

Minimum competency target: **L1-L2**

Note:
This solution is server-rendered first, so frontend depth can be lighter than in a full SPA team.

---

## 4.7 Testing and Quality Engineering

A maintainer should be able to:
- Run and interpret .NET test suites
- Add/update unit and integration tests for changed behavior
- Maintain TypeScript tests (`vitest`) for frontend logic changes
- Validate edge cases and security-sensitive flows before release

Minimum competency target: **L2**

---

## 4.8 DevOps and Deployment (Azure DevOps + IIS)

A maintainer should be able to:
- Read Azure DevOps YAML and understand stage flow
- Diagnose build, test, artifact, and deploy failures
- Understand artifact promotion model (Dev -> Test -> Prod)
- Perform safe IIS checks (site/app pool/env var issues)
- Follow rollback runbook without improvising risky actions

Minimum competency target: **L2**

---

## 4.9 Production Support and Incident Response

A maintainer should be able to:
- Execute first-10-minute triage checklist
- Identify whether incidents are app, infra, or DB related
- Use logs and telemetry to isolate probable root cause
- Execute approved recovery actions and validate service restoration
- Escalate clearly with high-quality incident evidence

Minimum competency target: **L2**

---

## 5. Role-Based Skill Profiles

## 5.1 Feature Maintainer (Day-to-Day Delivery)

Recommended profile:
- Strong in Razor Pages, C#, SQL, and tests
- Working knowledge of TypeScript
- Working knowledge of deployment pipeline basics

Primary responsibilities:
- Implement feature changes
- Maintain quality and test coverage
- Support non-critical issue triage

---

## 5.2 Release Maintainer (Build/Deploy Reliability)

Recommended profile:
- Strong in Azure DevOps, artifact flow, release validation
- Strong in IIS deploy/runtime troubleshooting
- Working knowledge of app/runtime behavior and DB deploy patterns

Primary responsibilities:
- Own release readiness checks
- Resolve deployment failures quickly
- Coordinate rollback decisions

---

## 5.3 On-Call Maintainer (Production Support)

Recommended profile:
- Strong in runbook execution and incident triage
- Strong in app + IIS + SQL diagnostics
- Strong communication and incident command discipline

Primary responsibilities:
- Incident response and service restoration
- Post-incident evidence and remediation follow-up

---

## 6. Practical Competency Checklist (Hands-On)

A maintainer is considered ready when they can perform all of the following:

1. Build and run the solution locally with configured dependencies
2. Explain the request flow from Razor Page to service to data store
3. Implement a small feature change with tests
4. Diagnose and fix a failing .NET test
5. Diagnose and fix a failing TypeScript test
6. Troubleshoot an authorization failure on an admin route
7. Trace and resolve a SQL data defect in a non-production environment
8. Analyze a pipeline failure and propose corrective action
9. Execute deployment smoke checks
10. Follow rollback steps from the production runbook

---

## 7. Recommended Learning Path for New Maintainers (30-60-90 Days)

## Days 1-30

Focus:
- Local setup, architecture basics, and core code navigation
- Razor Pages, service layer, and data flow understanding
- Run tests and small bugfix contributions

Deliverables:
- 1-2 low-risk bug fixes
- Demonstrated ability to run and interpret all test suites

## Days 31-60

Focus:
- Feature-level ownership and code review quality
- SQL/EF troubleshooting and auth flow understanding
- Basic pipeline diagnosis

Deliverables:
- 1 medium feature or refactor with tests
- One documented incident simulation/triage exercise

## Days 61-90

Focus:
- Release participation and operational readiness
- Deeper production diagnostics and runbook familiarity
- Mentored on-call shadow

Deliverables:
- Lead one lower-environment release validation
- Complete on-call readiness checklist

---

## 8. Hiring and Interview Guidance

Evaluate candidates on:
- ASP.NET Core and C# depth
- SQL troubleshooting and data correctness mindset
- Testing discipline and regression thinking
- Practical deployment and operations awareness
- Security-by-default decision-making
- Communication under incident pressure

Suggested interview artifacts:
- Small Razor Page change exercise
- Data bug triage case
- Production incident scenario discussion
- Pipeline failure diagnosis walkthrough

---

## 9. Anti-Patterns to Avoid in Maintainers

- Strong coding ability with weak operational discipline
- Feature delivery without test updates
- Security assumptions without explicit validation
- Making production fixes without rollback plan
- Debugging by guesswork rather than evidence

---

## 10. Team Composition Recommendation

For stable maintenance coverage, target:
- 2-3 developers at L2+ across app and data domains
- 1 developer with stronger DevOps/IIS release expertise
- Shared on-call coverage with clear escalation to DBA/platform support

If team size is small:
- Prioritize cross-training on deployment and incident playbooks
- Keep runbooks and troubleshooting docs continuously updated

---

## 11. Definition of "Maintenance-Ready Developer" for This Solution

A maintenance-ready developer can:
- Deliver changes safely
- Protect security and data integrity
- Keep tests healthy
- Deploy with confidence
- Respond to incidents using runbooks and evidence

That combination is more valuable than deep specialization in a single area.

---

## Related Documents

- `docs/06-deployment/PRODUCTION_SUPPORT_RUNBOOK.md`
- `docs/06-deployment/DEPLOYMENT_PLAN_AZDO_IIS_SQL.md`
- `docs/05-testing/TEST_PLAN.md`
- `docs/01-architecture/SYSTEMS_ARCHITECTURE.md`
- `docs/08-reference/SECURITY_SUMMARY_AND_REVIEW.md`
