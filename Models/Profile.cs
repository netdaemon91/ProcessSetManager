namespace ProcessSetManager.Models;

public sealed class ProcessProfile
{
    public int FormatVersion { get; set; } = 2;
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? LastExecutedAt { get; set; }
    public List<ProcessTarget> Processes { get; set; } = new();
}

public sealed class ProcessTarget
{
    public string Name { get; set; } = string.Empty;
    public string? Path { get; set; }
}

public sealed class SessionSnapshot
{
    public DateTime Created { get; set; } = DateTime.Now;
    public string? SourceProfile { get; set; }
    public List<RestartTarget> Processes { get; set; } = new();
}

public sealed class RestartTarget
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

public sealed class AppSettings
{
    public int FormatVersion { get; set; } = 1;
    public bool MinimizeToTray { get; set; } = true;
    public List<string> ProtectedProcesses { get; set; } = new();
}

public sealed class ModeState
{
    public bool IsActive { get; set; }
    public string? ProfileName { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public int RestorableCount { get; set; }
}


public sealed class ModePreview
{
    public List<ModePreviewItem> Items { get; set; } = new();

    public int RunningCount => Items.Count(i => i.IsRunning && !i.IsProtected);
    public int RestorableCount => Items.Count(i => i.IsRestorable && !i.IsProtected);
    public int ProtectedCount => Items.Count(i => i.IsProtected);
    public int NotRunningCount => Items.Count(i => !i.IsRunning && !i.IsProtected);
    public int NonRestorableRunningCount =>
        Items.Count(i => i.IsRunning && !i.IsProtected && !i.IsRestorable);
}

public sealed class ModePreviewItem
{
    public string Name { get; set; } = string.Empty;
    public bool IsProtected { get; set; }
    public bool IsRunning { get; set; }
    public int RunningInstances { get; set; }
    public bool IsRestorable { get; set; }
    public string? RestartPath { get; set; }
}

public sealed class StopOperationResult
{
    public int StoppedCount { get; set; }
    public int NotRunningCount { get; set; }
    public int ProtectedCount { get; set; }
    public int ErrorCount { get; set; }
    public int ForcedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public sealed class RestoreOperationResult
{
    public int StartedCount { get; set; }
    public int AlreadyRunningCount { get; set; }
    public int MissingFileCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Details { get; set; } = new();
}
