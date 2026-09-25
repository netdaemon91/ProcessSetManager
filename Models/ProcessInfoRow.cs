namespace ProcessSetManager.Models;

public sealed class ProcessInfoRow
{
    public string Name { get; init; } = string.Empty;
    public int Id { get; init; }
    public string Kind { get; init; } = string.Empty;
    public long MemoryBytes { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public string MemoryText =>
        MemoryBytes <= 0 ? string.Empty : $"{MemoryBytes / 1024d / 1024d:N0} MB";
}
