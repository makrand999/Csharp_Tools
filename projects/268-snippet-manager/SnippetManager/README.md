# Code Snippet Manager

A CLI tool for managing and organizing code snippets with tags and search functionality.

## Usage

```bash
# Interactive mode
dotnet run --project SnippetManager.csproj

# Add snippet
dotnet run --project SnippetManager.csproj add

# List all snippets
dotnet run --project SnippetManager.csproj list

# Search snippets
dotnet run --project SnippetManager.csproj search "authentication"

# Get snippet by ID
dotnet run --project SnippetManager.csproj get abc12345

# Delete snippet
dotnet run --project SnippetManager.csproj delete abc12345

# Export to markdown
dotnet run --project SnippetManager.csproj export
```

## Example

```
$ dotnet run --project SnippetManager.csproj list

📋 Code Snippet Manager
=======================

ID         Title                          Language     Tags
---------------------------------------------------------------------------
abc12345   JWT Authentication             csharp       auth,jwt,security
def67890   Python List Comprehension      python       python,lists,tips
ghi11111   React useEffect Hook           javascript   react,hooks,effects

Total: 3 snippets
```

## Features

- **Add snippets** - Store code with title, language, and tags
- **Search** - Find snippets by title, code content, or tags
- **Organize** - Tag-based organization
- **Export** - Export all snippets to markdown
- **Persistent storage** - JSON-based storage

## Data Storage

Snippets are stored in `snippets.json` in the current directory.

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- File I/O and persistence
- Search and filtering with LINQ
- Interactive CLI menus
- Data modeling with classes
