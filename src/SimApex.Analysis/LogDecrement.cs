namespace SimApex.Analysis;

/// <summary>Damping and frequency of a decaying oscillation (hands-off release, test M3).</summary>
/// <param name="DampingRatio">ζ.</param>
/// <param name="DampedFrequencyHz">f_d, from the spacing of the peaks.</param>
/// <param name="NaturalFrequencyHz">f_n = f_d / sqrt(1 − ζ²).</param>
/// <param name="PeaksUsed">Number of positive peaks used.</param>
public sealed record OscillationEstimate(double DampingRatio, double DampedFrequencyHz, double NaturalFrequencyHz, int PeaksUsed);

/// <summary>Logarithmic decrement on the positive peaks of a signal oscillating around zero.</summary>
public static class LogDecrement
{
    /// <param name="signal">Oscillation around zero (subtract the settling value first).</param>
    /// <param name="dt">Sample period, s.</param>
    /// <param name="minimumPeakFraction">Peaks smaller than this fraction of the first peak are noise and end the series.</param>
    public static OscillationEstimate? Estimate(ReadOnlySpan<double> signal, double dt, double minimumPeakFraction = 0.05)
    {
        var peaks = new List<(int Index, double Value)>();
        for (int i = 1; i < signal.Length - 1; i++)
        {
            double v = signal[i];
            if (v > 0 && v >= signal[i - 1] && v > signal[i + 1])
            {
                if (peaks.Count > 0 && v < peaks[0].Value * minimumPeakFraction)
                {
                    break;
                }

                peaks.Add((i, v));
            }
        }

        if (peaks.Count < 2)
        {
            return null; // No sustained oscillation to measure: over-damped or already settled.
        }

        int cycles = peaks.Count - 1;
        double delta = Math.Log(peaks[0].Value / peaks[^1].Value) / cycles;
        double zeta = delta / Math.Sqrt(4 * Math.PI * Math.PI + delta * delta);
        double period = (peaks[^1].Index - peaks[0].Index) * dt / cycles;
        double fd = 1.0 / period;
        double fn = zeta < 1 ? fd / Math.Sqrt(1 - zeta * zeta) : double.NaN;
        return new OscillationEstimate(zeta, fd, fn, peaks.Count);
    }
}
