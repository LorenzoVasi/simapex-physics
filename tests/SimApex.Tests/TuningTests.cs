using SimApex.Analysis.Tuning;

namespace SimApex.Tests;

public class TuningTests
{
    private static double Gaussian(Random rng) =>
        Math.Sqrt(-2 * Math.Log(1 - rng.NextDouble())) * Math.Cos(2 * Math.PI * rng.NextDouble());

    [Fact]
    public void Special_functions_match_known_values()
    {
        Assert.Equal(Math.Log(24), Stats.LogGamma(5), 10);
        Assert.Equal(0.3, Stats.RegularizedIncompleteBeta(1, 1, 0.3), 10);
        // Two-sided p for t = 2.228 with 10 degrees of freedom is 0.05.
        double p = Stats.RegularizedIncompleteBeta(5, 0.5, 10 / (10 + 2.228 * 2.228));
        Assert.Equal(0.05, p, 3);
    }

    [Fact]
    public void Welch_detects_a_real_difference_and_not_a_null_one()
    {
        var rng = new Random(1);
        var a = Enumerable.Range(0, 30).Select(_ => 1.50 + 0.05 * Gaussian(rng)).ToList();
        var b = Enumerable.Range(0, 30).Select(_ => 1.45 + 0.05 * Gaussian(rng)).ToList();
        var c = Enumerable.Range(0, 30).Select(_ => 1.50 + 0.05 * Gaussian(rng)).ToList();

        Assert.True(Stats.Welch(a, b)!.PValue < 0.01);
        Assert.True(Stats.Welch(a, c)!.PValue > 0.05);
    }

    [Fact]
    public void Ffb_gain_recovers_full_scale_and_recommends_the_right_percentile()
    {
        var rng = new Random(2);
        var frames = new List<TelemetryFrame>();
        for (int lap = 1; lap <= 8; lap++)
        {
            for (int i = 0; i < 5000; i++)
            {
                double torque = 15 * Gaussian(rng);
                double ffb = Math.Clamp(torque / 30.0, -1, 1);
                frames.Add(new TelemetryFrame(frames.Count * 0.01, lap, 150, ffb, torque, 0, 0, 0, 0, false, 5, false));
            }
        }

        var r = FfbGainAnalysis.Analyse(frames, targetSaturation: 0.01)!;

        Assert.Equal(30, r.FullScale, 1);
        Assert.True(r.FitR2 > 0.999);
        Assert.Equal(0.0455, r.Saturation, 2);              // P(|z| > 2) for a normal
        Assert.Equal(15 * 2.576, r.RecommendedFullScale, 0); // P(|z| > 2.576) = 1%
        Assert.InRange(r.RecommendedFullScale, r.RecommendedFullScaleLow, r.RecommendedFullScaleHigh);
        Assert.InRange(r.StrengthFactor, 0.76, 0.79); // analytic 30 / (15 * 2.576) = 0.776
    }

    [Fact]
    public void Hands_off_detects_growing_and_damped_oscillations_with_correct_sign()
    {
        var rng = new Random(3);
        var frames = new List<TelemetryFrame>();
        double dt = 0.01;
        for (int i = 0; i < 6000; i++)
        {
            double t = i * dt;
            double s = 0.001 * Gaussian(rng);
            // Episode 1 at 20 s: grows at +0.8 1/s, 6.5 Hz.
            if (t >= 20 && t < 23)
            {
                s += 0.02 * Math.Exp(0.8 * (t - 20)) * Math.Sin(2 * Math.PI * 6.5 * (t - 20));
            }

            // Episode 2 at 40 s: decays with zeta 0.05 at 6 Hz (rate = -zeta * 2 pi f).
            if (t >= 40 && t < 43)
            {
                s += 0.08 * Math.Exp(-0.05 * 2 * Math.PI * 6 * (t - 40)) * Math.Sin(2 * Math.PI * 6 * (t - 40));
            }

            frames.Add(new TelemetryFrame(t, 1, 220, 0, 0, s, 0, 0, 0, false, 5, false));
        }

        var r = HandsOffAnalysis.Analyse(frames);

        Assert.Equal(2, r.Episodes.Count);
        var grow = r.Episodes.Single(e => e.Start < 30);
        var decay = r.Episodes.Single(e => e.Start > 30);
        Assert.InRange(grow.FrequencyHz, 6.0, 7.0);
        Assert.True(grow.DampingRatio < 0, "a growing oscillation must have negative damping");
        Assert.InRange(decay.DampingRatio, 0.02, 0.09);
        Assert.False(r.Passed);
    }

    [Fact]
    public void Brake_events_and_abs_onset_are_measured()
    {
        var frames = new List<TelemetryFrame>();
        double t = 0;
        for (int stop = 0; stop < 6; stop++)
        {
            int map = stop < 3 ? 5 : 9;
            for (int i = 0; i < 200; i++, t += 0.01)
            {
                frames.Add(new TelemetryFrame(t, 1, 240, 0, 0, 0, 0, 0, 0, false, map, false));
            }

            double v = 240;
            for (int i = 0; i < 250; i++, t += 0.01)
            {
                double brake = Math.Min(1, i * 0.02);
                bool abs = brake >= 0.85;
                double decel = map == 5 ? 1.5 : 1.6;
                v -= decel * 9.80665 * 0.01 * 3.6;
                frames.Add(new TelemetryFrame(t, 1, v, 0, 0, 0, 0, -decel, brake, abs, map, false));
            }

            for (int i = 0; i < 50; i++, t += 0.01)
            {
                frames.Add(new TelemetryFrame(t, 1, v, 0, 0, 0, 0, 0, 0, false, map, false));
            }
        }

        var r = BrakeAnalysis.AnalyseTrack(frames);

        Assert.Equal(6, r.Events.Count);
        Assert.Equal(0.86, r.Events[0].BrakeAtAbsOnset, 2);
        Assert.Equal(2, r.Maps.Count);
        Assert.Equal(1.5, r.Maps.Single(m => m.AbsMap == 5).MeanDecelG, 2);
        Assert.NotNull(r.MapComparison);
    }

    [Fact]
    public void Pedal_pushes_are_found_with_their_plateau_level()
    {
        var pedal = new List<(double, double)>();
        double t = 0;
        foreach (double level in new[] { 0.6, 0.62, 0.95 })
        {
            for (int i = 0; i < 100; i++, t += 0.01)
            {
                pedal.Add((t, 0));
            }

            for (int i = 0; i < 30; i++, t += 0.01)
            {
                pedal.Add((t, level * i / 30.0));
            }

            for (int i = 0; i < 150; i++, t += 0.01)
            {
                pedal.Add((t, level));
            }
        }

        var pushes = BrakeAnalysis.DetectPushes(pedal);

        Assert.Equal(3, pushes.Count);
        Assert.Equal(0.95, pushes[2].Level, 3);
    }
}
