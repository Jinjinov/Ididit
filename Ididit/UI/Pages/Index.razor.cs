using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.UI.Pages;

public partial class Index
{
    readonly SortedList<string, string> _bootswatchThemes = new()
    {
        { "cerulean", "sha256-RW/ojwB9sS6DzIfFEaDiahxVPN/MupUlrbKrKVkgA9M=" },
        { "cosmo", "sha256-/H81OfRt8XcW5i6dfB86YU8k0XTUbj2ezDqejIrXWs4=" },
        { "cyborg", "sha256-3FdPvyIyBF62iTAVS30syY0eik+vm4G1fiCPErB7IbQ=" },
        { "darkly", "sha256-0tZXz4Wl0mk302PbATyRKjsHT1tI5FwnK5gY8DB5SeY=" },
        { "flatly", "sha256-K0Q0THV+T7huqMhZI87+P0A/uy+tbcyC751i8ScYytw=" },
        { "journal", "sha256-b9ANi+bPfqDaEOG9y/CZ2+cs5WlO6MdZ85ZEDwp99aw=" },
        { "litera", "sha256-YwRe9tp/LgR/N/G0wb0GXHfupruWc0Pf6Juel/cY+G8=" },
        { "lumen", "sha256-jlJkgYQWWRqOYNoAJrtSPAgJ8XzAB5oW8G8/ovdz/x0=" },
        { "lux", "sha256-v2HsAdJ0L9YjVjheYom4BSraR+nwJDgYj6wI73QIedI=" },
        { "materia", "sha256-MEhiSVHcxsJGoCn/OncBKfjYzl8Z+gHOA/TJ0s4oTP8=" },
        { "minty", "sha256-dH/ZJzNkhrTqLl30dAJ0pIDbuNWlF2Fht9I2Q8jhrcs=" },
        { "pulse", "sha256-1vQy0iTuGRchdjqh8TwEkCeFn81PyVt9KofS69uQxKs=" },
        { "sandstone", "sha256-Wj8Vq1ER7uigVDcLe8Kfwxu4ieW867kr9HQihUUfnQA=" },
        { "simplex", "sha256-Xsqt9TSw+DbYFdZSxbG/pMRF/JLdDeG6xFFRU3JwWxs=" },
        { "sketchy", "sha256-tT30p4dCoDHNDmxveHbPJQSFVn/7a7pDBqplXFNWff4=" },
        { "slate", "sha256-qdwWE5kCHBJrfGXA4BRFz6ixKw9PCpaYzJWA9260DzY=" },
        { "solar", "sha256-eeFZTjMs+Fz5lW8l5wrcJztSyJmQNRFu9FtwqSDOcaY=" },
        { "spacelab", "sha256-VdSljf9GE8Znxpt5oe70r3Yffh2TuRl1m+vMztZP2/4=" },
        { "superhero", "sha256-ADi3Xne8BjmmGG8K4lQWmU7SegWM4B2xWeapboEdA18=" },
        { "united", "sha256-7gdx77MsYTQV0shn+8C0CP7lc6xQRexoNubpaigHL6c=" },
        { "yeti", "sha256-hqMfmPlwrJpRN1EanXFcujx+vGhMfBbeKVyT09X3mIw=" }
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

    bool _showOptions;

    void ToggleOptions() => _showOptions = !_showOptions;

    [Inject]
    IRepository Repository { get; set; } = null!;

    CategoryModel? _selectedCategory;

    bool _filtersVisible = true;
    bool _categoriesVisible = true;

    bool _sidebarVisible = false;
    string? SidebarVisibilityCss => _sidebarVisible ? null : "sidebar-visible";

    void ToggleSidebar() => _sidebarVisible = !_sidebarVisible;

    Filters _filters = new();

    protected override async Task OnInitializedAsync()
    {
        _filters = new()
        {
            Sort = Repository.Settings.Sort,
            ElapsedToDesiredRatioMin = Repository.Settings.ElapsedToDesiredRatioMin,
            ShowElapsedToDesiredRatioOverMin = Repository.Settings.ShowElapsedToDesiredRatioOverMin,
            HideEmptyGoals = Repository.Settings.HideEmptyGoals,
            ShowCategoriesInGoalList = Repository.Settings.ShowCategoriesInGoalList,
            AlsoShowCompletedAsap = Repository.Settings.AlsoShowCompletedAsap,
        };

        await Repository.Initialize();

        StateHasChanged(); // refresh components with _repository.Settings
    }
}
