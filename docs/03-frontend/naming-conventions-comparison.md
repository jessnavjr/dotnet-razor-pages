# Naming Convention Differences: .NET vs SQL Server vs JavaScript vs CSS vs Git Repositories

## Overview

Different technology stacks favor different naming styles. Using the right convention per layer improves readability, consistency, tooling compatibility, and collaboration across teams.

This document compares common naming conventions for:

1. .NET applications
2. Microsoft SQL Server
3. JavaScript/TypeScript
4. CSS
5. Git repositories

---

## Quick Comparison Matrix

| Area | Common Case Style | Typical Separator | Example |
|------|-------------------|-------------------|---------|
| .NET Class/Type | PascalCase | None | CustomerAccountService |
| .NET Local Variable | camelCase | None | customerAccount |
| .NET Private Field | _camelCase | Leading underscore | _customerRepository |
| SQL Server Table | PascalCase or snake_case | None or _ | CustomerOrder or customer_order |
| SQL Server Column | PascalCase or snake_case | None or _ | CreatedDate or created_date |
| JavaScript Variable/Function | camelCase | None | fetchCustomerData |
| JavaScript Class | PascalCase | None | CustomerProfileCard |
| CSS Class | kebab-case | - | customer-profile-card |
| CSS Custom Property | kebab-case with -- | - | --color-primary |
| Git Repository | kebab-case | - | customer-portal-api |

---

## 1. .NET Naming Conventions

.NET conventions are strongly standardized by Microsoft and C# style guidelines.

### Preferred Patterns

- Namespaces: PascalCase
- Classes/Interfaces/Records/Enums: PascalCase
- Methods/Properties: PascalCase
- Method parameters/local variables: camelCase
- Private fields: _camelCase
- Constants: PascalCase
- Async methods: PascalCase + Async suffix
- Interfaces: I + PascalCase prefix

### .NET Examples

```csharp
namespace DotNetRazorPages.Services;

public interface IEmployeeService
{
    Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId);
}

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private const int MaxPageSize = 100;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
    {
        var employee = await _employeeRepository.FindByIdAsync(employeeId);
        return employee is null ? null : MapToDto(employee);
    }

    private static EmployeeDto MapToDto(Employee entity)
    {
        return new EmployeeDto { EmployeeId = entity.Id, FullName = entity.FullName };
    }
}
```

### .NET Do/Do Not

Do:
- Use PascalCase for public API surface
- Use meaningful, domain-aligned names
- Include Async suffix for async methods

Avoid:
- Hungarian notation (strName, iCount)
- Abbreviations that are unclear
- Inconsistent casing across files

---

## 2. Microsoft SQL Server Naming Conventions

SQL Server naming is less universally enforced than C#, so teams should standardize early.

### Common Patterns

Two popular styles:

1. PascalCase style
- Tables: CustomerOrder
- Columns: OrderDate, CustomerId
- Procedures: usp_GetCustomerOrders (legacy style) or GetCustomerOrders

2. snake_case style
- Tables: customer_order
- Columns: order_date, customer_id
- Procedures: get_customer_orders

### Typical Recommendation for SQL Server

- Pick one style and keep it consistent across schema
- Prefer singular table names if entity-centric (Customer), or plural if team standard (Customers)
- Foreign keys: RelatedEntityId (PascalCase) or related_entity_id (snake_case)
- Primary key: Id or EntityId
- Avoid spaces and reserved words

### SQL Server Examples

```sql
-- PascalCase style
CREATE TABLE CustomerOrder (
    CustomerOrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- snake_case style
CREATE TABLE customer_order (
    customer_order_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    order_date DATETIME2 NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL,
    created_date DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
```

### SQL Server Do/Do Not

Do:
- Keep table/column naming consistent
- Use explicit, descriptive constraint names
- Use deterministic naming for indexes and foreign keys

Avoid:
- Prefix-only naming like tblCustomer, colName (legacy patterns)
- Mixed case styles in same schema
- Cryptic abbreviations

---

## 3. JavaScript/TypeScript Naming Conventions

JavaScript ecosystem conventions are broadly consistent and similar to C# in many areas, but with different defaults for members and constants.

### Preferred Patterns

- Variables/functions: camelCase
- Classes/components: PascalCase
- Constants: UPPER_SNAKE_CASE (for true constants)
- File names (frontend): kebab-case or framework convention
- Booleans: is/has/can prefixes (isEnabled, hasAccess)
- Event handlers: handleX or onX naming

### JavaScript/TypeScript Examples

```ts
const MAX_RETRY_COUNT = 3;

class CustomerAccountService {
  async fetchCustomerOrders(customerId: number): Promise<Order[]> {
    return apiClient.get(`/customers/${customerId}/orders`);
  }
}

function calculateTotalAmount(lineItems: LineItem[]): number {
  return lineItems.reduce((sum, item) => sum + item.amount, 0);
}

const isPaymentAuthorized = true;
const handleSubmit = () => {
  // submit logic
};
```

### JS/TS Do/Do Not

Do:
- Use camelCase for most identifiers
- Use PascalCase for class and React component names
- Keep file naming consistent within project

Avoid:
- Mixing snake_case and camelCase for similar symbol types
- Overusing abbreviations
- Naming functions as nouns when they perform actions

---

## 4. CSS Naming Conventions

CSS conventions prioritize predictability, composability, and collision avoidance.

### Common Patterns

- Class names: kebab-case
- CSS custom properties: --kebab-case
- Utility classes: short but consistent (mt-4, text-center)
- Component naming systems: BEM, utility-first, or design token-driven styles

### BEM (Block Element Modifier)

Pattern:
- Block: .card
- Element: .card__title
- Modifier: .card--highlighted

### CSS Examples

```css
:root {
  --color-primary: #0057b8;
  --spacing-md: 1rem;
}

.customer-card {
  padding: var(--spacing-md);
  border-radius: 8px;
}

.customer-card__title {
  font-weight: 600;
}

.customer-card--premium {
  border: 2px solid var(--color-primary);
}
```

### CSS Do/Do Not

Do:
- Use kebab-case for class names
- Prefer predictable component naming strategy (BEM or utility convention)
- Scope styles by component to avoid collisions

Avoid:
- CamelCase class names in CSS
- Generic classes like .box, .thing, .item unless utility-intended
- Deep selector chains that are fragile

---

## 5. Git Repository Naming Conventions

Repository naming should optimize discoverability, automation compatibility, and team clarity.

### Preferred Patterns

- Use kebab-case
- Keep names concise but descriptive
- Include domain or purpose where useful
- Avoid spaces and special characters

### Common Repo Naming Formats

- product-name
- product-name-api
- product-name-web
- org-domain-service
- team-capability-platform

### Examples

Good:
- customer-portal
- customer-portal-api
- customer-portal-web
- payment-reconciliation-service
- fraud-detection-engine

Avoid:
- CustomerPortalRepo
- repo1
- my_project_final_v2
- cool app

### Branch Naming (Related, Optional Standard)

Common patterns:
- feature/add-employee-pagination
- bugfix/fix-null-check-in-service
- hotfix/payment-timeout-issue
- chore/update-dependencies

---

## Cross-Stack Naming Differences by Artifact Type

| Artifact Type | .NET | SQL Server | JavaScript | CSS | Git Repo |
|---------------|------|------------|------------|-----|----------|
| Service/Class | PascalCase | N/A | PascalCase (class) | N/A | N/A |
| Function/Method | PascalCase | Procedure often PascalCase/snake_case | camelCase | N/A | N/A |
| Variable | camelCase | Column often PascalCase/snake_case | camelCase | CSS variable is --kebab-case | N/A |
| File Name | Usually PascalCase.cs | N/A | kebab-case or framework standard | kebab-case.css | kebab-case |
| Identifier Separator | None or leading _ | Optional _ | None | - and __/-- (BEM) | - |

---

## Recommended Team Standard (Practical Enterprise Baseline)

If your stack includes .NET + SQL Server + JS + CSS + Git, a practical baseline is:

1. .NET
- Public symbols: PascalCase
- Private fields: _camelCase
- Parameters/locals: camelCase
- Async methods: Async suffix

2. SQL Server
- Choose PascalCase OR snake_case for all schema objects
- PK: EntityId or Id
- FK: RelatedEntityId (or related_entity_id)
- Constraints/indexes with deterministic prefixes

3. JavaScript/TypeScript
- Variables/functions: camelCase
- Classes/components: PascalCase
- Constants: UPPER_SNAKE_CASE

4. CSS
- Class names: kebab-case
- Prefer BEM or utility-first standard consistently
- Custom properties: --kebab-case

5. Git Repositories
- Repo names: kebab-case
- Short, descriptive, domain-oriented names

---

## Example End-to-End Naming Across Layers

Business concept: customer order approval

- .NET service class: CustomerOrderApprovalService
- .NET method: ApproveOrderAsync
- SQL table (PascalCase): CustomerOrderApproval
- SQL table (snake_case): customer_order_approval
- JavaScript function: approveCustomerOrder
- CSS class: customer-order-approval-form
- Git repo: customer-order-approval-service

---

## Common Naming Mistakes to Avoid

1. Mixing multiple naming conventions inside same layer
2. Using unclear abbreviations (Cfg, Mgr, ProcX)
3. Inconsistent singular/plural naming in database schema
4. Ignoring async suffix conventions in .NET
5. Overly generic CSS class names that create collisions
6. Long or ambiguous repository names
7. Legacy prefixes without value (tbl_, sp_, fn_)

---

## Conclusion

Naming conventions differ by ecosystem because each stack optimized for different history, tooling, and readability norms:

- .NET strongly favors PascalCase for public APIs and structured readability.
- SQL Server requires explicit team standards because conventions vary widely.
- JavaScript favors camelCase for runtime code and PascalCase for classes/components.
- CSS favors kebab-case and structured patterns like BEM to reduce collisions.
- Git repositories favor kebab-case for portability and discoverability.

The most important rule is consistency within each layer, plus a documented cross-stack standard so teams can move between codebases without friction.
