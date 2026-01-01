// File Organizer - Organizes files in a directory by their extension
class Program
{
    static void Main(string[] args)
    {
        string targetDir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

        if (!Directory.Exists(targetDir))
        {
            Console.WriteLine($"Error: Directory '{targetDir}' does not exist.");
            return;
        }

        Console.WriteLine($"Organizing files in: {targetDir}");
        
        var files = Directory.GetFiles(targetDir, "*.*", SearchOption.TopDirectoryOnly);
        var extensions = new Dictionary<string, List<string>>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file).TrimStart('.').ToLower();
            if (string.IsNullOrEmpty(ext)) ext = "no_extension";

            if (!extensions.ContainsKey(ext))
                extensions[ext] = new List<string>();
            
            extensions[ext].Add(file);
        }

        int movedCount = 0;
        foreach (var extGroup in extensions)
        {
            string folderName = extGroup.Key == "no_extension" ? "_NoExtension" : extGroup.Key.ToUpper();
            string folderPath = Path.Combine(targetDir, folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            foreach (var file in extGroup.Value)
            {
                string fileName = Path.GetFileName(file);
                string newPath = Path.Combine(folderPath, fileName);

                if (File.Exists(newPath))
                {
                    Console.WriteLine($"Skip: {fileName} (already exists)");
                    continue;
                }

                File.Move(file, newPath);
                Console.WriteLine($"Moved: {fileName} -> {folderName}/");
                movedCount++;
            }
        }

        Console.WriteLine($"\nDone! Moved {movedCount} files into {extensions.Count} folders.");
    }
}
