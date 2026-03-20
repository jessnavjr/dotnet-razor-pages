# DotNet Razor Pages Wireframes

## Document Control
- Document ID: DRP-WF-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Fidelity: Low-fidelity (structure and UX intent)

## 1. Purpose
This document provides page-level wireframes for key screens in the DotNet Razor Pages solution. These wireframes are intended to align Product, Design, Engineering, QA, and Security on layout, information hierarchy, and primary user actions.

## 2. Conventions
- `[Primary]` indicates a primary call-to-action
- `[Secondary]` indicates a secondary action
- `(Role: Admin)` indicates role-restricted UI
- `...` indicates repeating rows/items

## 3. Global Layout
Shared top-level layout used by most pages.

```text
+--------------------------------------------------------------------------------+
| BRAND: .NETRazorPages                                  [Employees] [Admin*]   |
|                                                        [User Menu v]           |
+--------------------------------------------------------------------------------+
|                                                                                |
|  Page Content Area                                                             |
|                                                                                |
+--------------------------------------------------------------------------------+
| Footer: Copyright / app info                                                   |
+--------------------------------------------------------------------------------+

* Admin links visible only for roles in Authorization:AdminRoles
```

## 4. Home Page Wireframe

```text
+--------------------------------------------------------------------------------+
| H1: Welcome / Intro                                                            |
|--------------------------------------------------------------------------------|
| Summary cards / informational content                                          |
| [Card] Feature overview                                                        |
| [Card] Quick links                                                             |
| [Card] Notes                                                                   |
|                                                                                |
| [Secondary] Go to Employees                                                    |
+--------------------------------------------------------------------------------+
```

## 5. Employees List Page Wireframe
Path: `/Employees`

```text
+--------------------------------------------------------------------------------+
| H1: Employees                                                                  |
|--------------------------------------------------------------------------------|
| Toolbar:                                                                       |
| [Search input........................................] [Search] [Clear]        |
| [Page Size v] [Export CSV] [Export JSON] [Primary: Create Employee]           |
|--------------------------------------------------------------------------------|
| Table                                                                           |
| [ID^v] [Name^v] [Age^v] [Email^v] [Job Title^v] [Hire Date^v] [Status^v]     |
|--------------------------------------------------------------------------------|
| 101 | Ava Carter    | 5 | ava@...    | Engineering Manager | 2020-03-15 | A   |
| 102 | Noah Nguyen   | 4 | noah@...   | SRE                 | 2021-07-08 | I   |
| ...                                                                            |
|--------------------------------------------------------------------------------|
| Pagination:  < Prev   Page X of Y   Next >                                     |
|                                                                                |
| Empty State (when no results):                                                 |
| "No employees found."                                                         |
+--------------------------------------------------------------------------------+
```

## 6. Employee Detail / Create Page Wireframe
Path: `/EmployeeDetail/{id?}`

```text
+--------------------------------------------------------------------------------+
| H1: Employee Details (or Create Employee)                                      |
|--------------------------------------------------------------------------------|
| [Status message alert area]                                                    |
| [Validation summary area]                                                      |
|                                                                                |
| First Name        [.............................................]              |
| Last Name         [.............................................]              |
| Email             [.............................................]              |
| Job Title         [.............................................]              |
| Hire Date         [........date picker........]                                |
| [ ] Is Active                                                                 |
|                                                                                |
| [Primary: Save/Create] [Delete*] [Export to PDF*] [Back to Employees]         |
+--------------------------------------------------------------------------------+

* Visible when editing an existing employee (not create mode)
```

## 7. Login Page Wireframe
Path: `/Login`

```text
+--------------------------------------------------------------------------------+
| Login Card                                                                     |
|--------------------------------------------------------------------------------|
| Username     [.............................................]                   |
| Select Role  [Admin / SuperAdmin / User / ... v]                              |
|                                                                                |
| [Primary: Login]                                                               |
|                                                                                |
| Info Panel: Demo guidance / role notes                                         |
+--------------------------------------------------------------------------------+
```

Authenticated variant:

```text
+--------------------------------------------------------------------------------+
| Already Logged In                                                              |
| Welcome, {username}                                                            |
| Your Roles: [Admin] [User]                                                     |
| [Primary: Go to Admin Panel or Home] [Secondary: Logout]                       |
+--------------------------------------------------------------------------------+
```

## 8. Access Denied Page Wireframe
Path: `/AccessDenied`

```text
+--------------------------------------------------------------------------------+
| Alert Card: Access Denied                                                      |
|--------------------------------------------------------------------------------|
| Message: You do not have permission to access this resource.                   |
| Current User: {username}                                                       |
| Roles: [badge] [badge]                                                         |
|                                                                                |
| [Primary: Go to Home] [Secondary: Login*]                                      |
+--------------------------------------------------------------------------------+

* Login button shown when unauthenticated
```

## 9. Admin Page Wireframe (Role Restricted)
Path: `/Admin` (Role: Admin)

```text
+--------------------------------------------------------------------------------+
| H1: Admin Dashboard                                                            |
|--------------------------------------------------------------------------------|
| Section: Current User                                                          |
| - Username                                                                      |
| - Role badges                                                                   |
|                                                                                |
| Section: Authorization Status                                                   |
| - Configured Admin Roles                                                        |
| - Access check summary                                                          |
|                                                                                |
| Section: Admin Actions                                                          |
| [Manage AD Lookup] [Other Admin Action]                                        |
+--------------------------------------------------------------------------------+
```

## 10. Active Directory Page Wireframe (Role Restricted)
Path: `/Admin/ActiveDirectory` (Role: Admin)

```text
+--------------------------------------------------------------------------------+
| H1: Active Directory Lookup                                                    |
|--------------------------------------------------------------------------------|
| Search User                                                                     |
| SamAccountName / Email [....................................] [Lookup]         |
|                                                                                |
| Result Panel                                                                    |
| - Display Name                                                                  |
| - Username                                                                      |
| - Email                                                                         |
| - Department / Title                                                            |
| - Status                                                                         |
|                                                                                |
| [Secondary: Back to Admin]                                                      |
+--------------------------------------------------------------------------------+
```

## 11. Interaction Notes
- Employees table headers toggle sorting direction.
- Search and paging state are preserved in query string.
- Export actions return downloadable files (CSV, JSON, PDF).
- Employee save button can be disabled until changes are detected.
- Authorization controls hide/show navigation and protect routes server-side.

## 12. Responsive Layout Notes
- Navigation collapses on small viewports.
- Employee table should use responsive wrapping/scroll behavior.
- Toolbar actions should stack into rows on mobile.
- Primary actions remain visible without horizontal overflow.

## 13. Future High-Fidelity Enhancements
- Add visual component states (hover, focus, disabled, validation error).
- Add accessibility annotations (tab order, landmarks, ARIA role expectations).
- Add redline spacing and typography specs for design handoff.
