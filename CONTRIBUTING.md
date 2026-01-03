# Contributing to Nexora Management

Thank you for your interest in contributing to Nexora Management! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

### Our Pledge

We are committed to making participation in this project a harassment-free experience for everyone, regardless of level of experience, gender, gender identity and expression, sexual orientation, disability, personal appearance, body size, race, ethnicity, age, religion, or nationality.

### Our Standards

- Use welcoming and inclusive language
- Be respectful of differing viewpoints and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

## Development Workflow

### 1. Setup Development Environment

Before contributing, ensure your development environment is properly configured:

```bash
# Clone the repository
git clone <repository-url>
cd Nexora_Management

# Install dependencies
npm install

# Setup git hooks (already configured via npm install)
npx husky install
```

See [Local Setup Guide](docs/development/local-setup.md) for detailed instructions.

### 2. Choose What to Work On

- Check [GitHub Issues](../../issues) for open issues and feature requests
- Look for issues labeled `good first issue` for beginners
- Comment on the issue to express interest and avoid duplication
- For large features, create a proposal issue first for discussion

### 3. Create a Branch

```bash
# Update your main branch
git checkout main
git pull origin main

# Create a feature branch
git checkout -b feature/your-feature-name
# or
git checkout -b fix/bug-description
```

Branch naming conventions:
- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Adding or updating tests
- `chore/` - Maintenance tasks

### 4. Make Your Changes

#### Coding Standards

Follow our [Code Standards](docs/code-standards.md):

- **TypeScript/JavaScript**:
  - Use TypeScript for type safety
  - Follow ESLint rules (run `npm run lint`)
  - Format code with Prettier (run `npm run format`)
  - Write meaningful variable and function names

- **C#/.NET**:
  - Follow C# coding conventions
  - Use async/await for asynchronous operations
  - Write XML documentation comments for public APIs
  - Use proper access modifiers

- **Architecture**:
  - Follow Clean Architecture principles
  - Separate concerns (Core, Application, Infrastructure, API)
  - Use dependency injection
  - Keep business logic independent of frameworks

#### Testing

- Write unit tests for new functionality
- Aim for >80% code coverage
- Test both positive and negative cases
- Mock external dependencies

```bash
# Run backend tests
cd apps/backend
dotnet test

# Run frontend tests
cd apps/frontend
npm test
```

### 5. Commit Your Changes

Use [Conventional Commits](https://www.conventionalcommits.org/):

```bash
# Feature
git commit -m "feat: add user authentication"

# Bug fix
git commit -m "fix: resolve issue with task creation"

# Documentation
git commit -m "docs: update API documentation"

# Breaking change
git commit -m "feat!: redesign workspace structure"
```

Commit types:
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting, etc.)
- `refactor:` - Code refactoring
- `test:` - Adding or updating tests
- `chore:` - Maintenance tasks
- `perf:` - Performance improvements

### 6. Pre-commit Checks

Husky hooks will automatically run lint-staged before each commit:

```bash
# This runs automatically on commit
npx lint-staged
```

If you need to bypass (not recommended):
```bash
git commit --no-verify -m "..."
```

### 7. Push Your Changes

```bash
git push origin feature/your-feature-name
```

### 8. Create a Pull Request

1. Go to the GitHub repository
2. Click "New Pull Request"
3. Select your feature branch
4. Provide a clear title and description
5. Link related issues
6. Request review from maintainers

#### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
Describe testing performed
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added to complex code
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated
- [ ] All tests passing

## Related Issues
Fixes #123
Related to #456
```

### 9. Review Process

- Automated CI checks run on every PR
- Maintainers review your code and provide feedback
- Address review comments by pushing new commits
- Keep the PR history clean (squash if necessary)
- PR must pass all checks before merging

### 10. Merge

Once approved:
- Maintainer will merge your PR
- Use "Squash and merge" for clean history
- Branch is automatically deleted after merge

## Pull Request Guidelines

### Before Submitting

- [ ] Ensure all tests pass locally
- [ ] Run `npm run lint` and fix issues
- [ ] Run `npm run format` to format code
- [ ] Update documentation if needed
- [ ] Add tests for new features
- [ ] Update type definitions if needed

### During Review

- Respond to review comments promptly
- Explain your reasoning for complex changes
- Be open to suggestions and improvements
- Mark conversations as resolved when addressed

### After Merge

- Delete your local and remote feature branches
- Celebrate your contribution! 🎉

## Development Tips

### Running Tests Locally

```bash
# All tests
npm test

# Specific package
cd apps/backend && dotnet test
cd apps/frontend && npm test

# Watch mode
cd apps/frontend && npm test -- --watch
```

### Debugging

- Backend: Use Visual Studio or VS Code debugger
- Frontend: Use Chrome DevTools or React DevTools
- Check browser console for errors
- Use `console.log` sparingly; remove before committing

### Common Issues

**Port already in use:**
```bash
# Kill process on port 3000
npx kill-port 3000
```

**Docker issues:**
```bash
# Rebuild containers
docker-compose down -v
docker-compose up --build
```

## Getting Help

- Check [documentation](docs/)
- Search existing [issues](../../issues)
- Ask in [discussions](../../discussions)
- Contact maintainers

## Recognition

Contributors are recognized in:
- CONTRIBUTORS.md file
- Release notes
- Project website (when applicable)

Thank you for contributing to Nexora Management! 🚀
