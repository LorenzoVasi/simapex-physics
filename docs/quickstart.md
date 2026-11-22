# Quickstart

Five minutes from clone to your first diagnostics report.

> SimApex Physics is an early prototype. Today it can identify a wheel base model from recorded data (tested on simulated data) and inspect your hardware and Le Mans Ultimate in **read-only** mode. It does not change any setting yet.

## 1. Requirements

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Git
- Optional: Le Mans Ultimate, a Fanatec wheel base, Thrustmaster or Fanatec pedals

## 2. Clone and build

```powershell
git clone https://github.com/LorenzoVasi/simapex-physics.git
cd simapex-physics
dotnet build SimApex.slnx
```

## 3. Run the tests

```powershell
dotnet test SimApex.slnx
```

You should see `13` tests passing. They check, on simulated data with known parameters, that the analysis recovers:
- the base's inertia, damping and friction;
- the game-to-base gain and delay;
- the oscillation damping ratio.

## 4. Inspect your rig

Switch on the wheel base. Start LMU and get on track, even just in the pits. Then run:

```powershell
dotnet run --project tools/SimApex.Diagnostics -- report
```

It lists the Fanatec and Thrustmaster HID devices it finds and the LMU shared-memory mappings it can see. It also saves `simapex-diagnostics.txt`.

Other commands:

| Command | What it does |
|---|---|
| `hid` | List Fanatec and Thrustmaster HID interfaces |
| `hid-watch <index> [s]` | Print raw input reports of one interface when they change |
| `lmu` | Check which LMU mappings exist and whether they are live |
| `lmu-dump <name> <file>` | Copy one mapping to a file for layout analysis |

All of them are read-only. They send no force feedback and write nothing to devices or game files.

## Troubleshooting

| Symptom | Cause and fix |
|---|---|
| `FileLoadException 0x800711C7` ("an application control policy has blocked this file") | Windows Smart App Control blocks unsigned local builds and has no exclusion list. Turn it off in Windows Security → App & browser control, or build on a machine without it. |
| No Fanatec device listed | The base is off or unplugged, or Fanatec drivers are missing. |
| LMU mappings "not found" | The game is not running or you are not on track yet. |
| A device is listed as "not in the known-ID table" | Open an issue with its VID and PID. |

## Next

- Read the [documentation rules](documentation-rules.md) before contributing.
- See the [project site](https://lorenzovasi.github.io/simapex-physics/) for the big picture.
