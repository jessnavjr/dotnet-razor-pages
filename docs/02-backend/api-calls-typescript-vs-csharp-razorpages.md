# API Calls in ASP.NET Core Razor Pages: TypeScript vs. Alternatives

## Overview

ASP.NET Core Razor Pages developers face a critical architectural decision: where should API calls happen? Should data fetching occur in TypeScript on the client, in C# on the server, or use a hybrid approach?

This document compares making API calls in TypeScript versus alternative approaches in ASP.NET Core Razor Pages applications, helping teams choose the right pattern for their use case.

---

## Context: Razor Pages Architecture

### Traditional Razor Pages Flow

```
HTTP Request
    ↓
[Page Handler (.cs)] ← Server-side C#
    ├─ OnGet() / OnPost()
    ├─ Call dependencies/services
    ├─ Fetch data from API or database
    ├─ Populate PageModel
    └─ Return PageResult
    ↓
[Razor Template (.cshtml)] ← Server-side template
    ├─ Access PageModel.Data
    ├─ Render HTML
    ├─ Include inline JavaScript/TypeScript
    └─ Include TypeScript bundles
    ↓
[Rendered HTML] ↓ Browser
    ├─ Executes JavaScript/TypeScript
    ├─ May make additional XHR/Fetch calls
    └─ Enhances interactivity
    ↓
Display to User
```

### Three API Call Strategies

1. **Server-side in Page Handler (C#)**
2. **Client-side in TypeScript**
3. **Hybrid (both with specific responsibilities)**

---

## 1. Client-Side API Calls in TypeScript

### What It Means

Making API calls directly from TypeScript code in the browser, after the page has loaded or in response to user interaction.

### Architecture

```
Browser (TypeScript)
    ↓
Fetch/XHR Request
    ↓
ASP.NET Core API Endpoint
    ↓
API Returns JSON
    ↓
TypeScript processes response
    ↓
Update DOM/State
```

### Implementation Example

```typescript
// wwwroot/ts/services/employeeService.ts
export class EmployeeService {
  private baseUrl = '/api';

  async getEmployees(): Promise<Employee[]> {
    const response = await fetch(`${this.baseUrl}/employees`);
    if (!response.ok) throw new Error('Failed to fetch employees');
    return response.json();
  }

  async saveEmployee(employee: Employee): Promise<Employee> {
    const response = await fetch(`${this.baseUrl}/employees`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(employee),
      credentials: 'include' // CSRF protection
    });
    if (!response.ok) throw new Error('Failed to save employee');
    return response.json();
  }
}

// wwwroot/ts/pages/employees.ts
import { EmployeeService } from '../services/employeeService';

export class EmployeesPage {
  private service = new EmployeeService();

  async initialize(): Promise<void> {
    try {
      const employees = await this.service.getEmployees();
      this.renderEmployees(employees);
    } catch (error) {
      console.error('Failed to load employees:', error);
      this.showError('Unable to load employees');
    }
  }

  private renderEmployees(employees: Employee[]): void {
    const container = document.getElementById('employees-container');
    if (!container) return;

    container.innerHTML = employees
      .map(emp => `
        <div class="employee-card">
          <h3>${emp.name}</h3>
          <p>${emp.email}</p>
        </div>
      `)
      .join('');
  }

  private showError(message: string): void {
    const error = document.getElementById('error-message');
    if (error) error.textContent = message;
  }
}

// In Razor Page (.cshtml)
// <script src="/js/employees.bundle.js"></script>
// <script>
//   document.addEventListener('DOMContentLoaded', () => {
//     const page = new EmployeesPage();
//     page.initialize();
//   });
// </script>
```

### Pros of Client-Side TypeScript API Calls

**Performance:**
- ✓ **Faster initial page load**: Server doesn't wait for API calls; sends HTML immediately
- ✓ **Parallel requests**: Multiple TypeScript API calls happen simultaneously in browser
- ✓ **Caching friendly**: Browser can cache API responses, reduces server load
- ✓ **Reduced server load**: API calls don't block server resources

**User Experience:**
- ✓ **Progressive loading**: Page shows skeleton/placeholder while data loads
- ✓ **Responsive interface**: Don't wait for full page to load before user can interact
- ✓ **Partial page updates**: Update only relevant section without full page refresh

**Developer Experience:**
- ✓ **Clear separation**: Frontend and backend concerns clearly separated
- ✓ **Frontend independence**: Frontend can be developed/deployed independently
- ✓ **Testability**: TypeScript code easily unit tested (no Razor dependency)
- ✓ **Reusability**: API services used across multiple pages/components
- ✓ **Modern SPA patterns**: Easier to implement SPA-like behavior

**Technical:**
- ✓ **Decoupled architecture**: Encourages API-first backend design
- ✓ **Framework agnostic**: Could swap Razor Pages for React/Vue later, API stays same
- ✓ **CORS consideration**: Forces thinking about API access patterns
- ✓ **Lower latency for updates**: Only sends data user actually needs

### Cons of Client-Side TypeScript API Calls

**Security:**
- ✗ **CSRF complexity**: Must handle CSRF tokens in every XHR request
- ✗ **API exposure**: API endpoints are visible to client (less sensitive data exposure risk)
- ✗ **Authentication tokens in browser**: Tokens must be stored securely (localStorage risks)
- ✗ **Client-side validation only initially**: Must duplicate validation on server anyway

**Performance:**
- ✗ **Slower for data-heavy pages**: Multiple round trips instead of single server load
- ✗ **Waterfall delays**: Initial HTML loads, then TypeScript loads, then API calls start
- ✗ **More network requests**: Separate requests for HTML, JavaScript, API data
- ✗ **Mobile penalty**: More requests = worse on 3G/poor connections
- ✗ **JavaScript parse time**: Must parse/execute TypeScript bundle before data loads

**User Experience:**
- ✗ **Flash of unstyled content**: Users see skeleton/loading states initially
- ✗ **Link prefetching less effective**: Hard to prefetch data-dependent content
- ✗ **SEO challenges**: Search engines see page structure without data
- ✗ **JavaScript required**: Page non-functional without JavaScript
- ✗ **Progressive enhancement difficult**: Data-heavy pages hard to degrade gracefully

**Developer Experience:**
- ✗ **Duplicate logic**: Validation logic in both TypeScript and server
- ✗ **Error handling complexity**: Handle network errors, timeouts, API errors in TypeScript
- ✗ **State management**: Must manage client-side state (loading, error, data)
- ✗ **Debugging difficulty**: Harder to debug when data crosses network boundary
- ✗ **Bundle size**: Additional TypeScript increases bundle (though tree-shaken)

**Technical:**
- ✗ **CORS required**: Same-origin or CORS configuration needed
- ✗ **API versioning**: Must maintain API compatibility
- ✗ **Error recovery**: Connection drops, timeouts require client-side handling
- ✗ **Rate limiting**: Client-side rate limits less reliable
- ✗ **ORM N+1 problems**: Harder to optimize for multiple API calls

### When Client-Side TypeScript Calls Work Well

- ✓ Page with frequently updated data (real-time dashboards)
- ✓ User-initiated searches or filters (load on demand)
- ✓ SPA-like single page with rich interactivity
- ✓ Multiple independent data sources on same page
- ✓ Gradual page enhancement (page works without data initially)
- ✓ API shared across multiple pages/apps
- ✓ Data doesn't affect initial page render or SEO
- ✓ Users on reasonably fast connections
- ✓ Team comfortable with TypeScript/SPA patterns

### Example: When Client-Side Works

```typescript
// Good use case: Search with progressive loading
export class EmployeeSearchPage {
  private service = new EmployeeService();
  private searchInput = document.getElementById('search') as HTMLInputElement;
  private resultsContainer = document.getElementById('results');

  initialize(): void {
    this.searchInput.addEventListener('input', (e) => {
      const query = (e.target as HTMLInputElement).value;
      if (query.length > 2) {
        this.performSearch(query);
      } else {
        this.clearResults();
      }
    });
  }

  private async performSearch(query: string): Promise<void> {
    this.showLoading();
    try {
      const results = await this.service.searchEmployees(query);
      this.displayResults(results);
    } catch (error) {
      this.showError('Search failed');
    }
  }

  private showLoading(): void {
    this.resultsContainer!.innerHTML = '<p>Searching...</p>';
  }

  private displayResults(employees: Employee[]): void {
    // Render results
  }

  private clearResults(): void {
    this.resultsContainer!.innerHTML = '';
  }
}
```

---

## 2. Server-Side API Calls in C# (Page Handlers)

### What It Means

Page handlers (OnGet, OnPost) make API calls from C#, populate the PageModel, and pass data to Razor template during initial page render.

### Architecture

```
HTTP Request
    ↓
[Page Handler (C#)]
    ├─ OnGet() calls API
    ├─ Receives JSON response
    ├─ Populates PageModel
    └─ Returns PageResult
    ↓
[Razor Template]
    ├─ Accesses PageModel data directly (no API call)
    └─ Renders HTML with data
    ↓
[Rendered HTML with data]
    ↓
Browser receives complete page
```

### Implementation Example

```csharp
// Pages/Employees.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmployeesPageModel : PageModel
{
    private readonly IEmployeeApiClient _apiClient;
    private readonly ILogger<EmployeesPageModel> _logger;

    public List<EmployeeDto> Employees { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public EmployeesPageModel(IEmployeeApiClient apiClient, ILogger<EmployeesPageModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Employees = await _apiClient.GetEmployeesAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch employees");
            ErrorMessage = "Unable to load employees. Please try again later.";
            Employees = new();
        }
    }
}

// Services/IEmployeeApiClient.cs
public interface IEmployeeApiClient
{
    Task<List<EmployeeDto>> GetEmployeesAsync();
    Task<EmployeeDto> SaveEmployeeAsync(EmployeeDto employee);
}

// Services/EmployeeApiClient.cs
public class EmployeeApiClient : IEmployeeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public EmployeeApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        var apiUrl = _config["ApiSettings:EmployeesEndpoint"];
        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        // Deserialize and return
        return await response.Content.ReadAsAsync<List<EmployeeDto>>();
    }

    public async Task<EmployeeDto> SaveEmployeeAsync(EmployeeDto employee)
    {
        var apiUrl = _config["ApiSettings:EmployeesEndpoint"];
        var content = new StringContent(
            JsonSerializer.Serialize(employee),
            Encoding.UTF8,
            "application/json"
        );
        var response = await _httpClient.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<EmployeeDto>();
    }
}
```

```html
<!-- Pages/Employees.cshtml -->
@page
@model EmployeesPageModel

@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <div class="alert alert-danger">@Model.ErrorMessage</div>
}

<div id="employees-container">
    @if (Model.Employees.Any())
    {
        <div class="employee-grid">
            @foreach (var employee in Model.Employees)
            {
                <div class="employee-card">
                    <h3>@employee.Name</h3>
                    <p>@employee.Email</p>
                </div>
            }
        </div>
    }
    else
    {
        <p>No employees found.</p>
    }
</div>
```

### Pros of Server-Side C# API Calls

**Performance:**
- ✓ **Single network round trip**: Single HTTP request returns complete page with data
- ✓ **No JavaScript parsing**: Browser renders immediately without executing TypeScript
- ✓ **Optimal for initial load**: Faster perceived performance on first page view
- ✓ **Better on slow networks**: 3G/poor connection speeds handle single request better
- ✓ **No waterfall delays**: HTML + data arrives together

**User Experience:**
- ✓ **Instant content**: Users see fully-rendered page immediately
- ✓ **No skeleton/loading states**: Page appears instantly
- ✓ **Progressive enhancement**: Page works without JavaScript
- ✓ **Better SEO**: Search engines see complete page with data
- ✓ **Graceful degradation**: Works on older browsers without JavaScript

**Security:**
- ✓ **CSRF automatic**: Razor Pages built-in CSRF protection applies to all requests
- ✓ **No client-side tokens**: Authentication handled server-side, no token storage concerns
- ✓ **Data not exposed to client code**: API responses handled in C#, never touch browser
- ✓ **Easier authorization**: Enforce permissions before returning data
- ✓ **Less verbose security**: No need to handle auth tokens in TypeScript

**Developer Experience:**
- ✓ **Central logic**: All application logic in C#, easier to maintain
- ✓ **Single language**: Developers don't need TypeScript expertise
- ✓ **IDE support**: Full IntelliSense and debugging in Visual Studio
- ✓ **Reusable services**: Shared services between API clients and direct use
- ✓ **Error handling centralized**: Logging, error handling in one place

**Technical:**
- ✓ **No CORS issues**: Same-origin policy simplified
- ✓ **Easier testing**: Test Page handlers like any other C# code
- ✓ **Caching friendly**: Server-side caching easier to reason about
- ✓ **Transaction support**: Database transactions wrap entire request
- ✓ **Database optimization**: Can optimize queries for exact page needs (N+1 prevention)

### Cons of Server-Side C# API Calls

**Performance:**
- ✗ **Server must wait**: Request blocks until external API responds
- ✗ **Server-side latency**: If internal system is slow, page load slow
- ✗ **Connection pool exhaustion**: Long-running requests can exhaust server resources
- ✗ **Cascading failures**: One slow API blocks entire page load
- ✗ **No partial content**: Must wait for all data before rendering

**Scalability:**
- ✗ **Server load**: Server must manage API call load + serving requests
- ✗ **Memory usage**: Server maintains open connections during API calls
- ✗ **Harder to scale**: Each request ties up server thread
- ✗ **Limited by server capacity**: Server becomes bottleneck

**User Experience:**
- ✗ **No progressive loading**: Users see nothing until all data loads
- ✗ **Slower for secondary data**: All data loads before any content displayed
- ✗ **Timeout risk**: Long API calls can exceed request timeout
- ✗ **All-or-nothing**: Page fails if any required API call fails
- ✗ **Poor for real-time**: Difficult to implement live data updates

**Developer Experience:**
- ✗ **Tight coupling**: Page depends on specific API structure
- ✗ **Harder to test**: Must mock external API calls in tests
- ✗ **Limited frontend independence**: Backend must know what frontend needs
- ✗ **Difficult to parallelize**: Multiple API calls happen sequentially
- ✗ **Client-side enhancement challenging**: Hard to add dynamic features

**Technical:**
- ✗ **API versioning tight**: Changes to API structure affect Razor Page model
- ✗ **Debugging cross-boundary calls**: Harder to trace request/response chains
- ✗ **Error recovery**: Page fails completely if API errors
- ✗ **Requires HTTP client setup**: Must configure named HttpClient instances
- ✗ **Async/await complexity**: Async operations throughout request pipeline

### When Server-Side C# Calls Work Well

- ✓ Traditional multi-page applications
- ✓ SEO-critical pages (blog posts, marketing content)
- ✓ Initial page load critical (first impression)
- ✓ Data required for page to render meaningfully
- ✓ Progressive enhancement important
- ✓ Mobile-first with slow connection support
- ✓ Team not comfortable with TypeScript
- ✓ Small data sets, fast API responses
- ✓ Page-specific data (not shared across many pages)

### Example: When Server-Side Works

```csharp
// Good use case: Blog post detail page
public class PostDetailsPageModel : PageModel
{
    private readonly IPostApiClient _apiClient;

    public PostDto Post { get; set; }
    public List<CommentDto> Comments { get; set; }

    public PostDetailsPageModel(IPostApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnGetAsync(int id)
    {
        // Load all data server-side before rendering
        Post = await _apiClient.GetPostAsync(id);
        Comments = await _apiClient.GetCommentsAsync(id);
        // Page renders with complete data
    }
}
```

---

## 3. Hybrid Approach: Server + Client

### What It Means

Server loads critical data in Page Handler, then TypeScript enhances with additional requests for secondary data or real-time updates.

### Architecture

```
Initial Request
    ↓
[Server Page Handler]
    ├─ Loads critical data (posts, users)
    └─ Returns PageResult
    ↓
[Razor Template]
    ├─ Renders critical content immediately
    └─ Loads TypeScript bundle
    ↓
[Browser loads TypeScript]
    ├─ TypeScript makes additional API calls
    ├─ Loads secondary data (comments, recommendations)
    ├─ Fetches live updates (real-time activity)
    └─ Updates relevant DOM sections
```

### Implementation Example

```csharp
// Pages/PostDetails.cshtml.cs
public class PostDetailsPageModel : PageModel
{
    private readonly IPostApiClient _apiClient;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public PostDto Post { get; set; } // Critical data, loaded server-side

    public PostDetailsPageModel(IPostApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnGetAsync()
    {
        // Load critical content for SEO and initial render
        Post = await _apiClient.GetPostAsync(Id);
        // Don't load comments server-side (handled by TypeScript)
    }
}
```

```html
<!-- Pages/PostDetails.cshtml -->
@page
@model PostDetailsPageModel

<article>
    <h1>@Model.Post.Title</h1>
    <p>@Model.Post.Content</p>
</article>

<!-- Comments loaded client-side -->
<section id="comments-container">
    <p>Loading comments...</p>
</section>

<script src="/js/post-details.bundle.js"></script>
<script>
    document.addEventListener('DOMContentLoaded', () => {
        const page = new PostDetailsPage(@Model.Id);
        page.initialize();
    });
</script>
```

```typescript
// wwwroot/ts/pages/post-details.ts
export class PostDetailsPage {
  private postId: number;
  private commentService = new CommentService();

  constructor(postId: number) {
    this.postId = postId;
  }

  async initialize(): Promise<void> {
    await this.loadComments();
    this.setupRealtimeUpdates();
  }

  private async loadComments(): Promise<void> {
    try {
      const comments = await this.commentService.getComments(this.postId);
      this.renderComments(comments);
    } catch {
      console.error('Failed to load comments');
    }
  }

  private setupRealtimeUpdates(): void {
    // WebSocket connection for real-time comment updates
    const ws = new WebSocket(`wss://${window.location.host}/comments/${this.postId}`);
    ws.onmessage = (event) => {
      const comment = JSON.parse(event.data);
      this.addCommentToDOM(comment);
    };
  }

  private renderComments(comments: CommentDto[]): void {
    const container = document.getElementById('comments-container');
    if (container) {
      container.innerHTML = comments
        .map(c => `<div class="comment"><p>${c.text}</p></div>`)
        .join('');
    }
  }

  private addCommentToDOM(comment: CommentDto): void {
    const container = document.getElementById('comments-container');
    const newComment = document.createElement('div');
    newComment.className = 'comment';
    newComment.innerHTML = `<p>${comment.text}</p>`;
    container?.appendChild(newComment);
  }
}
```

### Pros of Hybrid Approach

- ✓ **Best of both**: Server optimized for initial load, client for interactivity
- ✓ **Progressive enhancement**: Core content works without JavaScript
- ✓ **SEO + interaction**: Search engines see critical data, users get dynamic features
- ✓ **Resilient**: If secondary data fails, page still works with critical content
- ✓ **Performance**: Initial load fast (critical data only), then enhance
- ✓ **Scalability**: Server not burdened with all data, can parallelize with client
- ✓ **Real-time capable**: Secondary data can update via WebSocket
- ✓ **Flexible**: Can move responsibilityalong spectrum as needs change

### Cons of Hybrid Approach

- ✗ **Complexity**: Must manage both server + client data flow
- ✗ **Synchronization**: Risk of data inconsistency between server and client
- ✗ **Duplicate logic**: May repeat data fetching/validation logic
- ✗ **Harder to debug**: Data comes from multiple sources
- ✗ **Team coordination**: Server and frontend developers must coordinate
- ✗ **Testing complexity**: Must test both paths (with and without JavaScript)
- ✗ **UI inconsistency**: Page looks different during load (content appears incrementally)

### When Hybrid Works Best

- ✓ Content-heavy page with comments/related items
- ✓ Dashboard with critical metrics + secondary data
- ✓ Real-time features on content-focused page
- ✓ SEO + rich interactivity both important
- ✓ Progressive loading acceptable/desirable
- ✓ Independent data sources
- ✓ Graceful degradation required

---

## Comparison Matrix

| Aspect | Client-Side TypeScript | Server-Side C# | Hybrid |
|--------|----------------------|----------------|--------|
| **Initial Load Speed** | Slower (JS parse + API calls) | Faster (single round trip) | Good (critical data fast) |
| **CSRF Protection** | Manual (tokens in JS) | Automatic (Razor built-in) | Automatic + JS handling |
| **SEO** | Poor (no data initially) | Good (full content) | Good (critical content) |
| **Progressive Enhancement** | Hard (JS required) | Easy (works without JS) | Good (core works without JS) |
| **Real-time Updates** | Easy (client controls) | Harder (server must push) | Natural (client can listen) |
| **Server Load** | Lower (API calls only) | Higher (manages requests) | Medium (balanced) |
| **Security Complexity** | Higher (token management) | Lower (server handles) | Medium |
| **Developer Experience** | TypeScript required | C# only | Both skills needed |
| **Data Sharing** | Across many pages | Page-specific | Flexible |
| **Error Recovery** | Client-side retry needed | Automatic page reload | Flexible |
| **Caching** | Browser cache | Server cache | Both |
| **CORS Required** | Yes (same-origin policy) | No | Partial |
| **Testing Ease** | Easy (mock APIs) | Moderate (mock HTTP) | Complex |
| **Scalability** | Better (less server work) | Challenging (server busy) | Good (balanced) |
| **Framework Flexibility** | High (API-first) | Low (tightly coupled) | Medium |
| **Mobile Performance** | Worse (more requests) | Better (fewer requests) | Good |
| **Waterfall Delays** | High (HTML→JS→API) | None (single request) | Low (parallel after initial) |
| **Ideal For** | SPAs, dynamic pages | Traditional webs, SEO | Rich content + interaction |

---

## Decision Framework

### Choose Client-Side TypeScript If:

- ✓ Building single-page application (SPA)
- ✓ Multiple pages share same API
- ✓ Real-time updates critical
- ✓ Progressive loading desirable
- ✓ Frontend/backend can be deployed independently
- ✓ Team has strong TypeScript skills
- ✓ API-first architecture important
- ✓ Mobile users on fast connections
- ✓ Data doesn't affect SEO
- ✓ Can tolerate JavaScript dependency

### Choose Server-Side C# If:

- ✓ Traditional multi-page application
- ✓ SEO critical (blog, marketing)
- ✓ Initial page load speed paramount
- ✓ Progressive enhancement required
- ✓ Team prefers staying in C#
- ✓ Page-specific data loading
- ✓ Mobile users on slow connections
- ✓ JavaScript must not be required
- ✓ Simple page interactions
- ✓ Security/CSRF handling must be automatic

### Choose Hybrid Approach If:

- ✓ Content-heavy pages with rich interaction
- ✓ Both SEO and real-time important
- ✓ Independent data sources
- ✓ Want progressive loading
- ✓ Some features require JavaScript
- ✓ Resilience important (work without secondary data)
- ✓ Can tolerate incremental UI appearance

---

## Real-World Scenarios

### Scenario 1: Employee Directory Page

**Requirements:**
- Display list of 500 employees
- Users need to search/filter
- Real-time status updates
- SEO not critical
- Team experienced in TypeScript

**Recommendation: Client-Side TypeScript**

```typescript
// Load paginated/empty list server-side
// Client fetches employees on demand
// Client implements real-time status updates
```

### Scenario 2: Blog Post Detail

**Requirements:**
- Display blog post + comments
- SEO very important
- Search engines must see full content
- Mobile users on slow networks
- Comments load quickly enough

**Recommendation: Server-Side C#**

```csharp
// Load post and initial comments on server
// Render complete page with content
// Optional: client-side comment refresh
```

### Scenario 3: Dashboard

**Requirements:**
- Top metrics must load fast
- Detailed charts load separately
- Real-time metric updates
- Mobile-first design
- SEO not important

**Recommendation: Hybrid**

```
Server: Load top 3 metrics, render immediately
Client: Load detailed charts in background, subscribe to real-time updates
```

### Scenario 4: Public Marketing Website

**Requirements:**
- SEO absolutely critical
- Perfect Core Web Vitals
- Multiple pages with similar layout
- Simple interactions (contact form, newsletter)
- Must work without JavaScript

**Recommendation: Server-Side C#**

```csharp
// Server renders all content
// No reliance on TypeScript
// Possible: light progressive enhancement with vanilla JS
```

---

## Authentication & Authorization Patterns

### Client-Side TypeScript

```typescript
// Store JWT token (risky in localStorage)
async function login(credentials: Credentials) {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials)
  });
  const data = await response.json();
  localStorage.setItem('token', data.token);
}

// Include token in requests
async function authenticatedFetch(url: string) {
  const token = localStorage.getItem('token');
  return fetch(url, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
}
```

**Issues:**
- XSS vulnerability if token in localStorage
- Manual token refresh needed
- Must handle expiration in TypeScript

### Server-Side C#

```csharp
// Cookie-based authentication (HttpOnly, Secure)
public class LoginPageModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public async Task OnPostAsync(string username, string password)
    {
        var user = await _signInManager.PasswordSignInAsync(
            username, password, isPersistent: true, lockoutOnFailure: true);
        
        if (user.Succeeded)
        {
            Response.Redirect("/");
        }
    }
}

// Automatic inclusion in requests
// Automatic refresh handling
// No manual token management
```

**Advantages:**
- Secure HttpOnly cookies
- Automatic token refresh
- Built-in protection

### Hybrid Pattern

```csharp
// Server-side cookie for page authentication
// Client-side refresh token for API calls

public async Task OnGetAsync()
{
    // User logged in via cookie
    User = await GetCurrentUserAsync();
}
```

```typescript
// Use refresh token for client-side API calls
async function fetchWithToken(url: string) {
    let response = await fetch(url);
    if (response.status === 401) {
        // Refresh token
        await refreshAccessToken();
        response = await fetch(url); // Retry
    }
    return response;
}
```

---

## Performance Comparison

### Page Load Waterfall: Client-Side

```
0ms   ├─ HTTP Request
10ms  ├─ HTML Response (minimal, no data)
15ms  ├─ Parse HTML
20ms  ├─ Fetch TypeScript bundle
     │   └─ 200ms network delay
     │   └─ 100ms parse
320ms ├─ Execute TypeScript
350ms ├─ Make API call
     │   └─ 150ms network delay
500ms ├─ Render data
550ms └─ User sees content

Total: 550ms (with 3G delay: 800ms+)
```

### Page Load Waterfall: Server-Side

```
0ms   ├─ HTTP Request
50ms  ├─ Server fetches from API (150ms API latency)
200ms ├─ HTML Response (complete with data)
210ms ├─ Parse HTML
230ms ├─ CSS/Images start loading
300ms └─ User sees content

Total: 300ms (with 3G delay: 350ms+)
```

### Core Web Vitals Impact

| Metric | Client-Side | Server-Side | Hybrid |
|--------|-------------|------------|--------|
| **FCP** (First Contentful Paint) | Very Poor | Good | Good |
| **LCP** (Largest Contentful Paint) | Poor | Good | Good |
| **CLS** (Cumulative Layout Shift) | Poor (content jumps) | Good | Good |
| **TTFB** (Time to First Byte) | Good | Good | Good |
| **INP** (Interaction to Next Paint) | Good | Good | Good |

---

## CSRF & Security Patterns

### Client-Side TypeScript CSRF

```typescript
// Must manually handle CSRF token
async function saveEmployee(employee: Employee) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')
        ?.getAttribute('value');
    
    const response = await fetch('/api/employees', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-Token': token // Must manually include
        },
        body: JSON.stringify(employee),
        credentials: 'include' // Include cookies
    });
}
```

### Server-Side C# CSRF

```csharp
// Automatic CSRF protection in Razor Pages
// Just use form tag helper
```

```html
<form method="post">
    <!-- Automatic CSRF token inclusion -->
    @Html.AntiForgeryToken()
    <!-- or just use form tag helper -->
    <form asp-page="Post" />
</form>
```

### Hybrid CSRF

```html
<!-- Server includes token in page -->
<input type="hidden" name="__RequestVerificationToken" value="..." />
```

```typescript
// JavaScript learns to use it
const token = document.querySelector('input[name="__RequestVerificationToken"]')
    ?.getAttribute('value');
```

---

## When to Switch or Combine

### Start Server-Side...

Then graduate to client-side when:
- Multiple pages need same data
- Real-time features critical
- User interactivity increases
- SEO less important
- API becomes more stable

### Start Client-Side...

Then incorporate server-side when:
- SEO requirements emerge
- Initial load speed critical
- Mobile performance problematic
- Users on slow networks

### Hybrid from Start...

When you know you need:
- Both SEO and interactivity
- Real-time features + fast initial load
- Progressive enhancement

---

## Dependency Injection & Configuration

### Client-Side Service Registration

```typescript
// Not applicable - client doesn't have DI container
// Manual singleton pattern
export class ServiceLocator {
  private static instance: ServiceLocator;
  private services = new Map();

  static getInstance() {
    if (!ServiceLocator.instance) {
      ServiceLocator.instance = new ServiceLocator();
    }
    return ServiceLocator.instance;
  }

  register(name: string, service: any) {
    this.services.set(name, service);
  }

  get(name: string) {
    return this.services.get(name);
  }
}

// In app initialization
const employeeService = new EmployeeService('/api');
ServiceLocator.getInstance().register('employeeService', employeeService);

// Usage
const service = ServiceLocator.getInstance().get('employeeService');
```

### Server-Side Service Registration

```csharp
// Startup.cs / Program.cs
services.AddScoped<IEmployeeApiClient, EmployeeApiClient>();
services.AddScoped<IEmployeeService, EmployeeService>();

// Used in Page handler
public class EmployeesPageModel : PageModel
{
    private readonly IEmployeeService _service;

    public EmployeesPageModel(IEmployeeService service)
    {
        _service = service;
    }
}
```

---

## Testing Strategies

### Client-Side TypeScript Testing

```typescript
import { EmployeeService } from '../employeeService';

describe('EmployeeService', () => {
  let service: EmployeeService;

  beforeEach(() => {
    global.fetch = jest.fn();
    service = new EmployeeService('/api');
  });

  it('should fetch employees', async () => {
    (global.fetch as jest.Mock).mockResolvedValue({
      ok: true,
      json: () => Promise.resolve([{ id: 1, name: 'John' }])
    });

    const employees = await service.getEmployees();
    expect(employees).toHaveLength(1);
  });
});
```

### Server-Side C# Testing

```csharp
public class EmployeesPageModelTests
{
    [Fact]
    public async Task OnGetAsync_ShouldLoadEmployees()
    {
        // Arrange
        var mockClient = new Mock<IEmployeeApiClient>();
        mockClient.Setup(c => c.GetEmployeesAsync())
            .ReturnsAsync(new List<EmployeeDto> { /* test data */ });

        var page = new EmployeesPageModel(mockClient.Object);

        // Act
        await page.OnGetAsync();

        // Assert
        Assert.NotEmpty(page.Employees);
    }
}
```

### End-to-End Testing

```typescript
// Playwright - tests both server and client
test('should display employees loaded from server', async ({ page }) => {
  await page.goto('/employees');
  
  // Page already loaded with employees (server-side)
  const employees = await page.locator('.employee-card');
  expect(employees).toHaveCount(10);
});

test('should load comments when TypeScript runs', async ({ page }) => {
  await page.goto('/post/1');
  
  // Initial post loaded server-side
  await expect(page.locator('h1')).toContainText('Post Title');
  
  // Comments load via TypeScript
  await page.waitForSelector('#comments-loaded');
  const comments = await page.locator('.comment');
  expect(comments).toHaveCount(5);
});
```

---

## Monitoring & Observability

### Server-Side Logging

```csharp
public class EmployeesPageModel : PageModel
{
    private readonly ILogger<EmployeesPageModel> _logger;

    public async Task OnGetAsync()
    {
        _logger.LogInformation("Loading employees page");
        try
        {
            Employees = await _apiClient.GetEmployeesAsync();
            _logger.LogInformation("Successfully loaded {Count} employees", Employees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load employees");
            ErrorMessage = "Unable to load employees";
        }
    }
}
```

### Client-Side Monitoring

```typescript
export class EmployeeService {
  async getEmployees(): Promise<Employee[]> {
    const startTime = performance.now();
    try {
      const response = await fetch(`${this.baseUrl}/employees`);
      const duration = performance.now() - startTime;
      
      // Track performance
      this.reportMetric('employees.fetch.duration', duration);
      
      if (!response.ok) throw new Error('Failed to fetch');
      return response.json();
    } catch (error) {
      // Track error
      this.reportError('employees.fetch.error', error);
      throw error;
    }
  }

  private reportMetric(name: string, value: number): void {
    if (window.gtag) {
      gtag('event', name, { value });
    }
  }

  private reportError(name: string, error: any): void {
    if (window.Sentry) {
      Sentry.captureException({ message: name, originalException: error });
    }
  }
}
```

---

## Migration Path: From Server-Side to Client-Side

### Phase 1: Server-Side Only

```csharp
public class EmployeesPageModel : PageModel
{
    public async Task OnGetAsync()
    {
        Employees = await _apiClient.GetEmployeesAsync();
    }
}
```

### Phase 2: Add Client-Side Enhancement

```html
<!-- Keep server-side loading -->
<div id="employees-container">
  @foreach (var emp in Model.Employees)
  {
      <div data-emp-id="@emp.Id">@emp.Name</div>
  }
</div>

<!-- Add TypeScript enhancement -->
<script src="/js/employees-enhance.bundle.js"></script>
<script>
  const page = new EmployeesPage();
  page.enhanceWithRealtime();
</script>
```

```typescript
export class EmployeesPage {
  async enhanceWithRealtime(): Promise<void> {
    // Subscribe to real-time updates
    const ws = new WebSocket('wss://api/employees/stream');
    ws.onmessage = (event) => {
      const employee = JSON.parse(event.data);
      this.updateEmployeeInDOM(employee);
    };
  }
}
```

### Phase 3: Page-Based API Endpoints

```csharp
// Add API endpoint that returns JSON
[ApiController]
[Route("api/[controller]")]
public class EmployeesApiController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees()
    {
        var employees = await _apiClient.GetEmployeesAsync();
        return Ok(employees);
    }
}
```

### Phase 4: Pure Client-Side

```typescript
// Remove server-side page handler loading
// Move entirely to client-side TypeScript
export class EmployeesPage {
  async initialize(): Promise<void> {
    const employees = await this.service.getEmployees();
    this.renderEmployees(employees);
  }
}
```

---

## Recommendations by Application Type

| Type | Recommendation | Rationale |
|------|---|---|
| **Blog/Content Site** | Server-Side C# | SEO critical, content-focused |
| **SPA / Dashboard** | Client-Side TS | Real-time, user-driven |
| **E-Commerce** | Hybrid | Product pages (SEO) + cart (interactivity) |
| **Admin Panel** | Client-Side TS | Internal use, heavy interaction |
| **Marketing Site** | Server-Side C# | SEO paramount, simple interactions |
| **Internal Portal** | Hybrid | Speed + some real-time features |
| **Real-Time Chat** | Client-Side TS | WebSocket, constant updates |
| **Company Website** | Server-Side C# | SEO, static-heavy |
| **SaaS Web App** | Client-Side TS | Feature-rich, offline support |

---

## Conclusion

There is no single "best" approach for making API calls in ASP.NET Core Razor Pages. The right choice depends on your application's specific requirements:

**Server-Side C# API Calls:**
- Best for traditional web applications, SEO-critical content, and initial load performance
- Simpler security model, built-in CSRF protection
- Trade-off: less interactive, higher server load

**Client-Side TypeScript API Calls:**
- Best for interactive, real-time applications and SPAs
- Better scalability and frontend independence
- Trade-off: more complex security, slower initial load, requires JavaScript

**Hybrid Approach:**
- Best for balancing SEO with interactivity
- Server loads critical data, client enhances with secondary data
- Trade-off: increased complexity, must manage two data flows

**Key Metrics for Decision:**
1. Does SEO matter for this page?
2. Is initial load speed critical?
3. How interactive does this page need to be?
4. Will this data be shared across multiple pages/apps?
5. What's the team's comfort with TypeScript?
6. How important are real-time updates?
7. What's the target user's network speed?

Start simple (server-side), then graduate to client-side or hybrid patterns as requirements demand. The flexibility of Razor Pages allows migration from one approach to another without complete rewrites.
