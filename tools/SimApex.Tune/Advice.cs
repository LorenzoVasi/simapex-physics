using System.Globalization;
using System.Text;
using SimApex.Analysis.Tuning;

namespace SimApex.Tune;

/// <summary>Formatting (Italian UI) and the decision rules that turn analyses into recommendations.</summary>
internal static class Advice
{
    public static readonly CultureInfo Ui = CultureInfo.GetCultureInfo("it-IT");

    public const double TargetSaturation = 0.01;
    public const double NdpStep = 10;

    public static string F(FormattableString s) => s.ToString(Ui);

    public static string Pct(double fraction) => double.IsNaN(fraction) ? "n/d" : F($"{fraction * 100:0.0}%");

    // ---------------------------------------------------------------- T1
    public static string DescribeFfb(FfbGainResult r, double currentStrength)
    {
        var sb = new StringBuilder();
        sb.AppendLine(F($"Campioni {r.Samples:N0} su {r.Laps} giri. Uscita FFB ∝ coppia piantone: R² = {r.FitR2:0.000}."));
        sb.AppendLine(F($"Fondo scala attuale: coppia piantone {r.FullScale:0.0} → saturazione {Pct(r.Saturation)} del tempo."));
        sb.AppendLine(F($"Per stare all'{TargetSaturation * 100:0}% di saturazione serve fondo scala {r.RecommendedFullScale:0.0} (IC95% {r.RecommendedFullScaleLow:0.0}–{r.RecommendedFullScaleHigh:0.0})."));
        sb.AppendLine("Saturazione al variare del fondo scala:");
        foreach (var (fs, sat) in r.Curve)
        {
            sb.AppendLine(F($"  fondo scala {fs,6:0.0} → forza LMU {currentStrength * r.FullScale / fs / 100,5:0}%  saturazione {Pct(sat)}"));
        }

        return sb.ToString();
    }

    /// <summary>New LMU strength (0..10000), rounded to 1%, never above the current value unless the data asks for more.</summary>
    public static (double Value, double Low, double High) RecommendStrength(FfbGainResult r, double currentStrength)
    {
        static double Round(double v) => Math.Round(Math.Clamp(v, 500, 10000) / 100) * 100;
        return (Round(currentStrength * r.StrengthFactor), Round(currentStrength * r.StrengthFactorLow), Round(currentStrength * r.StrengthFactorHigh));
    }

    // ---------------------------------------------------------------- T2
    public static string DescribeHandsOff(HandsOffResult r)
    {
        var sb = new StringBuilder();
        if (r.Episodes.Count == 0)
        {
            sb.AppendLine("Nessun episodio a volante libero riconosciuto.");
            return sb.ToString();
        }

        sb.AppendLine(F($"Oscillazione tipica a mani sul volante (3–9 Hz): {r.BaselineAmplitude:0.0000} della rotazione totale."));
        foreach (var e in r.Episodes)
        {
            string zeta = double.IsNaN(e.DampingRatio) ? "n/d" : F($"{e.DampingRatio:0.000}");
            string verdict = double.IsNaN(e.DampingRatio) ? "non misurabile"
                : e.DampingRatio < 0 ? "CRESCE da sola (instabile)"
                : e.DampingRatio < r.TargetDampingRatio ? "poco smorzata" : "smorzata (OK)";
            sb.AppendLine(F($"  t={e.Start:0.0}s  {e.SpeedKmh:0} km/h  {e.FrequencyHz:0.00} Hz  ampiezza {e.Amplitude:0.000} ({e.AmplitudeRatio:0}× normale, ≈{e.Amplitude * 360:0}° su 720°)  ζ={zeta}  FFB saturo {Pct(e.FfbSaturation)}  → {verdict}"));
        }

        sb.AppendLine(F($"ζ mediano {r.MedianDampingRatio:0.000}; obiettivo ≥ {r.TargetDampingRatio:0.00} (oscillazione sotto il 10% entro ~3 cicli). Esito: {(r.Passed ? "SUPERATO" : "NON superato")}."));
        return sb.ToString();
    }

    // ---------------------------------------------------------------- T3
    public static string DescribeBrake(BrakeTrackResult r)
    {
        var sb = new StringBuilder();
        sb.AppendLine(F($"Frenate forti riconosciute: {r.Events.Count}. Tempo di frenata con pedale a fondo scala: {Pct(r.BottomedFraction)}."));
        foreach (var m in r.Maps)
        {
            sb.AppendLine(F($"  mappa ABS (indice LMU) {m.AbsMap}: {m.Events} frenate, decelerazione media {m.MeanDecelG:0.00} g (dev.st. {m.DecelStdG:0.00}), ABS attivo {Pct(m.MeanAbsFraction)}, pedale all'intervento ABS (mediana) {Pct(m.MedianBrakeAtOnset)}"));
        }

        if (r.MapComparison is { } c)
        {
            string sig = c.Test.PValue < 0.05 ? "differenza significativa" : "differenza NON significativa";
            sb.AppendLine(F($"  confronto mappa {c.MapA} vs {c.MapB}: Δ decelerazione {c.Test.MeanDifference:+0.00;-0.00} g, p = {c.Test.PValue:0.000} → {sig}."));
        }

        sb.AppendLine("Pedale all'intervento ABS in rettilineo, per velocità:");
        foreach (var b in r.OnsetBands.Where(b => b.Count > 0))
        {
            sb.AppendLine(F($"  {b.FromKmh:0}–{b.ToKmh:0} km/h: n={b.Count}, p10 {Pct(b.P10)}, mediana {Pct(b.P50)}, p90 {Pct(b.P90)}"));
        }

        return sb.ToString();
    }

    /// <summary>Brake Force recommendation from the static pushes and the ABS onset seen on track.</summary>
    public static string RecommendBrakeForce(IReadOnlyList<PedalPush> comfortable, IReadOnlyList<PedalPush> emergency, BrakeTrackResult? track, double brakeForceSetting)
    {
        var sb = new StringBuilder();
        double c = comfortable.Count > 0 ? Stats.Percentile(comfortable.Select(p => p.Level), 0.5) : double.NaN;
        double e = emergency.Count > 0 ? Stats.Percentile(emergency.Select(p => p.Peak), 0.5) : double.NaN;
        var onsets = track?.Events.Select(x => x.BrakeAtAbsOnset).Where(x => !double.IsNaN(x)).ToList() ?? [];
        double onset = onsets.Count > 0 ? Stats.Percentile(onsets, 0.5) : double.NaN;
        sb.AppendLine(F($"Spinta forte-comoda: {Pct(c)} del pedale. Spinta d'emergenza: {Pct(e)}. ABS in pista interviene (mediana) a {Pct(onset)}."));
        if (double.IsNaN(c) || double.IsNaN(onset))
        {
            sb.AppendLine("Dati insufficienti per una raccomandazione sul Brake Force.");
            return sb.ToString();
        }

        if (c < onset - 0.05)
        {
            sb.AppendLine(F($"→ Con la forza comoda non arrivi all'ABS: il pedale è troppo duro per la tua gamba. Abbassa BRAKE FORCE di 5 ({brakeForceSetting:0} → {brakeForceSetting - 5:0}) e ripeti la prova statica."));
        }
        else if (!double.IsNaN(e) && e < 0.99)
        {
            sb.AppendLine("→ Nemmeno con la spinta d'emergenza arrivi a fondo scala: resta margine inutilizzato. Valuta di abbassare BRAKE FORCE di 5.");
        }
        else if (c >= 0.99)
        {
            sb.AppendLine(F($"→ Arrivi a fondo scala già con la forza comoda: il pedale è morbido rispetto alla tua spinta. Alza BRAKE FORCE di 5 ({brakeForceSetting:0} → {Math.Min(100, brakeForceSetting + 5):0}) per avere più modulazione."));
        }
        else
        {
            sb.AppendLine("→ Coerente: con la forza comoda raggiungi la zona ABS e con l'emergenza il fondo scala. Nessuna modifica necessaria.");
        }

        sb.AppendLine("Forza in kg: NON misurata (serve un peso di riferimento). Stima per una GT3 reale a fondo pedale: 55–105 kgf (livello D, ±30%).");
        return sb.ToString();
    }
}
