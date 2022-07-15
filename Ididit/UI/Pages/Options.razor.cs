using Ididit.App;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.UI.Pages;

public partial class Options
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [Parameter]
    public IList<string> Themes { get; set; } = null!;

    Blazorise.Size Size => Repository.Settings.Size;

    string Theme => Repository.Settings.Theme;

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Repository.Settings.Size = size;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnThemeChanged(string theme)
    {
        Repository.Settings.Theme = theme;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }
}
