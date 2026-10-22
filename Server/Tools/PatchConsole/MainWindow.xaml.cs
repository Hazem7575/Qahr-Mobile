using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PatchConsole
{
    /// <summary>One line of a check list (review / tests).</summary>
    public sealed class CheckLine
    {
        public string Glyph { get; set; }
        public string Text { get; set; }
        public Brush Brush { get; set; }

        public static CheckLine Of(bool? ok, string text) => new CheckLine
        {
            Text = text,
            Glyph = ok == true ? "\uE73E" : ok == false ? "\uE711" : "\uE7BA",
            Brush = (Brush)Application.Current.FindResource(ok == true ? "Ok" : ok == false ? "Bad" : "Warn")
        };
    }

    /// <summary>A file or folder of the client resources in the tree (children are read when the folder opens).</summary>
    public sealed class FsNode : INotifyPropertyChanged
    {
        bool expanded;
        bool? isChecked = false;
        bool loaded;
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDir { get; set; }
        public long Size { get; set; }
        public string Glyph => IsDir ? "\uE8B7" : "\uE8A5";
        public string SizeText => IsDir ? "" : PatchCore.FormatSize(Size);
        public ObservableCollection<FsNode> Children { get; } = new ObservableCollection<FsNode>();
        public event PropertyChangedEventHandler PropertyChanged;

        public static FsNode Make(string path, bool dir)
        {
            var n = new FsNode { Name = System.IO.Path.GetFileName(path), FullPath = path, IsDir = dir };
            if (dir)
                n.Children.Add(new FsNode { Name = "..." });
            else
                try { n.Size = new FileInfo(path).Length; } catch { }
            return n;
        }

        public bool IsExpanded
        {
            get => expanded;
            set
            {
                expanded = value;
                if (value && !loaded)
                    Load();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
            }
        }

        public bool? IsChecked
        {
            get => isChecked;
            set { isChecked = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked))); }
        }

        void Load()
        {
            loaded = true;
            Children.Clear();
            try
            {
                foreach (string d in Directory.GetDirectories(FullPath).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    Children.Add(Make(d, true));
                foreach (string f in Directory.GetFiles(FullPath).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    Children.Add(Make(f, false));
            }
            catch { }
            if (isChecked == true)
                foreach (var c in Children)
                    c.IsChecked = true;
        }
    }

    public partial class MainWindow : Window
    {
        ServerConfig cfg;
        string configPath, baseRoot, sourceRoot;
        List<PatchInfo> patches = new List<PatchInfo>();
        List<StatEvent> events = new List<StatEvent>();
        readonly ObservableCollection<StageItem> staged = new ObservableCollection<StageItem>();
        readonly ObservableCollection<CheckLine> testLines = new ObservableCollection<CheckLine>();
        int step = 1;
        string builtZip;
        PatchMeta builtMeta;
        bool busy;
        Dictionary<string, List<string>> nameIndex;
        readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        static readonly string SettingsFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PatchConsole", "settings.txt");
        const string DefaultBase = @"D:\conquer\_private\comobile\work\base_smali\assets\ConquerRes";

        static readonly string[] StepNames = { "اختيار الملفات", "المقارنة", "الملاحظات", "المراجعة والنشر" };

        readonly string[] startPaths;

        public MainWindow() : this(new string[0]) { }

        public MainWindow(string[] paths)
        {
            startPaths = paths ?? new string[0];
            InitializeComponent();
            StageList.ItemsSource = staged;
            CompareGrid.ItemsSource = staged;
            TestResults.ItemsSource = testLines;
            staged.CollectionChanged += (s, e) => StageCount.Text = "الملفات المختارة (" + staged.Count + ")";
            BuildStepBar();
            LoadSettings();
            OpsLog.Added += l => Dispatcher.BeginInvoke(new Action(RefreshOps));
            timer.Tick += (s, e) => RefreshLive();
            Loaded += (s, e) =>
            {
                RefreshAll();
                ShowStep(1);
                timer.Start();
                if (startPaths.Length > 0)
                {
                    NavNew.IsChecked = true;
                    AddPaths(startPaths);
                }
                StartLampPulse();
            };
        }

        // ================================================================================================ settings
        void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                    foreach (string l in File.ReadAllLines(SettingsFile))
                    {
                        int eq = l.IndexOf('=');
                        if (eq < 0) continue;
                        string k = l.Substring(0, eq), v = l.Substring(eq + 1);
                        if (k == "config") configPath = v;
                        else if (k == "base") baseRoot = v;
                        else if (k == "source") sourceRoot = v;
                    }
            }
            catch { }
            if (configPath == null || !File.Exists(configPath))
                configPath = PatchCore.FindConfig();
            if (baseRoot == null || !Directory.Exists(baseRoot))
                baseRoot = Directory.Exists(DefaultBase) ? DefaultBase : null;
            if (sourceRoot == null || !Directory.Exists(sourceRoot))
                sourceRoot = baseRoot;
            LoadTree();
        }

        void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SettingsFile));
                File.WriteAllLines(SettingsFile, new[] { "config=" + configPath, "base=" + baseRoot, "source=" + sourceRoot });
            }
            catch { }
        }

        string PatchDir => cfg?.PatchDir;

        // ================================================================================================ refresh
        void Refresh_Click(object sender, RoutedEventArgs e) { RefreshAll(); ShowToast("تم تحديث البيانات", true); }

        void RefreshAll()
        {
            try { cfg = configPath != null && File.Exists(configPath) ? new ServerConfig(configPath) : null; }
            catch (Exception ex) { cfg = null; OpsLog.Write("خطأ", "قراءة ServerConfig.ini: " + ex.Message); }
            OpsLog.Dir = PatchDir;
            patches = cfg != null ? PatchCore.List(PatchDir) : new List<PatchInfo>();
            events = cfg != null ? Stats.Read(PatchDir, false) : new List<StatEvent>();
            var dl = Stats.DownloadsPerVersion(events);
            foreach (var p in patches)
                p.Downloads = dl.TryGetValue(p.Version, out int n) ? n : 0;
            RefreshHome();
            RefreshHistory();
            RefreshStats();
            RefreshOfficial();
            RefreshSettings();
            RefreshOps();
            RefreshLive();
        }

        /// <summary>Ports and lamps, every 5 s.</summary>
        void RefreshLive()
        {
            bool cOn = cfg != null && PatchCore.IsListening(cfg.CheckPort);
            bool hOn = cfg != null && PatchCore.IsListening(cfg.HttpPort);
            bool update = cfg != null && cfg.UpdateMode;
            bool running = cOn && (hOn || !update);
            var ok = (Brush)FindResource("Ok");
            var bad = (Brush)FindResource("Bad");
            var warn = (Brush)FindResource("Warn");
            Brush lamp = !running ? bad : update ? ok : warn;
            Lamp.Fill = LampHalo.Fill = SideLamp.Fill = lamp;
            CardServer.Text = running ? "شغال" : "واقف";
            CardServerSub.Text = cfg == null ? "ملف الإعدادات غير موجود" :
                                 !cOn ? "البورت " + cfg.CheckPort + " مقفول - الـ AccServer مش شغال؟" :
                                 update && !hOn ? "وضع Update لكن HTTP " + cfg.HttpPort + " مقفول - أعد تشغيل الـ AccServer" :
                                 update ? "وضع Update - البورتات " + cfg.CheckPort + " و " + cfg.HttpPort : "وضع Ready - مفيش تحديثات بتتبعت";
            SideLampText.Text = running ? (update ? "التحديثات شغالة" : "السيرفر شغال (Ready)") : "سيرفر التحديث واقف";
            if (cfg != null)
            {
                CheckLamp.Fill = cOn ? ok : bad;
                CheckLampText.Text = cOn ? "شغال" : "مقفول";
                HttpLamp.Fill = hOn ? ok : bad;
                HttpLampText.Text = hOn ? "شغال" : (update ? "مقفول" : "مقفول (وضع Ready)");
            }
        }

        void StartLampPulse()
        {
            var a = new DoubleAnimation(0.45, 0.05, TimeSpan.FromSeconds(1.4)) { RepeatBehavior = RepeatBehavior.Forever, AutoReverse = true };
            LampHalo.BeginAnimation(OpacityProperty, a);
            var s = new DoubleAnimation(14, 24, TimeSpan.FromSeconds(1.4)) { RepeatBehavior = RepeatBehavior.Forever, AutoReverse = true };
            LampHalo.BeginAnimation(WidthProperty, s);
            LampHalo.BeginAnimation(HeightProperty, s);
        }

        void RefreshHome()
        {
            int current = PatchCore.CurrentVersion(patches);
            int okCount = patches.Count(p => p.Ok);
            AnimateNumber(HeroVersion, current);
            HeroSub.Text = okCount == 0 ? "النسخة الأساسية للـ APK بدون تحديثات" : "النسخة الأساسية 5150 + " + okCount + " تحديث";
            bool update = cfg != null && cfg.UpdateMode;
            HeroMode.Text = update ? "التحديثات شغالة" : "التحديثات موقوفة (Ready)";
            HeroModeBadge.Background = (Brush)FindResource(update ? "OkSoft" : "WarnSoft");
            SideVersion.Text = "إصدار اللاعبين: " + current;
            AnimateNumber(CardCount, patches.Count);
            CardCountSub.Text = patches.Count == 0 ? "لا يوجد" : "آخر واحد " + patches.Last().Version + (patches.Any(p => !p.Ok) ? " - فيه ملفات بمشكلة" : "");
            var last = patches.LastOrDefault(p => p.Ok);
            AnimateNumber(CardDownloads, last?.Downloads ?? 0);
            CardDownloadsSub.Text = last == null ? "مفيش تحديث منشور" : "جهاز نزّل " + last.Version + " كامل";
            var lastEv = events.OrderByDescending(e => e.Time).FirstOrDefault();
            var lastOp = OpsLog.Read(1).FirstOrDefault();
            DateTime? opTime = lastOp != null && DateTime.TryParse(lastOp[0], out var t) ? t : (DateTime?)null;
            if (lastEv == null && opTime == null)
            {
                CardLast.Text = "-";
                CardLastSub.Text = "لا يوجد نشاط بعد";
            }
            else if (lastEv != null && (opTime == null || lastEv.Time >= opTime))
            {
                CardLast.Text = PatchCore.Ago(lastEv.Time);
                CardLastSub.Text = lastEv.Kind == "get" ? "تنزيل " + lastEv.Version + " " + lastEv.ResultText : "موبايل سأل عن الإصدار " + lastEv.Version;
            }
            else
            {
                CardLast.Text = PatchCore.Ago(opTime.Value);
                CardLastSub.Text = lastOp[3];
            }
            var recent = events.OrderByDescending(e => e.Time).Take(7).ToList();
            HomeEvents.ItemsSource = recent;
            HomeEventsEmpty.Visibility = recent.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        void RefreshOps()
        {
            var ops = OpsLog.Read(500);
            HomeOps.ItemsSource = ops.Take(7).ToList();
            HomeOpsEmpty.Visibility = ops.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LogGrid.ItemsSource = ops;
            LogPath.Text = PatchDir != null ? System.IO.Path.Combine(PatchDir, PatchCore.MetaFolder, "operations.log") : "";
        }

        static void AnimateNumber(TextBlock tb, int to)
        {
            int from;
            int.TryParse(tb.Text, out from);
            if (from == to || Math.Abs(to - from) > 5000 && from != 0)
            {
                tb.Text = to.ToString();
                return;
            }
            var start = DateTime.Now;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (s, e) =>
            {
                double k = Math.Min(1, (DateTime.Now - start).TotalMilliseconds / 600);
                k = 1 - Math.Pow(1 - k, 3);
                tb.Text = ((int)Math.Round(from + (to - from) * k)).ToString();
                if (k >= 1) timer.Stop();
            };
            timer.Start();
        }

        // ================================================================================================ navigation
        void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            FrameworkElement page = PageHome;
            string title = "الرئيسية", sub = "حالة التحديثات عند اللاعبين";
            if (sender == NavNew) { page = PageNew; title = "تحديث جديد"; sub = "اختار الملفات، راجع، واكتب الملاحظات، وبعدين انشر"; }
            else if (sender == NavHistory) { page = PageHistory; title = "سجل التحديثات"; sub = "كل الإصدارات المنشورة، مع الرجوع والحذف"; }
            else if (sender == NavOfficial) { page = PageOfficial; title = "التحديثات الرسمية"; sub = "باتشات TQ الرسمية وأرقامها عندنا"; }
            else if (sender == NavStats) { page = PageStats; title = "الإحصائيات"; sub = "التنزيلات وفحوصات الإصدار اللي سجّلها السيرفر"; }
            else if (sender == NavSettings) { page = PageSettings; title = "الإعدادات والاختبار"; sub = "وضع التحديث، البورتات، المجلدات، واختبار زي الموبايل"; }
            else if (sender == NavLog) { page = PageLog; title = "سجل العمليات"; sub = "كل اللي اتعمل من الأداة"; }
            PageTitle.Text = title;
            PageSub.Text = sub;
            foreach (FrameworkElement p in Pages.Children)
                p.Visibility = p == page ? Visibility.Visible : Visibility.Collapsed;
            Animate(page);
        }

        static void Animate(FrameworkElement page)
        {
            var tt = new TranslateTransform(0, 14);
            page.RenderTransform = tt;
            page.Opacity = 0;
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            page.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(260)) { EasingFunction = ease });
            tt.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(14, 0, TimeSpan.FromMilliseconds(320)) { EasingFunction = ease });
        }

        void GoNew_Click(object sender, RoutedEventArgs e) => NavNew.IsChecked = true;
        void GoHistory_Click(object sender, RoutedEventArgs e) => NavHistory.IsChecked = true;
        void GoTest_Click(object sender, RoutedEventArgs e) { NavSettings.IsChecked = true; FullTest_Click(sender, e); }

        void ShowToast(string text, bool ok)
        {
            ToastText.Text = text;
            ToastIcon.Text = ok ? "\uE73E" : "\uE7BA";
            ToastIcon.Foreground = (Brush)FindResource(ok ? "Ok" : "Warn");
            Toast.Visibility = Visibility.Visible;
            var sb = new Storyboard();
            var fade = new DoubleAnimationUsingKeyFrames();
            fade.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            fade.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            fade.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(2800))));
            fade.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(3200))));
            Storyboard.SetTarget(fade, Toast);
            Storyboard.SetTargetProperty(fade, new PropertyPath(OpacityProperty));
            sb.Children.Add(fade);
            sb.Completed += (s, e) => Toast.Visibility = Visibility.Collapsed;
            sb.Begin();
            ToastMove.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250)) { EasingFunction = new CubicEase() });
        }

        bool Ask(string title, string msg, string ok, DialogKind kind) => Dialog.Show(this, title, msg, ok, "إلغاء", kind);
        void Fail(string msg) => Dialog.Show(this, "تنبيه", msg, "حسنا", null, DialogKind.Warning);

        // ================================================================================================ wizard
        void BuildStepBar()
        {
            for (int i = 0; i < 4; i++)
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                var dot = new Border { Style = (Style)FindResource("StepDot"), Name = "Dot" + i };
                dot.Child = new TextBlock { Text = (i + 1).ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.Bold };
                sp.Children.Add(dot);
                sp.Children.Add(new TextBlock { Text = StepNames[i], Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold });
                Grid.SetColumn(sp, i * 2);
                StepBar.Children.Add(sp);
                if (i < 3)
                {
                    var line = new Border { Height = 2, Margin = new Thickness(14, 0, 14, 0), Background = (Brush)FindResource("Line"), CornerRadius = new CornerRadius(1) };
                    Grid.SetColumn(line, i * 2 + 1);
                    StepBar.Children.Add(line);
                }
            }
        }

        void PaintStepBar()
        {
            var gold = (Brush)FindResource("Gold");
            var line = (Brush)FindResource("Line");
            var card = (Brush)FindResource("Card");
            foreach (UIElement el in StepBar.Children)
            {
                int col = Grid.GetColumn(el);
                if (el is StackPanel sp)
                {
                    int i = col / 2 + 1;
                    var dot = (Border)sp.Children[0];
                    var num = (TextBlock)dot.Child;
                    var label = (TextBlock)sp.Children[1];
                    dot.BorderBrush = i <= step ? gold : line;
                    dot.Background = i < step ? gold : i == step ? (Brush)FindResource("GoldSoft") : card;
                    num.Text = i < step ? "\uE73E" : i.ToString();
                    num.FontFamily = i < step ? (FontFamily)FindResource("Icons") : FontFamily;
                    num.Foreground = i < step ? new SolidColorBrush(Color.FromRgb(0x1A, 0x13, 0x08)) : (Brush)FindResource("Text");
                    label.Foreground = (Brush)FindResource(i <= step ? "Text" : "Muted");
                }
                else if (el is Border b)
                    b.Background = (col + 1) / 2 < step ? gold : line;
            }
        }

        void ShowStep(int n)
        {
            step = n;
            FrameworkElement[] all = { Step1, Step2, Step3, Step4 };
            for (int i = 0; i < 4; i++)
                all[i].Visibility = i == n - 1 ? Visibility.Visible : Visibility.Collapsed;
            Animate(all[n - 1]);
            PaintStepBar();
            BackBtn.IsEnabled = n > 1 && !busy;
            NextText.Text = n == 4 ? "نشر التحديث" : "التالي";
            NextIcon.Text = n == 4 ? "\uE898" : "\uE72B";
            WizardNav.Visibility = Visibility.Visible;
            WizardHint.Text = n == 1 ? "اختار من الشجرة أو اسحب ملفات، وبعدين التالي." :
                              n == 2 ? "الملفات المطابقة اتشالت لوحدها. راجع المسارات." :
                              n == 3 ? "اكتب إيه اللي اتغير علشان يبقى في السجل." : "راجع كل حاجة قبل النشر.";
        }

        async void Next_Click(object sender, RoutedEventArgs e)
        {
            if (busy)
                return;
            if (cfg == null) { Fail("اختار ملف ServerConfig.ini الأول من الإعدادات."); return; }
            try
            {
                if (step == 1)
                {
                    if (staged.Count == 0) { Fail("اختار ملف واحد على الأقل."); return; }
                    await Compare();
                    ShowStep(2);
                }
                else if (step == 2)
                {
                    var inc = staged.Where(s => s.Include).ToList();
                    if (inc.Count == 0) { Fail("مفيش ملفات متعلم عليها للإرسال (كل الملفات مطابقة أو مرفوضة)."); return; }
                    var bad = inc.FirstOrDefault(s => (PatchCore.CheckEntryName(s.Target) ?? PatchCore.Forbidden(s.Target)) != null);
                    if (bad != null) { Fail("المسار مش مقبول: " + bad.Target + "\n" + (PatchCore.CheckEntryName(bad.Target) ?? PatchCore.Forbidden(bad.Target))); return; }
                    var dup = inc.GroupBy(s => s.Target, StringComparer.OrdinalIgnoreCase).FirstOrDefault(g => g.Count() > 1);
                    if (dup != null) { Fail("نفس المسار متكرر: " + dup.Key); return; }
                    int next = PatchCore.LastUsedVersion(PatchDir) + 1;
                    if (!int.TryParse(VersionBox.Text, out int v) || v < next)
                        VersionBox.Text = next.ToString();
                    VersionHint.Text = "المقترح " + next + ": الرقم الفاضي بعد آخر رقم مستخدم (" + (next - 1) + ")، رسمي أو بتاعنا";
                    ShowStep(3);
                }
                else if (step == 3)
                {
                    int last = PatchCore.LastUsedVersion(PatchDir);
                    if (!int.TryParse(VersionBox.Text.Trim(), out int v) || v <= last || v >= 9999 || VersionBox.Text.Trim().Length != 4)
                    {
                        Fail("رقم الإصدار لازم يكون 4 أرقام وأكبر من " + last + " (الرقم ده ممكن يكون عند لاعبين، ومينفعش يتكرر).");
                        return;
                    }
                    if (v != last + 1 && !Ask("رقم الإصدار", "الرقم المقترح " + (last + 1) + ". تكمل بـ " + v + "؟", "أكمل", DialogKind.Question))
                        return;
                    await BuildReview(v);
                    ShowStep(4);
                }
                else
                    await Publish();
            }
            catch (Exception ex)
            {
                busy = false;
                Fail(ex.Message);
            }
        }

        void Back_Click(object sender, RoutedEventArgs e)
        {
            if (busy || step <= 1)
                return;
            ShowStep(step - 1);
        }

        void NewAgain_Click(object sender, RoutedEventArgs e)
        {
            staged.Clear();
            TitleBox.Text = NotesBox.Text = VersionBox.Text = "";
            DonePanel.Visibility = Visibility.Collapsed;
            PublishPanel.Visibility = Visibility.Visible;
            UncheckTree(FileTree.ItemsSource as IEnumerable<FsNode>);
            ShowStep(1);
        }

        // ---------------------------------------------------------------------------------------- step 1: files
        void LoadTree()
        {
            SourceBox.Text = sourceRoot ?? "";
            FileTree.ItemsSource = null;
            if (sourceRoot == null || !Directory.Exists(sourceRoot))
                return;
            var root = new List<FsNode>();
            foreach (string d in Directory.GetDirectories(sourceRoot).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                root.Add(FsNode.Make(d, true));
            foreach (string f in Directory.GetFiles(sourceRoot).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                root.Add(FsNode.Make(f, false));
            FileTree.ItemsSource = root;
        }

        static void UncheckTree(IEnumerable<FsNode> nodes)
        {
            if (nodes == null) return;
            foreach (var n in nodes)
            {
                n.IsChecked = false;
                UncheckTree(n.Children);
            }
        }

        void PickSource_Click(object sender, RoutedEventArgs e)
        {
            var d = new Microsoft.Win32.OpenFolderDialog { Title = "مجلد ملفات الكلاينت (اللي فيه ini و luaui ...)" };
            if (sourceRoot != null) d.InitialDirectory = sourceRoot;
            if (d.ShowDialog(this) != true) return;
            sourceRoot = d.FolderName;
            SaveSettings();
            LoadTree();
        }

        void TreeCheck_Click(object sender, RoutedEventArgs e)
        {
            var node = (FsNode)((FrameworkElement)sender).Tag;
            bool on = node.IsChecked == true;
            if (node.IsDir)
            {
                SetChecked(node.Children, on);
                if (on)
                {
                    var files = Directory.GetFiles(node.FullPath, "*", SearchOption.AllDirectories);
                    if (files.Length > 3000 && !Ask("مجلد كبير", "المجلد فيه " + files.Length + " ملف. تضيفهم كلهم؟", "أضف", DialogKind.Warning))
                    {
                        node.IsChecked = false;
                        SetChecked(node.Children, false);
                        return;
                    }
                    foreach (string f in files.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                        Stage(f, RelTo(sourceRoot, f));
                }
                else
                {
                    string prefix = node.FullPath.TrimEnd('\\') + "\\";
                    foreach (var s in staged.Where(s => s.Source.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList())
                        staged.Remove(s);
                }
            }
            else if (on)
                Stage(node.FullPath, RelTo(sourceRoot, node.FullPath));
            else
                foreach (var s in staged.Where(s => s.Source.Equals(node.FullPath, StringComparison.OrdinalIgnoreCase)).ToList())
                    staged.Remove(s);
        }

        static void SetChecked(IEnumerable<FsNode> nodes, bool on)
        {
            foreach (var n in nodes)
            {
                if (n.Name == "..." && n.FullPath == null) continue;
                n.IsChecked = on;
                SetChecked(n.Children, on);
            }
        }

        static string RelTo(string root, string file)
        {
            if (root == null) return null;
            string r = System.IO.Path.GetFullPath(root).TrimEnd('\\') + "\\";
            string f = System.IO.Path.GetFullPath(file);
            return f.StartsWith(r, StringComparison.OrdinalIgnoreCase) ? f.Substring(r.Length).Replace('\\', '/') : null;
        }

        void Stage(string source, string target)
        {
            if (target == null) return;
            target = PatchCore.NormTarget(target);
            if (target.Equals("version.dat", StringComparison.OrdinalIgnoreCase))
                return;// the tool writes its own
            foreach (var old in staged.Where(s => s.Target.Equals(target, StringComparison.OrdinalIgnoreCase)).ToList())
                staged.Remove(old);
            long size = 0;
            try { size = new FileInfo(source).Length; } catch { }
            staged.Add(new StageItem { Source = System.IO.Path.GetFullPath(source), Target = target, Size = size });
        }

        void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            var d = new Microsoft.Win32.OpenFileDialog { Multiselect = true, Title = "اختار الملفات المعدلة" };
            if (sourceRoot != null) d.InitialDirectory = sourceRoot;
            if (d.ShowDialog(this) == true)
                AddPaths(d.FileNames);
        }

        void Drop_DragEnter(object sender, DragEventArgs e)
        {
            DropZone.BorderBrush = (Brush)FindResource("Gold");
            DropZone.Background = (Brush)FindResource("GoldSoft");
        }

        void Drop_DragLeave(object sender, DragEventArgs e)
        {
            DropZone.BorderBrush = new SolidColorBrush(Color.FromArgb(0x55, 0xD9, 0xAE, 0x5C));
            DropZone.Background = new SolidColorBrush(Color.FromArgb(0x0C, 0xD9, 0xAE, 0x5C));
        }

        void Drop_Drop(object sender, DragEventArgs e)
        {
            Drop_DragLeave(sender, e);
            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                AddPaths(paths);
        }

        Dictionary<string, List<string>> NameIndex()
        {
            if (nameIndex != null) return nameIndex;
            nameIndex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            if (baseRoot != null && Directory.Exists(baseRoot))
                foreach (string f in Directory.EnumerateFiles(baseRoot, "*", SearchOption.AllDirectories))
                {
                    string n = System.IO.Path.GetFileName(f);
                    if (!nameIndex.TryGetValue(n, out var l)) nameIndex[n] = l = new List<string>();
                    l.Add(RelTo(baseRoot, f));
                }
            return nameIndex;
        }

        /// <summary>Where dropped things go: inside the source folder they keep their place; a folder that looks like the
        /// resource root (has ini, luaui ...) goes as is; a folder named like a resource folder goes under that name; a
        /// single file goes where the file of the same name is in the base resources (when there is exactly one).</summary>
        void AddPaths(IEnumerable<string> paths)
        {
            var topDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (baseRoot != null && Directory.Exists(baseRoot))
                foreach (string d in Directory.GetDirectories(baseRoot))
                    topDirs.Add(System.IO.Path.GetFileName(d));
            int added = 0, guessed = 0;
            foreach (string p in paths)
            {
                if (Directory.Exists(p))
                {
                    string inside = RelTo(sourceRoot, System.IO.Path.Combine(p, "x"));
                    string prefix;
                    if (inside != null)
                        prefix = inside.Substring(0, inside.Length - 1);
                    else if (Directory.GetDirectories(p).Any(d => topDirs.Contains(System.IO.Path.GetFileName(d))))
                        prefix = "";
                    else if (topDirs.Contains(System.IO.Path.GetFileName(p)))
                        prefix = System.IO.Path.GetFileName(p) + "/";
                    else
                        prefix = "";
                    foreach (string f in Directory.GetFiles(p, "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    {
                        Stage(f, prefix + System.IO.Path.GetRelativePath(p, f).Replace('\\', '/'));
                        added++;
                    }
                }
                else if (File.Exists(p))
                {
                    string rel = RelTo(sourceRoot, p);
                    if (rel == null)
                    {
                        NameIndex().TryGetValue(System.IO.Path.GetFileName(p), out var hits);
                        rel = hits != null && hits.Count == 1 ? hits[0] : System.IO.Path.GetFileName(p);
                        if (hits == null || hits.Count != 1) guessed++;
                    }
                    Stage(p, rel);
                    added++;
                }
            }
            ShowToast("اتضاف " + added + " ملف" + (guessed > 0 ? " - " + guessed + " محتاج تحدد مساره في خطوة المقارنة" : ""), guessed == 0);
        }

        void RemoveStage_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is StageItem s)
                staged.Remove(s);
        }

        void ClearStage_Click(object sender, RoutedEventArgs e)
        {
            staged.Clear();
            UncheckTree(FileTree.ItemsSource as IEnumerable<FsNode>);
        }

        // ---------------------------------------------------------------------------------------- step 2: compare
        async Task Compare()
        {
            busy = true;
            WizardHint.Text = "بيقارن الملفات...";
            var items = staged.ToList();
            string dir = PatchDir, b = baseRoot;
            await Task.Run(() =>
            {
                var bl = new PatchCore.Baseline(b, dir);
                foreach (var it in items)
                    bl.Classify(it);
            });
            busy = false;
            ChipNew.Text = "جديد " + items.Count(i => i.Status == ItemStatus.New);
            ChipChanged.Text = "معدّل " + items.Count(i => i.Status == ItemStatus.Changed);
            ChipSame.Text = "مطابق " + items.Count(i => i.Status == ItemStatus.Same);
            ChipBad.Text = "مرفوض " + items.Count(i => i.Status == ItemStatus.Bad);
            CompareGrid.Items.Refresh();
        }

        // ---------------------------------------------------------------------------------------- step 4: review
        async Task BuildReview(int version)
        {
            busy = true;
            WizardHint.Text = "بيجهّز ملف التحديث...";
            if (builtZip != null) { try { File.Delete(builtZip); } catch { } builtZip = null; }
            var inc = staged.Where(s => s.Include).ToList();
            foreach (var s in inc.Where(s => s.Hash == null))
                s.Hash = PatchCore.Sha256File(s.Source);
            string zip = await Task.Run(() => PatchCore.Build(version, inc));
            builtZip = zip;
            long size = new FileInfo(zip).Length;
            string sha = PatchCore.Sha256File(zip);
            builtMeta = new PatchMeta
            {
                Version = version, Title = TitleBox.Text.Trim(), Notes = NotesBox.Text.Trim(), Created = DateTime.Now,
                Author = Environment.UserName, Sha256 = sha, Size = size, Kind = "ours",
                Files = inc.Select(s => new MetaFile { Path = s.Target, Size = s.Size, Sha256 = s.Hash, Status = s.StatusText }).ToList()
            };
            RevVersion.Text = version.ToString();
            RevFiles.Text = inc.Count.ToString();
            RevSize.Text = PatchCore.FormatSize(size);
            RevSha.Text = sha;
            RevList.ItemsSource = inc.Select(s => s.Target + "   (" + s.StatusText + ", " + s.SizeText + ")").Concat(new[] { "version.dat = " + version }).ToList();
            var checks = new List<CheckLine>
            {
                CheckLine.Of(true, "كل المسارات نسبية لمجلد الموارد ومفيهاش ..  أو مسار كامل"),
                CheckLine.Of(true, "الملف اتبنى واتفتح تاني واتقرأ صح (zip سليم)"),
                CheckLine.Of(true, "version.dat جوه الملف = " + version + " (زي اسم الملف)"),
                CheckLine.Of(true, "مفيش ملفات ممنوعة (إعدادات السيرفر، dll، exe، ...)"),
                CheckLine.Of(true, "رقم الإصدار أكبر من كل الأرقام المستخدمة قبل كده"),
                size > 50L * 1024 * 1024 ? CheckLine.Of(null, "الحجم " + PatchCore.FormatSize(size) + " أكبر من 50 MB: اللاعب على بيانات الموبايل هيتسأل قبل التنزيل")
                                         : CheckLine.Of(true, "الحجم " + PatchCore.FormatSize(size) + " (أقل من 50 MB، بينزل من غير سؤال)"),
                cfg.UpdateMode ? CheckLine.Of(true, "وضع التحديث Update: هيوصل للاعبين أول ما يفتحوا اللعبة")
                               : CheckLine.Of(null, "وضع التحديث Ready: مش هيوصل للاعبين إلا لما تشغل Update وتعيد تشغيل الـ AccServer"),
                PatchCore.IsListening(cfg.HttpPort) ? CheckLine.Of(true, "سيرفر التنزيل HTTP شغال على " + cfg.HttpPort)
                                                    : CheckLine.Of(null, "سيرفر التنزيل HTTP مش شغال على " + cfg.HttpPort)
            };
            if (builtMeta.Title.Length == 0 && builtMeta.Notes.Length == 0)
                checks.Add(CheckLine.Of(null, "مفيش ملاحظات للتحديث (اختياري)"));
            RevChecks.ItemsSource = checks;
            PublishProgress.Value = 0;
            PublishStatus.Text = "جاهز للنشر.";
            DonePanel.Visibility = Visibility.Collapsed;
            PublishPanel.Visibility = Visibility.Visible;
            busy = false;
        }

        async Task Publish()
        {
            if (builtZip == null || !File.Exists(builtZip)) { ShowStep(3); return; }
            builtMeta.Title = TitleBox.Text.Trim();
            builtMeta.Notes = NotesBox.Text.Trim();
            if (!Ask("نشر التحديث " + builtMeta.Version, "هيتنشر " + builtMeta.Files.Count + " ملف (" + PatchCore.FormatSize(builtMeta.Size) + ").\n" +
                     (cfg.UpdateMode ? "كل اللاعبين هينزلوه أول ما يفتحوا اللعبة." : "الوضع Ready: مش هيوصل للاعبين قبل تشغيل Update.") +
                     "\n\nمتأكد؟", "انشر", DialogKind.Question))
                return;
            busy = true;
            NextBtn.IsEnabled = BackBtn.IsEnabled = false;
            PublishStatus.Text = "بينسخ الملف لمجلد التحديثات...";
            var progress = new Progress<double>(v => PublishProgress.Value = v);
            try
            {
                string dest = await PatchCore.PublishAsync(builtZip, builtMeta, PatchDir, progress);
                builtZip = null;
                PublishProgress.Value = 1;
                OpsLog.Write("نشر", "نشر التحديث " + builtMeta.Version + " (" + builtMeta.Files.Count + " ملف، " + PatchCore.FormatSize(builtMeta.Size) + ")" +
                                    (builtMeta.Title.Length > 0 ? ": " + builtMeta.Title : ""));
                PublishPanel.Visibility = Visibility.Collapsed;
                DonePanel.Visibility = Visibility.Visible;
                DoneText.Text = "تم نشر التحديث " + builtMeta.Version;
                DoneSub.Text = (cfg.UpdateMode ? "السيرفر بيبعته من دلوقتي من غير إعادة تشغيل." : "التحديث جاهز، بس الوضع Ready.") + "\n" + dest;
                var pop = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseOut } };
                DoneScale.BeginAnimation(ScaleTransform.ScaleXProperty, pop);
                DoneScale.BeginAnimation(ScaleTransform.ScaleYProperty, pop);
                WizardNav.Visibility = Visibility.Collapsed;
                step = 5;
                PaintStepBar();
                RefreshAll();
                ShowToast("تم نشر " + builtMeta.Version, true);
            }
            finally
            {
                busy = false;
                NextBtn.IsEnabled = BackBtn.IsEnabled = true;
            }
        }

        // ================================================================================================ history
        void RefreshHistory()
        {
            var sel = (HistoryGrid.SelectedItem as PatchInfo)?.Version;
            var rows = patches.OrderByDescending(p => p.Version).ToList();
            HistoryGrid.ItemsSource = rows;
            HistoryGrid.SelectedItem = rows.FirstOrDefault(p => p.Version == sel) ?? rows.FirstOrDefault();
            ShowDetail(HistoryGrid.SelectedItem as PatchInfo);
        }

        void HistoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) => ShowDetail(HistoryGrid.SelectedItem as PatchInfo);

        void ShowDetail(PatchInfo p)
        {
            Detail.Visibility = p == null ? Visibility.Hidden : Visibility.Visible;
            if (p == null) return;
            DetVersion.Text = p.Version.ToString();
            DetTitle.Text = p.Title.Length > 0 ? p.Title : "(بدون عنوان)";
            DetMeta.Text = "النوع: " + p.KindText + (p.OfficialText != "-" ? " (الرقم الرسمي " + p.OfficialText + ")" : "") + "\nالتاريخ: " + p.DateText + "\nالحجم: " + p.SizeText + "     الملفات: " + p.FileCount +
                           "\nنزّله كامل: " + p.Downloads + " جهاز" +
                           (p.Meta != null && p.Meta.Author.Length > 0 ? "\nنشره: " + p.Meta.Author : "") + (p.Ok ? "" : "\nلن يرسل: " + p.Problem);
            DetNotes.Text = p.Meta != null && p.Meta.Notes.Length > 0 ? p.Meta.Notes : "لا توجد ملاحظات.";
            DetFiles.ItemsSource = p.Entries;
            DetSha.Text = p.Meta?.Sha256 ?? "";
            if (DetSha.Text.Length == 0)
                try { DetSha.Text = PatchCore.Sha256File(p.File); } catch { }
        }

        void Rollback_Click(object sender, RoutedEventArgs e)
        {
            var last = patches.LastOrDefault();
            if (last == null) { Fail("مفيش تحديثات منشورة."); return; }
            if (!Ask("رجوع: سحب التحديث " + last.Version,
                     "السيرفر هيبطل يبعت " + last.Version + " فورا (من غير إعادة تشغيل).\n\n" +
                     "• الملف مش هيتمسح، هيتنقل لـ _removed كنسخة احتياطية.\n" +
                     "• الأجهزة اللي نزلته (" + last.Downloads + ") هتفضل بملفاته. علشان ترجّع ملفاتهم القديمة انشر تحديث جديد بالنسخة القديمة.\n" +
                     "• الرقم " + last.Version + " مش هيتستخدم تاني، التحديث الجاي هياخد رقم أكبر.", "اسحب التحديث", DialogKind.Warning))
                return;
            RemovePatch(last, "رجوع");
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(HistoryGrid.SelectedItem is PatchInfo p)) { Fail("اختار تحديث من الجدول."); return; }
            bool isLast = patches.LastOrDefault()?.Version == p.Version;
            string msg = "حذف التحديث " + p.Version + " من السيرفر؟\n\n• هيتنقل لـ _removed كنسخة احتياطية (مش هيضيع).\n";
            if (!isLast)
                msg += "• تنبيه: ده مش آخر تحديث. اللاعبين اللي لسه منزلوش " + p.Version + " هيتخطوه ومش هيوصلهم ملفاته إلا لو تحديث تاني فيه نفس الملفات.\n";
            msg += "• الأجهزة اللي نزلته (" + p.Downloads + ") هتفضل بملفاته.";
            if (!Ask("حذف التحديث " + p.Version, msg, "احذف", DialogKind.Danger))
                return;
            RemovePatch(p, "حذف");
        }

        void RemovePatch(PatchInfo p, string op)
        {
            try
            {
                string dest = PatchCore.Remove(PatchDir, p.Version);
                OpsLog.Write(op, op + " التحديث " + p.Version + " (نسخة احتياطية: " + dest + ")");
                ShowToast("تم " + op + " التحديث " + p.Version, true);
            }
            catch (Exception ex) { Fail(ex.Message); }
            RefreshAll();
        }

        void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            if (PatchDir == null) return;
            Directory.CreateDirectory(PatchDir);
            Process.Start(new ProcessStartInfo("explorer.exe", "\"" + PatchDir + "\"") { UseShellExecute = true });
        }

        // ================================================================================================ official
        void RefreshOfficial()
        {
            var man = cfg != null ? OfficialManifest.Read(PatchDir) : null;
            OffCount.Text = patches.Count(p => p.IsOfficial).ToString();
            OffOurs.Text = patches.Count(p => !p.IsOfficial).ToString();
            OffLatest.Text = man != null && !string.IsNullOrEmpty(man.LatestOfficial) ? man.LatestOfficial : "-";
            OffChecked.Text = man?.CheckedAt != null ? PatchCore.Ago(man.CheckedAt.Value) : "-";
            var rows = man?.Patches ?? new List<OfficialEntry>();
            OfficialGrid.ItemsSource = rows.OrderByDescending(r => r.Ours).ToList();
            OffEmpty.Visibility = rows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            OfficialGrid.Visibility = rows.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            string path = PatchDir != null ? OfficialManifest.PathOf(PatchDir) : "_official\\manifest.json";
            OffEmptySub.Text = man?.Error != null ? "الملف موجود بس فيه خطأ: " + man.Error + "\n" + path
                                                  : "أول ما مرآة التحديثات الرسمية (patchmirror) تكتب الملف ده هتظهر الباتشات هنا:\n" + path;
        }

        // ================================================================================================ stats
        void StatsFilter_Click(object sender, RoutedEventArgs e) => RefreshStats();

        List<StatEvent> statRows = new List<StatEvent>();

        void RefreshStats()
        {
            statRows = cfg != null ? Stats.Read(PatchDir, StIncludeTests.IsChecked == true) : new List<StatEvent>();
            var gets = statRows.Where(e => e.Kind == "get").ToList();
            AnimateNumber(StOk, gets.Count(e => e.Result == "ok"));
            AnimateNumber(StFail, gets.Count(e => e.Result != "ok"));
            AnimateNumber(StDevices, statRows.Select(e => e.Device).Distinct().Count());
            AnimateNumber(StChecks, statRows.Count(e => e.Kind == "check" && e.Time.Date == DateTime.Today));
            StatsGrid.ItemsSource = statRows.OrderByDescending(e => e.Time).Take(1000).ToList();
            DrawChart();
        }

        void Chart_SizeChanged(object sender, SizeChangedEventArgs e) => DrawChart();

        /// <summary>Downloads per day for the last 14 days: stacked bars, ok at the base (gold) and failed on top (red).</summary>
        void DrawChart()
        {
            Chart.Children.Clear();
            double w = Chart.ActualWidth, h = Chart.ActualHeight;
            if (w < 100 || h < 60) return;
            var days = Enumerable.Range(0, 14).Select(i => DateTime.Today.AddDays(i - 13)).ToList();
            var gets = statRows.Where(e => e.Kind == "get").ToList();
            var data = days.Select(d => new
            {
                Day = d,
                Ok = gets.Count(e => e.Time.Date == d && e.Result == "ok"),
                Fail = gets.Count(e => e.Time.Date == d && e.Result != "ok")
            }).ToList();
            int max = Math.Max(1, data.Max(x => x.Ok + x.Fail));
            int stepY = max <= 4 ? 1 : (int)Math.Ceiling(max / 4.0);
            int top = stepY * (int)Math.Ceiling(max / (double)stepY);
            double left = 36, bottom = 26, plotH = h - bottom - 6, plotW = w - left - 4;
            var muted = (Brush)FindResource("Muted");
            var line = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));
            for (int v = 0; v <= top; v += stepY)
            {
                double y = 6 + plotH - plotH * v / top;
                Chart.Children.Add(new Line { X1 = left, X2 = w, Y1 = y, Y2 = y, Stroke = line, StrokeThickness = 1 });
                var t = new TextBlock { Text = v.ToString(), Foreground = muted, FontSize = 11 };
                Canvas.SetLeft(t, 0); Canvas.SetTop(t, y - 8);
                Chart.Children.Add(t);
            }
            double slot = plotW / days.Count, barW = Math.Min(34, slot * 0.6);
            var gold = (Brush)FindResource("Gold");
            var bad = (Brush)FindResource("Bad");
            for (int i = 0; i < data.Count; i++)
            {
                var d = data[i];
                double x = left + slot * i + (slot - barW) / 2;
                double hOk = plotH * d.Ok / top, hFail = plotH * d.Fail / top;
                double baseY = 6 + plotH;
                if (d.Ok > 0)
                {
                    var r = new Border { Width = barW, Height = Math.Max(2, hOk), Background = gold, CornerRadius = new CornerRadius(d.Fail > 0 ? 0 : 4, d.Fail > 0 ? 0 : 4, 0, 0) };
                    Canvas.SetLeft(r, x); Canvas.SetTop(r, baseY - Math.Max(2, hOk));
                    Chart.Children.Add(r);
                }
                if (d.Fail > 0)
                {
                    double gap = d.Ok > 0 ? 2 : 0;
                    var r = new Border { Width = barW, Height = Math.Max(2, hFail - gap), Background = bad, CornerRadius = new CornerRadius(4, 4, 0, 0) };
                    Canvas.SetLeft(r, x); Canvas.SetTop(r, baseY - hOk - Math.Max(2, hFail));
                    Chart.Children.Add(r);
                }
                var hit = new Border { Width = slot, Height = plotH + bottom, Background = Brushes.Transparent, Cursor = System.Windows.Input.Cursors.Hand,
                                       ToolTip = d.Day.ToString("yyyy-MM-dd") + "\nنجح: " + d.Ok + "\nفشل: " + d.Fail };
                Canvas.SetLeft(hit, left + slot * i); Canvas.SetTop(hit, 6);
                Chart.Children.Add(hit);
                var lbl = new TextBlock { Text = d.Day.ToString("MM-dd"), Foreground = d.Day == DateTime.Today ? (Brush)FindResource("Text") : muted, FontSize = 11, Width = slot, TextAlignment = TextAlignment.Center, IsHitTestVisible = false };
                Canvas.SetLeft(lbl, left + slot * i); Canvas.SetTop(lbl, h - bottom + 6);
                Chart.Children.Add(lbl);
            }
            if (gets.Count == 0)
            {
                var t = new TextBlock { Text = "لا توجد تنزيلات مسجلة في الفترة دي", Foreground = muted, FontSize = 14 };
                t.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(t, left + (plotW - t.DesiredSize.Width) / 2); Canvas.SetTop(t, plotH / 2 - 8);
                Chart.Children.Add(t);
            }
        }

        // ================================================================================================ settings page
        bool settingMode;

        void RefreshSettings()
        {
            ConfigBox.Text = configPath ?? "(غير موجود)";
            BaseBox.Text = baseRoot ?? "(غير محدد)";
            if (cfg == null) return;
            settingMode = true;
            ModeUpdate.IsChecked = cfg.UpdateMode;
            ModeReady.IsChecked = !cfg.UpdateMode;
            settingMode = false;
            CheckPortBox.Text = cfg.CheckPort.ToString();
            HttpPortBox.Text = cfg.HttpPort.ToString();
            UrlBox.Text = "http://" + cfg.UrlBase + "/NNNN.zip";
            PatchDirBox.Text = PatchDir;
            var probs = cfg.PortProblems(cfg.CheckPort, cfg.HttpPort);
            PortProblems.Text = string.Join("\n", probs);
        }

        void Mode_Click(object sender, RoutedEventArgs e)
        {
            if (settingMode || cfg == null) return;
            string to = sender == ModeUpdate ? "Update" : "Ready";
            if (string.Equals(cfg.Mode, to, StringComparison.OrdinalIgnoreCase)) return;
            string msg = to == "Update" ? "الموبايلات هتنزل كل التحديثات المنشورة أول ما تفتح اللعبة." : "الموبايلات هتاخد READY ومش هتنزل أي تحديث.";
            if (!Ask("تغيير الوضع إلى " + to, msg + "\n\nالتغيير بيتكتب في ServerConfig.ini (مع نسخة احتياطية .bak) وبيشتغل بعد إعادة تشغيل الـ AccServer، وإعادة التشغيل بتخرج اللاعبين.", "غيّر", DialogKind.Warning))
            {
                RefreshSettings();
                return;
            }
            try
            {
                ServerConfig.SetKey(configPath, "AutoPatch", "Mode", to);
                OpsLog.Write("إعدادات", "وضع التحديث بقى " + to + " (محتاج إعادة تشغيل الـ AccServer)");
                ShowToast("اتحفظ. أعد تشغيل الـ AccServer علشان يشتغل", true);
            }
            catch (Exception ex) { Fail(ex.Message); }
            RefreshAll();
        }

        void SavePorts_Click(object sender, RoutedEventArgs e)
        {
            if (cfg == null) return;
            if (!int.TryParse(CheckPortBox.Text.Trim(), out int c) || !int.TryParse(HttpPortBox.Text.Trim(), out int h)) { Fail("اكتب أرقام البورتات صح."); return; }
            var probs = cfg.PortProblems(c, h);
            if (probs.Count > 0) { Fail(string.Join("\n", probs)); return; }
            if (c == cfg.CheckPort && h == cfg.HttpPort) { ShowToast("مفيش تغيير", true); return; }
            if (!Ask("حفظ البورتات", "فحص الإصدار: " + c + "\nتحميل HTTP: " + h + "\n\n" +
                     (c != cfg.CheckPort ? "تنبيه: بورت الفحص مكتوب جوه الـ APK (autoupdate.ini)، لو غيرته الموبايلات القديمة مش هتلاقي السيرفر.\n" : "") +
                     "بيشتغل بعد إعادة تشغيل الـ AccServer، وافتح البورت في الفايروول.", "احفظ", DialogKind.Warning))
                return;
            try
            {
                if (c != cfg.CheckPort) ServerConfig.SetKey(configPath, "Server", "AutoPatchPort", c.ToString());
                if (h != cfg.HttpPort) ServerConfig.SetKey(configPath, "AutoPatch", "HttpPort", h.ToString());
                OpsLog.Write("إعدادات", "البورتات: فحص " + c + "، HTTP " + h + " (محتاج إعادة تشغيل الـ AccServer)");
                ShowToast("اتحفظ. أعد تشغيل الـ AccServer علشان يشتغل", true);
            }
            catch (Exception ex) { Fail(ex.Message); }
            RefreshAll();
        }

        void PickConfig_Click(object sender, RoutedEventArgs e)
        {
            var d = new Microsoft.Win32.OpenFileDialog { Filter = "ServerConfig.ini|ServerConfig.ini|ini|*.ini", Title = "اختار ServerConfig.ini" };
            if (d.ShowDialog(this) != true) return;
            configPath = d.FileName;
            SaveSettings();
            RefreshAll();
        }

        void PickPatchDir_Click(object sender, RoutedEventArgs e)
        {
            if (cfg == null) return;
            var d = new Microsoft.Win32.OpenFolderDialog { Title = "مجلد التحديثات (PatchDir)" };
            if (Directory.Exists(PatchDir)) d.InitialDirectory = PatchDir;
            if (d.ShowDialog(this) != true || string.Equals(d.FolderName, PatchDir, StringComparison.OrdinalIgnoreCase)) return;
            if (!Ask("تغيير مجلد التحديثات", "المجلد الجديد: " + d.FolderName + "\n\nالتحديثات اللي في المجلد القديم مش هتتنقل لوحدها. بيشتغل بعد إعادة تشغيل الـ AccServer.", "غيّر", DialogKind.Warning))
                return;
            try
            {
                ServerConfig.SetKey(configPath, "AutoPatch", "PatchDir", d.FolderName);
                OpsLog.Write("إعدادات", "مجلد التحديثات بقى " + d.FolderName + " (محتاج إعادة تشغيل الـ AccServer)");
            }
            catch (Exception ex) { Fail(ex.Message); }
            RefreshAll();
        }

        void PickBase_Click(object sender, RoutedEventArgs e)
        {
            var d = new Microsoft.Win32.OpenFolderDialog { Title = "النسخة الأساسية (ConquerRes من الـ APK)" };
            if (baseRoot != null) d.InitialDirectory = baseRoot;
            if (d.ShowDialog(this) != true) return;
            baseRoot = d.FolderName;
            nameIndex = null;
            SaveSettings();
            RefreshSettings();
        }

        async void ConnTest_Click(object sender, RoutedEventArgs e)
        {
            if (cfg == null) return;
            testLines.Clear();
            testLines.Add(CheckLine.Of(null, "اختبار الاتصال..."));
            var lines = new List<CheckLine>();
            foreach (var p in new[] { new { P = cfg.CheckPort, N = "فحص الإصدار" }, new { P = cfg.HttpPort, N = "تحميل HTTP" } })
            {
                bool listen = PatchCore.IsListening(p.P);
                bool conn = await PatchCore.CanConnect(p.P);
                lines.Add(CheckLine.Of(listen && conn, p.N + " " + p.P + ": " + (listen ? "مفتوح" : "مقفول") + (conn ? "، الاتصال نجح" : "، الاتصال فشل")));
            }
            try
            {
                var chain = await PatchCore.AskChain(cfg.CheckPort, PatchCore.BaseVersion);
                lines.Add(CheckLine.Of(true, "السيرفر رد على 5150 بـ: " + chain.FirstOrDefault().reply));
            }
            catch (Exception ex) { lines.Add(CheckLine.Of(false, ex.Message)); }
            lines.Add(CheckLine.Of(cfg.UpdateMode ? true : (bool?)null, "الوضع: " + cfg.Mode));
            testLines.Clear();
            foreach (var l in lines) testLines.Add(l);
        }

        async void FullTest_Click(object sender, RoutedEventArgs e)
        {
            if (cfg == null || busy) return;
            testLines.Clear();
            FullTestBtn.IsEnabled = false;
            testLines.Add(CheckLine.Of(null, "بيجرب زي موبايل نسخته 5150..."));
            bool ok = await PatchCore.SimulateClient(cfg.CheckPort, PatchDir,
                (t, good) => Dispatcher.Invoke(() => testLines.Add(CheckLine.Of(good, t))));
            testLines.RemoveAt(0);
            testLines.Add(CheckLine.Of(ok, ok ? "الاختبار نجح: الموبايل هيوصل لآخر إصدار" : "الاختبار فشل: راجع السطور الحمرا"));
            OpsLog.Write("اختبار", "اختبار تحديث كامل: " + (ok ? "نجح" : "فشل"));
            FullTestBtn.IsEnabled = true;
            RefreshStats();
        }

        void OpenLog_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(LogPath.Text))
                Process.Start(new ProcessStartInfo("notepad.exe", "\"" + LogPath.Text + "\"") { UseShellExecute = true });
        }
    }
}
