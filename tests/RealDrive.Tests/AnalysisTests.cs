using RealDrive.Analysis;

namespace RealDrive.Tests;

public class SavitzkyGolayTests
{
    [Fact]
    public void Derivatives_of_a_cubic_are_exact()
    {
        const double dt = 0.001;
        var x = Enumerable.Range(0, 400).Select(i => i * dt).ToArray();
        var y = x.Select(t => 2 * t * t * t - t * t + 3 * t).ToArray();

        var d1 = new SavitzkyGolay(10, 3, 1).Apply(y, dt);
        var d2 = new SavitzkyGolay(10, 3, 2).Apply(y, dt);

        for (int i = 10; i < 390; i++)
        {
            double t = x[i];
            Assert.Equal(6 * t * t - 2 * t + 3, d1[i], 6);
            Assert.Equal(12 * t - 2, d2[i], 4);
        }
    }

    [Fact]
    public void Edges_are_NaN_not_extrapolated()
    {
        var d = new SavitzkyGolay(5, 2, 1).Apply(new double[50], 0.01);
        Assert.True(double.IsNaN(d[0]));
        Assert.True(double.IsNaN(d[49]));
        Assert.False(double.IsNaN(d[25]));
    }
}

public class BaseIdentificationTests
{
    private const double Dt = 0.001;
    private const double J = 0.045, B = 0.06, Tc = 0.12;

    [Fact]
    public void Recovers_inertia_damping_and_friction_from_alternating_steps()
    {
        double Torque(double t) => Simulation.AlternatingSteps(t, 1.0, 1.6, 0.25);
        var torque = Enumerable.Range(0, 4000).Select(i => Torque(i * Dt)).ToArray();
        var angle = Simulation.WheelAngle(Torque, J, B, Tc, Dt, 4000);
        var noisy = Simulation.AddNoise(angle, 2e-6, seed: 1);

        var p = BaseIdentification.Fit(torque, noisy, Dt);

        Assert.InRange(p.Inertia.Value, J * 0.95, J * 1.05);
        Assert.InRange(p.ViscousDamping.Value, B * 0.90, B * 1.10);
        Assert.InRange(p.CoulombFriction.Value, Tc * 0.90, Tc * 1.10);
    }

    [Fact]
    public void Single_step_in_one_direction_is_rejected_as_not_identifiable()
    {
        double Torque(double t) => 1.0;
        var torque = Enumerable.Repeat(1.0, 1000).ToArray();
        var angle = Simulation.WheelAngle(Torque, J, B, Tc, Dt, 1000);

        // Collinear regressors: either the solver refuses, or the estimate is far from the truth.
        try
        {
            var p = BaseIdentification.Fit(torque, angle, Dt);
            bool farOff = Math.Abs(p.Inertia.Value - J) > 0.05 * J
                          || Math.Abs(p.ViscousDamping.Value - B) > 0.1 * B
                          || p.Inertia.HalfWidth > 0.05 * J;
            Assert.True(farOff, "A one-direction step must not look like a valid identification.");
        }
        catch (InvalidOperationException)
        {
            // Expected: singular system.
        }
    }
}

public class GameToBaseScaleTests
{
    [Fact]
    public void Recovers_gain_and_delay()
    {
        const double dt = 0.001, j = 0.045, b = 0.06, tc = 0.12, gain = 0.7, delay = 0.012;
        const int n = 6000;

        // Game torque: a few sinusoids, like a hands-off wheel being pulled around by the car model.
        double Game(double t) => 2.0 * Math.Sin(2 * Math.PI * 0.8 * t) + 0.8 * Math.Sin(2 * Math.PI * 2.3 * t + 1);
        double Delivered(double t) => gain * Game(t - delay);

        var game = Enumerable.Range(0, n).Select(i => Game(i * dt)).ToArray();
        var angle = Simulation.WheelAngle(Delivered, j, b, tc, dt, n);
        var parameters = new BaseParameters(
            RealDrive.Core.Quantity.Exact(j, RealDrive.Core.Unit.KilogramMetreSquared, RealDrive.Core.DataKind.Measurement),
            RealDrive.Core.Quantity.Exact(b, RealDrive.Core.Unit.NewtonMetreSecondPerRadian, RealDrive.Core.DataKind.Measurement),
            RealDrive.Core.Quantity.Exact(tc, RealDrive.Core.Unit.NewtonMetre, RealDrive.Core.DataKind.Measurement),
            0,
            n);

        var est = GameToBaseScale.Estimate(game, angle, dt, parameters);

        Assert.InRange(est.Gain, gain * 0.95, gain * 1.05);
        Assert.InRange(est.DelaySeconds, delay - 0.002, delay + 0.002);
        Assert.True(est.ExplainedVariance > 0.95);
    }
}

public class LogDecrementTests
{
    [Fact]
    public void Recovers_damping_ratio_and_frequency()
    {
        const double dt = 0.001, zeta = 0.15, fn = 4.0;
        double wn = 2 * Math.PI * fn, wd = wn * Math.Sqrt(1 - zeta * zeta);
        var x = Enumerable.Range(0, 3000)
            .Select(i => i * dt)
            .Select(t => Math.Exp(-zeta * wn * t) * Math.Cos(wd * t))
            .ToArray();

        var est = LogDecrement.Estimate(x, dt);

        Assert.NotNull(est);
        Assert.InRange(est!.DampingRatio, zeta * 0.97, zeta * 1.03);
        Assert.InRange(est.NaturalFrequencyHz, fn * 0.98, fn * 1.02);
    }

    [Fact]
    public void Returns_null_without_oscillation()
    {
        var x = Enumerable.Range(0, 500).Select(i => Math.Exp(-i * 0.01)).ToArray();
        Assert.Null(LogDecrement.Estimate(x, 0.001));
    }
}
