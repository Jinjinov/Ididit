using Ididit.App.Data;
using Ididit.Online;
using Ididit.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class Options
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    async Task LoadExamples()
    {
        await Repository.LoadExamples();
    }

    async Task DeleteAll()
    {
        await Repository.DeleteAll();
    }

    [Parameter]
    public IList<string> Themes { get; set; } = null!;

    [Parameter]
    public Blazorise.Size Size { get; set; }

    [Parameter]
    public string Theme { get; set; } = null!;

    [Parameter]
    public EventCallback<Blazorise.Size> SizeChanged { get; set; }

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Size = size;
        await SizeChanged.InvokeAsync(Size);
    }

    async Task OnThemeChanged(string theme)
    {
        Theme = theme;
        await ThemeChanged.InvokeAsync(Theme);
    }

    [Inject]
    DirectoryBackup DirectoryBackup { get; set; } = null!;

    [Inject]
    JsonBackup JsonBackup { get; set; } = null!;

    [Inject]
    YamlBackup YamlBackup { get; set; } = null!;

    [Inject]
    TsvBackup TsvBackup { get; set; } = null!;

    [Inject]
    MarkdownBackup MarkdownBackup { get; set; } = null!;

    [Inject]
    GoogleKeepImport GoogleKeepImport { get; set; } = null!;

    [Inject]
    IGoogleDriveBackup GoogleDriveBackup { get; set; } = null!;

    async Task Import(InputFileChangeEventArgs e)
    {
        Stream stream = e.File.OpenReadStream(maxAllowedSize: 5242880);

        if (e.File.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            await JsonBackup.ImportData(stream);
        }
        else if (e.File.Name.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
        {
            await YamlBackup.ImportData(stream);
        }
        else if (e.File.Name.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase))
        {
            await TsvBackup.ImportData(stream);
        }
        else if (e.File.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            await GoogleKeepImport.ImportData(stream);
        }
        else if (e.File.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            await MarkdownBackup.ImportData(stream);
        }
    }

    async Task ExportJson()
    {
        await JsonBackup.ExportData();
    }

    async Task ExportYaml()
    {
        await YamlBackup.ExportData();
    }

    async Task ExportTsv()
    {
        await TsvBackup.ExportData();
    }

    async Task ExportMarkdown()
    {
        await MarkdownBackup.ExportData();
    }

    async Task ImportDirectory()
    {
        await DirectoryBackup.ImportData();
    }

    async Task ExportDirectory()
    {
        await DirectoryBackup.ExportData();
    }

    async Task ImportGoogleDrive()
    {
        await GoogleDriveBackup.ImportData();
    }

    async Task ExportGoogleDrive()
    {
        await GoogleDriveBackup.ExportData();
    }
}
