// ScriptHazem UI kit — localization (Arabic first, English second) and right-to-left helpers.
// Copy this file as-is into a tool (keep the namespace). See tools\_Brand\Kit\KIT.md.
//
//   XAML:  xmlns:kit="clr-namespace:ScriptHazem.Kit"
//          <TextBlock Text="{kit:T nav.items}"/>                   live: updates when the language changes
//          <Grid FlowDirection="{Binding Flow, Source={x:Static kit:Loc.I}}">   RTL root
//          <TextBlock Style="{StaticResource LtrText}" .../>       LTR island (numbers, ids, paths, code)
//   C#:    Loc.Init(settings.Language ?? "ar");                    once, before the first window
//          Loc.Get("key"), Loc.F("key", arg0, ...), Loc.Lang = "en", Loc.Toggle()
//          Loc.FB("key", "PlayerStudio", 5)   bidi-safe F for Arabic (LRM around Latin args, RLM before numbers); Loc.Ltr(s)
//          kit:FieldState.State="changed|error|ok" on a FieldBox (overlay border, survives hover/focus)
//          Loc.LanguageChanged += () => { settings.Language = Loc.Lang; settings.Save(); RefreshCodeTexts(); };
//
// Strings: Strings.ar.json / Strings.en.json, flat { "key": "text" } (comments allowed), embedded with
//   <EmbeddedResource Include="Strings\Strings.*.json" LogicalName="Strings.%(Filename)%(Extension)" WithCulture="false" />
//   (WithCulture="false" is mandatory: otherwise Strings.ar.json becomes a satellite resource and never loads.)
// A Strings.<lang>.json file next to the exe overrides the embedded one (lets a translator fix wording without a build).
// Missing key: English text, else the key itself — never an exception. A JSON that fails to parse is reported
// (Debug output + loc.log next to the exe, Loc.LastWarning) instead of silently showing English.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ScriptHazem.Kit;

public sealed class Loc : INotifyPropertyChanged
{
    public static Loc I { get; } = new();

    private static readonly Dictionary<string, Dictionary<string, string>> Tables = new();
    private Dictionary<string, string> _cur = new();
    private Dictionary<string, string> _en;
    private string _lang = "ar";

    /// <summary>Raised on the UI thread after <see cref="Lang"/> changed (bindings are already refreshed).</summary>
    public static event Action LanguageChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    private Loc() { }

    /// <summary>"ar" or "en".</summary>
    public static string Lang
    {
        get => I._lang;
        set => I.SetLang(value);
    }

    /// <summary>Arabic → right-to-left.</summary>
    public static bool Rtl => I._lang == "ar";
    public bool IsRtl => Rtl;
    public FlowDirection Flow => Rtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    /// <summary>Text alignment for LTR islands: hug the reading edge (right in Arabic).</summary>
    public TextAlignment LtrAlign => Rtl ? TextAlignment.Right : TextAlignment.Left;
    /// <summary>Culture for number/date formatting. Western digits in both languages (what Egyptian users expect in tools).</summary>
    public static CultureInfo Culture { get; } = CultureInfo.InvariantCulture;

    /// <summary>Indexer used by <see cref="T"/> bindings: Loc.I["key"].</summary>
    public string this[string key] => Get(key);

    /// <summary>Call once at startup (no events raised).</summary>
    public static void Init(string lang)
    {
        I._lang = Normalize(lang);
        I._cur = Table(I._lang);
    }

    public static void Toggle() => Lang = Rtl ? "en" : "ar";

    public static string Get(string key)
    {
        if (key == null) return "";
        if (I._cur.TryGetValue(key, out var s)) return s;
        I._en ??= Table("en");
        return I._en.TryGetValue(key, out s) ? s : key;
    }

    /// <summary>True when the current language (or English) has the key.</summary>
    public static bool Has(string key)
    {
        if (key == null) return false;
        I._en ??= Table("en");
        return I._cur.ContainsKey(key) || I._en.ContainsKey(key);
    }

    /// <summary>Translated text if "prefix + raw" is a key, else raw unchanged (for data-driven labels).</summary>
    public static string Opt(string prefix, string raw) => raw != null && Has(prefix + raw) ? Get(prefix + raw) : raw;

    /// <summary>string.Format with the translated pattern, e.g. F("count.shown", 12).</summary>
    public static string F(string key, params object[] args)
    {
        try { return string.Format(Culture, Get(key), args); }
        catch (FormatException) { return Get(key); }
    }

    private void SetLang(string lang)
    {
        lang = Normalize(lang);
        if (lang == _lang) return;
        _lang = lang;
        _cur = Table(lang);
        Raise("Item[]");
        Raise(nameof(IsRtl));
        Raise(nameof(Flow));
        Raise(nameof(LtrAlign));
        LanguageChanged?.Invoke();
    }

    private static string Normalize(string lang) => string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "ar";

    private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private static Dictionary<string, string> Table(string lang)
    {
        lock (Tables)
        {
            if (Tables.TryGetValue(lang, out var t)) return t;
            t = Load(lang) ?? new Dictionary<string, string>();
            Tables[lang] = t;
            return t;
        }
    }

    private static Dictionary<string, string> Load(string lang)
    {
        string name = $"Strings.{lang}.json";
        var opts = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
        // 1) override file next to the exe (a broken override is reported and skipped, the embedded table still loads)
        foreach (var p in new[] { Path.Combine(AppContext.BaseDirectory, name), Path.Combine(AppContext.BaseDirectory, "Strings", name) })
        {
            if (!File.Exists(p)) continue;
            try { return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(p), opts); }
            catch (Exception ex) { Warn($"override {p} could not be parsed, using the embedded strings: {ex.Message}"); }
        }
        // 2) embedded resource (LogicalName "Strings.<lang>.json")
        try
        {
            var asm = Assembly.GetEntryAssembly() ?? typeof(Loc).Assembly;
            string res = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (res == null)
            {
                Warn($"embedded resource {name} not found (is the EmbeddedResource line missing WithCulture=\"false\"?)");
                return null;
            }
            using var s = asm.GetManifestResourceStream(res);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(s, opts);
        }
        catch (Exception ex)
        {
            Warn($"embedded {name} could not be parsed, falling back to English/keys: {ex.Message}");
            return null;
        }
    }

    /// <summary>Last string-table problem (null when all tables loaded). Also written to Debug and loc.log.</summary>
    public static string LastWarning { get; private set; }

    /// <summary>Reports a string-table problem: Debug output + "loc.log" next to the exe (or %LOCALAPPDATA%\ScriptHazem\loc.log).</summary>
    private static void Warn(string msg)
    {
        LastWarning = msg;
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [Loc] {msg}";
        System.Diagnostics.Debug.WriteLine(line);
        foreach (var dir in new[] { AppContext.BaseDirectory,
                                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScriptHazem") })
        {
            try { Directory.CreateDirectory(dir); File.AppendAllText(Path.Combine(dir, "loc.log"), line + Environment.NewLine); return; }
            catch { /* try the next folder */ }
        }
    }

    // ---------------- bidi helpers (WPF ignores LRE/PDF U+202A/U+202C; only the marks below work) ----------------

    /// <summary>LEFT-TO-RIGHT MARK U+200E.</summary>
    public const string Lrm = "\u200E";
    /// <summary>RIGHT-TO-LEFT MARK U+200F.</summary>
    public const string Rlm = "\u200F";

    /// <summary>
    /// A Latin/number run (file name, IP, "(PlayerStudio)", "x2") to put inside an Arabic sentence: in Arabic it gets an LRM
    /// on both sides so brackets, dots and a following number stay in place. English: unchanged.
    /// </summary>
    public static string Ltr(string s) => Rtl && !string.IsNullOrEmpty(s) ? Lrm + s + Lrm : s ?? "";

    /// <summary>
    /// Like <see cref="F"/> but bidi-safe for Arabic: string arguments that contain Latin letters/digits are wrapped with
    /// <see cref="Ltr"/>, numeric arguments get an RLM in front (so "+5", "-3", "1.5" are not glued to a preceding Latin run).
    /// </summary>
    public static string FB(string key, params object[] args)
    {
        if (!Rtl || args == null) return F(key, args);
        var a = new object[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            a[i] = args[i] switch
            {
                string s when s.Any(c => c < 0x0590 && char.IsLetterOrDigit(c)) => Ltr(s),
                sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal
                    => new RlmNumber((IFormattable)args[i]),   // keeps {0:N0} formats working
                _ => args[i],
            };
        }
        return F(key, a);
    }

    private sealed class RlmNumber : IFormattable
    {
        private readonly IFormattable _v;
        public RlmNumber(IFormattable v) { _v = v; }
        public string ToString(string format, IFormatProvider provider) => Rlm + _v.ToString(format, provider);
        public override string ToString() => Rlm + _v.ToString(null, Culture);
    }
}

/// <summary>
/// {kit:T key} — localized text that follows Loc.Lang live. On a non-dependency property it returns the current text once.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public sealed class T : MarkupExtension
{
    public T() { }
    public T(string key) { Key = key; }

    [ConstructorArgument("key")]
    public string Key { get; set; }

    public override object ProvideValue(IServiceProvider sp)
    {
        var target = sp?.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        bool bindable = target == null
            || target.TargetProperty is DependencyProperty
            || target.TargetObject is Setter
            || target.TargetObject is not DependencyObject; // inside a template (SharedDp): Binding handles it
        if (!bindable)
            return Loc.Get(Key);
        var b = new Binding("[" + Key + "]") { Source = Loc.I, Mode = BindingMode.OneWay };
        return b.ProvideValue(sp);
    }
}

/// <summary>
/// Attached helpers: kit:Bidi.Ltr="True" turns any element into a left-to-right island
/// (a TextBlock / TextBox also gets the reading-edge alignment of <see cref="Loc.LtrAlign"/>).
/// </summary>
public static class Bidi
{
    public static readonly DependencyProperty LtrProperty = DependencyProperty.RegisterAttached(
        "Ltr", typeof(bool), typeof(Bidi), new PropertyMetadata(false, OnLtr));

    public static bool GetLtr(DependencyObject d) => (bool)d.GetValue(LtrProperty);
    public static void SetLtr(DependencyObject d, bool v) => d.SetValue(LtrProperty, v);

    private static void OnLtr(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not true) return;
        d.SetValue(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight);
        var align = new Binding(nameof(Loc.LtrAlign)) { Source = Loc.I, Mode = BindingMode.OneWay };
        if (d is System.Windows.Controls.TextBlock)
            BindingOperations.SetBinding(d, System.Windows.Controls.TextBlock.TextAlignmentProperty, align);
        else if (d is System.Windows.Controls.TextBox)
            BindingOperations.SetBinding(d, System.Windows.Controls.TextBox.TextAlignmentProperty, align);
    }
}

/// <summary>
/// kit:FieldState.State on a FieldBox/Field/SearchBox: "changed" (amber edge + tint), "error" (red edge), "ok" (green edge),
/// empty = normal. Drawn as an overlay border, so hover/focus never hide it.
/// (Back-compat: FieldBox also treats Tag="changed"/"error"/"ok" the same way and then shows no placeholder.)
/// </summary>
public static class FieldState
{
    public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
        "State", typeof(string), typeof(FieldState), new PropertyMetadata(""));

    public static string GetState(DependencyObject d) => (string)d.GetValue(StateProperty);
    public static void SetState(DependencyObject d, string v) => d.SetValue(StateProperty, v);
}
