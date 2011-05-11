using System;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace iTunesRemote.WindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string _baseUri = "http://192.168.1.12:8081/";
		//private string _baseUri = "http://localhost:8080/";
        private WebRequest _request;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            RetrieveCurrentTrack();
        }

        private void GetResource(string resource, OpenReadCompletedEventHandler handler)
        {
            try
            {
                var wc = new WebClient();
                wc.OpenReadCompleted += handler;
                wc.OpenReadAsync(new Uri(_baseUri + resource));
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error connecting to service: " + ex.Message, "Error",
                                MessageBoxButton.OK);
            }
        }

        private void PostToResource(string resource, AsyncCallback callback)
        {
            try
            {
                _request = WebRequest.Create(_baseUri + resource);

                _request.Method = "POST";

                _request.BeginGetResponse(callback, null);
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error connecting to service: " + ex.Message, "Error",
                                MessageBoxButton.OK);
            }
        }

        private void RetrieveCurrentTrack()
        {
            GetResource("currentTrack", CurrentTrackOpenReadCompleted);
        }

        void CurrentTrackOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // Look into userstate property - see if you can centralize webexception error handling

                var jsonSerializer = new JsonSerializer();
                var jtr = new JsonTextReader(new StreamReader(e.Result));
                var currentTrack = jsonSerializer.Deserialize<string>(jtr);

                CurrentTrack.Text = currentTrack;
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error connecting to service: " + ex.Message, "Error",
                                MessageBoxButton.OK);
            }
        }

        private void PlayPauseButtonClick(object sender, RoutedEventArgs e)
        {
            PostToResource("command/PlayPause", CommandIssued);
        }

        private void CommandIssued(IAsyncResult result)
        {
            var response = _request.EndGetResponse(result);

            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                var jsonSerializer = new JsonSerializer();
                var jtr = new JsonTextReader(reader);
                var commandResult = jsonSerializer.Deserialize<iTunesCommandResult>(jtr);

                if (commandResult.Success)
                {
                    Dispatcher.BeginInvoke(() => { CurrentTrack.Text = commandResult.CurrentTrack; });
                }
            }
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            PostToResource("command/Next", CommandIssued);
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            PostToResource("command/Previous", CommandIssued);
        }
    }
}