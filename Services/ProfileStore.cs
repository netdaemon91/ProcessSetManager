using System.Text.Json;
using ProcessSetManager.Models;

namespace ProcessSetManager.Services;

public sealed class ProfileStore
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public string BaseDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProcessSetManager");

    public string ProfileDirectory => Path.Combine(BaseDirectory, "Profiles");
    public string LastSessionPath => Path.Combine(BaseDirectory, "last-session.json");
    public string SettingsPath => Path.Combine(BaseDirectory, "settings.json");
    public string ModeStatePath => Path.Combine(BaseDirectory, "mode-state.json");

    public ProfileStore()
    {
        Directory.CreateDirectory(ProfileDirectory);
    }

    public IReadOnlyList<string> GetProfileNames()
    {
        Directory.CreateDirectory(ProfileDirectory);

        return Directory
            .EnumerateFiles(ProfileDirectory, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase)
            .Cast<string>()
            .ToList();
    }

    public void SaveProfile(ProcessProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.Name))
            throw new InvalidOperationException(Localization.T("Das Profil benötigt einen Namen."));

        Directory.CreateDirectory(ProfileDirectory);
        File.WriteAllText(GetProfilePath(profile.Name), JsonSerializer.Serialize(profile, _jsonOptions));
    }

    public ProcessProfile LoadProfile(string name)
    {
        var path = GetProfilePath(name);
        if (!File.Exists(path))
            throw new FileNotFoundException(Localization.T("Das Profil wurde nicht gefunden."), path);

        return JsonSerializer.Deserialize<ProcessProfile>(File.ReadAllText(path), _jsonOptions)
               ?? throw new InvalidDataException(Localization.T("Das Profil konnte nicht gelesen werden."));
    }

    public void DeleteProfile(string name)
    {
        var path = GetProfilePath(name);
        if (File.Exists(path))
            File.Delete(path);
    }

    public void RenameProfile(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new InvalidOperationException(Localization.T("Der neue Profilname darf nicht leer sein."));

        var oldPath = GetProfilePath(oldName);
        var newPath = GetProfilePath(newName);

        if (!File.Exists(oldPath))
            throw new FileNotFoundException(Localization.T("Das Profil wurde nicht gefunden."), oldPath);

        if (!oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase) && File.Exists(newPath))
            throw new IOException(Localization.T("Ein Profil mit diesem Namen existiert bereits."));

        var profile = LoadProfile(oldName);
        profile.Name = newName.Trim();

        File.WriteAllText(newPath, JsonSerializer.Serialize(profile, _jsonOptions));

        if (!oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
            File.Delete(oldPath);
    }

    public void DuplicateProfile(string sourceName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new InvalidOperationException(Localization.T("Der neue Profilname darf nicht leer sein."));

        var newPath = GetProfilePath(newName);
        if (File.Exists(newPath))
            throw new IOException(Localization.T("Ein Profil mit diesem Namen existiert bereits."));

        var source = LoadProfile(sourceName);
        var copy = new ProcessProfile
        {
            FormatVersion = source.FormatVersion,
            Name = newName.Trim(),
            Created = DateTime.Now,
            LastExecutedAt = null,
            Processes = source.Processes
                .Select(p => new ProcessTarget { Name = p.Name, Path = p.Path })
                .ToList()
        };

        SaveProfile(copy);
    }

    public void SaveLastSession(SessionSnapshot snapshot)
    {
        Directory.CreateDirectory(BaseDirectory);
        File.WriteAllText(LastSessionPath, JsonSerializer.Serialize(snapshot, _jsonOptions));
    }

    public SessionSnapshot? LoadLastSession()
    {
        if (!File.Exists(LastSessionPath))
            return null;

        return JsonSerializer.Deserialize<SessionSnapshot>(
            File.ReadAllText(LastSessionPath), _jsonOptions);
    }

    public AppSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return new AppSettings();

            var settings = JsonSerializer.Deserialize<AppSettings>(
                               File.ReadAllText(SettingsPath), _jsonOptions)
                           ?? new AppSettings();

            settings.Language = Localization.NormalizeLanguage(settings.Language);
            settings.ProtectedProcesses ??= new List<string>();
            settings.ProtectedProcesses = settings.ProtectedProcesses
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(NormalizeProcessName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        Directory.CreateDirectory(BaseDirectory);
        settings.Language = Localization.NormalizeLanguage(settings.Language);
        settings.ProtectedProcesses = settings.ProtectedProcesses
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(NormalizeProcessName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, _jsonOptions));
    }

    public ModeState LoadModeState()
    {
        try
        {
            if (!File.Exists(ModeStatePath))
                return new ModeState();

            return JsonSerializer.Deserialize<ModeState>(
                       File.ReadAllText(ModeStatePath), _jsonOptions)
                   ?? new ModeState();
        }
        catch
        {
            return new ModeState();
        }
    }

    public void SaveModeState(ModeState state)
    {
        Directory.CreateDirectory(BaseDirectory);
        File.WriteAllText(ModeStatePath, JsonSerializer.Serialize(state, _jsonOptions));
    }

    public void ClearModeState()
    {
        if (File.Exists(ModeStatePath))
            File.Delete(ModeStatePath);
    }

    public static string NormalizeProcessName(string name)
    {
        var value = name.Trim();
        if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            value = value[..^4];

        return value;
    }

    private string GetProfilePath(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(name.Trim().Select(c => invalid.Contains(c) ? '_' : c).ToArray());

        if (string.IsNullOrWhiteSpace(safe))
            throw new InvalidOperationException(Localization.T("Ungültiger Profilname."));

        return Path.Combine(ProfileDirectory, safe + ".json");
    }
}
