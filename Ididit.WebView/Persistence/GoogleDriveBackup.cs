using Google.Apis.Auth.OAuth2;
//using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Ididit.Data;
using Ididit.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ididit.WebView.Persistence;

// https://developers.google.com/drive/api/guides/folder#.net
// https://developers.google.com/drive/api/guides/manage-uploads#.net
// https://developers.google.com/drive/api/guides/manage-downloads#.net

public class GoogleDriveBackup : IGoogleDriveBackup
{
    // If modifying these scopes, delete your previously saved token.json/ folder
    readonly string[] _scopes = { DriveService.Scope.DriveFile };
    readonly string _applicationName = "ididit";
    readonly string _folderName = "ididit";

    public async Task<DataModel> ImportData()
    {

        return null;
    }

    public async Task ExportData(IDataModel data)
    {

    }

    async Task<MemoryStream?> DriveDownloadFile(string fileId)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return null;

        FilesResource.GetRequest request = service.Files.Get(fileId);
        MemoryStream stream = new();

        /*
        // Add a handler which will be notified on progress changes
        // It will notify on each chunk download and when the download is completed or failed
        request.MediaDownloader.ProgressChanged +=
            progress =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
        /**/

        request.Download(stream);

        return stream;
    }

    async Task<string> DriveCreateFolder(string name)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File fileMetadata = new()
        {
            Name = name,
            MimeType = "application/vnd.google-apps.folder"
        };

        FilesResource.CreateRequest request = service.Files.Create(fileMetadata);
        request.Fields = "id";
        Google.Apis.Drive.v3.Data.File file = request.Execute();

        return file.Id;
    }

    async Task<string> DriveUploadToFolder(string filePath, string fileName, string folderId)
    {
        if (!File.Exists(filePath))
            return string.Empty;

        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File fileMetadata = new()
        {
            Name = fileName,
            Parents = new List<string> { folderId }
        };

        FilesResource.CreateMediaUpload request;

        using (FileStream stream = new(filePath, FileMode.Open))
        {
            request = service.Files.Create(fileMetadata, stream, "application/json");
            request.Fields = "id";
            request.Upload();
        }

        Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

        return file.Id;
    }

    async Task<string> DriveUpload(string filePath, string fileName)
    {
        if (!File.Exists(filePath))
            return string.Empty;

        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File fileMetadata = new()
        {
            Name = fileName
        };

        FilesResource.CreateMediaUpload request;

        using (FileStream stream = new(filePath, FileMode.Open))
        {
            request = service.Files.Create(fileMetadata, stream, "application/json");
            request.Fields = "id";
            request.Upload();
        }

        Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

        return file.Id;
    }

    async Task<string> GetFolderId()
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 10; // Acceptable values are 1 to 1000, inclusive. (Default: 100)
        listRequest.Fields = "files(id, name)";
        listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{_folderName}'";

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        return files.Any() ? files.First().Id : string.Empty;
    }

    async Task<DriveService?> GetDriveService()
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
