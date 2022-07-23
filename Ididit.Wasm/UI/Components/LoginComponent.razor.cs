using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

namespace Ididit.Wasm.UI.Components;

public partial class LoginComponent
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
