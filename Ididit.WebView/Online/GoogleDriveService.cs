using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ididit.WebView.Online;

public class GoogleDriveService : IGoogleDriveService
{
    // If modifying these scopes, delete your previously saved "Google.Apis.Auth.OAuth2" folder
    private readonly string[] _scopes = { DriveService.Scope.DriveFile };

    private const string _applicationName = "ididit";

    public async Task<DriveService?> GetDriveService()
    {
        string baseDirectory = AppContext.BaseDirectory; // = AppDomain.CurrentDomain.BaseDirectory; // = Environment.CurrentDirectory; // = Directory.GetCurrentDirectory();
        string path = Path.Combine(baseDirectory, "credentials.json");

        if (!File.Exists(path))
            return null;

        UserCredential credential;

        using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
        {
            // The folder "Google.Apis.Auth.OAuth2" stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time
            string credPath = "Google.Apis.Auth.OAuth2";

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                _scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath)); // Defines whether the folder parameter is absolute or relative to
                                              // Environment.SpecialFolder.ApplicationData on Windows,
                                              // C:\Users\<user>\AppData\Roaming\
                                              // or $HOME on Linux and MacOS.
        }

        DriveService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });

        return service;
    }
}
