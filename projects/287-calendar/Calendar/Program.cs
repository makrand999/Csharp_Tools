using System.Text.Json;

namespace Calendar;

class Program
{
    private const string DataFile = "events.json";
    private static List<Event> _events = new();

    static void Main(string[] args)
    {
        Console.WriteLine("📅 Calendar Manager");
        Console.WriteLine("===================\n");

        LoadEvents();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add": AddEvent(); break;
            case "list": ListEvents(); break;
            case "today": ShowToday(); break;
            case "delete": DeleteEvent(args.ElementAtOrDefault(1)); break;
            default: ShowMenu(); break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add    - Add new event");
        Console.WriteLine("  list   - List all events");
        Console.WriteLine("  today  - Show today's events");
        Console.WriteLine("  delete - Delete event by ID");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 2);
        string cmd = parts[0].ToLower();
        string? arg = parts.Length > 1 ? parts[1] : null;

        switch (cmd)
        {
            case "add": AddEvent(); break;
            case "list": ListEvents(); break;
            case "today": ShowToday(); break;
            case "delete": DeleteEvent(arg); break;
        }
    }

    private static void LoadEvents()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _events = JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
        }
    }

    private static void SaveEvents()
    {
        string json = JsonSerializer.Serialize(_events, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddEvent()
    {
        Console.Write("\nTitle: ");
        string? title = Console.ReadLine();

        Console.Write("Date (YYYY-MM-DD): ");
        string? dateStr = Console.ReadLine();

        Console.Write("Time (HH:MM, optional): ");
        string? timeStr = Console.ReadLine();

        Console.Write("Location (optional): ");
        string? location = Console.ReadLine();

        Console.Write("Notes (optional): ");
        string? notes = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(dateStr))
        {
            Console.WriteLine("Title and date are required!");
            return;
        }

        if (!DateTime.TryParse(dateStr, out DateTime date))
        {
            Console.WriteLine("Invalid date format!");
            return;
        }

        TimeSpan? time = null;
        if (!string.IsNullOrWhiteSpace(timeStr) && TimeSpan.TryParse(timeStr, out var t))
        {
            time = t;
        }

        var evt = new Event
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Title = title!,
            Date = date.Date,
            Time = time,
            Location = location ?? "",
            Notes = notes ?? "",
            CreatedAt = DateTime.Now
        };

        _events.Add(evt);
        SaveEvents();

        Console.WriteLine($"\n✅ Event added: {evt.Title}");
        Console.WriteLine($"   ID: {evt.Id}");
        Console.WriteLine($"   Date: {evt.Date:yyyy-MM-dd}{(evt.Time.HasValue ? $" at {evt.Time:hh\\:mm}" : "")}");
    }

    private static void ListEvents()
    {
        if (_events.Count == 0)
        {
            Console.WriteLine("No events scheduled.");
            return;
        }

        Console.Write("\nFilter by month (YYYY-MM, or Enter for all): ");
        string? monthFilter = Console.ReadLine();

        var filtered = _events;
        if (!string.IsNullOrWhiteSpace(monthFilter))
        {
            var parts = monthFilter!.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int year) && int.TryParse(parts[1], out int month))
            {
                filtered = _events.Where(e => e.Date.Year == year && e.Date.Month == month).ToList();
            }
        }

        if (filtered.Count == 0)
        {
            Console.WriteLine("No events found for the specified period.");
            return;
        }

        Console.WriteLine($"\n{"ID",-10} {"Date",-12} {"Time",-8} {"Title",-25} {"Location",-20}");
        Console.WriteLine(new string('-', 80));

        foreach (var evt in filtered.OrderBy(e => e.Date).ThenBy(e => e.Time))
        {
            string time = evt.Time.HasValue ? $"{evt.Time:hh\\:mm}" : "--:--";
            Console.WriteLine($"{evt.Id,-10} {evt.Date:yyyy-MM-dd,-12} {time,-8} {Truncate(evt.Title, 25),-25} {Truncate(evt.Location ?? "", 20),-20}");
        }

        Console.WriteLine($"\nTotal: {filtered.Count} event(s)");
    }

    private static void ShowToday()
    {
        var today = DateTime.Today;
        var todaysEvents = _events.Where(e => e.Date.Date == today).OrderBy(e => e.Time).ToList();

        Console.WriteLine($"\n📅 Today's Events ({today:dddd, MMMM d, yyyy})\n");

        if (todaysEvents.Count == 0)
        {
            Console.WriteLine("No events scheduled for today. Enjoy your day! ☀️");
            return;
        }

        foreach (var evt in todaysEvents)
        {
            string time = evt.Time.HasValue ? $"{evt.Time:hh\\:mm}" : "All day";
            Console.WriteLine($"⏰ {time} - {evt.Title}");
            if (!string.IsNullOrEmpty(evt.Location))
                Console.WriteLine($"   📍 {evt.Location}");
            if (!string.IsNullOrEmpty(evt.Notes))
                Console.WriteLine($"   📝 {evt.Notes}");
            Console.WriteLine();
        }

        Console.WriteLine($"Total: {todaysEvents.Count} event(s) today");
    }

    private static void DeleteEvent(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Event ID to delete: ");
            id = Console.ReadLine();
        }

        var evt = _events.FirstOrDefault(e => e.Id == id);
        if (evt == null)
        {
            Console.WriteLine($"Event '{id}' not found.");
            return;
        }

        Console.Write($"Delete '{evt.Title}' on {evt.Date:yyyy-MM-dd}? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _events.Remove(evt);
            SaveEvents();
            Console.WriteLine("✅ Event deleted.");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

class Event
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime Date { get; set; }
    public TimeSpan? Time { get; set; }
    public string Location { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
