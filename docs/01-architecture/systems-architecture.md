# DotNet Razor Pages Systems Architecture

## Document Control
- Document ID: DRP-SYS-ARCH-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft (baseline)
- Owners: Engineering, Security, Operations

## 1. Purpose
This document describes the end-to-end systems architecture of the DotNet Razor Pages solution, including runtime topology, trust boundaries, integration points, operational concerns, and target-state recommendations for enterprise deployment.

## 2. System Scope
The system is an internal employee management platform that provides:
- Employee CRUD and searchable listings
- Data exports (CSV, JSON, PDF)
- Role-restricted admin capabilities
- Optional Active Directory lookups

## 3. Architecture Drivers
- Keep implementation simple and maintainable for a small internal product
- Enforce clear layer boundaries (Web -> Services -> Data)
- Provide secure role-based access to privileged features
- Support local and enterprise deployment patterns
- Enable repeatable operations with automated tests and startup initialization

## 4. Current Runtime Topology (Logical)
```mermaid
flowchart LR
    U[Internal User] --> B[Browser]
    B --> W[Web App: ASP.NET Core Razor Pages]
    W --> S[Services Layer]
    S --> D[Data Layer / EF Core]
    D --> DB[(SQL Server)]
    S --> AD[(Active Directory)]
```

## 5. Deployment Topology (Current and Target)

### 5.1 Current (Developer Local)
```mermaid
flowchart TB
    subgraph DevMachine[Developer Machine]
      Browser[Browser]
      App[DotNetRazorPages.Web Process]
      SqlContainer[(SQL Server Container)]
    end

    Browser --> App
    App --> SqlContainer
```

### 5.2 Target (Enterprise Environment)
```mermaid
flowchart TB
    User[Internal User]
    IdP[Enterprise Identity Provider]
    WAF[Ingress/WAF]

    subgraph AppTier[Application Tier]
      App1[Web App Instance A]
      App2[Web App Instance B]
      AppN[Web App Instance N]
    end

    subgraph DataTier[Data Tier]
      SqlPrimary[(SQL Server Primary)]
      SqlReplica[(SQL Server Read Replica/HA)]
    end

    subgraph Ops[Operations]
      Logs[Centralized Logs]
      Metrics[Metrics/Tracing]
      Vault[Secret Store]
    end

    User --> WAF
    WAF --> App1
    WAF --> App2
    WAF --> AppN

    App1 --> IdP
    App2 --> IdP
    AppN --> IdP

    App1 --> SqlPrimary
    App2 --> SqlPrimary
    AppN --> SqlPrimary
    SqlPrimary --> SqlReplica

    App1 --> Logs
    App2 --> Logs
    AppN --> Logs
    App1 --> Metrics
    App2 --> Metrics
    AppN --> Metrics
    App1 --> Vault
    App2 --> Vault
    AppN --> Vault
```

## 6. Trust Boundaries and Security Zones
```mermaid
flowchart LR
    subgraph Zone1[User Zone]
      U[Internal User Browser]
    end

    subgraph Zone2[Application Zone]
      APP[ASP.NET Core Web App]
    end

    subgraph Zone3[Data and Directory Zone]
      SQL[(SQL Server)]
      AD[(Active Directory)]
    end

    U -->|HTTPS| APP
    APP -->|DB Connection (TLS recommended)| SQL
    APP -->|LDAP/LDAPS| AD
```

Security controls (baseline):
- Authentication via cookie middleware (development-friendly model in current state)
- Authorization via role-based policy (`AdminOnly`) and claims evaluation
- HSTS enabled for non-development runtime
- Startup fail-fast behavior on missing required DB connection string

Required enterprise hardening:
- Replace development auth model with enterprise SSO (OIDC/Entra ID)
- Move credentials and bind secrets to managed secret store
- Enforce TLS for all service-to-service traffic
- Add structured audit logging for privileged actions and exports

## 7. Core Runtime Flows

### 7.1 Request/Response Flow
```mermaid
sequenceDiagram
    autonumber
    participant C as Client Browser
    participant M as ASP.NET Middleware
    participant P as Razor PageModel
    participant S as Service
    participant R as Repository
    participant Q as SQL Server

    C->>M: HTTP Request
    M->>M: AuthN/AuthZ checks
    M->>P: Route to page handler
    P->>S: Call service abstraction
    S->>R: Execute business data operation
    R->>Q: SQL query/command
    Q-->>R: Data/result
    R-->>S: Entity/model
    S-->>P: DTO/response model
    P-->>C: HTML/File response
```

### 7.2 Startup Initialization Flow
```mermaid
sequenceDiagram
    autonumber
    participant Host as App Host
    participant DI as Service Provider Scope
    participant Init as IApplicationDbInitializer
    participant DB as SQL Server

    Host->>DI: Create startup scope
    DI->>Init: Resolve initializer
    Init->>DB: EnsureCreated / seed minimum data
    DB-->>Init: Initialization complete
    Init-->>Host: Ready
```

## 8. Data Architecture
Primary store: SQL Server
- Database: `DotNetRazorPagesDb`
- Core table: `Employees`
- Key index: unique index on `(FirstName, LastName)`

Data access characteristics:
- EF Core with SQL Server provider
- Repository pattern for encapsulated querying
- Server-side paging/sorting/filtering paths
- AsNoTracking used for read-heavy operations

## 9. Integration Architecture
External dependency: Active Directory
- Encapsulated behind `IActiveDirectoryService`
- Configuration-driven (domain, container, bind account, SSL flag)
- Should use LDAPS and managed credentials in enterprise mode

## 10. Availability, Scalability, and Resilience
Current state:
- Single app process in local dev
- SQL container-backed persistence

Target enterprise posture:
- Horizontal app scaling behind ingress/load balancer
- SQL HA/replication strategy
- Health checks and graceful shutdown
- Connection retry policies and transient fault handling
- Backup/restore validation and DR runbooks

## 11. Observability and Operations
Recommended baseline:
- Structured logs with correlation IDs
- Request, dependency, and exception telemetry
- Metrics for latency, throughput, and error rates
- Alerting on auth failures, export errors, and DB connectivity issues
- Deployment dashboards and release annotations

## 12. Deployment and Configuration Model
Configuration sources:
- `appsettings.json` + environment overrides + environment variables

Deployment recommendations:
- Immutable artifact builds
- Environment-specific configuration overlays
- Secret injection at runtime (never in source-controlled config)
- Blue/green or rolling deployment strategy

## 13. Risks and Mitigations
- Risk: Development auth model used in production
  - Mitigation: Enforce enterprise IdP integration before production release
- Risk: Credential leakage via static config
  - Mitigation: Move to managed secret store and key rotation policy
- Risk: Limited operational visibility
  - Mitigation: Implement centralized telemetry and SLO-driven alerting

## 14. Architecture Decisions (Current)
- AD-001: Layered monolith architecture for delivery speed and simplicity
- AD-002: EF Core repository abstraction to isolate data access concerns
- AD-003: Cookie + role policy authorization as current baseline
- AD-004: QuestPDF selected for server-side PDF generation

## 15. Related Documents
- Requirements specification: `docs/requirements.md`
- Solution architecture overview: `docs/architecture.md`
- One-page stakeholder summary: `docs/one-page-summary.md`
