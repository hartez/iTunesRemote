using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using iTunesModel;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;

namespace iTunesRemote.WindowsPhone.Service
{
	public class iTunesService : INotifyPropertyChanged
	{
		private readonly string _baseUri;
		private readonly ObservableCollection<iTunesPlaylist> _playlists = new ObservableCollection<iTunesPlaylist>();
		private iTunesStatus _currentStatus;
		private string _error;

		public iTunesService(string baseUri)
		{
			_baseUri = baseUri;
			CurrentStatus = new iTunesStatus();
			CurrentStatus.CurrentTrack = "";
			UpdateCurrentStatus();
			LoadPlaylists();
		}

		public iTunesStatus CurrentStatus
		{
			get { return _currentStatus; }
			private set
			{
				_currentStatus = value;
				InvokePropertyChanged(new PropertyChangedEventArgs("CurrentStatus"));
			}
		}

		public String Error
		{
			get { return _error; }
			set
			{
				_error = value;
				InvokePropertyChanged(new PropertyChangedEventArgs("Error"));
			}
		}

		public ObservableCollection<iTunesPlaylist> Playlists
		{
			get { return _playlists; }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void LoadPlaylists()
		{
			var wc = new WebClient();
			
			IObservable<IEvent<OpenReadCompletedEventArgs>> playlistObservable =
				Observable.FromEvent<OpenReadCompletedEventArgs>(wc, "OpenReadCompleted").Take(1);

			playlistObservable.Subscribe(p => ParsePlaylists(p.EventArgs), ex => { Error = ex.Message; });

			wc.OpenReadAsync(new Uri(_baseUri + "Playlists"));
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

		private void UpdateCurrentStatus()
		{
			var wc = new WebClient();

			IObservable<IEvent<OpenReadCompletedEventArgs>> observable =
				Observable.FromEvent<OpenReadCompletedEventArgs>(wc, "OpenReadCompleted").Take(1);

			observable.Subscribe(p =>
				{
					
					var jsonSerializer = new JsonSerializer();
					var jtr = new JsonTextReader(new StreamReader(p.EventArgs.Result));
					var status = jsonSerializer.Deserialize<iTunesStatus>(jtr);

					CurrentStatus = status;
				},
			(ex) =>
				{
					Error = ex.Message;
				}

				);

			wc.OpenReadAsync(new Uri(_baseUri + "currentstatus"));
		}

		public void PlayPause()
		{
			PostToResource("command/PlayPause");
		}

		public void NextTrack(int tracks)
		{
			PostToResource(String.Format("command/Next/{0}", tracks));
		}

		public void PreviousTrack(int tracks)
		{
			PostToResource(String.Format("command/Previous/{0}", tracks));
		}

		private void PostToResource(string resource)
		{
			var wc = new WebClient();

			IObservable<IEvent<UploadStringCompletedEventArgs>> observable =
				Observable.FromEvent<UploadStringCompletedEventArgs>(wc, "UploadStringCompleted").Take(1);

			observable.Subscribe(p =>
				{
					var jsonSerializer = new JsonSerializer();
					var jtr = new JsonTextReader(new StringReader(p.EventArgs.Result));

					var commandResult = jsonSerializer.Deserialize<iTunesCommandResult>(jtr);

					if (commandResult.Success)
					{
						CurrentStatus = commandResult.Status;
					}
				},
				(ex) =>
				{
					Error = ex.Message;
				});

			wc.UploadStringAsync(new Uri(_baseUri + resource), "POST", String.Empty);
		}

		public void InvokePropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public void PlaySelectedTrack(iTunesPlaylist playlist, Track track)
		{
			PostToResource(String.Format("command/play/{0}/{1}/{2}", playlist.Name, track.Low, track.High));
		}
	}
}