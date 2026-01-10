# GitHub Actions Workflows

Workflow syntax, triggers, and common patterns for GitHub Actions.

## Core Structure

```yaml
name: Workflow Name
on:
  push:
    branches: [main]
  pull_request:
    types: [opened, synchronize]

permissions:
  contents: read
  pull-requests: write

jobs:
  job-name:
    runs-on: ubuntu-latest
    timeout-minutes: 10
    steps:
      - uses: actions/checkout@v4
```

## Common Triggers

| Trigger | Use Case |
|---------|----------|
| `push` | Branch updates, tags |
| `pull_request` | PR validation |
| `schedule` | Cron jobs |
| `workflow_dispatch` | Manual trigger |
| `workflow_call` | Reusable workflows |

## Event Filtering

```yaml
on:
  push:
    branches:
      - main
      - 'releases/**'
    paths:
      - 'apps/backend/**'
      - '.github/workflows/*.yml'
    tags:
      - 'v*'
```

## Job Patterns

### Parallel Jobs
```yaml
jobs:
  backend-tests:
    runs-on: ubuntu-latest
  frontend-tests:
    runs-on: ubuntu-latest
```

### Sequential Jobs
```yaml
jobs:
  build:
    steps: [...]
  test:
    needs: build
    steps: [...]
  deploy:
    needs: test
    if: success() && github.ref == 'refs/heads/main'
```

### Matrix Strategy
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest]
    node: [18, 20]
```

## Key Job Properties

- `runs-on` - Runner environment
- `needs` - Job dependencies
- `if` - Conditional execution
- `timeout-minutes` - Max duration
- `concurrency` - Cancel in-progress runs
