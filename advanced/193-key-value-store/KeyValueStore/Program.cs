using System.Collections.Concurrent;
using System.Text.Json;

namespace KeyValueStore;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Key-Value Store - In-memory key-value store with persistence");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project KeyValueStore.csproj -- interactive [data-file.json]");
            Console.WriteLine("  dotnet run --project KeyValueStore.csproj -- demo");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            var dataFile = args.Length > 1 ? args[1] : "kvstore-data.json";
            RunInteractiveMode(dataFile);
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive [data-file.json]'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== Key-Value Store Demo ===\n");

        var store = new KeyValueStore();

        // Basic operations
        Console.WriteLine("1. SET operations:");
        store.Set("name", "John Doe");
        store.Set("age", 30);
        store.Set("email", "john@example.com");
        store.Set("active", true);
        store.Set("scores", new[] { 95, 87, 92, 88 });
        Console.WriteLine("   Added 5 keys");
        Console.WriteLine();

        // GET operations
        Console.WriteLine("2. GET operations:");
        Console.WriteLine($"   name = {store.Get<string>("name")}");
        Console.WriteLine($"   age = {store.Get<int>("age")}");
        Console.WriteLine($"   email = {store.Get<string>("email")}");
        Console.WriteLine($"   active = {store.Get<bool>("active")}");
        var scores = store.Get<int[]>("scores");
        Console.WriteLine($"   scores = [{string.Join(", ", scores!)}]");
        Console.WriteLine();

        // EXISTS and COUNT
        Console.WriteLine("3. Metadata:");
        Console.WriteLine($"   Exists 'name': {store.Exists("name")}");
        Console.WriteLine($"   Exists 'unknown': {store.Exists("unknown")}");
        Console.WriteLine($"   Total keys: {store.Count()}");
        Console.WriteLine();

        // KEYS with pattern
        Console.WriteLine("4. KEYS (all):");
        foreach (var key in store.Keys())
        {
            Console.WriteLine($"   - {key}");
        }
        Console.WriteLine();

        // TTL operations
        Console.WriteLine("5. TTL operations:");
        store.SetEx("temp_key", "expires_soon", TimeSpan.FromSeconds(2));
        Console.WriteLine($"   Set 'temp_key' with 2 second TTL");
        Console.WriteLine($"   TTL: {store.Ttl("temp_key")} seconds");
        Console.WriteLine($"   Value: {store.Get<string>("temp_key")}");
        Thread.Sleep(2500);
        Console.WriteLine($"   After expiry - Value: {store.Get<string>("temp_key")}");
        Console.WriteLine();

        // DELETE
        Console.WriteLine("6. DELETE operation:");
        store.Delete("age");
        Console.WriteLine($"   Deleted 'age', exists: {store.Exists("age")}");
        Console.WriteLine($"   Count after delete: {store.Count()}");
        Console.WriteLine();

        // Persistence
        Console.WriteLine("7. Persistence:");
        store.SaveToFile("demo-store.json");
        Console.WriteLine("   Saved to demo-store.json");

        var newStore = new KeyValueStore();
        newStore.LoadFromFile("demo-store.json");
        Console.WriteLine($"   Loaded from file, count: {newStore.Count()}");
        Console.WriteLine($"   name from persisted store: {newStore.Get<string>("name")}");
        Console.WriteLine();

        // Cleanup demo file
        if (File.Exists("demo-store.json"))
            File.Delete("demo-store.json");

        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode(string dataFile)
    {
        Console.WriteLine("Key-Value Store (Interactive Mode)");
        Console.WriteLine($"Data file: {dataFile}");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var store = new KeyValueStore();
        if (File.Exists(dataFile))
        {
            store.LoadFromFile(dataFile);
            Console.WriteLine($"Loaded {store.Count()} keys from {dataFile}");
        }

        while (true)
        {
            Console.Write("kv> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "quit":
                    case "exit":
                        store.SaveToFile(dataFile);
                        Console.WriteLine($"Saved {store.Count()} keys to {dataFile}");
                        return;

                    case "help":
                        ShowHelp();
                        break;

                    case "set":
                        HandleSet(store, parts);
                        break;

                    case "get":
                        HandleGet(store, parts);
                        break;

                    case "del":
                    case "delete":
                        HandleDelete(store, parts);
                        break;

                    case "exists":
                        HandleExists(store, parts);
                        break;

                    case "keys":
                        HandleKeys(store);
                        break;

                    case "count":
                        Console.WriteLine($"Keys: {store.Count()}");
                        break;

                    case "ttl":
                        HandleTtl(store, parts);
                        break;

                    case "save":
                        store.SaveToFile(dataFile);
                        Console.WriteLine($"Saved {store.Count()} keys to {dataFile}");
                        break;

                    case "load":
                        store.LoadFromFile(dataFile);
                        Console.WriteLine($"Loaded {store.Count()} keys from {dataFile}");
                        break;

                    case "clear":
                        store.Clear();
                        Console.WriteLine("Store cleared");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  set <key> <value>  - Store a value");
        Console.WriteLine("  get <key>          - Retrieve a value");
        Console.WriteLine("  del <key>          - Delete a key");
        Console.WriteLine("  exists <key>       - Check if key exists");
        Console.WriteLine("  keys               - List all keys");
        Console.WriteLine("  count              - Count keys");
        Console.WriteLine("  ttl <key>          - Show TTL in seconds");
        Console.WriteLine("  save               - Save to file");
        Console.WriteLine("  load               - Load from file");
        Console.WriteLine("  clear              - Clear all keys");
        Console.WriteLine("  quit               - Save and exit");
        Console.WriteLine();
    }

    private static void HandleSet(KeyValueStore store, string[] parts)
    {
        if (parts.Length < 3)
        {
            Console.WriteLine("Usage: set <key> <value>");
            return;
        }

        var key = parts[1];
        var value = parts[2];

        // Try to parse as JSON for complex types
        try
        {
            if (value.StartsWith('[') || value.StartsWith('{'))
            {
                var jsonElement = JsonDocument.Parse(value).RootElement;
                store.Set(key, jsonElement);
            }
            else if (int.TryParse(value, out var intVal))
                store.Set(key, intVal);
            else if (double.TryParse(value, out var doubleVal))
                store.Set(key, doubleVal);
            else if (bool.TryParse(value, out var boolVal))
                store.Set(key, boolVal);
            else
                store.Set(key, value);

            Console.WriteLine("OK");
        }
        catch
        {
            store.Set(key, value);
            Console.WriteLine("OK");
        }
    }

    private static void HandleGet(KeyValueStore store, string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: get <key>");
            return;
        }

        var key = parts[1];
        var value = store.Get<object>(key);
        Console.WriteLine(value == null ? "(nil)" : value.ToString());
    }

    private static void HandleDelete(KeyValueStore store, string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: del <key>");
            return;
        }

        var result = store.Delete(parts[1]);
        Console.WriteLine(result ? "1" : "0");
    }

    private static void HandleExists(KeyValueStore store, string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: exists <key>");
            return;
        }

        Console.WriteLine(store.Exists(parts[1]) ? "1" : "0");
    }

    private static void HandleKeys(KeyValueStore store)
    {
        foreach (var key in store.Keys())
        {
            Console.WriteLine($"  {key}");
        }
    }

    private static void HandleTtl(KeyValueStore store, string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: ttl <key>");
            return;
        }

        var ttl = store.Ttl(parts[1]);
        Console.WriteLine(ttl.HasValue ? $"{ttl.Value} seconds" : "-1");
    }
}

public class KeyValueStore
{
    private readonly ConcurrentDictionary<string, StoreEntry> _store = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    private record StoreEntry(object Value, DateTime? ExpiresAt);

    public KeyValueStore()
    {
        // Start expiration checker
        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
                var now = DateTime.UtcNow;
                foreach (var kvp in _store.Where(k => k.Value.ExpiresAt.HasValue && k.Value.ExpiresAt.Value <= now))
                {
                    _store.TryRemove(kvp.Key, out _);
                }
            }
        });
    }

    public void Set<T>(string key, T value)
    {
        _store[key] = new StoreEntry(value!, null);
    }

    public void SetEx<T>(string key, T value, TimeSpan ttl)
    {
        _store[key] = new StoreEntry(value!, DateTime.UtcNow + ttl);
    }

    public T? Get<T>(string key)
    {
        if (!_store.TryGetValue(key, out var entry))
            return default;

        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value <= DateTime.UtcNow)
        {
            _store.TryRemove(key, out _);
            return default;
        }

        if (entry.Value is T t)
            return t;
        
        // Handle JsonElement deserialization
        if (entry.Value is System.Text.Json.JsonElement element)
        {
            return element.Deserialize<T>();
        }
        
        return (T)entry.Value;
    }

    public bool Delete(string key)
    {
        return _store.TryRemove(key, out _);
    }

    public bool Exists(string key)
    {
        if (!_store.TryGetValue(key, out var entry))
            return false;

        if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value <= DateTime.UtcNow)
        {
            _store.TryRemove(key, out _);
            return false;
        }

        return true;
    }

    public int Count() => _store.Count(kvp =>
    {
        if (kvp.Value.ExpiresAt.HasValue && kvp.Value.ExpiresAt.Value <= DateTime.UtcNow)
        {
            _store.TryRemove(kvp.Key, out _);
            return false;
        }
        return true;
    });

    public IEnumerable<string> Keys()
    {
        var now = DateTime.UtcNow;
        return _store.Where(kvp => !(kvp.Value.ExpiresAt.HasValue && kvp.Value.ExpiresAt.Value <= now))
            .Select(kvp => kvp.Key);
    }

    public int? Ttl(string key)
    {
        if (!_store.TryGetValue(key, out var entry))
            return null;

        if (!entry.ExpiresAt.HasValue)
            return -1; // No expiration

        var remaining = (entry.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds;
        return remaining > 0 ? (int)remaining : null;
    }

    public void Clear()
    {
        _store.Clear();
    }

    public void SaveToFile(string path)
    {
        var data = _store
            .Where(kvp => !kvp.Value.ExpiresAt.HasValue || kvp.Value.ExpiresAt.Value > DateTime.UtcNow)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(path, json);
    }

    public void LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

        foreach (var kvp in data)
        {
            _store[kvp.Key] = new StoreEntry(kvp.Value, null);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
