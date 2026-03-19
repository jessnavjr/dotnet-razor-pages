# DotNet Razor Pages Security Summary and Review

## Document Control
- Document ID: DRP-SEC-001
- Version: 1.0
- Date: 2026-03-18
- Status: Draft
- Review Type: Architecture and code configuration review (non-penetration test)

## 1. Executive Summary
This review found that core security controls are present (authentication middleware, role-based authorization policy, HTTPS redirection, and protected admin routes), but there are several high-priority hardening gaps that must be addressed before production deployment.

Overall posture: **Moderate Risk (not production-ready without remediation).**

Key concerns:
- Development credentials and insecure connection settings are present in configuration defaults.
- Test-oriented authentication flow allows user-selected role claims.
- Sensitive error details can be surfaced from Active Directory exceptions.
- Employee CRUD/export routes appear accessible without explicit authorization policy.

## 2. Scope and Method
### Scope
- Web authentication and authorization setup
- Configuration and secret handling patterns
- Data export and admin route access patterns
- Active Directory integration handling

### Method
- Static review of solution code and config
- No dynamic penetration test or infrastructure scan performed

## 3. Security Strengths
- Authentication pipeline configured with cookie auth middleware.
- Policy-based admin authorization (`AdminOnly`) implemented.
- Admin and Active Directory page models are protected with `[Authorize(Policy = "AdminOnly")]`.
- HTTPS redirection enabled; HSTS enabled outside development.
- Input validation attributes exist on employee form fields.
- Layered architecture supports future security control centralization.

## 4. Findings and Risk Ratings

| ID | Severity | Finding | Evidence | Impact |
|---|---|---|---|---|
| SEC-001 | High | Development credentials and DB password in source-controlled config | `appsettings.json` connection string includes SQL credentials | Credential disclosure and environment compromise risk |
| SEC-002 | High | DB transport security is relaxed (`Encrypt=False`, `TrustServerCertificate=True`) | `appsettings.json` connection string | Potential MITM and confidentiality/integrity risk |
| SEC-003 | High | Test-style login allows user-selected role claims | `Pages/Auth/Login.cshtml.cs` role selected from UI and issued in auth cookie | Privilege escalation risk if used beyond controlled test context |
| SEC-004 | Medium | AD bind credential placeholders in appsettings and SSL default disabled | `appsettings.json` ActiveDirectory settings | Credential hygiene risk and weaker directory transport security |
| SEC-005 | Medium | Active Directory exception details are returned to end users | `Pages/Admin/ActiveDirectory.cshtml.cs` includes exception message text in UI | Information disclosure (internal host/AD details) |
| SEC-006 | Medium | Employee CRUD and export page models do not show explicit authorization attributes | `Pages/Employees/EmployeeDetail.cshtml.cs`, `Pages/Employees/Employees.cshtml.cs` | Potential unauthorized access to employee records and exports |
| SEC-007 | Low | Cookie security attributes are not explicitly hardened in configuration | `Program.cs` cookie options set login/access denied/expiry only | Reliance on defaults may lead to policy drift across environments |

## 5. Detailed Recommendations

### SEC-001 and SEC-002: Configuration and Transport Hardening (High)
- Remove plaintext secrets from source-controlled `appsettings.json`.
- Use environment variables and managed secret stores for all credentials.
- Enforce encrypted SQL connections for non-local environments.
- Remove `TrustServerCertificate=True` in production unless formally approved with compensating controls.

### SEC-003: Replace Test Authentication for Production (High)
- Disable role self-selection in production paths.
- Integrate enterprise identity provider (OIDC/Entra ID/ADFS).
- Map roles/groups from trusted identity claims, not user-submitted form input.

### SEC-004: Active Directory Security (Medium)
- Store bind credentials in a secure secret manager.
- Enable secure directory transport (LDAPS) and certificate validation.
- Review minimum privilege for AD bind account.

### SEC-005: Error Message Sanitization (Medium)
- Return generic user-facing error messages.
- Log detailed exception info server-side with restricted access.
- Add structured logging fields for correlation and triage.

### SEC-006: Data Access Authorization (Medium)
- Confirm intended access model for employee pages.
- If authenticated-only access is intended, add `[Authorize]` on employee list/detail page models.
- If role-specific access is required, introduce policy-based restrictions for export and mutation handlers.

### SEC-007: Cookie Policy Explicitness (Low)
- Set explicit cookie security options:
  - `Cookie.HttpOnly = true`
  - `Cookie.SecurePolicy = Always` (production)
  - `Cookie.SameSite = Lax/Strict` per UX requirements
  - Sliding expiration and session management policy as needed

## 6. Remediation Plan (Suggested)

### 0-14 Days (Critical Baseline)
- Remove committed credentials and rotate exposed secrets.
- Enforce secure SQL transport for non-dev environments.
- Disable or isolate test login behavior from production deployment paths.

### 15-30 Days (Policy and Data Protection)
- Add explicit authorization strategy for employee routes and export handlers.
- Sanitize AD error messages and add secure server-side diagnostics.
- Move AD bind credentials to secret management.

### 31-60 Days (Enterprise Hardening)
- Complete enterprise IdP integration and role/group mapping.
- Add security telemetry dashboards and alerting.
- Perform threat modeling and targeted security testing.

## 7. Validation Checklist
- [ ] No plaintext credentials in source-controlled configuration
- [ ] Production SQL connections enforce encryption and certificate validation
- [ ] Test auth flow disabled outside development/test environments
- [ ] Employee routes and exports align with intended authorization model
- [ ] AD integration uses secure transport and sanitized user errors
- [ ] Cookie policy security options explicitly configured
- [ ] Security logging and alerting baselines established

## 8. Residual Risk Statement
Until high-severity findings SEC-001, SEC-002, and SEC-003 are remediated, the application should be treated as **non-production** from a security standpoint.

## 9. References
- `DotNetRazorPages.Web/Program.cs`
- `DotNetRazorPages.Web/appsettings.json`
- `DotNetRazorPages.Web/Pages/Auth/Login.cshtml.cs`
- `DotNetRazorPages.Web/Pages/Admin/Admin.cshtml.cs`
- `DotNetRazorPages.Web/Pages/Admin/ActiveDirectory.cshtml.cs`
- `DotNetRazorPages.Web/Pages/Employees/Employees.cshtml.cs`
- `DotNetRazorPages.Web/Pages/Employees/EmployeeDetail.cshtml.cs`
