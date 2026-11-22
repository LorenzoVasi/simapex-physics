using System.Globalization;
using System.Text;
using RealDrive.Hardware;
using RealDrive.Sim.Lmu;

// RealDrive diagnostics: read-only discovery of devices and LMU shared memory.
// Nothing here sends force feedback or writes to any device or game file.

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
string command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

try
{
    switch (command)
    {
        case "hid":
            Console.Write(HidReport());
            break;
        case "hid-watch":
            WatchHid(args);
            break;
        case "lmu":
            Console.Write(LmuReport());
            break;
        case "lmu-dump":
            DumpLmu(args);
            break;
        case "report":
            WriteFullReport(args.Length > 1 ? args[1] : "realdrive-diagnostics.txt");
            break;
        default:
            PrintHelp();
            break;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

return 0;

static string HidReport()
{
    var sb = new StringBuilder();
    var devices = HidScanner.Scan();
    sb.AppendLine("== HID interfaces (Fanatec, Thrustmaster) ==");
    if (devices.Count == 0)
    {
        sb.AppendLine("None found. Check USB connections and that the drivers are installed.");
    }

    for (int i = 0; i < devices.Count; i++)
    {
        var d = devices[i];
        sb.AppendLine(CultureInfo.InvariantCulture, $"[{i}] {d.Vendor} VID 0x{d.VendorId:X4} PID 0x{d.ProductId:X4}  \"{d.ProductName}\"  report {d.MaxInputReportLength} B");
        sb.AppendLine(d.Known is null
            ? "     not in the known-ID table: record this PID"
            : $"     known: {d.Known.Description} ({d.Known.Role}){(d.Known.Note is null ? "" : " - " + d.Known.Note)}");
        sb.AppendLine(CultureInfo.InvariantCulture, $"     path: {d.DevicePath}");
    }

    return sb.ToString();
}

static void WatchHid(string[] args)
{
    if (args.Length < 2 || !int.TryParse(args[1], out int index))
    {
        throw new ArgumentException("Usage: hid-watch <index from 'hid'> [seconds]");
    }

    double seconds = args.Length > 2 ? double.Parse(args[2], CultureInfo.InvariantCulture) : 15;
    var devices = HidScanner.Scan();
    if (index < 0 || index >= devices.Count)
    {
        throw new ArgumentOutOfRangeException(nameof(args), "No device with that index.");
    }

    var d = devices[index];
    Console.WriteLine($"Watching {d.Vendor} PID 0x{d.ProductId:X4} for {seconds} s. Move one axis at a time.");
    byte[]? previous = null;
    HidScanner.WatchRawReports(d.DevicePath, TimeSpan.FromSeconds(seconds), (t, report) =>
    {
        if (previous is not null && report.AsSpan().SequenceEqual(previous))
        {
            return;
        }

        previous = report;
        Console.WriteLine($"{t,8:F3}s  {Convert.ToHexString(report)}");
    });
}

static string LmuReport()
{
    var sb = new StringBuilder();
    sb.AppendLine("== Le Mans Ultimate ==");
    var processes = SharedMemoryProbe.FindCandidateProcesses();
    sb.AppendLine(processes.Count == 0
        ? "No process that looks like LMU is running."
        : "Candidate processes: " + string.Join(", ", processes));

    foreach (string name in SharedMemoryProbe.AllKnownMappings)
    {
        var p = SharedMemoryProbe.Probe(name, TimeSpan.FromMilliseconds(200));
        sb.AppendLine(p.Exists
            ? $"{name,-28} present, {p.CapacityBytes} bytes, {(p.IsChanging ? "LIVE (changing)" : "static")}"
            : $"{name,-28} {p.Error}");
    }

    return sb.ToString();
}

static void DumpLmu(string[] args)
{
    if (args.Length < 3)
    {
        throw new ArgumentException("Usage: lmu-dump <mapping name> <output file>");
    }

    long bytes = SharedMemoryProbe.DumpToFile(args[1], args[2]);
    Console.WriteLine($"Wrote {bytes} bytes to {Path.GetFullPath(args[2])}");
}

static void WriteFullReport(string path)
{
    var sb = new StringBuilder();
    sb.AppendLine(CultureInfo.InvariantCulture, $"RealDrive diagnostics {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"OS: {Environment.OSVersion}  .NET: {Environment.Version}");
    sb.AppendLine();
    sb.Append(HidReport());
    sb.AppendLine();
    sb.Append(LmuReport());
    File.WriteAllText(path, sb.ToString());
    Console.Write(sb.ToString());
    Console.WriteLine($"\nReport saved to {Path.GetFullPath(path)}");
}

static void PrintHelp()
{
    Console.WriteLine("""
        RealDrive diagnostics (read-only)

          hid                         list Fanatec and Thrustmaster HID interfaces
          hid-watch <index> [s]       print raw input reports of one interface when they change
          lmu                         check which LMU shared memory mappings exist and are live
          lmu-dump <name> <file>      copy one mapping to a file for layout analysis
          report [file]               hid + lmu, saved to a text file
        """);
}
