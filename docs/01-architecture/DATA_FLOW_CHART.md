# DotNet Razor Pages Data Flow Chart

## Document Control
- Document ID: DRP-DFD-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Audience: Engineering, QA, Security, Architecture

## 1. Purpose
This document defines how data moves through the DotNet Razor Pages solution, from user input through business processing, persistence, and export output.

## 2. Data Flow Diagram (Level 0 - Context)
```mermaid
flowchart LR
    U[Internal User] -->|Form Input, Query Params| P0((DotNet Razor Pages System))
    P0 -->|HTML Views, Status Messages| U

    P0 -->|SQL Queries and Commands| DB[(SQL Server)]
    DB -->|Employee Data| P0

    P0 -->|Directory Lookup Request| AD[(Active Directory)]
    AD -->|Directory User Data| P0

    P0 -->|CSV, JSON, PDF Downloads| U
```

## 3. Data Flow Diagram (Level 1 - Application Internals)
```mermaid
flowchart TB
    subgraph External
      User[Internal User]
      AD[(Active Directory)]
      SQL[(SQL Server)]
    end

    subgraph WebLayer[Web Layer]
      W1[Employees PageModel]
      W2[EmployeeDetail PageModel]
      W3[Admin and AD PageModels]
    end

    subgraph ServiceLayer[Services Layer]
      S1[IEmployeeService / EmployeeService]
      S2[IEmployeePdfService / EmployeePdfService]
      S3[IActiveDirectoryService / ActiveDirectoryService]
    end

    subgraph DataLayer[Data Layer]
      D1[IEmployeeRepository / EmployeeRepository]
      D2[ApplicationDbContext]
    end

    User -->|List Params: page, size, sort, search| W1
    User -->|Employee Form Data| W2
    User -->|Admin and Lookup Inputs| W3

    W1 -->|GetEmployees/GetAllEmployees| S1
    W2 -->|Create/Update/Delete/GetById| S1
    W2 -->|GenerateEmployeeReport| S2
    W3 -->|Lookup Query| S3

    S1 --> D1
    D1 --> D2
    D2 -->|EF Core SQL Operations| SQL
    SQL -->|Entity Rows| D2

    S3 -->|LDAP/Directory Query| AD
    AD -->|User Directory Result| S3

    W1 -->|CSV/JSON File Stream| User
    W2 -->|PDF File Stream| User
    W1 -->|Rendered HTML| User
    W2 -->|Rendered HTML| User
    W3 -->|Rendered HTML| User
```

## 4. Data Flow Diagram (Level 2 - Employee CRUD and List)
```mermaid
flowchart TD
    A[User Request: Employees or EmployeeDetail] --> B[PageModel Input Binding]
    B --> C{Operation Type}

    C -- List --> D[Normalize Query: page, size, sort, search]
    D --> E[EmployeeService.GetEmployeesAsync]
    E --> F[EmployeeRepository.GetPagedAsync]
    F --> G[(SQL Server: Employees)]
    G --> F
    F --> E
    E --> H[DTO Mapping]
    H --> I[Render Employees View]

    C -- Create/Update/Delete --> J[Model Validation]
    J --> K{Valid?}
    K -- No --> L[Return View with Validation Errors]
    K -- Yes --> M[EmployeeService CRUD]
    M --> N[EmployeeRepository CRUD]
    N --> G
    G --> N
    N --> M
    M --> O[Redirect + Status Message]
```

## 5. Data Flow Diagram (Level 2 - Export Flows)
```mermaid
flowchart LR
    subgraph ListExports[Employees List Exports]
      A1[User Click: Export CSV] --> A2[EmployeesModel ExportCsv Handler]
      A3[User Click: Export JSON] --> A4[EmployeesModel ExportJson Handler]
      A2 --> A5[EmployeeService.GetAllEmployeesAsync]
      A4 --> A5
      A5 --> A6[Repository Query + DTO Mapping]
      A6 --> A7[Format to CSV or JSON]
      A7 --> A8[File Response to User]
    end

    subgraph PdfExport[Employee Detail PDF Export]
      B1[User Click: Export PDF] --> B2[EmployeeDetailModel ExportPdf Handler]
      B2 --> B3[EmployeeService.GetEmployeeByIdAsync]
      B3 --> B4[Repository Query]
      B4 --> B5[Employee DTO]
      B5 --> B6[IEmployeePdfService.GenerateEmployeeReport]
      B6 --> B7[PDF Byte Stream]
      B7 --> B8[application/pdf Response to User]
    end
```

## 6. Data Stores and Data Objects

### 6.1 Data Stores
- SQL Server database: employee master records
- Active Directory: directory identity attributes for admin lookup workflows

### 6.2 Primary Data Objects
- Employee Input Model (web binding)
- Employee DTO (service boundary)
- Employee Entity (data persistence)
- Export payloads (CSV text, JSON text, PDF bytes)

## 7. Data Governance Notes
- Input validation occurs at model-binding and service boundaries.
- Read operations use paged and filtered queries for performance.
- Export endpoints generate downloadable artifacts derived from current query/entity state.
- Role-based authorization controls route access for admin and directory features.

## 8. Related Documents
- User flows: docs/USER_FLOWS.md
- Requirements: docs/REQUIREMENTS.md
- Architecture: docs/ARCHITECTURE.md
- Systems architecture: docs/SYSTEMS_ARCHITECTURE.md
