# Development Standards

**Last Updated:** 2026-01-03
**Version:** Phase 03 Complete (Authentication)

## Overview

This document defines the coding standards, formatting rules, and development practices for the Nexora Management platform. All team members must follow these standards to ensure code consistency and quality.

## Code Formatting

### TypeScript/JavaScript

**Tool:** Prettier 3.0+

**Configuration:** `.prettierrc`

```json
{
  "semi": true,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 100,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

**Rules:**

- Use semicolons
- Use single quotes for strings
- 2-space indentation
- Trailing commas in ES5-compatible locations
- Maximum line width: 100 characters
- Always use parentheses around arrow function parameters
- Line endings: LF (Unix)

**Examples:**

```typescript
// Good
const getUser = (id: string): User => {
  return userService.findById(id);
};

// Bad (too wide)
const getUser = (id: string): User => {
  return userService.findById(id);
};

// Good
interface User {
  id: string;
  name: string;
  email: string;
}

// Bad (no trailing comma)
interface User {
  id: string;
  name: string;
  email: string;
}
```

**Running Prettier:**

```bash
# Format all files
npm run format

# Check formatting
npm run format:check

# Format specific file
npx prettier --write apps/frontend/src/components/Button.tsx
```

### C# / .NET

**Tool:** dotnet format (built-in)

**Configuration:** Implicit (follows C# coding conventions)

**EditorConfig:** `.editorconfig` (inherited from .NET defaults)

**Rules:**

- 4-space indentation
- Opening braces on new line for types, methods
- Opening braces on same line for properties, control structures
- Spaces around operators
- No trailing whitespace
- UTF-8 encoding

**Examples:**

```csharp
// Good
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.FindByIdAsync(id);
    }
}

// Bad (inconsistent indentation)
public class UserService {
private readonly IUserRepository _userRepository;
public UserService(IUserRepository userRepository) {
_userRepository = userRepository;
}
}
```

**Running dotnet format:**

```bash
cd apps/backend

# Format all code
dotnet format

# Format specific project
dotnet format --include src/Nexora.Management.API

# Verify formatting
dotnet format --verify-no-changes
```

### JSON

**Tool:** Prettier

**Rules:**

- 2-space indentation
- Double quotes for keys and string values
- Trailing commas where permitted
- No trailing whitespace

**Example:**

```json
{
  "name": "nexora-management",
  "version": "1.0.0",
  "scripts": {
    "dev": "turbo run dev",
    "build": "turbo run build"
  }
}
```

### Markdown

**Tool:** Prettier

**Rules:**

- Consistent header spacing
- Unordered list style: `-`
- Consistent code block language tags
- Maximum line width: 100 characters

**Example:**

````markdown
# Header 1

## Header 2

- Item 1
- Item 2

```typescript
const code = 'formatted';
```
````

````

## Linting

### TypeScript/JavaScript Linting

**Tool:** ESLint

**Configuration:** `.eslintrc.js`

```javascript
module.exports = {
  root: true,
  parser: '@typescript-eslint/parser',
  parserOptions: {
    ecmaVersion: 2022,
    sourceType: 'module',
    project: './tsconfig.json',
  },
  plugins: ['@typescript-eslint'],
  extends: [
    'eslint:recommended',
    '@typescript-eslint/recommended',
    '@typescript-eslint/recommended-requiring-type-checking',
  ],
  rules: {
    // Custom rules override
    '@typescript-eslint/no-unused-vars': 'error',
    '@typescript-eslint/explicit-function-return-type': 'warn',
    '@typescript-eslint/no-explicit-any': 'warn',
    '@typescript-eslint/strict-boolean-expressions': 'warn',
  },
};
````

**Key Rules:**

- No unused variables
- Prefer explicit return types on exported functions
- Avoid `any` type (use `unknown` if necessary)
- Use strict boolean expressions
- No console.log in production code

**Examples:**

```typescript
// Good
export function getUser(id: string): User {
  return userService.findById(id);
}

// Bad (no return type)
export function getUser(id: string) {
  return userService.findById(id);
}

// Good
const processData = (data: unknown): void => {
  if (typeof data === 'string') {
    console.log(data);
  }
};

// Bad (using any)
const processData = (data: any): void => {
  console.log(data);
};
```

**Running ESLint:**

```bash
# Lint all packages
npm run lint

# Lint frontend only
cd apps/frontend
npm run lint

# Auto-fix issues
npm run lint -- --fix

# Lint specific file
npx eslint apps/frontend/src/app/page.tsx
```

### C# Static Analysis

**Tools:**

- Roslyn Analyzers (built-in)
- StyleCop.Analyzers (package)

**Configuration:** `.editorconfig`

```ini
# Indentation
indent_size = 4

# Naming conventions
dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning
dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i

dotnet_naming_symbols.interface.applicable_kinds = interface
dotnet_naming_style.begins_with_i.required_prefix = I

# Code quality
dotnet_code_quality_unused_parameters = all:suggestion
```

**Key Analyzer Rules:**

- CA1062: Validate arguments (null checks)
- CA1303: Do not pass literals as localized parameters
- CA2007: Consider calling ConfigureAwait on await
- CA1031: Do not catch general exception types

**Examples:**

```csharp
// Good
public async Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken)
{
    var user = await _userRepository.FindByIdAsync(id, cancellationToken)
        .ConfigureAwait(false);

    if (user == null)
    {
        throw new NotFoundException($"User with ID {id} not found.");
    }

    return user;
}

// Bad (no null check)
public async Task<User> GetUserAsync(Guid id)
{
    return await _userRepository.FindByIdAsync(id);
}
```

## Pre-commit Hooks

### Husky Configuration

**Tool:** Husky 9.0+

**Setup:** Automatically configured via `npm install` (runs `npm run prepare`)

**Hook Location:** `.husky/pre-commit`

```bash
#!/bin/sh
. "$(dirname "$0")/_/husky.sh"

npx lint-staged
```

### Lint-staged Configuration

**Tool:** lint-staged 15.0+

**Configuration:** `.lintstagedrc.json`

```json
{
  "*.{cs,csproj}": ["dotnet format --include"],
  "*.{ts,tsx,js,jsx}": ["eslint --fix", "prettier --write"],
  "*.{json,md}": ["prettier --write"]
}
```

**Behavior:**

- Runs on every `git commit`
- Only processes staged files
- Runs formatters and linters
- Blocks commit if checks fail
- Auto-fixes issues where possible

**Supported File Types:**

| Pattern                    | Tools            |
| -------------------------- | ---------------- |
| `*.cs, *.csproj`           | dotnet format    |
| `*.ts, *.tsx, *.js, *.jsx` | ESLint, Prettier |
| `*.json, *.md`             | Prettier         |

**Workflow:**

```bash
# 1. Make changes to files
git add .

# 2. Attempt commit
git commit -m "feat: add new feature"

# 3. Pre-commit hook runs automatically
#    - Formats C# files with dotnet format
#    - Lints and formats TS/JS files with ESLint + Prettier
#    - Formats JSON and MD files with Prettier

# 4. If successful, commit proceeds
#    If failed, fix issues and try again

# 5. Bypass hook (NOT RECOMMENDED)
git commit --no-verify -m "feat: add new feature"
```

## Git Workflow

### Branch Naming Conventions

**Format:** `<type>/<short-description>`

**Types:**

- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Adding or updating tests
- `chore/` - Maintenance tasks
- `perf/` - Performance improvements

**Examples:**

```bash
feature/user-authentication
fix/task-creation-error
docs/api-documentation
refactor/workspace-management
test/user-repository-tests
chore/update-dependencies
perf/database-query-optimization
```

### Commit Message Conventions

**Format:** [Conventional Commits](https://www.conventionalcommits.org/)

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Types:**

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting, etc.)
- `refactor:` - Code refactoring
- `test:` - Adding or updating tests
- `chore:` - Maintenance tasks
- `perf:` - Performance improvements
- `ci:` - CI/CD changes

**Scopes:**

- `backend` - Backend changes
- `frontend` - Frontend changes
- `docker` - Docker configuration
- `docs` - Documentation
- `infra` - Infrastructure/DevOps

**Examples:**

```bash
# Simple feature
git commit -m "feat: add user authentication"

# Feature with scope
git commit -m "feat(frontend): add login page component"

# Bug fix
git commit -m "fix: resolve issue with task creation"

# Bug fix with body
git commit -m "fix(backend): resolve database connection timeout

The connection pool was not being properly configured, causing
timeouts under load. Updated connection string settings."

# Breaking change
git commit -m "feat!: redesign workspace structure

BREAKING CHANGE: Workspace IDs have changed from int to Guid."

# Documentation
git commit -m "docs: update API documentation for task endpoints"
```

### Branch Strategy

**Main Branches:**

- `main` - Production-ready code

**Supporting Branches:**

- `develop` - Development integration (not currently used)
- Feature branches - Short-lived branches for features

**Workflow:**

```bash
# 1. Start from main
git checkout main
git pull origin main

# 2. Create feature branch
git checkout -b feature/user-authentication

# 3. Make changes and commit
git add .
git commit -m "feat: add JWT authentication"

# 4. Push to remote
git push origin feature/user-authentication

# 5. Create Pull Request
#    - Target: main
#    - Request review from team

# 6. Address feedback
git add .
git commit -m "fix: address review feedback"

# 7. After approval, squash and merge
#    - Maintainer merges PR
#    - Branch auto-deleted

# 8. Update local main
git checkout main
git pull origin main

# 9. Delete local feature branch
git branch -d feature/user-authentication
```

## Code Quality Standards

### TypeScript Standards

**Type Safety:**

- Always use TypeScript (no `.js` files)
- Avoid `any` type - use `unknown` if type is unknown
- Prefer explicit return types on exported functions
- Use strict null checks (`strictNullChecks: true`)
- Enable no implicit any (`noImplicitAny: true`)

**Best Practices:**

```typescript
// Good
interface User {
  id: string;
  name: string;
  email: string;
}

export async function getUserById(id: string): Promise<User | null> {
  try {
    const response = await fetch(`/api/users/${id}`);
    if (!response.ok) return null;
    return await response.json();
  } catch (error) {
    console.error('Failed to fetch user:', error);
    return null;
  }
}

// Bad
function getUser(id) {
  return fetch(`/api/users/${id}`).then((r) => r.json());
}
```

**Component Standards:**

```typescript
// Good (typed props)
interface ButtonProps {
  label: string;
  onClick: () => void;
  disabled?: boolean;
}

export function Button({ label, onClick, disabled = false }: ButtonProps) {
  return (
    <button onClick={onClick} disabled={disabled}>
      {label}
    </button>
  );
}

// Bad (untyped props)
export function Button(props: any) {
  return <button>{props.label}</button>;
}
```

### C# Standards

**Naming Conventions:**

- Classes: `PascalCase` (e.g., `UserService`)
- Methods: `PascalCase` (e.g., `GetUserById`)
- Properties: `PascalCase` (e.g., `UserName`)
- Local variables: `camelCase` (e.g., `userName`)
- Private fields: `_camelCase` (e.g., `_userRepository`)
- Constants: `PascalCase` (e.g., `MaxRetryCount`)
- Interfaces: `IPascalCase` (e.g., `IUserRepository`)

**Best Practices:**

```csharp
// Good
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private const int MaxRetryCount = 3;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _userRepository.FindByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user with ID {UserId}", id);
            throw;
        }
    }
}

// Bad
public class UserService
{
    IUserRepository userRepo;

    public User GetUser(Guid id)
    {
        return userRepo.FindById(id);
    }
}
```

**Async/Await:**

- Always use `async` in method name for async methods
- Use `CancellationToken` for async operations
- Consider `ConfigureAwait(false)` in library code
- Avoid `async void` (use `async Task`)

```csharp
// Good
public async Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _userRepository.FindByIdAsync(id, cancellationToken)
        .ConfigureAwait(false);
}

// Bad
public async void GetUser(Guid id)
{
    var user = await _userRepository.FindByIdAsync(id);
}
```

**Dependency Injection:**

- Use constructor injection
- Register services in `Program.cs`
- Use appropriate service lifetimes (Scoped, Transient, Singleton)

```csharp
// Good
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserService _userService;

    public TaskService(ITaskRepository taskRepository, IUserService userService)
    {
        _taskRepository = taskRepository;
        _userService = userService;
    }
}

// Registration
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
```

## Testing Standards

### Unit Testing

**Backend:**

- Framework: xUnit
- Mocking: Moq
- Assertions: FluentAssertions

**Frontend:**

- Framework: Jest
- Testing Library: React Testing Library
- Mocking: jest.mock()

**Standards:**

- Arrange-Act-Assert pattern
- Descriptive test names
- Test one thing per test
- Mock external dependencies

**Example (C#):**

```csharp
public class UserServiceTests
{
    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User { Id = userId, Name = "John" };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.FindByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);
        var service = new UserService(mockRepo.Object);

        // Act
        var result = await service.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Name.Should().Be("John");
    }
}
```

**Example (TypeScript):**

```typescript
describe('UserService', () => {
  it('should return user when user exists', async () => {
    // Arrange
    const userId = '123';
    const expectedUser = { id: userId, name: 'John' };
    jest.spyOn(api, 'getUser').mockResolvedValue(expectedUser);

    // Act
    const result = await getUserById(userId);

    // Assert
    expect(result).toEqual(expectedUser);
    expect(api.getUser).toHaveBeenCalledWith(userId);
  });
});
```

## Documentation Standards

### Code Comments

**When to Comment:**

- Explain WHY, not WHAT
- Document complex algorithms
- Explain workarounds
- Document public APIs
- Note TODO items

**Examples:**

```csharp
// Good (explains WHY)
// Using JSONB for custom fields to support flexible task properties
// without schema migrations for each new field
public class Task
{
    public string CustomFieldsJsonb { get; set; }
}

// Bad (states the obvious)
// Get user by ID
public User GetUserById(Guid id) { ... }
```

```typescript
// Good (explains WHY)
// Debounce search to avoid excessive API calls while user types
const debouncedSearch = debounce((query: string) => {
  searchApi(query);
}, 300);
```

### XML Documentation (C#)

```csharp
/// <summary>
/// Service for managing user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    /// <exception cref="NotFoundException">Thrown when user is not found.</exception>
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
```

## Security Standards

### Secrets Management

**Rules:**

- Never commit secrets to repository
- Use environment variables for configuration
- Store secrets in GitHub Secrets (CI/CD)
- Use `.env` files locally (gitignored)

**Example:**

```bash
# .env (gitignored)
DATABASE_CONNECTION_STRING=Host=localhost;Database=nexora;...
JWT_SECRET_KEY=your-secret-key
```

### Input Validation

**Backend:**

```csharp
// Good
public class CreateUserCommand
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

**Frontend:**

```typescript
// Good
const schema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
});
```

---

**Documentation Version:** 1.0
**Maintained By:** Development Team
