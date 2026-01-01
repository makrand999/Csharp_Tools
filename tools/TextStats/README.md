# TextStats

Analyzes text files and provides detailed statistics including character count, word count, sentence count, and word frequency.

## Usage

```bash
# Analyze a single file
dotnet run --project TextStats.csproj -- myfile.txt

# Analyze multiple files
dotnet run --project TextStats.csproj -- file1.txt file2.txt file3.txt

# Analyze from stdin
cat myfile.txt | dotnet run --project TextStats.csproj -- --stdin
```

## Statistics Provided

- Characters (with and without spaces)
- Lines count
- Words count
- Sentences count
- Average word length
- Average words per line
- Top 5 most common words

## Build

```bash
dotnet build -c Release
```
