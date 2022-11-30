using Ididit.App;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Ididit.WebView.UI.Components;

public partial class LoginComponent
{
    [Inject]
    IUserDisplayName UserDisplayName { get; set; } = null!;

    async Task LogIn(MouseEventArgs args)
    {
        await UserDisplayName.GetUserDisplayName();
    }

    void LogOut(MouseEventArgs args)
    {
        UserDisplayName.DisplayName = string.Empty;
    }
}
