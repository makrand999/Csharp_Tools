# Binary File Editor (Hex Viewer)

View and analyze binary files in hexadecimal format.

## Usage

```bash
dotnet run --project 150-binary-editor.csproj -- <file>
```

## Example

```bash
dotnet run --project 150-binary-editor.csproj -- program.exe
dotnet run --project 150-binary-editor.csproj -- image.png --offset 0x100 --length 64
dotnet run --project 150-binary-editor.csproj -- file.bin --search "89 50 4E 47"
dotnet run --project 150-binary-editor.csproj -- unknown.bin --info
```

## Options

- `--offset N` - Start at offset N (hex or decimal)
- `--length N` - Show N bytes (default: 256)
- `--search HEX` - Search for hex pattern
- `--info` - Show file information

## Sample Output

```
File: program.exe
Offset: 0x00000000 (0)
Showing: 256 bytes

00000000  4D 5A 90 00 03 00 00 00  04 00 00 00 FF FF 00 00  |MZ..............|
00000010  B8 00 00 00 00 00 00 00  40 00 00 00 00 00 00 00  |........@.......|
...
```

## Concepts Demonstrated

- Binary file reading
- Hex dump formatting
- Pattern searching
- File metadata analysis
