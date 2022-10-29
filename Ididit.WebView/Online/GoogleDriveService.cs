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
    // If modifying these scopes, delete your previously saved token.json/ folder
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
            // The file token.json stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time
            string credPath = Path.Combine(baseDirectory, "token.json");

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                _scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));

            Console.WriteLine("Credential file saved to: " + credPath);
        }

        DriveService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });

        return service;
    }
}
