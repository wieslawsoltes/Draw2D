[![Build status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/Sources/Draw2D)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=73)

### Build

```
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r win7-x64 -o Draw2D_netcoreapp3.0_win7-x64
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r ubuntu.14.04-x64 -o Draw2D_netcoreapp3.0_ubuntu.14.04-x64
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r debian.8-x64 -o Draw2D_netcoreapp3.0_debian.8-x64
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r osx.10.12-x64 -o Draw2D_netcoreapp3.0_osx.10.12-x64
```

### CoreRT

```
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r win-x64 -o Draw2D_netcoreapp3.0_win-x64
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r linux-x64 -o Draw2D_netcoreapp3.0_linux-x64
dotnet publish ./src/Draw2D/Draw2D.csproj -f netcoreapp3.0 -c Release -r osx-x64 -o Draw2D_netcoreapp3.0_osx-x64
```

### Command-Line

Create view.
```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D/bin/Debug/netcoreapp3.0/Draw2D.dll --new-view
```

Export view.
```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D/bin/Debug/netcoreapp3.0/Draw2D.dll View0.json View0.svg
dotnet /home/ubuntu/environment/src/Draw2D/bin/Debug/netcoreapp3.0/Draw2D.dll View0.json View0.png
dotnet /home/ubuntu/environment/src/Draw2D/bin/Debug/netcoreapp3.0/Draw2D.dll View0.json View0.pdf
```

Create editor.
```
dotnet build
dotnet /home/ubuntu/environment/src/Draw2D/bin/Debug/netcoreapp3.0/Draw2D.dll --new-editor
```

### Linux Fonts

https://www.pcworld.com/article/2863497/how-to-install-microsoft-fonts-in-linux-office-suites.html

```
sudo apt-get install ttf-mscorefonts-installer

cd ~/
mkdir .fonts
fc-cache -f -v

sudo apt-get install cabextract
wget -qO- http://plasmasturm.org/code/vistafonts-installer/vistafonts-installer | bash
```
