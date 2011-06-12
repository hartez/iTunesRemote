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

		private List<string> ListTrackNames(IITPlaylist playlist)
		{
			var results = new List<string>();

			for (int n = 1; n <= playlist.Tracks.Count; n++)
			{
				results.Add(playlist.Tracks[n].Name);
			}

			return results;
		}

		private IITPlaylist GetPlaylist(string name)
		{
			IITPlaylistCollection playlistCollection = iTunes.LibrarySource.Playlists;

			return playlistCollection.ItemByName[name];
		}

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
						return Response.AsJson(ListTrackNames(playlist));
					}

					return 404;
				};

			Post["/command/{command}"] = parameters =>
				{
					String command = parameters.command;

					Debug.WriteLine("Request: " + Request.Uri);
					Debug.WriteLine("Request to execute command " + command);

					var cmd = (Command)Enum.Parse(typeof (Command), command);

					var result = new iTunesCommandResult();

					switch (cmd)
					{
						case Command.PlayPause:
							iTunes.PlayPause();
							result.Success = true;
							break;
						case Command.Next:
							iTunes.NextTrack();
							result.Success = true;
							break;
						case Command.Previous:
							iTunes.PreviousTrack();
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

		private List<iTunesPlaylist> ListPlaylists()
		{
			var result = new List<iTunesPlaylist>();

			IITPlaylistCollection playlists = iTunes.LibrarySource.Playlists;
			for (int n = 1; n <= playlists.Count; n++)
			{
				var playlist = new iTunesPlaylist(playlists[n].playlistID, playlists[n].Name);

				for(int t = 1; t < playlists[n].Tracks.Count; t++)
				{
					playlist.Tracks.Add(new Track(playlists[n].Tracks[t].Name));
				}

				result.Add(playlist);
			}

			return result;
		}
	}
}