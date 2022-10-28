using Ididit.App;
using Ididit.Backup.Online;
using Ididit.Wasm.Online;
using Microsoft.Extensions.DependencyInjection;

namespace Ididit.Wasm.App;

public static class Services
{
    public static IServiceCollection AddWasmServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGoogleDriveBackup, GoogleDriveBackup>();
        serviceCollection.AddScoped<IUserDisplayName, UserDisplayName>();

        serviceCollection.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.Authority = "https://accounts.google.com/";
            options.ProviderOptions.RedirectUri = "https://localhost:44333/authentication/login-callback";
            options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:44333/authentication/logout-callback";
            options.ProviderOptions.ClientId = "953393400208-sab1pb4ga5jeie0g50ft6uumf4uqa6in.apps.googleusercontent.com";

            options.ProviderOptions.ResponseType = "id_token token";

            //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive"); // See, edit, create, and delete all of your Google Drive files
            //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.appdata"); // See, create, and delete its own configuration data in your Google Drive
            options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.file"); // See, edit, create, and delete only the specific Google Drive files you use with this app
            //options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive.install"); // Connect itself to your Google Drive
        });

        return serviceCollection;
    }
}