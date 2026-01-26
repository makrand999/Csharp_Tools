using System.Text.Json;
using TagLib;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var action = "read";
var files = new List<string>();
var title = "";
var artist = "";
var album = "";
var year = "";
var genre = "";
var track = 0;
var removeCover = false;
var coverPath = "";

for (int i = 0; i < args.Length; i++)
{
    var arg = args[i];
    
    switch (arg)
    {
        case "--read":
        case "-r":
            action = "read";
            break;
        case "--write":
        case "-w":
            action = "write";
            break;
        case "--title":
        case "-t":
            if (i + 1 < args.Length) title = args[++i];
            break;
        case "--artist":
        case "-a":
            if (i + 1 < args.Length) artist = args[++i];
            break;
        case "--album":
        case "-l":
            if (i + 1 < args.Length) album = args[++i];
            break;
        case "--year":
        case "-y":
            if (i + 1 < args.Length) year = args[++i];
            break;
        case "--genre":
        case "-g":
            if (i + 1 < args.Length) genre = args[++i];
            break;
        case "--track":
        case "-n":
            if (i + 1 < args.Length && int.TryParse(args[++i], out var t)) track = t;
            break;
        case "--cover":
        case "-c":
            if (i + 1 < args.Length) coverPath = args[++i];
            break;
        case "--remove-cover":
            removeCover = true;
            break;
        case "--json":
        case "-j":
            action = "json";
            break;
        default:
            if (!arg.StartsWith("-") && System.IO.File.Exists(arg))
            {
                files.Add(arg);
            }
            break;
    }
}

if (files.Count == 0)
{
    Console.Error.WriteLine("Error: No audio files specified.");
    ShowUsage();
    Environment.Exit(1);
}

var supportedExtensions = new[] { ".mp3", ".ogg", ".flac", ".m4a", ".wav", ".wma", ".aac", ".opus" };

foreach (var filePath in files)
{
    var ext = Path.GetExtension(filePath).ToLowerInvariant();
    if (!supportedExtensions.Contains(ext))
    {
        Console.Error.WriteLine($"Warning: Unsupported format: {filePath}");
        continue;
    }
    
    try
    {
        using var file = TagLib.File.Create(filePath);
        
        if (action == "read" || action == "json")
        {
            var metadata = ReadTags(file);
            
            if (action == "json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                Console.WriteLine(JsonSerializer.Serialize(metadata, options));
            }
            else
            {
                PrintTags(filePath, metadata);
            }
        }
        else if (action == "write")
        {
            WriteTags(file, title, artist, album, year, genre, track, coverPath, removeCover);
            Console.WriteLine($"✓ Updated: {Path.GetFileName(filePath)}");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error processing {filePath}: {ex.Message}");
    }
}

static Dictionary<string, object?> ReadTags(TagLib.File file)
{
    var tag = file.Tag;
    
    return new Dictionary<string, object?>
    {
        ["File"] = Path.GetFileName(file.Name),
        ["Path"] = Path.GetFullPath(file.Name),
        ["Type"] = file.GetType().Name.Replace("File", ""),
        ["Duration"] = TimeSpan.FromSeconds(file.Properties.Duration.TotalSeconds).ToString(@"mm\:ss"),
        ["Bitrate"] = $"{file.Properties.AudioBitrate} kbps",
        ["SampleRate"] = $"{file.Properties.AudioSampleRate} Hz",
        ["Channels"] = file.Properties.AudioChannels,
        ["Title"] = string.IsNullOrEmpty(tag?.Title) ? null : tag.Title,
        ["Artist"] = string.IsNullOrEmpty(tag?.FirstPerformer) ? null : tag.FirstPerformer,
        ["Album"] = string.IsNullOrEmpty(tag?.Album) ? null : tag.Album,
        ["Year"] = tag?.Year == 0 ? null : tag.Year.ToString(),
        ["Genre"] = string.IsNullOrEmpty(tag?.FirstGenre) ? null : tag.FirstGenre,
        ["Track"] = tag?.Track == 0 ? null : tag.Track.ToString(),
        ["TrackCount"] = tag?.TrackCount == 0 ? null : tag.TrackCount.ToString(),
        ["Disc"] = tag?.Disc == 0 ? null : tag.Disc.ToString(),
        ["Comment"] = string.IsNullOrEmpty(tag?.Comment) ? null : tag.Comment,
        ["AlbumArtist"] = string.IsNullOrEmpty(tag?.FirstAlbumArtist) ? null : tag.FirstAlbumArtist,
        ["Composer"] = string.IsNullOrEmpty(tag?.FirstComposer) ? null : tag.FirstComposer,
        ["HasCoverArt"] = tag?.Pictures.Length > 0,
        ["CoverArtCount"] = tag?.Pictures.Length
    };
}

static void PrintTags(string filePath, Dictionary<string, object?> metadata)
{
    Console.WriteLine($"\n{'='*60}");
    Console.WriteLine($"{Path.GetFileName(filePath)}");
    Console.WriteLine($"{'='*60}");
    
    Console.WriteLine($"\n🎵 Audio Properties:");
    Console.WriteLine($"  Duration:   {metadata["Duration"]}");
    Console.WriteLine($"  Bitrate:    {metadata["Bitrate"]}");
    Console.WriteLine($"  SampleRate: {metadata["SampleRate"]}");
    Console.WriteLine($"  Channels:   {metadata["Channels"]}");
    
    Console.WriteLine($"\n🏷️  Tags:");
    
    var tagFields = new[] { "Title", "Artist", "Album", "AlbumArtist", "Year", "Genre", "Track", "Disc", "Composer", "Comment" };
    var hasTags = false;
    
    foreach (var field in tagFields)
    {
        if (metadata.TryGetValue(field, out var value) && value != null)
        {
            Console.WriteLine($"  {field,-12}: {value}");
            hasTags = true;
        }
    }
    
    if (!hasTags)
    {
        Console.WriteLine("  (No tags)");
    }
    
    if (metadata.TryGetValue("HasCoverArt", out var hasCover) && hasCover is bool cover && cover)
    {
        Console.WriteLine($"  Cover Art:  Yes ({metadata["CoverArtCount"]} image(s))");
    }
}

static void WriteTags(TagLib.File file, string title, string artist, string album, 
    string year, string genre, int track, string coverPath, bool removeCover)
{
    var tag = file.Tag;
    
    if (!string.IsNullOrEmpty(title)) tag.Title = title;
    if (!string.IsNullOrEmpty(artist)) tag.Performers = new[] { artist };
    if (!string.IsNullOrEmpty(album)) tag.Album = album;
    if (!string.IsNullOrEmpty(year) && int.TryParse(year, out var y)) tag.Year = (uint)y;
    if (!string.IsNullOrEmpty(genre)) tag.Genres = new[] { genre };
    if (track > 0) tag.Track = (uint)track;

    // Handle cover art
    if (removeCover)
    {
        tag.Pictures = Array.Empty<IPicture>();
    }
    else if (!string.IsNullOrEmpty(coverPath) && System.IO.File.Exists(coverPath))
    {
        var pictureData = System.IO.File.ReadAllBytes(coverPath);
        var picture = new Picture(pictureData);
        tag.Pictures = new[] { picture };
    }

    file.Save();
}

static void ShowUsage()
{
    Console.WriteLine(@"Audio Tagger - Read and write ID3 tags for audio files

Usage:
  dotnet run --project AudioTagger.csproj [options] <file1> [file2] ...

Actions:
  -r, --read              Read and display tags (default)
  -w, --write             Write tags to files
  -j, --json              Output as JSON

Tag Options (for --write):
  -t, --title <title>     Set track title
  -a, --artist <artist>   Set artist name
  -l, --album <album>     Set album name
  -y, --year <year>       Set release year
  -g, --genre <genre>     Set genre
  -n, --track <number>    Set track number
  -c, --cover <image>     Set cover art from image file
  --remove-cover          Remove existing cover art

Supported Formats:
  MP3, OGG, FLAC, M4A, WAV, WMA, AAC, Opus

Examples:
  dotnet run --project AudioTagger.csproj song.mp3
  dotnet run --project AudioTagger.csproj --json song.mp3
  dotnet run --project AudioTagger.csproj -w -t ""My Song"" -a ""Artist"" song.mp3
  dotnet run --project AudioTagger.csproj -w --cover cover.jpg song.mp3
  dotnet run --project AudioTagger.csproj -w --remove-cover song.mp3
  dotnet run --project AudioTagger.csproj *.mp3");
}
