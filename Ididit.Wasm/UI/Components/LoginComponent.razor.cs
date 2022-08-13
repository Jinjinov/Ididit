using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

    [CascadingParameter]
    Task<AuthenticationState> GetAuthenticationStateAsync { get; set; } = null!;

    public async Task<string> GetUserDisplayName()
    {
        AuthenticationState authenticationState = await GetAuthenticationStateAsync;
        return authenticationState.User.Identity?.Name ?? string.Empty;
    }

    private async Task BeginSignOut(MouseEventArgs args)
    {
        await SignOutManager.SetSignOutState();
        Navigation.NavigateTo("authentication/logout");
    }
}
