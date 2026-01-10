# Color Code Converter

A practical CLI tool for converting color codes between HEX, RGB, and HSL formats. Useful for web developers and designers working with different color representations.

## Usage

```bash
dotnet run --project ColorConverter.csproj "<color>"
```

### Supported Input Formats

| Format | Example | Description |
|--------|---------|-------------|
| HEX (6-digit) | `#ff5733` | Standard web color |
| HEX (3-digit) | `#f53` | Shorthand for `#ff5533` |
| RGB | `rgb(255,87,51)` | CSS RGB format |
| RGB (simple) | `255,87,51` | Comma-separated values |
| HSL | `hsl(11,100%,60%)` | CSS HSL format |

## Examples

```bash
# Convert from HEX
dotnet run --project ColorConverter.csproj "#ff5733"
# Output:
# HEX: #FF5733
# RGB: rgb(255,87,51)
# HSL: hsl(11,100%,60%)

# Convert from RGB
dotnet run --project ColorConverter.csproj "rgb(255,87,51)"
# Output:
# HEX: #FF5733
# RGB: rgb(255,87,51)
# HSL: hsl(11,100%,60%)

# Convert from HSL
dotnet run --project ColorConverter.csproj "hsl(11,100%,60%)"
# Output:
# HEX: #FF5733
# RGB: rgb(255,87,51)
# HSL: hsl(11,100%,60%)

# Convert shorthand HEX
dotnet run --project ColorConverter.csproj "#f53"
# Output:
# HEX: #FF5533
# RGB: rgb(255,85,51)
# HSL: hsl(6,100%,60%)
```

## Use Cases

- **Web Development**: Convert between CSS color formats
- **Design Tools**: Translate colors from design specs to code
- **Theme Generation**: Convert colors for different format requirements
- **Accessibility**: Calculate color variations and contrasts
- **Cross-Platform**: Match colors across different systems

## Common Colors Reference

| Color Name | HEX | RGB | HSL |
|------------|-----|-----|-----|
| Red | #FF0000 | rgb(255,0,0) | hsl(0,100%,50%) |
| Green | #00FF00 | rgb(0,255,0) | hsl(120,100%,50%) |
| Blue | #0000FF | rgb(0,0,255) | hsl(240,100%,50%) |
| Yellow | #FFFF00 | rgb(255,255,0) | hsl(60,100%,50%) |
| Cyan | #00FFFF | rgb(0,255,255) | hsl(180,100%,50%) |
| Magenta | #FF00FF | rgb(255,0,255) | hsl(300,100%,50%) |
| White | #FFFFFF | rgb(255,255,255) | hsl(0,0%,100%) |
| Black | #000000 | rgb(0,0,0) | hsl(0,0%,0%) |
| Gray | #808080 | rgb(128,128,128) | hsl(0,0%,50%) |

## Color Format Guide

### HEX (#RRGGBB)
- Used in HTML/CSS
- 6 hexadecimal digits (00-FF per channel)
- Shorthand #RGB expands each digit

### RGB (Red, Green, Blue)
- Additive color model for screens
- Values 0-255 per channel
- Used in CSS, graphics programming

### HSL (Hue, Saturation, Lightness)
- More intuitive for humans
- Hue: 0-360° (color wheel position)
- Saturation: 0-100% (color intensity)
- Lightness: 0-100% (brightness)

## Concepts Demonstrated

- Color space conversion algorithms
- Record types (C# 9+)
- Pattern matching with switch expressions
- String parsing and formatting
- Mathematical color calculations
- Tuple deconstruction
