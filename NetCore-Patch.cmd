@echo off
setlocal
set msvcp140_x86=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x86\Microsoft.VC150.CRT\msvcp140.dll
set vcruntime140_x86=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x86\Microsoft.VC150.CRT\vcruntime140.dll
set msvcp140_x64=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x64\Microsoft.VC150.CRT\msvcp140.dll
set vcruntime140_x64=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x64\Microsoft.VC150.CRT\vcruntime140.dll
set editbin=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.10.25017\bin\HostX86\x86\editbin.exe
set win7_x86=src\Core2D.Avalonia\bin\win7-x86
set win7_x64=src\Core2D.Avalonia\bin\win7-x64
set executable=Core2D.Avalonia.exe
copy /Y "%msvcp140_x86%" "%win7_x86%"
copy /Y "%vcruntime140_x86%" "%win7_x86%"
copy /Y "%msvcp140_x64%" "%win7_x64%"
copy /Y "%vcruntime140_x64%" "%win7_x64%"
"%editbin%" /subsystem:windows "%win7_x86%\%executable%"
"%editbin%" /subsystem:windows "%win7_x64%\%executable%"
endlocal
