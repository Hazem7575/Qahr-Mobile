using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ScriptHazem.Kit;

namespace MobileStudio.Core;

/// <summary>Copy support for every console, log list and table in the tool: Ctrl+A / Ctrl+C, a right-click menu (copy,
/// copy all, save to file) and the small "copy all" button. Hooked once for all ListBox / ListView / read-only TextBox.</summary>
public static class CopySupport
{
    private static bool _registered;

    /// <summary>Tab switches show new content: attach to it too. Pages and dialogs call <see cref="AttachTree"/>.</summary>
    public static void Register()
    {
        if (_registered) return;
        _registered = true;
        EventManager.RegisterClassHandler(typeof(TabControl), System.Windows.Controls.Primitives.Selector.SelectionChangedEvent,
            new SelectionChangedEventHandler((s, e) =>
            {
                if (s is TabControl tc && ReferenceEquals(e.OriginalSource, tc))
                    tc.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, () => AttachTree(tc));
            }));
    }

    /// <summary>Adds copy support to every ListBox / ListView / read-only TextBox under <paramref name="root"/>.</summary>
    public static void AttachTree(DependencyObject root)
    {
        if (root == null) return;
        if (root is ListBox lb) Attach(lb);
        else if (root is TextBox tb) Attach(tb);
        int n = root is Visual || root is System.Windows.Media.Media3D.Visual3D ? VisualTreeHelper.GetChildrenCount(root) : 0;
        for (int i = 0; i < n; i++) AttachTree(VisualTreeHelper.GetChild(root, i));
    }

    // ------------------------------------------------------------------ text of items
    /// <summary>The text of one row: LogLine/string as is, other objects = their visible values joined with tabs.</summary>
    public static string RowText(object item)
    {
        switch (item)
        {
            case null: return "";
            case string s: return s;
            case LogLine l: return l.Text;
        }
        var t = item.GetType();
        var tos = t.GetMethod("ToString", Type.EmptyTypes);
        if (tos != null && tos.DeclaringType != typeof(object)) return item.ToString();
        var vals = new List<string>();
        foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.GetIndexParameters().Length > 0) continue;
            var pt = p.PropertyType;
            if (typeof(Brush).IsAssignableFrom(pt) || pt == typeof(Visibility) || pt == typeof(bool)) continue;
            if (!(pt.IsPrimitive || pt == typeof(string) || pt == typeof(decimal) || pt == typeof(DateTime) || Nullable.GetUnderlyingType(pt) != null)) continue;
            // skip raw values that have a "...Text" twin (SizeText next to Size)
            if (t.GetProperty(p.Name + "Text") != null) continue;
            object v;
            try { v = p.GetValue(item); } catch { continue; }
            string sv = Convert.ToString(v, Loc.Culture)?.Replace("‎", "").Replace("‏", "");
            if (!string.IsNullOrEmpty(sv)) vals.Add(sv);
        }
        return string.Join("\t", vals);
    }

    public static string ItemsText(IEnumerable items) => string.Join(Environment.NewLine, items.Cast<object>().Select(RowText));

    public static string AllText(object target) => target switch
    {
        TextBox tb => tb.Text,
        ItemsControl ic => ItemsText(ic.Items),
        TextBlock b => b.Text,
        _ => "",
    };

    public static void SetClipboard(string text, int count = -1)
    {
        if (string.IsNullOrEmpty(text)) return;
        for (int i = 0; i < 3; i++)
        {
            try
            {
                Clipboard.SetDataObject(text, true);
                MobileStudio.MainWindow.Toast_(count >= 0 ? Loc.F("copy.copiedN", count) : Loc.Get("copy.copied"));
                return;
            }
            catch { Thread.Sleep(80); }
        }
        MobileStudio.MainWindow.Toast_(Loc.Get("copy.busy"), true);
    }

    public static void SaveToFile(string text, string name)
    {
        var d = new Microsoft.Win32.SaveFileDialog { FileName = (name ?? "log") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt", Filter = "Text|*.txt|All|*.*" };
        if (d.ShowDialog() != true) return;
        try { File.WriteAllText(d.FileName, text, new System.Text.UTF8Encoding(false)); MobileStudio.MainWindow.Toast_(Loc.FB("copy.saved", d.FileName)); }
        catch (Exception ex) { MobileStudio.MainWindow.Toast_(ex.Message, true); }
    }

    private static MenuItem Item(string key, string glyph, string gesture, Action a)
    {
        var mi = new MenuItem { Header = Loc.Get(key), InputGestureText = gesture };
        mi.Icon = new TextBlock { Text = glyph, FontFamily = (FontFamily)Application.Current.FindResource("IconFont"), FontSize = 12 };
        mi.Click += (_, _) => a();
        return mi;
    }

    // ------------------------------------------------------------------ lists / tables
    private static void Attach(ListBox lb)
    {
        if (lb == null || lb.Tag as string == "nocopy" || lb.ContextMenu != null || lb.ReadLocalValue(AttachedProperty) is true) return;
        lb.SetValue(AttachedProperty, true);
        if (lb.SelectionMode == SelectionMode.Single) lb.SelectionMode = SelectionMode.Extended;
        string name = lb.Name;
        void CopySel()
        {
            var set = new HashSet<object>(lb.SelectedItems.Cast<object>());
            var sel = lb.Items.Cast<object>().Where(set.Contains).ToList(); // keep display order
            if (sel.Count == 0) { SetClipboard(ItemsText(lb.Items), lb.Items.Count); return; }
            SetClipboard(ItemsText(sel), sel.Count);
        }
        lb.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (_, e) => { CopySel(); e.Handled = true; }));
        string cell = null;
        lb.PreviewMouseRightButtonDown += (_, e) =>
        {
            cell = null;
            DependencyObject d = e.OriginalSource as DependencyObject;
            while (d != null && d is not ListBoxItem)
            {
                if (cell == null && d is TextBlock tbk && !string.IsNullOrEmpty(tbk.Text)) cell = tbk.Text;
                d = d is Visual || d is System.Windows.Media.Media3D.Visual3D ? VisualTreeHelper.GetParent(d) : LogicalTreeHelper.GetParent(d);
            }
            // right-click on an unselected row selects it (like Explorer)
            if (d is ListBoxItem it && !it.IsSelected) { lb.SelectedItems.Clear(); it.IsSelected = true; }
        };
        lb.ContextMenuOpening += (_, _) =>
        {
            var cm = lb.ContextMenu; cm.Items.Clear();
            if (lb is ListView && cell != null)
            {
                string c = cell.Replace("‎", "").Replace("‏", "");
                cm.Items.Add(Item("copy.cell", "", "", () => SetClipboard(c)));
            }
            cm.Items.Add(Item(lb is ListView ? "copy.rows" : "copy.sel", "", "Ctrl+C", CopySel));
            cm.Items.Add(Item("copy.all", "", "", () => SetClipboard(ItemsText(lb.Items), lb.Items.Count)));
            cm.Items.Add(Item("copy.selectAll", "", "Ctrl+A", () => lb.SelectAll()));
            cm.Items.Add(new Separator());
            cm.Items.Add(Item("copy.save", "", "", () => SaveToFile(ItemsText(lb.Items), name)));
        };
        lb.ContextMenu = new ContextMenu(); // placeholder so ContextMenuOpening fires; rebuilt with current texts each time
    }

    private static readonly DependencyProperty AttachedProperty =
        DependencyProperty.RegisterAttached("CopyAttached", typeof(bool), typeof(CopySupport), new PropertyMetadata(false));

    // ------------------------------------------------------------------ read-only text boxes (outputs)
    private static void Attach(TextBox tb)
    {
        if (tb == null || !tb.IsReadOnly || tb.ReadLocalValue(AttachedProperty) is true) return;
        tb.SetValue(AttachedProperty, true);
        tb.IsReadOnlyCaretVisible = true;
        string name = tb.Name;
        tb.ContextMenuOpening += (_, _) =>
        {
            var cm = tb.ContextMenu; cm.Items.Clear();
            cm.Items.Add(Item("copy.sel", "", "Ctrl+C", () => SetClipboard(tb.SelectedText.Length > 0 ? tb.SelectedText : tb.Text)));
            cm.Items.Add(Item("copy.all", "", "", () => SetClipboard(tb.Text)));
            cm.Items.Add(Item("copy.selectAll", "", "Ctrl+A", () => { tb.Focus(); tb.SelectAll(); }));
            cm.Items.Add(new Separator());
            cm.Items.Add(Item("copy.save", "", "", () => SaveToFile(tb.Text, name)));
        };
        tb.ContextMenu = new ContextMenu();
    }

    /// <summary>True when the user is reading: a selection is active, or the list/box is scrolled away from the end.
    /// Auto-refresh then appends without scrolling (and the Logs page pauses).</summary>
    public static bool UserIsReading(Control c)
    {
        if (c is ListBox lb && lb.SelectedItems.Count > 0) return true;
        if (c is TextBox tb && tb.SelectionLength > 0) return true;
        var sv = FindScroll(c);
        return sv != null && sv.ScrollableHeight > 0 && sv.VerticalOffset < sv.ScrollableHeight - 2;
    }

    public static ScrollViewer FindScroll(DependencyObject d)
    {
        if (d == null) return null;
        if (d is ScrollViewer s) return s;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        {
            var r = FindScroll(VisualTreeHelper.GetChild(d, i));
            if (r != null) return r;
        }
        return null;
    }
}

/// <summary>The small "copy all" button above a console: Target = the ListBox / TextBox it copies.</summary>
public sealed class CopyAllButton : Button
{
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(object), typeof(CopyAllButton));

    public object Target { get => GetValue(TargetProperty); set => SetValue(TargetProperty, value); }

    public CopyAllButton()
    {
        SetResourceReference(StyleProperty, "QuietButton");
        Padding = new Thickness(8, 2, 8, 3);
        Margin = new Thickness(6, 0, 0, 0);
        var glyph = new TextBlock { Text = "", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 1, 6, 0), FontSize = 11.5 };
        glyph.SetResourceReference(TextBlock.FontFamilyProperty, "IconFont");
        glyph.SetResourceReference(TextBlock.ForegroundProperty, "IconBrush");
        var text = new TextBlock { FontSize = 11.5, VerticalAlignment = VerticalAlignment.Center };
        text.SetBinding(TextBlock.TextProperty, new Binding("[copy.allBtn]") { Source = Loc.I });
        Content = new StackPanel { Orientation = Orientation.Horizontal, Children = { glyph, text } };
        SetBinding(ToolTipProperty, new Binding("[copy.allTip]") { Source = Loc.I });
        Click += (_, _) =>
        {
            var t = Target;
            int n = t is ItemsControl ic ? ic.Items.Count : -1;
            CopySupport.SetClipboard(CopySupport.AllText(t), n);
        };
    }
}
