# Git Commit Message Generator

Interactive CLI tool for generating conventional commit messages.

## Usage

```bash
dotnet run --project CommitGenerator.csproj
```

## Example

```
$ dotnet run --project CommitGenerator.csproj

📝 Git Commit Message Generator
================================

Select commit type:
  1. feat
  2. fix
  3. docs
  4. style
  5. refactor
  ...

Choice (1-10): 1

Enter scope (optional, press Enter to skip): api

Enter short description (imperative mood): add user authentication endpoint

Enter body (optional, press Enter to skip): Implements JWT-based authentication for the user API

Enter footer (optional, e.g., 'Closes #123', press Enter to skip): Closes #42

--- Generated Commit Message ---

feat(api): add user authentication endpoint

Implements JWT-based authentication for the user API

Closes #42
```

## Commit Types

| Type | Description |
|------|-------------|
| feat | New feature |
| fix | Bug fix |
| docs | Documentation only |
| style | Formatting, missing semicolons, etc. |
| refactor | Code change that neither fixes a bug nor adds a feature |
| test | Adding missing tests, refactoring tests |
| chore | Updating build tasks, package manager configs, etc. |
| perf | Performance improvements |
| ci | CI configuration changes |
| build | Build system or external dependencies |

## Concepts Demonstrated

- Interactive CLI input handling
- String building
- Conventional commits specification
- File I/O
- Input validation
