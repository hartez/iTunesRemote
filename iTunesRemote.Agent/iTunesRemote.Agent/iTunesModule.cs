using System;
using System.Collections.Generic;
using System.Diagnostics;
using iTunesLib;
using iTunesModel;
using Nancy;

namespace iTunesRemote.Agent
{
	public class iTunesModule : NancyModule
	{
		private iTunesApp _itunes;

		public iTunesModule()
		{
			Get["/currentTrack"] = parameters =>
				{
					Debug.WriteLine("Request: " + Request.Uri);
					Debug.WriteLine("Request for current track");

					return Response.AsJson(CurrentTrack);
				};

			Get["/playlists"] = parameters =>
				{
					Debug.WriteLine("Request: " + Request.Uri);
					Debug.WriteLine("Request for list of playlists");

					return Response.AsJson(ListPlaylists());
				};

			Get["/playlists/{name}/tracks"] = parameters =>
				{
					String name = parameters.name;

					Debug.WriteLine("Request: " + Request.Uri);
					Debug.WriteLine("Request for tracks in playlist " + name);

					IITPlaylist playlist = GetPlaylist(name);

					if (playlist != null)
					{
						return Response.AsJson(ListTracks(playlist));
					}

					return 404;
				};

			Post["/command/play/{playlist}/{low}/{high}"] = parameters =>
				{
					string playlist = parameters.playlist;
					int low = parameters.low;
					int high = parameters.high;
					var result = new iTunesCommandResult();

					IITTrack newTrack = GetPlaylist(playlist).Tracks.ItemByPersistentID[high, low];

					if (newTrack != null && newTrack.Enabled)
					{
						newTrack.Play();

						result.CurrentTrack = newTrack.Name;
					}

					result.Success = true;

					return Response.AsJson(result);
				};

			Post["/command/PlayPause"] = parameters =>
				{
					iTunes.PlayPause();

					var result = new iTunesCommandResult();

					result.Success = true;
					result.CurrentTrack = CurrentTrack;

					return Response.AsJson(result);
				};

			Post["/command/{command}/{tracks}"] = parameters =>
				{
					String command = parameters.command;
					int? tracks = parameters.tracks;

					Debug.WriteLine("Request: " + Request.Uri);
					Debug.WriteLine("Request to execute command " + command);

					if (tracks.HasValue)
					{
						Debug.WriteLine(string.Format("By {0} tracks", tracks));
					}

					var cmd = (Command) Enum.Parse(typeof (Command), command);

					var result = new iTunesCommandResult();

					int distance = tracks.HasValue ? tracks.Value : 1;

					switch (cmd)
					{
						case Command.Next:

							for (int n = 0; n < distance; n++)
							{
								iTunes.NextTrack();
							}

							result.Success = true;
							break;
						case Command.Previous:

							for (int n = 0; n < distance; n++)
							{
								iTunes.PreviousTrack();
							}

							result.Success = true;
							break;
						default:
							result.Success = false;
							result.ErrorMessage = "Not a valid command";
							break;
					}

					result.CurrentTrack = CurrentTrack;

					return Response.AsJson(result);
				};
		}

		public string CurrentTrack
		{
			get { return iTunes.CurrentTrack != null ? iTunes.CurrentTrack.Name : "None"; }
		}

		private iTunesApp iTunes
		{
			get
			{
				if (_itunes == null)
				{
					_itunes = new iTunesApp();
				}

				return _itunes;
			}
		}

		private List<Track> ListTracks(IITPlaylist playlist)
		{
			var results = new List<Track>();

			for (int n = 1; n <= playlist.Tracks.Count; n++)
			{
				if (playlist.Tracks[n].Enabled)
				{
					int low;
					int high;
					object iObject = playlist.Tracks[n];
					iTunes.GetITObjectPersistentIDs(ref iObject, out high, out low);

					results.Add(new Track(playlist.Tracks[n].Name, low, high));
				}
			}

			return results;
		}

		private IITPlaylist GetPlaylist(string name)
		{
			IITPlaylistCollection playlistCollection = iTunes.LibrarySource.Playlists;

			return playlistCollection.ItemByName[name];
		}

		private List<iTunesPlaylist> ListPlaylists()
		{
			var result = new List<iTunesPlaylist>();

			IITPlaylistCollection playlists = iTunes.LibrarySource.Playlists;
			for (int n = 1; n <= playlists.Count; n++)
			{
				var playlist = new iTunesPlaylist(playlists[n].playlistID, playlists[n].Name);

				if (playlist.Name != "Library")
				{
					for (int t = 1; t < playlists[n].Tracks.Count; t++)
					{
						int low;
						int high;
						object iObject = playlists[n].Tracks[t];
						iTunes.GetITObjectPersistentIDs(ref iObject, out high, out low);

						playlist.Tracks.Add(new Track(playlists[n].Tracks[t].Name, low, high));
					}

					result.Add(playlist);
				}
			}

			return result;
		}
	}
}