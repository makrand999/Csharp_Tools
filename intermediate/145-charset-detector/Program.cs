using System.Text;

namespace CharsetDetector;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Character Encoding Detector");
            Console.WriteLine("Usage: dotnet run --project 145-charset-detector.csproj -- <file>");
            Console.WriteLine("\nDetects likely character encoding of text files.");
            return;
        }

        string file = args[0];

        if (!File.Exists(file))
        {
            Console.WriteLine($"Error: File not found: {file}");
            return;
        }

        try
        {
            var bytes = File.ReadAllBytes(file);
            var detected = DetectEncoding(bytes);

            Console.WriteLine($"File: {file}");
            Console.WriteLine($"Size: {bytes.Length:N0} bytes");
            Console.WriteLine();
            Console.WriteLine("=== Detected Encodings ===");
            
            foreach (var result in detected)
            {
                Console.WriteLine($"{result.Encoding,-12} Confidence: {result.Confidence,-5} Valid chars: {result.ValidChars}/{result.TotalChars} ({result.ValidChars * 100.0 / result.TotalChars:F1}%)");
            }

            // Try to display first few lines with best encoding
            var bestEncoding = detected.OrderByDescending(r => r.Confidence).First();
            Console.WriteLine($"\n=== Preview (using {bestEncoding.Encoding}) ===");
            
            try
            {
                var encoding = Encoding.GetEncoding(bestEncoding.Encoding);
                var text = encoding.GetString(bytes);
                var lines = text.Split('\n').Take(5);
                foreach (var line in lines)
                {
                    Console.WriteLine(line.TrimEnd());
                }
            }
            catch
            {
                Console.WriteLine("(Could not decode preview)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static List<EncodingResult> DetectEncoding(byte[] bytes)
    {
        var results = new List<EncodingResult>();
        
        var encodingsToTry = new[]
        {
            ("UTF-8", Encoding.UTF8),
            ("UTF-16LE", Encoding.Unicode),
            ("UTF-16BE", Encoding.BigEndianUnicode),
            ("UTF-32LE", Encoding.UTF32),
            ("ASCII", Encoding.ASCII),
            ("ISO-8859-1", Encoding.GetEncoding("ISO-8859-1")),
            ("Windows-1252", Encoding.GetEncoding("Windows-1252")),
        };

        foreach (var (name, encoding) in encodingsToTry)
        {
            try
            {
                var text = encoding.GetString(bytes);
                var validChars = text.Count(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t');
                var totalChars = text.Length;
                var confidence = validChars * 100.0 / totalChars;

                // Check for BOM
                if (HasBom(bytes, encoding))
                {
                    confidence = Math.Min(100, confidence + 20);
                }

                results.Add(new EncodingResult
                {
                    Encoding = name,
                    Confidence = confidence,
                    ValidChars = validChars,
                    TotalChars = totalChars
                });
            }
            catch
            {
                results.Add(new EncodingResult
                {
                    Encoding = name,
                    Confidence = 0,
                    ValidChars = 0,
                    TotalChars = 0
                });
            }
        }

        return results.OrderByDescending(r => r.Confidence).ToList();
    }

    static bool HasBom(byte[] bytes, Encoding encoding)
    {
        var bom = encoding.GetPreamble();
        if (bom.Length == 0) return false;
        if (bytes.Length < bom.Length) return false;

        for (int i = 0; i < bom.Length; i++)
        {
            if (bytes[i] != bom[i]) return false;
        }
        return true;
    }
}

class EncodingResult
{
    public string Encoding { get; set; } = "";
    public double Confidence { get; set; }
    public int ValidChars { get; set; }
    public int TotalChars { get; set; }
}
