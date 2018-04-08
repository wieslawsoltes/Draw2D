Push-Location -Path "src/Core2D.Avalonia"
dotnet restore -v m
dotnet publish -c Release -r win7-x64 -o bin/win7-x64 -v m /p:CoreRT=False
dotnet publish -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64 -v m /p:CoreRT=False
dotnet publish -c Release -r osx.10.12-x64 -o bin/osx.10.12-x64 -v m /p:CoreRT=False
dotnet publish -c Debug -r win-x64 -o bin/win-x64-debug -v m /p:CoreRT=True
dotnet publish -c Release -r win-x64 -o bin/win-x64-release -v m /p:CoreRT=True
Pop-Location
