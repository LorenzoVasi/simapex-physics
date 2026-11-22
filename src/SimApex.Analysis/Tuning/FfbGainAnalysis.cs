namespace SimApex.Analysis.Tuning;

/// <summary>Saturation of the game's FFB output and the gain change that would bring it to a target.</summary>
/// <param name="Samples">Frames used (moving, not excluded).</param>
/// <param name="Laps">Distinct laps among them (bootstrap blocks).</param>
/// <param name="FullScale">Shaft torque at which the game's output reaches ±1 now (1/k of the through-origin fit).</param>
/// <param name="FitR2">R² of FFB output vs shaft torque on unsaturated frames; near 1 means the output is linear in shaft torque.</param>
/// <param name="Saturation">Fraction of frames with |FFB| ≥ 0.999 now.</param>
/// <param name="TargetSaturation">Requested saturation fraction.</param>
/// <param name="RecommendedFullScale">Shaft torque percentile (1 − target): the full scale that gives the target saturation.</param>
/// <param name="RecommendedFullScaleLow">95% block-bootstrap lower bound.</param>
/// <param name="RecommendedFullScaleHigh">95% block-bootstrap upper bound.</param>
/// <param name="StrengthFactor">Multiply the current game FFB strength by this (FullScale / RecommendedFullScale).</param>
/// <param name="StrengthFactorLow">Lower bound (from the upper full-scale bound).</param>
/// <param name="StrengthFactorHigh">Upper bound (from the lower full-scale bound).</param>
/// <param name="Curve">Saturation fraction for a range of full-scale values.</param>
public sealed record FfbGainResult(
    int Samples,
    int Laps,
    double FullScale,
    double FitR2,
    double Saturation,
    double TargetSaturation,
    double RecommendedFullScale,
    double RecommendedFullScaleLow,
    double RecommendedFullScaleHigh,
    double StrengthFactor,
    double StrengthFactorLow,
    double StrengthFactorHigh,
    IReadOnlyList<(double FullScale, double Saturation)> Curve);

/// <summary>
/// Test T1. The game's FFB command is (for LMU) proportional to the steering shaft torque and clips at ±1.
/// The analysis finds the current proportionality, the saturation it causes, and the full scale that would
/// keep saturation at a target, with a confidence interval obtained by resampling whole laps.
/// </summary>
public static class FfbGainAnalysis
{
    public const double SaturationThreshold = 0.999;

    public static FfbGainResult? Analyse(
        IReadOnlyList<TelemetryFrame> frames, double targetSaturation = 0.01, double minSpeedKmh = 20, Func<TelemetryFrame, bool>? exclude = null)
    {
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetSaturation);
        var used = frames.Where(f => f.SpeedKmh >= minSpeedKmh && (exclude is null || !exclude(f))).ToList();
        if (used.Count < 500)
        {
            return null;
        }

        double sxy = 0, sxx = 0, syy = 0;
        foreach (var f in used.Where(f => Math.Abs(f.FfbOutput) < 0.98))
        {
            sxy += f.FfbOutput * f.ShaftTorque;
            sxx += f.ShaftTorque * f.ShaftTorque;
            syy += f.FfbOutput * f.FfbOutput;
        }

        if (sxx == 0 || sxy == 0)
        {
            return null;
        }

        double k = sxy / sxx;
        double fullScale = Math.Abs(1 / k);
        double r2 = sxy * sxy / (sxx * syy);
        double saturation = used.Count(f => Math.Abs(f.FfbOutput) >= SaturationThreshold) / (double)used.Count;

        var torque = used.Select(f => Math.Abs(f.ShaftTorque)).ToList();
        torque.Sort();
        double q = 1 - targetSaturation;
        double recommended = Stats.PercentileSorted(torque, q);

        var laps = used.GroupBy(f => f.Lap).Select(g => g.Select(f => Math.Abs(f.ShaftTorque)).ToList()).ToList();
        var (low, high) = Stats.BlockBootstrap(laps, sample =>
        {
            var all = sample.SelectMany(x => x).ToList();
            all.Sort();
            return Stats.PercentileSorted(all, q);
        });

        var curve = new List<(double, double)>();
        foreach (double scale in new[] { 0.8, 1.0, 1.15, 1.3, 1.5, 1.65, 1.8, 2.0 })
        {
            double fs = fullScale * scale;
            curve.Add((fs, torque.Count(t => t >= fs) / (double)torque.Count));
        }

        return new FfbGainResult(
            used.Count,
            laps.Count,
            fullScale,
            r2,
            saturation,
            targetSaturation,
            recommended,
            low,
            high,
            fullScale / recommended,
            double.IsNaN(high) ? double.NaN : fullScale / high,
            double.IsNaN(low) ? double.NaN : fullScale / low,
            curve);
    }
}
