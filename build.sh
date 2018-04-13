#!/usr/bin/env bash

dotnet restore src/Core2D.Avalonia -v m
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet build src/Core2D.Avalonia/Core2D.Avalonia.csproj -v m
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r win7-x64 -o bin/win7-x64 -v m /p:CoreRT=False
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64 -v m /p:CoreRT=False
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r osx.10.12-x64 -o bin/osx.10.12-x64 -v m /p:CoreRT=False
if [ $? -ne 0 ]; then
    exit 1
fi
