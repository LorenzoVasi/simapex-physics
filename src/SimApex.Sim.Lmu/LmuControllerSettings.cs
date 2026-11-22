using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SimApex.Sim.Lmu;

/// <summary>A change written to an LMU settings file.</summary>
/// <param name="File">File changed.</param>
/// <param name="Backup">Copy of the file before the change.</param>
/// <param name="Setting">Setting name.</param>
/// <param name="OldValue">Value before.</param>
/// <param name="NewValue">Value after.</param>
/// <param name="When">Local time of the change.</param>
public sealed record SettingChange(string File, string Backup, string Setting, double OldValue, double NewValue, DateTime When);

/// <summary>
/// Reads and changes the wheel's force feedback strength in LMU's <c>UserData\player\direct input.json</c>.
/// Safety rules: the game must be closed (it rewrites the file on exit), the file is backed up before any
/// change, only the one value is replaced, the result is re-parsed to verify it, and the strength can be
/// raised only when the caller explicitly allows it.
/// </summary>
public sealed class LmuControllerSettings
{
    public const string StrengthKey = "Steering effects strength";
    public const double MaxStrength = 10000; // 100% in LMU's scale

    public LmuControllerSettings(string installRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(installRoot);
        FilePath = Path.Combine(installRoot, "UserData", "player", "direct input.json");
    }

    public string FilePath { get; }

    /// <summary>LMU install root from the SDK header folder (&lt;root&gt;\Support\SharedMemoryInterface).</summary>
    public static string InstallRootFromSdk(string sdkHeaderDirectory) =>
        Path.GetFullPath(Path.Combine(sdkHeaderDirectory, "..", ".."));

    public static bool IsGameRunning() =>
        Process.GetProcesses().Any(p => p.ProcessName.Contains("Le Mans Ultimate", StringComparison.OrdinalIgnoreCase));

    /// <summary>Wheel device entries (name, FFB strength) found in the file.</summary>
    public IReadOnlyList<(string Device, double Strength)> ReadStrengths()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(FilePath));
        var list = new List<(string, double)>();
        foreach (var dev in doc.RootElement.GetProperty("Devices").EnumerateObject())
        {
            if (dev.Value.TryGetProperty("Force Feedback", out var ffb) && ffb.TryGetProperty(StrengthKey, out var s)
                && dev.Value.TryGetProperty("Type", out var type) && type.GetString() == "Wheel")
            {
                list.Add((dev.Name, s.GetDouble()));
            }
        }

        return list;
    }

    /// <summary>Sets the FFB strength of the device whose name starts with <paramref name="devicePrefix"/>.</summary>
    public SettingChange SetStrength(string devicePrefix, double newValue, bool allowIncrease = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(devicePrefix);
        if (newValue is < 0 or > MaxStrength)
        {
            throw new ArgumentOutOfRangeException(nameof(newValue), $"Strength must be between 0 and {MaxStrength}.");
        }

        if (IsGameRunning())
        {
            throw new InvalidOperationException("Le Mans Ultimate is running: close it first, otherwise it overwrites the file on exit.");
        }

        var matches = ReadStrengths().Where(d => d.Device.StartsWith(devicePrefix, StringComparison.Ordinal)).ToList();
        if (matches.Count != 1)
        {
            throw new InvalidOperationException($"Expected exactly one wheel device starting with '{devicePrefix}', found {matches.Count}.");
        }

        var (device, oldValue) = matches[0];
        if (newValue > oldValue && !allowIncrease)
        {
            throw new InvalidOperationException($"Refusing to raise FFB strength from {oldValue} to {newValue} without explicit confirmation.");
        }

        string text = File.ReadAllText(FilePath);
        var pattern = new Regex(
            "(\"" + Regex.Escape(device) + "\"\\s*:\\s*\\{\\s*\"Force Feedback\"\\s*:\\s*\\{[^}]*?\"" + Regex.Escape(StrengthKey) + "\"\\s*:\\s*)(-?[0-9.]+)",
            RegexOptions.CultureInvariant);
        var match = pattern.Match(text);
        if (!match.Success)
        {
            throw new InvalidOperationException("Could not locate the strength value in the file; nothing was changed.");
        }

        string backupDir = Path.Combine(Path.GetDirectoryName(FilePath)!, "simapex_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
        Directory.CreateDirectory(backupDir);
        string backup = Path.Combine(backupDir, Path.GetFileName(FilePath));
        File.Copy(FilePath, backup, overwrite: false);

        string formatted = newValue.ToString("0.0", CultureInfo.InvariantCulture);
        string updated = text[..match.Groups[2].Index] + formatted + text[(match.Groups[2].Index + match.Groups[2].Length)..];
        File.WriteAllText(FilePath, updated, new UTF8Encoding(false));

        double check = ReadStrengths().Single(d => d.Device == device).Strength;
        if (Math.Abs(check - newValue) > 1e-6)
        {
            File.Copy(backup, FilePath, overwrite: true);
            throw new InvalidOperationException("Verification failed; the original file was restored.");
        }

        return new SettingChange(FilePath, backup, device + " / " + StrengthKey, oldValue, newValue, DateTime.Now);
    }
}
