# Documentation Organization Guide

This folder contains comprehensive technical documentation organized by topic for the dotnet-razor-pages project.

## Folder Structure

```
docs/
├── 00-getting-started/
│   ├── README.md                          # Main documentation index
│   └── one-page-summary.md                # Quick project overview
│
├── 01-architecture/
│   ├── architecture.md                    # Core architecture overview
│   ├── SYSTEMS_architecture.md            # Complete system design
│   ├── architecture-patterns-comparison.md # Monolith vs Layered vs Clean
│   ├── enterprise-architecture-banking.md # Enterprise banking system reference
│   ├── modern-web-app-architecture-trends.md # Current architecture direction
│   ├── enterprise-service-bus-patterns.md # ESB patterns and integration
│   └── data-flow-chart.md                 # Data flow visualization
│
├── 02-backend/
│   ├── asp-net-razor-pages-enterprise-template.md # Razor Pages best practices
│   ├── aspnet-dotnet-security-evolution.md        # .NET security evolution
│   ├── aspnet-presentation-frameworks-comparison.md # Web Forms vs MVC vs Blazor
│   └── api-calls-typescript-vs-csharp-razorpages.md # API call patterns
│
├── 03-frontend/
│   ├── frontend-frameworks-comparison.md          # Angular, React, Blazor
│   ├── mvc-vs-mvvm-frontend-patterns.md            # Frontend architecture patterns
│   ├── bootstrap-and-css-framework-comparison.md  # CSS framework choices
│   ├── javascript-evolution-and-industry-perception.md # JavaScript history
│   ├── html-release-differences-and-common-patterns.md # HTML4/5/5.2 comparison
│   └── naming-conventions-comparison.md           # Naming standards across tech
│
├── 04-data/
│   ├── database-persistence-options.md   # SQL Server + 9 alternatives
│   └── entity-relationship-diagram.md    # Database schema design
│
├── 05-testing/
│   ├── test-plan.md                      # Testing strategy and coverage
│   └── testing-typescript-in-aspnet-core.md # TypeScript testing best practices
│
├── 06-deployment/
│   ├── deployment-plan-azdo-iis-sql.md   # Azure DevOps + IIS + SQL deployment
│   └── cloud-migration-roadmap-microsoft-standards.md # Cloud migration strategy
│
├── 07-business/
│   ├── business-requirements.md          # Business requirements document
│   ├── requirements.md                   # Technical requirements
│   ├── use-cases.md                      # Use cases and user stories
│   ├── user-flows.md                     # User flow diagrams
│   └── wireframes.md                     # UI wireframes
│
└── 08-reference/
    ├── security-summary-and-review.md    # Security review and standards
    ├── admin-role-implementation.md      # Admin role implementation guide
    ├── admin-role-quick-reference.md     # Quick reference for admins
    ├── DEPENDENCY_INVENTORY.json         # Project dependencies list
    └── dependency-lifecycle-and-upgrade-plan.md # Dependency management
```

## Quick Navigation

### For Getting Started
- **New to the project?** Start with  [00-getting-started/README.md](00-getting-started/README.md)
- **Need a quick overview?** See [00-getting-started/one-page-summary.md](00-getting-started/one-page-summary.md)

### For Architecture Decisions
- **Understanding the overall system?** Read [01-architecture/SYSTEMS_architecture.md](01-architecture/SYSTEMS_architecture.md)
- **Comparing architectural patterns?** Check [01-architecture/architecture-patterns-comparison.md](01-architecture/architecture-patterns-comparison.md)
- **Learning about modern trends?** See [01-architecture/modern-web-app-architecture-trends.md](01-architecture/modern-web-app-architecture-trends.md)

### For Backend Development
- **Working with Razor Pages?** Read [02-backend/asp-net-razor-pages-enterprise-template.md](02-backend/asp-net-razor-pages-enterprise-template.md)
- **Understanding API patterns?** Check [02-backend/api-calls-typescript-vs-csharp-razorpages.md](02-backend/api-calls-typescript-vs-csharp-razorpages.md)
- **Learning about .NET security?** See [02-backend/aspnet-dotnet-security-evolution.md](02-backend/aspnet-dotnet-security-evolution.md)

### For Frontend Development
- **Choosing a frontend framework?** Read [03-frontend/frontend-frameworks-comparison.md](03-frontend/frontend-frameworks-comparison.md)
- **Understanding MVC vs MVVM?** Check [03-frontend/mvc-vs-mvvm-frontend-patterns.md](03-frontend/mvc-vs-mvvm-frontend-patterns.md)
- **Comparing CSS frameworks?** See [03-frontend/bootstrap-and-css-framework-comparison.md](03-frontend/bootstrap-and-css-framework-comparison.md)

### For Data & Database
- **Choosing a database?** Read [04-data/database-persistence-options.md](04-data/database-persistence-options.md)
- **Understanding data structure?** See [04-data/entity-relationship-diagram.md](04-data/entity-relationship-diagram.md)

### For Testing
- **Planning tests?** Read [05-testing/test-plan.md](05-testing/test-plan.md)
- **Testing TypeScript code?** Check [05-testing/testing-typescript-in-aspnet-core.md](05-testing/testing-typescript-in-aspnet-core.md)

### For Deployment & Operations
- **Deploying to Azure?** Read [06-deployment/deployment-plan-azdo-iis-sql.md](06-deployment/deployment-plan-azdo-iis-sql.md)
- **Planning cloud migration?** See [06-deployment/cloud-migration-roadmap-microsoft-standards.md](06-deployment/cloud-migration-roadmap-microsoft-standards.md)

### For Business & Requirements
- **Understanding the product?** Read [07-business/business-requirements.md](07-business/business-requirements.md)
- **Exploring use cases?** Check [07-business/use-cases.md](07-business/use-cases.md)
- **Viewing mockups?** See [07-business/wireframes.md](07-business/wireframes.md)

### For Reference & Governance
- **Security compliance?** Read [08-reference/security-summary-and-review.md](08-reference/security-summary-and-review.md)
- **Admin setup?** Check [08-reference/admin-role-implementation.md](08-reference/admin-role-implementation.md)
- **Managing dependencies?** See [08-reference/dependency-lifecycle-and-upgrade-plan.md](08-reference/dependency-lifecycle-and-upgrade-plan.md)

## Document Summary by Category

### Architecture & Patterns (7 docs)
- Monolithic vs layered vs clean architecture comparisons
- Enterprise banking architecture reference
- Enterprise Service Bus patterns with real-world scenarios
- Modern web application architecture trends

### Backend Technologies (4 docs)
- ASP.NET Core Razor Pages best practices
- .NET security evolution from 1.0 to modern versions
- Presentation framework comparisons (Web Forms, MVC, Razor Pages, Blazor)
- API call patterns and trade-offs

### Frontend Technologies (6 docs)
- Frontend framework comparisons (Angular, React, Blazor)
- MVC vs MVVM architectural patterns for frontend
- CSS framework choices (Bootstrap, Tailwind, Foundation, Material Design)
- JavaScript evolution and history
- HTML version differences and modern patterns
- Naming conventions across all technologies

### Data & Persistence (2 docs)
- Database options: SQL Server + 9 alternatives with comparison
- Entity relationship diagrams and database design

### Testing & Quality (2 docs)
- Comprehensive testing strategy and plan
- TypeScript testing in ASP.NET Core with Jest, Playwright, and alternative runners

### Deployment & Operations (2 docs)
- Full deployment plan for Azure DevOps + IIS + SQL Server
- Cloud migration roadmap with Microsoft standards

### Business & Requirements (5 docs)
- Business requirements and product objectives
- Technical requirements and specifications
- Use cases and user stories
- User workflows and interactions
- UI/UX wireframes

### Security, Admin & Reference (5 docs)
- Security review and compliance standards
- Admin role implementation
- Dependency inventory and management
- Quick reference guides

## Key Features of This Documentation

✓ **Comprehensive**: 32+ detailed documents covering all aspects
✓ **Practical**: Real-world examples and implementation guidance
✓ **Current**: Updated for 2024-2026 technology landscape
✓ **Comparable**: Framework and technology comparisons with trade-offs
✓ **Enterprise-Ready**: Patterns suitable for large teams and organizations
✓ **Well-Organized**: Logical folder structure mirrors developer workflow
✓ **Cross-Referenced**: Related documents link to each other
✓ **Decision-Focused**: Decision frameworks help choose between alternatives

## Using This Documentation

1. **Start with your role**: Find your category (backend, frontend, devops, etc.)
2. **Select your topic**: Choose the specific document for your question
3. **Follow decision frameworks**: Most docs include "Choose X if..." sections
4. **Cross-reference**: Follow links to related documents
5. **Review examples**: Most docs include working code examples
6. **Check matrices**: Comparison tables help evaluate options

## Updating Documentation

When adding new documentation:
1. Determine the most appropriate folder
2. Name files descriptively (PascalCase, underscores between words)
3. Add entry to this README
4. Include table of contents if document > 500 lines
5. Add cross-references to related docs

---

**Last Updated**: March 18, 2026
**Document Count**: 32+ comprehensive guides
**Total Content**: 10,000+ lines of technical documentation
