using System.Text;

namespace TextEncodingConverter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Text Encoding Converter");
            Console.WriteLine("Usage: dotnet run --project 146-text-encoding-converter.csproj -- <input> <output> --from <enc> --to <enc>");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --from ENC   Source encoding (default: UTF-8)");
            Console.WriteLine("  --to ENC     Target encoding (default: UTF-8)");
            Console.WriteLine("\nSupported encodings:");
            Console.WriteLine("  UTF-8, UTF-16, UTF-16LE, UTF-16BE, UTF-32, ASCII, ISO-8859-1, Windows-1252");
            return;
        }

        string inputFile = args[0];
        string outputFile = args[1];
        string fromEncoding = "UTF-8";
        string toEncoding = "UTF-8";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--from" && i + 1 < args.Length)
            {
                fromEncoding = args[i + 1];
            }
            else if (args[i] == "--to" && i + 1 < args.Length)
            {
                toEncoding = args[i + 1];
            }
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File not found: {inputFile}");
            return;
        }

        try
        {
            var fromEnc = GetEncoding(fromEncoding);
            var toEnc = GetEncoding(toEncoding);

            var bytes = File.ReadAllBytes(inputFile);
            var text = fromEnc.GetString(bytes);
            var outputBytes = toEnc.GetBytes(text);

            File.WriteAllBytes(outputFile, outputBytes);

            Console.WriteLine($"Converted: {inputFile} -> {outputFile}");
            Console.WriteLine($"From: {fromEncoding} ({bytes.Length:N0} bytes)");
            Console.WriteLine($"To: {toEncoding} ({outputBytes.Length:N0} bytes)");
            Console.WriteLine($"Characters: {text.Length:N0}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static Encoding GetEncoding(string name)
    {
        return name.ToUpper() switch
        {
            "UTF-8" => Encoding.UTF8,
            "UTF-16" or "UTF-16BE" => Encoding.BigEndianUnicode,
            "UTF-16LE" => Encoding.Unicode,
            "UTF-32" => Encoding.UTF32,
            "ASCII" => Encoding.ASCII,
            "ISO-8859-1" or "LATIN1" => Encoding.GetEncoding("ISO-8859-1"),
            "WINDOWS-1252" or "CP1252" => Encoding.GetEncoding("Windows-1252"),
            _ => Encoding.UTF8
        };
    }
}
