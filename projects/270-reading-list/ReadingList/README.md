# Reading List Tracker

A CLI tool for tracking books you want to read, are reading, or have completed.

## Usage

```bash
# Interactive mode
dotnet run --project ReadingList.csproj

# Add a book
dotnet run --project ReadingList.csproj add

# List all books
dotnet run --project ReadingList.csproj list

# Update reading status
dotnet run --project ReadingList.csproj status abc12345 completed

# Add rating (1-5 stars)
dotnet run --project ReadingList.csproj rating abc12345 5

# Add notes
dotnet run --project ReadingList.csproj notes abc12345

# Show statistics
dotnet run --project ReadingList.csproj stats

# Delete a book
dotnet run --project ReadingList.csproj delete abc12345
```

## Example

```
$ dotnet run --project ReadingList.csproj add

Title: The Pragmatic Programmer
Author: Andrew Hunt
Pages (optional): 352
Genre (optional): Programming

✅ Book added: The Pragmatic Programmer by Andrew Hunt
   ID: abc12345

$ dotnet run --project ReadingList.csproj stats

📊 Reading Statistics
----------------------------------------
Total books:      15
To read:          8
Currently reading:2
Completed:        5
Rated:            4
Average rating:   4.5/5 ★★★★☆
Pages read:       1,840
```

## Features

- **Track books** - Add books with title, author, pages, and genre
- **Reading status** - To Read, Reading, Completed
- **Ratings** - Rate completed books (1-5 stars)
- **Notes** - Add personal notes and reviews
- **Statistics** - View reading progress and stats
- **Filtering** - Filter books by status

## Data Storage

Books are stored in `reading-list.json` in the current directory.

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- File I/O and persistence
- Interactive CLI menus
- Statistics calculation with LINQ
- Data modeling with classes
- Date/time handling
