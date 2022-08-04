using Google.Apis.Auth.OAuth2;
//using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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

public class GoogleDriveBackup : GoogleDriveBase, IGoogleDriveBackup
{
    // If modifying these scopes, delete your previously saved token.json/ folder
    readonly string[] _scopes = { DriveService.Scope.DriveFile };
    const string _applicationName = "ididit";

    protected override async Task<string> GetFile(string fileId)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

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

        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        return text;
    }

    protected override async Task<string> CreateFolder()
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File fileMetadata = new()
        {
            Name = _folderName,
            Description = _folderDescription,
            MimeType = _folderMimeType
        };

        FilesResource.CreateRequest request = service.Files.Create(fileMetadata);
        request.Fields = "id";
        Google.Apis.Drive.v3.Data.File file = request.Execute();

        return file.Id;
    }

    protected override async Task<string> CreateFile(string folderId, string content)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File fileMetadata = new()
        {
            Name = _fileName,
            Description = _fileDescription,
            MimeType = _fileMimeType,
            Parents = new List<string> { folderId }
        };

        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        FilesResource.CreateMediaUpload request = service.Files.Create(fileMetadata, stream, _fileMimeType);
        request.Fields = "id";
        request.Upload();

        Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

        return file.Id;
    }

    protected override async Task<string> UpdateFile(string fileId, string content)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        Google.Apis.Drive.v3.Data.File file = service.Files.Get(fileId).Execute();

        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        FilesResource.UpdateMediaUpload request = service.Files.Update(file, fileId, stream, _fileMimeType);
        request.Fields = "id";
        request.Upload();

        Google.Apis.Drive.v3.Data.File updatedFile = request.ResponseBody;

        return updatedFile.Id;
    }

    protected override async Task<string> GetFolderId()
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 10; // Acceptable values are 1 to 1000, inclusive. (Default: 100)
        listRequest.Fields = "files(id, name)";
        listRequest.Q = $"name = '{_folderName}' and mimeType = '{_folderMimeType}'";

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        return files.Any() ? files.First().Id : string.Empty;
    }

    protected override async Task<string> GetFileId(string folderId)
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 10; // Acceptable values are 1 to 1000, inclusive. (Default: 100)
        listRequest.Fields = "files(id, name)";
        listRequest.Q = $"name = '{_fileName}' and '{folderId}' in parents";

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

    public async Task<string> GetUserDisplayName()
    {
        DriveService? service = await GetDriveService();

        if (service is null)
            return string.Empty;

        AboutResource.GetRequest getRequest = service.About.Get();
        getRequest.Fields = "user";
        Google.Apis.Drive.v3.Data.About about = getRequest.Execute();

        return about.User.DisplayName;
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
