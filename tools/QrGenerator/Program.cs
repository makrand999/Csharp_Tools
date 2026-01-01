// QR Code Generator - Generate QR codes as PNG or ASCII
using QRCoder;

class Program
{
    static void Main(string[] args)
    {
        string? text = null;
        string? outputFile = null;
        int size = 10;
        bool ascii = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-t":
                case "--text":
                    if (i + 1 < args.Length)
                        text = args[++i];
                    break;
                case "-o":
                case "--output":
                    if (i + 1 < args.Length)
                        outputFile = args[++i];
                    break;
                case "-s":
                case "--size":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int s))
                        size = Math.Clamp(s, 1, 50);
                    break;
                case "--ascii":
                    ascii = true;
                    break;
                case "-h":
                case "--help":
                    PrintHelp();
                    return;
            }
        }

        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("Enter text to encode in QR code:");
            text = Console.ReadLine();
        }

        if (string.IsNullOrEmpty(text))
        {
            Console.WriteLine("Error: No text provided.");
            return;
        }

        using var generator = new QRCodeGenerator();
        var qrCode = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

        if (ascii)
        {
            string asciiArt = GenerateAsciiQR(qrCode, size);
            
            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, asciiArt);
                Console.WriteLine($"ASCII QR code written to: {outputFile}");
            }
            else
            {
                Console.WriteLine(asciiArt);
            }
        }
        else
        {
            string outputPath = string.IsNullOrEmpty(outputFile) ? "qrcode.png" : outputFile;
            if (!outputPath.EndsWith(".png"))
                outputPath += ".png";

            using var bitmap = new PngByteQRCode(qrCode);
            byte[] pngBytes = bitmap.GetGraphic(20);
            File.WriteAllBytes(outputPath, pngBytes);
            Console.WriteLine($"QR code saved to: {outputPath}");
        }
    }

    static string GenerateAsciiQR(QRCodeData qrCodeData, int size)
    {
        var result = new StringBuilder();
        int moduleCount = qrCodeData.ModuleMatrix.Count;

        for (int y = 0; y < moduleCount; y++)
        {
            for (int s = 0; s < size; s++)
            {
                for (int x = 0; x < moduleCount; x++)
                {
                    bool isBlack = qrCodeData.ModuleMatrix[y][x];
                    result.Append(isBlack ? new string('█', size) : new string(' ', size));
                }
                result.AppendLine();
            }
        }

        return result.ToString();
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
QR Code Generator - Generate QR codes as PNG or ASCII art

Usage: QrGenerator [options]

Options:
  -t, --text <text>    Text to encode in QR code
  -o, --output <file>  Output file (PNG image or ASCII file)
  -s, --size <n>       Size multiplier for ASCII art (default: 10, range: 1-50)
  --ascii              Generate ASCII art instead of PNG
  -h, --help           Show this help message

Examples:
  QrGenerator -t "https://example.com"              # Create PNG
  QrGenerator -t "Hello" -o myqr.png                # Save as PNG
  QrGenerator -t "Hello" --ascii                    # Print ASCII
  QrGenerator -t "Hello" --ascii -o qr.txt -s 5     # ASCII to file
");
    }
}
