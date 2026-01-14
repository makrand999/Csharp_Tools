# Lorem Ipsum Generator

A CLI tool for generating Lorem Ipsum placeholder text for documents, designs, and testing.

## Usage

```bash
dotnet run --project Lorem.csproj [options]
```

## Options

| Option | Description |
|--------|-------------|
| `--words <n>` | Generate n words (default: 50) |
| `--sentences <n>` | Generate n sentences |
| `--paragraphs <n>` | Generate n paragraphs |
| `--html` | Wrap output in HTML `<p>` tags |
| `--plain` | Plain text output (default) |

## Examples

```bash
# Generate 50 words
dotnet run --project Lorem.csproj --words 50

# Generate 10 sentences
dotnet run --project Lorem.csproj --sentences 10

# Generate 3 paragraphs
dotnet run --project Lorem.csproj --paragraphs 3

# Generate HTML output
dotnet run --project Lorem.csproj --paragraphs 2 --html
```

## Sample Output

```
# Words output (20 words)
lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna

# Sentence output
Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua ut enim ad minim veniam quis nostrud exercitation.

# Paragraph output
Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit voluptate velit esse cillum dolore eu fugiat nulla pariatur.

Excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium.
```

## HTML Output

```html
<p>Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>

<p>Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
```

## Concepts Demonstrated

- Random number generation
- String manipulation and joining
- Array operations
- Command-line argument parsing
- Conditional HTML formatting
- Text generation algorithms
- Class design for options pattern
