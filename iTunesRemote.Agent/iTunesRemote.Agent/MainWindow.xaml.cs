using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using iTunesRemote.Agent.Properties;
using Nancy.Hosting.Self;

namespace iTunesRemote.Agent
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ObservableCollection<string> _strings = new ObservableCollection<string>();
		private NancyHost _listener;

		public MainWindow()
		{
			InitializeComponent();

			DebugOutput.DataContext = this;
			DebugOutput.ItemsSource = _strings;

			Debug.Listeners.Add(new ObservableTraceListener(_strings));

			Loaded += MainWindow_Loaded;
			Closing += MainWindow_Closing;

			_contextMenu.MenuItems.Add(new MenuItem("Show", ContextMenuShowClick));
			_contextMenu.MenuItems.Add(new MenuItem("Exit", ContextMenuExitClick));

			var ni = new NotifyIcon
				{
					Icon = new System.Drawing.Icon("remote.ico"),
					Visible = true,
					Text = "iTunes Remote Agent",
					ContextMenu = _contextMenu
				};
			ni.DoubleClick +=
				delegate { ShowWindow(); };
		}

		void ContextMenuShowClick(object sender, EventArgs e)
		{
			ShowWindow();
		}

		void ContextMenuExitClick(object sender, EventArgs e)
		{
			Close();
		}

		private void ShowWindow()
		{
			Show();
			WindowState = WindowState.Normal;
		}

		private ContextMenu _contextMenu = new ContextMenu();

		protected override void OnStateChanged(EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				Hide();
			}

			base.OnStateChanged(e);
		}

		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (_listener != null)
			{
				_listener.Stop();
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				string endpoint = Settings.Default.endpoint;

				Trace.Write("Starting up web service");

				_listener = new NancyHost(new Uri(endpoint));
				
				_listener.Start();

				Trace.Write("Web service started");
			}
			catch (Exception ex)
			{
				var log = new EventLog("iTunesRemote.Agent");
				log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
				throw;
			}
		}
	}
}