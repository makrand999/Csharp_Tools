# Semantic Version Comparator

A CLI tool for comparing, validating, and manipulating semantic versions (SemVer).

## Usage

```bash
# Compare two versions
dotnet run --project SemVer.csproj compare <version1> <version2>

# Validate a version
dotnet run --project SemVer.csproj validate <version>

# Increment a version
dotnet run --project SemVer.csproj increment <version> <major|minor|patch>

# Quick compare (default command)
dotnet run --project SemVer.csproj <version1> <version2>
```

## Examples

```bash
# Compare versions
dotnet run --project SemVer.csproj compare 1.2.3 2.0.0
dotnet run --project SemVer.csproj 1.0.0 1.0.0

# Validate version
dotnet run --project SemVer.csproj validate 1.2.3-alpha.1
dotnet run --project SemVer.csproj validate v2.0.0+build.123

# Increment version
dotnet run --project SemVer.csproj increment 1.2.3 major
dotnet run --project SemVer.csproj increment 1.2.3 minor
dotnet run --project SemVer.csproj increment 1.2.3 patch
```

## Sample Output

```
Comparing: 1.2.3 vs 2.0.0

Version 1: 1.2.3
Version 2: 2.0.0

Result: 1.2.3 < 2.0.0

---

Validating: 1.2.3-beta.1
✓ Valid semantic version
  Major: 1
  Minor: 2
  Patch: 3
  Prerelease: beta.1

---

Incrementing 1.2.3 by minor
Result: 1.3.0
```

## Concepts Demonstrated

- Regular expressions for pattern matching
- Custom class implementation
- Interface implementation (IComparable)
- String parsing and manipulation
- Switch expressions
- Command-line argument handling
- Semantic versioning specification (SemVer 2.0.0)
