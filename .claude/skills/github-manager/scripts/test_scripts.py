#!/usr/bin/env python3
"""Tests for github-manager scripts."""

import importlib.util
import os
import sys
import tempfile
import unittest
from unittest.mock import MagicMock, patch
from pathlib import Path

# Add parent directory to path for imports
SCRIPT_DIR = Path(__file__).parent
sys.path.insert(0, str(SCRIPT_DIR))

# Helper to load modules with hyphens in name
def load_module(name, file_path):
    spec = importlib.util.spec_from_file_location(name, file_path)
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module

# Load all modules
check_workflow = load_module("check_workflow", SCRIPT_DIR / "check-workflow.py")
analyze_workflow = load_module("analyze_workflow", SCRIPT_DIR / "analyze-workflow.py")
git_operations = load_module("git_operations", SCRIPT_DIR / "git-operations.py")
fix_workflow = load_module("fix_workflow", SCRIPT_DIR / "fix-workflow.py")


class TestWorkflowChecker(unittest.TestCase):
    """Test check-workflow.py functionality."""

    def setUp(self):
        """Set up test fixtures."""
        from check_workflow import WorkflowChecker
        self.checker = WorkflowChecker("test_token", "owner/repo")

    def test_init(self):
        """Test WorkflowChecker initialization."""
        self.assertEqual(self.checker.token, "test_token")
        self.assertEqual(self.checker.repo, "owner/repo")
        self.assertEqual(self.checker.base_url, "https://api.github.com")


class TestWorkflowAnalyzer(unittest.TestCase):
    """Test analyze-workflow.py functionality."""

    def setUp(self):
        """Set up test fixtures."""
        from analyze_workflow import WorkflowAnalyzer
        self.analyzer = WorkflowAnalyzer("test_token", "owner/repo")

    def test_init(self):
        """Test WorkflowAnalyzer initialization."""
        self.assertEqual(self.analyzer.token, "test_token")
        self.assertEqual(self.analyzer.repo, "owner/repo")

    def test_analyze_error_enospc(self):
        """Test ENOSPC error detection."""
        logs = "Build failed with ENOSPC error: no space left on device"
        issues = self.analyzer.analyze_error(logs)
        self.assertTrue(any(i['pattern'] == 'ENOSPC' for i in issues))

    def test_analyze_error_cs_error(self):
        """Test C# compilation error detection."""
        logs = "error CS0234: The type or namespace name 'Common' does not exist"
        issues = self.analyzer.analyze_error(logs)
        self.assertTrue(any(i['pattern'] == 'CS0234' for i in issues))

    def test_analyze_error_npm(self):
        """Test npm error detection."""
        logs = "npm ERR! code ENOENT"
        issues = self.analyzer.analyze_error(logs)
        self.assertTrue(any(i['pattern'] == 'npm ERR' for i in issues))


class TestGitOperator(unittest.TestCase):
    """Test git-operations.py functionality."""

    def setUp(self):
        """Set up test fixtures."""
        from git_operations import GitOperator
        # Create temp directory for git operations
        self.temp_dir = tempfile.mkdtemp()
        self.git = GitOperator(self.temp_dir)

    def tearDown(self):
        """Clean up temp directory."""
        import shutil
        shutil.rmtree(self.temp_dir, ignore_errors=True)

    def test_init(self):
        """Test GitOperator initialization."""
        self.assertEqual(self.git.repo_path, self.temp_dir)

    @patch('subprocess.run')
    def test_suggest_commit_message_workflow(self, mock_run):
        """Test commit message suggestion for workflow changes."""
        changes = [" M .github/workflows/build.yml"]
        message = self.git.suggest_commit_message(changes)
        self.assertIn("workflow", message.lower())

    @patch('subprocess.run')
    def test_suggest_commit_message_docker(self, mock_run):
        """Test commit message suggestion for Docker changes."""
        changes = [" M Dockerfile", " M docker-compose.yml"]
        message = self.git.suggest_commit_message(changes)
        self.assertIn("docker", message.lower())


class TestWorkflowFixer(unittest.TestCase):
    """Test fix-workflow.py functionality."""

    def setUp(self):
        """Set up test fixtures."""
        from fix_workflow import WorkflowFixer
        self.temp_dir = tempfile.mkdtemp()
        self.workflow_path = os.path.join(self.temp_dir, "test-workflow.yml")
        self.fixer = WorkflowFixer(self.workflow_path)

    def tearDown(self):
        """Clean up temp directory."""
        import shutil
        shutil.rmtree(self.temp_dir, ignore_errors=True)

    def test_load_missing_file(self):
        """Test loading missing workflow file."""
        with self.assertRaises(FileNotFoundError):
            self.fixer.load()

    def test_load_valid_workflow(self):
        """Test loading valid workflow file."""
        workflow_content = """
name: Test Workflow
on: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@main
"""
        with open(self.workflow_path, 'w') as f:
            f.write(workflow_content)

        self.fixer.load()
        self.assertIsNotNone(self.fixer.data)
        self.assertEqual(self.fixer.data['name'], 'Test Workflow')

    def test_fix_unpinned_actions(self):
        """Test fixing unpinned action versions."""
        workflow_content = """
name: Test Workflow
on: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@main
      - uses: actions/setup-node@latest
"""
        with open(self.workflow_path, 'w') as f:
            f.write(workflow_content)

        self.fixer.load()
        result = self.fixer.fix_unpinned_actions()
        self.assertTrue(result)
        self.assertTrue(any('checkout@v4' in str(f) for f in self.fixer.fixes_applied))

    def test_add_permissions(self):
        """Test adding permissions block."""
        workflow_content = """
name: Test Workflow
on: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps: []
"""
        with open(self.workflow_path, 'w') as f:
            f.write(workflow_content)

        self.fixer.load()
        result = self.fixer.add_permissions()
        self.assertTrue(result)
        self.assertIn('permissions', self.fixer.data)

    def test_add_timeout(self):
        """Test adding timeout to jobs."""
        workflow_content = """
name: Test Workflow
on: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps: []
"""
        with open(self.workflow_path, 'w') as f:
            f.write(workflow_content)

        self.fixer.load()
        result = self.fixer.add_timeout(30)
        self.assertTrue(result)
        self.assertEqual(self.fixer.data['jobs']['build']['timeout-minutes'], 30)

    def test_add_concurrency(self):
        """Test adding concurrency control."""
        workflow_content = """
name: Test Workflow
on: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps: []
"""
        with open(self.workflow_path, 'w') as f:
            f.write(workflow_content)

        self.fixer.load()
        result = self.fixer.add_concurrency()
        self.assertTrue(result)
        self.assertIn('concurrency', self.fixer.data)


def run_tests():
    """Run all tests."""
    # Create test suite
    loader = unittest.TestLoader()
    suite = unittest.TestSuite()

    # Add test classes
    suite.addTests(loader.loadTestsFromTestCase(TestWorkflowChecker))
    suite.addTests(loader.loadTestsFromTestCase(TestWorkflowAnalyzer))
    suite.addTests(loader.loadTestsFromTestCase(TestGitOperator))
    suite.addTests(loader.loadTestsFromTestCase(TestWorkflowFixer))

    # Run tests
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)

    return result.wasSuccessful()


if __name__ == "__main__":
    success = run_tests()
    sys.exit(0 if success else 1)
