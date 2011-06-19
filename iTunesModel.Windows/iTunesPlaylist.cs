using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace iTunesModel 
{
	public class iTunesPlaylist : INotifyPropertyChanged
	{
		public iTunesPlaylist(int id, string name)
		{
			Id = id;
			Name = name;
			Tracks = new List<Track>();
		}

		public int Id { get; set; }
		public string Name { get; set; }

		public List<Track> Tracks { get; set; }

		private Track _selectedTrack = null;

		/// <summary>
		/// Gets the SelectedTrack property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public Track SelectedTrack
		{
			get { return _selectedTrack; }

			set
			{
				if (_selectedTrack == value)
				{
					return;
				}

				_selectedTrack = value;

				Debug.WriteLine(string.Format("Selected Track changed to {0}", SelectedTrack.Name));

				InvokePropertyChanged(new PropertyChangedEventArgs("SelectedTrack"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void InvokePropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}