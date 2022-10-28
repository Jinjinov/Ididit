using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using DnetIndexedDb;
using Ididit.App.Data;
using Ididit.Backup.Drive;
using Ididit.Backup.Online;
using Ididit.Database;
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
