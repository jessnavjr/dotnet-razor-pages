# Admin Role-Based Access Control - Quick Reference

## Features Implemented

✅ **Role-Based Authorization** - Admin panel accessible only to users with admin roles  
✅ **Configurable Roles** - Define allowed admin roles in `appsettings.json`  
✅ **Authentication Pages** - Login, Access Denied, and Admin panel pages  
✅ **User Navigation** - Admin link appears in navbar for authorized users  
✅ **Cookie Authentication** - Secure session management with 7-day persistence  
✅ **Functional Tests** - Comprehensive tests for authorization flow

## Quick Start

### 1. Access the Application

```
http://localhost:5230
```

### 2. Login

Click the **Login** button in the top-right corner and:
- Enter any username
- Select a role (Admin, SuperAdmin, or User)
- Submit the form

### 3. Access Admin Panel

If you logged in with an admin role:
- Click the **Admin** link in the top navigation (red, with shield icon)
- Or navigate directly to `/Admin`

### 4. Logout

Click your username dropdown and select **Logout**

## Configuration

Edit `appsettings.json` to customize roles:

```json
{
  "Authorization": {
    "AdminRoles": ["Admin", "SuperAdmin"],
    "EnableTestAuthentication": true,
    "TestUserRole": "User"
  }
}
```

### Configuration Options

- **AdminRoles** - Array of roles that can access the admin panel
- **EnableTestAuthentication** - Enable/disable the test login form
- **TestUserRole** - Default role when no selection is made

## API Usage

### Protect a Page with Admin Role

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "AdminOnly")]
public class MyAdminPageModel : PageModel 
{
    public void OnGet() { }
}
```

### Protect with Specific Roles

```csharp
[Authorize(Roles = "Admin,SuperAdmin")]
public class MyRolePageModel : PageModel { }
```

### Check Authorization in Code

```csharp
if (User.Identity?.IsAuthenticated == true)
{
    var isAdmin = User.FindAll("role")
        .Any(c => c.Value == "Admin" || c.Value == "SuperAdmin");
}
```

### Check Authorization in Razor Views

```html
@if (User.Identity?.IsAuthenticated == true)
{
    var roles = User.FindAll("role").Select(c => c.Value);
    @foreach (var role in roles)
    {
        <span class="badge">@role</span>
    }
}
```

## File Structure

| File | Purpose |
|------|---------|
| `Pages/Admin.cshtml.cs` | Admin page protected with authorization |
| `Pages/Admin.cshtml` | Admin panel UI |
| `Pages/Login.cshtml.cs` | Login handler with role assignment |
| `Pages/Login.cshtml` | Login form |
| `Pages/AccessDenied.cshtml.cs` | Access denied page model |
| `Pages/AccessDenied.cshtml` | Access denied UI |
| `Program.cs` | Auth middleware configuration |
| `appsettings.json` | Authorization settings |
| `Pages/Shared/_Layout.cshtml` | Navigation with user menu |

## Authentication Flow

```
1. User visits /Admin
   ↓
2. Check if authenticated
   - NO → Redirect to /Login
   - YES → Check user roles
   ↓
3. Check if user has admin role
   - YES → Load /Admin
   - NO → Redirect to /AccessDenied
```

## Testing

Run functional tests:

```bash
dotnet test DotNetRazorPages.Tests -c Debug
```

Tests verify:
- ✅ Unauthenticated access redirects to login
- ✅ Login page displays role options
- ✅ Access denied page displays correctly
- ✅ All core functionality still works (48/48 tests passing)

## Production Considerations

⚠️ **Important**: This implementation uses test cookie authentication for development.

For production, integrate with:
- Azure AD / Microsoft Entra ID
- OAuth 2.0 / OpenID Connect provider
- ASP.NET Core Identity
- SAML 2.0

Replace test authentication with a production identity provider in `Program.cs`.

## Troubleshooting

**Q: Admin link doesn't appear in navigation**  
A: Log in with an admin role (Admin or SuperAdmin by default)

**Q: Can't access admin panel after login**  
A: Check that your role is in `Authorization:AdminRoles` in appsettings.json

**Q: Need to add a new admin role**  
A: Update `appsettings.json` and add role name to `AdminRoles` array

## Next Steps

1. **Integrate Real Identity Provider** - Replace test auth with production provider
2. **Add User Management** - Implement user/role persistence in database
3. **Add More Protected Pages** - Create additional pages with different role requirements
4. **Audit Logging** - Log admin actions and access attempts
5. **Two-Factor Authentication** - Add 2FA for enhanced security
