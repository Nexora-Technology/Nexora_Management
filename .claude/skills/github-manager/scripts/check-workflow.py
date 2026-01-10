#!/usr/bin/env python3
"""Check GitHub Actions workflow run status and logs."""

import argparse
import os
import sys
from datetime import datetime

try:
    import requests
except ImportError:
    print("Installing requests...")
    os.system("pip install requests")
    import requests


class WorkflowChecker:
    def __init__(self, token: str, repo: str):
        self.token = token
        self.repo = repo
        self.base_url = "https://api.github.com"
        self.headers = {
            "Authorization": f"token {token}",
            "Accept": "application/vnd.github.v3+json"
        }

    def get_workflow_runs(self, workflow_name: str = None, limit: int = 10):
        """Get recent workflow runs."""
        if workflow_name:
            url = f"{self.base_url}/repos/{self.repo}/actions/workflows/{workflow_name}/runs"
        else:
            url = f"{self.base_url}/repos/{self.repo}/actions/runs"

        params = {"per_page": limit, "status": "all"}
        response = requests.get(url, headers=self.headers, params=params)
        response.raise_for_status()
        return response.json()["workflow_runs"]

    def get_run_logs(self, run_id: int):
        """Get logs for a specific run."""
        url = f"{self.base_url}/repos/{self.repo}/actions/runs/{run_id}/logs"
        response = requests.get(url, headers=self.headers)
        response.raise_for_status()
        return response.content

    def get_jobs(self, run_id: int):
        """Get jobs for a workflow run."""
        url = f"{self.base_url}/repos/{self.repo}/actions/runs/{run_id}/jobs"
        response = requests.get(url, headers=self.headers)
        response.raise_for_status()
        return response.json()["jobs"]

    def check_run_status(self, run_id: int):
        """Check detailed status of a workflow run."""
        jobs = self.get_jobs(run_id)

        print(f"\nWorkflow Run {run_id} - Job Status:")
        print("-" * 60)

        for job in jobs:
            status = job["conclusion"] or job["status"]
            name = job["name"]
            print(f"  [{status.upper()}] {name}")

            if job["conclusion"] not in ["success", None]:
                print(f"    Started: {job['started_at']}")
                print(f"    Completed: {job['completed_at']}")
                print(f"    Steps: {job['steps']}")

    def list_failed_runs(self, limit: int = 5):
        """List recent failed workflow runs."""
        runs = self.get_workflow_runs(limit=limit * 2)

        failed = [r for r in runs if r["conclusion"] == "failure"][:limit]

        if not failed:
            print("No failed runs found.")
            return

        print(f"\nRecent Failed Workflow Runs:")
        print("-" * 60)

        for run in failed:
            print(f"  [{run['conclusion'].upper()}] {run['name']}")
            print(f"    Run ID: {run['id']}")
            print(f"    Event: {run['event']}")
            print(f"    Branch: {run.get('head_branch', 'N/A')}")
            print(f"    Triggered by: {run.get('triggering_actor', {}).get('login', 'N/A')}")
            print(f"    Time: {run['created_at']}")
            print()

    def print_run_summary(self, run_id: int):
        """Print summary of a workflow run."""
        runs = self.get_workflow_runs()
        run = next((r for r in runs if r["id"] == run_id), None)

        if not run:
            print(f"Run {run_id} not found.")
            return

        print(f"\nWorkflow Run Summary:")
        print("-" * 60)
        print(f"  Name: {run['name']}")
        print(f"  Status: {run['status'].upper()}")
        print(f"  Conclusion: {(run.get('conclusion') or 'PENDING').upper()}")
        print(f"  Event: {run['event']}")
        print(f"  Branch: {run.get('head_branch', 'N/A')}")
        print(f"  Commit: {run.get('head_sha', 'N/A')[:7]}")
        print(f"  Triggered by: {run.get('triggering_actor', {}).get('login', 'N/A')}")
        print(f"  Created: {run['created_at']}")
        print(f"  Updated: {run['updated_at']}")
        print(f"  URL: {run['html_url']}")


def main():
    parser = argparse.ArgumentParser(description="Check GitHub Actions workflow status")
    parser.add_argument("--token", help="GitHub personal access token (or GITHUB_TOKEN env var)")
    parser.add_argument("--repo", help="Repository (e.g., owner/repo)")
    parser.add_argument("--workflow", help="Workflow filename (e.g., build.yml)")
    parser.add_argument("--run-id", type=int, help="Specific run ID to check")
    parser.add_argument("--list-failed", action="store_true", help="List recent failed runs")
    parser.add_argument("--limit", type=int, default=10, help="Number of runs to list")

    args = parser.parse_args()

    token = args.token or os.environ.get("GITHUB_TOKEN")
    if not token:
        print("Error: GITHUB_TOKEN not set. Use --token or set environment variable.")
        sys.exit(1)

    if not args.repo:
        print("Error: --repo is required (e.g., owner/repo)")
        sys.exit(1)

    checker = WorkflowChecker(token, args.repo)

    if args.list_failed:
        checker.list_failed_runs(args.limit)
    elif args.run_id:
        checker.print_run_summary(args.run_id)
        checker.check_run_status(args.run_id)
    else:
        runs = checker.get_workflow_runs(args.workflow, args.limit)
        print(f"\nRecent Workflow Runs (limit {args.limit}):")
        print("-" * 60)
        for run in runs[:args.limit]:
            status = run["conclusion"] or run["status"]
            print(f"  [{status.upper()}] {run['name']} - {run['created_at']}")


if __name__ == "__main__":
    main()
