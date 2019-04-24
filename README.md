[![Build status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/Sources/Draw2D)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=73)

### Build

```
dotnet publish .\src\Draw2D\Draw2D.csproj -f netcoreapp3.0 -c Release -r win7-x64 -o Draw2D_netcoreapp3.0_win7-x64
```

### CoreRT

```
dotnet publish .\src\Draw2D\Draw2D.csproj -f netcoreapp3.0 -c Release -r win-x64 -o Draw2D_netcoreapp3.0_win-x64
dotnet publish .\src\Draw2D\Draw2D.csproj -f netcoreapp3.0 -c Release -r linux-x64 -o Draw2D_netcoreapp3.0_linux-x64
dotnet publish .\src\Draw2D\Draw2D.csproj -f netcoreapp3.0 -c Release -r osx-x64 -o Draw2D_netcoreapp3.0_osx-x64
```
