dotnet restore src/Core2D.Avalonia
if ($LastExitCode -ne 0) { Exit 1 }

dotnet build src/Core2D.Avalonia/Core2D.Avalonia.csproj
if ($LastExitCode -ne 0) { Exit 1 }

dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r win7-x64 -o bin/win7-x64
if ($LastExitCode -ne 0) { Exit 1 }
