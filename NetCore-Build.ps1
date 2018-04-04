Push-Location -Path "src/Core2D.Avalonia"
dotnet restore -v m
dotnet build -v m
Pop-Location
