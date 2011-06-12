using System;
using iTunesRemote.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace iTunesRemote.WindowsPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		private readonly ApplicationBarIconButton NextButton;
		private readonly ApplicationBarIconButton PlayPauseButton;
		private readonly ApplicationBarIconButton PreviousButton;

		private LibraryViewModel _viewModel;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			PreviousButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
			PlayPauseButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
			NextButton = (ApplicationBarIconButton) ApplicationBar.Buttons[2];

			PreviousButton.Click += PreviousButton_Click;
			PlayPauseButton.Click += PlayPauseButton_Click;
			NextButton.Click += NextButton_Click;
		}

		private LibraryViewModel ViewModel
		{
			get { return _viewModel ?? (_viewModel = (LibraryViewModel) DataContext); }
		}

		private void NextButton_Click(object sender, EventArgs e)
		{
			ViewModel.NextTrackCommand.Execute(null);
		}

		private void PlayPauseButton_Click(object sender, EventArgs e)
		{
			ViewModel.PlayPauseCommand.Execute(null);
		}

		private void PreviousButton_Click(object sender, EventArgs e)
		{
			ViewModel.PreviousTrackCommand.Execute(null);
		}
	}
}