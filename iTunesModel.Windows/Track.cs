namespace iTunesModel
{
	public class Track
	{
		public Track(string name, int low, int high)
		{
			Low = low;
			High = high;
			Name = name;
		}

		public int Low { get; set; }
		public int High { get; set; }

		public string Name { get; set; }
	}
}