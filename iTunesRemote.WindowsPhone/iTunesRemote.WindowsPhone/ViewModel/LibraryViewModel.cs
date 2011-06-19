using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using iTunesModel;
using iTunesRemote.WindowsPhone.Service;

namespace iTunesRemote.WindowsPhone.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
		private iTunesService iTunesService { get; set; }

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
    	/// The <see cref="SelectedTrack" /> property's name.
    	/// </summary>
    	public const string SelectedTrackPropertyName = "SelectedTrack";

    	/// <summary>
    	/// The <see cref="SelectedPlayList" /> property's name.
    	/// </summary>
    	public const string SelectedPlayListPropertyName = "SelectedPlayList";

    	private iTunesPlaylist _selectedPlayList = null;

    	/// <summary>
    	/// Gets the SelectedPlayList property.
    	/// Changes to that property's value raise the PropertyChanged event. 
    	/// </summary>
		public iTunesPlaylist SelectedPlayList
    	{
    		get { return _selectedPlayList; }

    		set
    		{
    			if (_selectedPlayList == value)
    			{
    				return;
    			}

    			_selectedPlayList = value;

    			// Update bindings, no broadcast
    			Debug.WriteLine("Selected playlist changed");
    			RaisePropertyChanged(SelectedPlayListPropertyName);
    		}
    	}

        /// <summary>
        ///   Initializes a new instance of the MainViewModel class.
        /// </summary>
        public LibraryViewModel(iTunesService iTunesModel)
        {
        	iTunesService = iTunesModel;
        	if (IsInDesignMode)
        	{
        		var blendPlaylist = new iTunesPlaylist(3, "Wicked Awesome Playlist");
				blendPlaylist.Tracks.Add(new Track("Dancing Queen", 1, 2));
				blendPlaylist.Tracks.Add(new Track("The Humpty Dance", 2, 3));
				blendPlaylist.Tracks.Add(new Track("Birdhouse In Your Soul", 3, 4));

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
				_currentTrack = iTunesService.CurrentTrack;

				iTunesService.PropertyChanged += TunesModelPropertyChanged;

				NextTrackCommand = new RelayCommand<int>(tracks => iTunesService.NextTrack(tracks));
				PreviousTrackCommand = new RelayCommand<int>(tracks => iTunesService.PreviousTrack(tracks));
				PlayPauseCommand = new RelayCommand(() => iTunesService.PlayPause());
				PlayTrackCommand = new RelayCommand(() =>
					{
						if (SelectedPlayList != null)
						{
							if (SelectedPlayList.SelectedTrack != null)
							{
								iTunesService.PlaySelectedTrack(SelectedPlayList, SelectedPlayList.SelectedTrack);
							}
						}
					});
            }
        }

		private void SynchronizePlaylists()
		{
			Playlists.Clear();

			foreach (var playList in iTunesService.Playlists)
			{
				Playlists.Add(playList);
			}
		}

    	void TunesModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "CurrentTrack")
			{
				CurrentTrack = iTunesService.CurrentTrack;
			}
			else if(e.PropertyName == "Playlists")
			{
				SynchronizePlaylists();
			}
		}

		public RelayCommand<int> NextTrackCommand { get; private set; }
		public RelayCommand PlayPauseCommand { get; private set; }
		public RelayCommand<int> PreviousTrackCommand { get; private set; }
		public RelayCommand PlayTrackCommand { get; private set; }
    }
}