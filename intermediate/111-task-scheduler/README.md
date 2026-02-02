# TaskScheduler

CLI task management tool with JSON persistence. Add, list, complete, and delete tasks with priority levels and due dates.

## Usage

```bash
dotnet run --project TaskScheduler.csproj -- <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <title>` | Add a new task |
| `list` | List all tasks |
| `complete <id>` | Mark task as complete |
| `delete <id>` | Delete a task |
| `clear` | Remove all completed tasks |

## Options

### Add Command
- `--due YYYY-MM-DD` - Set due date
- `--priority low|medium|high` - Set priority (default: medium)

### List Command
- `--pending` - Show only pending tasks
- `--overdue` - Show only overdue tasks

## Examples

```bash
# Add a task with due date and priority
dotnet run --project TaskScheduler.csproj -- add "Buy groceries" --due 2026-04-05 --priority high

# List pending tasks
dotnet run --project TaskScheduler.csproj -- list --pending

# Mark task as complete
dotnet run --project TaskScheduler.csproj -- complete 3

# Delete a task
dotnet run --project TaskScheduler.csproj -- delete 2

# Clear completed tasks
dotnet run --project TaskScheduler.csproj -- clear
```

## Sample Output

```
ID   Status   Priority   Due          Title
------------------------------------------------------------
1    ○ Pending  🔴 High    2026-04-05   Buy groceries
2    ✓ Done    🟢 Low     -            Read documentation
```

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- Command-line argument parsing
- File I/O for data persistence
- LINQ for filtering and sorting
- Nullable reference types
- Pattern matching with switch expressions
- Required properties (C# 11+)
