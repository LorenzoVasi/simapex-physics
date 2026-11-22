namespace SimApex.Analysis.Tuning;

/// <summary>A hard braking event on track.</summary>
/// <param name="Start">Game time, s.</param>
/// <param name="Lap">Lap.</param>
/// <param name="AbsMap">ABS setting index (most frequent during the event).</param>
/// <param name="SpeedStart">km/h.</param>
/// <param name="SpeedEnd">km/h.</param>
/// <param name="Duration">s.</param>
/// <param name="MeanDecelG">Mean deceleration, g.</param>
/// <param name="PeakDecelG">Peak deceleration, g.</param>
/// <param name="AbsFraction">Fraction of the event with ABS active.</param>
/// <param name="MeanBrake">Mean brake input.</param>
/// <param name="BrakeAtAbsOnset">Brake input when ABS first engaged, or NaN.</param>
/// <param name="BottomedFraction">Fraction of the event with brake input ≥ 0.99 (pedal axis at its end).</param>
public sealed record BrakeEvent(
    double Start, int Lap, int AbsMap, double SpeedStart, double SpeedEnd, double Duration,
    double MeanDecelG, double PeakDecelG, double AbsFraction, double MeanBrake, double BrakeAtAbsOnset, double BottomedFraction);

/// <summary>Per-ABS-map statistics over hard braking events.</summary>
public sealed record AbsMapSummary(int AbsMap, int Events, double MeanDecelG, double DecelStdG, double MeanAbsFraction, double MedianBrakeAtOnset);

/// <summary>Brake input at ABS onset in one speed band.</summary>
public sealed record AbsOnsetBand(double FromKmh, double ToKmh, int Count, double P10, double P50, double P90);

/// <summary>Summary of the on-track part of test T3.</summary>
/// <param name="Events">Hard braking events.</param>
/// <param name="Maps">Statistics per ABS map.</param>
/// <param name="MapComparison">Welch test of mean deceleration between the two most used maps, if available.</param>
/// <param name="OnsetBands">Brake input at ABS onset by speed band (straight-line braking only).</param>
/// <param name="BottomedFraction">Over all hard braking time, fraction with the brake axis at its end.</param>
public sealed record BrakeTrackResult(
    IReadOnlyList<BrakeEvent> Events,
    IReadOnlyList<AbsMapSummary> Maps,
    (int MapA, int MapB, WelchResult Test)? MapComparison,
    IReadOnlyList<AbsOnsetBand> OnsetBands,
    double BottomedFraction);

/// <summary>One sustained push on the brake pedal (static part of test T3).</summary>
/// <param name="Start">s.</param>
/// <param name="End">s.</param>
/// <param name="Level">Median pedal output during the plateau, 0..1.</param>
/// <param name="Peak">Maximum pedal output, 0..1.</param>
public sealed record PedalPush(double Start, double End, double Level, double Peak);

/// <summary>Test T3 analyses: hard stops on track and static pushes on the pedal.</summary>
public static class BrakeAnalysis
{
    public static BrakeTrackResult AnalyseTrack(IReadOnlyList<TelemetryFrame> frames, double minStartKmh = 120)
    {
        ArgumentNullException.ThrowIfNull(frames);
        var events = new List<BrakeEvent>();
        for (int i = 1; i < frames.Count; i++)
        {
            if (!(frames[i].Brake > 0.7 && frames[i - 1].Brake <= 0.7 && frames[i].SpeedKmh > minStartKmh))
            {
                continue;
            }

            int j = i;
            while (j < frames.Count - 1 && frames[j].Brake > 0.3)
            {
                j++;
            }

            var seg = frames.Skip(i).Take(j - i).ToList();
            double duration = seg.Count > 0 ? seg[^1].Time - seg[0].Time : 0;
            if (duration < 0.3)
            {
                continue;
            }

            int onsetIndex = seg.FindIndex(f => f.AbsActive);
            events.Add(new BrakeEvent(
                seg[0].Time,
                seg[0].Lap,
                seg.GroupBy(f => f.AbsMap).MaxBy(g => g.Count())!.Key,
                seg[0].SpeedKmh,
                seg[^1].SpeedKmh,
                duration,
                -seg.Average(f => f.LongAccelG),
                -seg.Min(f => f.LongAccelG),
                seg.Count(f => f.AbsActive) / (double)seg.Count,
                seg.Average(f => f.Brake),
                onsetIndex >= 0 ? seg[onsetIndex].Brake : double.NaN,
                seg.Count(f => f.Brake >= 0.99) / (double)seg.Count));
            i = j;
        }

        var maps = events.GroupBy(e => e.AbsMap)
            .Select(g =>
            {
                var d = g.Select(e => e.MeanDecelG).ToList();
                var onset = g.Select(e => e.BrakeAtAbsOnset).Where(x => !double.IsNaN(x)).ToList();
                return new AbsMapSummary(g.Key, g.Count(), d.Average(), d.Count > 1 ? Math.Sqrt(Stats.Variance(d)) : double.NaN,
                    g.Average(e => e.AbsFraction), onset.Count > 0 ? Stats.Percentile(onset, 0.5) : double.NaN);
            })
            .OrderByDescending(m => m.Events)
            .ToList();

        (int, int, WelchResult)? comparison = null;
        if (maps.Count >= 2)
        {
            var a = events.Where(e => e.AbsMap == maps[0].AbsMap).Select(e => e.MeanDecelG).ToList();
            var b = events.Where(e => e.AbsMap == maps[1].AbsMap).Select(e => e.MeanDecelG).ToList();
            var w = Stats.Welch(a, b);
            if (w is not null)
            {
                comparison = (maps[0].AbsMap, maps[1].AbsMap, w);
            }
        }

        var onsets = new List<(double Speed, double Brake)>();
        for (int i = 1; i < frames.Count; i++)
        {
            var f = frames[i];
            if (f.AbsActive && !frames[i - 1].AbsActive && f.SpeedKmh > 60 && f.Brake > 0.05 && Math.Abs(f.LatAccelG) < 0.6)
            {
                onsets.Add((f.SpeedKmh, f.Brake));
            }
        }

        var bands = new List<AbsOnsetBand>();
        foreach (var (lo, hi) in new[] { (60.0, 120.0), (120.0, 180.0), (180.0, 230.0), (230.0, 400.0) })
        {
            var v = onsets.Where(o => o.Speed >= lo && o.Speed < hi).Select(o => o.Brake).ToList();
            v.Sort();
            bands.Add(new AbsOnsetBand(lo, hi, v.Count, Stats.PercentileSorted(v, 0.1), Stats.PercentileSorted(v, 0.5), Stats.PercentileSorted(v, 0.9)));
        }

        double totalTime = events.Sum(e => e.Duration);
        double bottomed = totalTime > 0 ? events.Sum(e => e.BottomedFraction * e.Duration) / totalTime : double.NaN;
        return new BrakeTrackResult(events, maps, comparison, bands, bottomed);
    }

    /// <summary>Finds sustained pushes (output above <paramref name="threshold"/> for at least <paramref name="minDuration"/> s).</summary>
    public static IReadOnlyList<PedalPush> DetectPushes(IReadOnlyList<(double Time, double Value)> pedal, double threshold = 0.15, double minDuration = 0.8)
    {
        ArgumentNullException.ThrowIfNull(pedal);
        var pushes = new List<PedalPush>();
        int i = 0;
        while (i < pedal.Count)
        {
            if (pedal[i].Value <= threshold)
            {
                i++;
                continue;
            }

            int s = i;
            while (i < pedal.Count && pedal[i].Value > threshold)
            {
                i++;
            }

            int e = i - 1;
            if (pedal[e].Time - pedal[s].Time >= minDuration)
            {
                // Plateau: the middle 60% of the push, away from the ramps.
                int a = s + (e - s) / 5, b = e - (e - s) / 5;
                var mid = new List<double>();
                for (int k = a; k <= b; k++)
                {
                    mid.Add(pedal[k].Value);
                }

                double peak = 0;
                for (int k = s; k <= e; k++)
                {
                    peak = Math.Max(peak, pedal[k].Value);
                }

                pushes.Add(new PedalPush(pedal[s].Time, pedal[e].Time, Stats.Percentile(mid, 0.5), peak));
            }
        }

        return pushes;
    }
}
