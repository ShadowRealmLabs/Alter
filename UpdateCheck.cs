using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

class AlterRelease { public string tag_name, name, html_url, published_at; public bool draft; }

partial class Alter
{
    const string CurrentReleaseTag = "v0.2.0";
    const string AlterReleasesPage = "https://github.com/an1dxsh/Alter/releases";
    const string AlterReleasesApi = "https://api.github.com/repos/an1dxsh/Alter/releases?per_page=30";
    bool updateChecked, updateChecking;
    string updateStatus = "Checking GitHub for releases…", newerReleaseUrl;

    Border UpdateSettingsCard()
    {
        var panel = new StackPanel();
        var heading = Copy("Alter updates", 16, "#F0E8F8"); heading.FontWeight = FontWeights.SemiBold; panel.Children.Add(heading);
        panel.Children.Add(Copy("Installed version  ·  " + CurrentReleaseTag, 11, "#A99AB9"));
        var status = Copy(updateStatus, 12, "#D1C1E1"); status.Margin = new Thickness(0, 12, 0, 4); panel.Children.Add(status);
        var actions = new StackPanel { Orientation = Orientation.Horizontal };
        var check = Button("Check for updates", () => { }, false); check.MinWidth = 0; check.Height = 34; check.Margin = new Thickness(0, 7, 7, 0); check.Padding = new Thickness(11, 0, 11, 0); check.Background = Brush("#241636"); check.BorderBrush = Brush("#4B3064"); actions.Children.Add(check);
        var open = Button("Open release  ↗", () => { if (!String.IsNullOrWhiteSpace(newerReleaseUrl)) OpenUrl(newerReleaseUrl); }, true); open.MinWidth = 0; open.Height = 34; open.Margin = new Thickness(0, 7, 0, 0); open.Padding = new Thickness(11, 0, 11, 0); open.Background = Brush("#581D9C"); open.BorderBrush = Brush("#6B2AAF"); open.Visibility = String.IsNullOrWhiteSpace(newerReleaseUrl) ? Visibility.Collapsed : Visibility.Visible; actions.Children.Add(open); panel.Children.Add(actions);
        check.Click += (s, e) => CheckForAlterUpdates(status, open, check);
        var all = Button("View all releases  ↗", () => OpenUrl(AlterReleasesPage), false); all.MinWidth = 0; all.Height = 30; all.Margin = new Thickness(0, 8, 0, 0); all.Padding = new Thickness(0); all.Background = Brush("#0E0C20"); all.BorderBrush = Brush("#0E0C20"); panel.Children.Add(all);
        var card = Surface(panel, new Thickness(18)); card.Margin = new Thickness(0, 16, 0, 0);
        if (!updateChecked && !updateChecking) Dispatcher.BeginInvoke(new Action(() => CheckForAlterUpdates(status, open, check)));
        return card;
    }

    async void CheckForAlterUpdates(TextBlock status, Button open, Button check)
    {
        if (updateChecking) return; updateChecking = true; check.IsEnabled = false; updateStatus = "Checking GitHub for releases…"; status.Text = updateStatus;
        try
        {
            var releases = await Task.Run(() => ProfileJson().Deserialize<List<AlterRelease>>(FetchCommunityText(AlterReleasesApi, 2 * 1024 * 1024)));
            if (releases == null || releases.Count == 0) { updateStatus = "No published releases found yet."; newerReleaseUrl = null; }
            else
            {
                AlterRelease latest = null;
                foreach (var release in releases.Where(x => x != null && !x.draft && ReleaseLink(x.html_url) && ParseReleaseVersion(x.tag_name) != null))
                    if (latest == null || CompareReleaseVersions(release.tag_name, latest.tag_name) > 0) latest = release;
                if (latest == null) { updateStatus = "GitHub returned releases with tags Alter could not compare."; newerReleaseUrl = null; }
                else if (CompareReleaseVersions(latest.tag_name, CurrentReleaseTag) > 0) { updateStatus = "Update available: " + latest.tag_name; newerReleaseUrl = latest.html_url; }
                else if (CompareReleaseVersions(latest.tag_name, CurrentReleaseTag) < 0) { updateStatus = "This build is newer than GitHub's latest release (" + latest.tag_name + ")."; newerReleaseUrl = null; }
                else { updateStatus = "You're up to date. Latest release: " + latest.tag_name; newerReleaseUrl = null; }
            }
        }
        catch (Exception) { updateStatus = "Could not check GitHub right now. Try again later."; newerReleaseUrl = null; }
        finally { updateChecked = true; updateChecking = false; check.IsEnabled = true; status.Text = updateStatus; open.Visibility = String.IsNullOrWhiteSpace(newerReleaseUrl) ? Visibility.Collapsed : Visibility.Visible; }
    }

    bool ReleaseLink(string url) { Uri uri; return Uri.TryCreate(url, UriKind.Absolute, out uri) && uri.Scheme == "https" && uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) && uri.AbsolutePath.StartsWith("/an1dxsh/Alter/releases/tag/", StringComparison.OrdinalIgnoreCase); }
    string[] ParseReleaseVersion(string tag) { var match = Regex.Match(tag ?? "", @"^v?(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?$", RegexOptions.IgnoreCase); if (!match.Success) return null; int major, minor, patch; if (!int.TryParse(match.Groups[1].Value, out major) || !int.TryParse(match.Groups[2].Value, out minor) || !int.TryParse(match.Groups[3].Value, out patch)) return null; return new[] { major.ToString(), minor.ToString(), patch.ToString(), match.Groups[4].Value }; }
    int CompareReleaseVersions(string left, string right) { var a = ParseReleaseVersion(left); var b = ParseReleaseVersion(right); if (a == null || b == null) throw new FormatException("Invalid release version."); for (int i = 0; i < 3; i++) { int compare = int.Parse(a[i]).CompareTo(int.Parse(b[i])); if (compare != 0) return compare; } if (a[3].Length == 0 && b[3].Length == 0) return 0; if (a[3].Length == 0) return 1; if (b[3].Length == 0) return -1; var ap = a[3].Split('.'); var bp = b[3].Split('.'); for (int i = 0; i < Math.Min(ap.Length, bp.Length); i++) { int an, bn; bool ai = int.TryParse(ap[i], out an), bi = int.TryParse(bp[i], out bn); int compare = ai && bi ? an.CompareTo(bn) : ai ? -1 : bi ? 1 : String.Compare(ap[i], bp[i], StringComparison.OrdinalIgnoreCase); if (compare != 0) return compare; } return ap.Length.CompareTo(bp.Length); }
}
