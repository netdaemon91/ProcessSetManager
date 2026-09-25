using System.Text;

namespace ProcessSetManager.Services;

public static class ExportService
{
    public static void ExportTxt(string fileName, IEnumerable<string> names)
    {
        File.WriteAllLines(fileName, names, new UTF8Encoding(true));
    }

    public static void ExportPowerShell(string fileName, IEnumerable<string> names)
    {
        var escaped = names.Select(n => "'" + n.Replace("'", "''") + "'");
        var array = string.Join("," + Environment.NewLine + "    ", escaped);

        var script = $$"""
        # Erzeugt von ProcessSet Manager
        # Reihenfolge: von oben nach unten

        $processNames = @(
            {{array}}
        )

        foreach ($name in $processNames) {
            Write-Host "Beende $name ..." -ForegroundColor Cyan

            $processes = @(Get-Process -Name $name -ErrorAction SilentlyContinue)
            foreach ($p in $processes) {
                try {
                    if ($p.MainWindowHandle -ne 0) {
                        [void]$p.CloseMainWindow()
                        $deadline = [DateTime]::UtcNow.AddSeconds(3)

                        while ([DateTime]::UtcNow -lt $deadline) {
                            if (-not (Get-Process -Id $p.Id -ErrorAction SilentlyContinue)) {
                                break
                            }
                            Start-Sleep -Milliseconds 150
                        }
                    }

                    if (Get-Process -Id $p.Id -ErrorAction SilentlyContinue) {
                        Stop-Process -Id $p.Id -Force -ErrorAction Stop
                    }
                }
                catch {
                    Write-Warning "Konnte $name (PID $($p.Id)) nicht beenden: $($_.Exception.Message)"
                }
            }
        }

        Write-Host "Fertig." -ForegroundColor Green
        """;

        File.WriteAllText(fileName, script, new UTF8Encoding(true));
    }

    public static void ExportBatch(string fileName, IEnumerable<string> names)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@echo off");
        sb.AppendLine("setlocal");
        sb.AppendLine("title ProcessSet Manager");
        sb.AppendLine();

        foreach (var name in names)
        {
            var exe = name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? name
                : name + ".exe";

            var safeEcho = exe
                .Replace("&", "")
                .Replace("|", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("^", "");

            sb.AppendLine($"echo Beende {safeEcho} ...");
            sb.AppendLine($"taskkill /IM \"{exe}\" /T >nul 2>&1");
            sb.AppendLine("timeout /t 2 /nobreak >nul");
            sb.AppendLine($"tasklist /FI \"IMAGENAME eq {exe}\" 2>nul | find /I \"{exe}\" >nul && taskkill /F /IM \"{exe}\" /T >nul 2>&1");
            sb.AppendLine();
        }

        sb.AppendLine("echo Fertig.");
        sb.AppendLine("timeout /t 2 /nobreak >nul");
        sb.AppendLine("endlocal");

        File.WriteAllText(fileName, sb.ToString(), Encoding.Default);
    }
}
