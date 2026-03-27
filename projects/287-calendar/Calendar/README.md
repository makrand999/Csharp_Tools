# Calendar Manager

Manage events and appointments with date/time tracking and reminders.

## Usage

```bash
# Interactive mode
dotnet run --project Calendar.csproj

# Add event
dotnet run --project Calendar.csproj add

# List events
dotnet run --project Calendar.csproj list

# Show today's events
dotnet run --project Calendar.csproj today

# Delete event
dotnet run --project Calendar.csproj delete abc12345
```

## Example

```
📅 Calendar Manager
===================

Commands:
  add    - Add new event
  list   - List all events
  today  - Show today's events
  delete - Delete event by ID

Enter command: add

Title: Team Meeting
Date (YYYY-MM-DD): 2024-01-20
Time (HH:MM, optional): 14:00
Location (optional): Conference Room A
Notes (optional): Q1 planning discussion

✅ Event added: Team Meeting
   ID: abc12345
   Date: 2024-01-20 at 14:00
```

```
$ dotnet run --project Calendar.csproj today

📅 Today's Events (Monday, January 15, 2024)

⏰ 09:00 - Morning Standup
   📍 Zoom

⏰ 14:00 - Team Meeting
   📍 Conference Room A
   📝 Q1 planning discussion

Total: 2 event(s) today
```

## Features

- **Event management** - Add, list, delete events
- **Date/time tracking** - Schedule with specific times
- **Location & notes** - Add details to events
- **Today view** - Quick look at today's schedule
- **Month filtering** - Filter events by month
- **Persistent storage** - JSON file storage

## Data Storage

Events are stored in `events.json` in the current directory.

## Concepts Demonstrated

- JSON serialization
- DateTime handling
- File I/O
- LINQ filtering and sorting
- Interactive CLI menus
