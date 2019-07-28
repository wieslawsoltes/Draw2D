# Svg.Skia

Skia SVG rendering library.

## Usage

### Tool

```
dotnet tool install -g Svg.Skia.Converter
```

```
Svg.Skia.Converter:
  Converts a svg file to an encoded image.

Usage:
  Svg.Skia.Converter [options]

Options:
  -f, --file <file>              The relative or absolute path to the input file
  -d, --directory <directory>    The relative or absolute path to the input directory
  -o, --output <output>          The relative or absolute path to the output directory
  -p, --pattern <pattern>        The search string to match against the names of files in the input path
  --format <format>              The output image format
  --quality <quality>            The output image quality
  --scaleX <scalex>              The output image horizontal scaling factor
  --scaleY <scaley>              The output image vertical scaling factor
  --debug                        The flag indicating whether to produce debug output to a file
  --version                      Display version information
```

### Library

```
dotnet add package Svg.Skia
```

```C#
using SkiaSharp;
using Svg.Skia;
```
```C#
var path = "image.svg";
var svg = new Svg();
var picture = svg.Load(path);
if (picture != null)
{
    svg.Save("image.png", SKEncodedImageFormat.Png, 100, 1f, 1f);
}
```

## Externals

The `Svg.Skia` library is using code from the https://github.com/vvvv/SVG

## License

Svg.Skia is licensed under the [MIT license](LICENSE.TXT).
