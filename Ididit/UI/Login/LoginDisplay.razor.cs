using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

namespace Ididit.UI.Login;

public partial class LoginDisplay
{
    [Inject]
    NavigationManager Navigation { get; set; } = null!;

    [Inject]
    SignOutSessionStateManager SignOutManager { get; set; } = null!;

    private async Task BeginSignOut(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
}
