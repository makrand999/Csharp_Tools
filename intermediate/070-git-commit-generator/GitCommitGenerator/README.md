# Git Commit Message Generator

An interactive CLI tool that generates commit messages following the [Conventional Commits](https://www.conventionalcommits.org) specification.

## Usage

```bash
dotnet run --project GitCommitGenerator.csproj
```

## Commit Types

| Type | Description |
|------|-------------|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation only changes |
| `style` | Code style changes (formatting, etc) |
| `refactor` | Code refactoring without behavior change |
| `perf` | Performance improvements |
| `test` | Adding or updating tests |
| `chore` | Maintenance tasks, build config |
| `ci` | CI/CD configuration changes |
| `build` | Build system or external dependencies |

## Example Session

```
Git Commit Message Generator
Conventional Commits Format: https://www.conventionalcommits.org
------------------------------------------------------------

Select commit type:
  1.       feat - A new feature
  2.        fix - A bug fix
  3.       docs - Documentation only changes
  ...

Enter choice (1-10): 1
Enter scope (optional, press Enter to skip): auth
Enter commit message (imperative mood, e.g., 'add feature'): add OAuth2 login support
Enter breaking changes details (optional, press Enter to skip):
Enter issue reference (optional, e.g., #123): #456

------------------------------------------------------------
Generated Commit Message:
------------------------------------------------------------
feat(auth): add OAuth2 login support

#456
------------------------------------------------------------
```

## Example with Breaking Change

```
feat(api): migrate to GraphQL

BREAKING CHANGE: REST endpoints are deprecated, use GraphQL instead

#789
```

## Concepts Demonstrated

- Interactive console input
- String building with StringBuilder
- Conventional Commits specification
- Input validation
- Optional parameters handling
- Structured data with anonymous types
