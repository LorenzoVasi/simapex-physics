using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SimApex.Sim.Lmu.Sdk;

/// <summary>A field resolved to an absolute byte range inside the shared memory block.</summary>
/// <param name="Path">The path that was resolved, e.g. <c>telemetry.telemInfo[0].mSteeringShaftTorque</c>.</param>
/// <param name="Offset">Absolute byte offset.</param>
/// <param name="Length">Byte length (whole array when the path stops at an array).</param>
/// <param name="Kind">Primitive kind of one element.</param>
/// <param name="TypeName">Declared C++ type.</param>
public sealed record ResolvedField(string Path, int Offset, int Length, PrimitiveKind Kind, string TypeName);

/// <summary>Identity of the SDK headers a layout was computed from.</summary>
/// <param name="Directory">Folder holding the headers.</param>
/// <param name="Files">Header file names with their SHA-256, in parse order.</param>
public sealed record LmuSdkSource(string Directory, IReadOnlyList<(string File, string Sha256)> Files);

/// <summary>
/// Shared memory layout of Le Mans Ultimate, computed at runtime from the official Studio 397 plugin
/// SDK headers installed with the game (<c>Support\SharedMemoryInterface</c>). The headers are read from
/// the user's installation and never copied or redistributed; their licence allows use for developing
/// extensions for Studio 397 products.
/// </summary>
public sealed class LmuSdk
{
    /// <summary>Name of the shared memory mapping (LMU_SHARED_MEMORY_FILE in the SDK).</summary>
    public const string MappingName = "LMU_Data";

    /// <summary>Root type of the mapping (SharedMemoryLayout { SharedMemoryObjectOut data; }).</summary>
    public const string RootType = "SharedMemoryObjectOut";

    private static readonly string[] HeaderOrder = ["PluginObjects.hpp", "InternalsPlugin.hpp", "SharedMemoryInterface.hpp"];
    private static readonly Regex PathSegment = new(@"^(?<name>[A-Za-z_]\w*)(?<idx>(\[\d+\])*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private LmuSdk(CppLayoutEngine engine, LmuSdkSource source)
    {
        Engine = engine;
        Source = source;
        Root = engine.GetLayout(RootType);
    }

    public CppLayoutEngine Engine { get; }

    public LmuSdkSource Source { get; }

    public TypeLayout Root { get; }

    /// <summary>Size of <c>SharedMemoryObjectOut</c> in bytes.</summary>
    public int Size => Root.Size;

    /// <summary>Number of vehicle slots in the telemetry array.</summary>
    public int MaxVehicles => Resolve("telemetry.telemInfo").Length / Engine.GetLayout("TelemInfoV01").Size;

    /// <summary>Loads the headers from a folder (normally <c>&lt;LMU&gt;\Support\SharedMemoryInterface</c>).</summary>
    public static LmuSdk Load(string headerDirectory)
    {
        var engine = new CppLayoutEngine();
        var files = new List<(string, string)>();
        foreach (string name in HeaderOrder)
        {
            string path = System.IO.Path.Combine(headerDirectory, name);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"LMU SDK header not found: {path}");
            }

            byte[] bytes = File.ReadAllBytes(path);
            files.Add((name, Convert.ToHexString(SHA256.HashData(bytes))));
            engine.AddHeader(Encoding.UTF8.GetString(bytes));
        }

        return new LmuSdk(engine, new LmuSdkSource(headerDirectory, files));
    }

    /// <summary>
    /// Finds <c>Support\SharedMemoryInterface</c> of the LMU installation: an explicit folder, the
    /// SIMAPEX_LMU_SDK environment variable, then every Steam library listed in libraryfolders.vdf.
    /// </summary>
    public static string? FindHeaderDirectory(string? explicitDirectory = null)
    {
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(explicitDirectory))
        {
            candidates.Add(explicitDirectory);
        }

        string? env = Environment.GetEnvironmentVariable("SIMAPEX_LMU_SDK");
        if (!string.IsNullOrWhiteSpace(env))
        {
            candidates.Add(env);
        }

        foreach (string library in SteamLibraries())
        {
            candidates.Add(System.IO.Path.Combine(library, "steamapps", "common", "Le Mans Ultimate", "Support", "SharedMemoryInterface"));
        }

        return candidates.FirstOrDefault(d => File.Exists(System.IO.Path.Combine(d, "SharedMemoryInterface.hpp")));
    }

    private static IEnumerable<string> SteamLibraries()
    {
        string? steam = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string
            ?? Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;
        if (steam is null)
        {
            yield break;
        }

        yield return steam;
        string vdf = System.IO.Path.Combine(steam, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdf))
        {
            yield break;
        }

        foreach (Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"(?<p>[^\"]+)\"", RegexOptions.CultureInvariant))
        {
            yield return m.Groups["p"].Value.Replace(@"\\", @"\", StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Resolves a dotted path from <c>SharedMemoryObjectOut</c>, with optional indices:
    /// <c>generic.FFBTorque</c>, <c>telemetry.telemInfo[3].mWheel[0].mTireLoad</c>, <c>telemetry.telemInfo[0].mLocalVel.x</c>.
    /// </summary>
    public ResolvedField Resolve(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        TypeLayout current = Root;
        int offset = 0;
        FieldLayout? field = null;
        int remainingCount = 1;
        string[] segments = path.Split('.');
        for (int s = 0; s < segments.Length; s++)
        {
            var match = PathSegment.Match(segments[s]);
            if (!match.Success)
            {
                throw new FormatException($"Bad path segment '{segments[s]}' in '{path}'.");
            }

            field = current.Find(match.Groups["name"].Value)
                ?? throw new KeyNotFoundException($"'{current.Name}' has no member '{match.Groups["name"].Value}' (path '{path}').");
            int[] indices = Regex.Matches(match.Groups["idx"].Value, @"\d+").Select(m => int.Parse(m.Value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
            if (indices.Length > field.Dimensions.Count)
            {
                throw new FormatException($"Too many indices for '{field.Name}' in '{path}'.");
            }

            int stride = field.TotalSize;
            int elementOffset = 0;
            for (int d = 0; d < indices.Length; d++)
            {
                stride /= field.Dimensions[d];
                if (indices[d] >= field.Dimensions[d])
                {
                    throw new ArgumentOutOfRangeException(nameof(path), $"Index {indices[d]} out of range for '{field.Name}' in '{path}'.");
                }

                elementOffset += indices[d] * stride;
            }

            offset += field.Offset + elementOffset;
            remainingCount = stride / field.ElementSize;
            if (s < segments.Length - 1)
            {
                if (field.Kind != PrimitiveKind.None || remainingCount != 1)
                {
                    throw new FormatException($"'{field.Name}' is not a single struct value, cannot continue '{path}'.");
                }

                current = Engine.GetLayout(field.TypeName);
            }
        }

        return new ResolvedField(path, offset, field!.ElementSize * remainingCount, field.Kind, field.TypeName);
    }

    /// <summary>Reads a numeric field as double (bool and integer kinds are converted).</summary>
    public static double ReadNumber(ReadOnlySpan<byte> memory, ResolvedField f)
    {
        ArgumentNullException.ThrowIfNull(f);
        var s = memory.Slice(f.Offset);
        return f.Kind switch
        {
            PrimitiveKind.Float64 => BinaryPrimitives.ReadDoubleLittleEndian(s),
            PrimitiveKind.Float32 => BinaryPrimitives.ReadSingleLittleEndian(s),
            PrimitiveKind.Int8 => (sbyte)s[0],
            PrimitiveKind.UInt8 or PrimitiveKind.Char => s[0],
            PrimitiveKind.Bool => s[0] != 0 ? 1 : 0,
            PrimitiveKind.Int16 => BinaryPrimitives.ReadInt16LittleEndian(s),
            PrimitiveKind.UInt16 => BinaryPrimitives.ReadUInt16LittleEndian(s),
            PrimitiveKind.Int32 => BinaryPrimitives.ReadInt32LittleEndian(s),
            PrimitiveKind.UInt32 => BinaryPrimitives.ReadUInt32LittleEndian(s),
            PrimitiveKind.Int64 => BinaryPrimitives.ReadInt64LittleEndian(s),
            PrimitiveKind.UInt64 or PrimitiveKind.Pointer => BinaryPrimitives.ReadUInt64LittleEndian(s),
            _ => throw new InvalidOperationException($"'{f.Path}' is not a numeric field."),
        };
    }

    /// <summary>Reads a char array as a NUL-terminated string (UTF-8, falling back to Latin-1 bytes).</summary>
    public static string ReadString(ReadOnlySpan<byte> memory, ResolvedField f)
    {
        ArgumentNullException.ThrowIfNull(f);
        var s = memory.Slice(f.Offset, f.Length);
        int end = s.IndexOf((byte)0);
        return Encoding.UTF8.GetString(end < 0 ? s : s[..end]);
    }
}
