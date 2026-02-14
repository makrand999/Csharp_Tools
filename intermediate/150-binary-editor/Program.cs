namespace BinaryEditor;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Binary File Editor (Hex Viewer)");
            Console.WriteLine("Usage: dotnet run --project 150-binary-editor.csproj -- <file>");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --offset N    Start at offset N (hex or decimal)");
            Console.WriteLine("  --length N    Show N bytes (default: 256)");
            Console.WriteLine("  --search HEX  Search for hex pattern");
            Console.WriteLine("  --info        Show file information");
            return;
        }

        string inputFile = "";
        int offset = 0;
        int length = 256;
        string? searchPattern = null;
        bool showInfo = false;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--offset" && i + 1 < args.Length)
            {
                offset = ParseNumber(args[i + 1]);
            }
            else if (args[i] == "--length" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out length);
            }
            else if (args[i] == "--search" && i + 1 < args.Length)
            {
                searchPattern = args[i + 1];
            }
            else if (args[i] == "--info")
            {
                showInfo = true;
            }
            else if (string.IsNullOrEmpty(inputFile))
            {
                inputFile = args[i];
            }
        }

        if (string.IsNullOrEmpty(inputFile))
        {
            Console.WriteLine("Error: No input file specified");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File not found: {inputFile}");
            return;
        }

        try
        {
            if (showInfo)
            {
                ShowFileInfo(inputFile);
                return;
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                SearchPattern(inputFile, searchPattern);
                return;
            }

            ViewHex(inputFile, offset, length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ViewHex(string file, int offset, int length)
    {
        using var stream = File.OpenRead(file);
        stream.Position = offset;

        var buffer = new byte[length];
        var bytesRead = stream.Read(buffer, 0, length);

        Console.WriteLine($"File: {file}");
        Console.WriteLine($"Offset: 0x{offset:X8} ({offset})");
        Console.WriteLine($"Showing: {bytesRead} bytes\n");

        for (int i = 0; i < bytesRead; i += 16)
        {
            // Address
            Console.Write($"{offset + i:X8}  ");

            // Hex bytes
            for (int j = 0; j < 16 && i + j < bytesRead; j++)
            {
                Console.Write($"{buffer[i + j]:X2} ");
                if (j == 7) Console.Write(" ");
            }

            // Pad to align
            for (int j = bytesRead - i; j < 16; j++)
            {
                Console.Write("   ");
                if (j == 7) Console.Write(" ");
            }

            // ASCII representation
            Console.Write("  |");
            for (int j = 0; j < 16 && i + j < bytesRead; j++)
            {
                char c = (char)buffer[i + j];
                Console.Write(char.IsControl(c) ? '.' : c);
            }
            Console.WriteLine("|");
        }
    }

    static void SearchPattern(string file, string pattern)
    {
        var bytes = ParseHexPattern(pattern);
        if (bytes.Length == 0)
        {
            Console.WriteLine("Invalid hex pattern");
            return;
        }

        using var stream = File.OpenRead(file);
        var fileBytes = new byte[stream.Length];
        stream.Read(fileBytes, 0, fileBytes.Length);

        var matches = new List<int>();
        for (int i = 0; i <= fileBytes.Length - bytes.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < bytes.Length; j++)
            {
                if (fileBytes[i + j] != bytes[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                matches.Add(i);
            }
        }

        Console.WriteLine($"Pattern: {pattern}");
        Console.WriteLine($"Found {matches.Count} match(es) at offset(s):");
        foreach (var match in matches.Take(20))
        {
            Console.WriteLine($"  0x{match:X8} ({match})");
        }
        if (matches.Count > 20)
        {
            Console.WriteLine($"  ... and {matches.Count - 20} more");
        }
    }

    static void ShowFileInfo(string file)
    {
        var info = new FileInfo(file);
        
        Console.WriteLine($"File: {info.Name}");
        Console.WriteLine($"Full path: {info.FullName}");
        Console.WriteLine($"Size: {info.Length:N0} bytes");
        Console.WriteLine($"Extension: {info.Extension}");
        Console.WriteLine($"Created: {info.CreationTime}");
        Console.WriteLine($"Modified: {info.LastWriteTime}");
        Console.WriteLine($"Attributes: {info.Attributes}");

        // Read first bytes for magic number detection
        using var stream = File.OpenRead(file);
        var header = new byte[16];
        var read = stream.Read(header, 0, header.Length);
        
        Console.WriteLine($"\nFirst {read} bytes (hex):");
        Console.WriteLine(BitConverter.ToString(header, 0, read).Replace("-", " "));
    }

    static int ParseNumber(string s)
    {
        if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToInt32(s[2..], 16);
        }
        return int.Parse(s);
    }

    static byte[] ParseHexPattern(string pattern)
    {
        pattern = pattern.Replace(" ", "").Replace("-", "");
        if (pattern.Length % 2 != 0)
        {
            return Array.Empty<byte>();
        }

        var bytes = new byte[pattern.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(pattern.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}
