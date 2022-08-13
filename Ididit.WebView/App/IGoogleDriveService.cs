using Google.Apis.Drive.v3;
using System.Threading.Tasks;

namespace Ididit.WebView.App;

public interface IGoogleDriveService
{
    Task<DriveService?> GetDriveService();
}
