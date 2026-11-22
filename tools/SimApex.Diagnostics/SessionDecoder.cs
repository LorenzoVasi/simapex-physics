using System.Globalization;
using System.Text;
using SimApex.Sim.Lmu;
using SimApex.Sim.Lmu.Recording;
using SimApex.Sim.Lmu.Sdk;

namespace SimApex.Diagnostics;

/// <summary>Decodes a recorded LMU session into a CSV of the player's car, using the SDK layout.</summary>
internal static class SessionDecoder
{
    public static void Run(string folder, string? sdkDirectory)
    {
        string headers = LmuSdk.FindHeaderDirectory(sdkDirectory)
            ?? throw new DirectoryNotFoundException("LMU SDK headers not found. Pass the folder <LMU>\\Support\\SharedMemoryInterface or set SIMAPEX_LMU_SDK.");
        var sdk = LmuSdk.Load(headers);
        var decoder = new LmuTelemetryDecoder(sdk);
        long frequency = LmuRecording.TimestampFrequency(folder);
        var report = new StringBuilder();
        var inv = CultureInfo.InvariantCulture;

        report.AppendLine(inv, $"SDK headers: {sdk.Source.Directory}");
        foreach (var (file, sha) in sdk.Source.Files)
        {
            report.AppendLine(inv, $"  {file}  sha256 {sha}");
        }

        int telemSize = sdk.Engine.GetLayout("TelemInfoV01").Size;
        report.AppendLine(inv, $"Layout: SharedMemoryObjectOut {sdk.Size} B, TelemInfoV01 {telemSize} B, TelemWheelV01 {sdk.Engine.GetLayout("TelemWheelV01").Size} B, slots {sdk.MaxVehicles}");
        foreach (string p in new[] { "generic.FFBTorque", "paths.userData", "scoring.scoringInfo.mTrackName", "telemetry.playerVehicleIdx", "telemetry.telemInfo[0].mSteeringShaftTorque", "telemetry.telemInfo[0].mWheel[0].mBrakePressure" })
        {
            var f = sdk.Resolve(p);
            report.AppendLine(inv, $"  {p,-50} offset 0x{f.Offset:X} ({f.Length} B, {f.Kind})");
        }

        string csvPath = Path.Combine(folder, "player_telemetry.csv");
        var values = new double[decoder.Channels.Count];
        long first = -1;
        int frames = 0, withPlayer = 0, uniqueEt = 0;
        double lastEt = double.NaN;
        var stats = new Dictionary<string, (double Min, double Max, double Sum, int N)>();
        bool validated = false;

        using (var csv = new StreamWriter(csvPath, false, new UTF8Encoding(false)))
        {
            csv.WriteLine("time_s,player_slot," + string.Join(',', decoder.Channels.Select(c => c.Name)));
            foreach (var (timestamp, memory) in LmuRecording.ReadFrames(folder))
            {
                if (!validated)
                {
                    if (sdk.Size > memory.Length)
                    {
                        throw new InvalidDataException($"Computed layout ({sdk.Size} B) is larger than the recorded mapping ({memory.Length} B): SDK headers and game version do not match.");
                    }

                    report.AppendLine(inv, $"Mapping in recording: {memory.Length} B");
                    report.AppendLine(inv, $"paths.userData: {LmuSdk.ReadString(memory, sdk.Resolve("paths.userData"))}");
                    report.AppendLine(inv, $"scoring track name: {LmuSdk.ReadString(memory, sdk.Resolve("scoring.scoringInfo.mTrackName"))}");
                    validated = true;
                }

                if (first < 0)
                {
                    first = timestamp;
                }

                frames++;
                int slot = decoder.PlayerIndex(memory);
                double t = (timestamp - first) / (double)frequency;
                if (slot < 0 || slot >= sdk.MaxVehicles)
                {
                    csv.WriteLine(string.Create(inv, $"{t:F4},-1"));
                    continue;
                }

                if (withPlayer == 0)
                {
                    report.AppendLine(inv, $"player slot {slot}: vehicle '{decoder.VehicleName(memory, slot)}', track '{decoder.TrackName(memory, slot)}', active vehicles {decoder.ActiveVehicles(memory)}");
                }

                withPlayer++;
                decoder.Decode(memory, slot, values);
                double et = values[decoder.IndexOf("game_elapsed_s")];
                if (et != lastEt)
                {
                    uniqueEt++;
                    lastEt = et;
                }

                var line = new StringBuilder();
                line.Append(t.ToString("F4", inv)).Append(',').Append(slot.ToString(inv));
                for (int k = 0; k < values.Length; k++)
                {
                    line.Append(',').Append(values[k].ToString("G7", inv));
                    string name = decoder.Channels[k].Name;
                    var s = stats.TryGetValue(name, out var cur) ? cur : (double.MaxValue, double.MinValue, 0.0, 0);
                    stats[name] = (Math.Min(s.Item1, values[k]), Math.Max(s.Item2, values[k]), s.Item3 + values[k], s.Item4 + 1);
                }

                csv.WriteLine(line.ToString());
            }
        }

        double duration = withPlayer > 0 ? stats["game_elapsed_s"].Max - stats["game_elapsed_s"].Min : 0;
        report.AppendLine(inv, $"Frames {frames}, with player vehicle {withPlayer}, distinct game timestamps {uniqueEt}");
        if (duration > 0)
        {
            report.AppendLine(inv, $"Game time covered {duration:F1} s -> telemetry update rate seen ~ {uniqueEt / duration:F1} Hz");
        }

        report.AppendLine();
        report.AppendLine("channel,min,max,mean,unit,origin,source");
        foreach (var c in decoder.Channels)
        {
            if (stats.TryGetValue(c.Name, out var s))
            {
                report.AppendLine(inv, $"{c.Name},{s.Min:G6},{s.Max:G6},{s.Sum / s.N:G6},\"{c.Unit}\",{c.Origin},\"{c.Source}\"");
            }
        }

        File.WriteAllText(Path.Combine(folder, "decode_summary.txt"), report.ToString());
        Console.Write(report.ToString());
        Console.WriteLine($"\nWrote {csvPath}");
    }
}
