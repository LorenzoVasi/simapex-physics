using SimApex.Core;

namespace SimApex.Analysis;

/// <summary>Identified mechanical parameters of a direct drive base with a given rim mounted.</summary>
/// <param name="Inertia">J, kg·m².</param>
/// <param name="ViscousDamping">b, N·m·s/rad.</param>
/// <param name="CoulombFriction">T_c, N·m.</param>
/// <param name="ResidualRms">RMS of the torque residual, N·m.</param>
/// <param name="SamplesUsed">Number of samples that entered the fit.</param>
public sealed record BaseParameters(
    Quantity Inertia,
    Quantity ViscousDamping,
    Quantity CoulombFriction,
    double ResidualRms,
    int SamplesUsed);

/// <summary>Options for <see cref="BaseIdentification"/>.</summary>
public sealed record BaseIdentificationOptions
{
    /// <summary>Half window of the Savitzky–Golay differentiator, in samples.</summary>
    public int HalfWindow { get; init; } = 25;

    /// <summary>Polynomial order of the differentiator.</summary>
    public int PolynomialOrder { get; init; } = 3;

    /// <summary>Samples with |ω| below this (rad/s) are skipped: the friction sign is ambiguous there.</summary>
    public double MinimumSpeed { get; init; } = 0.2;

    /// <summary>Uncertainty interval half-width, in standard errors.</summary>
    public double CoverageFactor { get; init; } = 2.0;
}

/// <summary>
/// Least-squares identification of J θ̈ + b θ̇ + T_c sgn(θ̇) = T from a commanded torque and the
/// measured wheel angle (test P1).
/// </summary>
/// <remarks>
/// A single torque step in one direction does not identify the model: during a step θ̈ is an affine
/// function of θ̇, so the three regressors are collinear. The excitation must contain at least two
/// torque levels and both directions of motion (for example alternating steps of two amplitudes).
/// </remarks>
public static class BaseIdentification
{
    public static BaseParameters Fit(
        ReadOnlySpan<double> commandedTorqueNm,
        ReadOnlySpan<double> angleRad,
        double dt,
        BaseIdentificationOptions? options = null)
    {
        options ??= new BaseIdentificationOptions();
        if (commandedTorqueNm.Length != angleRad.Length)
        {
            throw new ArgumentException("Torque and angle must have the same length.");
        }

        var velocity = new SavitzkyGolay(options.HalfWindow, options.PolynomialOrder, 1).Apply(angleRad, dt);
        var acceleration = new SavitzkyGolay(options.HalfWindow, options.PolynomialOrder, 2).Apply(angleRad, dt);

        var xtx = new double[3, 3];
        var xty = new double[3];
        int used = 0;
        var rows = new List<(double A, double W, double S, double T)>(angleRad.Length);

        for (int i = 0; i < angleRad.Length; i++)
        {
            double w = velocity[i];
            double a = acceleration[i];
            double t = commandedTorqueNm[i];
            if (!double.IsFinite(w) || !double.IsFinite(a) || !double.IsFinite(t) || Math.Abs(w) < options.MinimumSpeed)
            {
                continue;
            }

            double s = Math.Sign(w);
            Accumulate(xtx, xty, a, w, s, t);
            rows.Add((a, w, s, t));
            used++;
        }

        if (used < 10)
        {
            throw new InvalidOperationException("Not enough moving samples to identify the base.");
        }

        double[] theta = DenseSolver.Solve(xtx, xty);
        double j = theta[0], b = theta[1], tc = theta[2];

        double sse = 0;
        foreach (var r in rows)
        {
            double e = r.T - (j * r.A + b * r.W + tc * r.S);
            sse += e * e;
        }

        double sigma2 = sse / Math.Max(1, used - 3);
        var cov = DenseSolver.Inverse(xtx);
        double k = options.CoverageFactor;

        return new BaseParameters(
            Quantity.WithTolerance(j, k * Math.Sqrt(sigma2 * cov[0, 0]), Unit.KilogramMetreSquared, DataKind.Measurement),
            Quantity.WithTolerance(b, k * Math.Sqrt(sigma2 * cov[1, 1]), Unit.NewtonMetreSecondPerRadian, DataKind.Measurement),
            Quantity.WithTolerance(tc, k * Math.Sqrt(sigma2 * cov[2, 2]), Unit.NewtonMetre, DataKind.Measurement),
            Math.Sqrt(sse / used),
            used);
    }

    private static void Accumulate(double[,] xtx, double[] xty, double a, double w, double s, double t)
    {
        Span<double> x = [a, w, s];
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                xtx[r, c] += x[r] * x[c];
            }

            xty[r] += x[r] * t;
        }
    }
}
