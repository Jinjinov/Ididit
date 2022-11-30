using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Ididit.App;
using Ididit.WebView.Online;
using System.Threading.Tasks;

namespace Ididit.WebView.App;

public class UserDisplayName : IUserDisplayName
{
    public string DisplayName { get; set; } = string.Empty;

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
        About about = getRequest.Execute();

        DisplayName = about.User.DisplayName;

        return DisplayName;
    }
}
