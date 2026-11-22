namespace SimApex.Hardware;

public enum DeviceRole
{
    Unknown,
    WheelBase,
    Pedals,
}

/// <summary>A USB identity with what is known about it and how reliable that knowledge is.</summary>
/// <param name="VendorId">USB vendor ID.</param>
/// <param name="ProductId">USB product ID.</param>
/// <param name="Description">Models that report this ID.</param>
/// <param name="Role">Device role.</param>
/// <param name="Note">Caveat about the identification.</param>
public sealed record KnownDevice(int VendorId, int ProductId, string Description, DeviceRole Role, string? Note = null);

/// <summary>
/// USB IDs from community sources (hid-fanatecff README) plus IDs observed on real setups. They are
/// not an official list, and a model can report more than one PID (hid-fanatecff lists the CSL DD as
/// 0x0020, but one was observed reporting 0x0004), so the app treats them as declarations and asks
/// the user to confirm the exact model.
/// </summary>
public static class KnownDevices
{
    public const int FanatecVendorId = 0x0EB7;
    public const int ThrustmasterVendorId = 0x044F;

    public static IReadOnlyList<KnownDevice> All { get; } =
    [
        new(FanatecVendorId, 0x0020, "CSL DD / GT DD Pro / ClubSport DD", DeviceRole.WheelBase,
            "Same PID for several bases: confirm the model in the Fanatec App"),
        new(FanatecVendorId, 0x0006, "Podium DD1", DeviceRole.WheelBase, "Experimental in hid-fanatecff"),
        new(FanatecVendorId, 0x0007, "Podium DD2", DeviceRole.WheelBase, "Experimental in hid-fanatecff"),
        new(FanatecVendorId, 0x0E03, "CSL Elite wheel base", DeviceRole.WheelBase),
        new(FanatecVendorId, 0x0001, "ClubSport Wheel Base V2", DeviceRole.WheelBase, "Experimental in hid-fanatecff"),
        new(FanatecVendorId, 0x0004, "ClubSport Wheel Base V2.5 / CSL DD", DeviceRole.WheelBase,
            "hid-fanatecff lists it as ClubSport V2.5; a CSL DD in PC mode (orange LED) reported this PID on the user's setup on 2026-11-22 (Fanatec App 1.6.1.3, driver 0.53.1). Confirm the model in the Fanatec App"),
        new(FanatecVendorId, 0x183B, "ClubSport Pedals V3", DeviceRole.Pedals, "Experimental in hid-fanatecff"),
        new(FanatecVendorId, 0x6204, "CSL Elite Pedals", DeviceRole.Pedals),
        new(FanatecVendorId, 0x6205, "CSL Pedals Load Cell", DeviceRole.Pedals, "Experimental in hid-fanatecff"),
        new(ThrustmasterVendorId, 0xB371, "T-LCM (reports as \"Sim Pedals\")", DeviceRole.Pedals,
            "Observed on the user's setup on 2026-11-22; not from a published list"),
    ];

    public static KnownDevice? Find(int vendorId, int productId) =>
        All.FirstOrDefault(d => d.VendorId == vendorId && d.ProductId == productId);

    public static string VendorName(int vendorId) => vendorId switch
    {
        FanatecVendorId => "Fanatec",
        ThrustmasterVendorId => "Thrustmaster",
        _ => $"0x{vendorId:X4}",
    };

    public static bool IsRelevantVendor(int vendorId) => vendorId is FanatecVendorId or ThrustmasterVendorId;
}
