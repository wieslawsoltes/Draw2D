# Draw2D

[![Build status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/Sources/Draw2D)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=73)

A multi-platform 2D diagram editor.

## About

Draw2D is a multi-platform application for making 2D diagrams. 

## Build

To build `Draw2D` install [.NET Core 3.0 preview 6](https://dotnet.microsoft.com/download/dotnet-core/3.0).

```
git clone https://github.com/wieslawsoltes/Draw2D.git
cd Draw2D
dotnet build
```

You can also build for single target framework.

```
dotnet build -f netcoreapp3.0 -c Release
```

```
dotnet build -f net461 -c Release
```

### Publish Managed

```
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r win7-x64 -o Draw2D.Desktop_netcoreapp3.0_win7-x64
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r ubuntu.14.04-x64 -o Draw2D.Desktop_netcoreapp3.0_ubuntu.14.04-x64
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r debian.8-x64 -o Draw2D.Desktop_netcoreapp3.0_debian.8-x64
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r osx.10.12-x64 -o Draw2D.Desktop_netcoreapp3.0_osx.10.12-x64
```

### Publish Native

```
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r win-x64 -o Draw2D.Desktop_netcoreapp3.0_win-x64
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r linux-x64 -o Draw2D.Desktop_netcoreapp3.0_linux-x64
dotnet publish ./src/Draw2D.Desktop/Draw2D.Desktop.csproj -f netcoreapp3.0 -c Release -r osx-x64 -o Draw2D.Desktop_netcoreapp3.0_osx-x64
```

## Command-Line Usage

### Create Styles Library

```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --new-styles
```

### Create Groups Library

```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --new-groups
```

### Create View
```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --new-view
```

### Create Editor
```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --new-editor
```

### Export View

```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --demo
dotnet /home/ubuntu/environment/src/Draw2D.Desktop/bin/Debug/netcoreapp3.0/Draw2D.Desktop.dll --export demo.json demo.png
```

## Linux Fonts

https://www.pcworld.com/article/2863497/how-to-install-microsoft-fonts-in-linux-office-suites.html

```
sudo apt-get install ttf-mscorefonts-installer

cd ~/
mkdir .fonts
fc-cache -f -v

sudo apt-get install cabextract
wget -qO- http://plasmasturm.org/code/vistafonts-installer/vistafonts-installer | bash
```

## License

Draw2D is licensed under the [MIT license](LICENSE.TXT).
