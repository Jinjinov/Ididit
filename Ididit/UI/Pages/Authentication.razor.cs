using Microsoft.AspNetCore.Components;

namespace Ididit.UI.Pages;

public partial class Authentication
{
    [Inject]
    NavigationManager Navigation { get; set; } = null!;

    [Parameter]
    public string Action { get; set; } = null!;

    public void OnLogOutSucceeded()
    {
        Navigation.NavigateTo("/");
    }

    protected override void OnParametersSet()
    {
        if (Action == "logged-out")
            Navigation.NavigateTo("/");
    }
}
