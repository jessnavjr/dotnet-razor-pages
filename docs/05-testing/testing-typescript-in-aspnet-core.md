# Testing TypeScript in ASP.NET Core Applications

## Overview

Testing TypeScript in an ASP.NET Core environment requires understanding both frontend testing best practices and how to integrate them into a .NET development workflow. This document covers testing strategies, tooling, patterns, and best practices for TypeScript in ASP.NET Core applications.

For this repository, the standard frontend TypeScript test runner is Jest with `ts-jest` and `jsdom`.

---

## Context: TypeScript + ASP.NET Core Architecture

### Typical Architecture

```
ASP.NET Core API Backend
├─ Controllers/Endpoints
├─ Business Logic
├─ Database Layer
└─ Configuration

TypeScript Frontend (separate build pipeline)
├─ React/Vue/Angular components
├─ API client services
├─ State management
├─ Tests (separate from backend tests)
```

### Integration Points

- **API communication**: TypeScript frontend calls ASP.NET Core API
- **Build pipelines**: Frontend builds independently (npm/yarn) from backend (.NET CLI)
- **Development servers**: ASP.NET Core dev server + npm dev server (or integrated)
- **Deployment**: Frontend bundled and served from ASP.NET Core wwwroot folder

### Testing Scope

This document focuses on **frontend TypeScript testing** in the context of ASP.NET Core applications, not backend .NET testing.

---

## 1. Testing Tools and Frameworks

### Jest (Recommended for Most Scenarios)

**What it is:** Complete JavaScript/TypeScript testing framework with built-in mocking, assertion library, and code coverage.

**Why use with ASP.NET Core:**
- Fastest test discovery and execution
- Excellent TypeScript support (zero-config for most projects)
- Built-in code coverage reporting
- Great for testing React and Vue components
- Popular in enterprise environments

**Setup:**

```bash
npm install --save-dev jest @types/jest ts-jest
```

**jest.config.js:**

```javascript
module.exports = {
  preset: 'ts-jest',
  testEnvironment: 'jsdom', // or 'node' for non-browser code
  roots: ['<rootDir>/src'],
  testMatch: ['**/__tests__/**/*.ts?(x)', '**/?(*.)+(spec|test).ts?(x)'],
  moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx', 'json'],
  collectCoverageFrom: [
    'src/**/*.ts?(x)',
    '!src/**/*.d.ts',
    '!src/index.ts'
  ],
  coverageThreshold: {
    global: {
      branches: 70,
      functions: 70,
      lines: 70,
      statements: 70
    }
  },
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts']
};
```

**package.json scripts:**

```json
{
  "scripts": {
    "test:ts": "jest --coverage",
    "test:ts:watch": "jest --watch",
    "test:ts:ci": "jest --coverage --runInBand"
  }
}
```

### Vitest (Modern, Faster Alternative)

**What it is:** Modern JavaScript/TypeScript test runner with Vite-powered performance.

**Why use with ASP.NET Core:**
- Significantly faster than Jest for large test suites
- Better IDE integration and error messages
- Vite-powered for instant feedback
- Drop-in Jest replacement (same API)

**Setup:**

```bash
npm install --save-dev vitest happy-dom @vitest/ui
```

**vitest.config.ts:**

```typescript
import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    globals: true,
    environment: 'happy-dom', // or 'jsdom'
    setupFiles: ['./src/setupTests.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      thresholds: {
        lines: 70,
        functions: 70,
        branches: 70,
        statements: 70
      }
    }
  }
});
```

### Testing Library (Recommended Augmentation)

**What it is:** Component testing library that focuses on testing user behavior, not implementation.

**Why use with ASP.NET Core:**
- Tests components the way users interact with them
- Better maintainability (tests don't break on refactors)
- Works with React, Vue, Svelte, Solid

**Setup:**

```bash
npm install --save-dev @testing-library/react @testing-library/jest-dom @testing-library/user-event
```

**Comparison: Enzyme vs. Testing Library**

| Aspect | Enzyme | Testing Library |
|--------|--------|-----------------|
| Approach | Implementation testing | User-behavior testing |
| Maintenance | Changes with refactors | Resilient to refactors |
| Learning | Faster (queries enzyme API) | Steeper (think like user) |
| Modern use | Declining | Industry standard |
| Recommendation | Legacy projects | New projects |

---

## 2. TypeScript Unit Testing Patterns

### Basic Test Structure

```typescript
// src/services/employeeService.ts
export interface Employee {
  id: number;
  name: string;
  email: string;
}

export class EmployeeService {
  constructor(private apiBaseUrl: string) {}

  async getEmployee(id: number): Promise<Employee> {
    const response = await fetch(`${this.apiBaseUrl}/api/employees/${id}`);
    if (!response.ok) throw new Error('Failed to fetch employee');
    return await response.json();
  }

  async saveEmployee(employee: Employee): Promise<Employee> {
    const response = await fetch(`${this.apiBaseUrl}/api/employees`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(employee)
    });
    if (!response.ok) throw new Error('Failed to save employee');
    return await response.json();
  }

  validateEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  }
}

// src/services/__tests__/employeeService.test.ts
import { EmployeeService } from '../employeeService';

describe('EmployeeService', () => {
  let service: EmployeeService;

  beforeEach(() => {
    service = new EmployeeService('http://localhost:5000');
  });

  describe('validateEmail', () => {
    it('should validate correct email format', () => {
      const result = service.validateEmail('user@example.com');
      expect(result).toBe(true);
    });

    it('should reject invalid email format', () => {
      expect(service.validateEmail('invalid-email')).toBe(false);
      expect(service.validateEmail('user@')).toBe(false);
      expect(service.validateEmail('@example.com')).toBe(false);
    });
  });

  describe('getEmployee', () => {
    it('should fetch employee from API', async () => {
      // Mock fetch
      global.fetch = jest.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve({ id: 1, name: 'John', email: 'john@example.com' })
        })
      ) as jest.Mock;

      const employee = await service.getEmployee(1);
      expect(employee.name).toBe('John');
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/employees/1');
    });

    it('should throw error on API failure', async () => {
      global.fetch = jest.fn(() =>
        Promise.resolve({ ok: false })
      ) as jest.Mock;

      await expect(service.getEmployee(1)).rejects.toThrow('Failed to fetch employee');
    });
  });

  describe('saveEmployee', () => {
    it('should POST employee to API', async () => {
      const employee = { id: 0, name: 'Jane', email: 'jane@example.com' };
      global.fetch = jest.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve({ id: 1, ...employee })
        })
      ) as jest.Mock;

      const result = await service.saveEmployee(employee);
      expect(result.id).toBe(1);
      expect(global.fetch).toHaveBeenCalledWith(
        'http://localhost:5000/api/employees',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify(employee)
        })
      );
    });
  });
});
```

### Testing Async Operations

```typescript
// src/utils/retryableApi.ts
export async function retryableApiCall<T>(
  fn: () => Promise<T>,
  maxRetries: number = 3,
  delayMs: number = 1000
): Promise<T> {
  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      return await fn();
    } catch (error) {
      if (attempt === maxRetries) throw error;
      await new Promise(resolve => setTimeout(resolve, delayMs));
    }
  }
  throw new Error('Max retries exceeded');
}

// src/utils/__tests__/retryableApi.test.ts
import { retryableApiCall } from '../retryableApi';
import { jest } from '@jest/globals';

describe('retryableApiCall', () => {
  beforeEach(() => {
    jest.useFakeTimers();
  });

  afterEach(() => {
    jest.runOnlyPendingTimers();
    jest.useRealTimers();
  });

  it('should resolve on first success', async () => {
    const fn = jest.fn().mockResolvedValueOnce('success');
    const result = await retryableApiCall(fn);
    expect(result).toBe('success');
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it('should retry on failure and eventually succeed', async () => {
    const fn = jest
      .fn()
      .mockRejectedValueOnce(new Error('fail'))
      .mockRejectedValueOnce(new Error('fail'))
      .mockResolvedValueOnce('success');

    const promise = retryableApiCall(fn, 3, 100);
    await jest.runAllTimersAsync();
    
    const result = await promise;
    expect(result).toBe('success');
    expect(fn).toHaveBeenCalledTimes(3);
  });

  it('should throw error after max retries', async () => {
    const fn = jest.fn().mockRejectedValue(new Error('always fails'));
    
    const promise = retryableApiCall(fn, 2, 100);
    await jest.runAllTimersAsync();
    
    await expect(promise).rejects.toThrow('always fails');
    expect(fn).toHaveBeenCalledTimes(2);
  });
});
```

### Testing with Dependency Injection

```typescript
// src/services/notificationService.ts
export interface ILogger {
  log(message: string): void;
}

export class NotificationService {
  constructor(private logger: ILogger) {}

  sendNotification(message: string): void {
    // Business logic
    this.logger.log(`Notification sent: ${message}`);
  }
}

// src/services/__tests__/notificationService.test.ts
describe('NotificationService', () => {
  it('should log notification', () => {
    // Mock logger
    const mockLogger = {
      log: jest.fn()
    };

    const service = new NotificationService(mockLogger);
    service.sendNotification('Test message');

    expect(mockLogger.log).toHaveBeenCalledWith('Notification sent: Test message');
  });
});
```

---

## 3. React Component Testing

### Testing React Components with Testing Library

```typescript
// src/components/EmployeeForm.tsx
import React, { useState } from 'react';

interface EmployeeFormProps {
  onSubmit: (employee: { name: string; email: string }) => void;
  isLoading?: boolean;
}

export const EmployeeForm: React.FC<EmployeeFormProps> = ({ onSubmit, isLoading = false }) => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!name || !email) {
      setError('Name and email are required');
      return;
    }

    setError('');
    onSubmit({ name, email });
  };

  return (
    <form onSubmit={handleSubmit}>
      {error && <div role="alert" className="error">{error}</div>}
      
      <input
        type="text"
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Name"
        aria-label="Employee name"
        disabled={isLoading}
      />
      
      <input
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        placeholder="Email"
        aria-label="Employee email"
        disabled={isLoading}
      />
      
      <button type="submit" disabled={isLoading}>
        {isLoading ? 'Saving...' : 'Add Employee'}
      </button>
    </form>
  );
};

// src/components/__tests__/EmployeeForm.test.tsx
import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { EmployeeForm } from '../EmployeeForm';

describe('EmployeeForm', () => {
  it('should render form with inputs', () => {
    const mockSubmit = jest.fn();
    render(<EmployeeForm onSubmit={mockSubmit} />);

    expect(screen.getByLabelText('Employee name')).toBeInTheDocument();
    expect(screen.getByLabelText('Employee email')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /add employee/i })).toBeInTheDocument();
  });

  it('should validate required fields', async () => {
    const mockSubmit = jest.fn();
    render(<EmployeeForm onSubmit={mockSubmit} />);

    const button = screen.getByRole('button', { name: /add employee/i });
    fireEvent.click(button);

    expect(screen.getByRole('alert')).toHaveTextContent('Name and email are required');
    expect(mockSubmit).not.toHaveBeenCalled();
  });

  it('should submit form with valid data', async () => {
    const mockSubmit = jest.fn();
    const user = userEvent.setup();
    
    render(<EmployeeForm onSubmit={mockSubmit} />);

    await user.type(screen.getByLabelText('Employee name'), 'John Doe');
    await user.type(screen.getByLabelText('Employee email'), 'john@example.com');
    await user.click(screen.getByRole('button', { name: /add employee/i }));

    expect(mockSubmit).toHaveBeenCalledWith({
      name: 'John Doe',
      email: 'john@example.com'
    });
  });

  it('should disable inputs while loading', () => {
    const mockSubmit = jest.fn();
    render(<EmployeeForm onSubmit={mockSubmit} isLoading={true} />);

    expect(screen.getByLabelText('Employee name')).toBeDisabled();
    expect(screen.getByLabelText('Employee email')).toBeDisabled();
    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('should show loading state on button', () => {
    const mockSubmit = jest.fn();
    render(<EmployeeForm onSubmit={mockSubmit} isLoading={true} />);

    expect(screen.getByRole('button')).toHaveTextContent('Saving...');
  });
});
```

### Testing React Hooks

```typescript
// src/hooks/useEmployee.ts
import { useState, useCallback } from 'react';

export interface Employee {
  id: number;
  name: string;
  email: string;
}

export const useEmployee = (apiUrl: string) => {
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchEmployee = useCallback(async (id: number) => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${apiUrl}/api/employees/${id}`);
      if (!response.ok) throw new Error('Failed to fetch');
      const data = await response.json();
      setEmployee(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  }, [apiUrl]);

  return { employee, loading, error, fetchEmployee };
};

// src/hooks/__tests__/useEmployee.test.ts
import { renderHook, act, waitFor } from '@testing-library/react';
import { useEmployee } from '../useEmployee';

describe('useEmployee', () => {
  beforeEach(() => {
    global.fetch = jest.fn();
  });

  it('should fetch employee', async () => {
    const mockEmployee = { id: 1, name: 'John', email: 'john@example.com' };
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockEmployee)
    });

    const { result } = renderHook(() => useEmployee('http://localhost:5000'));

    act(() => {
      result.current.fetchEmployee(1);
    });

    await waitFor(() => {
      expect(result.current.employee).toEqual(mockEmployee);
      expect(result.current.loading).toBe(false);
    });
  });

  it('should handle error', async () => {
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: false
    });

    const { result } = renderHook(() => useEmployee('http://localhost:5000'));

    act(() => {
      result.current.fetchEmployee(1);
    });

    await waitFor(() => {
      expect(result.current.error).toBe('Failed to fetch');
      expect(result.current.loading).toBe(false);
    });
  });

  it('should set loading state', () => {
    const promise = new Promise(resolve => setTimeout(resolve, 100));
    (global.fetch as jest.Mock).mockReturnValueOnce(promise);

    const { result } = renderHook(() => useEmployee('http://localhost:5000'));

    act(() => {
      result.current.fetchEmployee(1);
    });

    expect(result.current.loading).toBe(true);
  });
});
```

---

## 4. API Client Testing

### Testing API Service with Mock Server

```typescript
// src/services/apiClient.ts
export class ApiClient {
  constructor(private baseUrl: string) {}

  async get<T>(path: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`);
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  }

  async post<T>(path: string, data: unknown): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
  }
}

// src/services/__tests__/apiClient.test.ts
import { ApiClient } from '../apiClient';

describe('ApiClient', () => {
  let client: ApiClient;

  beforeEach(() => {
    client = new ApiClient('http://localhost:5000');
    global.fetch = jest.fn();
  });

  describe('GET', () => {
    it('should fetch data', async () => {
      const mockData = { id: 1, name: 'Test' };
      (global.fetch as jest.Mock).mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve(mockData)
      });

      const result = await client.get('/api/data');
      expect(result).toEqual(mockData);
      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5000/api/data');
    });

    it('should throw on error status', async () => {
      (global.fetch as jest.Mock).mockResolvedValueOnce({
        ok: false,
        status: 404
      });

      await expect(client.get('/api/notfound')).rejects.toThrow('HTTP 404');
    });
  });

  describe('POST', () => {
    it('should post data', async () => {
      const postData = { name: 'New Item' };
      const mockResponse = { id: 1, ...postData };
      
      (global.fetch as jest.Mock).mockResolvedValueOnce({
        ok: true,
        json: () => Promise.resolve(mockResponse)
      });

      const result = await client.post('/api/items', postData);
      expect(result).toEqual(mockResponse);
      expect(global.fetch).toHaveBeenCalledWith(
        'http://localhost:5000/api/items',
        expect.objectContaining({
          method: 'POST',
          body: JSON.stringify(postData)
        })
      );
    });
  });
});
```

---

## 5. Integration Testing TypeScript with ASP.NET Core

### E2E Testing with Playwright

```typescript
// tests/e2e/employee.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Employee Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5173'); // React dev server
  });

  test('should display employee list', async ({ page }) => {
    await expect(page.locator('text=Employee List')).toBeVisible();
    const rows = page.locator('table tbody tr');
    expect(await rows.count()).toBeGreaterThan(0);
  });

  test('should add new employee', async ({ page }) => {
    // Wait for API call to complete
    await page.waitForResponse(response => 
      response.url().includes('/api/employees') && response.status() === 200
    );

    await page.fill('[aria-label="Employee name"]', 'Jane Doe');
    await page.fill('[aria-label="Employee email"]', 'jane@example.com');
    await page.click('button:has-text("Add Employee")');

    // Verify success message
    await expect(page.locator('text=Employee added')).toBeVisible();
    
    // Verify in list
    await expect(page.locator('text=Jane Doe')).toBeVisible();
  });

  test('should validate form before submission', async ({ page }) => {
    await page.click('button:has-text("Add Employee")');
    await expect(page.locator('text=Name and email required')).toBeVisible();
  });
});
```

**Setup for E2E testing:**

```bash
npm install --save-dev @playwright/test
npx playwright install
```

**playwright.config.ts:**

```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests/e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:5173',
    trace: 'on-first-retry',
  },
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:5173',
    reuseExistingServer: !process.env.CI,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
  ],
});
```

---

## 6. ASP.NET Core Project Integration

### TypeScript in ASP.NET Core Razor Pages

```typescript
// wwwroot/ts/page-handler.ts
import { ApiClient } from './services/apiClient';

export class EmployeePageHandler {
  private apiClient: ApiClient;

  constructor() {
    this.apiClient = new ApiClient('/api');
  }

  async initializePage(): Promise<void> {
    const employees = await this.apiClient.get('/employees');
    this.renderEmployees(employees);
  }

  private renderEmployees(employees: any[]): void {
    const container = document.getElementById('employees');
    if (!container) return;

    container.innerHTML = employees
      .map(emp => `<div>${emp.name} - ${emp.email}</div>`)
      .join('');
  }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  const handler = new EmployeePageHandler();
  handler.initializePage().catch(err => console.error('Failed to init:', err));
});

// wwwroot/ts/__tests__/page-handler.test.ts
import { EmployeePageHandler } from '../page-handler';

jest.mock('../services/apiClient');

describe('EmployeePageHandler', () => {
  let handler: EmployeePageHandler;
  let container: HTMLDivElement;

  beforeEach(() => {
    // Setup DOM
    container = document.createElement('div');
    container.id = 'employees';
    document.body.appendChild(container);

    handler = new EmployeePageHandler();
  });

  afterEach(() => {
    document.body.removeChild(container);
  });

  it('should render employees on page init', async () => {
    // Mock API response
    const mockEmployees = [
      { name: 'John', email: 'john@example.com' },
      { name: 'Jane', email: 'jane@example.com' }
    ];

    // Mock the ApiClient
    jest.spyOn(handler as any, 'apiClient').mockReturnValue({
      get: jest.fn().mockResolvedValue(mockEmployees)
    });

    // This test would need proper mocking setup
    // Simplified for illustration
  });
});
```

### Build Pipeline Integration

**package.json (ASP.NET Core project root):**

```json
{
  "scripts": {
    "dev": "npm run dev:build -- --watch",
    "dev:build": "tsc && webpack",
    "build": "tsc && webpack --mode production",
    "test": "jest",
    "test:watch": "jest --watch",
    "test:coverage": "jest --coverage",
    "e2e": "playwright test",
    "e2e:headed": "playwright test --headed"
  },
  "devDependencies": {
    "@types/jest": "^29.0.0",
    "@types/node": "^20.0.0",
    "jest": "^29.0.0",
    "ts-jest": "^29.0.0",
    "typescript": "^5.0.0",
    "webpack": "^5.0.0",
    "@playwright/test": "^1.40.0"
  }
}
```

**webpack.config.js:**

```javascript
module.exports = {
  mode: 'development',
  entry: {
    'page-handler': './wwwroot/ts/page-handler.ts'
  },
  output: {
    path: __dirname + '/wwwroot/js',
    filename: '[name].bundle.js'
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js']
  },
  devtool: 'source-map'
};
```

---

## 7. Testing Best Practices for ASP.NET Core + TypeScript

### 1. Isolation: API Contract Testing

```typescript
// src/services/__tests__/employeeApi.stubs.ts
// Test utilities for mocking ASP.NET Core API responses

export const stubEmployeeResponse = (overrides = {}) => ({
  id: 1,
  name: 'John Doe',
  email: 'john@example.com',
  createdAt: '2026-01-01T00:00:00Z',
  ...overrides
});

export const stubEmployeeErrorResponse = (statusCode = 400, message = 'Bad Request') => ({
  status: statusCode,
  message,
  errors: {}
});

// Use in tests:
describe('Employee API Integration', () => {
  it('should handle ASP.NET Core error response', async () => {
    const response = stubEmployeeErrorResponse(422, 'Email already exists');
    expect(response.status).toBe(422);
  });
});
```

### 2. Auth Token Handling in Tests

```typescript
// src/services/authService.ts
export class AuthService {
  private token: string | null = null;

  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('auth_token', token);
  }

  getToken(): string | null {
    return this.token || localStorage.getItem('auth_token');
  }

  clearToken(): void {
    this.token = null;
    localStorage.removeItem('auth_token');
  }
}

// src/services/__tests__/authService.test.ts
describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    localStorage.clear();
    service = new AuthService();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should set and retrieve token', () => {
    const token = 'eyJhbGc...';
    service.setToken(token);
    expect(service.getToken()).toBe(token);
    expect(localStorage.getItem('auth_token')).toBe(token);
  });

  it('should clear token', () => {
    service.setToken('token');
    service.clearToken();
    expect(service.getToken()).toBeNull();
    expect(localStorage.getItem('auth_token')).toBeNull();
  });
});
```

### 3. Mocking HTTP Interceptors

```typescript
// src/services/httpClient.ts
import axios from 'axios';

export const createHttpClient = (baseURL: string, authService: AuthService) => {
  const instance = axios.create({ baseURL });

  instance.interceptors.request.use(config => {
    const token = authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  instance.interceptors.response.use(
    response => response,
    error => {
      if (error.response?.status === 401) {
        authService.clearToken();
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );

  return instance;
};

// src/services/__tests__/httpClient.test.ts
import axios from 'axios';
import { createHttpClient } from '../httpClient';
import { AuthService } from '../authService';

jest.mock('axios');

describe('httpClient interceptors', () => {
  let mockAuthService: Partial<AuthService>;

  beforeEach(() => {
    mockAuthService = {
      getToken: jest.fn(() => 'fake-token'),
      clearToken: jest.fn()
    };

    (axios.create as jest.Mock).mockReturnValue(axios);
  });

  it('should add authorization header to requests', async () => {
    const client = createHttpClient('http://api', mockAuthService as AuthService);
    
    (axios.interceptors.request.use as jest.Mock).mockImplementation(
      (onFulfilled) => {
        const config = { headers: {} };
        onFulfilled(config);
        return config;
      }
    );

    expect(mockAuthService.getToken).toHaveBeenCalled();
  });
});
```

### 4. Coverage and CI Integration

**.github/workflows/test.yml:**

```yaml
name: Test TypeScript

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        run: npm ci
      
      - name: Run tests
        run: npm run test:coverage
      
      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./coverage/coverage-final.json
      
      - name: Run E2E tests
        run: npm run e2e
      
      - name: Upload E2E reports
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: playwright-report/
```

---

## 8. Common Testing Patterns

### Testing Validation Logic

```typescript
// src/validation/employeeValidator.ts
export interface ValidationError {
  field: string;
  message: string;
}

export class EmployeeValidator {
  validate(employee: any): ValidationError[] {
    const errors: ValidationError[] = [];

    if (!employee.name?.trim()) {
      errors.push({ field: 'name', message: 'Name is required' });
    }

    if (!employee.email?.trim()) {
      errors.push({ field: 'email', message: 'Email is required' });
    } else if (!this.isValidEmail(employee.email)) {
      errors.push({ field: 'email', message: 'Invalid email format' });
    }

    if (employee.age !== undefined && (employee.age < 18 || employee.age > 100)) {
      errors.push({ field: 'age', message: 'Age must be between 18 and 100' });
    }

    return errors;
  }

  private isValidEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  }
}

// src/validation/__tests__/employeeValidator.test.ts
import { EmployeeValidator } from '../employeeValidator';

describe('EmployeeValidator', () => {
  let validator: EmployeeValidator;

  beforeEach(() => {
    validator = new EmployeeValidator();
  });

  describe('name validation', () => {
    it('should require name', () => {
      const errors = validator.validate({ email: 'test@example.com' });
      expect(errors).toContainEqual({ field: 'name', message: 'Name is required' });
    });

    it('should reject empty name', () => {
      const errors = validator.validate({ name: '   ', email: 'test@example.com' });
      expect(errors).toContainEqual({ field: 'name', message: 'Name is required' });
    });
  });

  describe('email validation', () => {
    it('should require email', () => {
      const errors = validator.validate({ name: 'John' });
      expect(errors).toContainEqual({ field: 'email', message: 'Email is required' });
    });

    it('should validate email format', () => {
      const errors = validator.validate({
        name: 'John',
        email: 'invalid-email'
      });
      expect(errors).toContainEqual({ field: 'email', message: 'Invalid email format' });
    });

    it('should accept valid email', () => {
      const errors = validator.validate({
        name: 'John',
        email: 'john@example.com'
      });
      expect(errors.find(e => e.field === 'email')).toBeUndefined();
    });
  });

  describe('age validation', () => {
    it('should validate age range', () => {
      const errors = validator.validate({
        name: 'John',
        email: 'john@example.com',
        age: 15
      });
      expect(errors).toContainEqual({
        field: 'age',
        message: 'Age must be between 18 and 100'
      });
    });
  });
});
```

### Testing State Management

```typescript
// src/state/employeeStore.ts
import create from 'zustand';

interface EmployeeState {
  employees: any[];
  loading: boolean;
  error: string | null;
  addEmployee: (employee: any) => Promise<void>;
  fetchEmployees: () => Promise<void>;
  clearError: () => void;
}

export const useEmployeeStore = create<EmployeeState>((set, get) => ({
  employees: [],
  loading: false,
  error: null,

  addEmployee: async (employee) => {
    set({ loading: true, error: null });
    try {
      const response = await fetch('/api/employees', {
        method: 'POST',
        body: JSON.stringify(employee)
      });
      if (!response.ok) throw new Error('Failed to add');
      const saved = await response.json();
      set(state => ({
        employees: [...state.employees, saved],
        loading: false
      }));
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Unknown error', loading: false });
    }
  },

  fetchEmployees: async () => {
    set({ loading: true, error: null });
    try {
      const response = await fetch('/api/employees');
      if (!response.ok) throw new Error('Failed to fetch');
      const data = await response.json();
      set({ employees: data, loading: false });
    } catch (error) {
      set({ error: error instanceof Error ? error.message : 'Unknown error', loading: false });
    }
  },

  clearError: () => set({ error: null })
}));

// src/state/__tests__/employeeStore.test.ts
import { renderHook, act, waitFor } from '@testing-library/react';
import { useEmployeeStore } from '../employeeStore';

describe('employeeStore', () => {
  beforeEach(() => {
    useEmployeeStore.setState({
      employees: [],
      loading: false,
      error: null
    });
    global.fetch = jest.fn();
  });

  it('should add employee', async () => {
    const mockEmployee = { id: 1, name: 'John', email: 'john@example.com' };
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockEmployee)
    });

    const { result } = renderHook(() => useEmployeeStore());

    await act(async () => {
      await result.current.addEmployee({ name: 'John', email: 'john@example.com' });
    });

    expect(result.current.employees).toEqual([mockEmployee]);
    expect(result.current.loading).toBe(false);
    expect(result.current.error).toBeNull();
  });

  it('should handle error', async () => {
    (global.fetch as jest.Mock).mockResolvedValueOnce({ ok: false });

    const { result } = renderHook(() => useEmployeeStore());

    await act(async () => {
      await result.current.addEmployee({ name: 'John', email: 'john@example.com' });
    });

    expect(result.current.error).toBe('Failed to add');
    expect(result.current.loading).toBe(false);
  });
});
```

---

## 9. Performance Testing

```typescript
// src/__tests__/performance.test.ts
describe('performance benchmarks', () => {
  it('should process large list efficiently', () => {
    const largeList = Array.from({ length: 10000 }, (_, i) => ({
      id: i,
      name: `Employee ${i}`,
      email: `emp${i}@example.com`
    }));

    const start = performance.now();
    const filtered = largeList.filter(e => e.email.includes('100'));
    const end = performance.now();

    expect(end - start).toBeLessThan(10); // Should complete in <10ms
    expect(filtered).toHaveLength(11);
  });
});
```

---

## 10. Recommended Testing Stack for ASP.NET Core + TypeScript

| Layer | Tool | Reasoning |
|-------|------|-----------|
| **Unit Tests** | Jest + ts-jest | Fast, zero-config for TypeScript, excellent coverage |
| **Component Tests** | Testing Library | Tests user behavior, not implementation details |
| **Hook Tests** | @testing-library/react | Natural way to test React hooks |
| **API Mocking** | MSW (Mock Service Worker) | Intercepts HTTP requests, works across Jest/Playwright |
| **E2E Tests** | Playwright | Headless browser testing, great debugging tools |
| **Performance** | Bench tooling as needed | Add dedicated benchmarking only when there is a real performance target |
| **Coverage** | nyc or jest --coverage | Built-in or simple integration |

---

## Quick Start: Add Testing to Existing ASP.NET Core Project

### Step 1: Initialize Testing

```bash
npm init -y
npm install --save-dev typescript jest @types/jest ts-jest jest-environment-jsdom jsdom @testing-library/react
```

### Step 2: Add Test Scripts

```json
{
  "scripts": {
    "test:ts": "jest --coverage",
    "test:ts:watch": "jest --watch",
    "test:ts:ci": "jest --coverage --runInBand"
  }
}
```

### Step 3: Create First Test

```bash
mkdir -p src/__tests__
touch src/__tests__/example.test.ts
```

### Step 4: Run Tests

```bash
npm test
```

---

## Conclusion

Testing TypeScript in ASP.NET Core applications requires understanding both frontend testing frameworks and how they integrate with .NET development workflows. Key takeaways:

1. **Use Jest** as the primary test runner for TypeScript frontend code in this repository
2. **Use Testing Library** for React/Vue component testing focused on user behavior
3. **Separate frontend tests from backend tests** with independent build pipelines
4. **Mock ASP.NET Core APIs** using jest.mock or MSW for isolation
5. **Use Playwright for E2E testing** to verify integration between frontend and API
6. **Maintain 70%+ code coverage** for frontend TypeScript code
7. **Integrate with CI/CD** via GitHub Actions or Azure Pipelines
8. **Test async operations carefully** with proper waiting and timers
9. **Focus on user-centric testing** rather than implementation details
10. **Set up once, run often** with watch mode for fast feedback loops

In this repository, the combination of Jest for unit testing, jsdom for DOM-oriented browser behavior, and Playwright for future E2E coverage provides a clean testing baseline for TypeScript code in an ASP.NET Core application.
