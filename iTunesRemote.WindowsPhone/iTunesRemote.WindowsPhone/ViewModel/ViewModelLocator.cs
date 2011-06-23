/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:iTunesRemote.WindowsPhone"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using iTunesRemote.WindowsPhone.Service;

namespace iTunesRemote.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static MainViewModel _main;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
			if (ViewModelBase.IsInDesignModeStatic)
			{
				// Create design time services and viewmodels
				_main = new MainViewModel(null);
			}
			else
			{
				// Create run time services and view models
				//var model = new iTunesService("http://192.168.1.12:8081/");
				var model = new iTunesService("http://localhost:8080/");

				_main = new MainViewModel(model);
			}
        }

        /// <summary>
        /// Gets the Main property which defines the main viewmodel.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Library
        {
            get
            {
                return _main;
            }
        }
    }
}