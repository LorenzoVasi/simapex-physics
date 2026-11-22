namespace SimApex.Analysis;

/// <summary>Effective scale and delay between the game's steering torque and the torque the base delivers.</summary>
/// <param name="Gain">k: delivered N·m per N·m computed by the game.</param>
/// <param name="DelaySeconds">τ: total delay game → driver → base filter.</param>
/// <param name="ExplainedVariance">Share of the reconstructed torque variance explained by k·T_game(t−τ), 0–1.</param>
public sealed record ScaleEstimate(double Gain, double DelaySeconds, double ExplainedVariance);

/// <summary>
/// Estimates k and τ in J θ̈ + b θ̇ + T_c sgn θ̇ = k · T_game(t − τ) from a hands-off run, once J, b and
/// T_c are known from test P1. This replaces LMU's removed "Steering Torque Capability" setting with
/// a measurement.
/// </summary>
public static class GameToBaseScale
{
    /// <param name="gameTorqueNm">Game torque resampled on the same uniform grid as the angle.</param>
    /// <param name="angleRad">Wheel angle from HID, uniform grid.</param>
    /// <param name="dt">Grid period, s. The delay resolution equals dt.</param>
    /// <param name="maxDelaySeconds">Largest delay searched.</param>
    public static ScaleEstimate Estimate(
        ReadOnlySpan<double> gameTorqueNm,
        ReadOnlySpan<double> angleRad,
        double dt,
        BaseParameters parameters,
        double maxDelaySeconds = 0.1,
        BaseIdentificationOptions? options = null)
    {
        options ??= new BaseIdentificationOptions();
        if (gameTorqueNm.Length != angleRad.Length)
        {
            throw new ArgumentException("Game torque and angle must have the same length.");
        }

        var velocity = new SavitzkyGolay(options.HalfWindow, options.PolynomialOrder, 1).Apply(angleRad, dt);
        var acceleration = new SavitzkyGolay(options.HalfWindow, options.PolynomialOrder, 2).Apply(angleRad, dt);

        double j = parameters.Inertia.Value;
        double b = parameters.ViscousDamping.Value;
        double tc = parameters.CoulombFriction.Value;
        int n = angleRad.Length;

        // Torque delivered by the motor, reconstructed from the motion.
        var delivered = new double[n];
        for (int i = 0; i < n; i++)
        {
            double w = velocity[i];
            delivered[i] = Math.Abs(w) < options.MinimumSpeed || !double.IsFinite(acceleration[i])
                ? double.NaN
                : j * acceleration[i] + b * w + tc * Math.Sign(w);
        }

        int maxLag = (int)Math.Round(maxDelaySeconds / dt);
        double bestSse = double.PositiveInfinity;
        double bestGain = 0;
        int bestLag = 0;
        double bestTotal = 0;

        for (int lag = 0; lag <= maxLag; lag++)
        {
            double num = 0, den = 0, yy = 0, ySum = 0;
            int count = 0;
            for (int i = lag; i < n; i++)
            {
                double y = delivered[i];
                double x = gameTorqueNm[i - lag];
                if (!double.IsFinite(y) || !double.IsFinite(x))
                {
                    continue;
                }

                num += x * y;
                den += x * x;
                yy += y * y;
                ySum += y;
                count++;
            }

            if (count < 10 || den <= 0)
            {
                continue;
            }

            double k = num / den;
            double sse = yy - k * num; // Σ(y − kx)² at the least-squares k
            if (sse < bestSse)
            {
                bestSse = sse;
                bestGain = k;
                bestLag = lag;
                bestTotal = yy - ySum * ySum / count;
            }
        }

        if (double.IsPositiveInfinity(bestSse))
        {
            throw new InvalidOperationException("Not enough moving samples to estimate the scale.");
        }

        double explained = bestTotal > 0 ? Math.Clamp(1 - bestSse / bestTotal, 0, 1) : 0;
        return new ScaleEstimate(bestGain, bestLag * dt, explained);
    }
}
