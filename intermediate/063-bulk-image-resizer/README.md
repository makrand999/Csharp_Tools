# Bulk Image Resizer

CLI tool for batch resizing images to specified dimensions.

## Usage

```bash
# Resize single image to 800px width (maintains aspect ratio)
dotnet run --project BulkImageResizer/BulkImageResizer.csproj photo.jpg output/ -W=800

# Resize all images in folder to 1920x1080
dotnet run --project BulkImageResizer/BulkImageResizer.csproj images/ resized/ -W=1920 -H=1080

# Recursively process directory
dotnet run --project BulkImageResizer/BulkImageResizer.csproj photos/ output/ -W=1200 -r
```

## Example

```
$ dotnet run --project BulkImageResizer/BulkImageResizer.csproj images/ resized/ -W=800
Processing 5 images in images/...
Resized: photo1.jpg (4032x3024 -> 800x600)
Resized: photo2.png (3840x2160 -> 800x450)
Resized: screenshot.jpg (1920x1080 -> 800x450)
Resized: avatar.png (512x512 -> 800x800)
Resized: banner.jpg (2560x1440 -> 800x450)
Done: 5 succeeded, 0 failed
```

## Supported Formats

- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- BMP (.bmp)
- WebP (.webp)

## Concepts Demonstrated

- External NuGet package usage (SixLabors.ImageSharp)
- File system operations
- Batch processing
- Command-line argument parsing
- Image manipulation and resizing
- Recursive directory traversal
