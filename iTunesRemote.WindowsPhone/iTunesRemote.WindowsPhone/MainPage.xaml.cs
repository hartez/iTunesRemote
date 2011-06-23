using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using iTunesRemote.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;

namespace iTunesRemote.WindowsPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		private readonly ApplicationBarIconButton NextButton;
		private readonly ApplicationBarIconButton PlayPauseButton;
		private readonly ApplicationBarIconButton PreviousButton;

		private MainViewModel _viewModel;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			PreviousButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
			PlayPauseButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
			NextButton = (ApplicationBarIconButton) ApplicationBar.Buttons[2];

			// Start observing the click events
			var prevButtonClicks = Observable.FromEvent<EventArgs>(PreviousButton, "Click");
			var nextButtonClicks = Observable.FromEvent<EventArgs>(NextButton, "Click");

			// Create a observation of clicking any number of times and then not clicking again for an interval
			var prevButtonStop = prevButtonClicks.Throttle(TimeSpan.FromMilliseconds(2000));
			var nextButtonStop = nextButtonClicks.Throttle(TimeSpan.FromMilliseconds(2000));

			// Since we care about a "click and then stop clicking" scenario for both buttons,
			// merge those events together
			var stoppedClicking = prevButtonStop.Merge(nextButtonStop);
			
			// Now, if either of those sequences happens, stoppedClicking will return an event
				
			// While a button is being clicked, we want to keep track of how many times it was clicked
			// We can do that with an Aggregate
			var countPrevClicks = prevButtonClicks
				.TakeUntil(stoppedClicking)
				.Aggregate(0, (acc, ev) => acc - 1);

			// Create an Aggregate for each one
			var countNextClicks = nextButtonClicks
						.TakeUntil(stoppedClicking)
						.Aggregate(0, (acc, ev) => acc + 1);


			// Now we want to run those counters in parallel and when the user stops clicking,
			// we want to bring them together to see what the total number is; for that we can use
			// ForkJoin. Our selector will ultimately return an integer sum of the number of clicks
			// in either direction; when we subscribe to that result, we can use that value
			countPrevClicks.ForkJoin(countNextClicks, (prev, next) => prev + next)
				.Repeat()
				.Subscribe(e => Dispatcher.BeginInvoke(() => Jump(e, false)));

			PlayPauseButton.Click += PlayPauseButton_Click;

			Messenger.Default.Register<bool>(this,
				(playing) =>
				{
					PlayPauseButton.IconUri = playing
						? new Uri("/Buttons/pause.png", UriKind.Relative)
						: new Uri("/Buttons/play.png", UriKind.Relative);
				}

				);
		}

		private MainViewModel ViewModel
		{
			get { return _viewModel ?? (_viewModel = (MainViewModel) DataContext); }
		}

		private void Jump(int tracks, bool togglePlay)
		{
			if(togglePlay)
			{
				Messenger.Default.Register<PropertyChangedMessage<string>>(
					this, e =>
						{
							Messenger.Default.Unregister<PropertyChangedMessage<string>>(this);
							ViewModel.PlayPauseCommand.Execute(null); 
						});
			}

			if(tracks < 0)
			{
				ViewModel.PreviousTrackCommand.Execute(tracks * -1);
			}
			else if(tracks > 0)
			{
				ViewModel.NextTrackCommand.Execute(tracks);
			}
		}

		private void PlayPauseButton_Click(object sender, EventArgs e)
		{
			ViewModel.PlayPauseCommand.Execute(null);
		}
	}
}