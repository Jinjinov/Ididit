﻿using Blazorise.Localization;
using Ididit.App;
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

public partial class OptionsComponent
{
    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    public static bool IsPersonalComputer => OperatingSystem.IsBrowser() || OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    [Inject]
    IRepository Repository { get; set; } = null!;

    [Inject]
    IExamples Examples { get; set; } = null!;

    [Parameter]
    public CategoryModel SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    async Task LoadExamples()
    {
        await Examples.LoadExamples();

        await CloseOptions();
    }

    async Task DeleteAll()
    {
        await Repository.DeleteAll();

        await OnSelectedCategoryChanged();
        await CloseOptions();
    }

    async Task ResetSettings()
    {
        await Repository.ResetSettings();

        await CloseOptions();
    }

    private async Task CloseOptions()
    {
        Repository.Settings.Screen = Screen.Main;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    private async Task OnSelectedCategoryChanged()
    {
        SelectedCategory = Repository.Category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    private async Task SetMaxWidth(int? maxWidth)
    {
        Repository.Settings.MaxWidth = maxWidth;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    private async Task ClearMaxWidth()
    {
        Repository.Settings.MaxWidth = null;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Parameter]
    public EventCallback LanguageChanged { get; set; }

    [Parameter]
    public Blazorise.Size Size { get; set; }

    [Parameter]
    public EventCallback<Blazorise.Size> SizeChanged { get; set; }

    [Parameter]
    public IList<string> Themes { get; set; } = null!;

    [Parameter]
    public string Theme { get; set; } = null!;

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    [Parameter]
    public IList<string> Backgrounds { get; set; } = null!;

    [Parameter]
    public string Background { get; set; } = null!;

    [Parameter]
    public EventCallback<string> BackgroundChanged { get; set; }

    readonly Dictionary<Blazorise.Size, string> _buttonClassBySize = new()
        {
            { Blazorise.Size.Default, "" },
            { Blazorise.Size.ExtraSmall, "btn-sm" },
            { Blazorise.Size.Small, "btn-sm" },
            { Blazorise.Size.Medium, "btn-md" },
            { Blazorise.Size.Large, "btn-lg" },
            { Blazorise.Size.ExtraLarge, "btn-lg" }
        };

    public string ButtonSizeClass => _buttonClassBySize[Size];

    async Task OnSizeChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnSizeChanged(Enum.Parse<Blazorise.Size>(value));
    }

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Size = size;
        await SizeChanged.InvokeAsync(Size);
    }

    async Task OnThemeChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnThemeChanged(value);
    }

    async Task OnThemeChanged(string theme)
    {
        Theme = theme;
        await ThemeChanged.InvokeAsync(Theme);
    }

    async Task OnBackgroundChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnBackgroundChanged(value);
    }

    async Task OnBackgroundChanged(string background)
    {
        Background = background;
        await BackgroundChanged.InvokeAsync(Background);
    }

    [Inject]
    IImportExport ImportExport { get; set; } = null!;

    [Inject]
    DirectoryBackup DirectoryBackup { get; set; } = null!;

    [Inject]
    IGoogleDriveBackup GoogleDriveBackup { get; set; } = null!;

    const long _maxAllowedFileSize = 50 * 1024 * 1024; // 50 MB

    async Task Import(InputFileChangeEventArgs e)
    {
        if (ImportExport.FileImportByExtension.Where(pair => e.File.Name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase)).Select(pair => pair.Value).FirstOrDefault() is IFileImport fileImport)
        {
            Stream stream = e.File.OpenReadStream(maxAllowedSize: _maxAllowedFileSize);
            await fileImport.ImportData(stream);
            stream.Close();
        }

        await OnSelectedCategoryChanged();
        await CloseOptions();
    }

    [Parameter]
    public EventCallback<string> AdvancedInputTextChanged { get; set; }

    async Task ImportToString(InputFileChangeEventArgs e)
    {
        if (ImportExport.FileToStringByExtension.Where(pair => e.File.Name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase)).Select(pair => pair.Value).FirstOrDefault() is IFileToString fileToString)
        {
            Stream stream = e.File.OpenReadStream(maxAllowedSize: _maxAllowedFileSize);
            string fileString = await fileToString.GetString(stream);
            stream.Close();

            if (!string.IsNullOrEmpty(fileString))
            {
                await AdvancedInputTextChanged.InvokeAsync(fileString);
            }

            if (!Repository.Settings.ShowAdvancedInput)
            {
                Repository.Settings.ShowAdvancedInput = true;
                await Repository.UpdateSettings(Repository.Settings.Id);
            }
        }

        await CloseOptions();
    }

    async Task ExportData(DataFormat dataFormat)
    {
        await ImportExport.DataExportByFormat[dataFormat].ExportData();
    }

    async Task ImportDirectory()
    {
        await DirectoryBackup.ImportData();

        await CloseOptions();
    }

    async Task ImportGoogleDrive()
    {
        await GoogleDriveBackup.ImportData();

        await OnSelectedCategoryChanged();
        await CloseOptions();
    }
}
