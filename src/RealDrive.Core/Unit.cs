namespace RealDrive.Core;

/// <summary>
/// SI units used inside the domain. Conversions from other units happen only at the
/// edges (adapters), through <see cref="Units"/>.
/// </summary>
public enum Unit
{
    Dimensionless,
    Newton,
    NewtonMetre,
    Pascal,
    Metre,
    Second,
    Hertz,
    Radian,
    RadianPerSecond,
    RadianPerSecondSquared,
    MetrePerSecond,
    MetrePerSecondSquared,
    Kilogram,
    KilogramMetreSquared,
    NewtonMetreSecondPerRadian,
}

/// <summary>Edge conversions into SI.</summary>
public static class Units
{
    public const double StandardGravity = 9.80665; // m/s^2, exact by definition

    public static double KilogramForceToNewton(double kgf) => kgf * StandardGravity;

    public static double NewtonToKilogramForce(double newton) => newton / StandardGravity;

    public static double BarToPascal(double bar) => bar * 1e5;

    public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

    public static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;

    public static double KmhToMetresPerSecond(double kmh) => kmh / 3.6;

    /// <summary>Tangential force at the rim for a given torque and rim diameter.</summary>
    public static double RimForceNewton(double torqueNm, double rimDiameterMetres)
    {
        if (rimDiameterMetres <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rimDiameterMetres), "Diameter must be positive.");
        }

        return torqueNm / (rimDiameterMetres / 2.0);
    }
}
