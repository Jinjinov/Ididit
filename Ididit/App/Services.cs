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
        serviceCollection.AddScoped<MarkdownBackup>();

        serviceCollection.AddScoped<Theme>();

        serviceCollection.AddScoped<IRepository, Repository>();
        serviceCollection.AddScoped<IDatabaseAccess, DatabaseAccess>();

        serviceCollection.AddIndexedDbDatabase<IndexedDb>(options =>
        {
            options.UseDatabase(IndexedDb.GetDatabaseModel());
        });

        return serviceCollection;
    }

    public static IServiceProvider UseServices(this IServiceProvider serviceProvider)
    {
        //IRepository repository = serviceProvider.GetRequiredService<IRepository>();
        //IDatabaseAccess databaseAccess = serviceProvider.GetRequiredService<IDatabaseAccess>();

        // Works only in Wasm - doesn't work in WinForms or Wpf:

        //databaseAccess.Load(repository);

        return serviceProvider;
    }
}
