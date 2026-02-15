using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncBatchDownloader;

/// <summary>
/// Async batch downloader for downloading multiple files concurrently with progress tracking.
/// Demonstrates async streams, progress reporting, and cancellation tokens.
/// </summary>
class Program
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    private static readonly ConcurrentBag<DownloadResult> Results = new();
    private static CancellationTokenSource _cts = new();
    private static int _maxConcurrentDownloads = 5;
    private static string _outputDir = "./downloads";
    private static bool _showProgress = true;

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Async Batch Downloader");
            Console.WriteLine("=======================");
            Console.WriteLine("Usage: dotnet run --project AsyncBatchDownloader.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --urls <file>         File with URLs (one per line, required)");
            Console.WriteLine("  --output <dir>        Output directory (default: ./downloads)");
            Console.WriteLine("  --concurrency <n>     Max concurrent downloads (default: 5)");
            Console.WriteLine("  --no-progress         Disable progress display");
            Console.WriteLine("  --timeout <seconds>   Download timeout in seconds (default: 300)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt");
            Console.WriteLine("  dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt --concurrency 10");
            Console.WriteLine("  dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt --output ./files");
            return 0;
        }

        var urlsFile = string.Empty;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--urls" when i + 1 < args.Length:
                    urlsFile = args[++i];
                    break;
                case "--output" when i + 1 < args.Length:
                    _outputDir = args[++i];
                    break;
                case "--concurrency" when i + 1 < args.Length:
                    _maxConcurrentDownloads = int.Parse(args[++i]);
                    break;
                case "--no-progress":
                    _showProgress = false;
                    break;
                case "--timeout" when i + 1 < args.Length:
                    HttpClient.Timeout = TimeSpan.FromSeconds(int.Parse(args[++i]));
                    break;
            }
        }

        if (string.IsNullOrEmpty(urlsFile) || !File.Exists(urlsFile))
        {
            Console.WriteLine("Error: Please provide a valid URLs file with --urls <file>");
            return 1;
        }

        // Setup cancellation
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\n\nCancellation requested...");
            _cts.Cancel();
        };

        var urls = await File.ReadAllLinesAsync(urlsFile);
        urls = urls.Where(u => !string.IsNullOrWhiteSpace(u)).Select(u => u.Trim()).ToArray();

        if (urls.Length == 0)
        {
            Console.WriteLine("Error: No URLs found in the file.");
            return 1;
        }

        // Create output directory
        Directory.CreateDirectory(_outputDir);

        Console.WriteLine($"Async Batch Downloader");
        Console.WriteLine($"=======================");
        Console.WriteLine($"URLs file: {urlsFile}");
        Console.WriteLine($"Output directory: {Path.GetFullPath(_outputDir)}");
        Console.WriteLine($"Total URLs: {urls.Length}");
        Console.WriteLine($"Max concurrent: {_maxConcurrentDownloads}");
        Console.WriteLine($"Timeout: {HttpClient.Timeout.TotalSeconds}s");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to cancel all downloads");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await DownloadBatchAsync(urls);

            stopwatch.Stop();

            Console.WriteLine($"\n{'=',-70}");
            Console.WriteLine($"Download completed in {FormatTime(stopwatch.Elapsed)}");
            Console.WriteLine($"Total downloads: {Results.Count}");
            Console.WriteLine($"Successful: {Results.Count(r => r.Success)}");
            Console.WriteLine($"Failed: {Results.Count(r => !r.Success)}");
            Console.WriteLine($"Total data: {FormatSize(Results.Sum(r => r.BytesDownloaded))}");

            var successful = Results.Where(r => r.Success).ToList();
            if (successful.Count > 0)
            {
                Console.WriteLine($"\nAverage speed: {FormatSize((long)(successful.Sum(r => r.BytesDownloaded) / stopwatch.Elapsed.TotalSeconds))}/s");
                Console.WriteLine($"\nDownloaded files:");
                foreach (var result in successful.OrderBy(r => r.FileName))
                {
                    Console.WriteLine($"  ✓ {result.FileName} ({FormatSize(result.BytesDownloaded)})");
                }
            }

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"\nDownloads cancelled. {Results.Count} downloads completed before cancellation.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static async Task DownloadBatchAsync(string[] urls)
    {
        var semaphore = new SemaphoreSlim(_maxConcurrentDownloads);
        var tasks = urls.Select(url => DownloadWithSemaphoreAsync(semaphore, url, _cts.Token));
        await Task.WhenAll(tasks);
    }

    static async Task DownloadWithSemaphoreAsync(SemaphoreSlim semaphore, string url, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            await DownloadFileAsync(url, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    static async Task DownloadFileAsync(string url, CancellationToken cancellationToken)
    {
        var result = new DownloadResult
        {
            Url = url,
            FileName = GetFileNameFromUrl(url)
        };

        var outputPath = Path.Combine(_outputDir, result.FileName);
        var tempPath = outputPath + ".tmp";

        try
        {
            if (_showProgress)
                Console.WriteLine($"Starting: {result.FileName}");

            var stopwatch = Stopwatch.StartNew();

            using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            result.TotalBytes = totalBytes;

            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int read;

            while ((read = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                totalRead += read;

                if (_showProgress && totalBytes > 0)
                {
                    var percent = (double)totalRead / totalBytes * 100;
                    Console.Write($"\r  {result.FileName}: {percent:F1}% ({FormatSize(totalRead)}/{FormatSize(totalBytes)})");
                }
            }

            fileStream.Close();
            File.Move(tempPath, outputPath, true);

            stopwatch.Stop();

            result.BytesDownloaded = totalRead;
            result.Success = true;
            result.DownloadTime = stopwatch.Elapsed;

            if (_showProgress)
                Console.WriteLine($"\r  ✓ {result.FileName} completed ({FormatSize(totalRead)} in {FormatTime(stopwatch.Elapsed)})");

            Results.Add(result);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;

            // Cleanup temp file
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            if (_showProgress)
                Console.WriteLine($"\r  ✗ {result.FileName} failed: {ex.Message}");

            Results.Add(result);

            if (ex is OperationCanceledException)
                throw;
        }
    }

    static string GetFileNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.AbsolutePath);
            if (string.IsNullOrEmpty(fileName))
                fileName = $"download_{Guid.NewGuid():N}";
            return fileName;
        }
        catch
        {
            return $"download_{Guid.NewGuid():N}";
        }
    }

    static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F2} {sizes[order]}";
    }

    static string FormatTime(TimeSpan time)
    {
        if (time.TotalMinutes >= 1)
            return $"{time.Minutes}m {time.Seconds}s";
        if (time.TotalSeconds >= 1)
            return $"{time.Seconds}.{time.Milliseconds / 100}s";
        return $"{time.Milliseconds}ms";
    }
}

class DownloadResult
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public long BytesDownloaded { get; set; }
    public long? TotalBytes { get; set; }
    public TimeSpan DownloadTime { get; set; }
    public string? ErrorMessage { get; set; }
}
