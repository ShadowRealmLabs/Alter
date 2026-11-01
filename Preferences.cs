using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

class AlterPreferences
{
    public string Accent = "Amethyst";
    public bool CompactLayout = false;
    public bool ShowNews = true;
    public bool ConfirmWrites = true;
    public bool ConfirmRemovals = true;
    public bool RememberWindowSize = true;
    public bool MinimizeToTray = false;
    public bool CloseToTray = false;
    public string StartupPage = "Home";
    public double WindowWidth = 1280;
    public double WindowHeight = 853;
}

partial class Alter
{
    AlterPreferences prefs = new AlterPreferences();
    string PreferencePath { get { return Path.Combine(root, "settings.json"); } }
    Color AccentColor { get { return prefs.Accent == "Ocean" ? Color.FromRgb(40, 125, 231) : prefs.Accent == "Rose" ? Color.FromRgb(220, 72, 135) : Color.FromRgb(88, 29, 156); } }
    string AccentHex { get { return prefs.Accent == "Ocean" ? "#287DE7" : prefs.Accent == "Rose" ? "#DC4887" : "#581D9C"; } }
    string AccentSoftHex { get { return prefs.Accent == "Ocean" ? "#91BDF6" : prefs.Accent == "Rose" ? "#F0A1C4" : "#AD77DC"; } }
    void LoadPreferences() { try { if (File.Exists(PreferencePath)) { var loaded = new JavaScriptSerializer().Deserialize<AlterPreferences>(File.ReadAllText(PreferencePath)); if (loaded != null) prefs = loaded; } } catch { prefs = new AlterPreferences(); } if (prefs.Accent != "Amethyst" && prefs.Accent != "Ocean" && prefs.Accent != "Rose") prefs.Accent = "Amethyst"; if (prefs.StartupPage != "Home" && prefs.StartupPage != "Library") prefs.StartupPage = "Home"; if (prefs.RememberWindowSize) { Width = Math.Max(MinWidth, Math.Min(2600, prefs.WindowWidth)); Height = Math.Max(MinHeight, Math.Min(1600, prefs.WindowHeight)); } ApplyDensity(); }
    void SavePreferences() { File.WriteAllText(PreferencePath, new JavaScriptSerializer().Serialize(prefs)); }
    void SaveWindowPreference() { if (!prefs.RememberWindowSize) return; Rect bounds = WindowState == WindowState.Normal ? new Rect(Left, Top, Width, Height) : RestoreBounds; prefs.WindowWidth = bounds.Width; prefs.WindowHeight = bounds.Height; SavePreferences(); }
    void ApplyDensity() { foreach (var b in navButtons.Values) { b.Height = prefs.CompactLayout ? 36 : 42; b.Margin = new Thickness(16, prefs.CompactLayout ? 2 : 4, 16, prefs.CompactLayout ? 2 : 4); } }
    bool AllowWrite(string feature) { return !prefs.ConfirmWrites || MessageBox.Show("Write to the running game's memory for " + feature + "?", "Alter", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes; }
    UIElement SettingRow(string title, string detail, bool value, Action<bool> set)
    {
        var row = new Grid { MinHeight = 72, Margin = new Thickness(0, 2, 0, 2) }; row.ColumnDefinitions.Add(new ColumnDefinition()); row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(78) });
        var words = new StackPanel { VerticalAlignment = VerticalAlignment.Center }; words.Children.Add(Copy(title, 13, "#EFE9F7")); var desc = Copy(detail, 11, "#8E829F"); desc.Margin = new Thickness(0, 3, 0, 0); words.Children.Add(desc); row.Children.Add(words);
        var button = new Button { Content = value ? "On" : "Off", Width = 64, Height = 31, MinWidth = 0, Padding = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Foreground = Brush("#FFFFFF"), Background = Brush(value ? AccentHex : "#262036"), BorderBrush = Brush(value ? AccentHex : "#3A304E") }; Grid.SetColumn(button, 1); row.Children.Add(button);
        button.Click += (s, e) => { value = !value; set(value); SavePreferences(); button.Content = value ? "On" : "Off"; button.Background = Brush(value ? AccentHex : "#262036"); button.BorderBrush = Brush(value ? AccentHex : "#3A304E"); if (title == "Compact layout") ApplyDensity(); }; return row;
    }
    void Settings()
    {
        SelectNav("settings"); body.Children.Clear(); var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled }; body.Children.Add(scroll); var page = new StackPanel { Margin = new Thickness(28, 25, 28, 32) }; scroll.Content = page;
        page.Children.Add(Copy("Settings", 27, "#F3EDF9")); var intro = Copy("Make Alter feel like yours. Preferences are saved on this PC.", 12, "#9D91AE"); intro.Margin = new Thickness(0, 4, 0, 22); page.Children.Add(intro);
        var columns = new Grid(); columns.ColumnDefinitions.Add(new ColumnDefinition()); columns.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) }); columns.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(330) }); page.Children.Add(columns);
        var left = new StackPanel(); columns.Children.Add(left); var appearance = new StackPanel(); appearance.Children.Add(Copy("Appearance", 16, "#F0E8F8")); appearance.Children.Add(Copy("Accent color", 12, "#A89AB9")); var accents = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 13) }; foreach (string name in new[] { "Amethyst", "Ocean", "Rose" }) { string item = name; string hex = name == "Ocean" ? "#287DE7" : name == "Rose" ? "#DC4887" : "#581D9C"; var choice = new Button { Content = name, MinWidth = 96, Height = 35, Margin = new Thickness(0, 0, 9, 0), Padding = new Thickness(9, 0, 9, 0), Background = Brush(prefs.Accent == name ? hex : "#211B32"), BorderBrush = Brush(prefs.Accent == name ? "#D7C2F5" : "#3A304E"), Foreground = Brush("#FFFFFF") }; choice.Click += (s, e) => { prefs.Accent = item; SavePreferences(); Settings(); }; accents.Children.Add(choice); }
        appearance.Children.Add(accents);
        appearance.Children.Add(SettingRow("Compact layout", "Tighter navigation and Home spacing.", prefs.CompactLayout, v => prefs.CompactLayout = v)); left.Children.Add(Surface(appearance, new Thickness(20)));
        var privacy = new StackPanel(); privacy.Children.Add(Copy("Home & privacy", 16, "#F0E8F8")); privacy.Children.Add(SettingRow("Gaming news", "Show live headlines on Home.", prefs.ShowNews, v => prefs.ShowNews = v)); privacy.Children.Add(SettingRow("Remember window size", "Restore this window's size next time.", prefs.RememberWindowSize, v => prefs.RememberWindowSize = v)); var privacyCard = Surface(privacy, new Thickness(20)); privacyCard.Margin = new Thickness(0, 16, 0, 0); left.Children.Add(privacyCard);
        var window = new StackPanel(); window.Children.Add(Copy("Window behavior", 16, "#F0E8F8")); window.Children.Add(SettingRow("Minimize to tray", "Keep Alter active when minimized.", prefs.MinimizeToTray, v => prefs.MinimizeToTray = v)); window.Children.Add(SettingRow("Close to tray", "Keep Alter active when you close its window.", prefs.CloseToTray, v => prefs.CloseToTray = v)); window.Children.Add(Copy("Use Exit Alter from the tray icon to quit fully.", 11, "#9082A3")); var windowCard = Surface(window, new Thickness(20)); windowCard.Margin = new Thickness(0, 16, 0, 0); left.Children.Add(windowCard);
        var right = new StackPanel(); Grid.SetColumn(right, 2); columns.Children.Add(right); var launch = new StackPanel(); launch.Children.Add(Copy("Startup", 16, "#F0E8F8")); launch.Children.Add(Copy("Open Alter on", 11, "#9D91AE")); var starts = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 2) }; foreach (string name in new[] { "Home", "Library" }) { string item = name; var b = new Button { Content = name, Width = 110, Height = 34, MinWidth = 0, Margin = new Thickness(0, 0, 9, 0), Padding = new Thickness(0), Background = Brush(prefs.StartupPage == name ? AccentHex : "#211B32"), BorderBrush = Brush(prefs.StartupPage == name ? AccentHex : "#3A304E"), Foreground = Brush("#FFFFFF") }; b.Click += (s, e) => { prefs.StartupPage = item; SavePreferences(); Settings(); }; starts.Children.Add(b); }
        launch.Children.Add(starts); right.Children.Add(Surface(launch, new Thickness(18)));
        var safety = new StackPanel(); safety.Children.Add(Copy("Trainer controls", 16, "#F0E8F8")); safety.Children.Add(SettingRow("Confirm memory writes", "Ask before setting or freezing a value.", prefs.ConfirmWrites, v => prefs.ConfirmWrites = v)); safety.Children.Add(SettingRow("Confirm removal", "Ask before removing a local trainer.", prefs.ConfirmRemovals, v => prefs.ConfirmRemovals = v)); var safetyCard = Surface(safety, new Thickness(18)); safetyCard.Margin = new Thickness(0, 16, 0, 0); right.Children.Add(safetyCard); right.Children.Add(UpdateSettingsCard());
        var storage = new StackPanel(); storage.Children.Add(Copy("Local data", 16, "#F0E8F8")); storage.Children.Add(Copy("Trainers and settings stay in this Alter folder.", 11, "#9D91AE")); var open = Button("Open profiles folder", () => System.Diagnostics.Process.Start(Path.Combine(root, "profiles")), false); storage.Children.Add(open); var reset = Button("Reset preferences", () => { if (MessageBox.Show("Restore default settings?", "Alter", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return; prefs = new AlterPreferences(); Width = 1280; Height = 853; ApplyDensity(); SavePreferences(); Settings(); }, false); storage.Children.Add(reset); var storageCard = Surface(storage, new Thickness(18)); storageCard.Margin = new Thickness(0, 16, 0, 0); right.Children.Add(storageCard);
    }
}
