dotnet restore src/Core2D.Avalonia -v m
dotnet build src/Core2D.Avalonia/Core2D.Avalonia.csproj -v m
#dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r win7-x64 -o bin/win7-x64s /p:CoreRT=False
#dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj-c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64 -v m /p:CoreRT=False
#dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj-c Release -r osx.10.12-x64 -o bin/osx.10.12-x64 -v m /p:CoreRT=False
dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj-c Debug -r win-x64 -o bin/win-x64-debug -v m /p:CoreRT=True
dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj-c Release -r win-x64 -o bin/win-x64-release -v m /p:CoreRT=True
