#!/usr/bin/env python3
"""Analyze failed GitHub Actions workflow runs and suggest fixes."""

import argparse
import os
import re
import sys

try:
    import requests
except ImportError:
    print("Installing requests...")
    os.system("pip install requests")
    import requests


class WorkflowAnalyzer:
    def __init__(self, token: str, repo: str):
        self.token = token
        self.repo = repo
        self.base_url = "https://api.github.com"
        self.headers = {
            "Authorization": f"token {token}",
            "Accept": "application/vnd.github.v3+json"
        }

    def get_failed_jobs(self, run_id: int):
        """Get failed jobs from a workflow run."""
        url = f"{self.base_url}/repos/{self.repo}/actions/runs/{run_id}/jobs"
        response = requests.get(url, headers=self.headers)
        response.raise_for_status()
        return [j for j in response.json()["jobs"] if j["conclusion"] == "failure"]

    def get_job_logs(self, job_id: int):
        """Get logs for a specific job."""
        url = f"{self.base_url}/repos/{self.repo}/actions/jobs/{job_id}/logs"
        response = requests.get(url, headers=self.headers)
        if response.status_code == 200:
            return response.content
        return None

    def analyze_error(self, logs_text: str):
        """Analyze error logs and suggest fixes."""
        issues = []

        # Common error patterns
        patterns = {
            "ENOSPC": {
                "fix": "Docker disk space full. Run: docker system prune -a",
                "category": "docker"
            },
            "no space left on device": {
                "fix": "Disk space issue. Clean up Docker or increase disk allocation.",
                "category": "docker"
            },
            "CS0234": {
                "fix": "Missing namespace. Add using directive or check project references.",
                "category": "code"
            },
            "error CS": {
                "fix": "C# compilation error. Check syntax, types, and dependencies.",
                "category": "code"
            },
            "npm ERR": {
                "fix": "npm install failed. Check package.json and network connectivity.",
                "category": "node"
            },
            "Cannot find module": {
                "fix": "Missing dependency. Run npm install or check import paths.",
                "category": "node"
            },
            "lstat": {
                "fix": "Docker COPY path error. Check Dockerfile context and file paths.",
                "category": "docker"
            },
            "permission denied": {
                "fix": "File permission issue. Check file permissions or use USER instruction.",
                "category": "docker"
            },
            "Health check": {
                "fix": "Service health check failed. Check service logs and dependencies.",
                "category": "service"
            },
            "database is locked": {
                "fix": "Database connection issue. Check for multiple instances or long transactions.",
                "category": "database"
            }
        }

        for pattern, info in patterns.items():
            if pattern in logs_text:
                issues.append({
                    "pattern": pattern,
                    "fix": info["fix"],
                    "category": info["category"]
                })

        return issues

    def analyze_run(self, run_id: int):
        """Analyze a failed workflow run."""
        print(f"\nAnalyzing Workflow Run {run_id}")
        print("-" * 60)

        failed_jobs = self.get_failed_jobs(run_id)

        if not failed_jobs:
            print("No failed jobs found in this run.")
            return

        for job in failed_jobs:
            print(f"\n[{job['conclusion'].upper()}] {job['name']}")
            print(f"  Started: {job['started_at']}")
            print(f"  Completed: {job['completed_at']}")

            # Try to get logs
            logs = self.get_job_logs(job['id'])
            if logs:
                # Decode logs (zip file)
                import gzip
                import io

                try:
                    log_text = gzip.decompress(logs).decode('utf-8', errors='ignore')
                except:
                    log_text = logs.decode('utf-8', errors='ignore')

                issues = self.analyze_error(log_text)

                if issues:
                    print(f"\n  Issues Found:")
                    for issue in issues:
                        print(f"    [{issue['category']}] {issue['pattern']}")
                        print(f"    Fix: {issue['fix']}")
                else:
                    print(f"\n  No specific issues identified.")
                    print(f"  Check logs: {job['html_url']}")
            else:
                print(f"\n  Logs not available.")
                print(f"  URL: {job['html_url']}")

    def suggest_workflow_fixes(self, workflow_path: str):
        """Suggest fixes for common workflow issues."""
        print(f"\nAnalyzing {workflow_path}")
        print("-" * 60)

        if not os.path.exists(workflow_path):
            print(f"  File not found: {workflow_path}")
            return

        with open(workflow_path, 'r') as f:
            content = f.read()

        suggestions = []

        # Check for common issues
        if '@main' in content or '@latest' in content:
            suggestions.append({
                "issue": "Unpinned action versions",
                "fix": "Replace @main/@latest with specific versions (e.g., @v4)",
                "severity": "high"
            })

        if 'permissions:' not in content:
            suggestions.append({
                "issue": "Missing permissions",
                "fix": "Add permissions block (contents: read, pull-requests: write)",
                "severity": "medium"
            })

        if 'timeout-minutes' not in content:
            suggestions.append({
                "issue": "No job timeout",
                "fix": "Add timeout-minutes to prevent runaway jobs",
                "severity": "low"
            })

        if 'concurrency:' not in content:
            suggestions.append({
                "issue": "No concurrency control",
                "fix": "Add concurrency group to cancel in-progress runs",
                "severity": "low"
            })

        if 'uses: actions/cache' not in content:
            suggestions.append({
                "issue": "No caching",
                "fix": "Add actions/cache for dependencies",
                "severity": "medium"
            })

        if suggestions:
            print(f"  {len(suggestions)} suggestions found:\n")
            for s in suggestions:
                print(f"    [{s['severity'].upper()}] {s['issue']}")
                print(f"    Fix: {s['fix']}\n")
        else:
            print("  No obvious issues found.")


def main():
    parser = argparse.ArgumentParser(description="Analyze failed GitHub Actions workflows")
    parser.add_argument("--token", help="GitHub token (or GITHUB_TOKEN env var)")
    parser.add_argument("--repo", help="Repository (e.g., owner/repo)")
    parser.add_argument("--run-id", type=int, help="Workflow run ID to analyze")
    parser.add_argument("--workflow", help="Workflow file path to analyze")
    parser.add_argument("--list-failed", action="store_true", help="List and analyze failed runs")

    args = parser.parse_args()

    token = args.token or os.environ.get("GITHUB_TOKEN")
    if not token:
        print("Error: GITHUB_TOKEN not set.")
        sys.exit(1)

    if not args.repo:
        print("Error: --repo is required.")
        sys.exit(1)

    analyzer = WorkflowAnalyzer(token, args.repo)

    if args.workflow:
        analyzer.suggest_workflow_fixes(args.workflow)
    elif args.run_id:
        analyzer.analyze_run(args.run_id)
    elif args.list_failed:
        # Get failed runs and analyze each
        from check_workflow import WorkflowChecker
        checker = WorkflowChecker(token, args.repo)
        runs = checker.get_workflow_runs(limit=10)
        failed = [r for r in runs if r["conclusion"] == "failure"][:3]

        for run in failed:
            analyzer.analyze_run(run["id"])
            print()


if __name__ == "__main__":
    main()
