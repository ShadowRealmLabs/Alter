using System;
using System.IO;
using System.Windows;
using WinForms = System.Windows.Forms;

partial class Alter
{
	WinForms.NotifyIcon trayIcon;
	System.Drawing.Icon trayGraphic;
	bool exitRequested;

	void InitializeTray()
	{
		string iconPath = Path.Combine(root, "assets", "Alter.ico");
		trayGraphic = File.Exists(iconPath) ? new System.Drawing.Icon(iconPath) : System.Drawing.SystemIcons.Application;
		trayIcon = new WinForms.NotifyIcon { Icon = trayGraphic, Text = "Alter trainer hub", Visible = false };
		var menu = new WinForms.ContextMenuStrip();
		menu.Items.Add("Show Alter", null, (s, e) => Dispatcher.BeginInvoke(new Action(RestoreFromTray)));
		menu.Items.Add("Exit Alter", null, (s, e) => Dispatcher.BeginInvoke(new Action(ExitFromTray)));
		trayIcon.ContextMenuStrip = menu;
		trayIcon.DoubleClick += (s, e) => Dispatcher.BeginInvoke(new Action(RestoreFromTray));
		StateChanged += (s, e) => { if (WindowState == WindowState.Minimized && prefs.MinimizeToTray) HideToTray(); };
		Closing += (s, e) => { SaveWindowPreference(); if (!exitRequested && prefs.CloseToTray) { e.Cancel = true; HideToTray(); } };
		Closed += (s, e) => { trayIcon.Visible = false; trayIcon.Dispose(); if (File.Exists(iconPath)) trayGraphic.Dispose(); };
	}

	void HideToTray()
	{
		if (trayIcon == null)
			return;
		SaveWindowPreference();
		trayIcon.Visible = true;
		ShowInTaskbar = false;
		Hide();
	}

	void RestoreFromTray()
	{
		if (trayIcon == null)
			return;
		ShowInTaskbar = true;
		Show();
		WindowState = WindowState.Normal;
		Activate();
		trayIcon.Visible = false;
	}

	void ExitFromTray()
	{
		exitRequested = true;
		Close();
	}
}
