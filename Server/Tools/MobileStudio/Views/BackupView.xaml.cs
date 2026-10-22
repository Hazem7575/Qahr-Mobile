using System.IO;
using System.Windows;
using System.Windows.Controls;
using MobileStudio.Core;
using ScriptHazem.Kit;

namespace MobileStudio.Views;

public partial class BackupView : UserControl, IPage
{
    private bool _busy;
    private CancellationTokenSource _cts;

    public BackupView()
    {
        InitializeComponent();
        PatchesBox.IsChecked = Svc.Settings.BackupIncludePatches;
        OtherPath.Text = Path.Combine(Svc.Settings.BackupDir, "restore_check");
        UpdateHints();
    }

    public void OnShown() => Reload();
    public void OnHidden() { }
    public void OnTick(int tick) { }
    public void OnStatus(ServerSnapshot s) { }
    public void OnLanguage() { UpdateHints(); Reload(); }
    public Task PrepareScreenshotAsync() { Reload(); if (List.Items.Count > 0) List.SelectedIndex = 0; return Task.CompletedTask; }

    private void UpdateHints()
    {
        DbHint.Text = BackupOps.DumpExe != null ? Loc.FB("bk.dbHint", BackupOps.DbName, Path.GetFileName(BackupOps.DumpExe)) : Loc.FB("bk.noDump", Svc.Settings.MysqlBin);
        DataHint.Text = Loc.F("bk.dataHint", string.Join(", ", BackupOps.DataExcludes(PatchesBox.IsChecked == true)));
        SubText.Text = Loc.FB("bk.sub", Svc.Settings.BackupDir, Svc.Settings.BackupKeep);
    }

    private void Patches_Changed(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded) return;
        Svc.Settings.BackupIncludePatches = PatchesBox.IsChecked == true;
        Svc.SaveSettings();
        UpdateHints();
    }

    private void Reload()
    {
        var l = BackupOps.List();
        var sel = (List.SelectedItem as BackupInfo)?.File;
        List.ItemsSource = l;
        List.SelectedItem = l.FirstOrDefault(b => b.File == sel);
        Empty.Visibility = l.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void List_Selected(object sender, SelectionChangedEventArgs e)
    {
        if (List.SelectedItem is not BackupInfo b) { SelText.Text = Loc.Get("bk.pick"); Entries.ItemsSource = null; return; }
        SelText.Text = Loc.FB("bk.selected", b.Name, b.ContentText);
        VerifyText.Text = "";
        VerifyText.Foreground = Brushes2.Faint;
        RestoreDbBtn.IsEnabled = b.HasDb && !_busy;
        RestoreDataBtn.IsEnabled = b.DataFiles > 0 && !_busy;
        _ = Task.Run(() => BackupOps.Entries(b.File)).ContinueWith(t => Entries.ItemsSource = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(Svc.Settings.BackupDir);
        Ui.OpenFolder(Svc.Settings.BackupDir);
    }

    private void PickTarget_Click(object sender, RoutedEventArgs e)
    {
        var d = new Microsoft.Win32.OpenFolderDialog();
        if (d.ShowDialog() == true) OtherPath.Text = d.FolderName;
    }

    private void SetBusy(bool b)
    {
        _busy = b;
        BackupBtn.IsEnabled = !b;
        RestoreDbBtn.IsEnabled = RestoreDataBtn.IsEnabled = VerifyBtn.IsEnabled = !b && List.SelectedItem != null;
        Busy.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
        Busy.IsIndeterminate = b;
        if (b) MainWindow.BusyJobs++; else MainWindow.BusyJobs--;
    }

    private void Step(string s) => Dispatcher.BeginInvoke(() => StepText.Text = s);

    private async void Backup_Click(object sender, RoutedEventArgs e)
    {
        bool db = DbBox.IsChecked == true, data = DataBox.IsChecked == true;
        if (!db && !data) return;
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            var b = await BackupOps.CreateAsync(db, data, db && data ? "full" : db ? "db" : "data", NoteBox.Text.Trim(), Step, _cts.Token);
            StepText.Text = Loc.FB("bk.done", b.Name, b.SizeText);
            MainWindow.Toast_(StepText.Text);
            NoteBox.Clear();
        }
        catch (Exception ex) { StepText.Text = ex.Message; MainWindow.Toast_(ex.Message, true); }
        finally { SetBusy(false); Reload(); }
    }

    private async void Verify_Click(object sender, RoutedEventArgs e)
    {
        if (List.SelectedItem is not BackupInfo b) return;
        SetBusy(true);
        VerifyText.Text = Loc.Get("bk.verifying");
        _cts = new CancellationTokenSource();
        try
        {
            var (ok, text) = await BackupOps.VerifyAsync(b.File, _cts.Token);
            VerifyText.Text = (ok ? "✓ " : "✗ ") + text;
            VerifyText.Foreground = ok ? Brushes2.Ok : Brushes2.Err;
            ActionLog.Write("verify-backup", $"{b.Name} ok={ok} {text}");
        }
        catch (Exception ex) { VerifyText.Text = ex.Message; }
        finally { SetBusy(false); }
    }

    private async void RestoreDb_Click(object sender, RoutedEventArgs e)
    {
        if (List.SelectedItem is not BackupInfo b || !b.HasDb) return;
        bool live = DbLive.IsChecked == true;
        string target = live ? BackupOps.DbName : CopyName.Text.Trim();
        if (!System.Text.RegularExpressions.Regex.IsMatch(target, @"^[A-Za-z0-9_]+$")) { MainWindow.Toast_(Loc.Get("bk.badDbName"), true); return; }
        if (!live && target.Equals(BackupOps.DbName, StringComparison.OrdinalIgnoreCase)) { MainWindow.Toast_(Loc.Get("bk.copyIsLive"), true); return; }
        var s = Svc.Monitor.Last;
        if (live && (s.GameRunning || s.AccRunning))
        {
            ConfirmDialog.Info(Loc.Get("bk.refuseTitle"), Loc.Get("bk.refuseDb"));
            return;
        }
        var steps = live
            ? new[] { Loc.Get("bk.step.preDb"), Loc.FB("bk.step.import", b.Name, target), Loc.Get("bk.step.replace") }
            : new[] { Loc.FB("bk.step.import", b.Name, target), Loc.Get("bk.step.copyOnly") };
        if (!ConfirmDialog.Ask(Loc.Get(live ? "bk.restoreDbLiveTitle" : "bk.restoreDbCopyTitle"), Loc.FB("bk.restoreDbMsg", b.CreatedText), steps,
                Loc.Get("bk.restoreDb"), live, typeWord: live ? BackupOps.DbName : null))
            return;
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            if (live)
            {
                Step(Loc.Get("bk.step.preDb"));
                await BackupOps.CreateAsync(true, false, "pre-restore", Loc.FB("bk.preNote", b.Name), Step, _cts.Token);
            }
            await BackupOps.RestoreDbAsync(b.File, target, Step, _cts.Token);
            string tables = await BackupOps.QueryAsync($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='{target}'");
            StepText.Text = Loc.FB("bk.dbRestored", target, tables ?? "?");
            MainWindow.Toast_(StepText.Text);
        }
        catch (Exception ex) { StepText.Text = ex.Message; MainWindow.Toast_(ex.Message, true); }
        finally { SetBusy(false); Reload(); }
    }

    private async void RestoreData_Click(object sender, RoutedEventArgs e)
    {
        if (List.SelectedItem is not BackupInfo b || b.DataFiles == 0) return;
        bool live = DataLive.IsChecked == true;
        string target = live ? Svc.Settings.DataDir : OtherPath.Text.Trim();
        if (string.IsNullOrWhiteSpace(target)) return;
        if (!live && Path.GetFullPath(target).TrimEnd('\\').Equals(Path.GetFullPath(Svc.Settings.DataDir).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)) live = true;
        var s = Svc.Monitor.Last;
        if (live && s.GameRunning)
        {
            ConfirmDialog.Info(Loc.Get("bk.refuseTitle"), Loc.Get("bk.refuseData"));
            return;
        }
        var steps = live
            ? new[] { Loc.Get("bk.step.preData"), Loc.FB("bk.step.extract", b.DataFiles, target), Loc.Get("bk.step.keepOthers") }
            : new[] { Loc.FB("bk.step.extract", b.DataFiles, target), Loc.Get("bk.step.copyOnly") };
        if (!ConfirmDialog.Ask(Loc.Get(live ? "bk.restoreDataLiveTitle" : "bk.restoreDataCopyTitle"), Loc.FB("bk.restoreDataMsg", b.CreatedText), steps,
                Loc.Get("bk.restoreData"), live, typeWord: live ? "Data" : null))
            return;
        SetBusy(true);
        _cts = new CancellationTokenSource();
        try
        {
            if (live) await BackupOps.CreateAsync(false, true, "pre-restore", Loc.FB("bk.preNote", b.Name), Step, _cts.Token);
            Directory.CreateDirectory(target);
            int n = await BackupOps.RestoreDataAsync(b.File, target, Step, _cts.Token);
            StepText.Text = Loc.FB("bk.dataRestored", n, target);
            MainWindow.Toast_(StepText.Text);
        }
        catch (Exception ex) { StepText.Text = ex.Message; MainWindow.Toast_(ex.Message, true); }
        finally { SetBusy(false); Reload(); }
    }
}
