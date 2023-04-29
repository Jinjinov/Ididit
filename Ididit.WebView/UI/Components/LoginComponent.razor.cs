using Ididit.App;
using Ididit.Backup.Online;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Ididit.WebView.UI.Components;

public partial class LoginComponent
{
    [Inject]
    IUserDisplayName UserDisplayName { get; set; } = null!;

    [Inject]
    IGoogleDriveBackup GoogleDriveBackup { get; set; } = null!;

    DateTime _modifiedTime;

    protected override async Task OnParametersSetAsync()
    {
        _modifiedTime = await GoogleDriveBackup.GetFileModifiedTime();
    }

    async Task LogIn(MouseEventArgs args)
    {
        await UserDisplayName.GetUserDisplayName();
    }

    void LogOut(MouseEventArgs args)
    {
        UserDisplayName.DisplayName = string.Empty;
    }
}
