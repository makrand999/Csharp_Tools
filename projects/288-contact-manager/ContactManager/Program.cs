using System.Text.Json;

namespace ContactManager;

class Program
{
    private const string DataFile = "contacts.json";
    private static List<Contact> _contacts = new();

    static void Main(string[] args)
    {
        Console.WriteLine("👤 Contact Manager");
        Console.WriteLine("==================\n");

        LoadContacts();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add": AddContact(); break;
            case "list": ListContacts(); break;
            case "search": SearchContacts(args.Length > 1 ? string.Join(" ", args.Skip(1)) : null); break;
            case "get": GetContact(args.ElementAtOrDefault(1)); break;
            case "delete": DeleteContact(args.ElementAtOrDefault(1)); break;
            case "export": ExportContacts(); break;
            default: ShowMenu(); break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add     - Add new contact");
        Console.WriteLine("  list    - List all contacts");
        Console.WriteLine("  search  - Search contacts");
        Console.WriteLine("  get     - Get contact details");
        Console.WriteLine("  delete  - Delete contact");
        Console.WriteLine("  export  - Export to CSV");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 2);
        string cmd = parts[0].ToLower();
        string? arg = parts.Length > 1 ? parts[1] : null;

        switch (cmd)
        {
            case "add": AddContact(); break;
            case "list": ListContacts(); break;
            case "search": SearchContacts(arg); break;
            case "get": GetContact(arg); break;
            case "delete": DeleteContact(arg); break;
            case "export": ExportContacts(); break;
        }
    }

    private static void LoadContacts()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _contacts = JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
        }
    }

    private static void SaveContacts()
    {
        string json = JsonSerializer.Serialize(_contacts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddContact()
    {
        Console.Write("\nFirst Name: ");
        string? firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string? lastName = Console.ReadLine();

        Console.Write("Email: ");
        string? email = Console.ReadLine();

        Console.Write("Phone: ");
        string? phone = Console.ReadLine();

        Console.Write("Company (optional): ");
        string? company = Console.ReadLine();

        Console.Write("Tags (comma-separated, optional): ");
        string? tagsInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("First and last name are required!");
            return;
        }

        var contact = new Contact
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            FirstName = firstName!,
            LastName = lastName!,
            Email = email ?? "",
            Phone = phone ?? "",
            Company = company ?? "",
            Tags = tagsInput?.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
            CreatedAt = DateTime.Now
        };

        _contacts.Add(contact);
        SaveContacts();

        Console.WriteLine($"\n✅ Contact added: {contact.FullName}");
        Console.WriteLine($"   ID: {contact.Id}");
    }

    private static void ListContacts()
    {
        if (_contacts.Count == 0)
        {
            Console.WriteLine("No contacts found.");
            return;
        }

        Console.WriteLine($"\n{"ID",-10} {"Name",-25} {"Email",-30} {"Phone",-15} {"Tags",-20}");
        Console.WriteLine(new string('-', 105));

        foreach (var contact in _contacts.OrderBy(c => c.LastName).ThenBy(c => c.FirstName))
        {
            string tags = string.Join(",", contact.Tags.Take(2));
            Console.WriteLine($"{contact.Id,-10} {contact.FullName,-25} {Truncate(contact.Email, 30),-30} {contact.Phone ?? "-",-15} {tags,-20}");
        }

        Console.WriteLine($"\nTotal: {_contacts.Count} contact(s)");
    }

    private static void SearchContacts(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Console.Write("Search query: ");
            query = Console.ReadLine();
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("No search query provided.");
            return;
        }

        var results = _contacts.Where(c =>
            c.FullName.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            c.Email.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            c.Phone.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            c.Company.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            c.Tags.Any(t => t.Contains(query!, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        if (results.Count == 0)
        {
            Console.WriteLine($"No contacts found matching '{query}'.");
            return;
        }

        Console.WriteLine($"\nFound {results.Count} contact(s):\n");
        foreach (var contact in results)
        {
            Console.WriteLine($"[{contact.Id}] {contact.FullName}");
            Console.WriteLine($"  Email: {contact.Email}");
            Console.WriteLine($"  Phone: {contact.Phone ?? "N/A"}");
            Console.WriteLine($"  Company: {contact.Company ?? "N/A"}");
            if (contact.Tags.Any())
                Console.WriteLine($"  Tags: {string.Join(", ", contact.Tags)}");
            Console.WriteLine();
        }
    }

    private static void GetContact(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Contact ID: ");
            id = Console.ReadLine();
        }

        var contact = _contacts.FirstOrDefault(c => c.Id == id);
        if (contact == null)
        {
            Console.WriteLine($"Contact '{id}' not found.");
            return;
        }

        Console.WriteLine($"\n👤 {contact.FullName}");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"ID: {contact.Id}");
        Console.WriteLine($"Email: {contact.Email}");
        Console.WriteLine($"Phone: {contact.Phone ?? "N/A"}");
        Console.WriteLine($"Company: {contact.Company ?? "N/A"}");
        if (contact.Tags.Any())
            Console.WriteLine($"Tags: {string.Join(", ", contact.Tags)}");
        Console.WriteLine($"Added: {contact.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    }

    private static void DeleteContact(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Contact ID to delete: ");
            id = Console.ReadLine();
        }

        var contact = _contacts.FirstOrDefault(c => c.Id == id);
        if (contact == null)
        {
            Console.WriteLine($"Contact '{id}' not found.");
            return;
        }

        Console.Write($"Delete {contact.FullName}? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _contacts.Remove(contact);
            SaveContacts();
            Console.WriteLine("✅ Contact deleted.");
        }
    }

    private static void ExportContacts()
    {
        string outputFile = $"contacts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var csv = new System.Text.StringBuilder();

        csv.AppendLine("ID,FirstName,LastName,Email,Phone,Company,Tags");

        foreach (var contact in _contacts)
        {
            csv.AppendLine($"{contact.Id},{contact.FirstName},{contact.LastName},{EscapeCsv(contact.Email)},{EscapeCsv(contact.Phone)},{EscapeCsv(contact.Company)},{EscapeCsv(string.Join(";", contact.Tags))}");
        }

        File.WriteAllText(outputFile, csv.ToString());
        Console.WriteLine($"✅ Exported {csv.Length - 1} contacts to: {outputFile}");
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

class Contact
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Company { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
