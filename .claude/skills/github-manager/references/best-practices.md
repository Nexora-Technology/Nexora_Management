# GitHub Actions Best Practices

Security, caching, and optimization guidelines.

## Security

### Pin Action Versions
```yaml
# ✅ Good - Pinned versions
- uses: actions/checkout@v4
- uses: actions/setup-node@v4
- uses: actions/cache@v4

# ❌ Bad - Moving targets
- uses: actions/checkout@main
- uses: actions/setup-node@latest
```

### Use SHA Pins (Critical)
```yaml
- uses: actions/checkout@v4
  with:
    ref: 'abc123def456...' # Commit SHA
```

### Permissions Scoping
```yaml
permissions:
  contents: read
  pull-requests: write
  issues: write  # Only grant what's needed
```

### Secret Management
- Never log secrets (auto-redacted by GitHub)
- Use environment protection rules
- Rotate secrets regularly
- Use organization secrets for shared values

## Caching Strategies

### Dependencies
```yaml
- name: Cache node modules
  uses: actions/cache@v4
  with:
    path: ~/.npm
    key: ${{ runner.os }}-node-${{ hashFiles('**/package-lock.json') }}
```

### .NET
```yaml
- name: Cache NuGet packages
  uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

### Docker
```yaml
- name: Cache Docker layers
  uses: actions/cache@v4
  with:
    path: /tmp/.buildxcache
    key: ${{ runner.os }}-buildx-${{ github.sha }}
```

## Performance

### Concurrency Control
```yaml
concurrency:
  group: ${{ github.workflow }}-${{ github.ref }}
  cancel-in-progress: true
```

### Job Timeout
```yaml
jobs:
  build:
    timeout-minutes: 30
```

### Matrix Strategy (Parallel)
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    node: [18, 20, 22]
  fail-fast: false
```

## Testing Integration

```yaml
- name: Run tests
  run: dotnet test --logger "trx;LogFileName=test-results.trx"

- name: Upload test results
  if: always()
  uses: actions/upload-artifact@v4
  with:
    name: test-results
    path: TestResults/*.trx
```

## Deployment

### Branch/Tag Deployment
```yaml
deploy:
  if: github.ref == 'refs/heads/main'
  runs-on: ubuntu-latest
```

### Environment Protection
```yaml
deploy:
  environment: production
  runs-on: ubuntu-latest
```
