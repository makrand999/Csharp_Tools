# Twitter/X Thread Formatter

Formats long text content into Twitter/X thread format with character limits.

## Usage

```bash
# From stdin
dotnet run --project ThreadFormatter.csproj --stdin

# From file
dotnet run --project ThreadFormatter.csproj mythread.txt
```

## Example

```
$ dotnet run --project ThreadFormatter.csproj --stdin
> This is a really long thought that I want to share
> as a Twitter thread but it's way too long for a
> single tweet so this tool will help me split it up
> [empty line to end]

🧵 Thread
==================================================

[1/3]
This is a really long thought that I want to share as a Twitter thread but it's

---

[2/3]
way too long for a single tweet so this tool will help me split it up

---

[3/3]
nicely into multiple tweets.

---

End of thread (3 tweets)
```

## Concepts Demonstrated

- String manipulation and StringBuilder
- File I/O operations
- Command-line argument parsing
- Text formatting and splitting
- Character limit enforcement
