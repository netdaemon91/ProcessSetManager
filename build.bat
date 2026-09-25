@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>&1
if errorlevel 1 (
    echo .NET 10 SDK wurde nicht gefunden.
    echo Installiere das .NET 10 SDK und starte build.bat erneut.
    pause
    exit /b 1
)

dotnet restore
if errorlevel 1 goto :error

dotnet build -c Release
if errorlevel 1 goto :error

echo.
echo Build erfolgreich.
echo Ausgabe: bin\Release\net10.0-windows\win-x64\
pause
exit /b 0

:error
echo.
echo Build fehlgeschlagen.
pause
exit /b 1
