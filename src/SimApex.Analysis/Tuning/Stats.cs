namespace SimApex.Analysis.Tuning;

/// <summary>Result of Welch's unequal-variance t-test.</summary>
/// <param name="MeanDifference">mean(a) − mean(b).</param>
/// <param name="T">t statistic.</param>
/// <param name="DegreesOfFreedom">Welch–Satterthwaite degrees of freedom.</param>
/// <param name="PValue">Two-sided p-value.</param>
public sealed record WelchResult(double MeanDifference, double T, double DegreesOfFreedom, double PValue);

/// <summary>Small, dependency-free statistics used by the tuning analyses.</summary>
public static class Stats
{
    /// <summary>Linear-interpolated percentile of an already sorted list, q in [0, 1].</summary>
    public static double PercentileSorted(IReadOnlyList<double> sorted, double q)
    {
        ArgumentNullException.ThrowIfNull(sorted);
        if (sorted.Count == 0)
        {
            return double.NaN;
        }

        double pos = Math.Clamp(q, 0, 1) * (sorted.Count - 1);
        int lo = (int)Math.Floor(pos);
        int hi = Math.Min(lo + 1, sorted.Count - 1);
        return sorted[lo] + (sorted[hi] - sorted[lo]) * (pos - lo);
    }

    public static double Percentile(IEnumerable<double> values, double q)
    {
        var s = values.ToList();
        s.Sort();
        return PercentileSorted(s, q);
    }

    public static double Mean(IReadOnlyList<double> v)
    {
        ArgumentNullException.ThrowIfNull(v);
        return v.Count == 0 ? double.NaN : v.Average();
    }

    public static double Variance(IReadOnlyList<double> v)
    {
        ArgumentNullException.ThrowIfNull(v);
        if (v.Count < 2)
        {
            return double.NaN;
        }

        double m = v.Average();
        return v.Sum(x => (x - m) * (x - m)) / (v.Count - 1);
    }

    /// <summary>
    /// Percentile bootstrap confidence interval resampling whole blocks (laps), so that correlated
    /// samples inside a lap are not treated as independent. Deterministic for a given seed.
    /// </summary>
    public static (double Low, double High) BlockBootstrap<T>(
        IReadOnlyList<T> blocks, Func<IReadOnlyList<T>, double> statistic, int resamples = 1000, double level = 0.95, int seed = 20261122)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(statistic);
        if (blocks.Count < 2)
        {
            return (double.NaN, double.NaN);
        }

        var rng = new Random(seed);
        var results = new List<double>(resamples);
        var sample = new T[blocks.Count];
        for (int r = 0; r < resamples; r++)
        {
            for (int i = 0; i < sample.Length; i++)
            {
                sample[i] = blocks[rng.Next(blocks.Count)];
            }

            double s = statistic(sample);
            if (!double.IsNaN(s))
            {
                results.Add(s);
            }
        }

        results.Sort();
        double alpha = (1 - level) / 2;
        return (PercentileSorted(results, alpha), PercentileSorted(results, 1 - alpha));
    }

    /// <summary>Welch's t-test for two independent samples (e.g. per-lap metrics before/after a change).</summary>
    public static WelchResult? Welch(IReadOnlyList<double> a, IReadOnlyList<double> b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        if (a.Count < 2 || b.Count < 2)
        {
            return null;
        }

        double va = Variance(a) / a.Count;
        double vb = Variance(b) / b.Count;
        double diff = Mean(a) - Mean(b);
        if (va + vb == 0)
        {
            return new WelchResult(diff, diff == 0 ? 0 : double.PositiveInfinity, a.Count + b.Count - 2, diff == 0 ? 1 : 0);
        }

        double t = diff / Math.Sqrt(va + vb);
        double df = (va + vb) * (va + vb) / (va * va / (a.Count - 1) + vb * vb / (b.Count - 1));
        double p = RegularizedIncompleteBeta(df / 2, 0.5, df / (df + t * t));
        return new WelchResult(diff, t, df, p);
    }

    /// <summary>Regularised incomplete beta I_x(a, b) (continued fraction, Numerical Recipes §6.4).</summary>
    public static double RegularizedIncompleteBeta(double a, double b, double x)
    {
        if (x <= 0)
        {
            return 0;
        }

        if (x >= 1)
        {
            return 1;
        }

        double lnBeta = LogGamma(a + b) - LogGamma(a) - LogGamma(b);
        double front = Math.Exp(Math.Log(x) * a + Math.Log(1 - x) * b + lnBeta);
        return x < (a + 1) / (a + b + 2)
            ? front * BetaContinuedFraction(a, b, x) / a
            : 1 - front * BetaContinuedFraction(b, a, 1 - x) / b;
    }

    private static double BetaContinuedFraction(double a, double b, double x)
    {
        const double Tiny = 1e-30;
        double c = 1, d = 1 - (a + b) * x / (a + 1);
        d = Math.Abs(d) < Tiny ? Tiny : d;
        d = 1 / d;
        double h = d;
        for (int m = 1; m <= 300; m++)
        {
            int m2 = 2 * m;
            double aa = m * (b - m) * x / ((a + m2 - 1) * (a + m2));
            d = 1 + aa * d;
            d = Math.Abs(d) < Tiny ? Tiny : d;
            c = 1 + aa / c;
            c = Math.Abs(c) < Tiny ? Tiny : c;
            d = 1 / d;
            h *= d * c;
            aa = -(a + m) * (a + b + m) * x / ((a + m2) * (a + m2 + 1));
            d = 1 + aa * d;
            d = Math.Abs(d) < Tiny ? Tiny : d;
            c = 1 + aa / c;
            c = Math.Abs(c) < Tiny ? Tiny : c;
            d = 1 / d;
            double del = d * c;
            h *= del;
            if (Math.Abs(del - 1) < 1e-12)
            {
                break;
            }
        }

        return h;
    }

    /// <summary>ln Γ(x), Lanczos approximation (g = 7, n = 9).</summary>
    public static double LogGamma(double x)
    {
        double[] g = [0.99999999999980993, 676.5203681218851, -1259.1392167224028, 771.32342877765313,
            -176.61502916214059, 12.507343278686905, -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7];
        if (x < 0.5)
        {
            return Math.Log(Math.PI / Math.Abs(Math.Sin(Math.PI * x))) - LogGamma(1 - x);
        }

        x -= 1;
        double s = g[0];
        for (int i = 1; i < 9; i++)
        {
            s += g[i] / (x + i);
        }

        double t = x + 7.5;
        return 0.5 * Math.Log(2 * Math.PI) + (x + 0.5) * Math.Log(t) - t + Math.Log(s);
    }
}
