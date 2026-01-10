---
name: github-manager
description: Manage GitHub workflows, actions, commits, and CI/CD operations. Use for workflow debugging, Docker integration, log analysis, and automating git operations. Includes tools to check workflow status, fix workflow files, and optimize Docker files for GitHub Actions.
license: MIT
version: 1.0.0
---

# GitHub Manager

Manage GitHub Actions workflows, debug CI/CD failures, automate git operations, and integrate Docker containers.

## When to Use

- Debugging failed GitHub Actions workflows
- Analyzing workflow logs and errors
- Creating or fixing workflow YAML files
- Optimizing Docker files for GitHub Actions
- Automating commits, pushes, and PR operations
- Managing secrets and environment variables
- Setting up workflow caching and dependencies

## Core Operations

### Workflow Management

To check workflow run status, use `scripts/check-workflow.py`.
To analyze failed workflows, use `scripts/analyze-workflow.py`.

For workflow syntax and triggers, see `references/workflows.md`.
For Docker action patterns, see `references/docker-actions.md`.

### Debugging

Enable debug logging by adding secrets to repository:
- `ACTIONS_STEP_DEBUG` = `true`
- `ACTIONS_RUNNER_DEBUG` = `true`

For detailed debugging techniques, see `references/debugging.md`.

### Common Workflow Patterns

```yaml
# Cache dependencies
- uses: actions/cache@v4
  with:
    path: ~/.npm
    key: ${{ runner.os }}-node-${{ hashFiles('**/package-lock.json') }}

# Pin action versions (good practice)
- uses: actions/checkout@v4
- uses: actions/setup-dotnet@v4

# Job dependencies
jobs:
  build:
    steps: [...]
  test:
    needs: build
    steps: [...]
```

### Git Operations

To automate commits and pushes, use `scripts/git-operations.py`.

## Scripts

- `scripts/check-workflow.py` - Check workflow run status and logs
- `scripts/analyze-workflow.py` - Analyze workflow failures
- `scripts/git-operations.py` - Automate git operations
- `scripts/fix-workflow.py` - Fix common workflow issues

## References

- `workflows.md` - Workflow syntax, triggers, and patterns
- `debugging.md` - Troubleshooting techniques and tools
- `docker-actions.md` - Docker integration patterns
- `best-practices.md` - Security, caching, and optimization
