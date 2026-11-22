using System.Buffers.Binary;

namespace SimApex.Hardware;

/// <summary>Decoded T-LCM input report.</summary>
/// <param name="Throttle">0..1.</param>
/// <param name="Brake">0..1, already scaled by the pedal's Brake Force setting (not a raw load-cell reading).</param>
public readonly record struct TlcmState(double Throttle, double Brake);

/// <summary>
/// Thrustmaster T-LCM (USB VID 0x044F, PID 0xB371) input report layout, identified on the user's pedal set
/// (firmware v2.00) by correlating the raw report with LMU's pedal inputs on 2026-11-22:
/// throttle = little-endian uint16 at bytes 1–2 (correlation 1.000), brake = little-endian uint16 at bytes 5–6
/// (linear with LMU's brake input). This is an observation, not a published Thrustmaster specification.
/// </summary>
public static class TlcmPedals
{
    public const int ProductId = 0xB371;

    public static TlcmState? Decode(ReadOnlySpan<byte> report)
    {
        if (report.Length < 7)
        {
            return null;
        }

        return new TlcmState(
            BinaryPrimitives.ReadUInt16LittleEndian(report[1..]) / 65535.0,
            BinaryPrimitives.ReadUInt16LittleEndian(report[5..]) / 65535.0);
    }

    /// <summary>Device path of the first connected T-LCM, or null.</summary>
    public static string? Find() =>
        HidScanner.Scan().FirstOrDefault(d => d.VendorId == KnownDevices.ThrustmasterVendorId && d.ProductId == ProductId)?.DevicePath;
}
