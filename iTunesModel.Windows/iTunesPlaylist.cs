using System.Collections.Generic;

namespace iTunesModel 
{
	public class iTunesPlaylist
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
	}

	public class Track
	{
		public Track(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}