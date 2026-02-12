namespace MimeTypeDetector;

class Program
{
    private static readonly Dictionary<string, string> ExtensionToMime = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", "image/jpeg" }, { ".jpeg", "image/jpeg" }, { ".png", "image/png" },
        { ".gif", "image/gif" }, { ".bmp", "image/bmp" }, { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" }, { ".ico", "image/x-icon" }, { ".tiff", "image/tiff" },
        
        // Audio
        { ".mp3", "audio/mpeg" }, { ".wav", "audio/wav" }, { ".ogg", "audio/ogg" },
        { ".flac", "audio/flac" }, { ".m4a", "audio/mp4" }, { ".aac", "audio/aac" },
        
        // Video
        { ".mp4", "video/mp4" }, { ".avi", "video/x-msvideo" }, { ".mkv", "video/x-matroska" },
        { ".webm", "video/webm" }, { ".mov", "video/quicktime" }, { ".wmv", "video/x-ms-wmv" },
        
        // Documents
        { ".pdf", "application/pdf" }, { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        
        // Archives
        { ".zip", "application/zip" }, { ".tar", "application/x-tar" },
        { ".gz", "application/gzip" }, { ".rar", "application/vnd.rar" },
        { ".7z", "application/x-7z-compressed" },
        
        // Web
        { ".html", "text/html" }, { ".htm", "text/html" }, { ".css", "text/css" },
        { ".js", "application/javascript" }, { ".json", "application/json" },
        { ".xml", "application/xml" }, { ".csv", "text/csv" },
        
        // Fonts
        { ".ttf", "font/ttf" }, { ".otf", "font/otf" }, { ".woff", "font/woff" },
        { ".woff2", "font/woff2" }, { ".eot", "application/vnd.ms-fontobject" },
        
        // Other
        { ".txt", "text/plain" }, { ".md", "text/markdown" },
        { ".rtf", "application/rtf" }, { ".exe", "application/x-msdownload" },
        { ".dll", "application/x-msdownload" },
    };

    // Magic bytes for content-based detection
    private static readonly List<(byte[] Pattern, string Mime)> MagicBytes = new()
    {
        (new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "image/png"),
        (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg"),
        (new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif"),
        (new byte[] { 0x25, 0x50, 0x44, 0x46 }, "application/pdf"),
        (new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "application/zip"),
        (new byte[] { 0x1F, 0x8B, 0x08 }, "application/gzip"),
    };

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("MIME Type Detector");
            Console.WriteLine("Usage: dotnet run --project 144-mime-type-detector.csproj -- <file>");
            Console.WriteLine("\nDetects MIME type by extension and magic bytes.");
            return;
        }

        string file = args[0];

        if (!File.Exists(file))
        {
            Console.WriteLine($"Error: File not found: {file}");
            return;
        }

        var extension = Path.GetExtension(file);
        var extensionMime = GetMimeByExtension(extension);
        var contentMime = GetMimeByContent(file);

        Console.WriteLine($"File: {file}");
        Console.WriteLine($"Extension: {extension}");
        Console.WriteLine($"MIME by extension: {extensionMime}");
        Console.WriteLine($"MIME by content: {(contentMime ?? "Unknown")}");
        
        if (extensionMime != contentMime && contentMime != null)
        {
            Console.WriteLine($"Warning: Extension and content MIME types don't match!");
        }
    }

    static string GetMimeByExtension(string extension)
    {
        return ExtensionToMime.GetValueOrDefault(extension, "application/octet-stream");
    }

    static string? GetMimeByContent(string file)
    {
        try
        {
            using var stream = File.OpenRead(file);
            var header = new byte[16];
            var bytesRead = stream.Read(header, 0, header.Length);

            foreach (var (pattern, mime) in MagicBytes)
            {
                if (bytesRead >= pattern.Length)
                {
                    bool match = true;
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        if (header[i] != pattern[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return mime;
                    }
                }
            }
        }
        catch
        {
            // Ignore read errors
        }

        return null;
    }
}
