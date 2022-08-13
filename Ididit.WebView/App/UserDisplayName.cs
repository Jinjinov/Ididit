using Google.Apis.Drive.v3;
using Ididit.App;
using System.Threading.Tasks;

namespace Ididit.WebView.App;

internal class UserDisplayName : IUserDisplayName
{
    readonly IGoogleDriveService _googleDriveService;

    public UserDisplayName(IGoogleDriveService googleDriveService)
    {
        _googleDriveService = googleDriveService;
    }

    public async Task<string> GetUserDisplayName()
    {
        DriveService? service = await _googleDriveService.GetDriveService();

        if (service is null)
            return string.Empty;

        AboutResource.GetRequest getRequest = service.About.Get();
        getRequest.Fields = "user";
        Google.Apis.Drive.v3.Data.About about = getRequest.Execute();

        return about.User.DisplayName;
    }
}
