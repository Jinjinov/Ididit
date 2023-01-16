using Blazorise.Localization;
using Ididit.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class Translations
{
    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Inject]
    IRepository Repository { get; set; } = null!;

    [Inject]
    ITextLocalizerService LocalizationService { get; set; } = null!;

    [Parameter]
    public EventCallback LanguageChanged { get; set; }

    readonly Dictionary<string, CultureInfo> _cultures = new()
    {
        { "English", new CultureInfo("en") },
        { "Deutsch", new CultureInfo("de") },
        { "slovenščina", new CultureInfo("sl") },
    };

    async Task OnCultureChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnCultureChanged(value);
    }

    async Task OnCultureChanged(string culture)
    {
        Repository.Settings.Culture = culture;
        await Repository.UpdateSettings(Repository.Settings.Id);

        LocalizationService.ChangeLanguage(culture);

        await LanguageChanged.InvokeAsync();
    }
}
