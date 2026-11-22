# Brake pedal force and brake-system data on real GT3 cars (FIA/SRO GT3)

Research date: 2026-11-22. About 18 search/fetch calls were made. Several primary sources were blocked or unreadable; they are listed under each Gaps section. "Not found" here means not found in this search pass. It does not mean the data does not exist. Teams and manufacturers hold this data (pedal load cells, MC bores) but they do not publish it.

Classification keys: kind = REAL / MANUFACTURER / DERIVED / ESTIMATED / SIMULATED. Evidence = A (direct verifiable measurement), B (official manufacturer/supplier), C (indirect, can be reconstructed), D (engineering estimate with stated uncertainty), E (unverified).

## Q1. Is there any public measurement of brake pedal force (N / kgf) on a real GT3?

### Takeaway
No. I found no publicly accessible, verifiable pedal load-cell measurement of driver foot force on any FIA/SRO GT3 car (no car, year, sensor or track trace). Every kg figure in circulation is a sim-industry or forum estimate (evidence E). The only number close to real data is line *pressure*: a forum practitioner says about 100 bar or more is the target on current Porsche factory racers. Pressure is not pedal force.

### Cited Findings
- A sim-racing blog/forum statement says a GT car takes "about 60-100kg" and an F3 car "100kg+" to fully depress the brake pedal. No source, sensor or car generation is given, so this is ESTIMATED, evidence E. It appeared in the search snippet for the Overtake.gg thread "Adjusting Sim Pedals/Realism" — [Overtake.gg](https://www.overtake.gg/threads/adjusting-sim-pedals-realism.253973/). I did not fetch the full thread to confirm who wrote it.
- Fanatec, a sim pedal vendor with a commercial interest, writes: "GT Drivers have to literally stomp on the brake pedal, applying a force equal to or even greater than 190 bar of pressure". The page has no date or author (footer © 2026). It confuses force with pressure and gives no car, measurement or source, so it is evidence E — [Fanatec, Sim Racing braking technique](https://www.fanatec.com/uk/en/explorer/games/gaming-tips/sim-racing-braking-technique/)
- The same Fanatec page says "the average pressure applied to the brake pedal is around 35-55 bar" in a simulator. This is SIMULATED/marketing and uses the same unit confusion — [Fanatec](https://www.fanatec.com/uk/en/explorer/games/gaming-tips/sim-racing-braking-technique/)
- Rennlist forum, reported via search snippets because the threads returned HTTP 403 to fetch:
  - "100 bar is the target" for 991 Cup and 981/718 Clubsport comp cars when brake pressure is the logged channel.
  - "in the latest GT3 factory racers, if you aren't at 100 bar or more, you're not pushing hard enough."
  - These are practitioner (coach/engineer) statements, evidence E/C. The car generation is ambiguous: "GT3 factory racers" could mean Porsche Cup/GT3 R — [Rennlist data-acquisition forum](https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363684-vbox-hd-brake-pressure-help.html); [Rennlist Cup cars "max cup brake pedal pressure"](https://rennlist.com/forums/porsche-cup-cars/1162746-max-cup-brake-pedal-pressure.html) (fetch returned 403).
- Rennlist users also warn that logged pressure channels are often mis-scaled. One channel shown "in percentage" was actually bar 0–100. This matters when reading third-party traces — [Rennlist](https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363684-vbox-hd-brake-pressure-help.html) (snippet)
- McLaren GT3 driver Joe Osborne set up Heusinkveld pedals to "match" a McLaren 720S GT3 in a Demon Tweeks feature. The URL returned 404, so I have no numbers — [Demon Tweeks blog](https://blog.demon-tweeks.com/?p=9358) (inaccessible).
- Measurement hardware exists, but no GT3 data from it is published. Racelogic sells a VBOX brake pedal force sensor (spec page) — [Racelogic](https://en.racelogic.support/VBOX_Automotive/Product_Info/Sensors/Brake_Pedal_Force_Sensor/Brake_Pedal_Force_Sensor_User_Guide/05_-_Brake_Pedal_Force_Sensor_Specification). I did not fetch the spec contents.

### Inferences
- A defensible ESTIMATED range for peak GT3 driver foot force is about 600–1100 N (60–110 kgf), evidence D. It comes from the hydraulic model in Q3 combined with the ~100 bar practitioner figure. It is not a measurement.
- The "190 bar" figure is physically possible, but with typical race MC bores and pedal ratios it implies pedal forces of about 1300–2000 N (see Q3). That makes it unlikely as a routine peak for ABS-equipped GT3 cars. Treat it as unreliable marketing.

### Gaps
- I found no GT3 pedal-force trace (N vs time) from MoTeC, Bosch WinDarab, Cosworth or Magneti Marelli training material.
- I found no driver-specific maximum force and no light/medium/maximum split.
- I found no data on pedal travel in mm, a force–travel curve or pedal stiffness for any GT3 car.

## Q2. Brake hardware actually fitted per GT3 car (calipers, discs, ABS, circuits, booster)

### Takeaway
Manufacturers publish caliper piston counts and disc diameters, and sometimes the dual-circuit plus balance-bar layout. None of the sources I found publish master-cylinder bores, pedal ratio, or confirmation that there is or is not a booster. The Porsche 911 GT3 R is the best documented: two separate front/rear circuits, a driver-adjustable balance bar, race ABS, and front/rear brake pressure sensors.

### Cited Findings
- **Porsche 911 GT3 R (991.1, 2016–2018 MY; 2017 media guide):**
  - two separate brake circuits for front and rear axles, "adjustable by the driver via brake balance bar system"
  - Race ABS fitted as standard
  - front: six-piston aluminium monobloc racing calipers, ventilated and grooved steel discs, D = 380 mm
  - Kind MANUFACTURER, evidence B — [Porsche press kit Media Guide 2017](https://presskit.porsche.de/motorsport/en/mediaguide-2017/topic/race-cars/porsche-911-gt3-r.html); [911 GT3 R spec sheet PDF (EPA mirror)](https://19january2021snapshot.epa.gov/sites/static/files/2017-03/documents/911_gt3_r_specs.pdf)
- **Porsche 911 GT3 R (992):**
  - a spec listing describes two separate brake circuits "with brake pressure sensors" for front and rear, adjustable via a balance bar system (search snippet; seller-sourced listing, evidence C)
  - the official 992 page gives only engine/weight data (about 1250 kg, 416 kW); brake specs are in a linked PDF that I did not read — [Porsche Motorsport 911 GT3 R 992](https://racing.porsche.com/cars/911-gt3-r-992)
- **AP Racing GT3/GT4 calipers (supplier catalogue, no car named):**
  - "Forged, 6 Piston front CP7269 and 4 Piston rear CP7480", "Designed for GT3 & 4 applications"
  - matching discs: CP7269 Ø390×36 mm, CP7480 Ø370×32 mm
  - kind MANUFACTURER (supplier), evidence B — [AP Racing Catalogue Issue 4](https://catalogue.apracing.com/4)
- AP Racing offers ABS-specific "Centre Valve" master cylinders with a dedicated seal design, because ABS stresses the seals. The catalogue gives no GT3 bore table — [AP Racing catalogue](https://catalogue.apracing.com/11) (search snippet)
- **Mercedes-AMG GT3 Evo:** Bosch Motorsport says it uses the Bosch M5 ABS with 12 intervention stages. Kind MANUFACTURER (supplier), evidence B — [Bosch Motorsport Shop AU tech profile](https://www.bosch-motorsport-shop.com.au/amg-gt3-evo)
- **Mercedes-AMG GT3 Edition 130Y Motorsport (special-edition GT3):**
  - 390 mm front discs with 6-pot calipers, 355 mm rear discs with 4-pot calipers
  - the page lists carbon-ceramic discs, which is suspicious for a GT3, because SRO GT3 requires steel discs; flagged as unreliable
  - evidence C/E — [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8692/Mercedes-AMG-GT3-Edition-130Y-Motorsport.html)
- **Ferrari 296 GT3 (2023–):**
  - front 400×36 mm discs with 6-piston calipers and 30 mm pads; rear 332×32 mm discs with 4-piston calipers and 26.5 mm pads (Motorsport Week, evidence C)
  - Ultimatecarpage lists the same discs for the 296 GT3 Evo (2026)
  - the supplier (Brembo) was not confirmed by an official source — [Motorsport Week](https://www.motorsportweek.com/?p=213800); [Ultimatecarpage 296 GT3 Evo](https://www.ultimatecarpage.com/spec/8813/Ferrari-296-GT3-Evo.html)
  - Ferrari says the Evo's front-brake airflow is more than 20% higher than the 296 GT3's (MANUFACTURER) — [Ferrari 296 GT3 Evo article](https://www.ferrari.com/en-BG/competizioni-gt/articles/ferrari-296-gt3-evo-bred-from-victory-evolved-to-push-limits?amp=true)
- **Lamborghini Huracán GT3 Evo (2019–2022):**
  - front "Steel Brembo / Tm Racing 380×34 mm discs, 6-pistons monoblock caliper"; rear 355×32 mm discs with 4-piston monoblock calipers
  - source is a team/seller listing, evidence C — [Paul Miller Racing](https://paulmillerracing.com/?p=9670)
  - the original Huracán GT3 page says Brembo Racing discs (MANUFACTURER, evidence B) — [Lamborghini Squadra Corse Huracán GT3](https://squadracorse.lamborghini.com/cars/huracan-gt3)
- **Lamborghini Huracán GT3 EVO2 (2022–):** Lamborghini says only that it has "new calipers and pads" for a better endurance/sprint balance, with no dimensions (MANUFACTURER, evidence B) — [Lamborghini press release](https://www.lamborghini.com/en-en/motorsport/news/lamborghini-squadra-corse-presents-the-new-huracan-gt3-evo2-the-racing-version-of-huracan-sto)
- **BMW M4 GT3 (2022–):** the official info brochure (May/June 2021) was downloaded, but its text could not be extracted (compressed PDF). Brake data is not verified — [BMW M4 GT3 Infobrochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **BMW M4 GT4 (not GT3, comparison only):** an aftermarket AP Racing master cylinder replacing BMW Motorsport part 34308428607 is listed as 25.4 mm bore. This hints that an ABS tandem-type MC of that size class is used on BMW customer cars, but GT4 must not be mixed with GT3 — [BimmerWorld](https://www.bimmerworld.com/Brakes/Master-Cylinders/Brake-Master-Cylinder-25mm-BMW-F82-M4-GT4.html)
- **Audi R8 LMS GT3 evo II:** I found no brake specifications. The Audi media pages give no brake data — [Audi MediaCenter](https://www.audi-mediacenter.com/en/audi-r8-lms-gt3-2021-14104)

### Inferences
- Every current GT3 with a documented layout uses 6-piston front and 4-piston rear calipers, steel discs of about 370–400 mm front and about 330–370 mm rear, and motorsport ABS (Bosch M5/M7 class). Porsche documents a balance bar with separate front/rear circuits; this probably applies to the 911 GT3 R (991/992).
- I found no explicit statement of booster/servo presence or absence for any GT3 car.

### Gaps
- No data found for McLaren 720S GT3/Evo, Aston Martin Vantage AMR GT3, Corvette Z06 GT3.R, Ford Mustang GT3, Lexus RC F GT3, Honda/Acura NSX GT3/Evo, Bentley Continental GT3, Nissan GT-R NISMO GT3, Audi R8 LMS (all versions), Ferrari 488 GT3, or BMW M4 GT3 brake hardware. I did not search all of these within the call budget.
- Front/rear master-cylinder bores are not public for any GT3 car.
- Pedal ratio is not public for any GT3 car.
- Pedal box supplier is not public for any GT3 car.
- I found no data on brake-by-wire. GT3 regulations generally do not use it, but I did not verify this.
- FIA/SRO homologation forms (the likely source for disc and caliper data) were not accessed.

## Q3. Brake pressure vs pedal force: published relationship and DERIVED model

### Takeaway
No GT3-specific pressure–force correlation is published. The general physics is documented by brake suppliers and training providers. Below I compute an explicit parametric DERIVED model, which shows what about 100 bar front pressure implies for pedal force under stated assumptions.

### Cited Findings
- HP Academy brake course cheat sheet: "Brake Force (Per MC) = Pedal Effort (kg) x Pedal Ratio / Bias Position". In its worked example, 50 kg pedal effort, 5:1 ratio and centred bias give "125kg or 1230 newtons" into each master cylinder — [HP Academy Brake Course Cheat Sheet](https://hpacademy.com/assets/Course-Content/Brake-Course/920bc72d6d/Brake-Course-Cheat-Sheet.pdf); [HP Academy pedals and pedal boxes](https://hpacademy.com/courses/brake-system-design-and-optimization/pedals-and-pedal-boxes)
- HP Academy suggests about 50 kg (about 490 N) as a reasonable maximum pedal effort for an average adult — [HP Academy, proportioning valves, pedal feel and master cylinders](https://www.hpacademy.com/courses/brake-system-design-and-optimization/proportioning-valves-pedal-feel-and-master-cylinders) (search snippet)
- Merlin Motorsport gives a working range of about 100–150 lbf (445–667 N) leg force, with a 5.2:1 example — [Merlin Motorsport, brake system hydraulics top tips](https://de.merlinmotorsport.co.uk/knowledge_base_articles/view/brake-system-hydraulics-top-tips-for-racing-264) (search snippet)
- Wilwood recommends a pedal ratio of about 6:1 for manual (unboosted) brakes and about 4:1 for power brakes. A smaller bore means less effort but more travel — [Wilwood, pedal ratios](https://shop.wilwood.com/blogs/news/pedal-ratios-and-how-to-find-them)

### Inferences (DERIVED, evidence D)
Model: F_pedal = (p_f · A_f + p_r · A_r) / R

- R is the pedal ratio. A_f and A_r are the front/rear MC piston areas.
- The balance bar splits the pushrod force between the two MCs.
- Neglected: return springs, friction, pedal efficiency (about 5–10%), and ABS modulation.

Areas:

| Bore | Area |
|---|---|
| 5/8" (15.875 mm) | 197.9 mm² |
| 3/4" (19.05 mm) | 285.0 mm² |
| 7/8" (22.225 mm) | 387.9 mm² |

1 bar = 0.1 N/mm².

Assumed bores and ratios are typical race values from the Wilwood and HP Academy guidance above. They are not GT3-specific.

| Case | Front bore | Rear bore | p_f [bar] | p_r [bar] | Pushrod force [N] | F_pedal at R=5 [N / kgf] | F_pedal at R=6 [N / kgf] |
|---|---|---|---|---|---|---|---|
| D1 | 3/4" | 7/8" | 100 | 60 | 2850 + 2327 = 5177 | 1035 / 106 | 863 / 88 |
| D2 | 5/8" | 3/4" | 100 | 70 | 1979 + 1995 = 3974 | 795 / 81 | 662 / 68 |
| D3 (190 bar claim) | 5/8" | 3/4" | 190 | 133 | 3760 + 3791 = 7551 | 1510 / 154 | 1258 / 128 |
| D4 (190 bar claim) | 3/4" | 7/8" | 190 | 114 | 5415 + 4422 = 9837 | 1967 / 201 | 1640 / 167 |

The rear/front pressure ratios (0.6–0.7) are assumptions. Uncertainty is ±30% or more, driven by unknown bores and ratio.

- Conclusion: about 100 bar with no booster gives an estimated 650–1050 N (about 65–105 kgf) peak foot force, evidence D. This is close to the unsourced "60–100 kg" GT-car estimate. About 190 bar would need roughly 1250–2000 N, which is above most sim load cells (Fanatec states 60–200 kg designs).

### Gaps
- No published GT3 calibration of pressure against pedal force.
- No temperature effect data (fluid, pad mu vs pedal force) for GT3.
- No pedal travel figures to build the force–travel curve.

## Q4. Statements from GT3 drivers, engineers and pedal manufacturers about real-car pedal forces

### Takeaway
Pedal manufacturers quote their own *capacities* (Heusinkveld Ultimate+ up to 140 kg with a 200 kg load cell; Pro up to 55 kg). Fanatec quotes the "190 bar" GT claim. None of these is presented as a GT3 measurement. Simucube says kg is not the right quality metric.

### Cited Findings
- Heusinkveld specs as reported in search snippets (evidence B for product capacity, not real-car force):
  - Ultimate+ "capable of providing a maximum pedal force of 140kg (200kg loadcell)"
  - Pro: maximum force 55 kg (125 lb)
  - Source: snippet in the [Overtake.gg thread](https://www.overtake.gg/threads/adjusting-sim-pedals-realism.253973/) and related search results (Heusinkveld site not fetched)
- Fanatec: sim pedal load cells are "designed to handle pressures between 60 and 200 kg" (product context, evidence E for real cars) — [Fanatec](https://www.fanatec.com/uk/en/explorer/games/gaming-tips/sim-racing-braking-technique/)
- Fanatec forum users report that a ClubSport V3 load cell rated 90 kg maxes out at about 35–40 kg foot force at 100% brake-force setting. Anecdotal; SIMULATED context — [Fanatec forum](https://forum.fanatec.com/discussion/comment/60707/)
- A Fanatec forum thread notes that a real driver is braced and helped by deceleration g-forces, which a sim rig does not reproduce. Opinion — [Fanatec forum "Simulation setup to mirror real-life pressures"](https://bi.fanatec.com/discussion/comment/48491/)
- Simucube says kilograms do not define pedal quality and emphasises maximum load capacity (for example 2000 N). No real-car GT3 figure is given — [Simucube load cell article](https://simucube.com/?p=60795) (fetched; no real-car force values present)

### Inferences
- All vendor figures should be stored as SIMULATED/product capacity, never as REAL GT3 values.

### Gaps
- The Joe Osborne / McLaren 720S GT3 pedal-matching article was inaccessible (404).
- I did not fetch Asetek, Sim-Lab or Thrustmaster statements.

## Q5. Comparison data (non-GT3): Cup / GT4 / general racing

### Takeaway
The only comparison numbers are forum targets for Porsche Cup cars (about 100 bar) and general racing pedal-effort guidance (445–667 N). The data comes from other car classes, so keep it separate from the GT3 records.

### Cited Findings
- 991 Cup and 981/718 Clubsport: "100 bar is the target" (forum, evidence E) — [Rennlist](https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363684-vbox-hd-brake-pressure-help.html)
- Road Boxster data in the same forums shows about 850 psi (about 58.6 bar) average peak into Turn 1 on a road car. This is not GT3 and is listed for context only — [Rennlist](https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1120029-understanding-brake-pressure-psi-what-is-it-telling-me.html) (snippet)
- BMW M4 GT4: AP master cylinder bore 25.4 mm (aftermarket replacement listing) — [BimmerWorld](https://www.bimmerworld.com/Brakes/Master-Cylinders/Brake-Master-Cylinder-25mm-BMW-F82-M4-GT4.html)
- Ferrari 296 Challenge (single-make, not GT3): CCM-R Plus discs, 408 mm front and 390 mm rear — [Ferrari 296 Challenge](https://www.ferrari.com/corse-clienti/296-challenge) (search snippet)

### Inferences
- I did not search for a Politecnico di Torino thesis on brake pressure telemetry that might hold formula/GT pressure traces. It appeared in results but I did not fetch it — [PoliTo webthesis 15662](https://webthesis.biblio.polito.it/15662/1/tesi.pdf)

### Gaps
- No GTE, F3/F4 or touring pedal-force measurements were collected in this pass.

## Structured records

| id | manufacturer | model | generation | year | category | parameter | value | unit | min | max | conditions | sensor | source | url | pub_date | method | sample_rate | evidence_level | uncertainty | kind | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| BP01 | Porsche | 911 GT3 R | 991.1 | 2016-2018 | GT3 | brake circuits | 2 separate (front/rear) | - | - | - | standard | - | Porsche Media Guide 2017 | https://presskit.porsche.de/motorsport/en/mediaguide-2017/topic/race-cars/porsche-911-gt3-r.html | 2017 | spec sheet | - | B | - | MANUFACTURER | driver-adjustable balance bar |
| BP02 | Porsche | 911 GT3 R | 991.1 | 2016-2018 | GT3 | ABS | Race ABS standard | - | - | - | - | - | Porsche Media Guide 2017 | https://presskit.porsche.de/motorsport/en/mediaguide-2017/topic/race-cars/porsche-911-gt3-r.html | 2017 | spec sheet | - | B | - | MANUFACTURER | |
| BP03 | Porsche | 911 GT3 R | 991.1 | 2016-2018 | GT3 | front disc diameter | 380 | mm | - | - | steel, ventilated, grooved; 6-piston monobloc Al caliper | - | Porsche Media Guide 2017 | https://presskit.porsche.de/motorsport/en/mediaguide-2017/topic/race-cars/porsche-911-gt3-r.html | 2017 | spec sheet | - | B | - | MANUFACTURER | |
| BP04 | Porsche | 911 GT3 R | 992 | 2023- | GT3 | brake pressure sensors | front + rear fitted | - | - | - | balance bar | pressure sensors | spec listing (snippet) | https://racing.porsche.com/cars/911-gt3-r-992 | n/a | listing | - | C | not confirmed from official PDF | MANUFACTURER | official PDF not read |
| BP05 | Porsche (practitioner) | "GT3 factory racers" | unspecified | n/a | GT3 | peak brake line pressure target | 100 | bar | 100 | - | hard braking, "pushing hard" | logger pressure channel | Rennlist forum | https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363684-vbox-hd-brake-pressure-help.html | n/a | forum statement | - | E | unknown; generation ambiguous; 403 on fetch | REAL (claimed) | pressure, not pedal force |
| BP06 | Fanatec (vendor) | generic "GT" | - | n/a | GT (generic) | peak brake "pressure" | 190 | bar | 190 | - | "stomp" | - | Fanatec article | https://www.fanatec.com/uk/en/explorer/games/gaming-tips/sim-racing-braking-technique/ | undated (c) 2026 | marketing claim | - | E | high; unit confusion force/pressure | ESTIMATED | implausible as routine peak per DERIVED model |
| BP07 | Fanatec (vendor) | simulator | - | n/a | SIM | sim brake "pressure" avg | 35-55 | bar | 35 | 55 | simulator | sim load cell | Fanatec article | https://www.fanatec.com/uk/en/explorer/games/gaming-tips/sim-racing-braking-technique/ | undated | marketing | - | E | high | SIMULATED | |
| BP08 | unspecified | generic GT car | - | n/a | GT (generic) | pedal force to fully depress | 60-100 | kgf | 60 | 100 | full braking | none stated | Overtake.gg forum (snippet) | https://www.overtake.gg/threads/adjusting-sim-pedals-realism.253973/ | n/a | forum estimate | - | E | very high | ESTIMATED | 589-981 N |
| BP09 | AP Racing | GT3/GT4 front caliper CP7269 | - | n/a | GT3/GT4 supplier | pistons / disc | 6 / 390x36 | - / mm | - | - | forged monobloc | - | AP Racing catalogue Issue 4 | https://catalogue.apracing.com/4 | n/a | catalogue | - | B | car fitment not stated | MANUFACTURER | |
| BP10 | AP Racing | GT3/GT4 rear caliper CP7480 | - | n/a | GT3/GT4 supplier | pistons / disc | 4 / 370x32 | - / mm | - | - | forged monobloc | - | AP Racing catalogue Issue 4 | https://catalogue.apracing.com/4 | n/a | catalogue | - | B | car fitment not stated | MANUFACTURER | |
| BP11 | Mercedes-AMG | AMG GT3 Evo | C190 Evo | 2020- | GT3 | ABS | Bosch M5, 12 stages | - | - | - | - | - | Bosch Motorsport Shop AU | https://www.bosch-motorsport-shop.com.au/amg-gt3-evo | n/a | supplier profile | - | B | - | MANUFACTURER | |
| BP12 | Mercedes-AMG | AMG GT3 Edition 130Y | C190 | 2022 | GT3 | discs front/rear; pistons | 390 / 355; 6 / 4 | mm; - | - | - | listing says carbon ceramic (suspect) | - | Ultimatecarpage | https://www.ultimatecarpage.com/spec/8692/Mercedes-AMG-GT3-Edition-130Y-Motorsport.html | n/a | aggregator | - | E | disc material suspect | MANUFACTURER (via aggregator) | |
| BP13 | Ferrari | 296 GT3 | F171 GT3 | 2023- | GT3 | front disc / pistons / pad | 400x36 / 6 / 30 | mm / - / mm | - | - | - | - | Motorsport Week | https://www.motorsportweek.com/?p=213800 | 2022 | press report | - | C | supplier unconfirmed | MANUFACTURER (reported) | |
| BP14 | Ferrari | 296 GT3 | F171 GT3 | 2023- | GT3 | rear disc / pistons / pad | 332x32 / 4 / 26.5 | mm / - / mm | - | - | - | - | Motorsport Week | https://www.motorsportweek.com/?p=213800 | 2022 | press report | - | C | - | MANUFACTURER (reported) | |
| BP15 | Lamborghini | Huracán GT3 Evo | Evo | 2019-2022 | GT3 | front disc / pistons | 380x34 / 6 | mm / - | - | - | steel Brembo/Tm Racing, monobloc | - | Paul Miller Racing | https://paulmillerracing.com/?p=9670 | n/a | team listing | - | C | - | MANUFACTURER (reported) | |
| BP16 | Lamborghini | Huracán GT3 Evo | Evo | 2019-2022 | GT3 | rear disc / pistons | 355x32 / 4 | mm / - | - | - | monobloc | - | Paul Miller Racing | https://paulmillerracing.com/?p=9670 | n/a | team listing | - | C | - | MANUFACTURER (reported) | |
| BP17 | Lamborghini | Huracán GT3 EVO2 | EVO2 | 2022- | GT3 | brakes | new calipers + pads (no dims) | - | - | - | - | - | Lamborghini press release | https://www.lamborghini.com/en-en/motorsport/news/lamborghini-squadra-corse-presents-the-new-huracan-gt3-evo2-the-racing-version-of-huracan-sto | 2022 | press release | - | B | - | MANUFACTURER | |
| BP18 | generic | GT3 (model) | - | n/a | GT3 | peak pedal force @100 bar front | 662-1035 | N | 662 | 1035 | MC 5/8-7/8", R=5-6, p_r/p_f 0.6-0.7, no booster | - | this note (model) | - | 2026-11-22 | F=(p_f A_f+p_r A_r)/R | - | D | +/-30% or more | DERIVED | approx 68-106 kgf |
| BP19 | generic | GT3 (model) | - | n/a | GT3 | pedal force implied by 190 bar | 1258-1967 | N | 1258 | 1967 | same assumptions | - | this note (model) | - | 2026-11-22 | same | - | D | +/-30% or more | DERIVED | argues against 190 bar routine |
| BP20 | HP Academy | generic race car | - | n/a | general | reasonable max pedal effort | 50 | kgf | - | - | average adult | - | HP Academy | https://www.hpacademy.com/courses/brake-system-design-and-optimization/proportioning-valves-pedal-feel-and-master-cylinders | n/a | training guidance | - | D | - | ESTIMATED | approx 490 N |
| BP21 | Merlin Motorsport | generic race car | - | n/a | general | leg force working range | 100-150 | lbf | 445 N | 667 N | - | - | Merlin Motorsport | https://de.merlinmotorsport.co.uk/knowledge_base_articles/view/brake-system-hydraulics-top-tips-for-racing-264 | n/a | guidance | - | D | - | ESTIMATED | |
| BP22 | Wilwood | generic | - | n/a | general | recommended pedal ratio | 6:1 manual / 4:1 power | ratio | - | - | - | - | Wilwood | https://shop.wilwood.com/blogs/news/pedal-ratios-and-how-to-find-them | n/a | guidance | - | B (supplier) | - | MANUFACTURER | not GT3-specific |
| BP23 | Heusinkveld | Sim Pedals Ultimate+ | - | n/a | SIM | max pedal force | 140 | kgf | - | - | 200 kg load cell | load cell | search snippet | https://www.overtake.gg/threads/adjusting-sim-pedals-realism.253973/ | n/a | product spec | - | E (not fetched from vendor) | - | SIMULATED | product capacity |
| BP24 | Porsche | 991 Cup / 718 Clubsport (comparison) | 991 / 718 | n/a | Cup/GT4 | peak brake pressure target | 100 | bar | - | - | - | logger | Rennlist forum | https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363684-vbox-hd-brake-pressure-help.html | n/a | forum | - | E | - | REAL (claimed) | non-GT3 comparison |
| BP25 | BMW | M4 GT4 (comparison) | F82 | n/a | GT4 | master cylinder bore (aftermarket AP replacement) | 25.4 | mm | - | - | ABS car | - | BimmerWorld | https://www.bimmerworld.com/Brakes/Master-Cylinders/Brake-Master-Cylinder-25mm-BMW-F82-M4-GT4.html | n/a | retail listing | - | C | - | MANUFACTURER (reseller) | not GT3 |
