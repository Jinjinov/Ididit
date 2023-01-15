using Blazorise;
using Blazorise.Localization;
using Ididit.App;
using Ididit.Backup;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class MainComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    //[Inject]
    //IUserDisplayName UserDisplayName { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    [Inject]
    protected IPreRenderService PreRenderService { get; set; } = null!;

    CategoryModel _selectedCategory = new();

    [Inject]
    ITextLocalizerService LocalizationService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (!PreRenderService.IsPreRendering)
            await Repository.Initialize();

        Repository.DataChanged += (object? sender, EventArgs e) => StateHasChanged();

        _selectedCategory = Repository.Category;

        LocalizationService.ChangeLanguage(Repository.Settings.Culture);

        StateHasChanged(); // refresh components with _repository.Settings
    }

    [Inject]
    IImportExport ImportExport { get; set; } = null!;

    SettingsModel Settings => Repository.Settings;

    bool UnsavedChanges => ImportExport.DataExportByFormat[Settings.SelectedBackupFormat].UnsavedChanges;

    async Task Backup()
    {
        await ImportExport.DataExportByFormat[Settings.SelectedBackupFormat].ExportData();
    }

    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    public static bool IsPersonalComputer => OperatingSystem.IsBrowser() || OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    public static bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    readonly SortedList<string, string> _bootswatchThemes = new()
    {
        { "cerulean", "sha256-5FeWJNHtC2PdIw9W39dEuOyQN1BoL7BuHR3JdbpIzbw=" },
        { "cosmo", "sha256-iMtue7kH6spfqTbfU9XGcwtBF8bgiOnVTErTuh7LJr8=" },
        { "cyborg", "sha256-fO58jx4RDvdVgLJ4VWCNdWLLQF5cXb34EtdoGxlcJ68=" },
        { "darkly", "sha256-VZi/r/RC1MritcGE2Yyxb/ACi8WIOj1Y7BHuslF8+6I=" },
        { "flatly", "sha256-3LXKhyYmYxt+fGciLxN474K5Ycw5FXqQJDJpW54Q3XQ=" },
        { "journal", "sha256-Ont0XKIPs4G8F5vswFwCkN5t7KOSd8L8ifYhadEfEvk=" },
        { "litera", "sha256-oNAzP5red1ldRKkdYgkxkykk2qXm3hcosesq9UGvN4o=" },
        { "lumen", "sha256-27KHRZR9ihYCkP9BSm5cLLqJ4LsSZdLEYgKh5j2lrCs=" },
        { "lux", "sha256-WW13HpaaG94O2RHAP6ZIIEcijhqdeYjh3FkqE7zgMbY=" },
        { "materia", "sha256-I1/fNjCD26D0FEPPH2ox/AMQo4owDy1DsUkCJ5e/Ud4=" },
        { "minty", "sha256-X08VWhrLbfhaM0zE3n7Q7Mg9YVevZcIBFzpvSCWAWmo=" },
        { "morph", "sha256-1Wlk5rRLkqkcplEElHjnc+x3zrJ4qZRjzDxzAtI8H48=" }, /**/
        { "pulse", "sha256-d3j6nPvgdSos3dIAmJSHebf76C5yAALgWwcHIom40Mo=" },
        { "quartz", "sha256-GpjV2saTPcbYTZy+LZLbu2JpmSQfGJW7XE5V5EDdA/g=" }, /**/
        { "sandstone", "sha256-zWAnZkKmT2MYxdCMp506rQtnA9oE2w0/K/WVU7V08zw=" },
        { "simplex", "sha256-bFdwuvWKVAaFL6MZ6IlwACEx5uGox0TibRPTZstTN9o=" },
        { "sketchy", "sha256-H4KK1tCvREdvbtMG+OoveMdEkIsulg1bO5bJJpEBRyc=" },
        { "slate", "sha256-tuuKR9RAif6+FEcxArLfMAVcEfuamZw2J/dR9F5svcw=" },
        { "solar", "sha256-L3GhaXImktQTiaUA8LmRQ5W9/qn7eU2b+k7gSWn8U/A=" },
        { "spacelab", "sha256-gvYVyQ50XH5efNIn43UNuSOp7LhOePci95PJAAIfpek=" },
        { "superhero", "sha256-gox/GuMWCKC24lM1gRLnKpm/pgjHDI3u5bnjSxvC/QI=" },
        { "united", "sha256-rU1IToLlw3oMuNHSO29CP/HxYCcBqq9Vc73wVnN5prQ=" },
        { "vapor", "sha256-3s3k/1ZjojJ7E7SfJk0q/A4fe/weYY9gzvbE6C6JTI0=" }, /**/
        { "yeti", "sha256-g2YbprSKIubjkv6Pkd3RSeH9/6MW2MZcFrX/3eIu6vU=" },
        { "zephyr", "sha256-DWNfAPc9h5qI0aI4+9aPONXiE9jIz2nczDB3KvjiKlI=" } /**/
    };

    Blazorise.Size Size => Repository.Settings.Size;

    string Theme => Repository.Settings.Theme;

    int _workaround;

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Repository.Settings.Size = size;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnThemeChanged(string theme)
    {
        _workaround = 1 - _workaround;

        Repository.Settings.Theme = theme;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnBackgroundChanged(string background)
    {
        Repository.Settings.Background = background;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    int _debugTheme = 0;
    async Task DebugTheme()
    {
        ++_debugTheme;

        if (_debugTheme == _bootswatchThemes.Count)
        {
            await OnThemeChanged("default");
            return;
        }

        if (_debugTheme > _bootswatchThemes.Count)
        {
            _debugTheme = 0;
        }

        await OnThemeChanged(_bootswatchThemes.Keys[_debugTheme]);
    }

    Background _background
    {
        get
        {
            if (_backgrounds.ContainsKey(Repository.Settings.Background))
                return _backgrounds[Repository.Settings.Background];
            else
                return Background.Default;
        }
    }

    readonly SortedList<string, Background> _backgrounds = new()
    {
        { "Default", Background.Default },
        { "Primary", Background.Primary },
        { "Secondary", Background.Secondary },
        { "Success", Background.Success },
        { "Danger", Background.Danger },
        { "Warning", Background.Warning },
        { "Info", Background.Info },
        { "Light", Background.Light },
        { "Dark", Background.Dark },
        { "White", Background.White },
        { "Transparent", Background.Transparent },
        { "Body", Background.Body },
    };

    int _debugBackground = 0;
    async Task DebugBackground()
    {
        if (++_debugBackground == _backgrounds.Count)
            _debugBackground = 0;

        Repository.Settings.Background = _backgrounds.Keys[_debugBackground];
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    bool _showDebugControls;

    void OnShowDebugChanged(bool? val) => _showDebugControls = val ?? false;

    Screen Screen => Repository.Settings.Screen;

    async Task ToggleOptions()
    {
        if (Screen == Screen.Options)
            Repository.Settings.Screen = Screen.Main;
        else
            Repository.Settings.Screen = Screen.Options;

        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task ToggleHelp()
    {
        if (Screen == Screen.Help)
            Repository.Settings.Screen = Screen.Main;
        else
            Repository.Settings.Screen = Screen.Help;

        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task ToggleAbout()
    {
        if (Screen == Screen.About)
            Repository.Settings.Screen = Screen.Main;
        else
            Repository.Settings.Screen = Screen.About;

        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task ShowMainScreen()
    {
        Repository.Settings.Screen = Screen.Main;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnDataFormatChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnDataFormatChanged(Enum.Parse<DataFormat>(value));
    }

    async Task OnDataFormatChanged(DataFormat dataFormat)
    {
        if (Settings.SelectedBackupFormat != dataFormat)
        {
            Settings.SelectedBackupFormat = dataFormat;

            await Repository.UpdateSettings(Settings.Id);
        }
    }

    bool _filtersVisible = true;
    bool _categoriesVisible = true;

    bool _sidebarVisible = false;
    string? SidebarVisibilityCss => _sidebarVisible ? null : "sidebar-visible";

    void ToggleSidebar() => _sidebarVisible = !_sidebarVisible;

    Filters _filters = new();
}