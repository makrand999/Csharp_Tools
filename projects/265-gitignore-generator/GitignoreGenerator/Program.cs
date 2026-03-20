namespace GitignoreGenerator;

class Program
{
    private static readonly Dictionary<string, string> Templates = new()
    {
        ["dotnet"] = """
            # Build results
            [Bb]in/
            [Oo]bj/
            *.dll
            *.exe
            *.pdb
            *.user
            *.suo
            *.userosscache
            *.sln.docstates
            
            # Visual Studio
            .vs/
            *.user
            [Ss]hare[dD]Projects/
            
            # Rider
            .idea/
            
            # VS Code
            .vscode/
            
            # Test results
            [Tt]est[Rr]esult*/
            [Bb]uild[Ll]og.*
            
            # NuGet
            *.nupkg
            packages/
            
            """,

        ["node"] = """
            # Dependencies
            node_modules/
            npm-debug.log*
            yarn-debug.log*
            yarn-error.log*
            
            # Build output
            dist/
            build/
            
            # Environment
            .env
            .env.local
            .env.*.local
            
            # Logs
            logs/
            *.log
            
            # OS
            .DS_Store
            Thumbs.db
            
            # IDE
            .idea/
            .vscode/
            *.swp
            *.swo
            
            """,

        ["python"] = """
            # Byte-compiled
            __pycache__/
            *.py[cod]
            *$py.class
            
            # Virtual environments
            venv/
            env/
            .venv/
            .env/
            
            # Distribution
            dist/
            build/
            *.egg-info/
            *.egg
            
            # IDE
            .idea/
            .vscode/
            *.swp
            *.swo
            .project
            .pydevproject
            
            # Testing
            .pytest_cache/
            .coverage
            htmlcov/
            
            # Type checkers
            .mypy_cache/
            .pytype/
            
            """,

        ["java"] = """
            # Compiled
            *.class
            *.jar
            *.war
            *.ear
            
            # Build directories
            target/
            build/
            out/
            
            # IDE
            .idea/
            .vscode/
            *.iml
            .classpath
            .project
            .settings/
            
            # Maven
            pom.xml.tag
            pom.xml.releaseBackup
            
            # Gradle
            .gradle/
            gradle-app.setting
            
            """,

        ["go"] = """
            # Binaries
            *.exe
            *.exe~
            *.dll
            *.so
            *.dylib
            
            # Test binary
            *.test
            
            # Output
            *.out
            
            # Dependencies
            vendor/
            
            # IDE
            .idea/
            .vscode/
            *.swp
            *.swo
            
            """,

        ["rust"] = """
            # Build output
            target/
            **/target/
            
            # Cargo lock
            Cargo.lock
            
            # Build output (debug)
            debug/
            
            # IDE
            .idea/
            .vscode/
            *.swp
            *.swo
            *.rs.bk
            
            # Rust Analyzer
            rust-analyzer/
            
            """,

        ["docker"] = """
            # Docker
            *.log
            docker-compose.override.yml
            
            # Build context
            .dockerignore
            
            # Volumes
            /var/lib/docker/
            
            """,

        ["rails"] = """
            # Rails
            log/
            tmp/
            public/assets
            .bundle/
            vendor/bundle/
            
            # Database
            db/*.sqlite3
            db/*.sqlite3-journal
            
            # Environment
            .env
            .env.local
            
            # IDE
            .idea/
            .vscode/
            *.swp
            
            # OS
            .DS_Store
            
            """,
    };

    static void Main(string[] args)
    {
        Console.WriteLine("🔧 .gitignore Generator");
        Console.WriteLine("=======================\n");

        Console.WriteLine("Available templates:");
        int index = 1;
        var templateList = Templates.Keys.ToList();
        foreach (var template in templateList)
        {
            Console.WriteLine($"  {index}. {template}");
            index++;
        }

        Console.WriteLine("\nSelect templates (comma-separated numbers, e.g., 1,2,3):");
        Console.Write("> ");

        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No selection made. Exiting.");
            return;
        }

        var selected = new List<string>();
        foreach (var part in input.Split(','))
        {
            if (int.TryParse(part.Trim(), out int num) && num >= 1 && num <= templateList.Count)
            {
                selected.Add(templateList[num - 1]);
            }
        }

        if (selected.Count == 0)
        {
            Console.WriteLine("No valid templates selected.");
            return;
        }

        Console.WriteLine($"\nGenerating .gitignore for: {string.Join(", ", selected)}");

        var gitignore = new System.Text.StringBuilder();
        gitignore.AppendLine("# Auto-generated .gitignore");
        gitignore.AppendLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        gitignore.AppendLine($"# Templates: {string.Join(", ", selected)}");
        gitignore.AppendLine();

        foreach (var template in selected)
        {
            gitignore.AppendLine($"# === {template.ToUpper()} ===");
            gitignore.AppendLine(Templates[template]);
            gitignore.AppendLine();
        }

        string outputFile = ".gitignore";
        File.WriteAllText(outputFile, gitignore.ToString());

        Console.WriteLine($"\n✅ .gitignore generated successfully!");
        Console.WriteLine($"📁 Saved to: {Path.GetFullPath(outputFile)}");
        Console.WriteLine("\n--- Preview ---\n");
        Console.WriteLine(gitignore.ToString());
    }
}
