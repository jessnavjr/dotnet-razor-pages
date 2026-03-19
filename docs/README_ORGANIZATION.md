# Documentation Organization Guide

This folder contains comprehensive technical documentation organized by topic for the dotnet-razor-pages project.

## Folder Structure

```
docs/
├── 00-getting-started/
│   ├── README.md                          # Main documentation index
│   └── ONE_PAGE_SUMMARY.md                # Quick project overview
│
├── 01-architecture/
│   ├── ARCHITECTURE.md                    # Core architecture overview
│   ├── SYSTEMS_ARCHITECTURE.md            # Complete system design
│   ├── ARCHITECTURE_PATTERNS_COMPARISON.md # Monolith vs Layered vs Clean
│   ├── ENTERPRISE_ARCHITECTURE_BANKING.md # Enterprise banking system reference
│   ├── MODERN_WEB_APP_ARCHITECTURE_TRENDS.md # Current architecture direction
│   ├── ENTERPRISE_SERVICE_BUS_PATTERNS.md # ESB patterns and integration
│   └── DATA_FLOW_CHART.md                 # Data flow visualization
│
├── 02-backend/
│   ├── ASP_NET_RAZOR_PAGES_ENTERPRISE_TEMPLATE.md # Razor Pages best practices
│   ├── ASPNET_DOTNET_SECURITY_EVOLUTION.md        # .NET security evolution
│   ├── ASPNET_PRESENTATION_FRAMEWORKS_COMPARISON.md # Web Forms vs MVC vs Blazor
│   └── API_CALLS_TYPESCRIPT_VS_CSHARP_RAZORPAGES.md # API call patterns
│
├── 03-frontend/
│   ├── FRONTEND_FRAMEWORKS_COMPARISON.md          # Angular, React, Blazor
│   ├── MVC_VS_MVVM_FRONTEND_PATTERNS.md            # Frontend architecture patterns
│   ├── BOOTSTRAP_AND_CSS_FRAMEWORK_COMPARISON.md  # CSS framework choices
│   ├── JAVASCRIPT_EVOLUTION_AND_INDUSTRY_PERCEPTION.md # JavaScript history
│   ├── HTML_RELEASE_DIFFERENCES_AND_COMMON_PATTERNS.md # HTML4/5/5.2 comparison
│   └── NAMING_CONVENTIONS_COMPARISON.md           # Naming standards across tech
│
├── 04-data/
│   ├── DATABASE_PERSISTENCE_OPTIONS.md   # SQL Server + 9 alternatives
│   └── ENTITY_RELATIONSHIP_DIAGRAM.md    # Database schema design
│
├── 05-testing/
│   ├── TEST_PLAN.md                      # Testing strategy and coverage
│   └── TESTING_TYPESCRIPT_IN_ASPNET_CORE.md # TypeScript testing best practices
│
├── 06-deployment/
│   ├── DEPLOYMENT_PLAN_AZDO_IIS_SQL.md   # Azure DevOps + IIS + SQL deployment
│   └── CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md # Cloud migration strategy
│
├── 07-business/
│   ├── BUSINESS_REQUIREMENTS.md          # Business requirements document
│   ├── REQUIREMENTS.md                   # Technical requirements
│   ├── USE_CASES.md                      # Use cases and user stories
│   ├── USER_FLOWS.md                     # User flow diagrams
│   └── WIREFRAMES.md                     # UI wireframes
│
└── 08-reference/
    ├── SECURITY_SUMMARY_AND_REVIEW.md    # Security review and standards
    ├── ADMIN_ROLE_IMPLEMENTATION.md      # Admin role implementation guide
    ├── ADMIN_ROLE_QUICK_REFERENCE.md     # Quick reference for admins
    ├── DEPENDENCY_INVENTORY.json         # Project dependencies list
    └── DEPENDENCY_LIFECYCLE_AND_UPGRADE_PLAN.md # Dependency management
```

## Quick Navigation

### For Getting Started
- **New to the project?** Start with  [00-getting-started/README.md](00-getting-started/README.md)
- **Need a quick overview?** See [00-getting-started/ONE_PAGE_SUMMARY.md](00-getting-started/ONE_PAGE_SUMMARY.md)

### For Architecture Decisions
- **Understanding the overall system?** Read [01-architecture/SYSTEMS_ARCHITECTURE.md](01-architecture/SYSTEMS_ARCHITECTURE.md)
- **Comparing architectural patterns?** Check [01-architecture/ARCHITECTURE_PATTERNS_COMPARISON.md](01-architecture/ARCHITECTURE_PATTERNS_COMPARISON.md)
- **Learning about modern trends?** See [01-architecture/MODERN_WEB_APP_ARCHITECTURE_TRENDS.md](01-architecture/MODERN_WEB_APP_ARCHITECTURE_TRENDS.md)

### For Backend Development
- **Working with Razor Pages?** Read [02-backend/ASP_NET_RAZOR_PAGES_ENTERPRISE_TEMPLATE.md](02-backend/ASP_NET_RAZOR_PAGES_ENTERPRISE_TEMPLATE.md)
- **Understanding API patterns?** Check [02-backend/API_CALLS_TYPESCRIPT_VS_CSHARP_RAZORPAGES.md](02-backend/API_CALLS_TYPESCRIPT_VS_CSHARP_RAZORPAGES.md)
- **Learning about .NET security?** See [02-backend/ASPNET_DOTNET_SECURITY_EVOLUTION.md](02-backend/ASPNET_DOTNET_SECURITY_EVOLUTION.md)

### For Frontend Development
- **Choosing a frontend framework?** Read [03-frontend/FRONTEND_FRAMEWORKS_COMPARISON.md](03-frontend/FRONTEND_FRAMEWORKS_COMPARISON.md)
- **Understanding MVC vs MVVM?** Check [03-frontend/MVC_VS_MVVM_FRONTEND_PATTERNS.md](03-frontend/MVC_VS_MVVM_FRONTEND_PATTERNS.md)
- **Comparing CSS frameworks?** See [03-frontend/BOOTSTRAP_AND_CSS_FRAMEWORK_COMPARISON.md](03-frontend/BOOTSTRAP_AND_CSS_FRAMEWORK_COMPARISON.md)

### For Data & Database
- **Choosing a database?** Read [04-data/DATABASE_PERSISTENCE_OPTIONS.md](04-data/DATABASE_PERSISTENCE_OPTIONS.md)
- **Understanding data structure?** See [04-data/ENTITY_RELATIONSHIP_DIAGRAM.md](04-data/ENTITY_RELATIONSHIP_DIAGRAM.md)

### For Testing
- **Planning tests?** Read [05-testing/TEST_PLAN.md](05-testing/TEST_PLAN.md)
- **Testing TypeScript code?** Check [05-testing/TESTING_TYPESCRIPT_IN_ASPNET_CORE.md](05-testing/TESTING_TYPESCRIPT_IN_ASPNET_CORE.md)

### For Deployment & Operations
- **Deploying to Azure?** Read [06-deployment/DEPLOYMENT_PLAN_AZDO_IIS_SQL.md](06-deployment/DEPLOYMENT_PLAN_AZDO_IIS_SQL.md)
- **Planning cloud migration?** See [06-deployment/CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md](06-deployment/CLOUD_MIGRATION_ROADMAP_MICROSOFT_STANDARDS.md)

### For Business & Requirements
- **Understanding the product?** Read [07-business/BUSINESS_REQUIREMENTS.md](07-business/BUSINESS_REQUIREMENTS.md)
- **Exploring use cases?** Check [07-business/USE_CASES.md](07-business/USE_CASES.md)
- **Viewing mockups?** See [07-business/WIREFRAMES.md](07-business/WIREFRAMES.md)

### For Reference & Governance
- **Security compliance?** Read [08-reference/SECURITY_SUMMARY_AND_REVIEW.md](08-reference/SECURITY_SUMMARY_AND_REVIEW.md)
- **Admin setup?** Check [08-reference/ADMIN_ROLE_IMPLEMENTATION.md](08-reference/ADMIN_ROLE_IMPLEMENTATION.md)
- **Managing dependencies?** See [08-reference/DEPENDENCY_LIFECYCLE_AND_UPGRADE_PLAN.md](08-reference/DEPENDENCY_LIFECYCLE_AND_UPGRADE_PLAN.md)

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
- TypeScript testing in ASP.NET Core with Jest, Vitest, Playwright

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
