using Ididit.Data;
using Ididit.Backup;
using Ididit.Backup.Drive;
using Ididit.Backup.Online;
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
    IImportExport ImportExport { get; set; } = null!;

    [Inject]
    DirectoryBackup DirectoryBackup { get; set; } = null!;

    [Inject]
    IGoogleDriveBackup GoogleDriveBackup { get; set; } = null!;

    async Task Import(InputFileChangeEventArgs e)
    {
        Stream stream = e.File.OpenReadStream(maxAllowedSize: 5242880);

        foreach (var pair in ImportExport.FileImportByExtension)
        {
            if (e.File.Name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
            {
                await pair.Value.ImportData(stream);
                break;
            }
        }
    }

    async Task ExportData(DataFormat dataFormat)
    {
        await ImportExport.DataExportByFormat[dataFormat].ExportData();
    }

    async Task ImportDirectory()
    {
        await DirectoryBackup.ImportData();
    }

    async Task ImportGoogleDrive()
    {
        await GoogleDriveBackup.ImportData();
    }
}
