using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PatchConsole
{
    public enum DialogKind { Info, Question, Warning, Danger, Error, Success }

    /// <summary>The dark, right-to-left message box of the console.</summary>
    public static class Dialog
    {
        public static bool Show(Window owner, string title, string message, string ok, string cancel, DialogKind kind)
        {
            var app = Application.Current;
            Brush accent = (Brush)app.FindResource(kind == DialogKind.Danger || kind == DialogKind.Error ? "Bad" :
                                                   kind == DialogKind.Warning ? "Warn" : kind == DialogKind.Success ? "Ok" : "Gold");
            string glyph = kind == DialogKind.Danger ? "" : kind == DialogKind.Error ? "" : kind == DialogKind.Warning ? "" :
                           kind == DialogKind.Success ? "" : kind == DialogKind.Question ? "" : "";
            var w = new Window
            {
                Owner = owner != null && owner.IsVisible ? owner : null,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = owner != null && owner.IsVisible ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
                FlowDirection = FlowDirection.RightToLeft,
                FontFamily = new FontFamily("Segoe UI"),
                ShowInTaskbar = false,
                Foreground = (Brush)app.FindResource("Text"),
                Title = title
            };
            var card = new Border
            {
                Background = (Brush)app.FindResource("Card"),
                BorderBrush = accent,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(26, 22, 26, 18),
                Margin = new Thickness(20),
                MaxWidth = 560,
                MinWidth = 380,
                Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 30, ShadowDepth = 0, Opacity = 0.6 }
            };
            var stack = new StackPanel();
            var head = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 12) };
            head.Children.Add(new TextBlock { Text = glyph, FontFamily = (FontFamily)app.FindResource("Icons"), FontSize = 24, Foreground = accent, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) });
            head.Children.Add(new TextBlock { Text = title, FontSize = 18, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
            stack.Children.Add(head);
            stack.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, FontSize = 14, LineHeight = 22, Foreground = (Brush)app.FindResource("Text") });
            var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 20, 0, 0) };
            var okBtn = new Button { Content = ok, IsDefault = true, MinWidth = 100 };
            okBtn.Style = (Style)app.FindResource(kind == DialogKind.Danger ? "Danger" : "Primary");
            okBtn.Click += (s, e) => { w.DialogResult = true; };
            buttons.Children.Add(okBtn);
            if (cancel != null)
            {
                var c = new Button { Content = cancel, IsCancel = true, MinWidth = 90 };
                buttons.Children.Add(c);
            }
            stack.Children.Add(buttons);
            card.Child = stack;
            w.Content = card;
            w.MouseLeftButtonDown += (s, e) => { if (e.ButtonState == MouseButtonState.Pressed) w.DragMove(); };
            w.Opacity = 0;
            w.Loaded += (s, e) => w.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1, System.TimeSpan.FromMilliseconds(160)));
            if (cancel == null)
                w.KeyDown += (s, e) => { if (e.Key == Key.Escape) w.DialogResult = true; };
            return w.ShowDialog() == true;
        }
    }
}
