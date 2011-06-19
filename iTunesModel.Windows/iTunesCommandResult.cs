using System;

namespace iTunesModel
{
	public class iTunesCommandResult
	{
		public iTunesCommandResult()
		{
			Success = true;
			Status = new iTunesStatus();
		}

		public bool Success { get; set; }
		public iTunesStatus Status { get; set; }
		public string ErrorMessage { get; set; }
	}

	public class iTunesStatus
	{
		public string CurrentTrack { get; set; }
		public int SecondsRemaining { get; set; }
		public bool Playing { get; set; }
	}
}