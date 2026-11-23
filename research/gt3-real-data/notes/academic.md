# University and academic literature on GT3 and race-car steering and braking

Search run: 2026-11-23, web search across university repositories, IEEE, MDPI, arXiv and politesi/webthesis.
Status column uses the §4 rule 10 terms: `not found`, `not accessible`, `abstract/snippet only`, `read`.

## Summary

- **No university paper or thesis found that publishes measured hand-wheel torque (N·m) or brake-pedal force (N) on a homologated GT3.** This confirms findings 1 and 2 of the snapshot. It does not prove that no such work exists: repositories were searched by keyword, not exhaustively.
- GT3-specific academic work exists, but on **aerodynamics** (Ferrari 296 GT3, Huracán GT3 EVO), **setup optimisation in a simulator** (BMW Z4 GT3 in Assetto Corsa, SIMULATED) and **race strategy** (Nordschleife, timing data only).
- The closest work on *force feedback versus a real car* is from **TU Delft** on a single-seater, not a GT3. It cannot be transferred to GT3 (§4 rule 3), but its **method** is directly what SimApex does.

## Sources

| # | Institution, year | Title / author | Car | Relevance to SimApex | Status |
|---|---|---|---|---|---|
| U1 | TU Delft, MSc thesis, 2015-08-27 | R. Advocaat, *Comparing and improving steering forces in a race car and race simulator to increase simulator fidelity* | Real race car (single-seater; the "Formula 3" label comes from secondary snippets, not confirmed) vs rFactor model plus multibody model | **High (method).** Compares measured steering torque in a real race car with an rFactor force-feedback simulator. Snippets say the baseline rFactor model was less sensitive to steering input than the real car, and that the FFB motor limit (reported as 16 N·m, forum quote, unverified) clipped real peaks | not accessible (repository HTTP 500 on 2026-11-23); abstract/snippet only. https://repository.tudelft.nl/file/File_d227fb52-2202-4128-9861-64f8f882f29a?preview=1 |
| U2 | TU Delft, journal paper (Presence, vol. 20, 2011) | S. de Groot, J.C.F. de Winter, M. Mulder, P.A. Wieringa, *Car Racing in a Simulator: Validation and Assessment of Brake Pedal Stiffness* | Formula Student car / simulator | **High for T3 (brake).** Snippets: pedal stiffness soft 5.8 N/mm vs hard 53.0 N/mm; pedal force measured with strain gauges; a stiffer pedal gave higher pedal forces; simulator vs real lap-time correlation r = 0.57 (n = 13). Numbers come from a Scribd copy, so they are level E until the paper is read | not accessible (HTTP 500); snippet only. https://repository.tudelft.nl/record/uuid:349bf68b-3202-40d8-850c-43d1d3dcb927 |
| U3 | TU Delft, conference 2009 | de Groot et al., *The Effect of Brake Pedal Stiffness on Race Car Driving Performance* | as U2 | as U2 | not accessible. https://repository.tudelft.nl/record/uuid:b834b6bc-df74-4870-86ac-f0574e6db7aa |
| U4 | TU Delft, PhD thesis 2013 | S. de Groot, *Exploiting the possibilities of simulators for driver training* | as U2 | Includes U2 and the claim that racing drivers cope with a wide range of pedal stiffness | not accessible. https://repository.tudelft.nl/record/uuid:9778ec06-5bf9-43ad-959b-1f6459d28b21 |
| U5 | TU Delft, IEEE 2021 | *Experimental Validation of Torque-Based Control for Realistic Handwheel Haptics in Driving Simulators* | Generic high-end simulator | Medium. Torque-based steering control reduced oscillation and improved fidelity compared with position-based control (relevant to T2 hands-off stability). Already in `literature_patents_dd_limits.md` | abstract only. https://ieeexplore.ieee.org/document/9606570/ |
| U6 | Politecnico di Milano, MSc 2025 | Uderzo, Ugolini, *Ferrari 296 GT3 aerodynamic characterization through 3D scanning and CFD* (executive summary) | **Ferrari 296 GT3** | Low for steering and brake; possible source for aero balance and downforce, which affect steering load. It states that teams have very little aeromap knowledge in GT3 | not accessible (PDF > 10 MB for the fetcher). https://www.politesi.polimi.it/retrieve/fbe0ee27-9914-4209-a2dd-d4aeed609942/2025_04_Uderzo_Ugolini_ExecutiveSummary.pdf |
| U7 | Nelson Mandela University (South Africa), MEng 2025 | *Aerodynamic profiling of a GT3 racing vehicle using both simulation and component level physical testing* | **Lamborghini Huracán GT3 EVO** | Low/medium: CFD (Altair UltraFluidX) plus wind-tunnel test of a 3D-printed rear wing | not accessible (DNS failure). vital.seals.ac.za record vital:79612 |
| U8 | MDPI *Vehicles* 8(7):146 | *Staged GT3 Setup Optimization with Setup-Conditioned Telemetry Response Modeling in Simulation* | BMW Z4 GT3 in Assetto Corsa | **SIMULATED** (§4 rule 2). Statistical setup search: 134 configurations, best lap 91.430 → 91.040 s at the Red Bull Ring. Method close to SimApex.Tune | not accessible (HTTP 403); snippet only. https://doi.org/10.3390/vehicles8070146 |
| U9 | arXiv 2306.16088, 2023 | M. Boettinger, D. Klotz, *Mastering Nordschleife: a comprehensive race simulation for AI strategy decision-making* | GT racing at the NLS (no car named) | None for physics: strategy and timing only | read (abstract) |
| U10 | Univ. of Cambridge, PhD | T. Niu, *Measurement and Modelling of Human Car Driving with Steering Torque Feedback* | Road car / simulator | Medium: driver model and steering-torque identification | abstract only. https://www.repository.cam.ac.uk/items/a3998366-8f25-422b-93be-dc69f4db0096 |
| U11 | KTH, BSc 2015 | Steering-feel degree project | Road car | Low: steering ratio has the largest effect on steering torque; torque at 0 g indicates friction | snippet only. https://www.diva-portal.org/smash/get/diva2:854670/FULLTEXT01.pdf |
| U12 | Stanford DDL, PhD 2009 | Y.-H. J. Hsu, *Estimation and control of lateral tire forces using steering torque* | Research vehicle | Medium: grip-limit information in steering torque (the "grip-loss drop" shape SimApex wants to reproduce) | snippet only. https://ddl.stanford.edu/sites/g/files/sbiybj25996/files/media/file/2009_thesis_hsu_estimation_control_of_lateral_tire_forces_0.pdf |
| U13 | Univ. of Michigan, arXiv 2509.20653 | *Cyber Racing Coach: a haptic shared control framework* | Simulated race car | Low: torque sensor on the column as a measurement method | snippet only |

Stanford's Revs dataset (Kegelman, Harbott, Gerdes 2017) is already covered in `datasets.md`.

## Not found

- Politecnico di Torino and Milano repositories return general vehicle-dynamics and brake theses (for example webthesis 22457, 9404, 28758; politesi Spada 722186), none on GT3 steering torque or pedal force.
- No GT3 thesis found at TU Delft, Chalmers, KTH, TU Graz or TUM by keyword.
- No thesis found on Porsche 911 GT3 Cup / GT3 R data acquisition.

## Next steps (for a human or a run with working access)

1. Download U1 (Advocaat) when the TU Delft repository is back up. It is the only academic work found that measures **real race-car steering torque against an rFactor FFB simulator**, the same engine family as LMU. Extract N·m vs lateral g and the FFB scaling method.
2. Read U2 (de Groot) for pedal force vs stiffness. It is the only academic source found on brake-pedal stiffness in a racing simulator, relevant to the T-LCM elastomers.
3. Ask Politecnico di Milano (Dipartimento di Meccanica, Dynamics & Vehicle group) and Dallara-linked theses whether any GT3 thesis with steering or brake data is under embargo.
