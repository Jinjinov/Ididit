using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ididit.WebView.App;

public class GoogleDriveService : IGoogleDriveService
{
    // If modifying these scopes, delete your previously saved token.json/ folder
    private readonly string[] _scopes = { DriveService.Scope.DriveFile };

    private const string _applicationName = "ididit";

    public async Task<DriveService?> GetDriveService()
    {
        if (!File.Exists("credentials.json"))
            return null;

        UserCredential credential;

        using (FileStream stream = new("credentials.json", FileMode.Open, FileAccess.Read))
        {
            // The file token.json stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time
            string credPath = "token.json";

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

    /*
    DriveService GetDriveService()
    {
        // Load pre-authorized user credentials from the environment
        // TODO(developer) - See https://developers.google.com/identity for guides on implementing OAuth2 for your application
        GoogleCredential credential = GoogleCredential.GetApplicationDefault().CreateScoped(DriveService.Scope.Drive);

        DriveService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });

        return service;
    }
    /**/

    /*
    DriveService GetDriveService()
    {
        GoogleDrive credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDrive>(System.IO.File.ReadAllText(@"\\PATH_TO_JSONFILE\GoogleAPI.json"));

        ServiceAccountCredential credential = new(new ServiceAccountCredential.Initializer(credentials.client_email)
        {
            User = "xxx@xxx.xxx",
            Scopes = _scopes
        }.FromPrivateKey(credentials.private_key));

        DriveService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });

        return service;
    }
    /**/
}
