using System;
using System.Threading.Tasks;
using System.Windows;
using AstroTrack.Data;

namespace AstroTrack.UI
{
    /// <summary>  
    /// Interaction logic for App.xaml  
    /// </summary>  
    public partial class App : Application
    {
        /// <summary>  
        /// On startup, preload all astronomical events into the local database,  
        /// then open the main window.  
        /// </summary>  
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Define observer location (e.g., Paris)  
            const double latitude = 48.8566;
            const double longitude = 2.3522;
            const double elevation = 35;

            try
            {
                // Asynchronously initialize database and fetch events for all bodies  
                await DataService.Instance.InitializeAsync(latitude, longitude, elevation);
            }
            catch (Exception ex)
            {
                // Show error and exit if initialization fails  
                MessageBox.Show(
                    $"Failed to load astronomical data:\n{ex.Message}",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
                return;
            }

            // Launch the main window after data sync is complete  
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
