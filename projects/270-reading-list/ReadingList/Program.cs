using System.Text.Json;

namespace ReadingList;

class Program
{
    private const string DataFile = "reading-list.json";
    private static List<Book> _books = new();

    static void Main(string[] args)
    {
        Console.WriteLine("📚 Reading List Tracker");
        Console.WriteLine("=======================\n");

        LoadBooks();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add":
                AddBook();
                break;
            case "list":
                ListBooks();
                break;
            case "status":
                UpdateStatus(args.ElementAtOrDefault(1), args.ElementAtOrDefault(2));
                break;
            case "rating":
                AddRating(args.ElementAtOrDefault(1), args.ElementAtOrDefault(2));
                break;
            case "notes":
                AddNotes(args.ElementAtOrDefault(1));
                break;
            case "stats":
                ShowStats();
                break;
            case "delete":
                DeleteBook(args.ElementAtOrDefault(1));
                break;
            default:
                ShowMenu();
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add                  - Add a new book");
        Console.WriteLine("  list                 - List all books");
        Console.WriteLine("  status <id> <status> - Update reading status");
        Console.WriteLine("  rating <id> <1-5>    - Add rating (1-5 stars)");
        Console.WriteLine("  notes <id>           - Add notes for a book");
        Console.WriteLine("  stats                - Show reading statistics");
        Console.WriteLine("  delete <id>          - Remove a book");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 3);
        string cmd = parts[0].ToLower();
        string[] remaining = parts.Skip(1).ToArray();

        switch (cmd)
        {
            case "add": AddBook(); break;
            case "list": ListBooks(); break;
            case "status": UpdateStatus(remaining.ElementAtOrDefault(0), remaining.ElementAtOrDefault(1)); break;
            case "rating": AddRating(remaining.ElementAtOrDefault(0), remaining.ElementAtOrDefault(1)); break;
            case "notes": AddNotes(remaining.ElementAtOrDefault(0)); break;
            case "stats": ShowStats(); break;
            case "delete": DeleteBook(remaining.ElementAtOrDefault(0)); break;
        }
    }

    private static void LoadBooks()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }
    }

    private static void SaveBooks()
    {
        string json = JsonSerializer.Serialize(_books, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddBook()
    {
        Console.Write("Title: ");
        string? title = Console.ReadLine();

        Console.Write("Author: ");
        string? author = Console.ReadLine();

        Console.Write("Pages (optional): ");
        string? pagesInput = Console.ReadLine();
        int.TryParse(pagesInput, out int pages);

        Console.Write("Genre (optional): ");
        string? genre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title is required!");
            return;
        }

        var book = new Book
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Title = title!,
            Author = author ?? "Unknown",
            Pages = pages,
            Genre = genre ?? "",
            Status = "To Read",
            AddedAt = DateTime.Now
        };

        _books.Add(book);
        SaveBooks();

        Console.WriteLine($"\n✅ Book added: {book.Title} by {book.Author}");
        Console.WriteLine($"   ID: {book.Id}");
    }

    private static void ListBooks()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("No books in your reading list.");
            return;
        }

        Console.WriteLine("Filter by status:");
        Console.WriteLine("  1. All");
        Console.WriteLine("  2. To Read");
        Console.WriteLine("  3. Reading");
        Console.WriteLine("  4. Completed");
        Console.Write("\nChoice (1-4, default=1): ");
        
        string? choice = Console.ReadLine();
        string? filter = choice switch
        {
            "2" => "To Read",
            "3" => "Reading",
            "4" => "Completed",
            _ => null
        };

        var filtered = filter == null ? _books : _books.Where(b => b.Status == filter).ToList();

        if (filtered.Count == 0)
        {
            Console.WriteLine($"No books with status '{filter}'.");
            return;
        }

        Console.WriteLine($"\n{"ID",-10} {"Title",-30} {"Author",-20} {"Status",-12} {"Rating",-8}");
        Console.WriteLine(new string('-', 85));

        foreach (var book in filtered.OrderByDescending(b => b.AddedAt))
        {
            string rating = book.Rating > 0 ? new string('★', book.Rating) + new string('☆', 5 - book.Rating) : "-";
            Console.WriteLine($"{book.Id,-10} {Truncate(book.Title, 30),-30} {Truncate(book.Author, 20),-20} {book.Status,-12} {rating,-8}");
        }

        Console.WriteLine($"\nShowing {filtered.Count} book(s)");
    }

    private static void UpdateStatus(string? idArg, string? statusArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Book ID: ");
            id = Console.ReadLine();
        }

        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Console.WriteLine($"Book '{id}' not found.");
            return;
        }

        string? status = statusArg;
        if (string.IsNullOrWhiteSpace(status))
        {
            Console.WriteLine("\nStatus options:");
            Console.WriteLine("  1. To Read");
            Console.WriteLine("  2. Reading");
            Console.WriteLine("  3. Completed");
            Console.Write("\nChoose status (1-3): ");
            
            string? choice = Console.ReadLine();
            status = choice switch
            {
                "1" => "To Read",
                "2" => "Reading",
                "3" => "Completed",
                _ => book.Status
            };
        }

        book.Status = status!;
        if (status == "Completed")
        {
            book.CompletedAt = DateTime.Now;
        }

        SaveBooks();
        Console.WriteLine($"\n✅ Updated '{book.Title}' status to: {book.Status}");
    }

    private static void AddRating(string? idArg, string? ratingArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Book ID: ");
            id = Console.ReadLine();
        }

        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Console.WriteLine($"Book '{id}' not found.");
            return;
        }

        string? ratingStr = ratingArg;
        if (string.IsNullOrWhiteSpace(ratingStr))
        {
            Console.Write("Rating (1-5 stars): ");
            ratingStr = Console.ReadLine();
        }

        if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
        {
            book.Rating = rating;
            SaveBooks();
            Console.WriteLine($"\n✅ Rated '{book.Title}': {new string('★', rating)} ({rating}/5)");
        }
        else
        {
            Console.WriteLine("Invalid rating. Please enter 1-5.");
        }
    }

    private static void AddNotes(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Book ID: ");
            id = Console.ReadLine();
        }

        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Console.WriteLine($"Book '{id}' not found.");
            return;
        }

        Console.WriteLine($"\nAdding notes for '{book.Title}' (empty line to finish):");
        var notes = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            notes.Add(line);
        }

        book.Notes = string.Join("\n", notes);
        SaveBooks();
        Console.WriteLine("✅ Notes saved.");
    }

    private static void ShowStats()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("No books in your reading list.");
            return;
        }

        int toRead = _books.Count(b => b.Status == "To Read");
        int reading = _books.Count(b => b.Status == "Reading");
        int completed = _books.Count(b => b.Status == "Completed");
        int rated = _books.Count(b => b.Rating > 0);
        double avgRating = rated > 0 ? _books.Where(b => b.Rating > 0).Average(b => b.Rating) : 0;
        int totalPages = _books.Where(b => b.Status == "Completed").Sum(b => b.Pages);

        Console.WriteLine("📊 Reading Statistics");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Total books:      {_books.Count}");
        Console.WriteLine($"To read:          {toRead}");
        Console.WriteLine($"Currently reading:{reading}");
        Console.WriteLine($"Completed:        {completed}");
        Console.WriteLine($"Rated:            {rated}");
        Console.WriteLine($"Average rating:   {avgRating:F1}/5 {(avgRating > 0 ? new string('★', (int)Math.Round(avgRating)) : "")}");
        Console.WriteLine($"Pages read:       {totalPages:N0}");

        if (completed > 0 && _books.Any(b => b.CompletedAt.HasValue))
        {
            var firstBook = _books.Where(b => b.CompletedAt.HasValue).OrderBy(b => b.CompletedAt).First();
            var lastBook = _books.Where(b => b.CompletedAt.HasValue).OrderByDescending(b => b.CompletedAt).First();
            Console.WriteLine($"\nFirst completed:  {firstBook.Title} ({firstBook.CompletedAt:yyyy-MM-dd})");
            Console.WriteLine($"Last completed:   {lastBook.Title} ({lastBook.CompletedAt:yyyy-MM-dd})");
        }
    }

    private static void DeleteBook(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Book ID to delete: ");
            id = Console.ReadLine();
        }

        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            Console.WriteLine($"Book '{id}' not found.");
            return;
        }

        Console.Write($"Delete '{book.Title}' by {book.Author}? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _books.Remove(book);
            SaveBooks();
            Console.WriteLine("✅ Book deleted.");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

public class Book
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int Pages { get; set; }
    public string Genre { get; set; } = "";
    public string Status { get; set; } = "To Read";
    public int Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
