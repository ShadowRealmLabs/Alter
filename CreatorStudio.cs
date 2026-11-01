using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

partial class Alter
{
    sealed class ActiveFeature { public Feature Feature; public ScriptRuntime Runtime; public double Value; }
    readonly Dictionary<string, ActiveFeature> activeFeatures = new Dictionary<string, ActiveFeature>();
    string activeFeatureProfile;
    bool ScriptActive(Feature feature) { return feature.id != null && activeFeatures.ContainsKey(feature.id); }
    void StopScripts() { foreach (var item in activeFeatures.Values.ToList()) try { item.Runtime.Off(item.Value); } catch { } activeFeatures.Clear(); activeFeatureProfile = null; }
    void TickScripts() { if (selected == null || session == null || !session.Connected || activeFeatureProfile != selected.id) return; foreach (var item in activeFeatures.Values.ToList()) try { item.Runtime.Tick(item.Value); } catch { try { item.Runtime.Off(item.Value); } catch { } activeFeatures.Remove(item.Feature.id); } }
    void RunScript(Feature feature)
    {
        try
        {
            EnsureSession(); if (String.IsNullOrWhiteSpace(feature.id)) feature.id = Guid.NewGuid().ToString("N");
            ActiveFeature active; if (activeFeatures.TryGetValue(feature.id, out active)) { active.Runtime.Off(active.Value); activeFeatures.Remove(feature.id); Trainer(); return; }
            if (!feature.trusted) { if (MessageBox.Show("This imported feature contains C# code that runs with your Windows account. Run it only if you trust its author.\n\nRun " + feature.name + "?", "Run custom feature", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return; feature.trusted = true; SaveProfile(selected); }
            if (!AllowWrite(feature.name)) return; double value = Double.Parse(String.IsNullOrWhiteSpace(feature.value) ? "1" : feature.value, CultureInfo.InvariantCulture); var runtime = new ScriptRuntime(session, feature); try { runtime.On(value); } catch { try { runtime.Off(value); } catch { } throw; }
            if (feature.scriptMode == "action") { runtime.Off(value); } else { activeFeatureProfile = selected.id; activeFeatures.Add(feature.id, new ActiveFeature { Feature = feature, Runtime = runtime, Value = value }); }
            Trainer();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Custom feature"); }
    }
    Grid ScriptFeatureRow(Feature feature) { var row = new Grid { Height = 58, Margin = new Thickness(0, 5, 0, 0) }; row.ColumnDefinitions.Add(new ColumnDefinition()); row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) }); row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) }); var label = new StackPanel { VerticalAlignment = VerticalAlignment.Center }; label.Children.Add(Copy(feature.name ?? "Custom feature", 13, "#EEE7F7")); label.Children.Add(Copy(feature.scriptMode == "action" ? "Action · custom logic" : "Toggle · custom logic", 10, "#8B809D")); row.Children.Add(label); var run = Button(feature.scriptMode == "action" ? "Apply" : ScriptActive(feature) ? "On" : "Off", () => RunScript(feature), ScriptActive(feature)); run.Height = 30; run.MinWidth = 0; run.Margin = new Thickness(3); run.Padding = new Thickness(0); run.HorizontalAlignment = HorizontalAlignment.Stretch; run.HorizontalContentAlignment = HorizontalAlignment.Center; Grid.SetColumn(run, 1); row.Children.Add(run); var edit = Button("Edit", () => ScriptEditor(feature), false); edit.Height = 30; edit.MinWidth = 0; edit.Margin = new Thickness(3); edit.Padding = new Thickness(0); edit.HorizontalAlignment = HorizontalAlignment.Stretch; edit.HorizontalContentAlignment = HorizontalAlignment.Center; Grid.SetColumn(edit, 2); row.Children.Add(edit); return row; }
    void ScriptEditor(Feature existing)
    {
        if (selected == null) return; var w = new Window { Title = existing == null ? "Create custom feature · Alter" : "Edit custom feature · Alter", Width = 740, Height = 750, MinWidth = 650, MinHeight = 600, Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner, Background = Brush("#0B0A1B"), Foreground = Ink }; StyleDialog(w); var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto }; w.Content = scroll; var p = new StackPanel { Margin = new Thickness(24) }; scroll.Content = p;
        p.Children.Add(Copy("CUSTOM FEATURE", 13, "#B68BEF")); p.Children.Add(Copy("Write game-specific C# using the api object. Code runs locally when you click Apply or toggle On.", 11, "#A99CB9"));
        p.Children.Add(Copy("Name", 11, "#D7CBEB")); var name = new TextBox { Text = existing == null ? "" : existing.name }; p.Children.Add(name);
        p.Children.Add(Copy("Category", 11, "#D7CBEB")); var category = new TextBox { Text = existing == null ? "Player" : existing.category }; p.Children.Add(category);
        p.Children.Add(Copy("Behavior", 11, "#D7CBEB")); var mode = new ComboBox { ItemsSource = new[] { "toggle", "action" }, SelectedItem = existing == null ? "toggle" : existing.scriptMode == "action" ? "action" : "toggle", Height = 30, Background = Brush("#FFFFFF"), Foreground = Brush("#0B0A1B") }; p.Children.Add(mode);
        p.Children.Add(Copy("Value supplied to code (double)", 11, "#D7CBEB")); var value = new TextBox { Text = existing == null ? "1" : String.IsNullOrWhiteSpace(existing.value) ? "1" : existing.value }; p.Children.Add(value);
        p.Children.Add(Copy("On / Apply code", 11, "#D7CBEB")); var on = CodeBox(existing == null ? "// Example: api.WriteFloat(\"Game.exe+0x1234\", (float)value);" : existing.scriptOn); p.Children.Add(on);
        p.Children.Add(Copy("Off code (toggle only)", 11, "#D7CBEB")); var off = CodeBox(existing == null ? "// Restores patches automatically after this code runs." : existing.scriptOff); p.Children.Add(off);
        p.Children.Add(Copy("While On code (runs about every 300 ms)", 11, "#D7CBEB")); var tick = CodeBox(existing == null ? "// Example: api.WriteFloat(\"Game.exe+0x1234\", (float)value);" : existing.scriptTick); p.Children.Add(tick);
        var note = Copy("Custom C# has your Windows account's permissions. Imported code stays off until you explicitly run it.", 11, "#A99CB9"); note.Margin = new Thickness(0, 12, 0, 0); p.Children.Add(note);
        Func<Feature> makeDraft = () => { double.Parse(value.Text, CultureInfo.InvariantCulture); return new Feature { id = existing == null ? Guid.NewGuid().ToString("N") : existing.id, name = name.Text.Trim(), category = category.Text.Trim(), kind = "script", scriptMode = (string)mode.SelectedItem, value = value.Text.Trim(), scriptOn = on.Text, scriptOff = off.Text, scriptTick = tick.Text, trusted = true }; };
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 14, 0, 0) }; var validate = Button("Check code", () => { try { ScriptRuntime.Validate(makeDraft()); MessageBox.Show("Code compiled successfully.", "Alter"); } catch (Exception ex) { MessageBox.Show(ex.Message, "Code error"); } }, false); actions.Children.Add(validate);
        var save = Button("Save feature", () => { try { if (String.IsNullOrWhiteSpace(name.Text)) throw new FormatException("Enter a feature name."); var draft = makeDraft(); ScriptRuntime.Validate(draft); if (existing != null && ScriptActive(existing)) { var active = activeFeatures[existing.id]; active.Runtime.Off(active.Value); activeFeatures.Remove(existing.id); } if (existing == null) selected.cheats.Add(draft); else { existing.name = draft.name; existing.category = draft.category; existing.kind = "script"; existing.scriptMode = draft.scriptMode; existing.value = draft.value; existing.scriptOn = draft.scriptOn; existing.scriptOff = draft.scriptOff; existing.scriptTick = draft.scriptTick; existing.trusted = true; } SaveProfile(selected); w.Close(); Trainer(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Cannot save feature"); } }, true); save.Margin = new Thickness(10, 12, 0, 0); actions.Children.Add(save);
        if (existing != null) { var remove = Button("Remove", () => { if (ScriptActive(existing)) { var active = activeFeatures[existing.id]; active.Runtime.Off(active.Value); activeFeatures.Remove(existing.id); } selected.cheats.Remove(existing); SaveProfile(selected); w.Close(); Trainer(); }, false); remove.Margin = new Thickness(10, 12, 0, 0); actions.Children.Add(remove); }
        p.Children.Add(actions); w.ShowDialog();
    }
    TextBox CodeBox(string source) { return new TextBox { Text = source ?? "", AcceptsReturn = true, AcceptsTab = true, TextWrapping = TextWrapping.NoWrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, Height = 108, FontFamily = new FontFamily("Consolas"), FontSize = 12 }; }
}
