namespace SimApex.Analysis.Tuning;

/// <summary>One detected hands-off oscillation episode.</summary>
/// <param name="Start">Game time of the first window, s.</param>
/// <param name="End">Game time of the last window end, s.</param>
/// <param name="SpeedKmh">Mean speed, km/h.</param>
/// <param name="FrequencyHz">Dominant frequency in the 3–9 Hz band.</param>
/// <param name="Amplitude">Peak steering deviation, fraction of full lock.</param>
/// <param name="AmplitudeRatio">Band amplitude relative to the median of hands-on straights.</param>
/// <param name="EnvelopeRate">Slope of ln(peak amplitude) over 1.5 s from the first significant peak, 1/s (positive = growing).</param>
/// <param name="DampingRatio">ζ ≈ −rate / (2π f): negative means the oscillation grows by itself.</param>
/// <param name="FfbSaturation">Fraction of the episode with the game's FFB at ±1.</param>
/// <param name="PeaksUsed">Peaks used for the envelope fit.</param>
public sealed record HandsOffEpisode(
    double Start,
    double End,
    double SpeedKmh,
    double FrequencyHz,
    double Amplitude,
    double AmplitudeRatio,
    double EnvelopeRate,
    double DampingRatio,
    double FfbSaturation,
    int PeaksUsed);

/// <summary>Summary of test T2.</summary>
/// <param name="Episodes">Detected episodes.</param>
/// <param name="BaselineAmplitude">Median 3–9 Hz amplitude on straights (hands on), fraction of full lock.</param>
/// <param name="MedianDampingRatio">Median ζ over episodes.</param>
/// <param name="TargetDampingRatio">Pass threshold.</param>
/// <param name="Passed">True when every episode has ζ ≥ target and none is a self-sustained oscillation.</param>
public sealed record HandsOffResult(IReadOnlyList<HandsOffEpisode> Episodes, double BaselineAmplitude, double MedianDampingRatio, double TargetDampingRatio, bool Passed);

/// <summary>
/// Test T2: the driver releases the wheel on a straight. The free response of wheel + base + game FFB is
/// recognised by its 3–9 Hz steering oscillation, and its damping is estimated from the envelope.
/// A target ζ of 0.12 means the oscillation falls below 10% within about three cycles.
/// </summary>
public static class HandsOffAnalysis
{
    public const double DefaultTargetDamping = 0.12;
    private const double Rate = 100; // Hz, resampling rate
    private const double Window = 2.0; // s
    private const double Step = 0.5; // s

    public static HandsOffResult Analyse(
        IReadOnlyList<TelemetryFrame> frames, double minSpeedKmh = 100, double detectionRatio = 8, double minAmplitude = 0.01, double targetDamping = DefaultTargetDamping)
    {
        ArgumentNullException.ThrowIfNull(frames);
        var rs = Resample(frames);
        int n = (int)(Window * Rate);
        int step = (int)(Step * Rate);
        var windows = new List<(int Start, double Amp, double Freq)>();
        for (int i = 0; i + n <= rs.Count; i += step)
        {
            bool straight = true;
            for (int k = i; k < i + n && straight; k++)
            {
                var f = rs[k];
                straight = f.SpeedKmh >= minSpeedKmh && Math.Abs(f.LatAccelG) < 0.5 && f.Brake < 0.05;
            }

            if (straight)
            {
                var (amp, freq) = BandAmplitude(rs, i, n);
                windows.Add((i, amp, freq));
            }
        }

        if (windows.Count == 0)
        {
            return new HandsOffResult([], double.NaN, double.NaN, targetDamping, false);
        }

        double baseline = Stats.Percentile(windows.Select(w => w.Amp), 0.5);
        var hot = windows.Where(w => w.Amp > detectionRatio * baseline && w.Amp > minAmplitude / 2).ToList();
        var episodes = new List<HandsOffEpisode>();
        int e = 0;
        while (e < hot.Count)
        {
            int first = e;
            while (e + 1 < hot.Count && hot[e + 1].Start - hot[e].Start <= step)
            {
                e++;
            }

            var best = hot.Skip(first).Take(e - first + 1).MaxBy(w => w.Amp);
            int s = hot[first].Start;
            int end = Math.Min(rs.Count, hot[e].Start + n);
            var ep = Episode(rs, s, end, best.Freq, best.Amp / baseline);
            if (ep is not null && ep.Amplitude >= minAmplitude)
            {
                episodes.Add(ep);
            }

            e++;
        }

        double medianZeta = episodes.Count > 0 ? Stats.Percentile(episodes.Select(x => x.DampingRatio), 0.5) : double.NaN;
        bool passed = episodes.Count > 0 && episodes.All(x => x.DampingRatio >= targetDamping);
        return new HandsOffResult(episodes, baseline, medianZeta, targetDamping, passed);
    }

    private static HandsOffEpisode? Episode(List<TelemetryFrame> rs, int start, int end, double freq, double ratio)
    {
        int len = end - start;
        if (len < Rate)
        {
            return null;
        }

        // Remove slow steering with a moving average of one period.
        int half = Math.Max(1, (int)(Rate / freq / 2));
        var x = new double[len];
        for (int i = 0; i < len; i++)
        {
            int a = Math.Max(start, start + i - half), b = Math.Min(end - 1, start + i + half);
            double m = 0;
            for (int k = a; k <= b; k++)
            {
                m += rs[k].Steering;
            }

            x[i] = rs[start + i].Steering - m / (b - a + 1);
        }

        var peaks = new List<(double T, double A)>();
        double maxAbs = x.Max(Math.Abs);
        for (int i = 1; i < len - 1; i++)
        {
            if (x[i] > 0 && x[i] >= x[i - 1] && x[i] > x[i + 1] && x[i] > 0.1 * maxAbs)
            {
                peaks.Add((i / Rate, x[i]));
            }
        }

        // The free response starts at the first significant peak (the release), not at the window start.
        double firstPeak = peaks.Count > 0 ? peaks[0].T : 0;
        var fit = peaks.Where(p => p.T <= firstPeak + 1.5).ToList();
        double rate = double.NaN;
        if (fit.Count >= 3)
        {
            double mt = fit.Average(p => p.T), ml = fit.Average(p => Math.Log(p.A));
            double num = fit.Sum(p => (p.T - mt) * (Math.Log(p.A) - ml));
            double den = fit.Sum(p => (p.T - mt) * (p.T - mt));
            rate = num / den;
        }

        double zeta = double.IsNaN(rate) ? double.NaN : -rate / (2 * Math.PI * freq);
        double speed = 0, sat = 0;
        for (int i = start; i < end; i++)
        {
            speed += rs[i].SpeedKmh;
            sat += Math.Abs(rs[i].FfbOutput) >= FfbGainAnalysis.SaturationThreshold ? 1 : 0;
        }

        return new HandsOffEpisode(rs[start].Time, rs[end - 1].Time, speed / len, freq, maxAbs, ratio, rate, zeta, sat / len, fit.Count);
    }

    private static (double Amp, double Freq) BandAmplitude(List<TelemetryFrame> rs, int start, int n)
    {
        double mean = 0;
        for (int i = 0; i < n; i++)
        {
            mean += rs[start + i].Steering;
        }

        mean /= n;
        double best = 0, bestF = 0;
        for (double f = 3; f <= 9.0001; f += 0.25)
        {
            double re = 0, im = 0, w = 2 * Math.PI * f / Rate;
            for (int i = 0; i < n; i++)
            {
                double v = rs[start + i].Steering - mean;
                re += v * Math.Cos(w * i);
                im += v * Math.Sin(w * i);
            }

            double amp = 2 * Math.Sqrt(re * re + im * im) / n;
            if (amp > best)
            {
                best = amp;
                bestF = f;
            }
        }

        return (best, bestF);
    }

    /// <summary>Resamples to a uniform 100 Hz grid on game time (linear for steering, hold for the rest).</summary>
    public static List<TelemetryFrame> Resample(IReadOnlyList<TelemetryFrame> frames)
    {
        ArgumentNullException.ThrowIfNull(frames);
        var outp = new List<TelemetryFrame>();
        if (frames.Count < 2)
        {
            return outp;
        }

        double t0 = frames[0].Time, t1 = frames[^1].Time;
        int j = 0;
        for (double t = t0; t <= t1; t += 1 / Rate)
        {
            while (j < frames.Count - 2 && frames[j + 1].Time < t)
            {
                j++;
            }

            var a = frames[j];
            var b = frames[j + 1];
            double u = b.Time > a.Time ? Math.Clamp((t - a.Time) / (b.Time - a.Time), 0, 1) : 0;
            outp.Add(a with { Time = t, Steering = a.Steering + (b.Steering - a.Steering) * u });
        }

        return outp;
    }
}
