namespace SimApex.Analysis.Tuning;

/// <summary>
/// One telemetry sample as the tuning analyses need it, independent of the simulator.
/// All values are simulator telemetry (DataKind.SimTelemetry), not real-car data.
/// </summary>
/// <param name="Time">Game time, s (strictly increasing; one frame per distinct game update).</param>
/// <param name="Lap">Lap counter from the simulator.</param>
/// <param name="SpeedKmh">Vehicle speed, km/h.</param>
/// <param name="FfbOutput">Force feedback command sent by the game, normalised −1..1.</param>
/// <param name="ShaftTorque">Steering shaft torque from the vehicle model (unit not stated by the LMU SDK).</param>
/// <param name="Steering">Steering input, −1..1 of full lock (left to right).</param>
/// <param name="LatAccelG">Lateral acceleration, g, positive left.</param>
/// <param name="LongAccelG">Longitudinal acceleration, g, positive forward.</param>
/// <param name="Brake">Unfiltered brake input, 0..1.</param>
/// <param name="AbsActive">Whether ABS is intervening.</param>
/// <param name="AbsMap">ABS setting index as reported by the game (not necessarily the number shown on screen).</param>
/// <param name="OnKerb">Whether a front wheel is on a rumble strip.</param>
public readonly record struct TelemetryFrame(
    double Time,
    int Lap,
    double SpeedKmh,
    double FfbOutput,
    double ShaftTorque,
    double Steering,
    double LatAccelG,
    double LongAccelG,
    double Brake,
    bool AbsActive,
    int AbsMap,
    bool OnKerb);
