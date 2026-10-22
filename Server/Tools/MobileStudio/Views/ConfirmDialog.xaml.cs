using System.Windows;
using System.Windows.Input;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class ConfirmDialog : Window
{
    private string _typeWord;

    public ConfirmDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => CopySupport.AttachTree(this);
    }

    private void Ok_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var sb = new System.Text.StringBuilder(TitleText.Text);
        if (MessageText.Text.Length > 0) sb.AppendLine().Append(MessageText.Text);
        if (StepList.ItemsSource != null)
            foreach (var o in StepList.ItemsSource) sb.AppendLine().Append(o.GetType().GetProperty("N")?.GetValue(o) + ". " + o.GetType().GetProperty("Text")?.GetValue(o));
        if (NoteBox.Visibility == Visibility.Visible) sb.AppendLine().Append(NoteText.Text);
        try { Clipboard.SetDataObject(sb.ToString().Replace("\u200E", "").Replace("\u200F", ""), true); } catch { }
    }

    private void Drag(object sender, MouseButtonEventArgs e) { try { DragMove(); } catch { } }

    private void TypeBox_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
        OkButton.IsEnabled = _typeWord == null || string.Equals(TypeBox.Text.Trim(), _typeWord, StringComparison.OrdinalIgnoreCase);

    /// <summary>Themed confirmation. <paramref name="steps"/> = what will happen, in order. <paramref name="danger"/> = red
    /// glyph + destructive button. <paramref name="typeWord"/> = the user must type it before OK is enabled.
    /// <paramref name="cancelText"/> null = info box (OK only).</summary>
    public static bool Ask(string title, string message, IEnumerable<string> steps = null, string okText = null, bool danger = false,
        string note = null, string typeWord = null, string cancelText = "")
    {
        if (Svc.Headless) return false;
        var d = Create(title, message, steps, okText, danger, note, typeWord, cancelText);
        var owner = Application.Current?.MainWindow;
        if (owner != null && owner.IsVisible) d.Owner = owner;
        else d.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        return d.ShowDialog() == true;
    }

    public static void Info(string title, string message) => Ask(title, message, null, null, false, null, null, null);

    internal static ConfirmDialog Create(string title, string message, IEnumerable<string> steps, string okText, bool danger, string note,
        string typeWord, string cancelText)
    {
        var d = new ConfirmDialog();
        d.TitleText.Text = title;
        d.MessageText.Text = message ?? "";
        d.MessageText.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
        var list = steps?.Select((s, i) => new { N = (i + 1).ToString(), Text = s }).ToList();
        if (list != null && list.Count > 0)
        {
            d.StepList.ItemsSource = list;
            d.StepList.Visibility = Visibility.Visible;
        }
        if (!string.IsNullOrEmpty(note))
        {
            d.NoteText.Text = note;
            d.NoteBox.Visibility = Visibility.Visible;
        }
        if (!string.IsNullOrEmpty(typeWord))
        {
            d._typeWord = typeWord;
            d.TypeLabel.Text = Loc.F("dlg.typeToConfirm", typeWord);
            d.TypePanel.Visibility = Visibility.Visible;
            d.OkButton.IsEnabled = false;
        }
        d.OkButton.Content = okText ?? Loc.Get("dlg.ok");
        if (cancelText == null) d.CancelButton.Visibility = Visibility.Collapsed;
        else d.CancelButton.Content = cancelText.Length == 0 ? Loc.Get("dlg.cancel") : cancelText;
        d.Glyph.Text = danger ? "" : cancelText == null ? "" : "";
        d.Glyph.Foreground = danger ? Brushes2.Err : cancelText == null ? Brushes2.Subtle : Brushes2.Warn;
        if (danger) d.OkButton.Style = (Style)d.FindResource("DangerButton");
        return d;
    }
}
