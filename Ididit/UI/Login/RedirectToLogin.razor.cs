using Microsoft.AspNetCore.Components;
using System;

namespace Ididit.UI.Login;

public partial class RedirectToLogin
{
    [Inject]
    NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        Navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Navigation.Uri)}");
    }
}
