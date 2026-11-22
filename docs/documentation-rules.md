# Documentation rules

SimApex Physics exists to replace guesswork with measured, traceable numbers. Documentation is held to the same standard as the code: if a reader cannot tell where a value came from, the value is not documented.

## 1. Every number has a passport

Any physical value written in code, docs, tests or the UI states five things:

| Field | Rule | Example |
|---|---|---|
| Value | Best estimate | `8` |
| Unit | SI unit, or the original unit plus its SI conversion | `N·m` |
| Uncertainty | Range or ± with the coverage factor, or "unknown" | `±0.5 N·m (k = 2)` |
| Kind | `Measurement`, `Declaration`, `Estimate`, `SimTelemetry` or `UserInput` | `Declaration` |
| Source | Link or reference, plus the date it was read | Fanatec product page, read 2026-11-15 |

In code this is a `Quantity` with a `DataKind`. In prose write it inline: *8 N·m peak torque (declaration, Fanatec product page, read 2026-11-15; uncertainty unknown)*.

**Never** write a bare number for a physical quantity. **Never** fill a gap with a plausible-looking value. Write "unknown" and open an issue instead.

## 2. Rate the source

| Level | Meaning | Examples |
|---|---|---|
| A | Peer-reviewed or standard | SAE papers, ISO standards, textbooks |
| B | Manufacturer or developer, official | Datasheets, official docs, developer statements |
| C | Reproducible community work | Open-source drivers, published test logs with method |
| D | Anecdotal | Forum posts, videos, "feels like" reports |

Level D can motivate a test. It can never set a default.

## 3. Units and names

- Use SI internally: m, kg, s, N, N·m, rad, Pa.
- Convert at the edges only: UI, file import and export. Use the helpers in `Units`.
- Put the unit in the name wherever the type does not carry it: `delaySeconds`, `rimDiameterMetres`.
- Angles are in radians in code. Degrees appear only in the UI.

## 4. Code documentation

- Every public type and member gets an XML doc comment that says what it does, its units, and what it returns when there is not enough data (for example `null` or `NaN`).
- Explain *why* in comments, not *what*. Cite the method: "logarithmic decrement, see Inman, *Engineering Vibration*, chapter 1".
- Every estimation routine documents its assumptions and the conditions under which it fails. For example: a single one-direction step makes inertia and friction collinear.

## 5. Safety statements

Any doc or code that drives a device states:
- the maximum torque it can command and where that limit comes from;
- what it backs up before changing anything;
- how to stop it.

Nothing is ever documented as a way to bypass device protections or modify firmware.

## 6. Tests as documentation

Each identification method has a test on simulated data with **known** parameters. The test states the recovery tolerance and why it is acceptable.

## 7. Language and format

- Code, comments and repository docs are in English. User-facing text can be localised.
- Use Markdown, one sentence per idea, tables for comparisons.
- Write dates as ISO 8601 (`2026-11-22`).
- Commit messages are imperative and short ("Add T-LCM calibration wizard"), with a body explaining why when it isn't obvious.
