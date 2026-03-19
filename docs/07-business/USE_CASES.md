# DotNet Razor Pages Use Cases

## Document Control
- Document ID: DRP-UC-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Audience: Product, Engineering, QA, Security

## 1. Purpose
This document defines the primary use cases for the DotNet Razor Pages solution and maps user intent to expected system behavior.

## 2. Actors
- Standard User: authenticated user performing employee operations
- Admin User: authenticated user with admin role(s)
- Unauthenticated Visitor: user without an active authenticated session
- System: DotNet Razor Pages application and supporting services

## 3. Preconditions
- Application is running and database is reachable.
- Required configuration is loaded.
- For role-restricted cases, actor has appropriate authentication/authorization state.

## 4. Use Cases

### UC-001: View Employee List
- Primary Actor: Standard User
- Goal: View employee records in a paginated table.
- Trigger: User opens Employees page.
- Main Flow:
  1. User navigates to Employees page.
  2. System loads employee records for requested page.
  3. System displays rows with employee summary data.
- Alternate Flows:
  - A1: No records available -> system displays empty-state message.
- Postconditions:
  - Employee list is visible or empty-state is shown.

### UC-002: Search Employees
- Primary Actor: Standard User
- Goal: Find employees by search term.
- Trigger: User enters search term and submits.
- Main Flow:
  1. User enters a search term.
  2. System applies filter logic.
  3. System returns matching results.
- Alternate Flows:
  - A1: No matches -> empty-state message shown.
- Postconditions:
  - Filtered list corresponds to search criteria.

### UC-003: Sort and Paginate Employees
- Primary Actor: Standard User
- Goal: Reorder and navigate records.
- Trigger: User clicks sort headers or pagination controls.
- Main Flow:
  1. User selects sort column/direction.
  2. System returns sorted records.
  3. User navigates to another page.
  4. System returns records for selected page.
- Postconditions:
  - Results reflect selected sort and page state.

### UC-004: Create Employee
- Primary Actor: Standard User
- Goal: Add a new employee record.
- Trigger: User opens create mode and submits form.
- Main Flow:
  1. User opens employee detail in create mode.
  2. User fills required fields.
  3. User submits create action.
  4. System validates input.
  5. System creates record and redirects to detail page.
- Alternate Flows:
  - A1: Validation fails -> system shows validation errors.
- Postconditions:
  - New employee persists if valid.

### UC-005: Update Employee
- Primary Actor: Standard User
- Goal: Modify an existing employee record.
- Trigger: User edits fields and clicks save.
- Main Flow:
  1. User opens employee detail by id.
  2. User edits fields.
  3. User submits update.
  4. System validates and persists updates.
  5. System redirects with success message.
- Alternate Flows:
  - A1: Employee id not found -> not-found response.
  - A2: Validation fails -> errors shown, no update applied.
- Postconditions:
  - Employee changes persist when valid.

### UC-006: Delete Employee
- Primary Actor: Standard User
- Goal: Remove an employee record.
- Trigger: User clicks delete and confirms.
- Main Flow:
  1. User opens existing employee detail.
  2. User clicks delete.
  3. User confirms deletion.
  4. System deletes record.
  5. System redirects to employee list.
- Alternate Flows:
  - A1: User cancels confirmation -> no deletion.
  - A2: Employee id not found -> not-found response.
- Postconditions:
  - Employee removed when confirmed and found.

### UC-007: Export Employee List as CSV
- Primary Actor: Standard User
- Goal: Download employee list as CSV.
- Trigger: User clicks Export CSV.
- Main Flow:
  1. User requests CSV export.
  2. System retrieves list data using current query context.
  3. System generates CSV content.
  4. System returns downloadable CSV file.
- Postconditions:
  - CSV file is downloaded.

### UC-008: Export Employee List as JSON
- Primary Actor: Standard User
- Goal: Download employee list as JSON.
- Trigger: User clicks Export JSON.
- Main Flow:
  1. User requests JSON export.
  2. System retrieves list data using current query context.
  3. System serializes JSON payload.
  4. System returns downloadable JSON file.
- Postconditions:
  - JSON file is downloaded.

### UC-009: Export Employee Detail as PDF
- Primary Actor: Standard User
- Goal: Download employee detail as PDF.
- Trigger: User clicks Export to PDF on employee detail page.
- Main Flow:
  1. User requests PDF export for a specific employee id.
  2. System loads employee data.
  3. System generates PDF via PDF service.
  4. System returns downloadable PDF file.
- Alternate Flows:
  - A1: Employee id not found -> not-found response.
- Postconditions:
  - PDF file is downloaded when employee exists.

### UC-010: Access Admin Dashboard
- Primary Actor: Admin User
- Goal: Access admin-only functionality.
- Trigger: User navigates to Admin page.
- Main Flow:
  1. Authenticated user requests Admin route.
  2. System evaluates AdminOnly authorization policy.
  3. If authorized, system renders admin dashboard.
- Alternate Flows:
  - A1: Unauthenticated -> redirect to Login.
  - A2: Authenticated but not authorized -> redirect to Access Denied.
- Postconditions:
  - Admin page available only to allowed roles.

### UC-011: Active Directory User Lookup
- Primary Actor: Admin User
- Goal: Lookup a user in Active Directory.
- Trigger: Admin submits username on AD page.
- Main Flow:
  1. Admin opens Active Directory page.
  2. Admin enters username and submits.
  3. System invokes Active Directory service.
  4. System displays user profile details when found.
- Alternate Flows:
  - A1: Username empty -> validation-style message shown.
  - A2: No user found -> no-result message shown.
  - A3: AD/service error -> controlled error message shown.
- Postconditions:
  - Lookup result, no-result, or error state is visible.

### UC-012: Login and Logout
- Primary Actor: Unauthenticated Visitor / Standard User / Admin User
- Goal: Start or end authenticated session.
- Trigger: User submits login form or clicks logout.
- Main Flow (Login):
  1. User opens Login page.
  2. User enters username and role selection (current model).
  3. System creates claims identity and signs in via cookie auth.
  4. System redirects to home page.
- Main Flow (Logout):
  1. Authenticated user initiates logout.
  2. System signs out cookie session.
  3. System redirects to login with logout indicator.
- Alternate Flows:
  - A1: Missing username -> login validation error.
- Postconditions:
  - Session established or terminated accordingly.

## 5. Business Rules Referenced by Use Cases
- Admin access is governed by configured admin roles.
- Employee records require core field validation.
- Export outputs must return valid downloadable file content types.
- Protected routes enforce authentication and authorization behavior.

## 6. Traceability Summary
- Employee lifecycle use cases: UC-001 through UC-006
- Export use cases: UC-007 through UC-009
- Access control use cases: UC-010 and UC-012
- Directory integration use case: UC-011

## 7. Related Documents
- docs/BUSINESS_REQUIREMENTS.md
- docs/REQUIREMENTS.md
- docs/USER_FLOWS.md
- docs/WIREFRAMES.md
- docs/TEST_PLAN.md
