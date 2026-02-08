# CSS Validator

Validate CSS syntax and detect common errors.

## Usage

```bash
dotnet run --project 133-css-validator.csproj -- <file.css>
cat styles.css | dotnet run --project 133-css-validator.csproj
```

## Example

```bash
dotnet run --project 133-css-validator.csproj -- styles.css
```

### Sample Output

```
Validating: styles.css

✗ Found 2 issue(s):

  Line 15: Potentially unclosed string
  Line 42: Unknown property: colro
```

## Checks Performed

- Balanced braces
- Unclosed strings
- Unknown property names
- Common syntax errors

## Concepts Demonstrated

- Pattern matching
- Syntax validation
- Error reporting
- CSS structure analysis
- Statistics calculation
