using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using iTunesModel;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;

namespace iTunesRemote.WindowsPhone.Service
{
	public class iTunesService : INotifyPropertyChanged
	{
		private readonly ObservableCollection<iTunesPlaylist> _playlists = new ObservableCollection<iTunesPlaylist>();
		private String _currentTrack;

		private readonly string _baseUri;

		public string CurrentTrack
		{
			get { return _currentTrack; }
			private set { _currentTrack = value; InvokePropertyChanged(new PropertyChangedEventArgs("CurrentTrack"));
			}
		}

		public iTunesService(string baseUri)
		{
			_baseUri = baseUri;
			UpdateCurrentTrack();
			LoadPlaylists();
		}

		private void LoadPlaylists()
		{
			var wc = new WebClient();

			var playlistObservable =
				Observable.FromEvent<OpenReadCompletedEventArgs>(wc, "OpenReadCompleted").Take(1);

			playlistObservable.Subscribe(p => ParsePlaylists(p.EventArgs));

			wc.OpenReadAsync(new Uri(_baseUri + "Playlists"));
		}

		public ObservableCollection<iTunesPlaylist> Playlists
		{
			get
			{
				return _playlists;
			}
		}

		private void ParsePlaylists(OpenReadCompletedEventArgs e)
		{
			var jsonSerializer = new JsonSerializer();
			var jtr = new JsonTextReader(new StreamReader(e.Result));
			var playlists = jsonSerializer.Deserialize<List<iTunesPlaylist>>(jtr);

			_playlists.Clear();

			foreach (iTunesPlaylist iTunesPlaylist in playlists)
			{
				_playlists.Add(iTunesPlaylist);
			}

			InvokePropertyChanged(new PropertyChangedEventArgs("Playlists"));
		}

		private void UpdateCurrentTrack()
		{
			var wc = new WebClient();
			
			var observable =
				Observable.FromEvent<OpenReadCompletedEventArgs>(wc, "OpenReadCompleted").Take(1);

			observable.Subscribe(p =>
				{
					var jsonSerializer = new JsonSerializer();
					var jtr = new JsonTextReader(new StreamReader(p.EventArgs.Result));
					var currentTrack = jsonSerializer.Deserialize<string>(jtr);

					CurrentTrack = currentTrack;
				});

			wc.OpenReadAsync(new Uri(_baseUri + "currentTrack"));
		}

		public void PlayPause()
		{
			PostToResource("command/PlayPause");
		}

		public void NextTrack()
		{
			PostToResource("command/Next");
		}

		public void PreviousTrack()
		{
			PostToResource("command/Previous");
		}

		private void PostToResource(string resource)
		{
			var wc = new WebClient();

			var observable =
				Observable.FromEvent<UploadStringCompletedEventArgs>(wc, "UploadStringCompleted").Take(1);

			observable.Subscribe(p =>
			{
				var jsonSerializer = new JsonSerializer();
				var jtr = new JsonTextReader(new StringReader(p.EventArgs.Result));

				var commandResult = jsonSerializer.Deserialize<iTunesCommandResult>(jtr);

				if (commandResult.Success)
				{
					CurrentTrack = commandResult.CurrentTrack;
				}
			});

			wc.UploadStringAsync(new Uri(_baseUri + resource), "POST", String.Empty);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void InvokePropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}