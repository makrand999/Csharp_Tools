# MIME Type Detector

Detect MIME types by file extension and magic bytes.

## Usage

```bash
dotnet run --project 144-mime-type-detector.csproj -- <file>
```

## Example

```bash
dotnet run --project 144-mime-type-detector.csproj -- image.png
dotnet run --project 144-mime-type-detector.csproj -- document.pdf
```

### Sample Output

```
File: image.png
Extension: .png
MIME by extension: image/png
MIME by content: image/png
```

## Supported Types

- Images: JPEG, PNG, GIF, BMP, WebP, SVG, ICO, TIFF
- Audio: MP3, WAV, OGG, FLAC, M4A, AAC
- Video: MP4, AVI, MKV, WebM, MOV, WMV
- Documents: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX
- Archives: ZIP, TAR, GZ, RAR, 7Z
- Web: HTML, CSS, JS, JSON, XML, CSV
- Fonts: TTF, OTF, WOFF, WOFF2, EOT

## Concepts Demonstrated

- Magic byte detection
- Extension-based lookup
- File header analysis
- MIME type mapping
