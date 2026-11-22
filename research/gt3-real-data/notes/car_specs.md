# GT3 car specifications: official technical data for steering and braking models (per car and generation)

Research date: 2026-11-22. Scope: manufacturer-declared (MANUFACTURER kind) GT3 data for a steering/braking physics model (SimApex Physics). Evidence levels: A = direct verifiable measurement; B = official manufacturer/supplier data for that exact car; C = reconstructable/indirect (team or press reproducing manufacturer data, or a distributor); D = engineering estimate; E = unverified (Wikipedia, spec aggregators, fan sites).

Method note: primary PDFs (Porsche Newsroom press kit, BMW NA M4 GT3 info brochure, Audi Sport July 2021 press kit) were downloaded and their text extracted locally. Many official pages (ferrari.com Competizioni GT, lamborghini.com, mercedes-amg customer racing PDF brochure, bmw-m.com) render through JavaScript or returned HTTP 403/404 or empty content to the fetch tool. These are flagged as INACCESSIBLE, not as "no data exists". Roughly 20 tool calls were used, so about half of the requested cars could not be covered (see Gaps).

## Steering: type, power assistance, ratio, wheel diameter, lock-to-lock, quick-release/wheel supplier

### Takeaway
Manufacturers declare the steering *type* and assistance (Porsche 991.2 and 992: electro-hydraulic; Audi R8 LMS evo II: servo-assisted rack and pinion). No source found declares a GT3 steering ratio, steering wheel diameter or lock-to-lock for any car. These are NON DISPONIBILE and must not be filled with road-car figures.

### Cited Findings
- **Porsche 911 GT3 R (991.2, MY2019, released 11/05/2018):** "power-assisted, with electro-hydraulic pressure feed". The multi-function racing wheel has a quick-release coupling and shift paddles. The newsroom says all data was provisional pending FIA approval. — [Porsche Newsroom 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 911 GT3 R (992.1, MY2023, released 30/07/2022):** "Electro-hydraulic power steering". The car has a "multifunctional carbon-fibre steering wheel with quick release coupling". It has a "longitudinally adjustable pedal assembly and adjustable steering column with steering angle sensor". Steering wheel and pedals adjust longitudinally while the seat mount stays fixed. The 10.3-inch display comes from the 911 GT3 Cup. — [Porsche Newsroom 2022 (HTML)](https://newsroom.porsche.com/en_US/2022/motorsport/porsche-911-gt3-r-generation-992-customer-racing-car-premiere-29222.html); [press kit PDF](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **BMW M4 GT3 (G82-based, MY2022, brochure dated 2021-06):** "Adjustable steering column and pedalbox". An optional travel sensor kit for the brake pedal is listed. The brochure names Fanatec as a partner: "the BMW M4 GT3 podium steering wheel is the first steering wheel to be developed and built simultaneously for the simulator and for the real racing car by Fanatec". The steering type and assistance are not stated in the brochure. — [BMW NA M4 GT3 Infobrochure PDF](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **BMW road M4 (not GT3, do not use):** electromechanical rack-and-pinion steering, 15.0:1 overall ratio. This applies to the production car only. — [BMW press material (search snippet)](https://press.bmwgroup.com/global/article/attachment/T0316649EN/476824)
- **Audi R8 LMS GT3 evo II (2022 season, press kit July 2021):** "Servo-assisted rack and pinion steering". The kit lists a "Height- and length-adjustable safety steering column, multi-functional steering wheel" and "quick-adjustable pedals mounted on rails". — [Audi Sport press kit PDF, July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)
- **Lamborghini Huracán GT3 EVO2:** the official page lists steering wheel controls, including a TC knob with 10 maps, an ABS knob with 10 maps, pit limiter, radio, drink and TFT page knob. Steering type is not stated. — [Lamborghini Squadra Corse](https://www.lamborghini.com/en-en/motorsport/motorsport-models/huracan-gt3-evo2)

### Inferences
- Porsche states the steering assistance is electro-hydraulic, so the assistance is a hydraulic pump driven by an electric motor, not EPS. This matters for modelling: the rack-force feel is filtered through a hydraulic boost curve.
- The Porsche 992 has a "steering angle sensor" on the column, so real 992 GT3 R data loggers record steering angle. The ratio itself is still not published.
- The Fanatec Podium M4 GT3 wheel is the same design as the real car's wheel (BMW-declared). Its physical diameter could be measured from the Fanatec product (evidence A) and applied to the real M4 GT3. That is a measurement still to be done; no value is recorded here.

### Gaps
- Steering ratio, steering wheel diameter, lock-to-lock and maximum steering angle: NON DISPONIBILE for every car researched. No manufacturer, supplier or homologation source was found.
- Power steering supplier (for example ZF or Bosch) is not named for any car.
- BMW M4 GT3 / EVO steering type is not stated in the official brochure. It is NON DISPONIBILE, and the road-car EPS must not be used.
- Ferrari 488/296, Mercedes, McLaren, Corvette, Ford, Aston, Lexus, NSX, Bentley and Nissan: steering type was not found.

## Brakes: calipers, pistons, discs, material, balance, ABS

### Takeaway
Brake hardware is the best-documented area. Porsche 992 has AP calipers and discs: 6-piston with 390x35.7 mm discs at the front, 4-piston with 370x32.1 mm at the rear, steel, with Bosch ABS Gen 5. BMW M4 GT3 has 6-piston front and 4-piston rear, with 390x36 mm discs. Audi evo II has 380x36 mm front and 355x32 mm rear steel discs with 12-stage ABS. Master cylinder sizes and pedal ratios were not found for any car.

### Cited Findings
- **Porsche 911 GT3 R 991.2 (MY2019):** front six-piston aluminium monobloc calipers with ventilated multi-piece steel discs, 390 mm. Rear four-piston monobloc calipers with ventilated multi-piece steel discs, 370 mm. Two separate circuits with balance-bar adjustment by the driver. ABS is "BOSCH racing-ABS, generation 5". The brake supplier is not named. — [Porsche Newsroom 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 911 GT3 R 992 (MY2023):** "Racing brake callipers by the specialist AP". The brake discs "are now also supplied by the specialist company AP". Front: six-piston aluminium monobloc calipers with anti knock-back piston springs and "internally vented multi-piece 390 mm x 35.7 mm steel brake discs", with an aluminium disc chamber. Rear: four-piston, "370 mm x 32.1 mm steel brake discs". The system has "Two independent brake circuits incorporating front and rear axle brake pressure sensors, driver adjustable brake-force distribution via brake balance system". Brake temperature sensors are fitted. Pedal positioning allows left- and right-foot braking. ABS is "Generation 5 racing ABS from Bosch". — [Porsche press kit PDF 30/07/2022](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **Conflict:** an eBay part listing for a "992 GT3 / R" front disc gives Ø408x34 mm. It is unverified and probably a different part or variant. Porsche's 390 mm takes precedence. — [eBay listing](https://www.ebay.com/itm/176407601377)
- **BMW M4 GT3 (MY2022):** "6-piston, fixed caliper front 4-piston, fixed, caliper rear Diameter of brake discs (front / rear) 390 x 36 mm (15.35" x 1.42")". Only one disc size is printed, so the rear disc size is ambiguous (see Gaps). Fast couplings for brake lines and a quick-change system for calipers and discs are fitted. ABS is "Adjustable ABS". Electronics are by Bosch Motorsport ("manage the engine, transmission, ABS, traction control and displays"). Endless Brake Technology is listed as a partner (product role not specified). — [BMW NA M4 GT3 Infobrochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **Audi R8 LMS GT3 evo II (2022):** "Hydraulic dual-circuit braking system, variable brake pressure distribution (front axle/rear axle), steel brake discs front (380 x 36 mm) and rear (355 x 32 mm), racing ABS (twelve-stage)". Caliper supplier and piston count are not stated. — [Audi Sport press kit July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)
  - Note: a search-engine summary claimed "34 mm" front thickness. The primary PDF text reads 380 x 36 mm, so 36 mm is recorded.
- **Ferrari 296 GT3 (2023):** front 400 mm discs with six-piston calipers; rear 332 mm discs with four-piston calipers. This is press reproduction of Ferrari launch data. Ferrari's own pages were inaccessible (empty JavaScript-rendered content). — [CarExpert](https://carexpert.com.au/car-news/ferrari-296-gt3-racer-revealed); the same 400/332 mm and ABS front/rear appear for the 296 GT3 Evo on [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8813/Ferrari-296-GT3-Evo.html) (E). Brembo as supplier is not confirmed by any source found.
- **Mercedes-AMG GT3 Evo (2020-):** Bosch Motorsport M5 ABS with 12 adjustable stages; Bosch MS 6.2 ECU. — [Bosch Motorsport Shop Australia, technical profile](https://www.bosch-motorsport-shop.com.au/amg-gt3-evo) (C, a Bosch distributor)
- **Mercedes-AMG GT3 Edition 130Y Motorsport (special edition based on the GT3 Evo):** 6-piston front calipers with 390 mm discs, 4-piston rear calipers with 355 mm discs, ABS. This comes from the Mercedes-AMG customer racing news page as seen in a search snippet; the full page was not fetched. — [Mercedes-AMG Customer Racing news](https://customerracing.mercedes-amg.com/en/news/allnews/exclusive-small-series--the-mercedes-amg-gt3-edition-130y-motors.html). Ultimatecarpage calls the discs "carbon ceramic". — [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8692/Mercedes-AMG-GT3-Edition-130Y-Motorsport.html) (E, suspect)
- **Lamborghini Huracán GT3 EVO2 (2023):** the official page confirms ABS with 10 maps and TC with 10 maps. — [Lamborghini](https://www.lamborghini.com/en-en/motorsport/motorsport-models/huracan-gt3-evo2). Secondary sources (search snippets; pages not fetched) list TM Racing six-piston monobloc calipers and 380 mm front / 355 mm rear ventilated discs. — [Wayne Taylor Racing car specs](https://waynetaylorracing.com/customer/carspecs/gtd) (C, not verified by fetch); [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8305/Lamborghini-Huracan-GT3-EVO2.html) (E)
- **Chevrolet Corvette Z06 GT3.R (2024):** "Motorsport-specific 6-piston front / 4-piston rear brake system with specific cooling systems and quick-change capabilities". Supplier and disc size are not stated. — [TF Sport (customer team)](https://www.tfsport.co.uk/cars-details-corvette-gt3.php). Brembo and 14.6-inch rotor claims elsewhere refer to the road Z06, not the GT3.R. — [GM Authority](https://gmauthority.com/blog/2021/10/here-are-the-official-2023-corvette-z06-specs/)
- **McLaren 720S GT3 Evo (2023):** front brake 390 mm and rear 380 mm, from an aggregator. — [ConceptCarz](https://www.conceptcarz.com/s32741/mclaren-720s-gt3-evo.aspx) (E)
- **Ford Mustang GT3 Evo:** Ford says the Evo addresses "braking efficiency" among other areas. No figures are given. — via [Wikipedia](https://en.wikipedia.org/wiki/Ford_Mustang_GT3) (E)

### Inferences
- Disc diameters cluster between 380 and 400 mm at the front and 332 to 380 mm at the rear. Front calipers are 6-piston and rear calipers 4-piston on every car with data. A generic GT3 brake model can use these as bounds (DERIVED), but per-car values should carry only the declared figures.
- The "carbon ceramic" claim for the Mercedes 130Y is probably wrong. The other officially documented GT3 cars (Porsche, Audi) declare steel discs, and GT3 technical regulations generally require ferrous discs. This regulatory point was not verified against the SRO/FIA text in this session.
- Porsche 992 has front and rear axle pressure sensors, so real 992 GT3 R logs carry per-axle brake pressure. This is relevant for calibrating load-cell pedal force against line pressure, but no pressure-to-force data was found.

### Gaps
- Master cylinder bore sizes, pedal ratio, pedal force at lock-up and pedal box supplier: NON DISPONIBILE for all cars.
- BMW M4 GT3: rear disc dimension and caliper supplier are not stated. The brochure gives one "390 x 36 mm" figure under the "front / rear" label. BMW M4 GT3 EVO (2024/2025) brake changes were not found; the bmw-m.com EVO page did not render.
- Audi: caliper supplier and piston count are NON DISPONIBILE. The Audi 2019 (evo I) technical data PDF returned HTTP 404.
- Ferrari 488 GT3 / 488 GT3 Evo 2020, Huracán GT3 (2015) and Evo (2019), Aston Martin Vantage AMR GT3 / Evo, Lexus RC F GT3, Honda/Acura NSX GT3 / Evo / Evo22, Bentley Continental GT3, Nissan GT-R NISMO GT3 and Acura TLX-GT: not researched for lack of tool budget.

## Mass, dimensions, wheelbase, track, CoG

### Takeaway
Wheelbase and overall dimensions are declared for Porsche (991.2 and 992), BMW M4 GT3, Audi evo II and the Corvette (via team). Mass is always "BoP dependent". The declared numbers are the 992 R "ca. 1,250 kg" and the Audi evo II "1,235 kg dry weight acc. to homologation". No CoG height or weight distribution is published.

### Cited Findings
- **Porsche 991.2 GT3 R (MY2019):** length 4,629 mm; width 1,975 mm at the front axle and 2,002 mm at the rear axle; wheelbase 2,459 mm; weight "tbd (BoP dependent)"; fuel cell about 120 L FT3. — [Porsche Newsroom 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 992 GT3 R (MY2023):** "Base weight: ca. 1,250 kg (depending of BoP classification); wheelbase: 2,507 mm. Length: 4,619 mm; width: 2,039 mm (front axle) / 2,050 mm (rear axle)". The fuel cell is a 117-litre FT3, mounted at the front. — [Porsche press kit 30/07/2022](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **BMW M4 GT3 (MY2022):** length including splitter and rear wing 5,020 mm; width including mirrors 2,040 mm; height 1,308 mm (variable); wheelbase 2,917 mm. — [BMW NA brochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf). BMW's M4 GT3 EVO page also lists 2,917 mm (search snippet). — [bmw-m.com](https://www.bmw-m.com/en/fastlane/motorsport/race-cars/bmw-m4-gt3-evo.html)
- **Audi R8 LMS GT3 evo II:** length/width/height 4,599 / 1,997 / 1,171 mm; wheelbase 2,700 mm; dry weight according to homologation 1,235 kg; fuel cell 120 L FT3. The bare chassis weighs 252 kg. — [Audi press kit July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)
- **Ferrari 296 GT3:** wheelbase 2,660 mm; dry weight 1,250 kg. — [CarExpert](https://carexpert.com.au/car-news/ferrari-296-gt3-racer-revealed) (C). ConceptCarz's 102.36 in is likely the road 296 GTB (E, rejected).
- **Corvette Z06 GT3.R:** length 4,637 mm; width 2,050 mm; height 1,102 mm; wheelbase 2,722 mm. — [TF Sport](https://www.tfsport.co.uk/cars-details-corvette-gt3.php) (C)
- **Lamborghini Huracán GT3 EVO2:** wheelbase 2,648 mm; weight 1,230 or 1,239 kg depending on the source. — secondary sources only, [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8305/Lamborghini-Huracan-GT3-EVO2.html) (E)
- **McLaren 720S GT3 Evo:** 1,283 kg; wheelbase 2,670 mm per ConceptCarz or 2,696 mm per Ultimatecarpage (conflict). — [ConceptCarz](https://www.conceptcarz.com/s32741/mclaren-720s-gt3-evo.aspx), [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8447/McLaren-720S-GT3-Evo.html) (E)
- **Ford Mustang GT3:** 1,289 kg. — [Wikipedia](https://en.wikipedia.org/wiki/Ford_Mustang_GT3) (E)

### Inferences
- From 991.2 to 992 the Porsche wheelbase grew by 48 mm (2,459 to 2,507) and front body width by 64 mm (DERIVED from two B-level sources). Generations must not be mixed.
- Porsche "width at axle" figures are body widths at the axle, not track widths. True track widths are NON DISPONIBILE.

### Gaps
- Track widths (centre-to-centre), CoG height and static weight distribution: NON DISPONIBILE for all cars.
- SRO BoP mass and power tables per season were not retrieved in this session.

## Tyres and rims

### Takeaway
GT3 cars use 18-inch rims, 12.5J front and 13–13.5J rear, with slick sizes 30/68-18 front and 31/71-18 rear. This is declared by Porsche (both generations), BMW and Audi.

### Cited Findings
- **Porsche 991.2:** front BBS one-piece 12.5J x 18 with Michelin 300/680-18; rear 13J x 18 with 310/710-18. — [Porsche 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 992:** front 12.5J x 18 with 30/68-18; rear 13.5J x 18 with 31/71-18; TPMS fitted. — [Porsche 2022 PDF](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **BMW M4 GT3:** front 12.5 x 18 alloy, rear 13.0 x 18 alloy; slicks front 30/68-18 and rear 31/71-18; LDL TPMS optional. — [BMW NA brochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **Audi evo II:** forged aluminium wheels, front 12.5" x 18" ET45.65 and rear 13" x 18" ET43; tyres front 30-68/18, rear 31-71/18. — [Audi July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)

### Inferences
- The rim width going from 13J (991.2) to 13.5J (992) at the rear matches Porsche's stated aim of lower rear-tyre loads in the 992.

### Gaps
- The tyre supplier depends on the series (for example Pirelli in GTWC, Michelin in IMSA) and was not retrieved per season.

## Suspension and dampers

### Cited Findings
- **Porsche 991.2:** double wishbone front with forged aluminium arms; multi-link rear; anti-roll bars adjusted by blade position; shim set-up; 4-way adjustable dampers (supplier not named in the 2018 release). — [Porsche 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 992:** double wishbone front; multilink rear; "The KW shock absorbers were further improved and offer five adjustment settings"; sword-type anti-roll bars adjustable on both sides; spring travel potentiometer; shims. — [Porsche 2022 PDF](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **BMW M4 GT3:** "KW Motorsport shock absorbers front and rear, 5-way"; adjustable stabilisers front and rear; ride height via shims; optional spring-travel measurement. — [BMW NA brochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **Audi evo II:** "Double wishbones front and rear, suspension struts with coil springs and four-way gas pressure dampers", replacing earlier two-way dampers; three-way adjustable stabilisers. — [Audi July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)
- **Corvette Z06 GT3.R:** "Short/long arm (SLA) double wishbone, fabricated steel upper and lower control arms, coil-over adjustable shock absorbers". The supplier is not named; a Multimatic DSSV claim from a Medium blog is unverified (E). — [TF Sport](https://www.tfsport.co.uk/cars-details-corvette-gt3.php)
- **Huracán GT3 EVO2:** Öhlins TTX 36 dampers, from secondary sources only (E/C). — [Ultimatecarpage](https://www.ultimatecarpage.com/spec/8305/Lamborghini-Huracan-GT3-EVO2.html)

### Gaps
- Spring rates, motion ratios, damper curves and roll-bar rates: NON DISPONIBILE.

## Aero and electronics

### Cited Findings
- **Porsche 991.2:** Bosch MS 6.4 ECU; Cosworth CDU display and CLU logger with 6 GB memory; Porsche Motorsport Traction Control (PMTC). — [Porsche 2018](https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html)
- **Porsche 992:** Bosch MS 6.6 ECU; Cosworth electronic components; Porsche Logger Unit; 10.3-inch display. Aero has a swan-neck rear wing and diffuser; no downforce figures are given. — [Porsche 2022 PDF](https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f)
- **BMW M4 GT3:** Bosch MS 6 ECU; Bosch DDU 10 display; Bosch CAS-M3 optional; adjustable TC and ABS; up to 590 hp depending on BoP. — [BMW NA brochure](https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf)
- **Audi evo II:** Bosch Motorsport Motronic MS6.4; 12-stage ASR traction control, with new independent adjustment of apex and exit intervention; up to 430 kW. Aero: more of the downforce now comes from the rear wing and less from the floor, making the car "aerodynamically less sensitive under braking". No figures are given. — [Audi July 2021](https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download)
- **Mercedes-AMG GT3 Evo:** Bosch MS 6.2 ECU and M5 ABS (12 stages); DDU 9 display, superseded by DDU 10/11. — [Bosch Motorsport Shop AU](https://www.bosch-motorsport-shop.com.au/amg-gt3-evo) (C)
- **Huracán GT3 EVO2:** 10 TC maps, 10 ABS maps, manually adjustable rear wing. — [Lamborghini](https://www.lamborghini.com/en-en/motorsport/motorsport-models/huracan-gt3-evo2)

### Gaps
- Official downforce figures with a speed reference: none found for any car (NON DISPONIBILE).
- ABS model per car is known only for Porsche (Bosch Gen 5) and Mercedes (Bosch M5, from a distributor).

## SIMULATED (separate, not real data)
- iRacing BMW M4 GT3 manual: 12-position ABS map in three groups; double wishbones front and rear. — [iRacing manual](https://s100.iracing.com/wp-content/uploads/2024/07/BMW-M4-GT3-Manual_V3.pdf) (SIMULATED)
- iRacing Audi R8 LMS GT3 manual exists but was not used. — [iRacing](https://s100.iracing.com/wp-content/uploads/2023/10/UM-Audi-R8-LMS-GT3-Manual.pdf) (SIMULATED)

## Structured records

| id | manufacturer | model | generation | year | category | parameter | value | unit | min | max | conditions | sensor | source | url | pub_date | method | sample_rate | evidence_level | uncertainty | kind | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| POR991-01 | Porsche | 911 GT3 R | 991.2 | MY2019 | steering | assistance type | electro-hydraulic | - | | | | n/a | Porsche Newsroom | https://newsroom.porsche.com/en/motorsports/porsche-911-gt3-r-customer-racer-gt3-series-2019-racing-911-gt3-rs-aerodynamics-safety-15335.html | 2018-05-11 | press release | n/a | B | provisional pending FIA | MANUFACTURER | |
| POR991-02 | Porsche | 911 GT3 R | 991.2 | MY2019 | brakes | front disc diameter | 390 | mm | | | steel, ventilated, multi-piece | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | provisional | MANUFACTURER | 6-piston monobloc |
| POR991-03 | Porsche | 911 GT3 R | 991.2 | MY2019 | brakes | rear disc diameter | 370 | mm | | | steel | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | provisional | MANUFACTURER | 4-piston |
| POR991-04 | Porsche | 911 GT3 R | 991.2 | MY2019 | brakes | ABS | Bosch racing ABS Gen 5 | - | | | | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | |
| POR991-05 | Porsche | 911 GT3 R | 991.2 | MY2019 | mass | weight | NON DISPONIBILE | kg | | | BoP dependent | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | "tbd" |
| POR991-06 | Porsche | 911 GT3 R | 991.2 | MY2019 | dimensions | wheelbase | 2459 | mm | | | | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | provisional | MANUFACTURER | |
| POR991-07 | Porsche | 911 GT3 R | 991.2 | MY2019 | dimensions | length / width F / width R | 4629 / 1975 / 2002 | mm | | | width at axle | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | not track width |
| POR991-08 | Porsche | 911 GT3 R | 991.2 | MY2019 | tyres | rim F / R | 12.5J x 18 / 13J x 18 | in | | | BBS | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | tyres 300/680-18, 310/710-18 |
| POR991-09 | Porsche | 911 GT3 R | 991.2 | MY2019 | suspension | dampers | 4-way adjustable | - | | | | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | supplier not named |
| POR991-10 | Porsche | 911 GT3 R | 991.2 | MY2019 | electronics | ECU / display / TC | Bosch MS 6.4 / Cosworth CDU / PMTC | - | | | | n/a | Porsche Newsroom | same | 2018-05-11 | press release | n/a | B | | MANUFACTURER | |
| POR992-01 | Porsche | 911 GT3 R | 992.1 | MY2023 | steering | assistance type | electro-hydraulic | - | | | | steering angle sensor on column | Porsche press kit | https://newsroom.porsche.com/pdf/6c28be23-04fd-4367-8554-8982ebe0ac2f | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | quick-release wheel |
| POR992-02 | Porsche | 911 GT3 R | 992.1 | MY2023 | steering | ratio / wheel diameter / lock-to-lock | NON DISPONIBILE | - | | | | | | | | | | | | MANUFACTURER | |
| POR992-03 | Porsche | 911 GT3 R | 992.1 | MY2023 | brakes | front disc | 390 x 35.7 | mm | | | steel, internally vented, multi-piece, slotted | brake temp sensors | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | AP, 6-piston Al monobloc |
| POR992-04 | Porsche | 911 GT3 R | 992.1 | MY2023 | brakes | rear disc | 370 x 32.1 | mm | | | steel | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | AP, 4-piston |
| POR992-05 | Porsche | 911 GT3 R | 992.1 | MY2023 | brakes | brake balance | driver adjustable | - | | | 2 circuits | F/R pressure sensors | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | |
| POR992-06 | Porsche | 911 GT3 R | 992.1 | MY2023 | brakes | ABS | Bosch racing ABS Gen 5 | - | | | | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | |
| POR992-07 | Porsche | 911 GT3 R | 992.1 | MY2023 | mass | base weight | 1250 | kg | | | ca., BoP dependent | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | BoP | MANUFACTURER | |
| POR992-08 | Porsche | 911 GT3 R | 992.1 | MY2023 | dimensions | wheelbase | 2507 | mm | | | | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | |
| POR992-09 | Porsche | 911 GT3 R | 992.1 | MY2023 | dimensions | length / width F / width R | 4619 / 2039 / 2050 | mm | | | width at axle | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | |
| POR992-10 | Porsche | 911 GT3 R | 992.1 | MY2023 | tyres | rim F / R | 12.5J x 18 / 13.5J x 18 | in | | | | TPMS | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | tyres 30/68-18, 31/71-18 |
| POR992-11 | Porsche | 911 GT3 R | 992.1 | MY2023 | suspension | dampers | KW, 5-way | - | | | | spring travel potentiometer | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | DW front, multilink rear |
| POR992-12 | Porsche | 911 GT3 R | 992.1 | MY2023 | electronics | ECU / display | Bosch MS 6.6 / 10.3 in Porsche display | - | | | | | Porsche press kit | same | 2022-07-30 | press kit | n/a | B | | MANUFACTURER | Cosworth components, PLU |
| BMW-01 | BMW | M4 GT3 | G82-based | MY2022 | steering | wheel | Fanatec-built podium wheel | - | | | | | BMW NA brochure | https://s3.amazonaws.com/themes.limelightplatform.com/client/bmw-na/files/Other/BMW+M4+GT3+Infobrochure.pdf | 2021-06 (PDF metadata) | brochure | n/a | B | | MANUFACTURER | steering type NON DISPONIBILE |
| BMW-02 | BMW | M4 GT3 | G82-based | MY2022 | brakes | calipers F / R | 6-piston fixed / 4-piston fixed | - | | | | optional brake pedal travel sensor | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | supplier not stated |
| BMW-03 | BMW | M4 GT3 | G82-based | MY2022 | brakes | disc | 390 x 36 | mm | | | labelled "front / rear" | | BMW NA brochure | same | 2021-06 | brochure | n/a | B | rear may differ | MANUFACTURER | ambiguous axle |
| BMW-04 | BMW | M4 GT3 | G82-based | MY2022 | dimensions | wheelbase | 2917 | mm | | | | | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | EVO page gives the same |
| BMW-05 | BMW | M4 GT3 | G82-based | MY2022 | dimensions | L / W / H | 5020 / 2040 / 1308 | mm | | | L incl. splitter and wing; W incl. mirrors | | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | |
| BMW-06 | BMW | M4 GT3 | G82-based | MY2022 | tyres | rim F / R | 12.5 x 18 / 13.0 x 18 | in | | | | TPMS opt. | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | 30/68-18, 31/71-18 |
| BMW-07 | BMW | M4 GT3 | G82-based | MY2022 | suspension | dampers | KW Motorsport, 5-way | - | | | | spring travel opt. | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | adjustable ARB F/R |
| BMW-08 | BMW | M4 GT3 | G82-based | MY2022 | electronics | ECU / display | Bosch MS 6 / Bosch DDU 10 | - | | | | | BMW NA brochure | same | 2021-06 | brochure | n/a | B | | MANUFACTURER | adjustable TC and ABS |
| AUD-01 | Audi | R8 LMS GT3 | evo II | 2022 season | steering | type | servo-assisted rack and pinion | - | | | | | Audi Sport press kit | https://www.audi.com/en/audi-r8-lms-gt3-2021-14104/download | 2021-07 | press kit | n/a | B | | MANUFACTURER | |
| AUD-02 | Audi | R8 LMS GT3 | evo II | 2022 season | brakes | front disc | 380 x 36 | mm | | | steel | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | calipers NON DISPONIBILE |
| AUD-03 | Audi | R8 LMS GT3 | evo II | 2022 season | brakes | rear disc | 355 x 32 | mm | | | steel | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | |
| AUD-04 | Audi | R8 LMS GT3 | evo II | 2022 season | brakes | ABS | racing ABS, 12-stage | - | | | | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | variable F/R distribution |
| AUD-05 | Audi | R8 LMS GT3 | evo II | 2022 season | mass | dry weight acc. homologation | 1235 | kg | | | dry | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | BoP adds ballast | MANUFACTURER | |
| AUD-06 | Audi | R8 LMS GT3 | evo II | 2022 season | dimensions | L / W / H / wheelbase | 4599 / 1997 / 1171 / 2700 | mm | | | | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | |
| AUD-07 | Audi | R8 LMS GT3 | evo II | 2022 season | tyres | rim F / R | 12.5 x 18 ET45.65 / 13 x 18 ET43 | in | | | | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | 30-68/18, 31-71/18 |
| AUD-08 | Audi | R8 LMS GT3 | evo II | 2022 season | suspension | dampers | 4-way gas pressure | - | | | | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | DW F/R; supplier not stated |
| AUD-09 | Audi | R8 LMS GT3 | evo II | 2022 season | electronics | ECU / TC | Bosch MS6.4 / ASR 12-stage | - | | | | | Audi Sport press kit | same | 2021-07 | press kit | n/a | B | | MANUFACTURER | |
| FER296-01 | Ferrari | 296 GT3 | first | 2023 | brakes | front disc / calipers | 400 / 6-piston | mm | | | | | CarExpert | https://carexpert.com.au/car-news/ferrari-296-gt3-racer-revealed | 2022 | press report | n/a | C | | MANUFACTURER | ferrari.com inaccessible |
| FER296-02 | Ferrari | 296 GT3 | first | 2023 | brakes | rear disc / calipers | 332 / 4-piston | mm | | | | | CarExpert | same | 2022 | press report | n/a | C | | MANUFACTURER | |
| FER296-03 | Ferrari | 296 GT3 | first | 2023 | dimensions | wheelbase | 2660 | mm | | | | | CarExpert | same | 2022 | press report | n/a | C | | MANUFACTURER | Evo same per Ferrari (snippet) |
| FER296-04 | Ferrari | 296 GT3 | first | 2023 | mass | dry weight | 1250 | kg | | | dry | | CarExpert | same | 2022 | press report | n/a | C | | MANUFACTURER | |
| MER-01 | Mercedes-AMG | GT3 Evo | Evo | 2020- | brakes | ABS | Bosch M5, 12 stages | - | | | | | Bosch Motorsport Shop AU | https://www.bosch-motorsport-shop.com.au/amg-gt3-evo | n/d | distributor profile | n/a | C | | MANUFACTURER | ECU Bosch MS 6.2 |
| MER-02 | Mercedes-AMG | GT3 Edition 130Y | Evo-based | 2022 | brakes | discs F / R, calipers | 390 / 355, 6p / 4p | mm | | | | | M-AMG Customer Racing news | https://customerracing.mercedes-amg.com/en/news/allnews/exclusive-small-series--the-mercedes-amg-gt3-edition-130y-motors.html | n/d | search snippet | n/a | C | not fetched | MANUFACTURER | carbon-ceramic claim (Ultimatecarpage) suspect |
| LAM-01 | Lamborghini | Huracán GT3 EVO2 | EVO2 | 2023 | electronics | TC / ABS maps | 10 / 10 | maps | | | | | Lamborghini | https://www.lamborghini.com/en-en/motorsport/motorsport-models/huracan-gt3-evo2 | n/d | official page | n/a | B | | MANUFACTURER | |
| LAM-02 | Lamborghini | Huracán GT3 EVO2 | EVO2 | 2023 | brakes | discs F / R | 380 / 355 | mm | | | TM Racing 6-piston calipers | | WTR / Ultimatecarpage | https://waynetaylorracing.com/customer/carspecs/gtd | n/d | search snippet | n/a | E | not verified | MANUFACTURER | |
| LAM-03 | Lamborghini | Huracán GT3 EVO2 | EVO2 | 2023 | dimensions | wheelbase | 2648 | mm | | | | | Ultimatecarpage | https://www.ultimatecarpage.com/spec/8305/Lamborghini-Huracan-GT3-EVO2.html | n/d | aggregator | n/a | E | | MANUFACTURER | weight 1230–1239 kg conflict |
| COR-01 | Chevrolet | Corvette Z06 GT3.R | C8 | 2024 | brakes | calipers F / R | 6-piston / 4-piston | - | | | | | TF Sport | https://www.tfsport.co.uk/cars-details-corvette-gt3.php | n/d | team page | n/a | C | | MANUFACTURER | supplier and disc NON DISPONIBILE |
| COR-02 | Chevrolet | Corvette Z06 GT3.R | C8 | 2024 | dimensions | L / W / H / wheelbase | 4637 / 2050 / 1102 / 2722 | mm | | | | | TF Sport | same | n/d | team page | n/a | C | | MANUFACTURER | SLA double wishbone |
| MCL-01 | McLaren | 720S GT3 Evo | Evo | 2023 | brakes | discs F / R | 390 / 380 | mm | | | | | ConceptCarz | https://www.conceptcarz.com/s32741/mclaren-720s-gt3-evo.aspx | n/d | aggregator | n/a | E | | MANUFACTURER | |
| MCL-02 | McLaren | 720S GT3 Evo | Evo | 2023 | mass | weight | 1283 | kg | | | | | ConceptCarz | same | n/d | aggregator | n/a | E | | MANUFACTURER | wheelbase 2670 vs 2696 conflict |
| FOR-01 | Ford | Mustang GT3 | S650 | 2024 | mass | weight | 1289 | kg | | | | | Wikipedia | https://en.wikipedia.org/wiki/Ford_Mustang_GT3 | n/d | encyclopedia | n/a | E | | MANUFACTURER | |
| SIM-01 | BMW | M4 GT3 | G82-based | - | brakes | ABS map positions | 12 (3 groups) | - | | | iRacing | | iRacing manual | https://s100.iracing.com/wp-content/uploads/2024/07/BMW-M4-GT3-Manual_V3.pdf | 2024-07 | sim manual | n/a | E | | SIMULATED | not real data |

### Compact per-car summary

| Car (gen, year) | Steering | Brakes F / R | ABS | Mass | Wheelbase | Rims F / R | Dampers | Best evidence |
|---|---|---|---|---|---|---|---|---|
| Porsche 911 GT3 R 991.2 (MY2019) | electro-hydraulic | 6p, 390 steel / 4p, 370 steel | Bosch Gen 5 | BoP (tbd) | 2459 | 12.5J / 13J x18 | 4-way | B |
| Porsche 911 GT3 R 992.1 (MY2023) | electro-hydraulic, steering angle sensor | AP 6p, 390x35.7 / AP 4p, 370x32.1 steel | Bosch Gen 5 | ca. 1250 (BoP) | 2507 | 12.5J / 13.5J x18 | KW 5-way | B |
| BMW M4 GT3 (MY2022) | type n/d; Fanatec wheel | 6p / 4p; disc 390x36 (axle ambiguous) | adjustable (Bosch electronics) | n/d | 2917 | 12.5 / 13.0 x18 | KW 5-way | B |
| BMW M4 GT3 EVO (2024+) | n/d | n/d | n/d | n/d | 2917 (snippet) | 12.5 / 13 x18 (snippet) | n/d | C |
| Audi R8 LMS GT3 evo II (2022) | servo-assisted rack and pinion | 380x36 / 355x32 steel | 12-stage | 1235 dry (homol.) | 2700 | 12.5 / 13 x18 | 4-way | B |
| Ferrari 296 GT3 (2023) | n/d | 6p, 400 / 4p, 332 | yes | 1250 dry | 2660 | n/d | n/d | C |
| Mercedes-AMG GT3 Evo (2020) | n/d | 6p, 390 / 4p, 355 (130Y edition) | Bosch M5, 12 stages | n/d | n/d | n/d | n/d | C |
| Lamborghini Huracán GT3 EVO2 (2023) | n/d | TM 6p; 380 / 355 (unverified) | 10 maps | 1230–1239 (E) | 2648 (E) | n/d | Öhlins TTX36 (E) | B/E |
| Corvette Z06 GT3.R (2024) | n/d | 6p / 4p | n/d | n/d | 2722 | n/d | coil-over, n/d | C |
| McLaren 720S GT3 Evo (2023) | n/d | 390 / 380 (E) | n/d | 1283 (E) | 2670/2696 conflict | n/d | n/d | E |
| Ford Mustang GT3 (2024) | n/d | n/d | n/d | 1289 (E) | n/d | n/d | n/d | E |
| Ferrari 488 GT3/Evo, Huracán GT3/Evo, Audi evo I, Aston Vantage AMR GT3, Lexus RC F GT3, NSX GT3/Evo/Evo22, Bentley Continental GT3, Nissan GT-R GT3, Acura TLX-GT | not researched (tool budget) | | | | | | | — |

Steering ratio, steering wheel diameter, lock-to-lock, master cylinder sizes, track widths, CoG and official downforce: NON DISPONIBILE for every car.
