using System.Globalization;

namespace SimApex.Core;

/// <summary>
/// A physical value with its SI unit, an uncertainty interval and its origin.
/// </summary>
/// <param name="Value">Best value, in SI units.</param>
/// <param name="Unit">SI unit of <paramref name="Value"/>, <paramref name="Min"/> and <paramref name="Max"/>.</param>
/// <param name="Min">Lower bound of the uncertainty interval.</param>
/// <param name="Max">Upper bound of the uncertainty interval.</param>
/// <param name="Kind">Where the value comes from.</param>
public readonly record struct Quantity(double Value, Unit Unit, double Min, double Max, DataKind Kind)
{
    public static Quantity Exact(double value, Unit unit, DataKind kind) => new(value, unit, value, value, kind);

    public static Quantity WithRange(double value, double min, double max, Unit unit, DataKind kind)
    {
        if (min > value || value > max)
        {
            throw new ArgumentException($"Value {value} is outside its interval [{min}, {max}].");
        }

        return new Quantity(value, unit, min, max, kind);
    }

    public static Quantity WithTolerance(double value, double plusMinus, Unit unit, DataKind kind)
    {
        if (plusMinus < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(plusMinus), "Tolerance must be non-negative.");
        }

        return new Quantity(value, unit, value - plusMinus, value + plusMinus, kind);
    }

    public double HalfWidth => (Max - Min) / 2.0;

    public bool IsExact => Min == Max;

    /// <summary>True when the two intervals overlap (compatible within uncertainty). Units must match.</summary>
    public bool OverlapsWith(Quantity other)
    {
        EnsureSameUnit(other);
        return Min <= other.Max && other.Min <= Max;
    }

    public Quantity Scale(double factor)
    {
        double a = Min * factor;
        double b = Max * factor;
        return new Quantity(Value * factor, Unit, Math.Min(a, b), Math.Max(a, b), Kind);
    }

    private void EnsureSameUnit(Quantity other)
    {
        if (other.Unit != Unit)
        {
            throw new InvalidOperationException($"Unit mismatch: {Unit} vs {other.Unit}.");
        }
    }

    public override string ToString() => IsExact
        ? string.Create(CultureInfo.InvariantCulture, $"{Value:G6} {Unit} ({Kind})")
        : string.Create(CultureInfo.InvariantCulture, $"{Value:G6} [{Min:G6}, {Max:G6}] {Unit} ({Kind})");
}
