# Slack/Discord Message Formatter

Formats text with Slack and Discord markdown syntax.

## Usage

```bash
dotnet run --project MessageFormatter.csproj [options] [message]

Options:
  --bold      Make text bold
  --italic    Make text italic
  --code      Inline code
  --block     Code block
  --quote     Quote block
  --link      Create link
  --strike    Strikethrough
  --all       Show all formats demo
```

## Example

```
$ dotnet run --project MessageFormatter.csproj --bold "Important Notice"

💬 Slack/Discord Message Formatter
==================================

--- Formatted Message ---

*Important Notice*

(Copy and paste into Slack/Discord)

$ dotnet run --project MessageFormatter.csproj --all

=== Slack/Discord Formatting Guide ===

Format          Slack                     Discord
-----------------------------------------------------------------
Bold            *bold*                    **bold**
Italic          _italic_                  *italic*
...
```

## Concepts Demonstrated

- Pattern matching with switch expressions
- String formatting
- Command-line argument parsing
- Multi-platform formatting (Slack vs Discord)
- Interactive input handling
