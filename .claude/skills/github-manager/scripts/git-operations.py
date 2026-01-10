#!/usr/bin/env python3
"""Automate git operations for GitHub workflow fixes."""

import argparse
import os
import re
import subprocess
import sys
from datetime import datetime

try:
    import requests
except ImportError:
    print("Installing requests...")
    os.system("pip install requests")
    import requests


class GitOperator:
    def __init__(self, repo_path: str = "."):
        self.repo_path = repo_path

    def run_git(self, args: list, check: bool = True) -> subprocess.CompletedProcess:
        """Run git command."""
        cmd = ["git"] + args
        result = subprocess.run(cmd, cwd=self.repo_path, capture_output=True, text=True)
        if check and result.returncode != 0:
            raise RuntimeError(f"Git failed: {result.stderr}")
        return result

    def get_status(self) -> dict:
        """Get git status info."""
        result = self.run_git(["status", "--porcelain"])
        changed = [line for line in result.stdout.strip().split("\n") if line]
        return {
            "changed_files": len(changed),
            "changes": changed,
            "branch": self.run_git(["branch", "--show-current"]).stdout.strip()
        }

    def get_diff(self, file: str = None) -> str:
        """Get diff of changes."""
        args = ["diff"]
        if file:
            args.append(file)
        return self.run_git(args).stdout

    def commit_fix(self, message: str, files: list = None) -> bool:
        """Commit changes with conventional commit format."""
        # Stage files
        if files:
            for f in files:
                self.run_git(["add", f])
        else:
            self.run_git(["add", "-A"])

        # Check if anything to commit
        status = self.run_git(["status", "--porcelain"])
        if not status.stdout.strip():
            print("No changes to commit.")
            return False

        # Commit
        self.run_git(["commit", "-m", message])
        print(f"Committed: {message}")
        return True

    def create_branch(self, branch_name: str, base: str = "main") -> str:
        """Create and checkout new branch."""
        self.run_git(["checkout", "-b", branch_name, base])
        print(f"Created branch: {branch_name}")
        return branch_name

    def push(self, branch: str = None, remote: str = "origin") -> bool:
        """Push commits to remote."""
        args = ["push"]
        if branch:
            args.extend([remote, branch])
            args.extend(["-u", "--set-upstream"])
        result = self.run_git(args, check=False)
        return result.returncode == 0

    def get_commits(self, limit: int = 10) -> list:
        """Get recent commits."""
        result = self.run_git(["log", "-n", str(limit), "--pretty=format:%H|%s|%an|%ar"])
        commits = []
        for line in result.stdout.strip().split("\n"):
            if line:
                parts = line.split("|")
                commits.append({
                    "hash": parts[0][:7],
                    "message": parts[1],
                    "author": parts[2],
                    "time": parts[3]
                })
        return commits

    def suggest_commit_message(self, changes: list) -> str:
        """Suggest conventional commit message based on changes."""
        file_types = {"workflow": [], "docker": [], "code": [], "config": []}

        for change in changes:
            status = change[:2]
            file = change[3:]
            if ".github/workflows" in file:
                file_types["workflow"].append(file)
            elif file.endswith("Dockerfile") or "docker-compose" in file:
                file_types["docker"].append(file)
            elif file.endswith((".yml", ".yaml", ".json", ".toml")):
                file_types["config"].append(file)
            else:
                file_types["code"].append(file)

        if file_types["workflow"]:
            return "ci(workflows): fix workflow errors and add optimizations"
        if file_types["docker"]:
            return "fix(docker): fix Docker build configuration"
        if file_types["config"]:
            return "chore(config): update configuration files"

        return "chore: apply fixes and improvements"


class GitHubPR:
    def __init__(self, token: str, repo: str):
        self.token = token
        self.repo = repo
        self.base_url = "https://api.github.com"
        self.headers = {
            "Authorization": f"token {token}",
            "Accept": "application/vnd.github.v3+json"
        }

    def create_pr(self, title: str, branch: str, base: str = "main", body: str = "") -> dict:
        """Create pull request."""
        url = f"{self.base_url}/repos/{self.repo}/pulls"
        data = {
            "title": title,
            "head": branch,
            "base": base,
            "body": body
        }
        response = requests.post(url, headers=self.headers, json=data)
        response.raise_for_status()
        return response.json()

    def list_prs(self, state: str = "open", limit: int = 10) -> list:
        """List pull requests."""
        url = f"{self.base_url}/repos/{self.repo}/pulls"
        params = {"state": state, "per_page": limit}
        response = requests.get(url, headers=self.headers, params=params)
        response.raise_for_status()
        return response.json()


def format_workflow_commit(error_type: str, details: str) -> str:
    """Format commit message for workflow fixes."""
    templates = {
        "enospc": "ci(workflows): fix ENOSPC error - add Docker layer caching",
        "timeout": "ci(workflows): fix timeout - increase job timeout-minutes",
        "permission": "ci(workflows): fix permissions - scope to minimum required",
        "cache": "ci(workflows): add dependency caching for faster builds",
        "docker": "ci(docker): fix Docker build - optimize multi-stage build",
        "test": "test: fix test failures and add missing tests"
    }
    return templates.get(error_type, f"fix: resolve {error_type} error")


def main():
    parser = argparse.ArgumentParser(description="Automate git operations for GitHub fixes")
    parser.add_argument("--repo-path", default=".", help="Path to git repository")
    parser.add_argument("--status", action="store_true", help="Show git status")
    parser.add_argument("--diff", help="Show diff for file")
    parser.add_argument("--commit", help="Commit message (or use auto-suggest)")
    parser.add_argument("--auto-commit", action="store_true", help="Suggest and commit changes")
    parser.add_argument("--branch", help="Create new branch")
    parser.add_argument("--push", action="store_true", help="Push to remote")
    parser.add_argument("--commits", type=int, help="Show recent N commits")
    parser.add_argument("--token", help="GitHub token for PR operations")
    parser.add_argument("--github-repo", help="GitHub repo (owner/repo) for PR")
    parser.add_argument("--create-pr", help="Create PR with this title")

    args = parser.parse_args()

    git = GitOperator(args.repo_path)

    if args.status:
        status = git.get_status()
        print(f"\nGit Status:")
        print(f"  Branch: {status['branch']}")
        print(f"  Changed files: {status['changed_files']}")
        if status['changes']:
            print(f"  Changes:")
            for change in status['changes']:
                print(f"    {change}")

    elif args.diff:
        print(f"\nDiff for {args.diff}:")
        print("-" * 60)
        print(git.get_diff(args.diff))

    elif args.auto_commit:
        status = git.get_status()
        if status['changed_files'] == 0:
            print("No changes to commit.")
            return
        message = git.suggest_commit_message(status['changes'])
        git.commit_fix(message)
        if args.push:
            git.push()

    elif args.commit:
        git.commit_fix(args.commit)
        if args.push:
            git.push()

    elif args.branch:
        git.create_branch(args.branch)

    elif args.push:
        git.push()

    elif args.commits:
        commits = git.get_commits(args.commits)
        print(f"\nRecent Commits:")
        print("-" * 60)
        for c in commits:
            print(f"  [{c['hash']}] {c['message']}")
            print(f"    {c['author']} - {c['time']}")

    elif args.create_pr:
        token = args.token or os.environ.get("GITHUB_TOKEN")
        if not token or not args.github_repo:
            print("Error: --token and --github-repo required for PR creation")
            sys.exit(1)
        gh = GitHubPR(token, args.github_repo)
        branch = git.get_status()['branch']
        pr = gh.create_pr(args.create_pr, branch)
        print(f"Created PR: {pr['html_url']}")

    else:
        parser.print_help()


if __name__ == "__main__":
    main()
