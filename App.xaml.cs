using System;
using System.Windows;

namespace WordQuizApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize file storage on startup
            try
            {
                FileStorageHelper.InitializeStorage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing storage: {ex.Message}", "Storage Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

