@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>&1
if errorlevel 1 (
    echo .NET 10 SDK wurde nicht gefunden.
    echo Installiere das .NET 10 SDK und starte diese Datei erneut.
    pause
    exit /b 1
)

echo Alte Build-Ausgabe wird bereinigt ...
dotnet clean -c Release >nul 2>&1

if exist "bin\Release\net10.0-windows\win-x64\publish" (
    rmdir /s /q "bin\Release\net10.0-windows\win-x64\publish"
)

echo.
echo ProcessSet Manager wird als einzelne EXE gebaut ...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false
if errorlevel 1 goto :error

echo.
echo Fertig.
echo Die EXE liegt unter:
echo bin\Release\net10.0-windows\win-x64\publish\ProcessSetManager.exe
echo.
echo Version 0.6 enthaelt Deutsch/Englisch, Hilfe, Vorschau, Tray, Modi und Schutzliste.
pause
exit /b 0

:error
echo.
echo Publish fehlgeschlagen.
pause
exit /b 1
