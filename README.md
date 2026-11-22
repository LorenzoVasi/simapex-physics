# SimApex Physics

Calibrate a sim-racing rig so that what you feel matches real-car physics, using measured data rather than presets.

> Status: early prototype (Phase C). Not ready for end users.

**[Project site](https://lorenzovasi.github.io/simapex-physics/)** · [Quickstart](docs/quickstart.md) · [Documentation rules](docs/documentation-rules.md)

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
| `src/SimApex.Core` | Quantities with units, uncertainty and data provenance |
| `src/SimApex.Analysis` | Savitzkyâ€“Golay differentiation, base identification (J, b, T_c), game-to-base scale and delay, logarithmic decrement |
| `src/SimApex.Sim.Lmu` | Read-only probe of LMU / rF2 shared memory |
| `src/SimApex.Hardware` | Read-only HID discovery of Fanatec and Thrustmaster devices |
| `tools/SimApex.Diagnostics` | Read-only diagnostics CLI (`hid`, `hid-watch`, `lmu`, `lmu-dump`, `report`) |
| `tools/SimApex.Tune` | Guided tuning (Italian UI): T0 baseline laps, T1 FFB gain vs saturation, T2 hands-off stability, T3 brake/ABS, report with confidence intervals; `--replay <dir>` analyses recorded sessions |
| `tests/SimApex.Tests` | Tests against simulated data with known parameters |

## Build

Requires the .NET 10 SDK on Windows.

```powershell
dotnet test SimApex.slnx
dotnet run --project tools/SimApex.Diagnostics -- report
```

## License

MIT, see [LICENSE](LICENSE).

## Disclaimer

Not affiliated with Fanatec, Thrustmaster, Motorsport Games or Studio 397. Product names belong to their owners.
