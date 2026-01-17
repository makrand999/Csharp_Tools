using System.Security.Cryptography;

Console.WriteLine("Duplicate File Finder");
Console.WriteLine("=====================\n");

Console.Write("Enter directory path to scan: ");
var dirPath = Console.ReadLine()?.Trim();

if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
{
    Console.WriteLine("Error: Invalid directory path.");
    return;
}

Console.Write("Include subdirectories? (y/n): ");
var includeSubdirs = Console.ReadLine()?.Trim().ToLower() == "y";

Console.Write("Minimum file size in bytes (0 for all): ");
var minSizeInput = Console.ReadLine()?.Trim();
long minSize = 0;
long.TryParse(minSizeInput, out minSize);

Console.WriteLine($"\n🔍 Scanning: {dirPath}");
Console.WriteLine($"   Subdirectories: {(includeSubdirs ? "Yes" : "No")}");
Console.WriteLine($"   Minimum size: {minSize} bytes\n");

// Group files by size first (quick filter)
var filesBySize = new Dictionary<long, List<FileInfo>>();
var searchOption = includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

var allFiles = Directory.EnumerateFiles(dirPath, "*", searchOption);
int fileCount = 0;

foreach (var filePath in allFiles)
{
    try
    {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length >= minSize)
        {
            if (!filesBySize.ContainsKey(fileInfo.Length))
                filesBySize[fileInfo.Length] = new List<FileInfo>();
            
            filesBySize[fileInfo.Length].Add(fileInfo);
            fileCount++;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   ⚠️  Cannot access: {filePath} ({ex.Message})");
    }
}

Console.WriteLine($"📁 Found {fileCount} files to analyze.\n");

// Only check files with matching sizes
var potentialDuplicates = filesBySize.Where(kvp => kvp.Value.Count > 1).ToList();

if (potentialDuplicates.Count == 0)
{
    Console.WriteLine("✅ No duplicate files found!");
    return;
}

Console.WriteLine($"🔎 Found {potentialDuplicates.Count} size groups with potential duplicates.\n");
Console.WriteLine("   Computing hashes...\n");

var duplicates = new Dictionary<string, List<FileInfo>>();
int processed = 0;

foreach (var group in potentialDuplicates)
{
    var size = group.Key;
    var files = group.Value;
    
    var hashGroups = new Dictionary<string, List<FileInfo>>();
    
    foreach (var file in files)
    {
        try
        {
            var hash = ComputeFileHash(file.FullName);
            if (!hashGroups.ContainsKey(hash))
                hashGroups[hash] = new List<FileInfo>();
            
            hashGroups[hash].Add(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ⚠️  Cannot read: {file.Name} ({ex.Message})");
        }
    }
    
    // Add groups with duplicates
    foreach (var hashGroup in hashGroups.Where(g => g.Value.Count > 1))
    {
        duplicates[hashGroup.Key] = hashGroup.Value;
    }
    
    processed++;
    Console.Write($"\r   Progress: {processed}/{potentialDuplicates.Count} size groups");
}

Console.WriteLine("\n");

if (duplicates.Count == 0)
{
    Console.WriteLine("✅ No duplicate files found!");
    return;
}

// Report duplicates
Console.WriteLine($"❌ Found {duplicates.Count} sets of duplicate files:\n");

int duplicateSet = 1;
long totalWastedSpace = 0;

foreach (var group in duplicates)
{
    var files = group.Value;
    var fileSize = files[0].Length;
    var wastedSpace = fileSize * (files.Count - 1);
    totalWastedSpace += wastedSpace;
    
    Console.WriteLine($"📋 Duplicate Set #{duplicateSet++} ({files.Count} files, {FormatSize(fileSize)} each):");
    
    foreach (var file in files)
    {
        Console.WriteLine($"   - {file.FullName}");
    }
    
    Console.WriteLine($"   💾 Wasted space: {FormatSize(wastedSpace)}\n");
}

Console.WriteLine($"════════════════════════════════════════");
Console.WriteLine($"📊 Total duplicate sets: {duplicates.Count}");
Console.WriteLine($"💾 Total wasted space: {FormatSize(totalWastedSpace)}");
Console.WriteLine($"════════════════════════════════════════");

Console.WriteLine("\n✅ Done!");

static string ComputeFileHash(string filePath)
{
    using var sha256 = SHA256.Create();
    using var stream = File.OpenRead(filePath);
    var hash = sha256.ComputeHash(stream);
    return Convert.ToHexString(hash).ToLower();
}

static string FormatSize(long bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB" };
    int order = 0;
    double size = bytes;
    
    while (size >= 1024 && order < sizes.Length - 1)
    {
        order++;
        size /= 1024;
    }
    
    return $"{size:0.##} {sizes[order]}";
}
