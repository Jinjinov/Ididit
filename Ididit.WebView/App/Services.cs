using Ididit.App;
using Ididit.Backup;
using Ididit.Backup.Online;
using Ididit.WebView.Online;
using Microsoft.Extensions.DependencyInjection;

namespace Ididit.WebView.App;

public static class Services
{
    public static IServiceCollection AddWebViewServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGoogleDriveService, GoogleDriveService>();
        serviceCollection.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();

        if (GoogleDriveBase.IsGoogleDriveAvailable)
        {
            serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<IGoogleDriveBackup>());
        }

        serviceCollection.AddScoped<IUserDisplayName, UserDisplayName>();

        return serviceCollection;
    }
}