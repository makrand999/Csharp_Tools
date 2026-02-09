# Markdown Table of Contents Generator

Generate a table of contents from markdown headings.

## Usage

```bash
dotnet run --project 135-markdown-toc-generator.csproj -- <file.md>
dotnet run --project 135-markdown-toc-generator.csproj -- <file.md> --output <output.md>
```

## Example

```bash
dotnet run --project 135-markdown-toc-generator.csproj -- README.md
```

### Sample Output

```markdown
- [Introduction](#introduction)
  - [Getting Started](#getting-started)
  - [Installation](#installation)
- [Usage](#usage)
  - [Basic Usage](#basic-usage)
  - [Advanced Usage](#advanced-usage)
- [API Reference](#api-reference)
```

## Features

- Automatic anchor generation (GitHub-style)
- Proper nesting based on heading levels
- Optional file output with TOC insertion

## Concepts Demonstrated

- Markdown parsing
- String manipulation
- Anchor generation
- File manipulation
- Recursive structure building
