using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace iTunesRemote.Agent
{
	public class ObservableTraceListener : TraceListener
	{
		private readonly ObservableCollection<string> _strings;

		public ObservableTraceListener(ObservableCollection<string> strings)
		{
			_strings = strings;
		}

		public override void Write(string message)
		{
			Application.Current.Dispatcher.BeginInvoke((Action) (() => _strings.Add(message)));
		}

		public override void WriteLine(string message)
		{
			Application.Current.Dispatcher.BeginInvoke((Action) (() => _strings.Add(message)));
		}
	}
}