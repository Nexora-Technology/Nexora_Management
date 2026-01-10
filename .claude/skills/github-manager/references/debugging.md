# Debugging GitHub Actions

Troubleshooting techniques and tools for GitHub Actions workflows.

## Enable Debug Logging

Add repository secrets:
- `ACTIONS_STEP_DEBUG` = `true`
- `ACTIONS_RUNNER_DEBUG` = `true`

Or set in workflow YAML (temporary):
```yaml
env:
  ACTIONS_STEP_DEBUG: ${{ secrets.ACTIONS_STEP_DEBUG }}
  ACTIONS_RUNNER_DEBUG: ${{ secrets.ACTIONS_RUNNER_DEBUG }}
```

## Workflow Commands

```yaml
# Debug messages
run: echo "::debug::Detailed message"

# Warning
run: echo "::warning::Warning message"

# Error (fails step)
run: echo "::error::Error message"

# Set output
run: echo "result=value" >> $GITHUB_OUTPUT
```

## Verbose Git Logging

```yaml
env:
  GIT_CURL_VERBOSE: 1
  GIT_TRACE: 1
  GIT_SSH_COMMAND: "ssh -vvv"
```

## Troubleshooting Steps

1. **Check logs first** - Identify exact failure point
2. **Use visualization graph** - See job execution flow
3. **Review execution time** - Find slow steps
4. **Enable debug mode** - Get detailed logs
5. **Compare with success** - Diff against passing runs

## Common Issues

### Permission Errors
```yaml
permissions:
  contents: read
  pull-requests: write
```

### Path Issues
```yaml
- uses: actions/checkout@v4
  with:
    fetch-depth: 0  # Full history
```

### Cache Misses
```yaml
- uses: actions/cache@v4
  with:
    path: ~/.npm
    key: ${{ runner.os }}-node-${{ hashFiles('**/package-lock.json') }}
    restore-keys: |
      ${{ runner.os }}-node-
```

## Tools

- [act](https://github.com/nektos/act) - Run workflows locally
- [github-actions-viewer](https://github.com/yutiansing/github-actions-viewer) - Visual workflow inspector
- [workflow-quickstart](https://github.com/github/actions-workflow-quickstart) - Example workflows
