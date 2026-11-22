using HidSharp;

namespace RealDrive.Hardware;

/// <summary>One HID interface found on the system.</summary>
public sealed record HidInterfaceInfo(
    int VendorId,
    int ProductId,
    string Vendor,
    string? ProductName,
    string? Manufacturer,
    int MaxInputReportLength,
    string DevicePath,
    KnownDevice? Known);

/// <summary>Read-only enumeration of Fanatec and Thrustmaster HID interfaces.</summary>
public static class HidScanner
{
    public static IReadOnlyList<HidInterfaceInfo> Scan(bool relevantVendorsOnly = true)
    {
        var result = new List<HidInterfaceInfo>();
        foreach (HidDevice d in DeviceList.Local.GetHidDevices())
        {
            if (relevantVendorsOnly && !KnownDevices.IsRelevantVendor(d.VendorID))
            {
                continue;
            }

            result.Add(new HidInterfaceInfo(
                d.VendorID,
                d.ProductID,
                KnownDevices.VendorName(d.VendorID),
                TryGet(d.GetProductName),
                TryGet(d.GetManufacturer),
                TryGetInt(d.GetMaxInputReportLength),
                d.DevicePath,
                KnownDevices.Find(d.VendorID, d.ProductID)));
        }

        return result;
    }

    /// <summary>
    /// Streams raw input reports from one interface, to locate the brake and steering bytes.
    /// Reading input reports does not send anything to the device.
    /// </summary>
    public static void WatchRawReports(string devicePath, TimeSpan duration, Action<double, byte[]> onReport)
    {
        HidDevice device = DeviceList.Local.GetHidDevices().FirstOrDefault(d => d.DevicePath == devicePath)
            ?? throw new InvalidOperationException("Device not found: it may have been disconnected.");

        if (!device.TryOpen(out HidStream stream))
        {
            throw new InvalidOperationException("Cannot open the device for reading (another program may hold it exclusively).");
        }

        using (stream)
        {
            stream.ReadTimeout = 250;
            var buffer = new byte[device.GetMaxInputReportLength()];
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (clock.Elapsed < duration)
            {
                try
                {
                    int n = stream.Read(buffer, 0, buffer.Length);
                    onReport(clock.Elapsed.TotalSeconds, buffer[..n]);
                }
                catch (TimeoutException)
                {
                    // No report in this interval: the axis did not move. Keep waiting.
                }
            }
        }
    }

    private static string? TryGet(Func<string> getter)
    {
        try
        {
            return getter();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            return null;
        }
    }

    private static int TryGetInt(Func<int> getter)
    {
        try
        {
            return getter();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            return -1;
        }
    }
}
