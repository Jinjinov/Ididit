using Ididit.App;
using Ididit.Persistence;
using Ididit.WebView.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Ididit.WebView.App;

public static class Services
{
    public static IServiceCollection AddWebViewServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGoogleDriveService, GoogleDriveService>();
        serviceCollection.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();
        serviceCollection.AddScoped<IUserDisplayName, UserDisplayName>();

        return serviceCollection;
    }
}