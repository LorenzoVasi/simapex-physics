namespace SimApex.Analysis;

/// <summary>Small dense linear algebra for the few-parameter fits used here (n &lt;= 10).</summary>
internal static class DenseSolver
{
    /// <summary>Solves A x = b by Gaussian elimination with partial pivoting. A and b are not modified.</summary>
    public static double[] Solve(double[,] a, double[] b)
    {
        int n = b.Length;
        var m = (double[,])a.Clone();
        var x = (double[])b.Clone();

        for (int col = 0; col < n; col++)
        {
            int pivot = col;
            for (int row = col + 1; row < n; row++)
            {
                if (Math.Abs(m[row, col]) > Math.Abs(m[pivot, col]))
                {
                    pivot = row;
                }
            }

            if (Math.Abs(m[pivot, col]) < 1e-300)
            {
                throw new InvalidOperationException("Singular system: the excitation does not identify all parameters.");
            }

            if (pivot != col)
            {
                for (int k = 0; k < n; k++)
                {
                    (m[col, k], m[pivot, k]) = (m[pivot, k], m[col, k]);
                }

                (x[col], x[pivot]) = (x[pivot], x[col]);
            }

            for (int row = col + 1; row < n; row++)
            {
                double f = m[row, col] / m[col, col];
                if (f == 0)
                {
                    continue;
                }

                for (int k = col; k < n; k++)
                {
                    m[row, k] -= f * m[col, k];
                }

                x[row] -= f * x[col];
            }
        }

        for (int row = n - 1; row >= 0; row--)
        {
            double s = x[row];
            for (int k = row + 1; k < n; k++)
            {
                s -= m[row, k] * x[k];
            }

            x[row] = s / m[row, row];
        }

        return x;
    }

    /// <summary>Inverse of a small square matrix, column by column.</summary>
    public static double[,] Inverse(double[,] a)
    {
        int n = a.GetLength(0);
        var inv = new double[n, n];
        for (int c = 0; c < n; c++)
        {
            var e = new double[n];
            e[c] = 1.0;
            double[] col = Solve(a, e);
            for (int r = 0; r < n; r++)
            {
                inv[r, c] = col[r];
            }
        }

        return inv;
    }
}
