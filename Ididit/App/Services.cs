using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using DnetIndexedDb;
using Ididit.Backup;
using Ididit.Backup.Drive;
using Ididit.Backup.Online;
using Ididit.Data;
using Ididit.Data.Database;
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
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();

        serviceCollection.AddSingleton<IErrorBoundaryLogger, ErrorBoundaryLogger>();

        serviceCollection.AddScoped<IPreRenderService, PreRenderService>();

        serviceCollection.AddScoped<JsInterop>();

        serviceCollection.AddScoped<DirectoryBackup>();
        serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<DirectoryBackup>());

        serviceCollection.AddScoped<JsonBackup>();
        serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<JsonBackup>());
        serviceCollection.AddScoped<IFileImport>(x => x.GetRequiredService<JsonBackup>());

        serviceCollection.AddScoped<YamlBackup>();
        serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<YamlBackup>());
        serviceCollection.AddScoped<IFileImport>(x => x.GetRequiredService<YamlBackup>());

        serviceCollection.AddScoped<TsvBackup>();
        serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<TsvBackup>());
        serviceCollection.AddScoped<IFileImport>(x => x.GetRequiredService<TsvBackup>());

        serviceCollection.AddScoped<MarkdownBackup>();
        serviceCollection.AddScoped<IDataExport>(x => x.GetRequiredService<MarkdownBackup>());
        serviceCollection.AddScoped<IFileImport>(x => x.GetRequiredService<MarkdownBackup>());
        serviceCollection.AddScoped<IFileToString>(x => x.GetRequiredService<MarkdownBackup>());

        serviceCollection.AddScoped<GoogleKeepImport>();
        serviceCollection.AddScoped<IFileImport>(x => x.GetRequiredService<GoogleKeepImport>());
        serviceCollection.AddScoped<IFileToString>(x => x.GetRequiredService<GoogleKeepImport>());

        serviceCollection.AddScoped<IImportExport, ImportExport>();

        serviceCollection.AddScoped<IRepository, Repository>();
        serviceCollection.AddScoped<IDatabaseAccess, DatabaseAccess>();
        serviceCollection.AddScoped<IExamples, Examples>();

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
