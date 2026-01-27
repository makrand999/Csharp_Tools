using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;

if (args.Length == 0)
{
    Console.WriteLine("Photo EXIF Reader - Extract metadata from images");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project PhotoExifReader.csproj <image-path>");
    Console.WriteLine("  dotnet run --project PhotoExifReader.csproj <image-path> --json");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project PhotoExifReader.csproj photo.jpg");
    Console.WriteLine("  dotnet run --project PhotoExifReader.csproj image.png --json");
    return;
}

string imagePath = args[0];
bool outputJson = args.Contains("--json");

if (!File.Exists(imagePath))
{
    Console.Error.WriteLine($"Error: File '{imagePath}' not found.");
    return;
}

try
{
    var directories = ImageMetadataReader.ReadMetadata(imagePath);
    
    if (outputJson)
    {
        OutputJson(directories, imagePath);
    }
    else
    {
        OutputHumanReadable(directories);
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error reading metadata: {ex.Message}");
}

static void OutputHumanReadable(IReadOnlyList<MetadataExtractor.Directory> directories)
{
    foreach (var directory in directories)
    {
        Console.WriteLine($"\n{directory.Name}");
        Console.WriteLine(new string('-', 50));
        
        foreach (var tag in directory.Tags)
        {
            Console.WriteLine($"  {tag.Name}: {tag.Description}");
        }
    }
    
    // Highlight key photo information
    var exifIfd = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
    var exifSubIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
    var gpsIfd = directories.FirstOrDefault(d => d.Name.Contains("GPS", StringComparison.OrdinalIgnoreCase));
    
    Console.WriteLine("\n" + new string('=', 50));
    Console.WriteLine("KEY INFORMATION");
    Console.WriteLine(new string('=', 50));
    
    if (exifIfd != null)
    {
        var model = exifIfd.GetString(ExifIfd0Directory.TagModel);
        var make = exifIfd.GetString(ExifIfd0Directory.TagMake);
        if (model != null) Console.WriteLine($"Camera Model: {model}");
        if (make != null) Console.WriteLine($"Manufacturer: {make}");
    }
    
    if (exifSubIfd != null)
    {
        var dateTaken = exifSubIfd.GetDescription(ExifSubIfdDirectory.TagDateTimeOriginal);
        var fNumber = exifSubIfd.GetString(ExifSubIfdDirectory.TagFNumber);
        var exposure = exifSubIfd.GetDescription(ExifSubIfdDirectory.TagExposureTime);
        var iso = exifSubIfd.GetDescription(ExifSubIfdDirectory.TagIsoEquivalent);
        var focalLength = exifSubIfd.GetString(ExifSubIfdDirectory.TagFocalLength);
        
        if (dateTaken != null) Console.WriteLine($"Date Taken: {dateTaken}");
        if (fNumber != null) Console.WriteLine($"Aperture: f/{fNumber}");
        if (exposure != null) Console.WriteLine($"Exposure: {exposure}s");
        if (iso != null) Console.WriteLine($"ISO: {iso}");
        if (focalLength != null) Console.WriteLine($"Focal Length: {focalLength}mm");
    }
    
    if (gpsIfd != null)
    {
        var lat = gpsIfd.GetDescription(2);
        var lon = gpsIfd.GetDescription(4);
        if (lat != null && lon != null)
        {
            Console.WriteLine($"GPS Location: {lat}, {lon}");
        }
    }
    
    var imageWidth = exifIfd?.GetDescription(ExifIfd0Directory.TagImageWidth);
    var imageHeight = exifIfd?.GetDescription(ExifIfd0Directory.TagImageHeight);
    
    if (imageWidth != null || imageHeight != null)
    {
        Console.WriteLine($"Dimensions: {imageWidth} x {imageHeight}");
    }
}

static void OutputJson(IReadOnlyList<MetadataExtractor.Directory> directories, string fileName)
{
    var exifIfd = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
    var exifSubIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
    var gpsIfd = directories.FirstOrDefault(d => d.Name.Contains("GPS", StringComparison.OrdinalIgnoreCase));
    
    var metadata = new
    {
        fileName = System.IO.Path.GetFileName(fileName),
        camera = new
        {
            make = exifIfd?.GetString(ExifIfd0Directory.TagMake),
            model = exifIfd?.GetString(ExifIfd0Directory.TagModel)
        },
        settings = new
        {
            dateTaken = exifSubIfd?.GetDescription(ExifSubIfdDirectory.TagDateTimeOriginal),
            aperture = exifSubIfd?.GetString(ExifSubIfdDirectory.TagFNumber),
            exposure = exifSubIfd?.GetDescription(ExifSubIfdDirectory.TagExposureTime),
            iso = exifSubIfd?.GetDescription(ExifSubIfdDirectory.TagIsoEquivalent),
            focalLength = exifSubIfd?.GetString(ExifSubIfdDirectory.TagFocalLength)
        },
        gps = new
        {
            latitude = gpsIfd?.GetDescription(2),
            longitude = gpsIfd?.GetDescription(4)
        },
        allTags = directories.SelectMany(d => d.Tags.Select(t => new
        {
            directory = d.Name,
            tag = t.Name,
            value = t.Description
        })).ToList()
    };
    
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    }));
}
