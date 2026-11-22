namespace SimApex.Analysis;

/// <summary>
/// Savitzky–Golay smoothing differentiator for uniformly sampled signals.
/// Samples closer than half a window to either end are returned as NaN rather than
/// extrapolated, so downstream fits can skip them explicitly.
/// </summary>
public sealed class SavitzkyGolay
{
    private readonly double[] _coefficients;

    public SavitzkyGolay(int halfWindow, int polynomialOrder, int derivative)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(halfWindow, 1);

        if (polynomialOrder < derivative || polynomialOrder >= 2 * halfWindow + 1)
        {
            throw new ArgumentOutOfRangeException(nameof(polynomialOrder), "Need derivative <= order < window length.");
        }

        HalfWindow = halfWindow;
        Derivative = derivative;
        _coefficients = ComputeCoefficients(halfWindow, polynomialOrder, derivative);
    }

    public int HalfWindow { get; }

    public int Derivative { get; }

    /// <summary>Applies the filter. <paramref name="dt"/> is the sample period in seconds.</summary>
    public double[] Apply(ReadOnlySpan<double> signal, double dt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dt);

        int n = signal.Length;
        var result = new double[n];
        double scale = 1.0 / Math.Pow(dt, Derivative);
        int m = HalfWindow;

        for (int i = 0; i < n; i++)
        {
            if (i < m || i >= n - m)
            {
                result[i] = double.NaN;
                continue;
            }

            double acc = 0;
            for (int j = -m; j <= m; j++)
            {
                acc += _coefficients[j + m] * signal[i + j];
            }

            result[i] = acc * scale;
        }

        return result;
    }

    private static double[] ComputeCoefficients(int m, int order, int derivative)
    {
        int width = 2 * m + 1;
        int p = order + 1;

        // Normal matrix (A^T A) for the Vandermonde design A[j, k] = x_j^k, x_j = -m..m.
        var ata = new double[p, p];
        for (int r = 0; r < p; r++)
        {
            for (int c = 0; c < p; c++)
            {
                double s = 0;
                for (int x = -m; x <= m; x++)
                {
                    s += Math.Pow(x, r + c);
                }

                ata[r, c] = s;
            }
        }

        var inv = DenseSolver.Inverse(ata);
        double factorial = 1;
        for (int k = 2; k <= derivative; k++)
        {
            factorial *= k;
        }

        // Coefficient for sample x_j: d! * sum_k inv[d, k] * x_j^k
        var coefficients = new double[width];
        for (int x = -m; x <= m; x++)
        {
            double s = 0;
            for (int k = 0; k < p; k++)
            {
                s += inv[derivative, k] * Math.Pow(x, k);
            }

            coefficients[x + m] = factorial * s;
        }

        return coefficients;
    }
}
