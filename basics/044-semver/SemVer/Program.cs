using System;
using System.Text.RegularExpressions;

namespace SemVer;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Semantic Version Comparator");
            Console.WriteLine("Usage: dotnet run --project SemVer.csproj <version1> <version2>");
            Console.WriteLine("Example: dotnet run --project SemVer.csproj 1.2.3 2.0.0");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  compare <v1> <v2>  - Compare two versions");
            Console.WriteLine("  validate <version> - Validate a version string");
            Console.WriteLine("  increment <version> <part> - Increment version (major/minor/patch)");
            return 1;
        }

        string command = args[0].ToLower();

        try
        {
            return command switch
            {
                "compare" when args.Length >= 3 => CompareVersions(args[1], args[2]),
                "validate" when args.Length >= 2 => ValidateVersion(args[1]),
                "increment" when args.Length >= 3 => IncrementVersion(args[1], args[2]),
                _ => CompareVersions(args[0], args[1])
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int CompareVersions(string version1, string version2)
    {
        var v1 = SemanticVersion.Parse(version1);
        var v2 = SemanticVersion.Parse(version2);

        Console.WriteLine($"Comparing: {version1} vs {version2}");
        Console.WriteLine();
        Console.WriteLine($"Version 1: {v1}");
        Console.WriteLine($"Version 2: {v2}");
        Console.WriteLine();

        int comparison = v1.CompareTo(v2);
        
        if (comparison < 0)
        {
            Console.WriteLine($"Result: {version1} < {version2}");
        }
        else if (comparison > 0)
        {
            Console.WriteLine($"Result: {version1} > {version2}");
        }
        else
        {
            Console.WriteLine($"Result: {version1} == {version2}");
        }

        return 0;
    }

    static int ValidateVersion(string version)
    {
        Console.WriteLine($"Validating: {version}");
        
        try
        {
            var semVer = SemanticVersion.Parse(version);
            Console.WriteLine("✓ Valid semantic version");
            Console.WriteLine($"  Major: {semVer.Major}");
            Console.WriteLine($"  Minor: {semVer.Minor}");
            Console.WriteLine($"  Patch: {semVer.Patch}");
            if (!string.IsNullOrEmpty(semVer.Prerelease))
                Console.WriteLine($"  Prerelease: {semVer.Prerelease}");
            if (!string.IsNullOrEmpty(semVer.BuildMetadata))
                Console.WriteLine($"  Build: {semVer.BuildMetadata}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Invalid: {ex.Message}");
            return 1;
        }
    }

    static int IncrementVersion(string version, string part)
    {
        var semVer = SemanticVersion.Parse(version);
        
        Console.WriteLine($"Incrementing {version} by {part.ToLower()}");
        Console.WriteLine();

        var newVersion = part.ToLower() switch
        {
            "major" => new SemanticVersion(semVer.Major + 1, 0, 0),
            "minor" => new SemanticVersion(semVer.Major, semVer.Minor + 1, 0),
            "patch" => new SemanticVersion(semVer.Major, semVer.Minor, semVer.Patch + 1),
            _ => throw new ArgumentException("Invalid part. Use: major, minor, or patch")
        };

        Console.WriteLine($"Result: {newVersion}");
        return 0;
    }
}

class SemanticVersion : IComparable<SemanticVersion>
{
    private static readonly Regex Pattern = new(
        @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?$"
    );

    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    public string? Prerelease { get; }
    public string? BuildMetadata { get; }

    public SemanticVersion(int major, int minor, int patch, string? prerelease = null, string? buildMetadata = null)
    {
        if (major < 0 || minor < 0 || patch < 0)
            throw new ArgumentException("Version numbers must be non-negative");

        Major = major;
        Minor = minor;
        Patch = patch;
        Prerelease = prerelease;
        BuildMetadata = buildMetadata;
    }

    public static SemanticVersion Parse(string version)
    {
        var match = Pattern.Match(version.TrimStart('v'));
        
        if (!match.Success)
            throw new ArgumentException($"Invalid semantic version: {version}");

        return new SemanticVersion(
            int.Parse(match.Groups[1].Value),
            int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value),
            match.Groups[4].Success ? match.Groups[4].Value : null,
            match.Groups[5].Success ? match.Groups[5].Value : null
        );
    }

    public int CompareTo(SemanticVersion? other)
    {
        if (other is null) return 1;

        int majorCompare = Major.CompareTo(other.Major);
        if (majorCompare != 0) return majorCompare;

        int minorCompare = Minor.CompareTo(other.Minor);
        if (minorCompare != 0) return minorCompare;

        int patchCompare = Patch.CompareTo(other.Patch);
        if (patchCompare != 0) return patchCompare;

        // Prerelease versions have lower precedence
        if (string.IsNullOrEmpty(Prerelease) && !string.IsNullOrEmpty(other.Prerelease))
            return 1;
        if (!string.IsNullOrEmpty(Prerelease) && string.IsNullOrEmpty(other.Prerelease))
            return -1;

        return string.Compare(Prerelease, other.Prerelease, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if (!string.IsNullOrEmpty(Prerelease))
            version += $"-{Prerelease}";
        if (!string.IsNullOrEmpty(BuildMetadata))
            version += $"+{BuildMetadata}";
        return version;
    }
}
