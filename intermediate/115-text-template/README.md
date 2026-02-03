# TextTemplateEngine

Simple template engine with support for variables, loops, and conditional sections. Render text templates with dynamic data from JSON or key-value pairs.

## Usage

```bash
dotnet run --project TextTemplate.csproj -- render <template-file> [data]
dotnet run --project TextTemplate.csproj -- validate <template-file>
```

## Template Syntax

| Syntax | Description |
|--------|-------------|
| `{{variable}}` | Insert variable value |
| `{{#list}}...{{/list}}` | Loop over list items |
| `{{.}}` | Current item in loop |
| `{{^condition}}...{{/condition}}` | Inverted section (if false/empty) |
| `{{else}}` | Else clause |

## Data Formats

**JSON:**
```bash
dotnet run -- render template.txt '{"name": "John", "age": 30}'
```

**Key-Value pairs:**
```bash
dotnet run -- render template.txt name=John age=30
```

**Using --vars flag:**
```bash
dotnet run -- render template.txt --vars name=John age=30
```

## Examples

### Basic Variables

**Template (greeting.txt):**
```
Hello, {{name}}!
You are {{age}} years old.
```

```bash
dotnet run -- render greeting.txt name=Alice age=25
```

**Output:**
```
Hello, Alice!
You are 25 years old.
```

### Loops

**Template (list.txt):**
```
Shopping List:
{{#items}}
- {{.}}
{{/items}}
```

```bash
dotnet run -- render list.txt --vars items='["Apples","Bananas","Oranges"]'
```

**Output:**
```
Shopping List:
- Apples
- Bananas
- Oranges
```

### Conditional Sections

**Template (messages.txt):**
```
{{^messages}}
No new messages.
{{else}}
You have messages:
{{#messages}}
  * {{.}}
{{/messages}}
{{/messages}}
```

## Commands

| Command | Description |
|---------|-------------|
| `render` | Render a template with data |
| `validate` | Validate template syntax |

## Concepts Demonstrated

- Regular expressions for template parsing
- Dictionary-based variable storage
- JSON parsing without external libraries
- Template engine design patterns
- Loop and conditional processing
- String building and manipulation
