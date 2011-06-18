using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using iTunesModel;
using iTunesRemote.WindowsPhone.Service;

namespace iTunesRemote.WindowsPhone.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
		private iTunesService TunesModel { get; set; }

		public ObservableCollection<iTunesPlaylist> Playlists { get; set; }

		/// <summary>
		/// The <see cref="CurrentTrack" /> property's name.
		/// </summary>
		public const string CurrentTrackPropertyName = "CurrentTrack";

		private String _currentTrack = String.Empty;

		/// <summary>
		/// Gets the CurrentTrack property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public String CurrentTrack
		{
			get
			{
				if(!String.IsNullOrEmpty(_currentTrack))
				{
					return _currentTrack;
				}

				return String.Empty;
			}

			set
			{
				if (_currentTrack == value)
				{
					return;
				}

				_currentTrack = value;

				// Update bindings, no broadcast
				RaisePropertyChanged(CurrentTrackPropertyName);
			}
		}

        /// <summary>
        ///   Initializes a new instance of the MainViewModel class.
        /// </summary>
        public LibraryViewModel(iTunesService iTunesModel)
        {
        	TunesModel = iTunesModel;
        	if (IsInDesignMode)
        	{
        		var blendPlaylist = new iTunesPlaylist(3, "Blend Playlist");
				blendPlaylist.Tracks.Add(new Track("Dancing Queen"));
				blendPlaylist.Tracks.Add(new Track("The Humpty Dance"));
				blendPlaylist.Tracks.Add(new Track("Birdhouse In Your Soul"));

				Playlists = new ObservableCollection<iTunesPlaylist>
					{
						blendPlaylist,
						new iTunesPlaylist(1, "Test"),
						new iTunesPlaylist(2, "Test2")
					};

            	_currentTrack = "Blister in the Sun";
            }
            else
            {
				Playlists = new ObservableCollection<iTunesPlaylist>();
            	SynchronizePlaylists();
				_currentTrack = TunesModel.CurrentTrack;

				TunesModel.PropertyChanged += TunesModelPropertyChanged;

				NextTrackCommand = new RelayCommand<int>(tracks => TunesModel.NextTrack(tracks));
				PreviousTrackCommand = new RelayCommand<int>(tracks => TunesModel.PreviousTrack(tracks));
				PlayPauseCommand = new RelayCommand(() => TunesModel.PlayPause());
            }
        }

		private void SynchronizePlaylists()
		{
			Playlists.Clear();

			foreach (var playList in TunesModel.Playlists)
			{
				Playlists.Add(playList);
			}
		}

    	void TunesModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "CurrentTrack")
			{
				CurrentTrack = TunesModel.CurrentTrack;
			}
			else if(e.PropertyName == "Playlists")
			{
				SynchronizePlaylists();
			}
		}

		public RelayCommand<int> NextTrackCommand { get; private set; }
		public RelayCommand PlayPauseCommand { get; private set; }
		public RelayCommand<int> PreviousTrackCommand { get; private set; }
    }
}