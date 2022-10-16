using Google.Apis.Drive.v3;
using System.Threading.Tasks;

namespace Ididit.WebView.Online;

public interface IGoogleDriveService
{
    Task<DriveService?> GetDriveService();
}
