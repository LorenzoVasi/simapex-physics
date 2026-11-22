using SimApex.Core;

namespace SimApex.Tests;

public class QuantityTests
{
    [Fact]
    public void Value_outside_interval_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => Quantity.WithRange(5, 6, 7, Unit.NewtonMetre, DataKind.Estimate));
    }

    [Fact]
    public void Overlap_requires_same_unit()
    {
        var a = Quantity.WithTolerance(8, 0.5, Unit.NewtonMetre, DataKind.Declaration);
        var b = Quantity.WithTolerance(8.3, 0.2, Unit.NewtonMetre, DataKind.Measurement);
        var c = Quantity.Exact(8, Unit.Newton, DataKind.Measurement);

        Assert.True(a.OverlapsWith(b));
        Assert.Throws<InvalidOperationException>(() => a.OverlapsWith(c));
    }

    [Fact]
    public void Negative_scale_keeps_interval_ordered()
    {
        var q = Quantity.WithRange(2, 1, 4, Unit.NewtonMetre, DataKind.Estimate).Scale(-1);
        Assert.Equal(-4, q.Min);
        Assert.Equal(-1, q.Max);
    }
}

public class UnitsTests
{
    [Theory]
    [InlineData(8.0, 0.300, 53.333)]
    [InlineData(9.0, 0.300, 60.000)]
    public void Rim_force_for_the_McLaren_GT3_V2(double torque, double diameter, double expected)
    {
        Assert.Equal(expected, Units.RimForceNewton(torque, diameter), 3);
    }

    [Fact]
    public void Kilogram_force_uses_standard_gravity()
    {
        Assert.Equal(980.665, Units.KilogramForceToNewton(100), 6);
    }
}
