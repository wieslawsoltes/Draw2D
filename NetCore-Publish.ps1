Push-Location -Path "src/Draw2D.Avalonia"
dotnet restore
dotnet publish -c Release -r win7-x64 -o bin/win7-x64
dotnet publish -c Release -r win7-x86 -o bin/win7-x86
dotnet publish -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64
dotnet publish -c Release -r ubuntu.16.10-x64 -o bin/ubuntu.16.10-x64
dotnet publish -c Release -r osx.10.12-x64 -o bin/osx.10.12-x64
Pop-Location
