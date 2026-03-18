# Admin Role Page Implementation

This document describes the role-based access control (RBAC) implementation for the admin page.

## Overview

The application now includes:
- **Admin Page** (`/Admin`) - Accessible only to users with admin roles
- **Login Page** (`/Login`) - Simple authentication with role selection
- **Access Denied Page** (`/AccessDenied`) - Displayed when unauthorized access is attempted
- **Configurable Roles** - Admin roles defined in `appsettings.json`

## Configuration

All authorization settings are configurable in `appsettings.json`:

```json
"Authorization": {
  "AdminRoles": ["Admin", "SuperAdmin"],
  "EnableTestAuthentication": true,
  "TestUserRole": "User"
}
```

### Configuration Properties

| Property | Description | Default |
|----------|-------------|---------|
| `AdminRoles` | Array of roles that have admin access | `["Admin", "SuperAdmin"]` |
| `EnableTestAuthentication` | Enable test login endpoint | `true` |
| `TestUserRole` | Default role for new test users | `"User"` |

## How to Use

### 1. Login

1. Navigate to `/Login`
2. Enter any username
3. Select a role from the dropdown (or leave blank for default role)
4. Click **Login**

### 2. Access Admin Panel

- If logged in with an admin role (Admin, SuperAdmin), click the **Admin** link in the top navigation
- The admin panel shows:
  - Current user information
  - User's assigned roles
  - Configured admin roles
  - Authorization status
  - Available admin actions

### 3. Logout

- Click the user dropdown menu in the top-right corner
- Click **Logout**

### 4. Access Control

- Unauthenticated users accessing `/Admin` are redirected to `/Login`
- Authenticated users without admin roles are redirected to `/AccessDenied`
- Only users with roles in `Authorization:AdminRoles` can access the admin panel

## Implementation Details

### Authentication

- Uses **ASP.NET Core Cookie Authentication**
- Sets up a 7-day persistent session
- Creates claims-based identity with role information

### Authorization

- Uses **Authorization Policies** with the `AdminOnly` policy
- Policy requires at least one role from `Authorization:AdminRoles`
- Applied via `[Authorize(Policy = "AdminOnly")]` attribute on Admin page

### Pages

| Page | Route | Protected | Description |
|------|-------|-----------|-------------|
| Admin | `/Admin` | Yes | Admin panel for authorized users |
| Login | `/Login` | No | User login and authentication |
| AccessDenied | `/AccessDenied` | No | Error page for unauthorized access |

### Code Locations

- **Authorization Configuration**: `Program.cs` (lines 14-25)
- **Admin Page Model**: `Pages/Admin.cshtml.cs`
- **Admin Page View**: `Pages/Admin.cshtml`
- **Login Page Model**: `Pages/Login.cshtml.cs`
- **Login Page View**: `Pages/Login.cshtml`
- **Navigation Updates**: `Pages/Shared/_Layout.cshtml`

## Customization

### Change Admin Roles

Edit `appsettings.json`:
```json
"Authorization": {
  "AdminRoles": ["SuperUser", "Manager", "SystemAdmin"]
}
```

### Add More Role-Based Pages

1. Create a new page model with `[Authorize(Roles = "RoleName")]`
2. Or create a new policy in `Program.cs` and use `[Authorize(Policy = "PolicyName")]`

Example:
```csharp
[Authorize(Roles = "Moderator")]
public class ModerationPanelModel : PageModel { }
```

### Implement Persistent Authentication

For production, implement a proper authentication provider (Azure AD, Identity Provider, etc.) instead of the test cookie authentication.

## Testing

The implementation includes a functional test that verifies:
- Unauthenticated users cannot access admin page
- Users without admin roles cannot access admin page
- Users with admin roles can access admin page

Run tests with:
```bash
dotnet test DotNetRazorPages.Tests -c Debug
```

## Security Notes

⚠️ **Important**: The test authentication implementation is for **development only**.

For production:
- Implement proper authentication (OAuth, OIDC, Azure AD, etc.)
- Never use simple cookie authentication for real scenarios
- Enforce HTTPS in all environments
- Implement proper role management and persistence
- Consider using ASP.NET Core Identity for user/role management
