using System;
using System.Windows;

namespace Ididit.WebView.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            try
            {
                string? message = error.ExceptionObject.ToString();

                System.Diagnostics.Debug.WriteLine(message);

                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                System.IO.File.WriteAllText("Error.log", message);
            }
            catch
            {
            }
        };
    }
}
