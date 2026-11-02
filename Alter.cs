using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Shell;
using System.Text.Json;
class Profile
{
	public string id { get; set; }
	public string name { get; set; }
	public string process { get; set; }
	public string version { get; set; }
	public string externalTrainer { get; set; }
	public string sourcePage { get; set; }
	public string bannerImage { get; set; }
	public string coverImage { get; set; }
	public List<Feature> cheats { get; set; } = new List<Feature>();
}

class Feature
{
	public string id { get; set; }
	public string name { get; set; }
	public string category { get; set; }
	public string kind { get; set; }
	public string address { get; set; }
	public string dataType { get; set; }
	public string value { get; set; }
	public string hotkey { get; set; }
	public string scriptMode { get; set; }
	public string scriptOn { get; set; }
	public string scriptOff { get; set; }
	public string scriptTick { get; set; }
	public bool freeze { get; set; }
	public bool hidden { get; set; }
	public bool trusted { get; set; }
}

partial class Alter : Window
{
	string root = AppDomain.CurrentDomain.BaseDirectory; Grid body; TextBox search; Profile selected; List<Profile> profiles = new List<Profile>();
	Brush Ink = new SolidColorBrush(Color.FromRgb(231, 229, 244)), Muted = new SolidColorBrush(Color.FromRgb(137, 135, 162));
	public Alter()
	{
		Title = "Alter";
		Width = 1280;
		Height = 853;
		MinWidth = 1100;
		MinHeight = 700;
		WindowStartupLocation = WindowStartupLocation.CenterScreen;
		WindowStyle = WindowStyle.None;
		ResizeMode = ResizeMode.CanResize;
		Background = new SolidColorBrush(Color.FromRgb(9, 9, 21));
		Foreground = Ink;
		FontFamily = new FontFamily("Segoe UI");
		FontSize = 12;
		WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 57, ResizeBorderThickness = new Thickness(6), GlassFrameThickness = new Thickness(0), CornerRadius = new CornerRadius(0), UseAeroCaptionButtons = false });
		var x = @"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><Grid.Resources><Style TargetType='Button'><Setter Property='Foreground' Value='#DAD6ED'/><Setter Property='Background' Value='#191329'/><Setter Property='BorderBrush' Value='#302344'/><Setter Property='Padding' Value='16,8'/><Setter Property='Cursor' Value='Hand'/><Setter Property='Template'><Setter.Value><ControlTemplate TargetType='Button'><Border Background='{TemplateBinding Background}' BorderBrush='{TemplateBinding BorderBrush}' BorderThickness='1' CornerRadius='8'><ContentPresenter Margin='{TemplateBinding Padding}' HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}' VerticalAlignment='Center'/></Border></ControlTemplate></Setter.Value></Setter></Style><Style TargetType='TextBox'><Setter Property='Foreground' Value='#DAD6ED'/><Setter Property='Background' Value='#111020'/><Setter Property='BorderBrush' Value='#262039'/><Setter Property='Padding' Value='10,6'/></Style></Grid.Resources><Grid.ColumnDefinitions><ColumnDefinition Width='208'/><ColumnDefinition Width='*'/></Grid.ColumnDefinitions><Border Background='#0C0B1C' BorderBrush='#201A32' BorderThickness='0,0,1,0'><Grid><StackPanel x:Name='Nav'/><Border x:Name='FooterHost' VerticalAlignment='Bottom' Margin='15,0,15,20' Height='78' Padding='12,10' CornerRadius='11' Background='#100E22' BorderBrush='#29213B' BorderThickness='1'><Grid><Grid.ColumnDefinitions><ColumnDefinition Width='34'/><ColumnDefinition Width='*'/></Grid.ColumnDefinitions><Grid.RowDefinitions><RowDefinition Height='*'/><RowDefinition Height='19'/></Grid.RowDefinitions><Border Width='31' Height='31' CornerRadius='9' Background='#21143A' HorizontalAlignment='Left' VerticalAlignment='Center'><Image x:Name='FooterLogo' Width='24' Height='24' Stretch='Uniform'/></Border><StackPanel Grid.Column='1' VerticalAlignment='Center' Margin='4,0,0,0'><TextBlock Foreground='#F4ECFF' FontWeight='SemiBold' FontSize='12' Text='Alter'/><TextBlock Foreground='#877A9E' FontSize='7.5' Text='Open Source · Community Driven'/></StackPanel><TextBlock Grid.Row='1' Grid.ColumnSpan='2' Foreground='#847699' FontSize='9' VerticalAlignment='Bottom' Text='v0.2.0'/></Grid></Border></Grid></Border><Grid Grid.Column='1'><Grid.RowDefinitions><RowDefinition Height='57'/><RowDefinition Height='*'/></Grid.RowDefinitions><Grid x:Name='Top'><TextBox x:Name='Search' Width='360' Height='32' HorizontalAlignment='Left' Margin='22,0,0,0' VerticalAlignment='Center' Text='Search games…'/><StackPanel Orientation='Horizontal' HorizontalAlignment='Right' VerticalAlignment='Center' Margin='0,0,8,0' x:Name='WindowButtons'/></Grid><Grid x:Name='Body' Grid.Row='1'/></Grid></Grid>";
		var shell = (Grid)XamlReader.Parse(x);
		Content = shell;
		InstallScrollStyle();
		body = (Grid)shell.FindName("Body");
		search = (TextBox)shell.FindName("Search");
		WindowChrome.SetIsHitTestVisibleInChrome(search, true);
		search.GotFocus += (s, e) => { if (search.Text == "Search games…") search.Text = ""; };
		search.KeyDown += (s, e) => { if (e.Key == Key.Enter) Library(); };
		var nav = (StackPanel)shell.FindName("Nav");
		var branding = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(26, 16, 8, 24) };
		var brandLogo = Image("logo.png", new Int32Rect(65, 90, 369, 300), 32, 28);
		brandLogo.Margin = new Thickness(0, 0, 10, 0);
		branding.Children.Add(brandLogo);
		var brandName = Image("wordmark.jpg", new Int32Rect(240, 560, 780, 120), 100, 30);
		brandName.Margin = new Thickness(0);
		branding.Children.Add(brandName);
		nav.Children.Add(branding);
		var footerLogo = (System.Windows.Controls.Image)shell.FindName("FooterLogo");
		footerLogo.Source = Image("logo.png", new Int32Rect(65, 90, 369, 300), 24, 24).Source;
		nav.Children.Add(Nav("Home", "home", () => Home()));
		nav.Children.Add(Nav("Library", "library", () => Library()));
		nav.Children.Add(Nav("Extensions", "extensions", () => Extensions()));
		nav.Children.Add(Nav("Community", "community", () => Community()));
		nav.Children.Add(Nav("Settings", "settings", () => Settings()));
		var wb = (StackPanel)shell.FindName("WindowButtons");
		wb.Children.Add(WindowButton("—", () => WindowState = WindowState.Minimized));
		wb.Children.Add(WindowButton("□", () => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized));
		wb.Children.Add(WindowButton("×", () => Close()));
		foreach (Button windowButton in wb.Children)
			WindowChrome.SetIsHitTestVisibleInChrome(windowButton, true);
		Load();
		LoadPreferences();
		InitializeTray();
		if (prefs.StartupPage == "Library")
			Library();
		else
			Home();
		Closed += (s, e) => { StopScripts(); if (session != null) session.Dispose(); };
	}

	Image Image(string file, Int32Rect crop, double w, double h)
	{
		ImageSource source = null;
		string path = Path.Combine(root, "assets", file);
		if (File.Exists(path))
			try
			{
				var bitmap = new BitmapImage(new Uri(path));
				source = crop.Width > 0 && crop.Height > 0 && crop.X >= 0 && crop.Y >= 0 && crop.X + crop.Width <= bitmap.PixelWidth && crop.Y + crop.Height <= bitmap.PixelHeight ? (ImageSource)new CroppedBitmap(bitmap, crop) : bitmap;
			}
			catch { }
		return new Image { Source = source, Width = w, Height = h, Stretch = Stretch.Uniform, Margin = new Thickness(0, 0, 10, 0) };
	}

	Button Button(string label, Action action, bool primary)
	{
		var b = new Button { Content = label, HorizontalAlignment = HorizontalAlignment.Left, MinWidth = 125, Height = 38, Margin = new Thickness(0, 12, 0, 0), Background = new SolidColorBrush(primary ? AccentColor : Color.FromRgb(15, 13, 31)), HorizontalContentAlignment = HorizontalAlignment.Left };
		b.Click += (s, e) => action();
		return b;
	}

	TextBlock Text(string t, double size, Brush color)
	{
		return new TextBlock { Text = t, FontSize = size, Foreground = color, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 5, 0, 5) };
	}

	Border Card(UIElement child)
	{
		return new Border { Child = child, Background = new SolidColorBrush(Color.FromRgb(13, 13, 28)), BorderBrush = new SolidColorBrush(Color.FromRgb(33, 28, 48)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(10), Padding = new Thickness(16), Margin = new Thickness(0, 0, 0, 12) };
	}

	void Load()
	{
		Directory.CreateDirectory(Path.Combine(root, "profiles"));
		profiles.Clear();
		foreach (var f in Directory.GetFiles(Path.Combine(root, "profiles"), "*.json"))
			try
			{
				profiles.Add(JsonSerializer.Deserialize<Profile>(File.ReadAllText(f)));
			}
			catch { }
		if (selected == null)
			selected = profiles.FirstOrDefault();
	}

	void Extensions()
	{
		ExtensionsPage();
	}
	
	void Community()
	{
		CommunityPage();
	}
	
	void Library()
	{
		LibraryPage();
	}
	
	
	[STAThread]
	static void Main()
	{
		try
		{
			new Application().Run(new Alter());
		}
		catch (Exception ex) { try { File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup-error.txt"), ex.ToString()); } catch { } MessageBox.Show(ex.Message, "Alter could not open"); }
	}
}




