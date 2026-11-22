using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;

namespace RealDrive.Sim.Lmu;

/// <summary>Result of probing one named shared memory mapping.</summary>
/// <param name="Name">Mapping name.</param>
/// <param name="Exists">True when the mapping could be opened for reading.</param>
/// <param name="CapacityBytes">Size of the mapped view, when it exists.</param>
/// <param name="IsChanging">True when the content changed between two reads: the game is writing to it.</param>
/// <param name="Error">Why the mapping could not be opened, when it does not exist.</param>
public sealed record MappingProbe(string Name, bool Exists, long CapacityBytes, bool IsChanging, string? Error);

/// <summary>
/// Read-only discovery of the shared memory published by Le Mans Ultimate. The struct layouts are
/// not officially documented, so the first step on a real system is to confirm which mappings exist,
/// their size, and that they are live, before any field is decoded.
/// </summary>
public static class SharedMemoryProbe
{
    /// <summary>Native mapping published by LMU when Settings → Gameplay → Enable Plugins is ON.</summary>
    public const string NativeMapping = "LMU_Data";

    /// <summary>Mappings published by the community rFactor 2 shared memory plugin.</summary>
    public static readonly IReadOnlyList<string> PluginMappings =
    [
        "$rFactor2SMMP_Telemetry$",
        "$rFactor2SMMP_Scoring$",
        "$rFactor2SMMP_Extended$",
    ];

    public static IReadOnlyList<string> AllKnownMappings { get; } = [NativeMapping, .. PluginMappings];

    public static MappingProbe Probe(string name, TimeSpan liveCheckInterval)
    {
        try
        {
            using var mmf = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.Read);
            using var view = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
            long capacity = view.Capacity;

            byte[] first = Snapshot(view, capacity);
            Thread.Sleep(liveCheckInterval);
            byte[] second = Snapshot(view, capacity);

            bool changing = !SHA256.HashData(first).AsSpan().SequenceEqual(SHA256.HashData(second));
            return new MappingProbe(name, true, capacity, changing, null);
        }
        catch (FileNotFoundException)
        {
            return new MappingProbe(name, false, 0, false, "not found (game not running, or option/plugin not enabled)");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            return new MappingProbe(name, false, 0, false, ex.Message);
        }
    }

    /// <summary>Copies the whole mapping to a file, for offline layout analysis.</summary>
    public static long DumpToFile(string name, string path)
    {
        using var mmf = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.Read);
        using var view = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        byte[] bytes = Snapshot(view, view.Capacity);
        File.WriteAllBytes(path, bytes);
        return bytes.LongLength;
    }

    /// <summary>Running processes whose name suggests Le Mans Ultimate (the executable name is not documented).</summary>
    public static IReadOnlyList<string> FindCandidateProcesses()
    {
        var names = new List<string>();
        foreach (var p in Process.GetProcesses())
        {
            using (p)
            {
                string n = p.ProcessName;
                if (n.Contains("lemans", StringComparison.OrdinalIgnoreCase)
                    || n.Contains("le mans", StringComparison.OrdinalIgnoreCase)
                    || n.StartsWith("lmu", StringComparison.OrdinalIgnoreCase))
                {
                    names.Add($"{n} (pid {p.Id})");
                }
            }
        }

        return names;
    }

    private static byte[] Snapshot(MemoryMappedViewAccessor view, long capacity)
    {
        if (capacity > int.MaxValue)
        {
            throw new InvalidOperationException($"Mapping of {capacity} bytes is larger than expected.");
        }

        var buffer = new byte[capacity];
        view.ReadArray(0, buffer, 0, buffer.Length);
        return buffer;
    }
}
