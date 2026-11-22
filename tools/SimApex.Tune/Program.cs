using System.Globalization;
using System.Text;
using System.Text.Json;
using SimApex.Analysis.Tuning;
using SimApex.Hardware;
using SimApex.Sim.Lmu;
using SimApex.Sim.Lmu.Sdk;
using SimApex.Tune;
using static SimApex.Tune.Advice;

// SimApex guided tuning.
//   SimApex.Tune                      interactive procedure (T0–T4) with LMU and the T-LCM
//   SimApex.Tune --replay <dir> ...   run the analyses on sessions recorded with SimApex.Diagnostics
// Read-only towards devices: the only file it writes outside its own folder is LMU's
// "direct input.json" (FFB strength), after a backup, with the game closed and the user's consent.

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = Ui;

try
{
    string sdkDir = LmuSdk.FindHeaderDirectory()
        ?? throw new DirectoryNotFoundException("Non trovo i file SDK di LMU (Support\\SharedMemoryInterface). Imposta SIMAPEX_LMU_SDK.");
    var sdk = LmuSdk.Load(sdkDir);
    var mapper = new FrameMapper(new LmuTelemetryDecoder(sdk));
    string root = Path.Combine("sessions", "tuning_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
    Directory.CreateDirectory(root);
    var report = new Report(root, sdk);

    int r = args.Length > 0 && args[0] == "--replay"
        ? Replay(args.Skip(1).ToList(), mapper, report)
        : Interactive(mapper, report, sdkDir);
    report.Save();
    Console.WriteLine(F($"\nReport: {Path.GetFullPath(Path.Combine(root, "report.md"))}"));
    return r;
}
catch (Exception ex) when (ex is IOException or InvalidOperationException or UnauthorizedAccessException or FormatException or KeyNotFoundException)
{
    Console.Error.WriteLine("Errore: " + ex.Message);
    return 1;
}

static int Replay(List<string> folders, FrameMapper mapper, Report report)
{
    if (folders.Count == 0)
    {
        throw new InvalidOperationException("Uso: SimApex.Tune --replay <cartella sessione> [altre cartelle]");
    }

    var all = new List<TelemetryFrame>();
    for (int k = 0; k < folders.Count; k++)
    {
        Console.WriteLine("Carico " + folders[k] + " ...");
        // Laps are offset per session so that laps from different sessions stay separate bootstrap blocks.
        all.AddRange(RecordingSource.Load(folders[k], mapper).Select(f => f with { Lap = f.Lap + 1000 * k }));
        report.Line(F($"- Sessione {k + 1}: `{folders[k]}`"));
    }

    var handsOff = HandsOffAnalysis.Analyse(all);
    var ffb = FfbGainAnalysis.Analyse(all, TargetSaturation, exclude: InEpisode(handsOff));
    report.Section("T1 – Guadagno FFB (episodi a volante libero esclusi)");
    report.Block(ffb is null ? "Dati insufficienti." : DescribeFfb(ffb, 10000) + "(le percentuali di forza LMU sono riferite a 100% come base)");
    report.Section("T2 – Stabilità a volante libero");
    report.Block(DescribeHandsOff(handsOff));
    report.Section("T3 – Frenata e ABS in pista");
    report.Block(DescribeBrake(BrakeAnalysis.AnalyseTrack(all)));
    return 0;
}

static Func<TelemetryFrame, bool> InEpisode(HandsOffResult h) =>
    f => h.Episodes.Any(e => f.Time >= e.Start - 1 && f.Time <= e.End + 1);

static int Interactive(FrameMapper mapper, Report report, string sdkDir)
{
    var settings = new LmuControllerSettings(LmuControllerSettings.InstallRootFromSdk(sdkDir));
    var wheel = settings.ReadStrengths().FirstOrDefault(d => d.Device.StartsWith("FANATEC", StringComparison.Ordinal));
    if (wheel.Device is null)
    {
        throw new InvalidOperationException("Nel file dei comandi di LMU non trovo una base FANATEC.");
    }

    Console.WriteLine("=== SimApex – messa a punto guidata ===");
    Console.WriteLine("Ogni test ti dice cosa fare; il programma misura e propone. Non scrive nulla su base o pedaliera.");
    Console.WriteLine(F($"Forza FFB attuale in LMU: {wheel.Strength / 100:0}%  ({wheel.Device})\n"));
    double ff = Ask.Number("Valore FFB nella Fanatec App (%)", 100);
    double ndp = Ask.Number("Valore NDP nella Fanatec App (%)", 50);
    double brakeForce = Ask.Number("BRAKE FORCE nel software T-LCM", 92);
    report.Section("Impostazioni dichiarate dall'utente (UserInput)");
    report.Block(F($"Fanatec FFB {ff:0}%, NDP {ndp:0}%; T-LCM BRAKE FORCE {brakeForce:0}; LMU forza FFB {wheel.Strength / 100:0}%."));

    // ---------------- T0 + T1
    List<TelemetryFrame> baseline = [];
    if (Ask.YesNo("\nT0 – Giri di riferimento: avvio?"))
    {
        Console.WriteLine("Avvia LMU ed entra in pista. Guida 5 giri puliti al tuo ritmo, con le mani sul volante.");
        Console.WriteLine("La cattura si ferma da sola dopo 5 giri completi (INVIO per fermarla prima).");
        baseline = CaptureLive(mapper, f => Completed(f) >= 5,
            f => F($"giro {Last(f)?.Lap} | completati {Completed(f)}/5 | {Last(f)?.SpeedKmh:0} km/h | FFB saturo {Pct(SatFraction(f))}"));
        var ffb = FfbGainAnalysis.Analyse(baseline, TargetSaturation);
        report.Section("T0/T1 – Giri di riferimento e guadagno FFB");
        if (ffb is null)
        {
            report.Block("Dati insufficienti (servono almeno qualche giro in movimento).");
        }
        else
        {
            report.Block(DescribeFfb(ffb, wheel.Strength));
            var (value, low, high) = RecommendStrength(ffb, wheel.Strength);
            string advice = F($"Forza FFB consigliata in LMU: {value / 100:0}% (IC95% {low / 100:0}–{high / 100:0}%), oggi {wheel.Strength / 100:0}%.");
            report.Block(advice);
            Console.WriteLine(advice);
            if (Math.Abs(value - wheel.Strength) >= 100 && Ask.YesNo(F($"Applico {value / 100:0}% in LMU? Servirà chiudere LMU")))
            {
                bool increase = value > wheel.Strength;
                if (!increase || Ask.YesNo("Il valore è PIÙ ALTO di quello attuale (più coppia al volante). Confermi?"))
                {
                    WaitForGameClosed();
                    var change = settings.SetStrength("FANATEC", value, allowIncrease: increase);
                    report.Change(change);
                    Console.WriteLine(F($"Fatto. Backup: {change.Backup}"));
                    wheel = (wheel.Device, value);
                }
            }
        }
    }

    // ---------------- T2
    if (Ask.YesNo("\nT2 – Stabilità a volante libero: avvio?"))
    {
        for (int round = 1; round <= 5; round++)
        {
            Console.WriteLine("Riapri LMU ed entra in pista. Sul rettilineo principale, sopra i 200 km/h e in linea retta,");
            Console.WriteLine("LASCIA il volante per 3 secondi tenendo le mani vicine, poi riprendilo. Ripeti in 3 giri diversi.");
            Console.WriteLine("Se l'oscillazione diventa violenta, riprendi subito il volante. INVIO per terminare prima.");
            HandsOffResult result = new([], double.NaN, double.NaN, HandsOffAnalysis.DefaultTargetDamping, false);
            long lastAnalysis = 0;
            var frames = CaptureLive(mapper,
                f =>
                {
                    if (f.Count - lastAnalysis > 300)
                    {
                        lastAnalysis = f.Count;
                        result = HandsOffAnalysis.Analyse(f);
                    }

                    return result.Episodes.Count(e => e.SpeedKmh >= 180) >= 3;
                },
                f => F($"giro {Last(f)?.Lap} | {Last(f)?.SpeedKmh:0} km/h | episodi riconosciuti {result.Episodes.Count}/3"));
            result = HandsOffAnalysis.Analyse(frames);
            report.Section(F($"T2 – Stabilità, prova {round} (NDP {ndp:0}%)"));
            report.Block(DescribeHandsOff(result));
            var ffbCheck = FfbGainAnalysis.Analyse(frames, TargetSaturation, exclude: InEpisode(result));
            if (ffbCheck is not null)
            {
                report.Block(F($"Verifica T1 nella stessa sessione (episodi esclusi): saturazione FFB {Pct(ffbCheck.Saturation)}."));
            }

            Console.WriteLine(DescribeHandsOff(result));
            if (result.Passed)
            {
                Console.WriteLine("Stabilità raggiunta: NDP va bene così.");
                break;
            }

            if (result.Episodes.Count == 0)
            {
                Console.WriteLine("Non ho riconosciuto episodi: ripeti il rilascio del volante sul rettilineo.");
                if (!Ask.YesNo("Ripeto la prova?"))
                {
                    break;
                }

                continue;
            }

            if (ndp + NdpStep > 100)
            {
                report.Block("NDP già al massimo: obiettivo non raggiunto con il solo damper della base.");
                break;
            }

            Console.WriteLine(F($"Smorzamento insufficiente. Nella Fanatec App porta NDP da {ndp:0}% a {ndp + NdpStep:0}%."));
            if (!Ask.YesNo("Fatto? Ripeto la prova"))
            {
                break;
            }

            report.Block(F($"Modifica manuale richiesta: Fanatec NDP {ndp:0}% → {ndp + NdpStep:0}% (UserInput)."));
            ndp += NdpStep;
        }
    }

    // ---------------- T3
    List<PedalPush> comfortable = [], emergency = [];
    BrakeTrackResult? track = null;
    if (Ask.YesNo("\nT3a – Prova statica del freno (pedaliera collegata via USB): avvio?"))
    {
        string? path = TlcmPedals.Find() ?? throw new InvalidOperationException("T-LCM non trovata via USB.");
        Console.WriteLine("Premi il freno 3 volte come in una frenata FORTE MA COMODA, tenendo ~2 s, pausa di 2 s tra le spinte.");
        Console.WriteLine("Poi 3 volte con la MASSIMA forza d'emergenza. INVIO quando hai finito.");
        var samples = PedalCapture.Run(path, TimeSpan.FromSeconds(90),
            s => F($"freno {Pct(s.Count > 0 ? s[^1].Brake : 0)} | spinte riconosciute {BrakeAnalysis.DetectPushes(s).Count}/6"));
        var pushes = BrakeAnalysis.DetectPushes(samples);
        comfortable = [.. pushes.Take(3)];
        emergency = [.. pushes.Skip(3).Take(3)];
        report.Section("T3a – Prova statica del freno");
        report.Block(string.Join(Environment.NewLine, pushes.Select((p, i) => F($"spinta {i + 1}: livello {Pct(p.Level)}, picco {Pct(p.Peak)}, durata {p.End - p.Start:0.0} s"))));
    }

    if (Ask.YesNo("\nT3b – Frenate in pista: avvio?"))
    {
        Console.WriteLine("Scegli una staccata forte (es. Tosa o Rivazza). Fai 5 frenate al massimo nello stesso punto,");
        Console.WriteLine("poi cambia mappa ABS e fai altre 5 frenate. INVIO quando hai finito.");
        var frames = CaptureLive(mapper, _ => false,
            f => F($"giro {Last(f)?.Lap} | {Last(f)?.SpeedKmh:0} km/h | frenate forti {BrakeAnalysis.AnalyseTrack(f).Events.Count} | mappa ABS {Last(f)?.AbsMap}"));
        track = BrakeAnalysis.AnalyseTrack(frames);
        report.Section("T3b – Frenate e ABS in pista");
        report.Block(DescribeBrake(track));
    }

    if (comfortable.Count > 0 || track is not null)
    {
        report.Section("T3 – Raccomandazione freno");
        string advice = RecommendBrakeForce(comfortable, emergency, track, brakeForce);
        report.Block(advice);
        Console.WriteLine(advice);
    }

    return 0;
}

static List<TelemetryFrame> CaptureLive(FrameMapper mapper, Func<List<TelemetryFrame>, bool> done, Func<List<TelemetryFrame>, string> status)
{
    while (true)
    {
        try
        {
            using var live = new LiveCapture(mapper);
            return live.Run(done, status);
        }
        catch (FileNotFoundException)
        {
            Console.Write("\rIn attesa di LMU (telemetria non disponibile)...");
            Thread.Sleep(2000);
        }
    }
}

static void WaitForGameClosed()
{
    while (LmuControllerSettings.IsGameRunning())
    {
        Console.Write("\rChiudi LMU per applicare la modifica (attendo)...");
        Thread.Sleep(2000);
    }

    Console.WriteLine();
}

static TelemetryFrame? Last(List<TelemetryFrame> f) => f.Count > 0 ? f[^1] : null;

static int Completed(List<TelemetryFrame> f) => f.Count == 0 ? 0 : f[^1].Lap - f[0].Lap;

static double SatFraction(List<TelemetryFrame> f) =>
    f.Count == 0 ? double.NaN : f.Count(x => Math.Abs(x.FfbOutput) >= FfbGainAnalysis.SaturationThreshold && x.SpeedKmh > 20) / (double)Math.Max(1, f.Count(x => x.SpeedKmh > 20));

namespace SimApex.Tune
{
    internal static class Ask
    {
        public static bool YesNo(string question)
        {
            Console.Write(question + " [s/n] ");
            string? a = Console.ReadLine();
            return a is not null && a.Trim().StartsWith('s');
        }

        public static double Number(string question, double fallback)
        {
            Console.Write(F($"{question} [{fallback}]: "));
            string? a = Console.ReadLine();
            return double.TryParse(a?.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double v) ? v : fallback;
        }
    }

    /// <summary>Markdown report plus a JSON log of every change written.</summary>
    internal sealed class Report(string folder, LmuSdk sdk)
    {
        private readonly StringBuilder _md = new();
        private readonly List<SettingChange> _changes = [];

        public void Section(string title) => _md.AppendLine().AppendLine("## " + title).AppendLine();

        public void Block(string text) => _md.AppendLine("```").AppendLine(text.TrimEnd()).AppendLine("```");

        public void Line(string text) => _md.AppendLine(text);

        public void Change(SettingChange c)
        {
            _changes.Add(c);
            Line(F($"- Modifica scritta: `{c.Setting}` {c.OldValue} → {c.NewValue} in `{c.File}` (backup `{c.Backup}`)"));
        }

        public void Save()
        {
            var head = new StringBuilder();
            head.AppendLine("# SimApex – report messa a punto").AppendLine();
            head.AppendLine(F($"Data: {DateTime.Now:yyyy-MM-dd HH:mm}. Dati: telemetria LMU (SimTelemetry), layout dai file SDK in `{sdk.Source.Directory}`."));
            head.AppendLine("Tutti i valori di questo report sono misure sul simulatore e sulla postazione, non dati di una GT3 reale.");
            File.WriteAllText(Path.Combine(folder, "report.md"), head.ToString() + _md, new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(folder, "changes.json"), JsonSerializer.Serialize(_changes, JsonOptions));
        }

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    }
}
