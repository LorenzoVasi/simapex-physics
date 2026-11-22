using System.Diagnostics;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using SimApex.Hardware;

namespace SimApex.Diagnostics;

/// <summary>
/// Read-only session recorder for layout analysis. It never writes to the game or to any device.
/// <para>Output folder:</para>
/// <list type="bullet">
/// <item><c>lmu_base.bin</c>: first full copy of the LMU_Data mapping.</item>
/// <item><c>lmu_frames.bin</c>: one frame per tick, as [int64 timestamp][int32 runs] then runs of
/// [int32 offset][int32 length][bytes], holding only the bytes that changed since the previous frame.</item>
/// <item><c>hid_&lt;n&gt;.bin</c>: raw HID input reports as [int64 timestamp][int32 length][bytes].</item>
/// <item><c>session.txt</c>: timestamp frequency, devices, rates, counts.</item>
/// </list>
/// Timestamps are <see cref="Stopwatch.GetTimestamp"/> ticks, shared by all streams.
/// Recording stops when a file named <c>STOP</c> appears in the folder, or after the time limit.
/// </summary>
internal static class SessionRecorder
{
    private const string Mapping = "LMU_Data";

    public static void Run(string folder, double hz, double maxMinutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hz);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxMinutes);
        Directory.CreateDirectory(folder);
        string stopFile = Path.Combine(folder, "STOP");
        File.Delete(stopFile);

        using var mmf = MemoryMappedFile.OpenExisting(Mapping, MemoryMappedFileRights.Read);
        using var view = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        int size = (int)view.Capacity;
        var current = new byte[size];
        var previous = new byte[size];
        view.ReadArray(0, previous, 0, size);
        File.WriteAllBytes(Path.Combine(folder, "lmu_base.bin"), previous);

        long start = Stopwatch.GetTimestamp();
        var hidWriters = new List<(HidInterfaceInfo Device, BinaryWriter Writer, object Gate, int[] Count)>();
        // Never open the wheel base: on 2026-11-22 force feedback stopped working in LMU while the recorder
        // held the base's HID interfaces open from before the game acquired the device. The game telemetry
        // already carries steering and FFB, so only the pedals are recorded.
        var devices = HidScanner.Scan().Where(d => d.Known?.Role != DeviceRole.WheelBase && d.VendorId != KnownDevices.FanatecVendorId).ToList();
        for (int i = 0; i < devices.Count; i++)
        {
            var writer = new BinaryWriter(new BufferedStream(File.Create(Path.Combine(folder, $"hid_{i}.bin")), 1 << 16));
            var entry = (devices[i], writer, new object(), new int[1]);
            hidWriters.Add(entry);
            var thread = new Thread(() => RecordHid(entry.Item1, entry.writer, entry.Item3, entry.Item4, maxMinutes))
            {
                IsBackground = true,
                Name = $"hid_{i}",
            };
            thread.Start();
        }

        long frames = 0;
        long changedBytes = 0;
        long period = (long)(Stopwatch.Frequency / hz);
        long next = Stopwatch.GetTimestamp();
        long deadline = start + (long)(maxMinutes * 60 * Stopwatch.Frequency);
        using (var frameWriter = new BinaryWriter(new BufferedStream(File.Create(Path.Combine(folder, "lmu_frames.bin")), 1 << 20)))
        {
            var runs = new List<(int Offset, int Length)>();
            while (Stopwatch.GetTimestamp() < deadline && !File.Exists(stopFile))
            {
                next += period;
                long wait = next - Stopwatch.GetTimestamp();
                if (wait > 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds((double)wait / Stopwatch.Frequency));
                }

                long now = Stopwatch.GetTimestamp();
                view.ReadArray(0, current, 0, size);
                FindChangedRuns(previous, current, runs);
                frameWriter.Write(now);
                frameWriter.Write(runs.Count);
                foreach (var (offset, length) in runs)
                {
                    frameWriter.Write(offset);
                    frameWriter.Write(length);
                    frameWriter.Write(current, offset, length);
                    changedBytes += length;
                }

                (previous, current) = (current, previous);
                frames++;
                if (frames % (long)(hz * 30) == 0)
                {
                    Console.WriteLine($"{(now - start) / (double)Stopwatch.Frequency,8:F0} s  frames {frames}  changed bytes {changedBytes}");
                }
            }
        }

        var summary = new System.Text.StringBuilder();
        double seconds = (Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency;
        summary.AppendLine(CultureInfo.InvariantCulture, $"started {DateTime.Now.AddSeconds(-seconds):yyyy-MM-dd HH:mm:ss}");
        summary.AppendLine(CultureInfo.InvariantCulture, $"duration_s {seconds:F1}");
        summary.AppendLine(CultureInfo.InvariantCulture, $"timestamp_frequency {Stopwatch.Frequency}");
        summary.AppendLine(CultureInfo.InvariantCulture, $"start_timestamp {start}");
        summary.AppendLine(CultureInfo.InvariantCulture, $"lmu_mapping {Mapping} size {size} target_hz {hz} frames {frames} changed_bytes {changedBytes}");
        foreach (var (device, writer, gate, count) in hidWriters)
        {
            lock (gate)
            {
                writer.Flush();
                writer.Dispose();
            }

            summary.AppendLine(CultureInfo.InvariantCulture, $"hid {device.Vendor} PID 0x{device.ProductId:X4} report {device.MaxInputReportLength} B reports {count[0]} path {device.DevicePath}");
        }

        File.WriteAllText(Path.Combine(folder, "session.txt"), summary.ToString());
        Console.Write(summary.ToString());
    }

    private static void RecordHid(HidInterfaceInfo device, BinaryWriter writer, object gate, int[] count, double maxMinutes)
    {
        try
        {
            HidScanner.WatchRawReports(device.DevicePath, TimeSpan.FromMinutes(maxMinutes), (_, report) =>
            {
                long now = Stopwatch.GetTimestamp();
                lock (gate)
                {
                    if (writer.BaseStream.CanWrite)
                    {
                        writer.Write(now);
                        writer.Write(report.Length);
                        writer.Write(report);
                        count[0]++;
                    }
                }
            });
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or ObjectDisposedException)
        {
            Console.Error.WriteLine($"HID 0x{device.ProductId:X4}: {ex.Message}");
        }
    }

    private static void FindChangedRuns(byte[] a, byte[] b, List<(int Offset, int Length)> runs)
    {
        runs.Clear();
        int n = a.Length;
        int i = 0;
        while (i < n)
        {
            if (a[i] == b[i])
            {
                i++;
                continue;
            }

            int startRun = i;
            int lastDiff = i;
            // Merge differences separated by fewer than 8 equal bytes into one run.
            while (i < n && i - lastDiff < 8)
            {
                if (a[i] != b[i])
                {
                    lastDiff = i;
                }

                i++;
            }

            runs.Add((startRun, lastDiff - startRun + 1));
        }
    }
}
