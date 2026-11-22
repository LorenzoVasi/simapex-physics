using SimApex.Sim.Lmu.Sdk;

namespace SimApex.Sim.Lmu;

/// <summary>How a channel value was obtained.</summary>
public enum ChannelOrigin
{
    /// <summary>Read directly from an SDK field.</summary>
    SdkField,

    /// <summary>Computed from SDK fields with a stated formula.</summary>
    Derived,
}

/// <summary>One decoded telemetry channel.</summary>
/// <param name="Name">Column name.</param>
/// <param name="Unit">Unit as stated by the SDK header comment, or "unstated" when the SDK gives none.</param>
/// <param name="Origin">SDK field or derived.</param>
/// <param name="Source">SDK field path(s) or the formula.</param>
public sealed record TelemetryChannel(string Name, string Unit, ChannelOrigin Origin, string Source);

/// <summary>
/// Decodes the player's car from an LMU shared memory snapshot, with every field located through the
/// SDK layout (<see cref="LmuSdk"/>), not hard-coded offsets.
/// <para>
/// LMU vehicle axes (SDK header): +x left, +y up, +z rearward. Channels marked derived convert these to
/// forward/left-positive values and state the formula.
/// </para>
/// </summary>
public sealed class LmuTelemetryDecoder
{
    private const double G = 9.80665;

    private static readonly (string Name, string Unit, string Field)[] VehicleFields =
    [
        ("game_elapsed_s", "s", "mElapsedTime"),
        ("game_delta_s", "s", "mDeltaTime"),
        ("lap", "count", "mLapNumber"),
        ("gear", "gear (-1 R, 0 N)", "mGear"),
        ("engine_rpm", "rpm", "mEngineRPM"),
        ("throttle_raw", "0..1", "mUnfilteredThrottle"),
        ("brake_raw", "0..1", "mUnfilteredBrake"),
        ("steering_raw", "-1..1 (left to right)", "mUnfilteredSteering"),
        ("clutch_raw", "0..1", "mUnfilteredClutch"),
        ("throttle_filtered", "0..1", "mFilteredThrottle"),
        ("brake_filtered", "0..1", "mFilteredBrake"),
        ("steering_filtered", "-1..1 (left to right)", "mFilteredSteering"),
        ("steering_shaft_torque", "unstated (SDK: torque around steering shaft)", "mSteeringShaftTorque"),
        ("physical_wheel_range", "unstated (SDK: physical steering wheel range)", "mPhysicalSteeringWheelRange"),
        ("visual_wheel_range", "unstated (SDK: visual steering wheel range)", "mVisualSteeringWheelRange"),
        ("abs_active", "bool", "mABSActive"),
        ("tc_active", "bool", "mTCActive"),
        ("abs_setting", "index", "mABS"),
        ("abs_setting_max", "index", "mABSMax"),
        ("tc_setting", "index", "mTC"),
        ("rear_brake_bias", "fraction on rear", "mRearBrakeBias"),
        ("front_downforce", "unstated (SDK: front downforce)", "mFrontDownforce"),
        ("rear_downforce", "unstated (SDK: rear downforce)", "mRearDownforce"),
        ("front_ride_height", "unstated (SDK: front ride height)", "mFrontRideHeight"),
        ("rear_ride_height", "unstated (SDK: rear ride height)", "mRearRideHeight"),
        ("fuel_l", "l", "mFuel"),
        ("local_vel_x", "m/s (+x left)", "mLocalVel.x"),
        ("local_vel_y", "m/s (+y up)", "mLocalVel.y"),
        ("local_vel_z", "m/s (+z rearward)", "mLocalVel.z"),
        ("local_accel_x", "m/s^2 (+x left)", "mLocalAccel.x"),
        ("local_accel_y", "m/s^2 (+y up)", "mLocalAccel.y"),
        ("local_accel_z", "m/s^2 (+z rearward)", "mLocalAccel.z"),
        ("local_rot_y", "rad/s (+y yaws right)", "mLocalRot.y"),
    ];

    private static readonly (string Name, string Unit, string Field)[] WheelFields =
    [
        ("brake_pressure", "0..1 (SDK: depends on driver input and brake balance)", "mBrakePressure"),
        ("rotation", "rad/s", "mRotation"),
        ("tire_load", "N", "mTireLoad"),
        ("lateral_force", "N", "mLateralForce"),
        ("longitudinal_force", "N", "mLongitudinalForce"),
        ("grip_fract", "fraction of contact patch sliding (SDK approximation)", "mGripFract"),
        ("susp_deflection", "m", "mSuspensionDeflection"),
        ("brake_temp", "SDK says Celsius; LMU data (min 291.15 at ambient) looks like Kelvin", "mBrakeTemp"),
        ("surface_type", "0 dry,1 wet,2 grass,3 dirt,4 gravel,5 rumblestrip,6 special", "mSurfaceType"),
    ];

    private static readonly string[] WheelNames = ["fl", "fr", "rl", "rr"];

    private readonly LmuSdk _sdk;
    private readonly ResolvedField _playerIndex;
    private readonly ResolvedField _playerHasVehicle;
    private readonly ResolvedField _activeVehicles;
    private readonly ResolvedField _ffbTorque;
    private readonly Dictionary<int, ResolvedField[]> _perVehicle = [];

    public LmuTelemetryDecoder(LmuSdk sdk)
    {
        ArgumentNullException.ThrowIfNull(sdk);
        _sdk = sdk;
        _playerIndex = sdk.Resolve("telemetry.playerVehicleIdx");
        _playerHasVehicle = sdk.Resolve("telemetry.playerHasVehicle");
        _activeVehicles = sdk.Resolve("telemetry.activeVehicles");
        _ffbTorque = sdk.Resolve("generic.FFBTorque");

        var channels = new List<TelemetryChannel>
        {
            new("ffb_torque", "unstated (SDK: SharedMemoryGeneric.FFBTorque)", ChannelOrigin.SdkField, "generic.FFBTorque"),
        };
        channels.AddRange(VehicleFields.Select(f => new TelemetryChannel(f.Name, f.Unit, ChannelOrigin.SdkField, "telemInfo[p]." + f.Field)));
        foreach (string w in WheelNames)
        {
            channels.AddRange(WheelFields.Select(f => new TelemetryChannel($"{f.Name}_{w}", f.Unit, ChannelOrigin.SdkField, $"telemInfo[p].mWheel[{Array.IndexOf(WheelNames, w)}].{f.Field}")));
        }

        channels.Add(new("speed_kmh", "km/h", ChannelOrigin.Derived, "3.6 * |mLocalVel|"));
        channels.Add(new("lat_accel_g", "g (+left)", ChannelOrigin.Derived, "mLocalAccel.x / 9.80665"));
        channels.Add(new("long_accel_g", "g (+forward)", ChannelOrigin.Derived, "-mLocalAccel.z / 9.80665"));
        channels.Add(new("yaw_rate_deg_s", "deg/s (+right)", ChannelOrigin.Derived, "mLocalRot.y * 180/pi"));
        channels.Add(new("steering_wheel_deg", "deg (assumes physical range is lock-to-lock in degrees; LMU 2026 reports 0, so this is 0)", ChannelOrigin.Derived, "mUnfilteredSteering * mPhysicalSteeringWheelRange / 2"));
        Channels = channels;
    }

    public IReadOnlyList<TelemetryChannel> Channels { get; }

    /// <summary>Player slot index, or -1 when the player has no vehicle.</summary>
    public int PlayerIndex(ReadOnlySpan<byte> memory) =>
        LmuSdk.ReadNumber(memory, _playerHasVehicle) != 0 ? (int)LmuSdk.ReadNumber(memory, _playerIndex) : -1;

    public int ActiveVehicles(ReadOnlySpan<byte> memory) => (int)LmuSdk.ReadNumber(memory, _activeVehicles);

    public string VehicleName(ReadOnlySpan<byte> memory, int index) => LmuSdk.ReadString(memory, _sdk.Resolve($"telemetry.telemInfo[{index}].mVehicleName"));

    public string TrackName(ReadOnlySpan<byte> memory, int index) => LmuSdk.ReadString(memory, _sdk.Resolve($"telemetry.telemInfo[{index}].mTrackName"));

    /// <summary>Fills <paramref name="values"/> (length = <see cref="Channels"/>.Count) for vehicle slot <paramref name="index"/>.</summary>
    public void Decode(ReadOnlySpan<byte> memory, int index, Span<double> values)
    {
        if (values.Length != Channels.Count)
        {
            throw new ArgumentException("Wrong buffer length.", nameof(values));
        }

        var fields = FieldsFor(index);
        values[0] = LmuSdk.ReadNumber(memory, _ffbTorque);
        for (int k = 0; k < fields.Length; k++)
        {
            values[k + 1] = LmuSdk.ReadNumber(memory, fields[k]);
        }

        int d = fields.Length + 1;
        double vx = Get("local_vel_x", values), vy = Get("local_vel_y", values), vz = Get("local_vel_z", values);
        values[d] = 3.6 * Math.Sqrt(vx * vx + vy * vy + vz * vz);
        values[d + 1] = Get("local_accel_x", values) / G;
        values[d + 2] = -Get("local_accel_z", values) / G;
        values[d + 3] = Get("local_rot_y", values) * 180 / Math.PI;
        values[d + 4] = Get("steering_raw", values) * Get("physical_wheel_range", values) / 2;
    }

    private double Get(string name, Span<double> values) => values[IndexOf(name)];

    private readonly Dictionary<string, int> _indexCache = [];

    public int IndexOf(string name)
    {
        if (!_indexCache.TryGetValue(name, out int i))
        {
            i = Channels.ToList().FindIndex(c => c.Name == name);
            _indexCache[name] = i >= 0 ? i : throw new KeyNotFoundException(name);
        }

        return i;
    }

    private ResolvedField[] FieldsFor(int index)
    {
        if (_perVehicle.TryGetValue(index, out var cached))
        {
            return cached;
        }

        string v = $"telemetry.telemInfo[{index}]";
        var list = VehicleFields.Select(f => _sdk.Resolve($"{v}.{f.Field}")).ToList();
        for (int w = 0; w < 4; w++)
        {
            list.AddRange(WheelFields.Select(f => _sdk.Resolve($"{v}.mWheel[{w}].{f.Field}")));
        }

        var array = list.ToArray();
        _perVehicle[index] = array;
        return array;
    }
}
