using System.Text.Json;

var dataFile = "tasks.json";
var tasks = LoadTasks(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddTask(args.Skip(1).ToArray());
        break;
    case "list":
        ListTasks(args.Skip(1).ToArray());
        break;
    case "complete":
        CompleteTask(args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteTask(args.Skip(1).ToArray());
        break;
    case "clear":
        ClearCompleted();
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void AddTask(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- add <title> [--due YYYY-MM-DD] [--priority low|medium|high]");
        return;
    }

    var title = args[0];
    var dueDate = DateTime.MinValue;
    var priority = "medium";

    for (var i = 1; i < args.Length; i++)
    {
        if (args[i] == "--due" && i + 1 < args.Length)
        {
            if (DateTime.TryParse(args[++i], out var parsed))
                dueDate = parsed;
        }
        else if (args[i] == "--priority" && i + 1 < args.Length)
        {
            priority = args[++i].ToLower();
            var validPriorities = new[] { "low", "medium", "high" };
            if (!validPriorities.Contains(priority))
                priority = "medium";
        }
    }

    var task = new TaskItem
    {
        Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1,
        Title = title,
        DueDate = dueDate == DateTime.MinValue ? null : dueDate,
        Priority = priority,
        CreatedAt = DateTime.Now,
        Completed = false
    };

    tasks.Add(task);
    SaveTasks(dataFile);
    Console.WriteLine($"✓ Task #{task.Id} added: {title}");
}

void ListTasks(string[] args)
{
    var filter = args.Contains("--pending") ? false : (bool?)null;
    var showOverdue = args.Contains("--overdue");

    var query = tasks.AsQueryable();

    if (filter.HasValue)
        query = query.Where(t => t.Completed == filter.Value);

    var result = query.OrderByDescending(t => t.Priority == "high")
                      .ThenBy(t => t.DueDate)
                      .ToList();

    if (showOverdue)
        result = result.Where(t => !t.Completed && t.DueDate < DateTime.Today).ToList();

    if (result.Count == 0)
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"ID",-4} {"Status",-8} {"Priority",-8} {"Due",-12} Title");
    Console.WriteLine(new string('-', 60));

    foreach (var task in result)
    {
        var status = task.Completed ? "✓ Done" : "○ Pending";
        var priority = task.Priority switch
        {
            "high" => "🔴 High",
            "medium" => "🟡 Med",
            "low" => "🟢 Low",
            _ => task.Priority
        };
        var due = task.DueDate?.ToString("yyyy-MM-dd") ?? "-";
        var title = task.Title.Length > 30 ? task.Title[..27] + "..." : task.Title;

        Console.WriteLine($"{task.Id,-4} {status,-8} {priority,-10} {due,-12} {title}");
    }
    Console.WriteLine();
}

void CompleteTask(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- complete <id>");
        return;
    }

    if (int.TryParse(args[0], out var id))
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Completed = true;
            task.CompletedAt = DateTime.Now;
            SaveTasks(dataFile);
            Console.WriteLine($"✓ Task #{id} marked as complete");
        }
        else
        {
            Console.WriteLine($"Task #{id} not found");
        }
    }
}

void DeleteTask(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- delete <id>");
        return;
    }

    if (int.TryParse(args[0], out var id))
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            tasks.Remove(task);
            SaveTasks(dataFile);
            Console.WriteLine($"✓ Task #{id} deleted");
        }
        else
        {
            Console.WriteLine($"Task #{id} not found");
        }
    }
}

void ClearCompleted()
{
    var count = tasks.Count(t => t.Completed);
    tasks.RemoveAll(t => t.Completed);
    SaveTasks(dataFile);
    Console.WriteLine($"✓ Cleared {count} completed task(s)");
}

void ShowHelp()
{
    Console.WriteLine("""
        TaskScheduler - CLI Task Management Tool

        Usage:
          dotnet run -- <command> [arguments]

        Commands:
          add <title> [options]    Add a new task
          list [options]           List tasks
          complete <id>            Mark task as complete
          delete <id>              Delete a task
          clear                    Remove all completed tasks

        Add Options:
          --due YYYY-MM-DD         Set due date
          --priority low|medium|high  Set priority (default: medium)

        List Options:
          --pending                Show only pending tasks
          --overdue                Show only overdue tasks

        Examples:
          dotnet run -- add "Buy groceries" --due 2026-04-05 --priority high
          dotnet run -- list --pending
          dotnet run -- complete 3
        """);
}

static List<TaskItem> LoadTasks(string path)
{
    if (!File.Exists(path))
        return [];

    try
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? [];
    }
    catch
    {
        return new List<TaskItem>();
    }
}

void SaveTasks(string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(tasks, options);
    File.WriteAllText(path, json);
}

class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime? DueDate { get; set; }
    public required string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}
