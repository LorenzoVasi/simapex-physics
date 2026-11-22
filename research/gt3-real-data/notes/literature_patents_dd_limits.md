# Literature, patents and DD / load-cell limits for physically realistic FFB and brake calibration (SimApex Physics)

Scope note: about 17 search and fetch calls. Several key PDFs could not be read: ResearchGate returned 403, SAE Mobilus is paywalled, the Fanatec Help Center returned 403, and a Chalmers PDF could not be parsed because no PDF text tool is installed locally. Each item below says how much of the source was actually read. Nothing here comes from a full-text GT3 measurement paper, because none was found.

Classification legend: kind = REAL / MANUFACTURER / DERIVED / ESTIMATED / SIMULATED. Evidence level: A direct verifiable measurement, B official manufacturer, C reconstructable indirect, D engineering estimate, E unverified.

Terminology (applies throughout):
- **Hand-wheel (steering) torque** [Nm] is the torque at the steering-wheel rim or column.
- **Rack force** [N] is the axial force in the steering rack.
- **Hand force** [N] is the tangential force at the rim, equal to torque divided by rim radius.
- **Brake line pressure** [bar] is a hydraulic quantity.
- **Pedal force** [N] is the force at the pedal pad.
- **Software brake curve** is a mapping from load-cell output to game input. It is not mechanical pedal stiffness.

---

## Q1. Experimental steering-wheel torque measurements in race cars / high-performance cars (incl. GT3)

### Takeaway
I found no peer-reviewed or full-text engineering source with measured GT3 hand-wheel torque. The only GT3 figures available are a manufacturer marketing statement (Fanatec, "6-8 Nm") and anecdotes from forums and drivers. All of them are E-level for real-car calibration. The academic steering-torque work found covers passenger cars and instrumentation methods.

### Cited Findings
- Fanatec article "How Realistic Is Fanatec Torque Compared to a Real Car?" (read in full; no date shown, no method, no sources named). It gives these figures:
  - Standard sedan/compact with EPS: "2-3 Nm".
  - Performance road models: "5-7 Nm".
  - High-performance sports cars: 10-12 Nm stationary and 2-8 Nm while driving.
  - Performance cars with hydraulic steering: "5-10 Nm under dynamic conditions".
  - GT3: "typically capped at 6-8 Nm", with some drivers preferring 10-12 Nm.
  - LMP2: "a reported 11 Nm torque limit at all tracks".
  - Race cars in general: "average forces range between 8-15 Nm".
  - Rack force is never mentioned.
  - Source: [Fanatec](https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/). Kind: MANUFACTURER secondary claim with no measurement, so level E for real-car data. The vendor also has a commercial interest.
- A racing driver/commentator on X: "GT3 cars have power steering. Stick to 6-8Nm". Kind: opinion, level E. [X post, Scott Thomson](https://x.com/Scotracer1987/status/1651878765121732610)
- Forum users claim 8-12 Nm for GT3. Kind: anecdote, level E. [Assetto Corsa forum](https://www.assettocorsa.net/forum/index.php?threads/how-stiff-are-the-steering-wheels-for-racecars-in-real-life-t300.44894/)
- A Granite Devices forum poster reports that iRacing telemetry shows up to about 60 Nm for a GT3 car. This is the simulator's internal steering-column/FFB value, not a measured real load. Kind: SIMULATED, level E. It shows that sim "Nm" outputs are not comparable to physical hand torque without scaling. [Granite Devices community](https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267)
- Sharp, R. S. and Granger, R., "On car steering torques at parking speeds" (Imperial College, 2003). This is a passenger-car study of parking-speed torque, not racing. Only the search snippet was read. [PDF](http://www2.ee.ic.ac.uk/cap/publications/2003/parking_torques.pdf)
- "Race-Car Instrumentation for Driving Behavior Studies" appears on ResearchGate (403; only the snippet was read). The snippet describes instrumented cars recording steering-wheel angle, torque, speed and yaw rate. Its exact content (race car vs passenger car) could not be verified. [ResearchGate](https://www.researchgate.net/publication/220409320_Race-Car_Instrumentation_for_Driving_Behavior_Studies)
- Instrumented steering wheels that measure 3 forces and 3 moments per hand have been published in a data article. These are passenger-car / driver-model studies. [PMC7214765](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7214765/); [ScienceDirect S095741581200030X](https://www.sciencedirect.com/science/article/abs/pii/S095741581200030X)
- KTH degree project (2015) on steering feel. It uses a single-track model to study how mass, steering ratio and power-steering curves affect steering-wheel torque. Only the snippet was read. [DiVA](https://www.diva-portal.org/smash/get/diva2:854670/FULLTEXT01.pdf)

### Inferences
- Treat "GT3 = 6-8 Nm" as a hypothesis to be validated, not as a calibration constant. Possible validation routes:
  - a team's column torque sensor channel, if one is logged;
  - rack-force or hydraulic-assist pressure channels combined with the steering ratio;
  - an instrumented wheel.
- Because GT3 cars have power assistance, hand torque is decoupled from rack force by the assist curve. Rack force alone cannot give GT3 hand torque without the assist map (see Q2).

### Gaps
- No GT3-specific measured hand-wheel torque was found in SAE, VSD, IMechE or university theses within this search budget.
- Theses from PoliMi, PoliTo, TUM, Cranfield and Oxford Brookes on race-car steering torque were not located in full text. Politecnico di Torino webthesis hits appeared for related tyre and brake topics but were not read.

---

## Q2. Steering torque models and typical gradients (Nm/g)

### Takeaway
The standard modelling chain is well established in textbooks:

tyre Mz (pneumatic trail) plus Fy times mechanical (caster) trail, plus jacking / scrub terms → kingpin moment → rack force → column torque via steering ratio, minus assist, plus friction and compliance → hand-wheel torque.

The standard on-centre metrics come from Norman, SAE 840069. I found no openly readable numeric Nm/g ranges for performance cars, so the gradient values remain a gap.

### Cited Findings
- Norman, K. D., "Objective Evaluation of On-Center Handling Performance", SAE Technical Paper 840069 (1984; the TRID record is dated 1985). Only the abstract was read; the paper is paywalled.
  - Method: steering-wheel angle and torque, speed and yaw rate are recorded during a low-frequency sinusoidal steer.
  - The data are reduced to steering hysteresis, on- and off-centre sensitivities, torque gradients, and a steering work parameter.
  - [SAE Mobilus](https://saemobilus.sae.org/papers/objective-evaluation-center-handling-performance-840069); [TRID](https://trid.trb.org/View/211291)
- A later study (search snippet only) describes typical on-centre test conditions: sinusoid at 0.2 Hz with an amplitude giving 0.2 g lateral acceleration. Another thesis defines a "torque gradient at 0.1 g [Nm/g]" linked to road feel just off centre. [Chalmers thesis PDF, not parsed](https://publications.lib.chalmers.se/records/fulltext/230941/230941.pdf); [Cranfield repository](https://dspace.lib.cranfield.ac.uk/server/api/core/bitstreams/5ecc7721-e9ab-4a36-9ddb-10af33361838/content)
- Related SAE papers (abstracts only, paywalled):
  - "On-Centre Steering Wheel Torque Characteristics during Steady State Cornering", SAE 2008-01-0502. [SAE](https://saemobilus.sae.org/papers/centre-steering-wheel-torque-characteristics-steady-state-cornering-2008-01-0502)
  - "Developments in On-Centre Steering Evaluation and Testing", SAE 2006-01-0796. [SAE](https://saemobilus.sae.org/papers/developments-centre-steering-evaluation-testing-2006-01-0796)
- Textbook references for the model chain. These were not re-read in this session, so no numeric values are taken from them:
  - Pacejka, H. B., *Tyre and Vehicle Dynamics*, 3rd ed. (with Besselink), Butterworth-Heinemann, 2012, ISBN 978-0-08-097016-5. It covers the Magic Formula Fy, Fx and Mz, pneumatic trail, and the relaxation length rule of thumb.
  - Milliken, W. F. and Milliken, D. L., *Race Car Vehicle Dynamics*, SAE R-146, 1995. It covers steering-system geometry (caster, trail, kingpin inclination, scrub radius) and aligning torque.
  - Segers, J., *Analysis Techniques for Racecar Data Acquisition*, 2nd ed., SAE R-408, 2014. It covers data-acquisition practice, including steering and brake channels.

### Inferences
- A physics-based hand-torque estimate needs these inputs (kind DERIVED, level C if all inputs are measured):
  - per-front-wheel Fy and Mz (from tyre data or a model);
  - geometry: caster trail, kingpin inclination, scrub radius;
  - steering arm length;
  - steering ratio;
  - GT3 hydraulic or EPS assist map;
  - column friction and compliance.
  - With all of these, the result is approximately T_hand ≈ (Σ kingpin moments / effective steering-arm-to-column ratio) − T_assist(T_column, speed).
- Pneumatic trail collapses before peak Fy. This produces the drop in hand torque near the limit that drivers use as a grip cue. Preserving the shape of the torque-vs-ay curve, not just its magnitude, is the key DD calibration objective.

### Gaps
- No open numeric Nm/g torque-gradient ranges were obtained, for road performance cars or for race cars. The Norman paper and the Chalmers/Cranfield theses would contain them but were not read in full.
- No published GT3 power-steering assist curves were found.

---

## Q3. Human factors: brake pedal force of trained / racing drivers, pedal feel

### Takeaway
I found no peer-reviewed measurement of maximum pedal force by professional racing drivers. The available data cover general driver populations (HSRI/UMTRI, 1970s), one simulator study and one blog-level estimate for a race car. Race-car pedal force can be derived from logged line pressure, master-cylinder bore and pedal ratio. Pressure and force must not be equated.

### Cited Findings
- HSRI Report HuF-6a, "Brake Force Requirement Study: Driver-Vehicle Braking Performance as a Function of Brake System Design Variables" (University of Michigan HSRI). It used an instrumented vehicle and measured right-foot maximum force on 276 drivers. Search summary: the report suggested a maximum brake-pedal effort of 85 lbf (≈378 N) as a design guideline for road cars. Only the snippet was read. Kind: REAL for a general population, road-car context, level C. [UMich Deep Blue](https://backend.production.deepblue-documents.lib.umich.edu/server/api/core/bitstreams/f3c97bb5-459e-4dc7-9c64-5b38ba5803b8/content)
- "Foot Brake Pedal Force Capability of Drivers" covers 599 US drivers (276 female, 323 male), with a 5th-percentile female maximum of about 400 N. It was read only via a Scribd copy, so the citation details are unverified; level C/E. [Scribd](https://www.scribd.com/document/235921970/Foot-Brake-Pedal-Force-Capability-of-Drivers)
- "Car Racing in a Simulator: Validation and Assessment of Brake Pedal Stiffness" (ResearchGate 305317564; 403, snippet only). Pedal force was measured with strain gauges on a simulator pedal, and the 100% brake input corresponded to 375 N. Kind: SIMULATED for the rig setting, level C. [ResearchGate](https://www.researchgate.net/publication/305317564_Car_Racing_in_a_Simulator_Validation_and_Assessment_of_Brake_Pedal_Stiffness)
- I. Platas, "Race car brake pedal study" (LinkedIn, Formula-style car). Driver force of about 1200 N was back-calculated from logged pressure and pedal geometry. Kind: DERIVED, non-GT3, blog-level E. [LinkedIn](https://www.linkedin.com/pulse/race-car-pedal-study-iv%C3%A1n-platas)
- Hydraulics: line pressure depends on pedal force, pedal ratio and master-cylinder bore. Wilwood's worked example: a 5.2:1 ratio turns 100 lbf into 520 lbf at the master cylinder, and a 13/16" bore (0.5185 in²) gives ≈1003 psi. A larger bore raises pedal effort and shortens travel. [Merlin Motorsport](https://es.merlinmotorsport.co.uk/knowledge_base_articles/view/brake-system-hydraulics-top-tips-for-racing-264); [Wilwood](https://shop.wilwood.com/blogs/news/pedal-ratios-and-how-to-find-them); [NAPA](https://knowhow.napaonline.com/how-to-measure-brake-pedal-ratio/)
- ISO/ECE context: ECE R13-H / FMVSS 135 road-car pedal-force limits were not re-verified in this session. They are only context and must not be used for GT3.

### Inferences
- Use this relation with GT3 telemetry (DERIVED, level C when the master-cylinder bores, balance-bar split and pedal ratio are known):

  F_pedal = p_line × A_MC,eff / (pedal_ratio × η)

  For a dual master cylinder with a balance bar: F_pushrod = p_front·A_front + p_rear·A_rear. Without those parameters, pedal force cannot be reconstructed from pressure.
- The T-LCM is rated to 100 kg (≈981 N) of load (see Q4). The general-population data (≈400 N female p5) and the blog estimate (≈1200 N, non-GT3) bracket that rating. Real GT3 peak pedal force therefore may or may not exceed T-LCM capacity. This is unknown until it is derived from team data.

### Gaps
- No measured GT3 pedal force or pedal travel was found.
- No peer-reviewed pedal-feel or force-travel preference study for racing drivers was read in full. The simulator stiffness paper is the best lead.

---

## Q4. Racing tyre data (self-aligning torque, longitudinal slip, relaxation length)

### Takeaway
Neither Pirelli nor Michelin publishes GT3 tyre force-and-moment data (cornering stiffness, peak slip angle, peak slip ratio, relaxation length). None was found. The usable public substitute is FSAE TTC data (Calspan TIRF, members only), which is non-GT3 (Hoosier 10"/13" FSAE tyres) and must not be transferred to GT3.

### Cited Findings
- The FSAE TTC is a closed forum giving member schools Calspan TIRF force and moment data. A 2020 paper compares Hoosier 43075 16x7.5-10 R25B vs LCO on peak Fy, cornering stiffness and friction coefficient: *J. Phys.: Conf. Ser.* 1478 (2020), DOI 10.1088/1742-6596/1478/1/012032. It was read only via a Scribd copy, so no numbers were extracted. Non-GT3. [Scribd](https://www.scribd.com/doc/36835708/Ttc-Sae-Paper); [Texas A&M thesis using TTC data](https://oaktrust.library.tamu.edu/bitstream/handle/1969.1/2600/etd-tamu-2005B-MEEN-Mueller.pdf?sequence=1)
- Loeb, J. S., Guenther, D. A., Chen, H.-H. F., Ellis, J. R., "Lateral Stiffness, Cornering Stiffness and Relaxation Length of the Pneumatic Tire", SAE 900129 (1990). This is the foundational experiment. Abstract only (paywalled). [SAE](https://www.sae.org/publications/technical-papers/content/900129/)
- Relaxation length:
  - The general range of 0.12-0.45 m, higher at higher speed and load, is attributed to Pacejka on Wikipedia. This is a secondary source, not specific to racing tyres; level D/E.
  - Pacejka's rule of thumb puts relaxation length at about the wheel radius at nominal load.
  - [Wikipedia](https://en.wikipedia.org/wiki/Relaxation_length)
- A VERTEC project paper, with Pirelli as partner, studied how temperature affects cornering stiffness and relaxation length. Abstract only; it is not confirmed to cover GT3 tyres. [ResearchGate](https://www.researchgate.net/publication/389204096_On_the_relationship_between_tyre_cornering_stiffness_and_road_surface_roughness)
- A Politecnico di Torino thesis on pneumatic trail / tyre modelling exists. It was not read. [PoliTo webthesis 34676](https://webthesis.biblio.polito.it/34676/1/tesi.pdf)

### Inferences
- For GT3, Mz and pneumatic-trail behaviour must be either:
  - identified from team data (e.g. fitting a Magic Formula to logged ay, steer and speed); or
  - labelled SIMULATED or ESTIMATED, taken from a sim tyre model.
- FSAE TTC fits may be used only to test the shape of the method, labelled non-GT3.

### Gaps
- No public Pirelli P Zero DH/DHF or Michelin Pilot Sport GT data was found. It is not public.

---

## Q5. Patents (architecture only)

### Takeaway
Steering-wheel and column torque-measurement patents exist. I found no motorsport-specific or sim-racing-FFB patent within this search budget.

### Cited Findings
- US9354130B2, "Torque measurement device for steering wheel" (published as US20140290386). It describes an architecture for measuring torque at the steering wheel. [Google Patents](https://patents.google.com/patent/US9354130); [Justia](https://patents.justia.com/patent/20140290386)
- US6517113B1, "Steering column with torquemeter". [Google Patents](https://patents.google.com/patent/US6517113)
- US7219576, "Variable ratio pedal assembly"; US6609436, "Vehicle brake pedal apparatus". These are road-car pedal architecture patents. [USPTO 7219576](https://image-ppubs.uspto.gov/dirsearch-public/print/downloadPdf/7219576); [USPTO 6609436](https://image-ppubs.uspto.gov/dirsearch-public/print/downloadPdf/6609436)

### Gaps
- Claims were not read. No Fanatec, Simucube or Thrustmaster FFB patents and no motorsport pedal-force-sensor patents were identified. Next step: search Espacenet for "Endor AG force feedback" and "Granite Devices".

---

## Q6. Direct Drive wheel and load-cell pedal physical limits; real vs DD

### Takeaway
Fanatec publishes torque figures only. Its own pages conflict because of a May 2026 torque update. Encoder resolution, update rate, latency and slew rate are not officially published. The T-LCM is rated at 100 kg on the load cell with 16-bit resolution. It is a force sensor on a spring-and-elastomer pedal, so it reproduces force-to-input mapping but not hydraulic travel or stiffness behaviour. An active pedal (Simucube ActivePedal) can program force vs travel.

### Cited Findings
- Fanatec "Direct Drive Bases compared" (last updated 11 Mar 2026):
  - CSL DD: 5 Nm, or 8 Nm with Boost Kit 180.
  - GT DD Pro: 5 Nm, or 8 Nm with Boost Kit 180.
  - ClubSport DD: 12 Nm holding torque.
  - ClubSport DD+: 15 Nm.
  - Podium DD: 25 Nm.
  - The page claims "low latency" but gives no figure, no encoder spec and no update rate.
  - [Fanatec](https://www.fanatec.com/ww/en/explorer/products/racing-wheels-wheel-bases/whats-the-difference-between-csl-dd-gran-turismo-dd-pro-clubsport-dd-and-clubsport-dd/)
- The Fanatec Help "Wheel Base Torque FAQ" (search summary; the page returned 403) lists ClubSport DD+ up to 18 Nm and ClubSport DD up to 15 Nm after a "torque increase in May 2026". This conflicts with the March 2026 page. [Fanatec Help](https://help.fanatec.com/hc/en-us/articles/48812077676177-Wheel-Base-Torque-FAQ)
- GT DD Pro product page: 5 Nm peak, 8 Nm with Boost Kit 180, with a "high-resolution Hall position sensor" (no bit count given). [Fanatec](https://fanatec.com/us-en/detail/index/sArticle/1466)
- Encoder resolution:
  - Forum teardown claims a 12-bit MHL200 sensor.
  - Others say it is 12 bits per 4 mm magnetic period, interpolated to 16-bit data.
  - These conflict, are unofficial and are level E. [Fanatec forum](https://forum.fanatec.com/discussion/comment/100138); [comment 100092](https://forum.fanatec.com/discussion/comment/100092); [comment 100258](https://forum.fanatec.com/discussion/comment/100258)
- Thrustmaster T-LCM:
  - The brake load cell accepts up to 100 kg (220 lb) of pressure.
  - Resolution is 16-bit (65,536 values).
  - Six springs are included for mechanical feel.
  - Software sets brake force and dead zones.
  - Mass is 5.2-5.3 kg.
  - [Thrustmaster](https://thrustmaster.com/products/t-lcm-pedals); [Thrustmaster FR](https://www.thrustmaster.com/fr-be/products/t-lcm-pedals/); [Motorsport.com review](https://www.motorsport.com/gaming/news/thrustmaster-lcm-pedals-review/4684791/)
  - Note: 100 kg is the maximum accepted load. It is not necessarily the calibrated full-scale range, and the T-LCM's internal load-cell capacity is not published.
- Simucube ActivePedal [Simucube docs](https://docs.simucube.com/ActivePedal/Specifications.html); [Simucube](https://simucube.com/simucube-activepedal-pro):
  - Ultimate: up to 170 kg (default) or 120 kg (user configurable); travel 5-62 mm (default) or 5-79 mm; 480 W peak.
  - Pro: 1-110 kg force range, 74 mm max travel.
  - No update rate was found. One retailer page lists 150/120 kg, which conflicts.
- Academic FFB fidelity work:
  - An ECCOMAS Multibody 2015 abstract compared geared vs direct-drive motors over 0-30 Hz and concluded that direct drive is preferred. [ECCOMAS](http://congress.cimne.com/multibody2015/admin/files/fileabstract/a39.pdf)
  - Torque-based control reduces wheel oscillation and improves fidelity, and the friction model strongly affects stability. [IEEE 9606570](https://ieeexplore.ieee.org/document/9606570/)
  - The relevant bandwidth for steering feedback evaluation is debated, with proposals of 30-120 Hz. [arXiv 2504.11097](https://arxiv.org/pdf/2504.11097)
  - Further design references: Design Issues for Haptic Steering Force Feedback [ResearchGate](https://www.researchgate.net/publication/224091773_Design_Issues_for_Haptic_Steering_Force_Feedback_on_an_Automotive_Simulator); Chalmers force-feedback design thesis [PDF](https://publications.lib.chalmers.se/records/fulltext/208873/208873.pdf). These were read as snippets only.
- DD filters (static force reduction, slew-rate limiting) and quality metrics (clipping, latency, backlash, resolution) are summarized on Wikipedia. This is a secondary source. [Wikiwand](https://www.wikiwand.com/en/articles/Direct-drive_wheel)

### Inferences
- **Real-time GT3/DD torque ratio.** If the GT3 hand torque of about 6-8 Nm (level E) is correct:
  - an 8 Nm CSL DD or GT DD Pro could reproduce it at roughly 1:1 only with no headroom for kerb and impact transients;
  - a 15-18 Nm ClubSport DD/DD+ or the 25 Nm Podium DD leaves headroom.
  - This is DERIVED and ESTIMATED (level D), not published.
  - Recommended mapping: physical Nm = k × estimated real hand Nm, with k ≤ 1 and a soft limiter. A clipping metric should be logged.
- **What a DD cannot reproduce without extra hardware:**
  - the inertia of the real wheel, column and rack (only partly emulated in software);
  - true column compliance;
  - hydraulic assist dynamics, which can be approximated;
  - the real rim diameter and grip, which depend on the hardware rim;
  - vibration above the motor/controller bandwidth, which is unpublished for Fanatec;
  - cockpit vibration and chassis loads, which need transducers or motion.
- **Load-cell pedal.** The T-LCM measures force, but its travel is set by springs and elastomers. A software curve changes the force-to-brake-input mapping, not the physical pedal stiffness. Matching a GT3 force-travel curve requires either:
  - mechanical spring and elastomer changes; or
  - an active pedal such as the Simucube ActivePedal.

### Gaps
- No official Fanatec data were found for encoder resolution, FFB update rate, end-to-end latency, torque slew rate or small-signal bandwidth.
- No published, measured comparison of real GT3 hand torque vs DD output was found.

---

## Structured records

| id | manufacturer | model | generation | year | category | parameter | value | unit | min | max | conditions | sensor | source | url | pub_date | method | sample_rate | evidence_level | uncertainty | kind | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R01 | generic | GT3 class | n/a | n/a | GT3 | hand-wheel torque (typical cap) | 7 | Nm | 6 | 8 | unspecified, power-assisted | none stated | Fanatec article | https://www.fanatec.com/us/en/explorer/products/racing-wheels-wheel-bases/how-realistic-is-fanatec-torque-compared-to-a-real-car/ | n/a | marketing claim, no method | n/a | E | unknown | MANUFACTURER | vendor claim; some drivers prefer 10-12 Nm (same source) |
| R02 | generic | LMP2 | n/a | n/a | Prototype (non-GT3) | hand-wheel torque limit | 11 | Nm | | | "all tracks" | none stated | Fanatec article | same as R01 | n/a | "reported", no method | n/a | E | unknown | MANUFACTURER | non-GT3; do not transfer |
| R03 | generic | race cars (general) | n/a | n/a | Race (mixed) | average hand-wheel torque | | Nm | 8 | 15 | cornering; peaks higher | none stated | Fanatec article | same as R01 | n/a | claim | n/a | E | unknown | MANUFACTURER | mixed classes |
| R04 | generic | performance road cars, hydraulic PS | n/a | n/a | Road performance | dynamic hand-wheel torque | | Nm | 5 | 10 | dynamic driving | none stated | Fanatec article | same as R01 | n/a | claim | n/a | E | unknown | MANUFACTURER | non-GT3 |
| R05 | generic | GT3 class | n/a | n/a | GT3 | hand-wheel torque | | Nm | 8 | 12 | unspecified | none | Assetto Corsa forum | https://www.assettocorsa.net/forum/index.php?threads/how-stiff-are-the-steering-wheels-for-racecars-in-real-life-t300.44894/ | n/a | anecdote | n/a | E | unknown | ESTIMATED | user opinion |
| R06 | iRacing | GT3 car (sim) | n/a | n/a | GT3 (sim) | sim steering-column torque output | 60 | Nm | | 60 | in-game telemetry | sim model | Granite Devices forum | https://community.granitedevices.com/t/how-close-to-real-life-is-the-steering-forces-we-have-in-iracing/10267 | n/a | sim telemetry quote | n/a | E | unknown | SIMULATED | not physical hand torque |
| R07 | n/a | general drivers (US) | n/a | 1970s | Road (human factors) | max foot brake force, 5th pct female | 400 | N | | | instrumented vehicle | force transducer | Foot Brake Pedal Force Capability of Drivers (Scribd copy) | https://www.scribd.com/document/235921970/Foot-Brake-Pedal-Force-Capability-of-Drivers | n/a | measurement, 599 drivers | n/a | C | citation unverified | REAL | non-racing population |
| R08 | n/a | general drivers (US) | n/a | 1970s | Road (human factors) | recommended max brake-pedal effort | 378 | N | | | design guideline (85 lbf) | instrumented vehicle | HSRI HuF-6a | https://backend.production.deepblue-documents.lib.umich.edu/server/api/core/bitstreams/f3c97bb5-459e-4dc7-9c64-5b38ba5803b8/content | n/a | measurement-based guideline, 276 drivers | n/a | C | snippet only | REAL | road-car context only |
| R09 | n/a | racing simulator rig | n/a | ~2016 | Simulator | pedal force at 100% brake input | 375 | N | | | rig setting | strain gauges | Car Racing in a Simulator: Brake Pedal Stiffness | https://www.researchgate.net/publication/305317564_Car_Racing_in_a_Simulator_Validation_and_Assessment_of_Brake_Pedal_Stiffness | n/a | rig measurement | n/a | C | snippet only | SIMULATED | not a car value |
| R10 | n/a | Formula-style race car | n/a | n/a | Formula (non-GT3) | driver pedal force (back-calculated) | 1200 | N | | | from logged pressure and geometry | pressure transducer | LinkedIn (I. Platas) | https://www.linkedin.com/pulse/race-car-pedal-study-iv%C3%A1n-platas | n/a | calculation | n/a | E | unknown | DERIVED | blog-level; non-GT3 |
| R11 | Fanatec | CSL DD | QR2 | 2026 | DD wheelbase | peak torque | 5 | Nm | 5 | 8 | 8 Nm with Boost Kit 180 | n/a | Fanatec bases compared | https://www.fanatec.com/ww/en/explorer/products/racing-wheels-wheel-bases/whats-the-difference-between-csl-dd-gran-turismo-dd-pro-clubsport-dd-and-clubsport-dd/ | 2026-03-11 | spec | n/a | B | n/a | MANUFACTURER | |
| R12 | Fanatec | GT DD Pro | | 2026 | DD wheelbase | peak torque | 5 | Nm | 5 | 8 | 8 Nm with Boost Kit 180 | Hall position sensor | Fanatec product page | https://fanatec.com/us-en/detail/index/sArticle/1466 | n/a | spec | n/a | B | n/a | MANUFACTURER | encoder bits not published |
| R13 | Fanatec | ClubSport DD | | 2026 | DD wheelbase | holding torque | 12 | Nm | 12 | 15 | 15 Nm per FAQ after May 2026 update | n/a | Fanatec compared page; Torque FAQ | https://help.fanatec.com/hc/en-us/articles/48812077676177-Wheel-Base-Torque-FAQ | 2026 | spec | n/a | B | conflicting versions | MANUFACTURER | verify firmware version |
| R14 | Fanatec | ClubSport DD+ | | 2026 | DD wheelbase | torque | 15 | Nm | 15 | 18 | 18 Nm per FAQ after May 2026 update | n/a | Fanatec compared page; Torque FAQ | https://help.fanatec.com/hc/en-us/articles/48812077676177-Wheel-Base-Torque-FAQ | 2026 | spec | n/a | B | conflicting versions | MANUFACTURER | |
| R15 | Fanatec | Podium DD | | 2026 | DD wheelbase | torque | 25 | Nm | | | | n/a | Fanatec compared page | https://www.fanatec.com/ww/en/explorer/products/racing-wheels-wheel-bases/whats-the-difference-between-csl-dd-gran-turismo-dd-pro-clubsport-dd-and-clubsport-dd/ | 2026-03-11 | spec | n/a | B | n/a | MANUFACTURER | DD1/DD2 not separately listed |
| R16 | Fanatec | CSL DD / GT DD Pro | | n/a | DD wheelbase | position sensor resolution | 12 | bit | 12 | 16 | 12-bit hardware, possibly per 4 mm magnetic period; 16-bit interpolated data | MHL200 (claimed) | Fanatec forum | https://forum.fanatec.com/discussion/comment/100138 | n/a | teardown claim | n/a | E | conflicting | ESTIMATED | unofficial |
| R17 | Thrustmaster | T-LCM | | 2018- | Load-cell pedal | max load on brake load cell | 100 | kg | | 100 | max accepted load (≈981 N) | load cell | Thrustmaster | https://thrustmaster.com/products/t-lcm-pedals | n/a | spec | n/a | B | full-scale calibration not stated | MANUFACTURER | not a calibrated range |
| R18 | Thrustmaster | T-LCM | | 2018- | Load-cell pedal | resolution | 16 | bit | | | 65,536 values | load cell / ADC | Thrustmaster; Motorsport.com | https://www.motorsport.com/gaming/news/thrustmaster-lcm-pedals-review/4684791/ | n/a | spec | n/a | B | n/a | MANUFACTURER | |
| R19 | Simucube | ActivePedal Ultimate | | n/a | Active pedal | max force | 170 | kg | 120 | 170 | 170 default, 120 user-configurable | servo with force sensing | Simucube docs | https://docs.simucube.com/ActivePedal/Specifications.html | n/a | spec | n/a | B | retailer lists 150 kg | MANUFACTURER | |
| R20 | Simucube | ActivePedal Ultimate | | n/a | Active pedal | travel | | mm | 5 | 79 | 5-62 default, 5-79 configurable | servo | Simucube docs | https://docs.simucube.com/ActivePedal/Specifications.html | n/a | spec | n/a | B | n/a | MANUFACTURER | |
| R21 | Simucube | ActivePedal Pro | | n/a | Active pedal | force range | | kg | 1 | 110 | | servo | Simucube | https://simucube.com/simucube-activepedal-pro | n/a | spec | n/a | B | n/a | MANUFACTURER | 74 mm max travel |
| R22 | generic | pneumatic tyres (Pacejka) | n/a | n/a | Tyre (general, non-GT3) | relaxation length | | m | 0.12 | 0.45 | increases with speed and load | n/a | Wikipedia (attr. Pacejka) | https://en.wikipedia.org/wiki/Relaxation_length | n/a | secondary | n/a | D | wide | ESTIMATED | not GT3-specific |
| R23 | Wilwood (example) | generic brake hydraulics | n/a | n/a | Brake hydraulics | line pressure for 100 lbf at 5.2:1, 13/16" bore | 1003 | psi | | | worked example | n/a | Wilwood | https://shop.wilwood.com/blogs/news/pedal-ratios-and-how-to-find-them | n/a | calculation | n/a | C | n/a | DERIVED | illustrates force→pressure, not GT3 |
| R24 | n/a | on-centre test (Norman) | n/a | 1984 | Road (method) | test conditions | 0.2 | Hz | | | sinusoid, ~0.2 g ay | steering torque/angle, yaw rate | SAE 840069 (abstract) and later study | https://saemobilus.sae.org/papers/objective-evaluation-center-handling-performance-840069 | 1984 | method | n/a | C | abstract only | REAL | method, not a GT3 value |
| R25 | n/a | steering FFB research | n/a | 2025 | Simulator | relevant steering feedback bandwidth | | Hz | 30 | 120 | debated in literature | n/a | arXiv 2504.11097 | https://arxiv.org/pdf/2504.11097 | 2025 | literature review | n/a | C | debated | ESTIMATED | Fanatec bandwidth unpublished |
