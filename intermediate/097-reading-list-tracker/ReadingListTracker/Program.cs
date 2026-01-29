using System.Text.Json;

var dataFile = "reading-list.json";
var books = LoadBooks(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddBook(books, args.Skip(1).ToArray());
        break;
    case "list":
        ListBooks(books, args.Skip(1).ToArray());
        break;
    case "status":
        UpdateStatus(books, args.Skip(1).ToArray());
        break;
    case "note":
        AddNote(books, args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteBook(books, args.Skip(1).ToArray());
        break;
    case "stats":
        ShowStats(books);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

SaveBooks(dataFile, books);

void ShowHelp()
{
    Console.WriteLine("""
        Reading List Tracker - Track books and articles with status and notes
        
        Usage:
          dotnet run --project ReadingListTracker.csproj <command> [arguments]
        
        Commands:
          add <title> <author> [tags...]    Add a new book/article
          list [status]                     List all or filter by status
          status <id> <status>              Update reading status
          note <id> <text>                  Add a note to a book
          delete <id>                       Remove a book from the list
          stats                             Show reading statistics
        
        Statuses: to-read, reading, completed, abandoned
        
        Examples:
          dotnet run -- add "Clean Code" "Robert Martin" programming classic
          dotnet run -- list reading
          dotnet run -- status 1 completed
          dotnet run -- note 1 "Great chapter on functions!"
          dotnet run -- stats
        """);
}

void AddBook(List<Book> books, string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: add <title> <author> [tags...]");
        return;
    }

    var title = args[0];
    var author = args[1];
    var tags = args.Skip(2).Select(t => t.ToLower()).ToList();

    var book = new Book
    {
        Id = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1,
        Title = title,
        Author = author,
        Tags = tags,
        Status = "to-read",
        AddedAt = DateTime.Now
    };

    books.Add(book);
    Console.WriteLine($"✓ Added: \"{title}\" by {author}");
    Console.WriteLine($"  Status: to-read | ID: {book.Id}");
    if (tags.Count > 0)
        Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
}

void ListBooks(List<Book> books, string[] args)
{
    var filtered = books;
    
    if (args.Length > 0)
    {
        var status = args[0].ToLower();
        filtered = books.Where(b => b.Status == status).ToList();
    }
    else
    {
        // Default: group by status
        filtered = books.OrderBy(b => b.Status).ThenBy(b => b.Title).ToList();
    }

    if (filtered.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    var currentStatus = "";
    foreach (var book in filtered)
    {
        if (args.Length == 0 && book.Status != currentStatus)
        {
            currentStatus = book.Status;
            Console.WriteLine($"\n📚 {currentStatus.ToUpper()}");
            Console.WriteLine(new string('-', 40));
        }
        
        Console.WriteLine($"[{book.Id}] {book.Title} by {book.Author}");
        if (book.Tags.Count > 0)
            Console.WriteLine($"    Tags: {string.Join(", ", book.Tags)}");
        if (!string.IsNullOrEmpty(book.Note))
            Console.WriteLine($"    Note: {book.Note}");
        Console.WriteLine($"    Added: {book.AddedAt:yyyy-MM-dd}");
        if (book.CompletedAt.HasValue)
            Console.WriteLine($"    Completed: {book.CompletedAt:yyyy-MM-dd}");
        Console.WriteLine();
    }
}

void UpdateStatus(List<Book> books, string[] args)
{
    if (args.Length < 2 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: status <id> <status>");
        Console.WriteLine("Statuses: to-read, reading, completed, abandoned");
        return;
    }

    var status = args[1].ToLower();
    var validStatuses = new[] { "to-read", "reading", "completed", "abandoned" };
    
    if (!validStatuses.Contains(status))
    {
        Console.WriteLine($"Invalid status. Valid options: {string.Join(", ", validStatuses)}");
        return;
    }

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null)
    {
        Console.WriteLine($"Book #{id} not found.");
        return;
    }

    var oldStatus = book.Status;
    book.Status = status;
    
    if (status == "completed" && oldStatus != "completed")
        book.CompletedAt = DateTime.Now;
    else if (status != "completed")
        book.CompletedAt = null;

    Console.WriteLine($"✓ Updated \"{book.Title}\" to {status}");
}

void AddNote(List<Book> books, string[] args)
{
    if (args.Length < 2 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: note <id> <text>");
        return;
    }

    var note = string.Join(" ", args.Skip(1));
    var book = books.FirstOrDefault(b => b.Id == id);
    
    if (book == null)
    {
        Console.WriteLine($"Book #{id} not found.");
        return;
    }

    book.Note = note;
    Console.WriteLine($"✓ Note added to \"{book.Title}\"");
}

void DeleteBook(List<Book> books, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: delete <id>");
        return;
    }

    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null)
    {
        Console.WriteLine($"Book #{id} not found.");
        return;
    }

    books.Remove(book);
    Console.WriteLine($"✓ Removed: \"{book.Title}\" by {book.Author}");
}

void ShowStats(List<Book> books)
{
    if (books.Count == 0)
    {
        Console.WriteLine("No books in your reading list.");
        return;
    }

    var total = books.Count;
    var toRead = books.Count(b => b.Status == "to-read");
    var reading = books.Count(b => b.Status == "reading");
    var completed = books.Count(b => b.Status == "completed");
    var abandoned = books.Count(b => b.Status == "abandoned");

    Console.WriteLine("""
        
        📊 Reading Statistics
        ═══════════════════════════════════
        """);
    Console.WriteLine($"  Total books:     {total}");
    Console.WriteLine($"  To read:         {toRead}");
    Console.WriteLine($"  Currently read:  {reading}");
    Console.WriteLine($"  Completed:       {completed}");
    Console.WriteLine($"  Abandoned:       {abandoned}");
    
    if (completed > 0)
    {
        var completionRate = (double)completed / total * 100;
        Console.WriteLine($"  Completion rate: {completionRate:F1}%");
        
        var firstCompleted = books.Where(b => b.CompletedAt.HasValue).MinBy(b => b.CompletedAt);
        var lastCompleted = books.Where(b => b.CompletedAt.HasValue).MaxBy(b => b.CompletedAt);
        
        if (firstCompleted != null && lastCompleted != null && firstCompleted.CompletedAt != lastCompleted.CompletedAt)
        {
            var days = (lastCompleted.CompletedAt!.Value - firstCompleted.CompletedAt!.Value).Days;
            if (days > 0)
            {
                var booksPerMonth = (double)completed / days * 30;
                Console.WriteLine($"  Reading pace:    {booksPerMonth:F1} books/month");
            }
        }
    }
    
    Console.WriteLine();
}

List<Book> LoadBooks(string path)
{
    if (!File.Exists(path))
        return new List<Book>();
    
    try
    {
        var json = File.ReadAllText(path);
        var books = JsonSerializer.Deserialize<List<Book>>(json);
        return books ?? new List<Book>();
    }
    catch (JsonException)
    {
        Console.WriteLine("Warning: Could not parse reading-list.json, starting fresh.");
        return new List<Book>();
    }
}

void SaveBooks(string path, List<Book> books)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(books, options);
    File.WriteAllText(path, json);
}

class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string Status { get; set; } = "to-read";
    public string? Note { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
