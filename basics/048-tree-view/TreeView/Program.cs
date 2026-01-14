using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TreeView;

class Program
{
    static int Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : ".";
        int maxDepth = args.Length > 1 && int.TryParse(args[1], out int d) ? d : 10;
        bool showFiles = args.Length < 3 || args[2].ToLower() != "--dirs-only";
        bool showHidden = args.Contains("--hidden") || args.Contains("-a");

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Error: Directory not found: {path}");
            return 1;
        }

        try
        {
            var fullPath = Path.GetFullPath(path);
            Console.WriteLine(GetDirectoryName(fullPath));
            Console.WriteLine();
            
            PrintTree(fullPath, "", true, maxDepth, 0, showFiles, showHidden);
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static string GetDirectoryName(string path)
    {
        var dir = new DirectoryInfo(path);
        return dir.Name == "" ? path : dir.Name;
    }

    static void PrintTree(string path, string prefix, bool isTail, int maxDepth, int currentDepth, 
                          bool showFiles, bool showHidden)
    {
        if (currentDepth >= maxDepth)
            return;

        var entries = GetEntries(path, showFiles, showHidden);
        
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            bool isLast = i == entries.Length - 1;
            string connector = isLast ? "└── " : "├── ";
            
            Console.WriteLine(prefix + connector + GetEntryName(entry));
            
            if (entry is DirectoryInfo dir)
            {
                string newPrefix = prefix + (isLast ? "    " : "│   ");
                try
                {
                    PrintTree(dir.FullName, newPrefix, isLast, maxDepth, currentDepth + 1, showFiles, showHidden);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine(newPrefix + "└── [Access Denied]");
                }
            }
        }
    }

    static FileSystemInfo[] GetEntries(string path, bool showFiles, bool showHidden)
    {
        var dirs = new List<DirectoryInfo>();
        var files = new List<FileInfo>();

        var dirInfo = new DirectoryInfo(path);
        
        try
        {
            foreach (var entry in dirInfo.EnumerateFileSystemInfos())
            {
                // Skip hidden files/directories unless --hidden is specified
                if (!showHidden && (entry.Attributes & FileAttributes.Hidden) != 0)
                    continue;

                // Skip system files
                if ((entry.Attributes & FileAttributes.System) != 0)
                    continue;

                if (entry is DirectoryInfo dir)
                    dirs.Add(dir);
                else if (entry is FileInfo file && showFiles)
                    files.Add(file);
            }
        }
        catch (UnauthorizedAccessException)
        {
            return Array.Empty<FileSystemInfo>();
        }

        // Sort directories first, then files, alphabetically
        return dirs.OrderBy(d => d.Name).Concat<FileSystemInfo>(files.OrderBy(f => f.Name)).ToArray();
    }

    static string GetEntryName(FileSystemInfo entry)
    {
        if (entry is DirectoryInfo)
            return entry.Name + "/";
        
        var file = entry as FileInfo;
        if (file != null)
        {
            // Add file size for files
            string size = FormatFileSize(file.Length);
            return $"{entry.Name} ({size})";
        }
        
        return entry.Name;
    }

    static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}
