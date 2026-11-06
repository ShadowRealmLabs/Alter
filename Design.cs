using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
partial class Alter
{
	readonly Dictionary<string, Button> navButtons = new Dictionary<string, Button>();
	Brush Brush(string hex)
	{
		return (Brush)new BrushConverter().ConvertFromString(hex);
	}
	UIElement Glyph(string name, double size, string color)
	{
		string data = name == "home" ? "M2,10 L12,2 22,10 M5,9 L5,22 10,22 10,15 15,15 15,22 20,22 20,9" : name == "library" ? "M3,3 L10,3 10,10 3,10 Z M14,3 L21,3 21,10 14,10 Z M3,14 L10,14 10,21 3,21 Z M14,14 L21,14 21,21 14,21 Z" : name == "extensions" ? "M3,5 L9,5 C7,-1 17,-1 15,5 L21,5 21,11 C27,9 27,19 21,17 L21,23 3,23 3,17 C9,19 9,9 3,11 Z" : name == "community" ? "M8,12 C1,12 1,2 8,2 C15,2 15,12 8,12 M1,22 C1,13 15,13 15,22 M18,3 C25,3 25,12 18,12 M18,16 C24,16 24,20 24,22" : name == "clock" ? "M12,1 A11,11 0 1 1 11.99,1 M12,5 L12,12 17,15" : name == "arrow" ? "M3,12 L21,12 M15,6 L21,12 15,18" : name == "star" ? "M12,2 L15,9 22,9 17,14 19,22 12,18 5,22 7,14 2,9 9,9 Z" : name == "upload" ? "M12,17 L12,3 M7,8 L12,3 17,8 M4,15 L4,21 20,21 20,15" : name == "search" ? "M10,2 A8,8 0 1 1 9.99,2 M16,16 L22,22" : "M12,2 L15,6 20,6 20,11 23,14 20,18 15,18 12,22 9,18 4,18 4,13 1,10 4,6 9,6 Z M12,8 A4,4 0 1 1 11.99,8";
		return new Viewbox { Width = size, Height = size, Child = new System.Windows.Shapes.Path { Data = Geometry.Parse(data), Stroke = Brush(color), StrokeThickness = 1.6, StrokeLineJoin = PenLineJoin.Round, Stretch = Stretch.Uniform } };
	}
	Button Nav(string label, string key, Action action)
	{
		var row = new StackPanel { Orientation = Orientation.Horizontal };
		row.Children.Add(Glyph(key, 21, "#A49AB9"));
		row.Children.Add(new TextBlock { Text = label, Margin = new Thickness(16, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, FontSize = 13 });
		var b = new Button { Content = row, HorizontalContentAlignment = HorizontalAlignment.Left, Height = 42, Margin = new Thickness(16, 4, 16, 4), Padding = new Thickness(14, 0, 0, 0), Background = Brush("#0C0B1C"), BorderBrush = Brush("#0C0B1C") };
		b.Click += (s, e) => action();
		navButtons[key] = b;
		return b;
	}
	void SelectNav(string key)
	{
		foreach (var item in navButtons)
		{
			item.Value.Background = item.Key == key ? new LinearGradientBrush(Color.FromArgb(140, AccentColor.R, AccentColor.G, AccentColor.B), Color.FromRgb(24, 13, 48), 0) : Brush("#0C0B1C");
			item.Value.BorderBrush = Brush(item.Key == key ? AccentHex : "#0C0B1C");
			item.Value.Foreground = Brush(item.Key == key ? "#FFFFFF" : "#A49AB9");
		}
	}
	Button WindowButton(string label, Action action)
	{
		var b = new Button { Content = label, Width = 48, Height = 42, MinWidth = 0, Margin = new Thickness(0), Padding = new Thickness(0), FontSize = label == "×" ? 25 : 18, FontFamily = new FontFamily("Segoe UI Symbol"), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Background = Brush("#090915"), BorderBrush = Brush("#090915") };
		b.Click += (s, e) => action();
		return b;
	}
	Border Surface(UIElement child, Thickness padding)
	{
		return new Border { Child = child, Padding = padding, CornerRadius = new CornerRadius(13), BorderBrush = Brush("#282038"), BorderThickness = new Thickness(1), Background = Brush("#0E0C20") };
	}
	TextBlock Copy(string label, double size, string color)
	{
		return new TextBlock { Text = label, FontSize = size, Foreground = Brush(color), TextWrapping = TextWrapping.Wrap, LineHeight = size * 1.5 };
	}
	Button ActionLink(string title, string sub, string icon, Action action)
	{
		var g = new Grid();
		g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
		g.ColumnDefinitions.Add(new ColumnDefinition());
		g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
		var tile = new Border { Width = 38, Height = 38, CornerRadius = new CornerRadius(9), Background = Brush("#21143B"), Child = Glyph(icon, 19, AccentSoftHex) };
		g.Children.Add(tile);
		var t = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0) };
		t.Children.Add(Copy(title, 12, "#E9E3F5"));
		t.Children.Add(Copy(sub, 10, "#80758F"));
		Grid.SetColumn(t, 1);
		g.Children.Add(t);
		var a = Copy("›", 20, "#9383AE");
		Grid.SetColumn(a, 2);
		g.Children.Add(a);
		var b = new Button { Content = g, Height = prefs.CompactLayout ? 52 : 60, Background = Brush("#0E0C20"), BorderBrush = Brush("#0E0C20"), Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Stretch };
		b.Click += (s, e) => action();
		return b;
	}
	void Home()
	{
		SelectNav("home");
		body.Children.Clear();
		var scroll = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
		body.Children.Add(scroll);
		var g = new Grid { Margin = new Thickness(24, prefs.CompactLayout ? 13 : 23, 24, 28) };
		scroll.Content = g;
		g.ColumnDefinitions.Add(new ColumnDefinition());
		g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
		g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(305) });
		var left = new StackPanel();
		g.Children.Add(left);
		var hero = new Grid { Height = prefs.CompactLayout ? 275 : 318, ClipToBounds = true };
		var art = Image("hero.jpg", new Int32Rect(0, 160, 3840, 1776), Double.NaN, Double.NaN);
		hero.Background = new LinearGradientBrush(Color.FromRgb(13, 8, 31), Color.FromRgb(22, 14, 38), 0);
		hero.Children.Add(new Border { Width = 500, Height = 500, CornerRadius = new CornerRadius(250), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, -140, 0), Background = new RadialGradientBrush(Color.FromArgb(160, 83, 32, 143), Color.FromArgb(0, 83, 32, 143)) });
		art.Width = Double.NaN;
		art.Height = Double.NaN;
		art.Stretch = Stretch.UniformToFill;
		art.HorizontalAlignment = HorizontalAlignment.Stretch;
		art.VerticalAlignment = VerticalAlignment.Stretch;
		art.Margin = new Thickness(0);
		var artMask = new LinearGradientBrush();
		artMask.StartPoint = new Point(0, 0);
		artMask.EndPoint = new Point(1, 0);
		artMask.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0));
		artMask.GradientStops.Add(new GradientStop(Colors.White, .15));
		artMask.GradientStops.Add(new GradientStop(Colors.White, .85));
		artMask.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 1));
		art.OpacityMask = artMask;
		hero.Children.Add(art);
		var fade = new LinearGradientBrush();
		fade.StartPoint = new Point(0, 0);
		fade.EndPoint = new Point(1, 0);
		fade.GradientStops.Add(new GradientStop(Color.FromRgb(13, 8, 31), 0));
		fade.GradientStops.Add(new GradientStop(Color.FromArgb(245, 13, 8, 31), .28));
		fade.GradientStops.Add(new GradientStop(Color.FromArgb(0, 13, 8, 31), .58));
		fade.GradientStops.Add(new GradientStop(Color.FromArgb(0, 13, 8, 31), 1));
		hero.Children.Add(new Border { Background = fade });
		var intro = new StackPanel { Margin = new Thickness(28, prefs.CompactLayout ? 30 : 38, 20, 12), Width = 350, HorizontalAlignment = HorizontalAlignment.Left };
		intro.Children.Add(Copy("MAKE IT YOURS  ·  v0.2.0", 10, AccentSoftHex));
		var heading = Copy("Your games.\nYour rules.", prefs.CompactLayout ? 27 : 30, "#F6F1FF");
		heading.FontWeight = FontWeights.SemiBold;
		heading.LineHeight = prefs.CompactLayout ? 31 : 35;
		heading.Margin = new Thickness(0, 8, 0, 8);
		intro.Children.Add(heading);
		intro.Children.Add(Copy("Build a collection of the trainers you love.\nEverything stays on your PC.", prefs.CompactLayout ? 12 : 13, "#B0A2C3"));
		var cta = Button("＋  Add your first trainer", () => Import(), true);
		cta.Margin = new Thickness(0, prefs.CompactLayout ? 12 : 16, 0, 0);
		cta.Height = prefs.CompactLayout ? 36 : 40;
		cta.Padding = new Thickness(18, 0, 18, 0);
		intro.Children.Add(cta);
		hero.Children.Add(intro);
		left.Children.Add(Surface(hero, new Thickness(0)));
		var recentHead = new Grid { Margin = new Thickness(0, 23, 0, 13) };
		var h = Copy("Recently Played", 14, "#E9E3F4");
		recentHead.Children.Add(h);
		var view = Button("View library  →", () => Library(), false);
		view.Background = Brush("#090915");
		view.BorderBrush = Brush("#090915");
		view.HorizontalAlignment = HorizontalAlignment.Right;
		view.Height = 24;
		view.MinWidth = 0;
		view.Margin = new Thickness(0);
		view.Padding = new Thickness(0);
		recentHead.Children.Add(view);
		left.Children.Add(recentHead);
		var recent = new Grid { Height = prefs.CompactLayout ? 205 : 225 };
		var empty = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
		empty.Children.Add(new Border { Width = 43, Height = 43, CornerRadius = new CornerRadius(12), Background = Brush("#201333"), Child = Glyph("library", 22, AccentSoftHex), Margin = new Thickness(0, 0, 0, 12) });
		var eTitle = Copy("A little empty. All yours.", 15, "#D9CDE9");
		eTitle.TextAlignment = TextAlignment.Center;
		empty.Children.Add(eTitle);
		var desc = Copy("Your recently used trainers will appear here.", 11, "#82758F");
		desc.Margin = new Thickness(0, 7, 0, 0);
		empty.Children.Add(desc);
		recent.Children.Add(empty);
		FillRecent(recent);
		left.Children.Add(Surface(recent, new Thickness(0)));
		var banner = new Grid { Height = 100, Margin = new Thickness(0, 22, 0, 0) };
		banner.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(92) });
		banner.ColumnDefinitions.Add(new ColumnDefinition());
		banner.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
		banner.Background = new LinearGradientBrush(Color.FromArgb(105, AccentColor.R, AccentColor.G, AccentColor.B), Color.FromRgb(15, 9, 32), 0);
		var bannerLogo = Image("logo.png", new Int32Rect(65, 90, 369, 300), 70, 60);
		bannerLogo.HorizontalAlignment = HorizontalAlignment.Center;
		bannerLogo.VerticalAlignment = VerticalAlignment.Center;
		bannerLogo.Margin = new Thickness(0);
		banner.Children.Add(bannerLogo);
		var words = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
		words.Children.Add(Copy("Join the Alter Community", 14, "#EFE5FF"));
		var caption = Copy("Share your creations. Find your people.", 11, "#9B87BA");
		caption.Margin = new Thickness(0, 6, 0, 0);
		words.Children.Add(caption);
		Grid.SetColumn(words, 1);
		banner.Children.Add(words);
		var join = Button("Join now  →", () => OpenDiscord(), false);
		join.VerticalAlignment = VerticalAlignment.Center;
		join.MinWidth = 0;
		join.Margin = new Thickness(0, 0, 18, 0);
		join.Padding = new Thickness(12, 0, 12, 0);
		Grid.SetColumn(join, 2);
		banner.Children.Add(join);
		left.Children.Add(banner);
		var right = new StackPanel();
		Grid.SetColumn(right, 2);
		g.Children.Add(right);
		var quick = new StackPanel();
		quick.Children.Add(Copy("Quick Actions", 14, "#EFE7F8"));
		quick.Children.Add(new Border { Height = 12 });
		quick.Children.Add(ActionLink("Browse Trainers", "Find your installed trainers", "library", () => Library()));
		quick.Children.Add(ActionLink("Add Extensions", "Add a local extension", "extensions", () => Extensions()));
		quick.Children.Add(ActionLink("Join Community", "Get help, share, and connect", "community", () => OpenDiscord()));
		right.Children.Add(Surface(quick, new Thickness(18)));
		if (prefs.ShowNews)
		{
			var news = new StackPanel();
			news.Children.Add(Copy("Latest Gaming News", 14, "#EFE7F8"));
			news.Children.Add(Copy("PC Gamer · live headlines", 10, "#7F718F"));
			var newsItems = new StackPanel { Margin = new Thickness(0, 14, 0, 0) };
			newsItems.Children.Add(Copy("Loading headlines…", 12, "#A093B4"));
			news.Children.Add(newsItems);
			var nc = Surface(news, new Thickness(18));
			nc.Margin = new Thickness(0, 16, 0, 0);
			right.Children.Add(nc);
			Dispatcher.BeginInvoke(new Action(() => LoadNews(newsItems)));
		}
		var stats = new StackPanel();
		stats.Children.Add(Copy("Your collection", 14, "#EFE7F8"));
		var number = Copy(profiles.Count.ToString("00"), 30, AccentSoftHex);
		number.Margin = new Thickness(0, 12, 0, 0);
		stats.Children.Add(number);
		stats.Children.Add(Copy("installed trainers", 11, "#92819F"));
		var sc = Surface(stats, new Thickness(18));
		sc.Margin = new Thickness(0, 16, 0, 0);
		right.Children.Add(sc);
	}
	async void LoadNews(StackPanel target)
	{
		try
		{
			var bytes = await Task.Run(() => FetchBytes(new Uri("https://www.pcgamer.com/rss/"), "Alter/0.4", null, 10000, 4 * 1024 * 1024, "News feed is too large."));
			string xml = Encoding.UTF8.GetString(bytes);
			var allItems = XDocument.Parse(xml).Descendants("item").Take(50).Select(item => new { Title = (string)item.Element("title"), Url = (string)item.Element("link") }).Where(item => !String.IsNullOrWhiteSpace(item.Title) && !String.IsNullOrWhiteSpace(item.Url)).ToList();
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var diverse = new List<Tuple<string, string>>();
			foreach (var item in allItems)
			{
				Uri uri;
				if (!Uri.TryCreate(item.Url, UriKind.Absolute, out uri) || uri.Scheme != "https")
					continue;
				var parts = uri.AbsolutePath.Trim('/').Split('/');
				string topic = parts.Length > 1 && parts[0] == "games" ? parts[1] : parts[0];
				if (!seen.Add(topic))
					continue;
				diverse.Add(Tuple.Create(item.Title, item.Url));
				if (diverse.Count == 5)
					break;
			}
			target.Children.Clear();
			foreach (var item in diverse)
			{
				string url = item.Item2;
				var b = Button(item.Item1, () => OpenUrl(url), false);
				b.Content = Copy(item.Item1, 12, "#C8BBD9");
				b.Height = Double.NaN;
				b.Margin = new Thickness(0, 0, 0, 12);
				b.Padding = new Thickness(0);
				b.Background = Brush("#0E0C20");
				b.BorderBrush = Brush("#0E0C20");
				target.Children.Add(b);
			}
			if (diverse.Count == 0)
				target.Children.Add(Copy("No headlines available right now.", 12, "#A093B4"));
		}
		catch { target.Children.Clear(); target.Children.Add(Copy("Headlines are unavailable right now.", 12, "#A093B4")); }
		var all = Button("Read all gaming news  ↗", () => OpenUrl("https://www.pcgamer.com/news/"), false);
		all.Margin = new Thickness(0, 6, 0, 0);
		target.Children.Add(all);
	}
}



