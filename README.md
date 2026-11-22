# SimApex Physics

Calibrate a sim-racing rig so that what you feel matches real-car physics, using measured data rather than presets.

> Status: early prototype (Phase C). Not ready for end users.

## Principles

- **No invented data.** Every value carries its source, its uncertainty and its kind: measurement, manufacturer declaration, estimate, simulator telemetry or user input.
- **Identify, don't guess.** The wheel base's inertia, damping and friction, and the scale and delay between game torque and delivered torque, are fitted from recorded tests, with confidence intervals.
- **Safe by design.** Torque stays within user limits. Settings are backed up before any change. Device protections are never disabled. No firmware changes or invasive methods.

## Target setup (first)

- Simulator: Le Mans Ultimate (LMU shared memory)
- Wheel base: Fanatec CSL DD
- Pedals: Thrustmaster T-LCM
- Car class: LMGT3

## Layout

| Project | Purpose |
|---|---|
| `src/RealDrive.Core` | Quantities with units, uncertainty and data provenance |
| `src/RealDrive.Analysis` | Savitzky–Golay differentiation, base identification (J, b, T_c), game-to-base scale and delay, logarithmic decrement |
| `src/RealDrive.Sim.Lmu` | Read-only probe of LMU / rF2 shared memory |
| `src/RealDrive.Hardware` | Read-only HID discovery of Fanatec and Thrustmaster devices |
| `tools/RealDrive.Diagnostics` | Read-only diagnostics CLI (`hid`, `hid-watch`, `lmu`, `lmu-dump`, `report`) |
| `tests/RealDrive.Tests` | Tests against simulated data with known parameters |

## Build

Requires the .NET 10 SDK on Windows.

```powershell
dotnet test RealDrive.slnx
dotnet run --project tools/RealDrive.Diagnostics -- report
```

## Disclaimer

Not affiliated with Fanatec, Thrustmaster, Motorsport Games or Studio 397. Product names belong to their owners.
