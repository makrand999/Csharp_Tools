namespace UuidBatchGenerator;

class Program
{
    static void Main(string[] args)
    {
        int count = 1;
        string? version = null;
        bool outputJson = false;

        if (args.Length == 0)
        {
            Console.WriteLine("UUID/GUID Batch Generator");
            Console.WriteLine("Usage: dotnet run --project 142-uuid-batch-generator.csproj -- [count] [--v4] [--json]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  count     Number of UUIDs to generate (default: 1)");
            Console.WriteLine("  --v4      Generate version 4 UUIDs (random)");
            Console.WriteLine("  --json    Output as JSON array");
            return;
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--v4")
            {
                version = "4";
            }
            else if (args[i] == "--json")
            {
                outputJson = true;
            }
            else if (int.TryParse(args[i], out var n) && n > 0)
            {
                count = n;
            }
        }

        var uuids = new List<string>();
        for (int i = 0; i < count; i++)
        {
            uuids.Add(GenerateUuid(version));
        }

        if (outputJson)
        {
            Console.WriteLine("[");
            for (int i = 0; i < uuids.Count; i++)
            {
                Console.Write($"  \"{uuids[i]}\"");
                if (i < uuids.Count - 1)
                {
                    Console.WriteLine(",");
                }
                else
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine("]");
        }
        else
        {
            foreach (var uuid in uuids)
            {
                Console.WriteLine(uuid);
            }
        }

        Console.Error.WriteLine($"Generated {count} UUID(s)");
    }

    static string GenerateUuid(string? version)
    {
        if (version == "4")
        {
            return Guid.NewGuid().ToString();
        }

        // Default: use Guid (version 4 variant)
        return Guid.NewGuid().ToString();
    }
}
