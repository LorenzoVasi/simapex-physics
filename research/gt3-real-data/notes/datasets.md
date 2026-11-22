# Real GT3 and other real race-car telemetry: publicly downloadable data-acquisition files (for SimApex Physics)

Research date: 2026-11-22. Downloaded files for inspection are under
`C:\Users\loren\AppData\Local\Temp\claude\2a2275ea99b2c258\a7ee3b02-82bc-4be7-9757-b2d8a7569554\scratchpad\datasets\` (they are not in the repo and none were executed). The headers were inspected with a read-only Node.js script I wrote, `...\scratchpad\scripts\ldhead.js`, which implements the MoTeC .ld layout used by gotzl/ldparser. Python was not available on this machine. For the Bosch and Pi binaries I only extracted strings (`strs.js`).

Evidence levels: A = direct verifiable measurement, B = official manufacturer or supplier, C = reconstructable or indirect, D = engineering estimate with stated uncertainty, E = unverified.

## Q1. Are real GT3 logs downloadable (MoTeC, WinDarab, Pi/Cosworth, WinTAX, AiM, CSV, MAT, MDF, TDMS)?

### Takeaway
I found **no file that is provably from a homologated GT3 car with a stated reuse licence**. The closest material I actually downloaded and verified is course sample data hosted publicly by High Performance Academy (HPAcademy):
- a **MoTeC C125 log from an unnamed "GT car"** (Highlands Motorsport Park, NZ, 2017). It has steering angle, front and rear brake pressure, four wheel speeds, lateral and longitudinal g, yaw rate, 500 Hz damper positions, TPMS pressures and temperatures, throttle, rpm and gear.
- a **Bosch WinDarab (C60 logger) log from a Ferrari 488 GTE** (GTLM-class, a sibling of the 488 GT3) at Daytona and Sebring. Its embedded paths contain "488GTE" and "F142MGT".

Both are clearly real-car data, not simulator output. Neither states a licence, so reuse rights are unclear. GT3 class is **not confirmed** for the MoTeC file, and the Bosch file is GTE, not GT3.

### Cited Findings
- HPAcademy forum post "example logged data and software tools download 1" (Professional Motorsport Data Analysis course) links three ZIPs:
  - Bosch WinDarab ("Data is from a GT car at two different circuits")
  - Pi Toolbox ("2 different touring cars and drivers")
  - MoTeC i2 (GT car, two drivers)
  
  No licence or terms are stated on the post — [HPAcademy forum](https://www.hpacademy.com/forum/professional-motorsport-data-analysis/show/example-logged-data-and-software-tools-download-1/)
- **DOWNLOADED AND VERIFIED: HPA MoTeC i2 ZIP.** URL: https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/6812cc9282/MoTeC-I2.zip?vid=2 (HTTP 200, 1,590,554 bytes). It contains `20170407-1005900.ld` (10.1 MB) and `20170407-1005901.ld` (5.6 MB). — [HPAcademy forum](https://www.hpacademy.com/forum/professional-motorsport-data-analysis/show/example-logged-data-and-software-tools-download-1/)
  - Header, from my own parse:
    - logger device "C125", serial 10059, firmware/device version 610
    - date 07/04/2017, times 09:45:33 and 10:49:47
    - drivers anonymised as "Driver A" and "Driver B"; vehicle id "#22"; team "Team A"
    - venue "Highlands Motorsport Park"
    - event "2017 Highlands Test Day 1", session "Morning Session"
    - short comments "DL04" and "DL05"
    - long comments "Driver A in for qualy sim. RARB adj. to M-M from H-M" and "Driver B in for qualy sim. Same setup as DL04"
  - 52 channels:
    - Steering Angle (20 Hz)
    - Brake Pres F and Brake Pres R (20 Hz)
    - Wheel Speed FL/FR/RL/RR (20 Hz)
    - LAT-G Car and LONG-G Car (20 Hz), G Force Vert (25 Hz)
    - Vehicle Yaw Rate (20 Hz)
    - Damper Pos FL/FR/RL/RR (**500 Hz**)
    - Throttle Pedal (20 Hz), Engine Speed (10 Hz), Gear (10 Hz), Ground Speed (20 Hz)
    - Tyre Pres FL..RR and Tyre Temp FL..RR (5 Hz)
    - Lambda Left/Right, Fuel Pump 1/2 Current, Bat Volts Dash, oil, fuel, gearbox and coolant pressures and temperatures, Inlet Manifold Pressure, lap and beacon channels
  - **No steering torque and no brake pedal force.**
  - The per-channel unit fields are empty in the .ld. Decoded value ranges for file ...900 suggest these units:
    - Ground Speed 0–245 (km/h)
    - Steering Angle −273.8…+274.9 (deg at the wheel)
    - Brake Pres F max 8101 and R max 4562 (likely kPa, i.e. about 81 and 46 bar)
    - LAT-G −2.41…+2.22 g, LONG-G −2.50…+1.25 g
    - Engine Speed max 8394 rpm, Gear −1…6
    - Damper Pos 0–49 mm
    - Tyre Pres about 150–280 (kPa; 637.5 is a no-signal sentinel)
    - Tyre Temp 50–110 °C (255 is a sentinel)
    - Reference lap time 90.24 s
  - Vehicle Yaw Rate is effectively zero throughout (−0.036…0), so that channel looks unpopulated or mis-scaled.
  - Exact car model: **not identified.** HPA says only "GT car".
- **DOWNLOADED AND VERIFIED: HPA Bosch WinDarab ZIP.** URL: https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/2672e01cb1/Bosch-Windarab.zip?vid=2 (8,951,759 bytes). It contains `Daytona.bmsbin` (5.7 MB) and `Sebring.bmsbin` (18.2 MB). — [HPAcademy forum](https://www.hpacademy.com/forum/professional-motorsport-data-analysis/show/example-logged-data-and-software-tools-download-1/)
  - File magic "Darab 7.07.036". ToolInfo "RaceCon 2.5.1.605" (Daytona) and "RaceCon 2.5.1.612" (Sebring). Config "C60_BASE_0727", "SC_3808_C60".
  - Embedded project paths:
    - `...Daytona_USCC\Elettronica\488GTE\C60\RLP\F142MGT_C60Log727_TEAM_049_CAS.rlp`
    - `_Sebring 12h\Elettronica\488GTE\C60\RLP\488GT_C60Log727_TEAM_063_CAS_SC3808.rlp`
    
    These point to a **Ferrari 488 GTE** run by a team in the IMSA (USCC) championship at Daytona and the Sebring 12h, logged on a Bosch C60.
  - Channel names found:
    - Steering, brakes and dynamics: steer, yaw, pbrake_f, pbrake_r, pclutch, accx, accy, accz
    - Wheel speeds: vwheel_fl/fr/rl/rr; speed
    - Suspension and ride height: trvdam_fl/fr/rl/rr (damper travel), LOAD_FL/FR/RL/RR, laser_front, laser_RL, laser_RR (ride height)
    - Brake temperatures: t_brake_fl/fr/rl/rr
    - Tyres (TPMS): Pres_FL..RR, Temp_FL..RR
    - Driver inputs and powertrain: aps (pedal), ath (throttle), nmot (rpm), gear
    - TC and ABS maps: tcstage, absmappos_can
    - Sample-rate blocks: 1 s, 500 ms, 200 ms, 100 ms, 20 ms and 10 ms (so up to 100 Hz)
  - A channel named "redact" is present, which suggests the file was sanitised.
  - Date of the sessions: not decoded. The ZIP entries are dated 2021-02/03.
- **DOWNLOADED AND VERIFIED: HPA "Pi Toolbox" ZIP.** URL: https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/f180d2d844/Pi-Toolbox.zip?vid=2 (41,507,761 bytes). It contains `Driver_A-001.pds` (112.9 MB) and `Driver_B-001.pds` (90.9 MB). — [HPAcademy forum](https://www.hpacademy.com/forum/professional-motorsport-data-analysis/show/example-logged-data-and-software-tools-download-1/)
  - Metadata string: "Source: Offload; Device Type: Omega L2; Variant: 'L2 Ultra'". Logger serials 792 and 428.
  - Venue and year: "Croft\2017". Chassis: "Chassis 34 #71" and "Chassis 72 #428".
  - The embedded path says "Course Sample Data\Pi Touring Car". This is a **touring car, not GT3.**
  - Channels:
    - Steering: Angle_Steering_Omega, **Power Steering Press**
    - Brakes: Press_Brake F_Omega, Press_Brake R_Omega, Brake Bias
    - Suspension: Disp_Damper FL..RR, Damper Vel FL/FR
    - Wheel speeds: Rot_Speed and Speed FL..RR_Omega
    - Accelerations: Lateral Acceleration, Longitudinal Acceleration
    - Powertrain: RPM_Engine_ECU, Stat_Gear Pos_ECU, Angle_Throttle A/B_ECU, Press_Air Restrictor_ECU
- **DOWNLOADED AND VERIFIED: HPA "Data Analysis Fundamentals" sample data.** — [HPAcademy forum (off-topic copy)](https://www.hpacademy.com/forum/off-topic-discussion/show/example-logged-data-and-software-tools-download)
  - MoTeC ZIP: https://www.hpacademy.com/assets/Course-Content/Data-Fundamentals-/c6d3af1dbb/03-MOTEC-EXAMPLE-LOGGED-DATA.zip (2.7 MB)
    - Contents: 2× .ld and 2× .ldx from a MoTeC C125 (serial 3642), vehicle "Toyota GT86", Highlands Motorsport Park, 30/09/2020, "Test Day 3 30AUG20".
    - .ldx metadata: weight 1250 kg, fastest lap 1:47.171, locale "English_New Zealand.1252".
    - 43 channels, including Steering Angle 20 Hz, a single Brake Pres 20 Hz, four wheel speeds 20 Hz, G Force Lat/Long 50 Hz, Vehicle Yaw Rate 20 Hz, Engine RPM 50 Hz, Gear, Throttle Pedal, tyre pressures and GPS.
    - No dampers.
  - AiM ZIP: https://www.hpacademy.com/assets/Course-Content/Data-Fundamentals-/3f224dca6b/01-AIM-EXAMPLE-LOGGED-DATA.zip (4.9 MB), three AiM .drk files:
    - **"Porsche 997" at Daytona** (header date string 19-02-07). Channels: RR/RF/LR/LF Damper, LatAcc, SPEEDFL/FR/RL/RR, GPS_Speed, GPS_LatAcc, GPS_LonAcc, GPS_Gyro, Lambda. The variant (GT3 Cup or other) is **not stated.**
    - **"Jaguar" at Misano**: ECU and engine channels plus accelerations.
    - **"Toyota GT86 V8" at Ruapuna**, South Island Endurance Series 2019 race (MoTeC M1 ECU over CAN). Channels: BRK_PRESS_F, BRK_PRESS_R, **STEER_PRESS** (power-steering pressure), RollRate, PitchRate, YawRate, LateralAcc, InlineAcc, VerticalAcc, ECU_WH_SP_FL..RR, ECU_RPM, ECU_GEAR, ECU_PEDAL_POS, ECU_THROTTLE.
  - The VBOX sample is on a Google Drive folder and was **not inspected**.
- Rennlist forum: a member offered real 981 Clubsport (Porsche Cayman GT4 Clubsport predecessor) MoTeC files **only via private message**. This is not a public dataset. — [Rennlist](https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363608-vir-iracing-data-log-telemetry.html) (via search snippet only)
- MoTeC i2 Pro is free for non-professional use. Sources say i2 ships with sample logs:
  - an old Silicon Chip article says so — [Silicon Chip](https://siliconchip.com.au/Issue/2006/August/MoTeC+Race+Car+Data+logging%2C+Pt.2)
  - j-antunes/ldparser says it was "tested with the sample ld files that come with a MoTec Mi2 Pro installation" — [j-antunes/ldparser](https://github.com/j-antunes/ldparser)
  
  I did **not** install i2 or inspect these samples. — [MoTeC i2](https://www.motec.com.au/i2/)
- AiM's XRK DLL MATLAB example is reported to be supplied with two sample files (not inspected; car unknown). — [AiM XRK DLL docs](https://docs.aim-sportline.com/racestudio3/html/xrk-dll.html); [MathWorks File Exchange "Read AIM Xrk Files"](https://jp.mathworks.com/matlabcentral/fileexchange/180222-read-aim-xrk-files)
- I did not find any GT3 dataset on Zenodo, Figshare, Kaggle, Mendeley or in GitHub searches during this session. The Zenodo hit 3967879 is a **synthetic** fleet dataset (OpenXC model-derived). — [Zenodo 3967879](https://zenodo.org/record/3967879) (via search snippet)

### Inferences
- **Why the HPA MoTeC and Bosch files are REAL, not SIM:**
  - Hardware-specific metadata: logger model C125 or C60, logger serial numbers, RaceCon and WinDarab tool versions, ECU calibration names, a Pi "Offload" device string with boot-loader dates.
  - Physical-sensor artefacts: no-signal sentinels (tyre pressure 637.5, tyre temperature 255), lambda saturating at 4.0, fuel-pump currents, battery volts, a broken or unpopulated yaw channel.
  - Engineer comments about ARB changes, and team project folders ("Elettronica\488GTE").
  - None of the files contains ACC, iRacing or rFactor names. The channel names are logger-native MoTeC names (e.g. "Damper Pos FL", "Brake Pres F") and Bosch/Ferrari ECU names (pbrake_f, trvdam_fl, nmot, aps), not the uppercase sim-plugin channel sets.
  - Evidence level for "real measurement": **A**. Evidence level for "is GT3": **E** for the MoTeC GT car (class and model unknown). For the 488 GTE: car identity **C** (reconstructed from embedded paths), and it is not GT3.
- **Car identity of the HPA GT car is undetermined.** The channel set (twin lambda, about 8400 rpm, 6-speed gearbox, a C125 dash) is consistent with a front-line GT car, but it does not identify one. I would not label it GT3 without confirmation from HPA.
- **Licensing:** HPA publishes these openly as course resources but states no licence. For SimApex Physics, treat all HPA files as "publicly accessible, reuse rights unclear". Using them for internal method validation is low risk. Redistribution or commercial calibration should require written permission from HPAcademy.
- **The 488 GTE file is factory and team customer-racing data** from an IMSA programme, shared by a training provider. Its authorisation chain cannot be verified, so treat it as **use only with HPA confirmation**. I found no evidence that it is leaked, but no explicit licence either.
- **Missing channels:** none of the downloaded GT/GTE files has **steering torque** or **brake pedal force**. Two touring-type files have power-steering pressure, which is a partial proxy for steering effort, but it is not column torque.

### Gaps
- The exact GT car model or year in the HPA MoTeC data is unknown. HPA could be asked directly.
- I could not decode the session date or the per-channel units for the Bosch .bmsbin files (no open parser found). A free WinDarab installation is needed for full inspection.
- I did not inspect the MoTeC i2 Pro bundled sample projects, AiM XRK DLL samples, Bosch WinDarab demo data, Cosworth Toolbox demo data or Magneti Marelli WinTAX demo data, because they require installing vendor software.
- I found no Porsche Motorsport or customer-racing public data packs, no official GT3 training course with downloadable data other than HPA, and no driver-coaching real-car GT3 packs. The coaching sites found (Coach Dave and others) are sim-only.
- I did not search Figshare, Mendeley or Kaggle directly through their site UIs; I used web search only.

## Q2. Which real non-GT3 race-car and vehicle-dynamics datasets with relevant channels exist?

### Takeaway
The strongest **openly licensed** real datasets are:
- **Stanford Revs Vehicle Dynamics Database** (ODC-By 1.0): vintage race cars at Laguna Seca and Palm Beach, with handwheel angle, brake pedal *force* (%), yaw rate, sideslip, suspension deflection, CSV and MAT files.
- **RACECAR** (CC BY-NC 4.0): Dallara AV-21 Indy autonomous cars, ROS 2.
- **commaSteeringControl** (MIT): real road cars with normalised steering torque.

The HPA files above add a GT86 (MoTeC and AiM), a Porsche 997 (AiM), a Jaguar (AiM) and a touring car (Pi), all with unclear licence.

### Cited Findings
- **Stanford Revs Vehicle Dynamics Database** (ODC Attribution v1.0) — [Revs collection](https://purl.stanford.edu/cv083fq4881)
  - Items:
    - 2013 Monterey Motorsports Reunion, Laguna Seca: 1963 Corvette Grand Sport, two drivers, eight sessions — [purl tt103jr6546](https://purl.stanford.edu/tt103jr6546)
    - 2013 Targa Sixty-Six, Palm Beach International Raceway, 2013-02-22/23: Corvette Grand Sport — [purl yf219gg2055](https://purl.stanford.edu/yf219gg2055)
    - 2014 Targa Sixty-Six: 1965 Ferrari 250 LM, six sessions — [purl hd122pw0365](https://purl.stanford.edu/hd122pw0365)
  - Each session has a .csv and a .mat file plus a vehicle-parameter CSV.
  - **Verified by download:** `channels.txt` lists (with units):
    - driver inputs: handwheelAngle (deg), throttle (%), brake ("brake pedal force in percent"), clutch, engineSpeed
    - GNSS/INS: position and velocities, axCG/ayCG/azCG (m/s²), roll/pitch/yawRate (deg/s), sideSlip (deg)
    - corner sensors: chassisAccelFL..RR, deflectionFL..RR (m), wheelAccelFL..RR
    
    — [channels.txt](https://stacks.stanford.edu/file/druid:hd122pw0365/channels.txt)
  - **Verified by download:** `250lm.csv` gives wheelbase 2.4 m, mass 982 kg, corner masses, diff and gear ratios, motion ratios 3.0/4.0, and a steering-ratio table (handwheel −345° → roadwheel −25.5°/−27°). — [250lm.csv](https://stacks.stanford.edu/file/druid:hd122pw0365/250lm.csv)
  - No wheel speeds, brake pressure or steering torque appear in channels.txt. The sample rate is not stated on the item pages.
  - Related paper: Kegelman, Harbott & Gerdes, Vehicle System Dynamics. — [Stanford DDL](https://ddl.stanford.edu/publications/journal/insights-vehicle-trajectories-handling-limits-analysing-open-data-race-car)
- **RACECAR dataset** (Indy Autonomous Challenge 2021–22, Dallara AV-21) — [AWS Open Data](https://registry.opendata.aws/racecar-dataset/); [HF README](https://huggingface.co/datasets/madhurbehl/RACECAR_DATA); [arXiv 2306.03252](https://arxiv.org/abs/2306.03252v1)
  - 27 sessions, more than 6.5 h of data, speeds up to about 170 mph.
  - Sensors: LiDAR, radar, cameras and GNSS/IMU.
  - Formats: ROS 2 bags and nuScenes. Downloadable anonymously from `s3://racecar-dataset` with `--no-sign-request`.
  - Licence **CC BY-NC 4.0**.
  - Status: identified only. I did not download it, so I did not check whether vehicle CAN channels (steering, brake) are in the bags.
- **A2RL V_max** (2024 Yas Marina): perception-focused (LiDAR, radar). The EAV24 (Dallara SF23-based) steering telemetry used in arXiv 2507.20427 is **not confirmed public**. — [arXiv 2607.17813](https://arxiv.org/abs/2607.17813); [arXiv 2507.20427](https://arxiv.org/pdf/2507.20427)
- **Toyota GR Cup North America ("Hack the Track" 2025, GR86)**
  - The official Devpost dataset overview PDF (verified by reading) covers results, lap and sector timing, weather and lap-timing events. — [overview PDF](https://devpost-public.s3.amazonaws.com/Toyota/gr_hackathon_dataset_overview.pdf)
  - A community repository lists telemetry channels pbrake_f and pbrake_r (bar), accx_can, accy_can, Steering_Angle, aps, ath, Speed, Gear, nmot, and VBOX lat/long for 7 US tracks. — [Novapool/hack_the_track](https://github.com/Novapool/hack_the_track)
  - The terms are **unknown**: the rules page returned 403. Status: identified only.
- **Edinburgh University Formula Student ROS 2 dataset** (CC-BY-4.0, MCAP format: lidar, wheel encoders, IMU). This is driverless FS. — [EIDF catalogue](https://catalogue.eidf.ac.uk/dataset/groups/eidf156-edinburgh-university-formula-student)
- **commaSteeringControl**
  - About 12,500 h of openpilot-engaged driving across hundreds of car models.
  - Steering torque is normalised to [−1, 1] ("steer" and "steerFiltered"), so **no absolute N·m**.
  - Licence MIT.
  
  — [HF commaai/commaSteeringControl](https://huggingface.co/datasets/commaai/commaSteeringControl/tree/main); [ecosyste.ms summary](https://awesome.ecosyste.ms/projects/github.com%2Fcommaai%2Fcomma-steering-control)
- **Instrumented steering wheel dataset** (Politecnico di Milano, Data in Brief 2020)
  - Kick-plate test, 9 drivers × 6 passes, MATLAB files.
  - Contains steering-wheel force and torque measurements.
  - The authors state "research purposes only". The vehicle was modified.
  
  — [PMC7214765](https://pmc.ncbi.nlm.nih.gov/articles/PMC7214765)
- **VTT (Vehicle Power Type Trajectory) dataset**: 10 Hz CAN data including normalised steering torque. — [HF HenryYHW/Vehicle_Power_Type_Trajectory_Dataset](https://huggingface.co/datasets/HenryYHW/Vehicle_Power_Type_Trajectory_Dataset) (via search snippet)
- **NHTSA handling test reports** list handwheel torque measured with strain gauges (N·m). NHTSA notes that very little public instrumented handling data exists. — [NHTSA Taurus test PDF](https://www.nhtsa.gov/sites/nhtsa.dot.gov/files/taurus_test.pdf) (via search snippet)
- **F1 data** (TracingInsights/FastF1-derived): about 3.7 Hz speed, throttle, brake (on/off) and DRS from live timing. It is coarse and has no steering or pressures, and its licence and terms derive from F1 live timing. — [TracingInsights/2026](https://github.com/TracingInsights/2026) (via search snippet)

### Inferences
- Stanford Revs is the best fully open real race-car source for validating steering angle to yaw and lateral response with a published vehicle-parameter set. Its brake channel is pedal force (%), not pressure.
- For **steering torque**, only road-car datasets were found:
  - comma: normalised torque only
  - Politecnico di Milano: kick-plate only, research-only licence
  - NHTSA: test reports
  
  No public real race car with column torque was found.
- The HPA GT86 MoTeC and AiM files and the Pi touring-car files are real club and national-level race cars with brake pressure, steering angle, wheel speeds and accelerations. They are useful for pipeline testing, but the licence is unclear.

### Gaps
- I did not determine whether the RACECAR rosbags include steering, brake or wheel-speed CAN topics.
- The Formula Student, F3, rally and karting public telemetry I looked for was not found in this session.
- The GR Cup data licence was not accessible.

## Q3. Simulator-derived datasets, which must be labelled SIMULATED

### Takeaway
Most public MoTeC .ld files online come from ACC, iRacing or other sim exporters. Never treat these as real data.

### Cited Findings
- **ACC MoTeC export:** ACC writes .ld files from in-game GT3 cars; this is simulated. — [Coach Dave Academy](https://coachdaveacademy.com/tutorials/how-to-use-motec-data-in-assetto-corsa-competizione/); [overtake.gg ACC blog](https://overtake.gg/threads/acc-blog-motec-telemetry-and-dedicated-acc-workspace.165714/page-3)
- **sim-to-motec** converts sim telemetry to .ld. Files produced by it are SIMULATED. — [GeekyDeaks/sim-to-motec](https://github.com/GeekyDeaks/sim-to-motec)
- **j-antunes/ldparser** is built mainly for ACC files. — [j-antunes/ldparser](https://github.com/j-antunes/ldparser)
- **Assetto Corsa Gym** (Hugging Face): 50 Hz sim telemetry. — [HF assettoCorsaGym](https://huggingface.co/datasets/dasgringuen/assettoCorsaGym/blob/main/README.md)
- **TRACK** (sim racing telemetry kit, arXiv 2610.10061). — [arXiv](https://arxiv.org/pdf/2610.10061)
- **mcryan123/SimRacingTelemetry.** — [GitHub](https://github.com/mcryan123/SimRacingTelemetry)
- **iRacing MoTeC/IBT data.** — [iRacing MoTeC tag](https://iracing.com/tag/motec)

### Inferences
- **Screening rule for .ld files.** Treat a file as SIMULATED if any of these hold:
  - the header has no believable logger serial or device (C125, C185, ADL3, M1 ECU and so on)
  - the vehicle or venue fields contain sim car or track ids
  - there are no electrical or health channels (battery volts, pump currents, lambda)
  - there are no sensor dropouts or sentinels and the signals are noise-free
  
  The HPA files pass this screen; sim exports typically fail it. This is my heuristic (evidence D), not a vendor specification.

### Gaps
- I did not catalogue exact ACC or iRacing header signatures byte by byte, because I downloaded no sim .ld file this session.

## Q4. Open-source parsers

### Takeaway
MoTeC .ld/.ldx and AiM .xrk have usable open readers. I found no open reader for Bosch .bmsbin or Pi .pds; those need the free vendor viewers (WinDarab free, Pi Toolbox).

### Cited Findings
- gotzl/ldparser (Python, MoTeC .ld; reverse-engineered). — [GitHub](https://github.com/gotzl/ldparser). It has a known issue with M130 ECU logs. — [issue 19](https://github.com/gotzl/ldparser/issues/19)
- j-antunes/ldparser (fork, ACC-oriented). — [GitHub](https://github.com/j-antunes/ldparser)
- motec-i2 Rust crate (ld and ldx). — [lib.rs](https://lib.rs/crates/motec-i2)
- racer-coder/TrackDataAnalysis opens AiM XRK, MoTeC LD, VBOX, iRacing IBT and more. — [GitHub](https://github.com/racer-coder/TrackDataAnalysis)
- AiM official XRK/XRZ DLL with MATLAB and Visual Studio examples. — [AiM docs](https://docs.aim-sportline.com/racestudio3/html/xrk-dll.html)
- MoTeC i2 API (official). — [i2 API User Guide](https://www.motec.com.au/hessian/uploads/i2_API_User_Guide_330ed28c83.pdf)
- .ldx is plain XML, verified on the HPA GT86 files.

### Inferences
- In the HPA GT-car file, the .ld channel unit fields were empty while the scaling (shift, mul, scale, dec) decoded to physically sensible values. Parsers should allow for missing units and confirm units in i2 or with the data owner.

### Gaps
- I did not verify asammdf (MDF/MF4) or npTDMS (TDMS) URLs in this session. Both are commonly used, but they are not cited here because they were not fetched.
- I found no open parser for Bosch WinDarab .bmsbin, Pi .pds, AiM .drk (the older RS2 format) or Magneti Marelli WinTAX.

## Dataset table

| id | name | url | format | size | car | generation | year | provenance | channels | sample_rate | units | conditions | license | status | kind | evidence_level | notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R1 | HPA Pro Data course – MoTeC i2 "GT car" | https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/6812cc9282/MoTeC-I2.zip?vid=2 | MoTeC .ld (C125) | 1.59 MB zip / 15.7 MB | "GT car" #22, model unknown | unknown | 2017 | HPAcademy course resource; header: C125 s/n 10059, Highlands Motorsport Park, "2017 Highlands Test Day 1" | Steering Angle, Brake Pres F/R, Wheel Speed ×4, LAT/LONG/Vert G, Yaw Rate (unpopulated), Damper Pos ×4, Throttle, RPM, Gear, Ground Speed, Tyre Pres/Temp ×4, lambda, temps | 1–25 Hz; dampers 500 Hz | unit fields empty; inferred km/h, deg, kPa, g, mm, °C | test day, qualifying sims, 2 drivers, ARB change; weather and tyre not stated | not stated | DOWNLOADED_VERIFIED | REAL | A (measurement) / E (GT3 class) | No steering torque or pedal force; GT3 not confirmed |
| R2 | HPA Pro Data course – Bosch WinDarab 488 GTE | https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/2672e01cb1/Bosch-Windarab.zip?vid=2 | Bosch WinDarab .bmsbin (Darab 7.07, C60) | 8.95 MB zip / 23.9 MB | Ferrari 488 GTE (F142M GT) | 488 GTE | not decoded (ZIP 2021) | HPAcademy; embedded paths "Daytona_USCC\...\488GTE\C60" and "Sebring 12h" | steer, yaw, pbrake_f/r, accx/y/z, vwheel ×4, trvdam ×4, LOAD ×4, laser ride height, t_brake ×4, TPMS, aps/ath, nmot, gear, TC/ABS map | 1 s to 10 ms blocks (up to 100 Hz) | not decoded | Daytona and Sebring 12h (IMSA) | not stated | DOWNLOADED_VERIFIED | REAL | A (measurement) / C (car id) | GTE, not GT3; factory or team data, authorisation unverifiable; "redact" channel |
| R3 | HPA Pro Data course – Pi Toolbox touring car | https://www.hpacademy.com/assets/Course-Content/Professional-Data-Course/f180d2d844/Pi-Toolbox.zip?vid=2 | Pi/Cosworth .pds (Omega L2 Ultra) | 41.5 MB zip / 204 MB | touring car, chassis 34 #71 and 72 #428 | unknown | 2017 | HPAcademy; Croft | Angle_Steering, Power Steering Press, Press_Brake F/R, Brake Bias, Disp_Damper ×4, wheel speeds ×4, Lat/Long Acc, RPM, Gear, throttle | not decoded | not decoded | Croft, 2 drivers | not stated | DOWNLOADED_VERIFIED | REAL | A / C | Has power-steering pressure (effort proxy) |
| R4 | HPA Fundamentals – MoTeC GT86 | https://www.hpacademy.com/assets/Course-Content/Data-Fundamentals-/c6d3af1dbb/03-MOTEC-EXAMPLE-LOGGED-DATA.zip | MoTeC .ld + .ldx (C125) | 2.7 MB | Toyota GT86 race car | ZN6 | 2020 | HPAcademy; C125 s/n 3642, Highlands | Steering Angle, Brake Pres, Wheel Speed ×4, G Lat/Long/Vert, Yaw Rate, RPM, Gear, Throttle, Tyre Pres, GPS | 1–50 Hz | empty in .ld; ldx gives kg, l | test day, cooling shakedown, 2 drivers, rear ARB test | not stated | DOWNLOADED_VERIFIED | REAL | A | 1250 kg, best lap 1:47.171 |
| R5 | HPA Fundamentals – AiM samples | https://www.hpacademy.com/assets/Course-Content/Data-Fundamentals-/3f224dca6b/01-AIM-EXAMPLE-LOGGED-DATA.zip | AiM .drk | 4.9 MB | Porsche 997 (Daytona); Jaguar (Misano); Toyota GT86 V8 (Ruapuna) | 997 variant unknown | 2007(?), 2006(?), 2019 | HPAcademy | 997: dampers ×4, LatAcc, wheel speeds ×4, GPS. GT86 V8: BRK_PRESS_F/R, STEER_PRESS, roll/pitch/yaw rate, 3-axis acc, wheel speeds, RPM, gear, pedal | not decoded | km/h, deg/s visible | 2019 South Island Endurance race | not stated | DOWNLOADED_VERIFIED | REAL | A / C | 997 dates parsed from header strings, uncertain |
| R6 | Stanford Revs – 2014 Targa 66 | https://purl.stanford.edu/hd122pw0365 | CSV + MAT | not listed | Ferrari 250 LM | 1965 | 2014 | Stanford DDL research | handwheelAngle, throttle, brake pedal force %, rpm, GNSS/INS, ax/ay/az, yaw/roll/pitch rate, sideSlip, suspension deflection ×4, wheel and chassis acc | not stated | SI and deg (channels.txt) | Palm Beach Int'l Raceway, 2014-02-21/22, vintage race | ODC-By 1.0 | DOWNLOADED_VERIFIED (metadata only: channels.txt, 250lm.csv) | REAL | A | Vehicle params CSV; no wheel speeds or brake pressure |
| R7 | Stanford Revs – 2013 Targa 66 | https://purl.stanford.edu/yf219gg2055 | CSV + MAT | not listed | Corvette Grand Sport | 1963 | 2013 | Stanford DDL | as R6 | not stated | as R6 | PBIR, 2013-02-22/23 | ODC-By 1.0 | DOWNLOADED_VERIFIED (channels.txt only) | REAL | A | |
| R8 | Stanford Revs – 2013 Monterey Reunion | https://purl.stanford.edu/tt103jr6546 | CSV + MAT | not listed | Corvette Grand Sport | 1963 | 2013 | Stanford DDL | as R6 | not stated | as R6 | Laguna Seca, 2013-08-10..17, 2 drivers, 8 sessions | ODC-By 1.0 | DOWNLOADED_VERIFIED (channels.txt only) | REAL | A | |
| R9 | RACECAR | https://registry.opendata.aws/racecar-dataset/ | ROS 2 bag / nuScenes | multi-TB (not checked) | Dallara AV-21 (IAC) | AV-21 | 2021–22 | IAC teams, UVA LinkLab | LiDAR, radar, cameras, GNSS/IMU (CAN topics unverified) | not checked | SI | IMS, LVMS and others, up to ~170 mph | CC BY-NC 4.0 | IDENTIFIED | REAL | B | Non-commercial only |
| R10 | Toyota GR Cup NA "Hack the Track" | https://devpost-public.s3.amazonaws.com/Toyota/gr_hackathon_dataset_overview.pdf | CSV (per community repo) | 100+ GB (community claim) | Toyota GR86 Cup | GR86 | 2024–25 | Toyota GR / SRO via Devpost | pbrake_f/r (bar), accx/accy_can, Steering_Angle, aps, ath, Speed, Gear, nmot, GPS | not stated | partly | 7 US tracks | unknown (rules 403) | IDENTIFIED | REAL | C | Channels from third-party README |
| R11 | commaSteeringControl | https://huggingface.co/datasets/commaai/commaSteeringControl | (HF dataset) | ~12,500 h | hundreds of road-car models | various | ~2022–24 | comma.ai openpilot | normalised steer torque, steerFiltered, vehicle state | not checked | normalised [−1, 1] | public roads | MIT | IDENTIFIED | REAL | B | No absolute N·m |
| R12 | Instrumented steering wheel (PoliMi) | https://pmc.ncbi.nlm.nih.gov/articles/PMC7214765 | MATLAB | not checked | modified road car | – | 2020 | Data in Brief | steering-wheel forces and torques | not checked | not checked | kick-plate test, 9 drivers | research only | IDENTIFIED | REAL | B | |
| R13 | EUFS ROS 2 dataset | https://catalogue.eidf.ac.uk/dataset/groups/eidf156-edinburgh-university-formula-student | MCAP | not checked | EUFS driverless FS car | – | ongoing | Univ. of Edinburgh | lidar, wheel encoders, IMU | not checked | – | – | CC-BY-4.0 | IDENTIFIED | REAL | B | |
| R14 | MoTeC i2 Pro bundled samples | https://www.motec.com.au/i2/ | .ld | – | unknown | – | – | MoTeC (per third-party claims) | unknown | – | – | – | MoTeC software licence | IDENTIFIED | REAL? (unverified) | E | Requires install |
| R15 | AiM XRK DLL MATLAB samples | https://docs.aim-sportline.com/racestudio3/html/xrk-dll.html | .xrk | – | unknown | – | – | AiM | unknown | – | – | – | AiM | IDENTIFIED | unknown | E | |
| R16 | 981 Clubsport MoTeC (Rennlist offer) | https://rennlist.com/forums/data-acquisition-and-analysis-for-racing-and-de/1363608-vir-iracing-data-log-telemetry.html | .ld | – | Porsche Cayman 981 Clubsport | 981 | ~2023 | private forum offer | unknown | – | – | VIR | none (private) | IDENTIFIED | REAL (claimed) | E | Not public; needs owner consent |
| S1 | ACC MoTeC exports | https://coachdaveacademy.com/tutorials/how-to-use-motec-data-in-assetto-corsa-competizione/ | .ld/.ldx | – | ACC GT3 models | – | – | Kunos sim | sim channels | – | – | sim | – | IDENTIFIED | SIMULATED | – | Never real |
| S2 | Assetto Corsa Gym | https://huggingface.co/datasets/dasgringuen/assettoCorsaGym | (HF) | – | AC cars | – | 2024 | sim | speed, position, acc, inputs | 50 Hz | – | sim | see HF | IDENTIFIED | SIMULATED | – | |
| S3 | sim-to-motec outputs | https://github.com/GeekyDeaks/sim-to-motec | .ld | – | sim cars | – | – | converter | sim | – | – | sim | see repo | IDENTIFIED | SIMULATED | – | |
| S4 | Zenodo 3967879 fleet data | https://zenodo.org/record/3967879 | – | – | synthetic road fleet | – | 2020 | OpenXC model | vehicle signals | – | – | synthetic | see Zenodo | IDENTIFIED | SIMULATED | – | Synthetic |
