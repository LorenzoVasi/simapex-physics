# Real measured steering-wheel torque and steering systems on FIA/SRO GT3 cars

Research date: 2026-11-22. About 30 tool calls. Searches were in English, German and Italian.
Classification: kind = REAL / MANUFACTURER / DERIVED / ESTIMATED / SIMULATED. Evidence: A (direct verifiable measurement), B (official manufacturer or supplier), C (reconstructable indirect), D (engineering estimate with stated uncertainty), E (unverified, not for calibration).

## Q1. Is there any public measurement of steering torque [Nm] on a real GT3 car?

### Takeaway
None found. No public telemetry trace, paper, data-logger export or engineer statement with numbers gives measured hand-wheel or column torque in Nm on any homologated GT3 car. This means "not found in public sources", not "does not exist": teams and OEMs may hold such data privately. Every GT3 Nm figure found online (6-8 Nm, 8-12 Nm, "about 10 Nm") comes from sim-hardware vendors or forums. None cites a car, a sensor or a measurement.

### Cited Findings
- Fanatec's article "How Realistic Is Fanatec Torque Compared to a Real Car?" says "GT3 cars are typically capped at 6-8 Nm, but some drivers prefer settings closer to 10-12 Nm". It also gives an "average 8-15 Nm" for race cars in general, and an LMP2 "reported 11 Nm torque limit". The article cites no source, names no author, does not say any figure was measured, and has no original date (it shows "Last updated: October 01, 2026"). Classification: ESTIMATED, evidence E. This is vendor marketing, and the vendor sells 8-25 Nm bases. — [Fanatec](https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/)
- The Italian and German Fanatec pages repeat the same figures. The search excerpts also show a "5-8 Nm" starting point for GT3 in a sim. These are vendor estimates, evidence E. — [Fanatec IT](https://www.fanatec.com/eu/it/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/), [Fanatec DE, Lenkkraft](https://www.fanatec.com/eu/de/explorer/products/racing-wheels-wheel-bases/sim-racing-steering-load/)
- Sim-racing guides give similar ranges: roughly 10 Nm at the hands for GT3, and a typical 6-8 Nm cap. Both are unsourced. Evidence E. — [Sim Racing Manual](https://simracingmanual.com/hardware/wheelbase-torque-guide/)
- A post on X by Scott Thomson (Apr 2023, sim racer) says "GT3 cars have power steering. Stick to 6-8Nm". This is opinion, not a measurement. Evidence E. — [X post](https://x.com/Scotracer1987/status/1651878765121732610)
- An Assetto Corsa forum thread estimates 8-12 Nm for GT3, with power steering given as the reason. This is anecdotal, evidence E. — [assettocorsa.net forum](https://www.assettocorsa.net/forum/index.php?threads/how-stiff-are-the-steering-wheels-for-racecars-in-real-life-t300.44894/)
- A GTPlanet thread contains a second-hand claim that an unnamed GT3 driver runs "around 8nm on his Simucube" as a constant or average force, with peaks "much higher". I saw this only in a search snippet and did not open the thread. It is hearsay and describes a sim setting, not real-car torque. Evidence E. — [GTPlanet](https://gtplanet.net/forum/threads/how-strong-forcefeedback-in-gr3-cars.395540)
- iRacing in-sim telemetry reportedly shows GT3 peaks of up to about 60 Nm (Granite Devices forum, user MacIntyre, 17 Nov 2022). This is SIMULATED and must never be used as REAL. — [Granite Devices Community](https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267)
- The vendor and forum estimates (6-12 Nm) and the sim output (about 60 Nm peaks) are about 5x apart. Neither side has a real GT3 measurement behind it.

### Inferences
- The 6-8 / 8-12 Nm ranges keep getting repeated, but I could not trace them to any primary measurement. They may be circular: vendor copy citing forum consensus, and forum consensus citing vendor copy.
- For calibration, GT3 steering torque currently has to be classified as unknown. A defensible value would need a new measurement (see Q2 and Q6).

### Gaps
- No OEM (Porsche, BMW, Ferrari, Mercedes-AMG, Audi, Lamborghini, McLaren, Ford, GM, Aston Martin, Lexus, Honda, Bentley, Nissan) has published steering torque, rack force or assist maps for its GT3 car.
- The Racecar Engineering article "Steering Group" might discuss steering effort, but only a paywalled preview was available, and I have not verified its content. — [Everand / Racecar Engineering](https://www.everand.com/article/573009556/Steering-Group)
- I found no YouTube onboard video with a real-car steering-torque overlay from a GT3 car.

## Q2. Do GT3 cars log a steering torque channel?

### Takeaway
I found no evidence that a steering-torque channel is standard or mandated on GT3 cars. Steering angle is logged: Porsche lists a steering angle sensor in the GT3 R cockpit, and the 992 Cup ECU channel list includes `ASteer`. The SRO scrutineering logger manual shows no steering-torque channel. Commercial torque sensors exist that a team could add, for example the Racelogic RLSTSENSOR and the Michigan Scientific SW-TEL.

### Cited Findings
- Porsche 911 GT3 R (992), technical data, 30 Jul 2022: "adjustable steering column with steering angle senor [sic]". A steering angle sensor is listed. A torque sensor is not mentioned. MANUFACTURER, evidence B. — [Porsche Newsroom](https://newsroom.porsche.com/en/2022/motorsports/porsche-911-gt3-r-generation-992-customer-racing-car-premiere-29201.html)
- SRO Appendix 15 data logger manual (2023, also valid for 2024), used for GT World Challenge Asia. I extracted the PDF text locally. It covers the SRO DL/ODL/SDL/RTSL logger kit supplied via emotag, mandatory ECU channels, and pressure sensors, but contains no steering-related channel. Evidence B for absence in this document. Absence from one document does not prove the channel is absent from team loggers. — [SRO Appendix 15 PDF](https://www.gt-world-challenge-asia.com/documents/team/2149/Appendix%2015%20SRO%20MANUAL%202023%20(same%20for%202024)%20V230420.pdf)
- AiM ECU channel list for the Porsche 992 GT3 Cup (a Cup car, not GT3; listed here only as a logging reference). The only steering channel is `ASteer` ("Steering position angle"). There is no torque or power-steering channel. Evidence B. — [AiM PDF](https://aim-sportline.com/download/ecu/racing/Porsche_992_GT3_Cup_101_eng.pdf)
- Racelogic RLSTSENSOR steering torque sensor: CAN and analogue outputs, 50/100/200 Nm variants. It is aimed at ECE R79, ISO 4138 and ISO 7401 vehicle testing. It shows a measurement could be made, not that one has been made on a GT3. — [VBOX Automotive](https://www.vboxautomotive.co.uk/en/products/sensors/rl-steering-wheel-sensor), [Datasheet PDF](https://www.racelogic.co.uk/_downloads/vbox/Datasheets/Sensors/RLSTSENSOR_Data.pdf)
- Michigan Scientific SW-TEL: a wireless steering wheel torque transducer rated 135 N·m (100 lb-ft), which mounts to racing-wheel bolt patterns. — [Michigan Scientific](https://www.michsci.com/products/transducers/specialty-transducers/sw-tel-steering-torque-transducer/)
- ZF's motorsport EPHS pump unit varies assist "depending on the vehicle speed and rate of steer". Newer EPHS pumps are controlled over CAN using vehicle speed and steering angle. Source: search snippets of the ZF page. The ZF page itself returned HTTP 403, so I have not verified it. — [ZF Race Engineering](https://www.zf.com/products/en/zf_race_engineering/products_64353.html)
- The Sportscar365 "torque sensors" article concerns driveshaft or engine torque sensors used for BoP. It is not about steering. Do not confuse the two. — [Sportscar365](https://sportscar365.substack.com/p/insight-torque-sensors-and-virtual)

### Inferences
- On EHPS-equipped GT3 cars, the pump controller probably has internal signals such as pump speed or current, and possibly line pressure. Inference D: I found no public channel list showing these logged. Rack force could in principle be reconstructed from hydraulic pressure plus a column torque measurement (evidence C if done), but I found no public example.

### Gaps
- I found no public channel list for Bosch MS/DDU, MoTeC, Cosworth or Magneti Marelli GT3 installations that shows steering torque or power-steering pressure.

## Q3. Hydraulic vs electric power steering per GT3 car, and is assist adjustable?

### Takeaway
Only Porsche (992 GT3 R) and Lamborghini (Huracán GT3 EVO) have official or near-official statements: both use electro-hydraulic or hydraulic power steering, from ZF in Lamborghini's case. For the other GT3 cars, the official steering system type was not found in accessible sources.

### Cited Findings
- **Porsche 911 GT3 R (992.1, 2023-):** Technical data, front axle: "Electro-hydraulic power steering." MANUFACTURER, evidence B, dated 30 Jul 2022. — [Porsche Newsroom](https://newsroom.porsche.com/en/2022/motorsports/porsche-911-gt3-r-generation-992-customer-racing-car-premiere-29201.html)
- **Porsche 911 GT3 R (992.2, 2026-):** An aggregator says "Electro-hydraulic power steering with additional liquid cooling to improve thermal behaviour". This is a secondary source; I did not open the Porsche primary page, so evidence C or E. — [stuttcars](https://www.stuttcars.com/porsche-911-gt3-r-992-1-2023-2025-specifications-performance-2/), primary to check: [Porsche 911 GT3 R 2026](https://racing.porsche.com/cars/911-gt3-r-2026)
- **Lamborghini Huracán GT3 EVO (2019-2022):** According to a search snippet, the official spec page lists "ZF hydraulic power steering". The page text I could fetch only showed engine and aero entries, so this is only partly verified (evidence B/E). — [Lamborghini](https://lamborghini.com/en-en/motorsport/motorsport-models/huracán-gt3-evo)
- Autocar India says the 2018 EVO update moved from hydraulic to electro-hydraulic power steering. Pirelli's history page reportedly says EHPS came with the GT3 EVO, with assist adjustable during a race. I saw the Pirelli claim only in a search snippet; the page failed with a DNS error, so it is unverified (E). — [Autocar India](https://www.autocarindia.com/car-news-amp/lamborghini-huracan-gt3-evo2-revealed-424340), [Pirelli (not accessible)](https://racingspot.pirelli.com/global/en-ww/race/lamborghini-huracan-gt3-price-specs-history)
- **Lamborghini Huracán GT3 EVO2 (2023-):** Steering entry not found. Carry-over from the EVO is plausible but not confirmed. — [Lamborghini EVO2](https://www.lamborghini.com/en-en/motorsport/motorsport-models/huracan-gt3-evo2)
- **Audi R8 LMS GT3:** According to a search snippet, Audi Canada lists "servo assisted rack and pinion steering" with no evo stated. The page returned HTTP 403. auto-data.net lists "electric" steering for the 2019 R8 LMS GT3 (aggregator, E). The R8 LMS GT4 is described as electrohydraulic (different car). — [Audi Canada (403)](https://www.audi.ca/ca/web/en/models/motorsport/r8-lms-gt3/layer/technical-information.html), [auto-data.net](https://www.auto-data.net/en/compare&carId1=34624&carId2=&carId3=)
- **Mercedes-AMG GT3:** A CarAdvice 2016 track test describes a "direct-ratio rack-and-pinion that is servo assisted". The type is not specified, and it is unclear whether the text refers to the race car or a road GT. Evidence E. The AMG GT4 and GT2 are officially listed with electro-mechanical power steering (different cars). — [CarAdvice](https://www.caradvice.com.au/444760/mercedes-amg-gt3-review-track-test/), [AMG GT4](https://customerracing.mercedes-amg.com/en/news/allnews/mercedes-amg-motorsport-presents-more-powerful-mercedes-amg-gt4-.html)
- **BMW M4 GT3 / M4 GT3 EVO:** Official pages mention an adjustable steering column and a Fanatec-co-developed wheel, but not the power steering type. A Wikiwand claim about EPS refers to the road M3/M4. — [BMW M](https://www.bmw-m.com/en_PM/fastlane/motorsport/race-cars/bmw-m4-gt3-evo.html), [Paul Miller Racing](https://paulmillerracing.com/bmw_m4_gt3/)
- **Ferrari 296 GT3:** Steering type not found. The road 296 GTB has electric assistance, but this must not be carried over to the race car. — [Ferrari 296 GT3](https://www.ferrari.com/en-EN/competizioni-gt/296-gt3-preview-details)
- **McLaren 720S GT3, Corvette Z06 GT3.R, Ford Mustang GT3, Aston Martin Vantage AMR GT3, Lexus RC F GT3, Acura/Honda NSX GT3, Bentley Continental GT3, Nissan GT-R NISMO GT3:** No accessible official steering-type statement found.
- Forum claims (Rennlist, unverified, E) say the Porsche 991 Cup and factory race 911s use an electric-motor-driven hydraulic pump. — [Rennlist](https://rennlist.com/forums/997-gt2-gt3-forum/738291-991-cup-uses-mezger-and-hydraulic-steering.html)

### Inferences
- Electro-hydraulic assist (an electric pump driving a hydraulic rack) looks common in GT3. This is based on two OEMs plus forum claims, so the evidence is weak. With speed- and steer-rate-dependent assist (ZF EPHS), hand-wheel torque is not proportional to rack force. Any model needs an assist curve, and that curve is not public.

### Gaps
- FIA/SRO homologation forms would settle the steering type for each car, but they are not public.
- I found no official documentation on whether assist is adjustable, beyond the unverified Pirelli claim for the Lamborghini.

## Q4. Steering ratio, wheel diameter, lock-to-lock, max wheel angle, rack force, steering velocity

### Takeaway
I found no official values for any GT3 car. The per-car "steering lock" values in circulation (450-800 deg) come from Assetto Corsa Competizione (SIMULATED). ACC is said to model real ratios, but this cannot be verified.

### Cited Findings
- ACC per-car steering lock values: about 450 deg (Honda NSX GT3 EVO) to 800 deg (Ferrari 296 GT3, Porsche 991 II GT3 R), about 640 deg (Bentley, Aston), about 720 deg (Audi R8 LMS). SIMULATED, E. — [Coach Dave Academy](https://coachdaveacademy.com/tutorials/correct-steering-locks-and-ratios-in-acc/), [GoSetups](https://gosetups.gg/blog/your-guide-to-steering-locks-in-assetto-corsa-competizione/)
- Owner-reported ratios for road 996 GT3 (16.9:1, 2.98 turns) and 997 GT3 RS (17.1-13.8:1, 2.6 turns) are road cars, not GT3-category race cars. Excluded from GT3 values. — [Rennlist 996](https://rennlist.com/forums/996-forum/24703-various-996-model-s-rack-ratio-and-lock-to-lock.html), [Rennlist 997](https://rennlist.com/forums/997-gt2-gt3-forum/562009-gt3-rs-steering-ratio.html)
- An unsourced forum claim puts lock-to-lock at about 540 deg for "GT1 and 3 spec race cars". Evidence E. — [GTPlanet](https://www.gtplanet.net/forum/threads/degrees-of-rotation.200829)

### Gaps
- GT3 wheel diameter, rack force, maximum wheel angle and steering velocity: not found.

## Q5. Torque behaviour by situation (slow/medium/fast corners, braking, kerbs, under/oversteer, straights)

### Takeaway
I found no numbers for GT3. The only situational real-car figures are from a non-GT3, non-assisted IndyCar DW12 (see Q6). They must not be transferred to GT3.

### Gaps
- GT3 torque by corner type, kerb peaks, understeer torque drop and straight-line torque: none found.

## Q6. Comparison: measurements on non-GT3 cars (NOT transferable to GT3)

### Takeaway
A few real measurements exist, but on unassisted open-wheelers, FSAE cars, a road car and an unassisted IndyCar. They are useful only as order-of-magnitude context and as method references.

### Cited Findings
- Static (standstill) hand-wheel torque measured by Steve Lyman with a "Sensor Development hand wheel torque sensor". Figures: Autokraft Midget 6.0 Nm (380 mm wheel, no PAS); Lola T332 5.5-8.5 Nm (318 mm, no PAS); Lola T190 6.0-7.5 Nm; Lotus 20 4.0-4.5 Nm (330 mm); Dodge Viper SRT-10 road car 4.5 Nm (350 mm, with PAS). Article by Steve Fox, originally July 2010. REAL, evidence A- (the measurement method is described but there is no raw data), static only. — [Design Judges, Cockpit Control Forces](https://www.designjudges.com/articles/cockpit-control-forces)
- Maximum torque drivers can apply on a 254 mm FSAE wheel: 34-88 Nm across 16 drivers. This is human capability, not car demand. REAL, evidence A-. — [Design Judges](https://www.designjudges.com/articles/cockpit-control-forces)
- IndyCar Dallara DW12 (no power steering), Long Beach: reported "real data" of about 27-28 Nm peak on the T9 inside kerb and 15-20 Nm in corners. Rise from 0 to 28 Nm in about 0.2 s, compared with about 0.05 s in iRacing. Posted by a forum user (MacIntyre, Nov 2022) from an unnamed source; no sensor or date. REAL (claimed), evidence E. iRacing at the same location showed about 110 Nm at the kerb and 30-40 Nm in corners (SIMULATED). — [Granite Devices Community](https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267)
- Fanatec reports an LMP2 "11 Nm torque limit". No source, evidence E. — [Fanatec](https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/)
- A TU Delft thesis compared steering force from a real Formula 3 car with a simulator and a multibody model. Its sim wheel was limited to 16 Nm. The repository returned HTTP 500, so I could not extract its values; seen only in a search snippet. — [TU Delft repository](https://repository.tudelft.nl/file/File_d227fb52-2202-4128-9861-64f8f882f29a?preview=1)
- JTEKT (2014) describes a pinion-type EPS developed for a Super GT GT500 car (1100 kg, FR, 3396 cc). The PDF gives no torque, rack-force or assist numbers. This is GT500, not GT3. — [JTEKT Engineering Journal 1011E](https://www.jtekt.co.jp/e/engineering-journal/assets/1011/1011e_13.pdf)
- Road-car instrumented-wheel research (Politecnico di Milano, 2020) shows the measurement methodology (6-axis per hand). Not a race car. — [PMC / Data in Brief](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7214765/)

### Inferences
- These values are not transferable to GT3, which has power assist, different tyres, different aero and heavier mass.

### Gaps
- GT4, GTE, TCR, DTM and rally: no public measured steering torque found in this search.

## Structured records

| id | manufacturer | model | generation | year | category | parameter | value | unit | min | max | conditions | sensor | source | url | pub_date | method | sample_rate | evidence_level | uncertainty | kind | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| ST-001 | (generic) | GT3 (unspecified) | - | - | GT3 | hand-wheel torque "typical cap" | - | Nm | 6 | 8 | unspecified | none | Fanatec blog | https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/ | undated (updated 2026-10-01) | unstated | - | E | unknown | ESTIMATED | vendor marketing, no source |
| ST-002 | (generic) | GT3 (unspecified) | - | - | GT3 | preferred sim setting | - | Nm | 10 | 12 | sim preference | none | Fanatec blog | same as ST-001 | undated | opinion | - | E | unknown | ESTIMATED | sim setting, not car torque |
| ST-003 | (generic) | GT3 (unspecified) | - | - | GT3 | hand-wheel torque | - | Nm | 8 | 12 | unspecified | none | assettocorsa.net forum | https://www.assettocorsa.net/forum/index.php?threads/how-stiff-are-the-steering-wheels-for-racecars-in-real-life-t300.44894/ | n/a | forum estimate | - | E | unknown | ESTIMATED | anecdotal |
| ST-004 | (generic) | GT3 (unspecified) | - | 2023 | GT3 | recommended sim torque | - | Nm | 6 | 8 | - | none | Scott Thomson on X | https://x.com/Scotracer1987/status/1651878765121732610 | 2023-04 | opinion | - | E | unknown | ESTIMATED | opinion |
| ST-005 | (generic) | GT3 (iRacing model) | - | 2022 | GT3 | peak steering torque | 60 | Nm | - | 60 | iRacing telemetry | sim | Granite Devices forum | https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267 | 2022-11-17 | sim output | sim | E | unknown | SIMULATED | never use as REAL |
| ST-006 | (generic) | GT3 (unnamed driver) | - | - | GT3 | average sim FFB used by a GT3 driver | 8 | Nm | - | - | sim, average not peak | Simucube | GTPlanet forum (snippet only) | https://gtplanet.net/forum/threads/how-strong-forcefeedback-in-gr3-cars.395540 | n/a | hearsay | - | E | unknown | ESTIMATED | thread not opened |
| ST-007 | Porsche | 911 GT3 R | 992.1 | 2023 | GT3 | power steering type | electro-hydraulic | - | - | - | - | - | Porsche Newsroom tech data | https://newsroom.porsche.com/en/2022/motorsports/porsche-911-gt3-r-generation-992-customer-racing-car-premiere-29201.html | 2022-07-30 | OEM spec | - | B | - | MANUFACTURER | "Electro-hydraulic power steering." |
| ST-008 | Porsche | 911 GT3 R | 992.1 | 2023 | GT3 | steering sensor fitted | steering angle sensor | - | - | - | - | angle sensor in column | Porsche Newsroom | same as ST-007 | 2022-07-30 | OEM spec | - | B | - | MANUFACTURER | no torque sensor mentioned |
| ST-009 | Porsche | 911 GT3 R | 992.2 | 2026 | GT3 | power steering type | EHPS + liquid cooling | - | - | - | - | - | stuttcars (aggregator) | https://www.stuttcars.com/porsche-911-gt3-r-992-1-2023-2025-specifications-performance-2/ | n/a | secondary | - | C | primary unverified | MANUFACTURER (via aggregator) | check racing.porsche.com |
| ST-010 | Lamborghini | Huracán GT3 EVO | EVO | 2019 | GT3 | power steering type | ZF hydraulic / electro-hydraulic | - | - | - | - | - | Lamborghini spec page (snippet); Autocar India | https://lamborghini.com/en-en/motorsport/motorsport-models/huracán-gt3-evo | n/a | OEM spec (partly verified) | - | B/E | page text not fully retrieved | MANUFACTURER | "ZF hydraulic power steering" seen only in snippet |
| ST-011 | Lamborghini | Huracán GT3 EVO | EVO | 2019 | GT3 | assist adjustable during race | yes (claimed) | - | - | - | - | - | Pirelli Racing Spot (snippet) | https://racingspot.pirelli.com/global/en-ww/race/lamborghini-huracan-gt3-price-specs-history | n/a | secondary | - | E | page not accessible | MANUFACTURER (claimed) | unverified |
| ST-012 | Audi | R8 LMS GT3 | unspecified evo | - | GT3 | steering type | servo-assisted rack and pinion | - | - | - | - | - | Audi Canada (snippet, 403) | https://www.audi.ca/ca/web/en/models/motorsport/r8-lms-gt3/layer/technical-information.html | n/a | OEM spec (snippet) | - | E | page not accessible | MANUFACTURER | assist type not stated |
| ST-013 | (generic) | GT3 (ACC models) | various | - | GT3 | steering lock | - | deg | 450 | 800 | ACC per-car values | sim | Coach Dave / GoSetups | https://coachdaveacademy.com/tutorials/correct-steering-locks-and-ratios-in-acc/ | n/a | game data | - | E | unknown vs real | SIMULATED | NSX 450, 296/991 GT3 R 800, R8 720, Bentley/Aston 640 |
| ST-014 | SRO | logger kit (all GT3) | 2023-2024 | 2023 | GT3 | steering channel in SRO logger | none listed | - | - | - | - | - | SRO Appendix 15 | https://www.gt-world-challenge-asia.com/documents/team/2149/Appendix%2015%20SRO%20MANUAL%202023%20(same%20for%202024)%20V230420.pdf | 2023-04-20 | document review | - | B | absence in one doc only | MANUFACTURER | text extracted locally |
| CMP-001 | Porsche | 911 GT3 Cup (NOT GT3) | 992 | 2021 | Cup | steering channel | ASteer (angle) | - | - | - | - | - | AiM channel list | https://aim-sportline.com/download/ecu/racing/Porsche_992_GT3_Cup_101_eng.pdf | n/a | document review | - | B | - | MANUFACTURER | no torque channel |
| CMP-002 | Lola | T332 (F5000) | - | - | open-wheel | static hand-wheel torque | - | Nm | 5.5 | 8.5 | standstill, 318 mm wheel, no PAS | Sensor Development hand-wheel torque sensor | Design Judges (S. Fox, data S. Lyman) | https://www.designjudges.com/articles/cockpit-control-forces | 2010-07 | direct measurement | n/a | A | not stated | REAL | not GT3 |
| CMP-003 | Lola | T190 | - | - | open-wheel | static hand-wheel torque | - | Nm | 6.0 | 7.5 | standstill, 318 mm | same as CMP-002 | same as CMP-002 | same as CMP-002 | 2010-07 | direct | n/a | A | not stated | REAL | not GT3 |
| CMP-004 | Lotus | 20 | - | - | open-wheel | static hand-wheel torque | - | Nm | 4.0 | 4.5 | standstill, 330 mm | same as CMP-002 | same as CMP-002 | same as CMP-002 | 2010-07 | direct | n/a | A | not stated | REAL | not GT3 |
| CMP-005 | Autokraft | Midget | - | - | oval | static hand-wheel torque | 6.0 | Nm | - | - | standstill, 380 mm | same as CMP-002 | same as CMP-002 | same as CMP-002 | 2010-07 | direct | n/a | A | not stated | REAL | not GT3 |
| CMP-006 | Dodge | Viper SRT-10 | - | - | road car | static hand-wheel torque | 4.5 | Nm | - | - | standstill, 350 mm, with PAS | same as CMP-002 | same as CMP-002 | same as CMP-002 | 2010-07 | direct | n/a | A | not stated | REAL | road car |
| CMP-007 | (FSAE) | instrumented FSAE car | - | - | FSAE | max driver-applied torque | - | Nm | 34 | 88 | 16 drivers, 254 mm wheel, 3 reach positions | unnamed | Design Judges | same as CMP-002 | 2010-07 | direct | n/a | A- | sensor not named | REAL | human capability, not car demand |
| CMP-008 | Dallara | IndyCar DW12 | - | - | IndyCar | peak torque at kerb (Long Beach T9) | - | Nm | 27 | 28 | inside kerb, no PAS | unknown | Granite Devices forum (unnamed real data) | https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267 | 2022-11 | claimed logger graph | unknown | E | unknown | REAL (claimed) | provenance unknown |
| CMP-009 | Dallara | IndyCar DW12 | - | - | IndyCar | cornering torque (Long Beach) | - | Nm | 15 | 20 | corners, no PAS | unknown | same as CMP-008 | same as CMP-008 | 2022-11 | claimed | unknown | E | unknown | REAL (claimed) | - |
| CMP-010 | Dallara | IndyCar DW12 | - | - | IndyCar | torque rise time 0 to 28 Nm | 0.2 | s | - | - | kerb strike | unknown | same as CMP-008 | same as CMP-008 | 2022-11-19 | estimated from screenshots | unknown | E | unknown | DERIVED | iRacing about 0.05 s |
| CMP-011 | (generic) | LMP2 | - | - | LMP2 | torque limit | 11 | Nm | - | - | "all tracks" | none | Fanatec blog | https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/ | undated | "reported" | - | E | unknown | ESTIMATED | no source |
