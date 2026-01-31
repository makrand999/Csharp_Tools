using System.Text.Json;
using System.Text.Json.Serialization;

var contactsFile = "contacts.json";
var contacts = LoadContacts(contactsFile);

while (true)
{
    Console.WriteLine("\n=== Contact Manager ===");
    Console.WriteLine("1. Add Contact");
    Console.WriteLine("2. List Contacts");
    Console.WriteLine("3. Search Contacts");
    Console.WriteLine("4. Filter by Tag");
    Console.WriteLine("5. Delete Contact");
    Console.WriteLine("6. Export to JSON");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddContact(contacts); break;
        case "2": ListContacts(contacts); break;
        case "3": SearchContacts(contacts); break;
        case "4": FilterByTag(contacts); break;
        case "5": DeleteContact(contacts); break;
        case "6": ExportContacts(contacts, contactsFile); break;
        case "0": SaveAndExit(contacts, contactsFile); return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static List<Contact> LoadContacts(string path)
{
    if (!File.Exists(path)) return [];
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<List<Contact>>(json) ?? [];
}

static void SaveContacts(List<Contact> contacts, string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(path, JsonSerializer.Serialize(contacts, options));
}

static void AddContact(List<Contact> contacts)
{
    Console.Write("Name: ");
    var name = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Phone: ");
    var phone = Console.ReadLine() ?? "";
    Console.Write("Email: ");
    var email = Console.ReadLine() ?? "";
    Console.Write("Tags (comma-separated): ");
    var tagsInput = Console.ReadLine() ?? "";
    var tags = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    contacts.Add(new Contact
    {
        Id = contacts.Count > 0 ? contacts.Max(c => c.Id) + 1 : 1,
        Name = name,
        Phone = phone,
        Email = email,
        Tags = tags,
        CreatedAt = DateTime.Now
    });

    Console.WriteLine($"Contact '{name}' added successfully!");
}

static void ListContacts(List<Contact> contacts)
{
    if (contacts.Count == 0)
    {
        Console.WriteLine("No contacts found.");
        return;
    }

    Console.WriteLine($"\n{"Id",-5} {"Name",-20} {"Phone",-15} {"Email",-25} {"Tags"}");
    Console.WriteLine(new string('-', 80));
    foreach (var contact in contacts)
    {
        var tagsStr = string.Join(", ", contact.Tags);
        Console.WriteLine($"{contact.Id,-5} {contact.Name,-20} {contact.Phone,-15} {contact.Email,-25} {tagsStr}");
    }
}

static void SearchContacts(List<Contact> contacts)
{
    Console.Write("Search query: ");
    var query = (Console.ReadLine() ?? "").ToLower();
    if (string.IsNullOrWhiteSpace(query))
    {
        Console.WriteLine("Query cannot be empty.");
        return;
    }

    var results = contacts.Where(c =>
        c.Name.ToLower().Contains(query) ||
        c.Phone.Contains(query) ||
        c.Email.ToLower().Contains(query) ||
        c.Tags.Any(t => t.ToLower().Contains(query))
    ).ToList();

    if (results.Count == 0)
    {
        Console.WriteLine("No contacts found matching your search.");
        return;
    }

    Console.WriteLine($"\nFound {results.Count} contact(s):");
    foreach (var contact in results)
    {
        Console.WriteLine($"  {contact.Name} - {contact.Phone} - {contact.Email}");
    }
}

static void FilterByTag(List<Contact> contacts)
{
    Console.Write("Tag to filter: ");
    var tag = (Console.ReadLine() ?? "").Trim().ToLower();
    if (string.IsNullOrWhiteSpace(tag))
    {
        Console.WriteLine("Tag cannot be empty.");
        return;
    }

    var results = contacts.Where(c => c.Tags.Any(t => t.ToLower() == tag)).ToList();

    if (results.Count == 0)
    {
        Console.WriteLine($"No contacts found with tag '{tag}'.");
        return;
    }

    Console.WriteLine($"\nFound {results.Count} contact(s) with tag '{tag}':");
    foreach (var contact in results)
    {
        Console.WriteLine($"  {contact.Name} - {contact.Phone} - {contact.Email}");
    }
}

static void DeleteContact(List<Contact> contacts)
{
    Console.Write("Enter contact ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var contact = contacts.FirstOrDefault(c => c.Id == id);
    if (contact == null)
    {
        Console.WriteLine($"Contact with ID {id} not found.");
        return;
    }

    contacts.Remove(contact);
    Console.WriteLine($"Contact '{contact.Name}' deleted successfully!");
}

static void ExportContacts(List<Contact> contacts, string path)
{
    SaveContacts(contacts, path);
    Console.WriteLine($"Contacts exported to {path}");
}

static void SaveAndExit(List<Contact> contacts, string path)
{
    SaveContacts(contacts, path);
    Console.WriteLine("Contacts saved. Goodbye!");
}

class Contact
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}
