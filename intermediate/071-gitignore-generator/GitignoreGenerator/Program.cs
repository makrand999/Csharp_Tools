var templates = new Dictionary<string, string>
{
    ["dotnet"] = """
        # .NET
        [Bb]in/
        [Oo]bj/
        *.user
        *.suo
        *.userosscache
        *.sln.docstates
        [Ll]og/
        [Ll]ogs/
        *.nupkg
        *.snk
        .vs/
        _ReSharper*/
        *.resharper
        TestResults/
        coverage*.json
        """,

    ["node"] = """
        # Node.js
        node_modules/
        npm-debug.log*
        yarn-debug.log*
        yarn-error.log*
        .npm
        .yarn-integrity
        .pnp.*
        dist/
        build/
        """,

    ["python"] = """
        # Python
        __pycache__/
        *.py[cod]
        *$py.class
        .Python
        venv/
        env/
        .env
        .venv
        *.egg-info/
        dist/
        build/
        .pytest_cache/
        .mypy_cache/
        .coverage
        htmlcov/
        """,

    ["java"] = """
        # Java
        *.class
        *.log
        *.jar
        *.war
        *.ear
        target/
        build/
        .gradle/
        .idea/
        *.iml
        .settings/
        .project
        .classpath
        """,

    ["go"] = """
        # Go
        *.exe
        *.exe~
        *.dll
        *.so
        *.dylib
        *.test
        *.out
        vendor/
        go.sum
        """,

    ["rust"] = """
        # Rust
        target/
        **/*.rs.bk
        Cargo.lock
        **/*.pdb
        """,

    ["ruby"] = """
        # Ruby
        *.gem
        *.rbc
        .bundle/
        vendor/bundle
        Gemfile.lock
        .ruby-version
        .ruby-gemset
        log/
        tmp/
        """,

    ["php"] = """
        # PHP
        vendor/
        .phpunit.result.cache
        .php_cs.cache
        composer.lock
        .env
        """,

    ["docker"] = """
        # Docker
        *.pid
        *.seed
        *.pid.lock
        """,

    ["vscode"] = """
        # VS Code
        .vscode/*
        !.vscode/settings.json
        !.vscode/tasks.json
        !.vscode/launch.json
        !.vscode/extensions.json
        *.code-workspace
        """,

    ["idea"] = """
        # IntelliJ IDEA
        .idea/
        *.iml
        *.ipr
        *.iws
        .DS_Store
        """,

    ["os"] = """
        # Operating System
        .DS_Store
        .AppleDouble
        .LSOverride
        Thumbs.db
        Desktop.ini
        """,
};

Console.WriteLine(".gitignore Generator");
Console.WriteLine(new string('-', 60));
Console.WriteLine("\nSelect technologies for your project:\n");

var index = 1;
var techList = templates.Keys.ToList();
foreach (var tech in techList)
{
    Console.WriteLine($"  {index,2}. {tech}");
    index++;
}

Console.WriteLine($"\n  0. Generate (finish selection)");

var selected = new List<string>();

while (true)
{
    Console.Write("\nEnter choice (0 to finish): ");
    var input = Console.ReadLine()?.Trim();
    
    if (input == "0") break;
    
    if (int.TryParse(input, out var choice) && choice >= 1 && choice <= techList.Count)
    {
        var tech = techList[choice - 1];
        if (!selected.Contains(tech))
        {
            selected.Add(tech);
            Console.WriteLine($"  ✓ Added: {tech}");
        }
        else
        {
            Console.WriteLine($"  Already selected: {tech}");
        }
    }
    else
    {
        Console.WriteLine("  Invalid choice");
    }
}

if (selected.Count == 0)
{
    Console.WriteLine("\nNo technologies selected. Exiting.");
    return;
}

Console.WriteLine($"\nGenerating .gitignore for: {string.Join(", ", selected)}");
Console.WriteLine(new string('-', 60));

var gitignoreContent = new System.Text.StringBuilder();
gitignoreContent.AppendLine("# Auto-generated .gitignore");
gitignoreContent.AppendLine($"# Generated for: {string.Join(", ", selected)}");
gitignoreContent.AppendLine($"# Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
gitignoreContent.AppendLine();

foreach (var tech in selected)
{
    gitignoreContent.AppendLine($"# {tech.ToUpper()}");
    gitignoreContent.AppendLine(templates[tech]);
    gitignoreContent.AppendLine();
}

Console.WriteLine(gitignoreContent.ToString());

Console.WriteLine(new string('-', 60));
Console.Write("Save to .gitignore file? (y/n): ");
var save = Console.ReadLine()?.Trim().ToLower();

if (save == "y")
{
    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
    await File.WriteAllTextAsync(outputPath, gitignoreContent.ToString());
    Console.WriteLine($"\n✓ Saved to: {outputPath}");
}
