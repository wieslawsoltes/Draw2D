@echo off
setlocal

set msvcp140_x64=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x64\Microsoft.VC141.CRT\msvcp140.dll
set vcruntime140_x64=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x64\Microsoft.VC141.CRT\vcruntime140.dll
set editbin=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.14.26428\bin\HostX86\x86\editbin.exe
set win7_x64=src\Core2D.Avalonia\bin\win7-x64
set win_x64_debug=src\Core2D.Avalonia\bin\win-x64-debug
set win_x64_release=src\Core2D.Avalonia\bin\win-x64-release
set libSkiaSharp=%UserProfile%\.nuget\packages\skiasharp\1.57.1\runtimes\win7-x64\native\libSkiaSharp.dll
set executable=Core2D.Avalonia.exe

copy /Y "%msvcp140_x64%" "%win7_x64%"
copy /Y "%vcruntime140_x64%" "%win7_x64%"
rem "%editbin%" /subsystem:windows "%win7_x64%\%executable%"

copy /Y "%msvcp140_x64%" "%win_x64_debug%"
copy /Y "%vcruntime140_x64%" "%win_x64_debug%"
copy /Y "%libSkiaSharp%" "%win_x64_debug%"
rem "%editbin%" /subsystem:windows "%win_x64_debug%\%executable%"

copy /Y "%msvcp140_x64%" "%win_x64_release%"
copy /Y "%vcruntime140_x64%" "%win_x64_release%"
copy /Y "%libSkiaSharp%" "%win_x64_release%"
rem "%editbin%" /subsystem:windows "%win_x64_release%\%executable%"

endlocal
