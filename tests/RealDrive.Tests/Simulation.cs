namespace RealDrive.Tests;

/// <summary>Synthetic wheel dynamics with known parameters, used as ground truth.</summary>
internal static class Simulation
{
    /// <summary>
    /// Integrates J θ̈ = T(t) − b θ̇ − T_c sgn θ̇ with a smooth friction approximation (tanh) and fixed-step RK4.
    /// </summary>
    public static double[] WheelAngle(Func<double, double> torque, double j, double b, double tc, double dt, int samples)
    {
        var angle = new double[samples];
        double theta = 0, omega = 0;
        const double frictionSmoothing = 0.02; // rad/s

        double Accel(double t, double w) => (torque(t) - b * w - tc * Math.Tanh(w / frictionSmoothing)) / j;

        for (int i = 0; i < samples; i++)
        {
            angle[i] = theta;
            double t = i * dt;
            double k1w = Accel(t, omega), k1t = omega;
            double k2w = Accel(t + dt / 2, omega + dt / 2 * k1w), k2t = omega + dt / 2 * k1w;
            double k3w = Accel(t + dt / 2, omega + dt / 2 * k2w), k3t = omega + dt / 2 * k2w;
            double k4w = Accel(t + dt, omega + dt * k3w), k4t = omega + dt * k3w;
            omega += dt / 6 * (k1w + 2 * k2w + 2 * k3w + k4w);
            theta += dt / 6 * (k1t + 2 * k2t + 2 * k3t + k4t);
        }

        return angle;
    }

    /// <summary>
    /// Excitation for test P1: alternating torque steps of two amplitudes, so that both directions of
    /// motion and two torque levels are present.
    /// </summary>
    public static double AlternatingSteps(double t, double low, double high, double period)
    {
        int step = (int)Math.Floor(t / period);
        double sign = step % 2 == 0 ? 1 : -1;
        double amplitude = (step / 2) % 2 == 0 ? low : high;
        return sign * amplitude;
    }

    public static double[] AddNoise(double[] signal, double sigma, int seed)
    {
        var rng = new Random(seed);
        var result = new double[signal.Length];
        for (int i = 0; i < signal.Length; i++)
        {
            // Box–Muller
            double u1 = 1.0 - rng.NextDouble(), u2 = rng.NextDouble();
            result[i] = signal[i] + sigma * Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        return result;
    }
}
