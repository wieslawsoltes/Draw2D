# Svg.Skia

Skia SVG rendering library.

## Usage

### Library

```
dotnet add package Svg.Skia
```

```C#
using SkiaSharp;
using Svg.Skia;
```

```C#
var path = "image.svg"; // You can also load `image.svgz` images.
var svg = new Svg();
var picture = svg.Load(path);
if (picture != null)
{
    svg.Save("image.png", SKEncodedImageFormat.Png, 100, 1f, 1f);
}
```

### Tool

```
dotnet tool install -g Svg.Skia.Converter
```

```
Svg.Skia.Converter:
  Converts a svg file to an encoded bitmap image.

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
  --debug                        Write debug output to a file
  --quiet                        Set verbosity level to quiet
  --version                      Display version information
```

## Build

### Library

```
git clone git@github.com:wieslawsoltes/Draw2D.git
cd Draw2D
git submodule update --init --recursive
dotnet build -c Release ./svg/Svg.Skia.Converter/Svg.Skia.Converter.csproj
```

### Tool

```
git clone git@github.com:wieslawsoltes/Draw2D.git
cd Draw2D
git submodule update --init --recursive
export LANG=en-US.UTF-8
dotnet build -c Release ./svg/Svg.Skia/Svg.Skia.csproj
```

## Publish

### Library

```
git clone git@github.com:wieslawsoltes/Draw2D.git
cd Draw2D
git submodule update --init --recursive
export LANG=en-US.UTF-8
dotnet pack -c Release -o ./artifacts --version-suffix "preview1" ./svg/Svg.Skia/Svg.Skia.csproj
```

```
dotnet nuget push ./artifacts/Svg.Skia.0.0.1-preview1.nupkg -k <key> -s https://api.nuget.org/v3/index.json
```

### Tool

```
git clone git@github.com:wieslawsoltes/Draw2D.git
cd Draw2D
git submodule update --init --recursive
export LANG=en-US.UTF-8
dotnet pack -c Release -o ./artifacts --version-suffix "preview1" ./svg/Svg.Skia.Converter/Svg.Skia.Converter.csproj
```

```
dotnet nuget push ./artifacts/Svg.Skia.Converter.0.0.1-preview1.nupkg -k <key> -s https://api.nuget.org/v3/index.json
```

## Testing

```
dotnet tool install --global --add-source ./artifacts Svg.Skia.Converter
export DOTNET_ROOT=$HOME/dotnet
```

```
dotnet tool uninstall -g Svg.Skia.Converter
Svg.Skia.Converter --help
```

## Externals

The `Svg.Skia` library is using code from the https://github.com/vvvv/SVG

## License

Svg.Skia is licensed under the [MIT license](LICENSE.TXT).
