namespace FileSplitter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("File Splitter/Joiner");
            Console.WriteLine("Usage: dotnet run --project 149-file-splitter.csproj -- split <file> --parts N");
            Console.WriteLine("       dotnet run --project 149-file-splitter.csproj -- split <file> --size 10MB");
            Console.WriteLine("       dotnet run --project 149-file-splitter.csproj -- join <output> <part1> <part2> ...");
            Console.WriteLine("\nSplit options:");
            Console.WriteLine("  --parts N     Split into N equal parts");
            Console.WriteLine("  --size SIZE   Split by size (e.g., 10MB, 100KB, 1GB)");
            return;
        }

        string operation = args[0].ToLower();

        if (operation == "split")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: No input file specified");
                return;
            }

            string inputFile = args[1];
            int parts = 0;
            long? sizeBytes = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--parts" && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out parts);
                }
                else if (args[i] == "--size" && i + 1 < args.Length)
                {
                    sizeBytes = ParseSize(args[i + 1]);
                }
            }

            if (parts > 0)
            {
                SplitByParts(inputFile, parts);
            }
            else if (sizeBytes.HasValue)
            {
                SplitBySize(inputFile, sizeBytes.Value);
            }
            else
            {
                Console.WriteLine("Error: Specify --parts or --size");
            }
        }
        else if (operation == "join")
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Error: Need output file and at least 2 input parts");
                return;
            }

            string outputFile = args[1];
            var inputFiles = args.Skip(2).ToList();
            JoinFiles(outputFile, inputFiles);
        }
        else
        {
            Console.WriteLine($"Unknown operation: {operation}");
        }
    }

    static void SplitByParts(string inputFile, int parts)
    {
        var fileInfo = new FileInfo(inputFile);
        var partSize = fileInfo.Length / parts;
        var baseName = Path.GetFileNameWithoutExtension(inputFile);
        var extension = Path.GetExtension(inputFile);

        using var stream = File.OpenRead(inputFile);
        var buffer = new byte[81920]; // 80KB

        for (int i = 0; i < parts; i++)
        {
            var partName = $"{baseName}.part{i + 1:D3}{extension}";
            using var partStream = File.Create(partName);
            
            long remaining = (i == parts - 1) 
                ? fileInfo.Length - stream.Position 
                : partSize;

            while (remaining > 0)
            {
                int toRead = (int)Math.Min(buffer.Length, remaining);
                int read = stream.Read(buffer, 0, toRead);
                if (read == 0) break;
                partStream.Write(buffer, 0, read);
                remaining -= read;
            }

            Console.WriteLine($"Created: {partName} ({partStream.Length:N0} bytes)");
        }

        Console.WriteLine($"\nSplit {fileInfo.Length:N0} bytes into {parts} parts");
    }

    static void SplitBySize(string inputFile, long partSize)
    {
        var fileInfo = new FileInfo(inputFile);
        var baseName = Path.GetFileNameWithoutExtension(inputFile);
        var extension = Path.GetExtension(inputFile);

        using var stream = File.OpenRead(inputFile);
        var buffer = new byte[81920];
        int partNum = 1;

        while (stream.Position < fileInfo.Length)
        {
            var partName = $"{baseName}.part{partNum:D3}{extension}";
            using var partStream = File.Create(partName);
            long written = 0;

            while (written < partSize && stream.Position < fileInfo.Length)
            {
                int toRead = (int)Math.Min(buffer.Length, partSize - written);
                int read = stream.Read(buffer, 0, toRead);
                if (read == 0) break;
                partStream.Write(buffer, 0, read);
                written += read;
            }

            Console.WriteLine($"Created: {partName} ({partStream.Length:N0} bytes)");
            partNum++;
        }

        Console.WriteLine($"\nSplit {fileInfo.Length:N0} bytes into {partNum - 1} parts ({partSize:N0} bytes each)");
    }

    static void JoinFiles(string outputFile, List<string> inputFiles)
    {
        using var output = File.Create(outputFile);
        var buffer = new byte[81920];
        long totalSize = 0;

        foreach (var inputFile in inputFiles.OrderBy(f => f))
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Warning: File not found: {inputFile}");
                continue;
            }

            using var input = File.OpenRead(inputFile);
            input.CopyTo(output);
            totalSize += input.Length;
            Console.WriteLine($"Joined: {inputFile} ({input.Length:N0} bytes)");
        }

        Console.WriteLine($"\nCreated: {outputFile} ({totalSize:N0} bytes)");
    }

    static long ParseSize(string size)
    {
        size = size.ToUpper().Trim();
        
        if (size.EndsWith("KB"))
            return long.Parse(size[..^2]) * 1024;
        if (size.EndsWith("MB"))
            return long.Parse(size[..^2]) * 1024 * 1024;
        if (size.EndsWith("GB"))
            return long.Parse(size[..^2]) * 1024 * 1024 * 1024;
        
        return long.Parse(size);
    }
}
