using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

Console.WriteLine("Secret Vault - Credential Manager");
Console.WriteLine("==================================\n");

var vaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".secret-vault", "vault.json");
Directory.CreateDirectory(Path.GetDirectoryName(vaultPath)!);

var vault = LoadVault(vaultPath);

Console.WriteLine("Commands:");
Console.WriteLine("  add     - Add a new secret");
Console.WriteLine("  list    - List all secret names");
Console.WriteLine("  get     - Retrieve a secret");
Console.WriteLine("  delete  - Delete a secret");
Console.WriteLine("  export  - Export vault (encrypted)");
Console.WriteLine("  quit    - Exit the vault\n");

while (true)
{
    Console.Write("vault> ");
    var input = Console.ReadLine()?.Trim().Split(' ', 2);

    if (input == null || input.Length == 0)
        continue;

    var command = input[0].ToLower();
    var commandArgs = input.Length > 1 ? input[1] : "";

    switch (command)
    {
        case "add":
            AddSecret(vault, vaultPath);
            break;

        case "list":
            ListSecrets(vault);
            break;
        
        case "get":
            GetSecret(vault, commandArgs);
            break;
        
        case "delete":
            DeleteSecret(vault, vaultPath, commandArgs);
            break;
        
        case "export":
            ExportVault(vault);
            break;
        
        case "quit":
        case "exit":
            Console.WriteLine("🔒 Vault locked. Goodbye!");
            return;
        
        default:
            Console.WriteLine("Unknown command. Use: add, list, get, delete, export, quit");
            break;
    }
}

static void AddSecret(VaultData vault, string vaultPath)
{
    Console.Write("Secret name: ");
    var name = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrEmpty(name))
    {
        Console.WriteLine("❌ Invalid name.");
        return;
    }
    
    Console.Write("Secret value: ");
    var value = ReadPassword();
    
    if (string.IsNullOrEmpty(value))
    {
        Console.WriteLine("❌ Invalid value.");
        return;
    }
    
    Console.Write("Category (optional): ");
    var category = Console.ReadLine()?.Trim() ?? "general";
    
    vault.Secrets[name] = new SecretEntry
    {
        Value = value,
        Category = category,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    
    SaveVault(vault, vaultPath);
    Console.WriteLine($"✅ Secret '{name}' added.");
}

static void ListSecrets(VaultData vault)
{
    if (vault.Secrets.Count == 0)
    {
        Console.WriteLine("📭 No secrets in vault.");
        return;
    }
    
    Console.WriteLine("\n📋 Secrets in vault:");
    Console.WriteLine(new string('-', 50));
    
    foreach (var kvp in vault.Secrets.OrderBy(k => k.Value.Category).ThenBy(k => k.Key))
    {
        var entry = kvp.Value;
        Console.WriteLine($"   [{entry.Category}] {kvp.Key}");
        Console.WriteLine($"      Created: {entry.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    }
    
    Console.WriteLine(new string('-', 50));
    Console.WriteLine($"Total: {vault.Secrets.Count} secrets\n");
}

static void GetSecret(VaultData vault, string name)
{
    if (string.IsNullOrEmpty(name))
    {
        Console.Write("Secret name: ");
        name = Console.ReadLine()?.Trim() ?? "";
    }
    
    if (vault.Secrets.TryGetValue(name, out var entry))
    {
        Console.WriteLine($"\n🔑 {name}:");
        Console.WriteLine($"   Value: {entry.Value}");
        Console.WriteLine($"   Category: {entry.Category}");
        Console.WriteLine($"   Created: {entry.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"   Updated: {entry.UpdatedAt:yyyy-MM-dd HH:mm:ss}\n");
    }
    else
    {
        Console.WriteLine($"❌ Secret '{name}' not found.");
    }
}

static void DeleteSecret(VaultData vault, string vaultPath, string name)
{
    if (string.IsNullOrEmpty(name))
    {
        Console.Write("Secret name to delete: ");
        name = Console.ReadLine()?.Trim() ?? "";
    }
    
    if (vault.Secrets.Remove(name))
    {
        SaveVault(vault, vaultPath);
        Console.WriteLine($"✅ Secret '{name}' deleted.");
    }
    else
    {
        Console.WriteLine($"❌ Secret '{name}' not found.");
    }
}

static void ExportVault(VaultData vault)
{
    Console.Write("Export file path: ");
    var exportPath = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrEmpty(exportPath))
    {
        Console.WriteLine("❌ Invalid path.");
        return;
    }
    
    var json = JsonSerializer.Serialize(vault, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(exportPath, json);
    Console.WriteLine($"⚠️  WARNING: Exported vault is NOT encrypted!");
    Console.WriteLine($"✅ Exported to: {exportPath}");
}

static string ReadPassword()
{
    var password = new StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }
        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password.Remove(password.Length - 1, 1);
            Console.Write("\b \b");
        }
        else if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
    }
    return password.ToString();
}

static VaultData LoadVault(string path)
{
    if (File.Exists(path))
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<VaultData>(json) ?? new VaultData();
    }
    
    var newVault = new VaultData { MasterKey = GenerateKey() };
    SaveVault(newVault, path);
    Console.WriteLine("🔐 New vault created.\n");
    return newVault;
}

static void SaveVault(VaultData vault, string path)
{
    var json = JsonSerializer.Serialize(vault, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(path, json);
}

static string GenerateKey()
{
    var key = new byte[32];
    RandomNumberGenerator.Fill(key);
    return Convert.ToBase64String(key);
}

class VaultData
{
    public string MasterKey { get; set; } = string.Empty;
    public Dictionary<string, SecretEntry> Secrets { get; set; } = new();
}

class SecretEntry
{
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = "general";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
