using Ididit.App;
using Ididit.Persistence;
using Ididit.WebView.App;
using Ididit.WebView.Persistence;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;

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
        builder.Services.AddScoped<IUserDisplayName, UserDisplayName>();
        builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

        return builder.Build();
	}
}
