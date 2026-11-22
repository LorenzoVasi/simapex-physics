# Motorsport ABS on GT3 race cars: how and when it intervenes

Research date: 2026-11-22. Scope: public documentation only. Several primary documents could not be read (see Gaps under each question). Classification: kind = REAL / MANUFACTURER / DERIVED / ESTIMATED / SIMULATED; evidence A–E as defined in the brief.

Access log (so the "not found" vs "does not exist" distinction is clear):
- READ IN FULL: Bosch ABS M5 Kit datasheet (V1, 02 Mar 2018, shopify CDN mirror); Bosch ABS M5 product-highlight page (bosch-motorsport.com, undated, footer (c) 2026); FIA Appendix J Art. 257A 2024 (published 08.12.2023).
- NOT ACCESSIBLE: Bosch ABS M5 Kit Operation Manual 70618763 (v1.3, 13/03/2026 per search index title) and ABS M5 Porsche Cup manual 148638347: both exceeded the 10 MB fetch limit, so no slip-map tables were read. FIA 257A 2025/2026 PDFs (404). Manthey 991 GT3 Cup Gen 2 ABS manual (404 after redirect). PCA "Brakes – Racing and ABS style systems" PDF (404). PMW "Insight: GT3 brake development" (403). xtramotorsport "How motorsport ABS works" (403). Bosch Motorsport Shop Australia pages incl. Ferrari 296 GT3 / AMG GT3 Evo profiles (DNS failure). Contents of these are known only from search-engine snippets and are flagged as such.

## 1. Which ABS systems are used on GT3 cars, on which models, and is ABS allowed by the rules?

### Takeaway
Bosch Motorsport ABS M5 (Kit 1/Kit 2, the OEM-calibrated variants) is described by Bosch-affiliated dealers as the standard GT3 ABS; no public "ABS M7" or "M8" Bosch Motorsport product was found, and no Continental/Teves system on a current GT3 car was confirmed. FIA Art. 257A does not name ABS at all: it bans stability control and permits only engine-output traction control, and says control of other devices is forbidden "unless specifically homologated", so ABS is fitted via each car's homologation rather than mandated.

### Cited Findings
- FIA Art. 257A (2024), Electrical equipment, "Driving aids": "Any electronic stability control system is forbidden. Only traction control systems managing the engine output are permitted. The control of other devices is forbidden unless specifically homologated." The Brakes section covers only fluid tanks, lines (Art. 253-3) and ducted air cooling; no ABS wording anywhere in the document (full-text search for "ABS", "anti-lock", "wheel speed"). Published 08.12.2023. — [FIA 257A 2024](https://www.fia.com/sites/default/files/257a_2024.pdf)
- Older FIA Appendix J Art. 257 (2010) listed ABS among prohibited aids (per search summary; not re-verified, likely superseded). — [FIA 257 2010](https://argent.fia.com/web/fia-public.nsf/0AF33C583192A268C125766D0037A948/$FILE/257_2010.pdf)
- The Bosch ABS M5 Kit Operation Manual reportedly states the system is designed to conform to FIA GT3 rules because it uses only 8 active hydraulic valves (search-index snippet of the manual; document itself not readable). — [Bosch ABS M5 Kit Manual v1.3](https://www.bosch-motorsport.com/content/downloads/Raceparts/Resources/pdf/Operation%20Manual_70618763_ABS_M5_Kit.pdf)
- Bosch datasheet: "2 hydraulic valves per wheel", "2 brake circuits (front and rear)", "2 hydraulic high pressures pumps", "2 hydraulic accumulators 5 cm3/each" (2 x 4 wheels = 8 valves, consistent with the snippet above). — [Bosch ABS M5 Kit datasheet, 02 Mar 2018](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Bosch Motorsport Australia dealer (Ferrari 296 GT3 profile, search snippet): the Bosch Motorsport M5 ABS is "the system of choice" for all GT World Challenge Australia cars and GT3 racing globally. AMG GT3 Evo profile (snippet): M5 gives the driver 12 stages of adjustable intervention. — [296 GT3 profile](https://www.bosch-motorsport-shop.com.au/296-gt3); [AMG GT3 Evo profile](https://www.bosch-motorsport-shop.com.au/amg-gt3-evo)
- A retailer lists the M5 Kit 1 as the version for GT3, GT4 and OEM race-car programmes, with advanced algorithms (downforce-dependent slip, corner-inside-wheel slip reduction) requiring programming by Bosch engineers. — [xtramotorsport Kit 1](https://xtramotorsport.com/product/bosch-motorsport-abs-m5-kit-1/)
- Bosch publishes Porsche Cup-specific M5 variants (991 Gen 1, 991 Gen 2, 997), i.e. one-make Cup cars, not GT3 R. — [Data sheet ABS M5 Kit Porsche Cup](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/Data_sheet_70647947_ABS_M5_Kit_Porsche_Cup.pdf)
- Searches for "ABS M7" with GT3 returned nothing; only M4 and M5 datasheets were found on Bosch domains. — searches 2026-11-22, see [ABS M4 Kit datasheet](https://www.bosch-motorsport.com/media/downloads/abs_m4_kit.pdf)

### Inferences
- ABS in GT3 is permitted (not mandatory) through homologation of the specific car; the 8-valve architecture (no extra stability-control valves) is what keeps the M5 compatible with the ESC ban. Evidence C.
- The per-model attribution (Porsche 911 GT3 R 991/992, Ferrari 488/296, Huracán, M4 GT3, AMG, R8 LMS, 720S, Vantage, Z06 GT3.R, Mustang, RC F, NSX, Bentley, Nissan) to Bosch M5 rests only on the dealer's generic "all GT3 cars" claim plus the 296 and AMG profiles. Treat as E for any model not explicitly named.

### Gaps
- No manufacturer press release or homologation form found confirming the ABS supplier for any specific GT3 model (search for 992 GT3 R / M4 GT3 / Huracán GT3 + Bosch returned only product pages). Homologation forms are not public. "Not found", not "does not exist".
- No public evidence of Continental, ATE or other ABS on current GT3s; also "not found".
- 2025/2026 Art. 257A not readable (404); wording may have changed.
- Whether an M7/M8 exists as a non-public OEM unit: unknown.

## 2. Publicly documented control logic: inputs, maps, slip targets, failure modes

### Takeaway
Bosch documents the inputs (4 active wheel speeds, front and rear circuit pressure, longitudinal/lateral acceleration and yaw rate), a 12-position map switch (11 calibrated settings plus an ABS-off position), and optional downforce-dependent slip, lateral-deceleration slip and corner-inside-wheel slip reduction. Bosch publishes no numerical slip target, wheel-deceleration threshold or map table. Any number for "the GT3 ABS slip threshold" is therefore not public.

### Cited Findings
- Inputs: "4 active wheel speed DF11"; "Brake pressure (front brake circuit / rear brake circuit)"; "Longitudinal acceleration, lateral acceleration, yaw rate (MM5.10 sensor)". — [Bosch M5 datasheet 2018](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Current Bosch pages list the yaw/accel sensor as MM7.10 vs MM5.10 on the 2018 datasheet (search snippet; sensor revision conflict). Product page: yaw/acceleration sensor range "±4.2 g" transversal; pressure sensors "260 bar", one per brake circuit. — [Bosch ABS M5 product page](https://bosch-motorsport.com/products-and-services/product-highlights/abs-m5-kit/)
- Settings: "11 adjustment settings (applicable for OEMs)"; "ABS function can be deactivated (Pos. 1 or Pos. 12)"; "Selection of ABS maps via Bosch 12-position switch or via specified CAN signal"; "ABS-Off position optional on position 1" (Kit 1/2). — [Bosch M5 datasheet 2018](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Options (Kit 1 and Kit 2, not Clubsport): "Downforce depending slip option", "Lateral deceleration slip option", "Corner inside wheel slip reduction option". — [Bosch M5 datasheet 2018](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Bosch: "revised ABS control algorithm" with new hardware; "adjustment of slip with respect to downforce"; map switch can be changed while driving; switch position broadcast on CAN (DDU 9/10); sensitivity and control characteristics adjustable; the system "calculates current brake proportioning and suggests optimizations". — [Bosch ABS M5 product page](https://bosch-motorsport.com/products-and-services/product-highlights/abs-m5-kit/)
- Calibratable vehicle parameters: vehicle weight, track, wheel weights, wheel circumferences, wheelbase, number of sensor increments. — [Bosch M5 datasheet 2018](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Failure mode: all four wheel-speed signals from active sensors (DF11) must be present, "or the system switches off". Outputs include ABS warning light (MIL) and an EBD warning light if needed, plus TTL wheel speed signals FL/FR/RL/RR. — [Bosch ABS M5 product page](https://bosch-motorsport.com/products-and-services/product-highlights/abs-m5-kit/); [datasheet](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)
- Downforce-dependent slip, as described by a retailer (not Bosch): it "tightens the wheel slip target as aerodynamic downforce increases at speed". Kit-tier claims differ between retailers. — [gomuchfaster M5 buyer's guide](https://www.gomuchfaster.com/pages/bosch-motorsport-abs-m5-system); [xtramotorsport](https://xtramotorsport.com/bosch-motorsport/bosch-motorsport-abs/)
- A motorsport-ABS blog (secondary, 403 on fetch, snippet only) says the system watches each wheel's deceleration rate as well as slip, and that optimum slip on slicks is lower than the 10–20% often quoted for road tyres. No number or source given. — [xtramotorsport blog](https://xtramotorsport.com/blog/how-motorsport-abs-works/)
- Rennlist user on Bosch M4: switch positions select separate sets of slip targets and acceleration values (anecdotal). — [Rennlist Bosch motorsport ABS thread](https://rennlist.com/forums/racing-and-drivers-education-forum/1115440-boschmotorsports-abs-2.html)

### Inferences
- From the documented inputs, the control is per-wheel (two valves per wheel) with front/rear circuit pressure feedback, and can raise or lower slip targets by speed or aero load, lateral acceleration and inside/outside wheel. So a simulator should model a slip target that varies with speed/load and lateral g, plus a wheel-deceleration trigger, rather than one fixed threshold. Evidence C (architecture), values unknown.
- Map ordering (whether position 2 or 11 is "least intervention") is not public and may be calibrated per OEM; it should not be assumed.
- No rear-axle-only mode is documented in the readable sources. The front/rear split exists only at circuit level (2 circuits, 2 pressure sensors).

### Gaps
- No slip-ratio target values, wheel deceleration thresholds, pressure-release rates or per-map tables found in any readable public document. The Operation Manual (unreadable, >10 MB) is the most likely public place; it may contain them.
- Fallback behaviour beyond "switches off" when wheel-speed signals are lost (e.g. EBD-only mode, pressure-sensor failure) is not documented in the readable sources.
- Loop/sample rates of the ABS ECU: not found.

## 3. Real track data of ABS intervention

### Takeaway
No public real GT3 ABS trace (wheel speeds, slip, pressure during modulation, deceleration with car, track and conditions) was found. Sim-racing guides and forums provide only SIMULATED or anecdotal material.

### Cited Findings
- Sim-racing guide (SIMULATED context): ABS activity at the top of the brake trace is generally fine, activity down in the trail is time lost, and the threshold "is not a fixed pedal percentage". — [GitGud GT3 sim guide](https://gitgudracingacademy.com/guides/gt3-sim-racing-guide/)
- A GitHub PR on iRacing GT3 brake-trace analysis (SIMULATED, unverified). — [GitHub PR #311](https://github.com/MisterKinn/f1-data-analyst/pull/311)
- Related academic work on racing ABS is Formula Student / autonomous / simulation-based, not GT3: a cascade fuzzy-PID slip controller for FS (Técnico Lisboa); per-wheel slip-ratio PID ABS in an autonomous racing stack; extremum-seeking control to find optimal slip online without tyre knowledge (arXiv 2411.15823, results in simulation). — [Thesis 76063](https://fenix.tecnico.ulisboa.pt/downloadFile/281870113705725/Thesis_76063.pdf); [arXiv 2512.06892](https://arxiv.org/pdf/2512.06892); [arXiv 2411.15823](https://arxiv.org/html/2411.15823v1)

### Inferences
- Since the optimum slip moves with conditions (the point of the extremum-seeking work), a calibration tool should treat ABS onset as the point where measured slip passes a load- and speed-dependent peak, not a constant. Evidence D, no numeric value.

### Gaps
- No REAL GT3 data with numbers (slip %, bar, g) found. Possible but unverified sources: team data in Racecar Engineering (not reached), the PMW GT3 brake article (403), MoTeC/Bosch WinDarab sample logs (not searched).

## 4. Pedal feedback: pulsation, kickback, travel

### Takeaway
No Bosch or OEM document describes pedal feel during M5 intervention. Driver reports are anecdotal and mixed: some feel one or two pulses near the threshold, others feel almost nothing on track. A separate "ice pedal" (very hard pedal, little decel) failure is reported on Porsche GT4 Clubsport / GT3 road cars, but its cause is not confirmed.

### Cited Findings
- Rennlist (Porsche GT3 owners, anecdotal, E): near the threshold one could "feel it pulse one or two times"; another says they never feel on track the pulsations felt at a stop sign in the rain. — [Rennlist "abs off on track"](https://rennlist.com/forums/996-gt2-gt3-forum/266969-abs-off-on-track-3.html); [Rennlist ABS ice mode thread](https://rennlist.com/forums/997-gt2-gt3-forum/534934-well-abs-ice-mode-finally-got-me-9.html)
- Rennlist: the Bosch unit gives more feedback when pumping pressure back to the pedal, while the Teves feels numb (anecdotal, E). — [Rennlist Bosch motorsport ABS](https://rennlist.com/forums/racing-and-drivers-education-forum/1115440-boschmotorsports-abs-2.html)
- GT4 Clubsport "ice pedal": rock-hard pedal with much less braking for heavy-footed drivers (anecdotal, E). — [Rennlist GT4CS ice pedal](https://rennlist.com/forums/racing-and-drivers-education-forum/1023152-urgent-help-needed-gt4cs-ice-pedal.html)
- Bosch M5 hardware includes 2 high-pressure return pumps and two 5 cm3 accumulators (pumping fluid back toward the master cylinder is the physical source of any kickback). — [Bosch M5 datasheet](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)

### Inferences
- With return pumps and accumulators present, some pedal feedback during modulation is physically plausible; its magnitude, frequency and travel change are undocumented. A simulator effect should be optional or adjustable, not tuned to a claimed "real" amplitude. Evidence D.

### Gaps
- No quantitative pedal-force or pedal-travel data during GT3 ABS intervention found. The PCA Porsche brake document (404) reportedly covers Cup-car pedal firmness, but it could not be read.

## 5. Tyre influence on peak longitudinal slip

### Takeaway
No public peak-longitudinal-slip value for Pirelli P Zero DHF, Michelin or Goodyear GT3 slicks was found. Generic statements (peak around 10–20% for road tyres, lower for slicks) come from secondary sources without data.

### Cited Findings
- Optimum slip on motorsport slicks is said to be lower than the 10–20% often quoted for road tyres (secondary blog, snippet only, no data). — [xtramotorsport blog](https://xtramotorsport.com/blog/how-motorsport-abs-works/)
- Bosch's downforce-dependent and lateral-deceleration slip options imply the target changes with vertical load and combined slip, rather than being a single tyre constant. — [Bosch M5 datasheet](https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf)

### Inferences
- Tyre supplier GT3 data (Pacejka/TTC-type) is proprietary. Sim titles' tyre models are SIMULATED and must not be relabelled as REAL.

### Gaps
- Pirelli, Michelin and Goodyear do not publish GT3 slick force-slip curves. Not found; likely proprietary.

## Structured records

| id | manufacturer | model | generation | year | category | parameter | value | unit | min | max | conditions | sensor | source | url | pub_date | method | sample_rate | evidence_level | uncertainty | kind | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ABS-001 | FIA | Group GT3 (all) | Art. 257A | 2024 | regulation | electronic stability control | forbidden | - | - | - | all GT3 | - | FIA Appendix J Art. 257A 2024 | https://www.fia.com/sites/default/files/257a_2024.pdf | 2023-12-08 | document text | - | B | none | MANUFACTURER | Regulator text; ABS not named; "control of other devices forbidden unless specifically homologated" |
| ABS-002 | FIA | Group GT3 (all) | Art. 257A | 2024 | regulation | traction control scope | engine output only | - | - | - | all GT3 | - | FIA Art. 257A 2024 | https://www.fia.com/sites/default/files/257a_2024.pdf | 2023-12-08 | document text | - | B | none | MANUFACTURER | Context for ABS legality via homologation |
| ABS-003 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS hardware | hydraulic valves per wheel | 2 | count | - | - | all kits | - | M5 datasheet V1 | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | 8 valves total; manual reportedly cites this for FIA GT3 conformity (snippet only) |
| ABS-004 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS hardware | brake circuits | 2 (front/rear) | count | - | - | all kits | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | Plus 2 high-pressure pumps, 2 accumulators |
| ABS-005 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS hardware | accumulator volume | 5 | cm3 | - | - | each of 2 | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | Relevant to pedal kickback physics; no pedal data |
| ABS-006 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS input | wheel speed sensors | 4 | count | - | - | active DF11 | DF11 | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | All 4 required or system switches off (product page) |
| ABS-007 | Bosch Motorsport | ABS M5 Kit | M5 | undated (c)2026 | ABS input | brake pressure sensor range | 260 | bar | - | 260 | one per circuit | pressure sensor | Bosch M5 product page | https://bosch-motorsport.com/products-and-services/product-highlights/abs-m5-kit/ | undated | product page | - | B | none | MANUFACTURER | Sensor full scale, not operating pressure |
| ABS-008 | Bosch Motorsport | ABS M5 Kit | M5 | undated (c)2026 | ABS input | yaw/accel sensor lateral range | 4.2 | g | -4.2 | 4.2 | - | MM5.10 (2018) / MM7.10 (current, snippet) | Bosch M5 product page | https://bosch-motorsport.com/products-and-services/product-highlights/abs-m5-kit/ | undated | product page | - | B | sensor model conflict | MANUFACTURER | Inputs: long. acc, lat. acc, yaw rate |
| ABS-009 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS settings | map switch positions | 12 | count | - | - | Kit 1/2; or via CAN | 12-pos switch | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | Changeable while driving; position on CAN |
| ABS-010 | Bosch Motorsport | ABS M5 Kit | M5 | 2018 | ABS settings | calibrated settings (OEM) | 11 | count | - | - | ABS off on Pos. 1 or Pos. 12 | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | none | MANUFACTURER | Content of each map not public |
| ABS-011 | Bosch Motorsport | ABS M5 Kit 1/2 | M5 | 2018 | ABS logic | downforce-dependent slip | available (option) | - | - | - | Kit 1/2 only | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | values unknown | MANUFACTURER | Retailer: target tightens with downforce (not Bosch wording) |
| ABS-012 | Bosch Motorsport | ABS M5 Kit 1/2 | M5 | 2018 | ABS logic | lateral-deceleration slip | available (option) | - | - | - | Kit 1/2 only | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | values unknown | MANUFACTURER | - |
| ABS-013 | Bosch Motorsport | ABS M5 Kit 1/2 | M5 | 2018 | ABS logic | corner-inside wheel slip reduction | available (option) | - | - | - | Kit 1/2 only | - | M5 datasheet | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/ABS_M5_Kit_Datasheet_51_en_31624027147.pdf | 2018-03-02 | datasheet | - | B | values unknown | MANUFACTURER | - |
| ABS-014 | Bosch Motorsport | ABS M5 Kit | M5 | - | ABS logic | slip target value | not public | % | - | - | - | - | none found | - | - | - | - | E | unknown | - | Manual (>10 MB) not readable; do not assume a value |
| ABS-015 | various | GT3 (all) | - | 2024-2026 | ABS supplier | ABS system | Bosch Motorsport M5 | - | - | - | GTWC Australia / "GT3 globally" | - | Bosch Motorsport Shop AU (snippet) | https://www.bosch-motorsport-shop.com.au/296-gt3 | undated | dealer claim | - | C | generic claim; page not fetched | MANUFACTURER | Named: Ferrari 296 GT3, AMG GT3 Evo; other models unconfirmed |
| ABS-016 | Porsche | 911 GT3 Cup | 991 Gen1/Gen2, 997 | - | ABS supplier | ABS system | Bosch ABS M5 Porsche Cup | - | - | - | one-make Cup | - | Bosch Porsche Cup datasheet (title) | https://cdn.shopify.com/s/files/1/0057/7712/9587/files/Data_sheet_70647947_ABS_M5_Kit_Porsche_Cup.pdf | - | datasheet title | - | B | contents not read | MANUFACTURER | Cup car, not GT3 R |
| ABS-017 | - | GT3 (drivers) | - | - | pedal feel | pulsation near threshold | 1-2 pulses felt | - | - | - | road/track GT3 (Porsche), not race-spec confirmed | driver | Rennlist forum | https://rennlist.com/forums/996-gt2-gt3-forum/266969-abs-off-on-track-3.html | - | anecdote | - | E | high | REAL (anecdotal) | Others report not feeling it |
| ABS-018 | - | GT3 sim | - | - | trace interpretation | ABS onset not a fixed pedal % | - | - | - | - | simulator | - | GitGud sim guide | https://gitgudracingacademy.com/guides/gt3-sim-racing-guide/ | - | guide | - | E | - | SIMULATED | Supports variable-threshold modelling only |
| ABS-019 | Pirelli/Michelin/Goodyear | GT3 slicks | - | - | tyre | peak longitudinal slip | not public | % | - | - | - | - | none found | - | - | - | - | E | unknown | - | Secondary: "lower than 10-20% road-tyre figure", no data |
