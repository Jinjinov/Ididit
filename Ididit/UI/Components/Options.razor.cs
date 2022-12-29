using Ididit.Backup;
using Ididit.Backup.Drive;
using Ididit.Backup.Online;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class Options
{
    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    public static bool IsPersonalComputer => OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    [Inject]
    IRepository Repository { get; set; } = null!;

    [Parameter]
    public CategoryModel SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    async Task LoadExamples()
    {
        await Repository.LoadExamples();
    }

    async Task DeleteAll()
    {
        await Repository.DeleteAll();

        await OnSelectedCategoryChanged();
    }

    private async Task OnSelectedCategoryChanged()
    {
        SelectedCategory = Repository.Category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
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

    async Task OnSizeChangeEvent(ChangeEventArgs e)
    {
        await OnSizeChanged(Enum.Parse<Blazorise.Size>((string)e.Value));
    }

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Size = size;
        await SizeChanged.InvokeAsync(Size);
    }

    async Task OnThemeChangeEvent(ChangeEventArgs e)
    {
        await OnThemeChanged((string)e.Value);
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
        if (ImportExport.FileImportByExtension.Where(pair => e.File.Name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase)).Select(pair => pair.Value).FirstOrDefault() is IFileImport fileImport)
        {
            Stream stream = e.File.OpenReadStream(maxAllowedSize: 5242880);
            await fileImport.ImportData(stream);
            stream.Close();
        }

        await OnSelectedCategoryChanged();
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

        await OnSelectedCategoryChanged();
    }
}
