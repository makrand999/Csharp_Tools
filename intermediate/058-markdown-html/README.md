# Markdown to HTML Converter

A CLI tool that converts Markdown text or files to HTML using the Markdig library.

## Usage

```bash
dotnet run --project MarkdownHtml.csproj
```

## Example

```
Markdown to HTML Converter
==========================

Options:
  1. Convert markdown file to HTML
  2. Convert markdown text (inline) to HTML

Choose option (1 or 2): 2

Enter markdown text (type '---' on a new line to finish):
# Hello World
This is **bold** and this is *italic*.
- Item 1
- Item 2
---

--- HTML Output ---

<h1>Hello World</h1>
<p>This is <strong>bold</strong> and this is <em>italic</em>.</p>
<ul>
<li>Item 1</li>
<li>Item 2</li>
</ul>

--- End HTML ---

Save HTML to file? (y/n): y
Enter output file path: output.html
✅ Saved to: output.html

✅ Done!
```

## Concepts Demonstrated

- Markdown parsing with Markdig library
- Pipeline configuration for extensions
- File I/O for reading/writing
- Interactive CLI input handling
- HTML generation from markdown
- Advanced markdown extensions (emoji, autolinks, etc.)
