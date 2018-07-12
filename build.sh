#!/usr/bin/env bash

dotnet restore src/Core2D.Avalonia
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet build src/Core2D.Avalonia/Core2D.Avalonia.csproj
if [ $? -ne 0 ]; then
    exit 1
fi

dotnet publish src/Core2D.Avalonia/Core2D.Avalonia.csproj -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64
if [ $? -ne 0 ]; then
    exit 1
fi
