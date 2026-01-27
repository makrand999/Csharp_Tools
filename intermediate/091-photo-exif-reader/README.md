# Photo EXIF Reader

Extract and display EXIF metadata from image files (JPEG, PNG, TIFF, etc.). Shows camera information, photo settings, GPS coordinates, and all embedded metadata.

## Usage

```bash
# Basic human-readable output
dotnet run --project PhotoExifReader.csproj <image-path>

# JSON output for programmatic use
dotnet run --project PhotoExifReader.csproj <image-path> --json
```

## Examples

```
$ dotnet run --project PhotoExifReader.csproj photo.jpg

Exif IFD0
--------------------------------------------------
  Make: Canon
  Model: Canon EOS 5D Mark IV
  ...

Exif SubIFD
--------------------------------------------------
  Date/Time Original: 2024:03:15 14:30:22
  F-Number: f/2.8
  Exposure Time: 1/250s
  ISO: 400
  ...

==================================================
KEY INFORMATION
==================================================
Camera Model: Canon EOS 5D Mark IV
Manufacturer: Canon
Date Taken: 2024-03-15 14:30:22
Aperture: f/2.8
Exposure: 1/250s
ISO: 400
Focal Length: 50mm
GPS Location: 40.712800, -74.006000
```

```bash
# JSON output
$ dotnet run --project PhotoExifReader.csproj photo.jpg --json
{
  "camera": {
    "make": "Canon",
    "model": "Canon EOS 5D Mark IV"
  },
  "settings": {
    "dateTaken": "2024:03:15 14:30:22",
    "aperture": "2.8",
    "exposure": "1/250",
    "iso": "400"
  },
  ...
}
```

## Concepts Demonstrated

- External NuGet package integration (MetadataExtractor)
- File I/O and path handling
- EXIF metadata extraction and parsing
- GPS coordinate extraction
- Multiple output formats (human-readable, JSON)
- LINQ for filtering directory types
- Type-safe metadata access with TryGet methods
