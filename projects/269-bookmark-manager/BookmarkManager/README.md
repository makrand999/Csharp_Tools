# Bookmark Manager

A CLI tool for managing web bookmarks with tags, search, and URL validation.

## Usage

```bash
# Interactive mode
dotnet run --project BookmarkManager.csproj

# Add bookmark
dotnet run --project BookmarkManager.csproj add https://example.com "Example Site" tech,reference

# List all bookmarks
dotnet run --project BookmarkManager.csproj list

# Search bookmarks
dotnet run --project BookmarkManager.csproj search "tutorial"

# Open bookmark in browser
dotnet run --project BookmarkManager.csproj open abc12345

# Delete bookmark
dotnet run --project BookmarkManager.csproj delete abc12345

# Show all tags
dotnet run --project BookmarkManager.csproj tags

# Validate all URLs
dotnet run --project BookmarkManager.csproj validate

# Export to HTML
dotnet run --project BookmarkManager.csproj export
```

## Example

```
$ dotnet run --project BookmarkManager.csproj add https://docs.microsoft.com "Microsoft Docs" docs,reference

🔖 Bookmark Manager
===================

✅ Bookmark added: Microsoft Docs
   ID: abc12345

$ dotnet run --project BookmarkManager.csproj list

ID         Title                               Tags                      Added
-------------------------------------------------------------------------------------
abc12345   Microsoft Docs                      docs,reference            2024-01-15
def67890   GitHub                              dev,code                  2024-01-14

Total: 2 bookmarks
```

## Features

- **Add bookmarks** - Store URLs with titles and tags
- **Auto-fetch titles** - Automatically fetch page titles from URLs
- **Search** - Find bookmarks by title, URL, or tags
- **Tag organization** - Group and filter by tags
- **URL validation** - Check if bookmarked URLs are still accessible
- **Export** - Export bookmarks to HTML format
- **Open in browser** - Launch URLs directly from CLI

## Data Storage

Bookmarks are stored in `bookmarks.json` in the current directory.

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- HTTP client for URL validation and title fetching
- File I/O and persistence
- Search and filtering with LINQ
- Interactive CLI menus
- HTML generation
- Process starting for opening URLs
