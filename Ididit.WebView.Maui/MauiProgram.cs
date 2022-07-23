using Ididit.App;
using Ididit.Persistence;
using Ididit.WebView.Persistence;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Ididit.WebView.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder.UseMauiApp<App>();

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        builder.Services.AddServices();
		builder.Services.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();

		return builder.Build();
	}
}
