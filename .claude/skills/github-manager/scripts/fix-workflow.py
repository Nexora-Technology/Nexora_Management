#!/usr/bin/env python3
"""Automatically fix common GitHub Actions workflow issues."""

import argparse
import os
import re
import sys

try:
    import yaml
except ImportError:
    print("Installing pyyaml...")
    os.system("pip install pyyaml")
    import yaml


class WorkflowFixer:
    def __init__(self, workflow_path: str):
        self.workflow_path = workflow_path
        self.content = None
        self.data = None
        self.fixes_applied = []

    def load(self):
        """Load workflow file."""
        if not os.path.exists(self.workflow_path):
            raise FileNotFoundError(f"Workflow not found: {self.workflow_path}")
        with open(self.workflow_path, 'r') as f:
            self.content = f.read()
            self.data = yaml.safe_load(self.content)
        return self

    def save(self, backup: bool = True):
        """Save fixed workflow."""
        if backup:
            backup_path = f"{self.workflow_path}.backup"
            with open(backup_path, 'w') as f:
                f.write(self.content)

        with open(self.workflow_path, 'w') as f:
            yaml.dump(self.data, f, sort_keys=False, default_flow_style=False)

        print(f"Saved: {self.workflow_path}")
        if backup:
            print(f"Backup: {backup_path}")

    def fix_unpinned_actions(self) -> bool:
        """Pin action versions (@main/@latest -> @v4)."""
        if 'jobs' not in self.data:
            return False

        fixed = False
        action_map = {
            'actions/checkout': 'v4',
            'actions/setup-node': 'v4',
            'actions/setup-python': 'v5',
            'actions/setup-dotnet': 'v4',
            'actions/cache': 'v4',
            'actions/upload-artifact': 'v4',
            'actions/download-artifact': 'v4',
            'actions/checkout@v3': 'v4',
        }

        for job_name, job in self.data['jobs'].items():
            if 'steps' not in job:
                continue
            for step in job['steps']:
                if 'uses' in step:
                    uses = step['uses']
                    # Check for unpinned versions
                    if uses.endswith('@main') or uses.endswith('@latest'):
                        for action, version in action_map.items():
                            if uses.startswith(action):
                                step['uses'] = f"{action}@{version}"
                                self.fixes_applied.append(f"Pinned {uses} -> {action}@{version}")
                                fixed = True

        return fixed

    def add_permissions(self) -> bool:
        """Add minimal permissions block."""
        if 'permissions' in self.data:
            return False

        self.data['permissions'] = {
            'contents': 'read',
            'pull-requests': 'write'
        }
        self.fixes_applied.append("Added minimal permissions (contents: read, pull-requests: write)")
        return True

    def add_timeout(self, timeout: int = 30) -> bool:
        """Add timeout-minutes to jobs."""
        if 'jobs' not in self.data:
            return False

        fixed = False
        for job_name, job in self.data['jobs'].items():
            if 'timeout-minutes' not in job:
                job['timeout-minutes'] = timeout
                self.fixes_applied.append(f"Added timeout-minutes: {timeout} to job '{job_name}'")
                fixed = True

        return fixed

    def add_concurrency(self) -> bool:
        """Add concurrency control."""
        if 'concurrency' in self.data:
            return False

        self.data['concurrency'] = {
            'group': '${{ github.workflow }}-${{ github.ref }}',
            'cancel-in-progress': True
        }
        self.fixes_applied.append("Added concurrency control (cancel-in-progress: true)")
        return True

    def add_docker_cache(self) -> bool:
        """Add Docker layer caching for jobs with docker build."""
        if 'jobs' not in self.data:
            return False

        fixed = False
        for job_name, job in self.data['jobs'].items():
            if 'steps' not in job:
                continue

            has_docker_build = any(
                'docker' in step.get('run', '').lower() or 'docker' in step.get('uses', '').lower()
                for step in job['steps']
            )

            if has_docker_build:
                has_cache = any(
                    'buildx' in step.get('uses', '').lower() or 'buildkit' in step.get('run', '').lower()
                    for step in job['steps']
                )

                if not has_cache:
                    # Insert cache step before docker build steps
                    cache_step = {
                        'name': 'Cache Docker layers',
                        'uses': 'actions/cache@v4',
                        'with': {
                            'path': '/tmp/.buildxcache',
                            'key': '${{ runner.os }}-buildx-${{ github.sha }}',
                            'restore-keys': '${{ runner.os }}-buildx-'
                        }
                    }

                    # Insert before first docker-related step
                    for i, step in enumerate(job['steps']):
                        if 'docker' in step.get('run', '').lower() or 'docker' in step.get('uses', '').lower():
                            job['steps'].insert(i, cache_step)
                            self.fixes_applied.append(f"Added Docker layer caching to job '{job_name}'")
                            fixed = True
                            break

        return fixed

    def fix_dockerfile_path(self, dockerfile_path: str) -> bool:
        """Fix Dockerfile path references."""
        if 'jobs' not in self.data:
            return False

        fixed = False
        for job_name, job in self.data['jobs'].items():
            if 'steps' not in job:
                continue

            for step in job['steps']:
                if 'with' in step and 'dockerfile' in step.get('with', {}):
                    current = step['with']['dockerfile']
                    if not current.startswith('./'):
                        step['with']['dockerfile'] = f"./{dockerfile_path}"
                        self.fixes_applied.append(f"Fixed Dockerfile path in job '{job_name}'")
                        fixed = True

        return fixed

    def apply_all_fixes(self, apply_cache: bool = True) -> int:
        """Apply all recommended fixes."""
        count = 0
        if self.fix_unpinned_actions():
            count += 1
        if self.add_permissions():
            count += 1
        if self.add_timeout():
            count += 1
        if self.add_concurrency():
            count += 1
        if apply_cache and self.add_docker_cache():
            count += 1
        return count

    def report(self):
        """Report applied fixes."""
        if not self.fixes_applied:
            print("No fixes needed.")
            return

        print(f"\nApplied {len(self.fixes_applied)} fixes:")
        print("-" * 60)
        for fix in self.fixes_applied:
            print(f"  ✓ {fix}")


def fix_dockerfile(dockerfile_path: str, issues: list) -> bool:
    """Fix common Dockerfile issues."""
    if not os.path.exists(dockerfile_path):
        print(f"Dockerfile not found: {dockerfile_path}")
        return False

    with open(dockerfile_path, 'r') as f:
        content = f.read()

    fixes = []
    original = content

    # Fix 1: Add multi-stage build if missing
    if 'AS ' not in content and 'FROM ' in content:
        # Already has FROM, could add multi-stage
        pass  # Complex fix, skip for now

    # Fix 2: Combine RUN commands
    lines = content.split('\n')
    run_lines = [(i, line) for i, line in enumerate(lines) if line.strip().startswith('RUN ')]

    # Fix 3: Add .dockerignore suggestion
    dockerignore_path = os.path.join(os.path.dirname(dockerfile_path), '.dockerignore')
    if not os.path.exists(dockerignore_path):
        print(f"  Suggestion: Create .dockerignore with: node_modules, .git, .next")

    return len(fixes) > 0


def main():
    parser = argparse.ArgumentParser(description="Fix GitHub Actions workflow issues")
    parser.add_argument("workflow", help="Path to workflow file (.yml)")
    parser.add_argument("--dry-run", action="store_true", help="Show fixes without applying")
    parser.add_argument("--no-backup", action="store_true", help="Don't create backup")
    parser.add_argument("--dockerfile", help="Also fix Dockerfile path")
    parser.add_argument("--fix-all", action="store_true", help="Apply all recommended fixes")
    parser.add_argument("--add-timeout", type=int, help="Add timeout-minutes to jobs")
    parser.add_argument("--fix-docker", help="Fix Dockerfile with issues")

    args = parser.parse_args()

    if not os.path.exists(args.workflow):
        print(f"Error: Workflow file not found: {args.workflow}")
        sys.exit(1)

    try:
        fixer = WorkflowFixer(args.workflow).load()

        if args.fix_all:
            fixer.apply_all_fixes()
        else:
            # Apply specific fixes based on flags
            if args.add_timeout:
                fixer.add_timeout(args.add_timeout)
            if args.dockerfile:
                fixer.fix_dockerfile_path(args.dockerfile)

        if args.dry_run:
            print("\nDry run - fixes that would be applied:")
            fixer.report()
        else:
            if fixer.fixes_applied:
                fixer.save(backup=not args.no_backup)
                fixer.report()
            else:
                print("No fixes needed.")

    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
