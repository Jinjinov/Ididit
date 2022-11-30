using Ididit.App;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Ididit.Wasm.App;

public class UserDisplayName : IUserDisplayName
{
    public string DisplayName { get; set; } = string.Empty;

    readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserDisplayName(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string> GetUserDisplayName()
    {
        AuthenticationState authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        DisplayName = authenticationState.User.Identity?.Name ?? string.Empty;

        return DisplayName;
    }
}
