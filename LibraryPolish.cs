using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

partial class Alter
{
    void InstallScrollStyle()
    {
        const string xaml = @"<Style xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='ScrollBar'>
    <Setter Property='Width' Value='11'/><Setter Property='Margin' Value='0,10,7,10'/>
    <Setter Property='Background' Value='Transparent'/>
    <Setter Property='Template'><Setter.Value><ControlTemplate TargetType='ScrollBar'>
      <Grid Width='11' Background='Transparent'>
        <Track x:Name='PART_Track' IsDirectionReversed='True' Minimum='{TemplateBinding Minimum}' Maximum='{TemplateBinding Maximum}' Value='{TemplateBinding Value}' ViewportSize='{TemplateBinding ViewportSize}'>
          <Track.Thumb><Thumb><Thumb.Template><ControlTemplate TargetType='Thumb'>
            <Border Width='5' HorizontalAlignment='Center' CornerRadius='3' Background='#805D4980'/>
          </ControlTemplate></Thumb.Template></Thumb></Track.Thumb>
        </Track>
      </Grid>
    </ControlTemplate></Setter.Value></Setter>
  </Style>";
        Resources[typeof(System.Windows.Controls.Primitives.ScrollBar)] = (Style)XamlReader.Parse(xaml);
    }
    Button LibraryAction(string label, Action action, bool primary, double width) { var b = Button(label, action, primary); b.Width = width; b.MinWidth = 0; b.Height = 32; b.Margin = new Thickness(0); b.Padding = new Thickness(5, 0, 5, 0); b.HorizontalContentAlignment = HorizontalAlignment.Center; if (!primary) { b.Background = Brush("#211630"); b.BorderBrush = Brush("#453058"); } return b; }
    void LibraryPage()
    {
        SelectNav("library"); body.Children.Clear();
        var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled }; body.Children.Add(scroll);
        var panel = new StackPanel { Margin = new Thickness(28, 24, 34, 32) }; scroll.Content = panel;
        panel.Children.Add(Copy("Library", 27, "#F2EAFB")); panel.Children.Add(Copy("Your trainers, stored on this PC.", 13, "#A99AB9"));
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 16, 0, 20) };
        var add = Button("＋ Add trainer", () => Import(), true); add.Margin = new Thickness(0); actions.Children.Add(add);
        var create = Button("Create your own", () => Create(), false); create.Margin = new Thickness(10, 0, 0, 0); actions.Children.Add(create); panel.Children.Add(actions);
        string q = search.Text == "Search games…" ? "" : search.Text;
        var matches = profiles.Where(t => (t.name ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(t => t.name).ToList();
        if (matches.Count == 0) { var empty = new StackPanel { Margin = new Thickness(20) }; empty.Children.Add(Copy("No trainers here yet", 18, "#E8DDF3")); empty.Children.Add(Copy("Add a trainer profile or create one to start your library.", 12, "#9889AA")); panel.Children.Add(Surface(empty, new Thickness(0))); return; }
        var grid = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Stretch }; panel.Children.Add(grid);
        foreach (var profile in matches)
        {
            var card = new StackPanel();
            var header = new StackPanel { Orientation = Orientation.Horizontal };
            var cover = ArtworkPreview(profile.coverImage, 68, 86); cover.Margin = new Thickness(0, 0, 15, 0); header.Children.Add(cover);
            var details = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Width = 157 };
            var name = Copy(profile.name, 16, "#F2EBFA"); name.FontWeight = FontWeights.SemiBold; name.Height = 51; name.TextTrimming = TextTrimming.CharacterEllipsis; details.Children.Add(name);
            var count = Copy(profile.cheats.Count + " features", 11, "#AB9CBA"); details.Children.Add(count);
            header.Children.Add(details); card.Children.Add(header);
            var buttons = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0) };
            buttons.Children.Add(LibraryAction("Open  →", () => OpenProfile(profile), true, 88));
            var art = LibraryAction("Artwork", () => { selected = profile; ArtworkEditor(); }, false, 76); art.Margin = new Thickness(7, 0, 0, 0); buttons.Children.Add(art);
            var remove = LibraryAction("Remove", () => RemoveProfile(profile), false, 71); remove.Margin = new Thickness(7, 0, 0, 0); buttons.Children.Add(remove);
            card.Children.Add(buttons);
            var outer = new Border { Child = card, Width = 285, MinHeight = 190, Padding = new Thickness(16), Margin = new Thickness(0, 0, 14, 14), CornerRadius = new CornerRadius(18), BorderBrush = Brush("#3B2B52"), BorderThickness = new Thickness(1), Background = new LinearGradientBrush(Color.FromArgb(115, 29, 21, 48), Color.FromArgb(45, 16, 13, 33), 90) };
            grid.Children.Add(outer);
        }
    }
}
