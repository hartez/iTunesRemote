using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using GalaSoft.MvvmLight;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;

namespace iTunesRemote.WindowsPhone.ViewModel
{
    /// <summary>
    ///   This class contains properties that the main View can data bind to.
    ///   <para>
    ///     Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///   </para>
    ///   <para>
    ///     You can also use Blend to data bind with the tool's support.
    ///   </para>
    ///   <para>
    ///     See http://www.galasoft.ch/mvvm
    ///   </para>
    /// </summary>
    public class LibraryViewModel : ViewModelBase
    {
        private readonly ObservableCollection<iTunesPlaylist> _playlists = new ObservableCollection<iTunesPlaylist>();

        /// <summary>
        ///   Initializes a new instance of the MainViewModel class.
        /// </summary>
        public LibraryViewModel()
        {
            if (IsInDesignMode)
            {
                Playlists.Add(new iTunesPlaylist(1, "Test"));
                Playlists.Add(new iTunesPlaylist(2, "Test2"));
            }
            else
            {
                //LoadPlaylists();
            }
        }

        public ObservableCollection<iTunesPlaylist> Playlists
        {
            get { return _playlists; }
        }

        private string _baseUri = "http://192.168.1.12:8081/";

        private void LoadPlaylists()
        {
            var wc = new WebClient();

            var playlistObservable =
                Observable.FromEvent<OpenReadCompletedEventArgs>(wc, "OpenReadCompleted").Take(1);

            playlistObservable.Subscribe(p => ParsePlaylists(p.EventArgs));

            wc.OpenReadAsync(new Uri(_baseUri + "Playlists"));
        }

        private void ParsePlaylists(OpenReadCompletedEventArgs e)
        {
            var jsonSerializer = new JsonSerializer();
            var jtr = new JsonTextReader(new StreamReader(e.Result));
            var playlists = jsonSerializer.Deserialize<List<iTunesPlaylist>>(jtr);

            foreach (iTunesPlaylist iTunesPlaylist in playlists)
            {
                Playlists.Add(iTunesPlaylist);
            }
        }
    }
}