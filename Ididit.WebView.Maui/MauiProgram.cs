using Ididit.App;
using Ididit.WebView.App;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace Ididit.WebView.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            try
            {
                string? message = error.ExceptionObject.ToString();

                System.Diagnostics.Debug.WriteLine(message);

                Application.Current?.Dispatcher.Dispatch(async () =>
                {
                    if (Application.Current.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                });

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ididit", "Error.log");
                File.WriteAllText(path, message);
            }
            catch
            {
            }
        };

        var builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>();

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        builder.Services.AddServices();
		builder.Services.AddWebViewServices();

        return builder.Build();
	}
}
