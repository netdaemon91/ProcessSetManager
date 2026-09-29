using System.Diagnostics;
using ProcessSetManager.Models;

namespace ProcessSetManager.Services;

public sealed class ProcessService
{
    private static readonly HashSet<string> BuiltInProtectedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Idle",
        "System",
        "Registry",
        "Memory Compression",
        "Secure System",
        "smss",
        "csrss",
        "wininit",
        "services",
        "lsass",
        "winlogon",
        "fontdrvhost"
    };

    public static IReadOnlyCollection<string> GetBuiltInProtectedNames() =>
        BuiltInProtectedNames.OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToArray();

    public static bool IsProtected(string name, IEnumerable<string>? userProtected = null)
    {
        if (BuiltInProtectedNames.Contains(name))
            return true;

        return userProtected?.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase)) == true;
    }

    public IReadOnlyList<ProcessInfoRow> GetProcesses()
    {
        var rows = new List<ProcessInfoRow>();

        foreach (var process in Process.GetProcesses()
                     .OrderBy(p => Safe(() => p.ProcessName))
                     .ThenBy(p => Safe(() => p.Id)))
        {
            using (process)
            {
                try
                {
                    if (process.Id == Environment.ProcessId)
                        continue;

                    var name = Safe(() => process.ProcessName) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(name) || BuiltInProtectedNames.Contains(name))
                        continue;

                    var title = Safe(() => process.MainWindowTitle) ?? string.Empty;
                    var path = Safe(() => process.MainModule?.FileName) ?? string.Empty;
                    var hasWindow = Safe(() => process.MainWindowHandle) != IntPtr.Zero;
                    var memory = Safe(() => process.WorkingSet64);

                    rows.Add(new ProcessInfoRow
                    {
                        Name = name,
                        Id = process.Id,
                        Kind = hasWindow ? "App" : "Hintergrund",
                        MemoryBytes = memory,
                        Title = title,
                        Path = path
                    });
                }
                catch
                {
                    // Ein unlesbarer Prozess darf die Gesamtliste nicht abbrechen.
                }
            }
        }

        return rows;
    }

    public ModePreview CreateModePreview(
        IEnumerable<string> processNames,
        IEnumerable<string>? userProtected = null)
    {
        var result = new ModePreview();

        foreach (var rawName in processNames
                     .Where(n => !string.IsNullOrWhiteSpace(n))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var name = ProfileStore.NormalizeProcessName(rawName);
            var item = new ModePreviewItem
            {
                Name = name,
                IsProtected = IsProtected(name, userProtected)
            };

            var processes = Process.GetProcessesByName(name);
            try
            {
                item.RunningInstances = processes.Length;
                item.IsRunning = processes.Length > 0;

                if (!item.IsProtected && item.IsRunning)
                {
                    foreach (var process in processes)
                    {
                        try
                        {
                            var path = process.MainModule?.FileName;
                            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                            {
                                item.RestartPath = path;
                                item.IsRestorable = true;
                                break;
                            }
                        }
                        catch
                        {
                            // Manche Prozesse geben den Startpfad nicht frei.
                        }
                    }
                }
            }
            finally
            {
                foreach (var process in processes)
                    process.Dispose();
            }

            result.Items.Add(item);
        }

        return result;
    }

    public SessionSnapshot CreateSnapshot(
        IEnumerable<string> processNames,
        string? sourceProfile,
        IEnumerable<string>? userProtected = null)
    {
        var preview = CreateModePreview(processNames, userProtected);

        return new SessionSnapshot
        {
            Created = DateTime.Now,
            SourceProfile = sourceProfile,
            Processes = preview.Items
                .Where(i => i.IsRunning && i.IsRestorable && !i.IsProtected && !string.IsNullOrWhiteSpace(i.RestartPath))
                .Select(i => new RestartTarget
                {
                    Name = i.Name,
                    Path = i.RestartPath!
                })
                .ToList()
        };
    }

    public async Task<StopOperationResult> StopProcessesAsync(
        IEnumerable<string> names,
        IEnumerable<string>? userProtected = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new StopOperationResult();

        foreach (var rawName in names
                     .Where(n => !string.IsNullOrWhiteSpace(n))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var name = ProfileStore.NormalizeProcessName(rawName);

            if (IsProtected(name, userProtected))
            {
                result.ProtectedCount++;
                progress?.Report(Localization.F("{0}: geschützt, übersprungen", name));
                continue;
            }

            var matches = Process.GetProcessesByName(name);

            if (matches.Length == 0)
            {
                result.NotRunningCount++;
                progress?.Report(Localization.F("{0}: läuft nicht", name));
                continue;
            }

            var nameHadError = false;
            var nameForced = false;

            foreach (var process in matches)
            {
                using (process)
                {
                    if (process.Id == Environment.ProcessId)
                        continue;

                    try
                    {
                        progress?.Report(Localization.F("Beende {0} (PID {1}) …", name, process.Id));

                        var hasWindow = false;
                        try { hasWindow = process.MainWindowHandle != IntPtr.Zero; } catch { }

                        if (hasWindow)
                        {
                            try { process.CloseMainWindow(); } catch { }

                            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                            timeout.CancelAfter(TimeSpan.FromSeconds(3));

                            try
                            {
                                await process.WaitForExitAsync(timeout.Token);
                            }
                            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                            {
                                // Timeout: danach ggf. erzwingen.
                            }
                        }

                        bool stillRunning;
                        try { stillRunning = !process.HasExited; }
                        catch { stillRunning = false; }

                        if (stillRunning)
                        {
                            nameForced = true;
                            process.Kill(entireProcessTree: true);
                            await process.WaitForExitAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        nameHadError = true;
                        result.Errors.Add(Localization.F("{0} (PID {1}): Fehler – {2}", name, process.Id, ex.Message));
                        progress?.Report(Localization.F("{0}: Fehler – {1}", name, ex.Message));
                    }
                }
            }

            var remaining = Process.GetProcessesByName(name);
            try
            {
                if (remaining.Length == 0 && !nameHadError)
                {
                    result.StoppedCount++;
                    if (nameForced)
                        result.ForcedCount++;
                }
                else
                {
                    result.ErrorCount++;
                    if (!nameHadError)
                        result.Errors.Add(Localization.F("{0}: Prozess läuft weiterhin.", name));
                }
            }
            finally
            {
                foreach (var process in remaining)
                    process.Dispose();
            }
        }

        return result;
    }

    public RestoreOperationResult RestoreSnapshot(
        SessionSnapshot snapshot,
        IProgress<string>? progress = null)
    {
        var result = new RestoreOperationResult();

        foreach (var target in snapshot.Processes)
        {
            try
            {
                var existing = Process.GetProcessesByName(target.Name);
                var alreadyRunning = existing.Length > 0;
                foreach (var process in existing)
                    process.Dispose();

                if (alreadyRunning)
                {
                    result.AlreadyRunningCount++;
                    result.Details.Add(Localization.F("{0}: läuft bereits", target.Name));
                    continue;
                }

                if (!File.Exists(target.Path))
                {
                    result.MissingFileCount++;
                    result.Details.Add(Localization.F("{0}: Datei nicht gefunden", target.Name));
                    continue;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = target.Path,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(target.Path) ?? Environment.CurrentDirectory
                });

                result.StartedCount++;
                result.Details.Add(Localization.F("{0}: gestartet", target.Name));
                progress?.Report(Localization.F("{0}: gestartet", target.Name));
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                result.Details.Add(Localization.F("{0}: Fehler – {1}", target.Name, ex.Message));
            }
        }

        return result;
    }

    private static T? Safe<T>(Func<T> getter)
    {
        try { return getter(); }
        catch { return default; }
    }
}
