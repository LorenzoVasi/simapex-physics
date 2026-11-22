using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using SimApex.Analysis.Tuning;
using SimApex.Hardware;
using SimApex.Sim.Lmu;
using SimApex.Sim.Lmu.Recording;

namespace SimApex.Tune;

/// <summary>Turns decoded LMU channels into analysis frames.</summary>
internal sealed class FrameMapper(LmuTelemetryDecoder decoder)
{
    private readonly int _et = decoder.IndexOf("game_elapsed_s");
    private readonly int _lap = decoder.IndexOf("lap");
    private readonly int _speed = decoder.IndexOf("speed_kmh");
    private readonly int _ffb = decoder.IndexOf("ffb_torque");
    private readonly int _torque = decoder.IndexOf("steering_shaft_torque");
    private readonly int _steer = decoder.IndexOf("steering_raw");
    private readonly int _lat = decoder.IndexOf("lat_accel_g");
    private readonly int _long = decoder.IndexOf("long_accel_g");
    private readonly int _brake = decoder.IndexOf("brake_raw");
    private readonly int _abs = decoder.IndexOf("abs_active");
    private readonly int _absMap = decoder.IndexOf("abs_setting");
    private readonly int _kerbL = decoder.IndexOf("surface_type_fl");
    private readonly int _kerbR = decoder.IndexOf("surface_type_fr");
    private readonly double[] _values = new double[decoder.Channels.Count];

    public LmuTelemetryDecoder Decoder { get; } = decoder;

    /// <summary>Decodes the player's car; null when there is no player vehicle.</summary>
    public TelemetryFrame? Map(byte[] memory)
    {
        int slot = Decoder.PlayerIndex(memory);
        if (slot < 0)
        {
            return null;
        }

        Decoder.Decode(memory, slot, _values);
        var v = _values;
        return new TelemetryFrame(v[_et], (int)v[_lap], v[_speed], v[_ffb], v[_torque], v[_steer], v[_lat], v[_long], v[_brake],
            v[_abs] != 0, (int)v[_absMap], v[_kerbL] == 5 || v[_kerbR] == 5);
    }
}

/// <summary>Frames from a session recorded with SimApex.Diagnostics, one per distinct game update.</summary>
internal static class RecordingSource
{
    public static List<TelemetryFrame> Load(string folder, FrameMapper mapper)
    {
        var frames = new List<TelemetryFrame>();
        double last = double.NaN;
        foreach (var (_, memory) in LmuRecording.ReadFrames(folder))
        {
            if (mapper.Map(memory) is { } f && f.Time != last)
            {
                frames.Add(f);
                last = f.Time;
            }
        }

        return frames;
    }
}

/// <summary>Polls LMU's shared memory (read-only) and the T-LCM while a test runs.</summary>
internal sealed class LiveCapture : IDisposable
{
    private readonly FrameMapper _mapper;
    private readonly MemoryMappedFile _mmf;
    private readonly MemoryMappedViewAccessor _view;
    private readonly byte[] _buffer;

    public LiveCapture(FrameMapper mapper)
    {
        _mapper = mapper;
        _mmf = MemoryMappedFile.OpenExisting(LmuSdkMapping.Name, MemoryMappedFileRights.Read);
        _view = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        _buffer = new byte[_view.Capacity];
    }

    /// <summary>
    /// Captures until <paramref name="done"/> returns true or the user presses Enter.
    /// <paramref name="status"/> is printed on one line twice per second.
    /// </summary>
    public List<TelemetryFrame> Run(Func<List<TelemetryFrame>, bool> done, Func<List<TelemetryFrame>, string> status)
    {
        var frames = new List<TelemetryFrame>();
        double last = double.NaN;
        var clock = Stopwatch.StartNew();
        long nextStatus = 0;
        while (true)
        {
            _view.ReadArray(0, _buffer, 0, _buffer.Length);
            if (_mapper.Map(_buffer) is { } f && f.Time != last)
            {
                frames.Add(f);
                last = f.Time;
            }

            if (clock.ElapsedMilliseconds >= nextStatus)
            {
                nextStatus = clock.ElapsedMilliseconds + 500;
                string s = status(frames);
                Console.Write("\r" + s.PadRight(Math.Max(s.Length, Console.WindowWidth - 1)));
                if (done(frames))
                {
                    break;
                }
            }

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
            {
                break;
            }

            Thread.Sleep(5);
        }

        Console.WriteLine();
        return frames;
    }

    public void Dispose()
    {
        _view.Dispose();
        _mmf.Dispose();
    }
}

internal static class LmuSdkMapping
{
    public const string Name = Sim.Lmu.Sdk.LmuSdk.MappingName;
}

/// <summary>Reads the T-LCM brake output while a static test runs (game not needed).</summary>
internal static class PedalCapture
{
    public static List<(double Time, double Brake)> Run(string devicePath, TimeSpan maxDuration, Func<List<(double Time, double Brake)>, string> status)
    {
        var samples = new List<(double, double)>();
        var gate = new object();
        using var cts = new CancellationTokenSource();
        var thread = new Thread(() => HidScanner.WatchRawReports(devicePath, maxDuration, (t, report) =>
        {
            if (TlcmPedals.Decode(report) is { } s)
            {
                lock (gate)
                {
                    samples.Add((t, s.Brake));
                }
            }
        }, cts.Token))
        { IsBackground = true };
        thread.Start();
        var clock = Stopwatch.StartNew();
        while (thread.IsAlive && clock.Elapsed < maxDuration)
        {
            Thread.Sleep(250);
            string line;
            lock (gate)
            {
                line = status(samples);
            }

            Console.Write("\r" + line.PadRight(Math.Max(line.Length, Console.WindowWidth - 1)));
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
            {
                break;
            }
        }

        cts.Cancel();
        thread.Join(1000);
        Console.WriteLine();
        lock (gate)
        {
            return [.. samples];
        }
    }
}
