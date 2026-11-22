namespace SimApex.Core;

/// <summary>
/// Origin of a numeric value. The UI must never show a value without its kind:
/// a manufacturer claim or a model estimate is never presented as a measurement.
/// </summary>
public enum DataKind
{
    /// <summary>Measured with a documented method and test conditions.</summary>
    Measurement,

    /// <summary>Stated by a manufacturer or vendor without a published method.</summary>
    Declaration,

    /// <summary>Computed by a physical model from stated assumptions.</summary>
    Estimate,

    /// <summary>Read from a simulator: the output of the game's model, not of a real car.</summary>
    SimTelemetry,

    /// <summary>Entered by the user (for example a value read from the Fanatec App).</summary>
    UserInput,
}
