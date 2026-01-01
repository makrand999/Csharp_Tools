# FileOrganizer

Organizes scattered files in a directory by moving them into folders based on their file extension.

## Usage

```bash
# Organize current directory
dotnet run --project FileOrganizer.csproj

# Organize a specific directory
dotnet run --project FileOrganizer.csproj -- /path/to/dir
```

## Example

**Before:**
```
file.txt, image.png, doc.pdf, script.py
```

**After:**
```
TXT/file.txt
PNG/image.png
PDF/doc.pdf
PY/script.py
```

## Build

```bash
dotnet build -c Release
```
