using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DnetIndexedDb;
using Ididit.Database;
using Ididit.Persistence;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ididit.App;

public static class Services
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddBlazorise(options =>
            {
                options.Immediate = true; // ChangeTextOnKeyPress
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        serviceCollection.AddSingleton<IErrorBoundaryLogger, ErrorBoundaryLogger>();

        serviceCollection.AddScoped<JsInterop>();

        serviceCollection.AddScoped<DirectoryBackup>();
        serviceCollection.AddScoped<GoogleDriveBackup>();
        serviceCollection.AddScoped<JsonBackup>();
        serviceCollection.AddScoped<YamlBackup>();
        serviceCollection.AddScoped<TsvBackup>();
        serviceCollection.AddScoped<MarkdownBackup>();
        serviceCollection.AddScoped<GoogleKeepImport>();

        serviceCollection.AddScoped<IRepository, Repository>();
        serviceCollection.AddScoped<IDatabaseAccess, DatabaseAccess>();

        serviceCollection.AddIndexedDbDatabase<IndexedDb>(options =>
        {
            options.UseDatabase(IndexedDb.GetDatabaseModel());
        });

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

    public static IServiceProvider UseServices(this IServiceProvider serviceProvider)
    {
        //IRepository repository = serviceProvider.GetRequiredService<IRepository>();

        // Works only in Wasm - doesn't work in WinForms or Wpf:

        //repository.Initialize();

        return serviceProvider;
    }
}
