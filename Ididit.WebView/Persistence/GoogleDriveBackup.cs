using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Ididit.Data;
using Ididit.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ididit.WebView.Persistence;

public class GoogleDriveBackup : IGoogleDriveBackup
{
    // If modifying these scopes, delete your previously saved token.json/ folder
    readonly string[] _scopes = { DriveService.Scope.DriveFile };
    readonly string _applicationName = "ididit";
    readonly string _folderName = "ididit";

    public async Task<DataModel> ImportData()
    {
        // https://developers.google.com/drive/api/guides/manage-downloads#.net

        return null;
    }

    public void ExportData(IDataModel data)
    {
        // https://stackoverflow.com/questions/60774277/check-if-folder-exist-in-google-drive-by-folder-name-c

        // https://stackoverflow.com/questions/47136576/google-drive-api-v3-c-net-searching-folders-files-by-title-throws-requesterr

        // https://stackoverflow.com/questions/72733152/google-drive-api-search-by-file-name-only-finds-the-file-if-it-has-recently-be

        // https://developers.google.com/drive/api/guides/folder#.net

        // https://developers.google.com/drive/api/guides/manage-uploads#.net
    }

    public MemoryStream DriveDownloadFile(string fileId)
    {
        try
        {
            // Load pre-authorized user credentials from the environment

            // TODO(developer) - See https://developers.google.com/identity for guides on implementing OAuth2 for your application

            GoogleCredential credential = GoogleCredential.GetApplicationDefault().CreateScoped(DriveService.Scope.Drive);

            DriveService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });

            FilesResource.GetRequest request = service.Files.Get(fileId);
            MemoryStream stream = new();

            // Add a handler which will be notified on progress changes

            // It will notify on each chunk download and when the download is completed or failed

            request.MediaDownloader.ProgressChanged +=
                progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                Console.WriteLine("Download complete.");
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };

            request.Download(stream);

            return stream;
        }
        catch (Exception e)
        {
            // TODO(developer) - handle error appropriately
            if (e is AggregateException)
            {
                Console.WriteLine("Credential Not found");
            }
            else
            {
                throw;
            }
        }

        return null;
    }

    public string DriveCreateFolder(string name)
    {
        try
        {
            // Load pre-authorized user credentials from the environment

            // TODO(developer) - See https://developers.google.com/identity for guides on implementing OAuth2 for your application

            GoogleCredential credential = GoogleCredential.GetApplicationDefault().CreateScoped(DriveService.Scope.Drive);

            DriveService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });

            Google.Apis.Drive.v3.Data.File fileMetadata = new()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder"
            };

            FilesResource.CreateRequest request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            Google.Apis.Drive.v3.Data.File file = request.Execute();

            Console.WriteLine("Folder ID: " + file.Id);

            return file.Id;
        }
        catch (Exception e)
        {
            // TODO(developer) - handle error appropriately
            if (e is AggregateException)
            {
                Console.WriteLine("Credential Not found");
            }
            else
            {
                throw;
            }
        }

        return null;
    }

    public Google.Apis.Drive.v3.Data.File DriveUploadToFolder(string filePath, string fileName, string folderId)
    {
        try
        {
            // Load pre-authorized user credentials from the environment

            // TODO(developer) - See https://developers.google.com/identity for guides on implementing OAuth2 for your application

            GoogleCredential credential = GoogleCredential.GetApplicationDefault().CreateScoped(DriveService.Scope.Drive);

            DriveService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });

            Google.Apis.Drive.v3.Data.File fileMetadata = new()
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            FilesResource.CreateMediaUpload request;

            using (FileStream stream = new(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }

            Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

            Console.WriteLine("File ID: " + file.Id);

            return file;
        }
        catch (Exception e)
        {
            // TODO(developer) - handle error appropriately
            if (e is AggregateException)
            {
                Console.WriteLine("Credential Not found");
            }
            else if (e is FileNotFoundException)
            {
                Console.WriteLine("File not found");
            }
            else if (e is DirectoryNotFoundException)
            {
                Console.WriteLine("Directory Not found");
            }
            else
            {
                throw;
            }
        }

        return null;
    }

    public string DriveUploadBasic(string filePath, string fileName)
    {
        try
        {
            // Load pre-authorized user credentials from the environment

            // TODO(developer) - See https://developers.google.com/identity for guides on implementing OAuth2 for your application

            GoogleCredential credential = GoogleCredential.GetApplicationDefault().CreateScoped(DriveService.Scope.Drive);

            DriveService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });

            // Upload file photo.jpg on drive.
            Google.Apis.Drive.v3.Data.File fileMetadata = new()
            {
                Name = fileName
            };

            FilesResource.CreateMediaUpload request;

            using (FileStream stream = new(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }

            Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

            Console.WriteLine("File ID: " + file.Id);

            return file.Id;
        }
        catch (Exception e)
        {
            // TODO(developer) - handle error appropriately
            if (e is AggregateException)
            {
                Console.WriteLine("Credential Not found");
            }
            else if (e is FileNotFoundException)
            {
                Console.WriteLine("File not found");
            }
            else
            {
                throw;
            }
        }

        return null;
    }

    public async Task Login()
    {
        try
        {
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

            FilesResource.ListRequest listRequest = service.Files.List();
            //listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{_folderName}'";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

            if (files == null || files.Count == 0)
            {
                return;
            }

            foreach (Google.Apis.Drive.v3.Data.File file in files)
            {
                Console.WriteLine("{0} ({1})", file.Name, file.Id);
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static string CreateFolder(DriveService service, string folderName)
    {
        Google.Apis.Drive.v3.Data.File newFile = new() { Name = folderName, MimeType = "application/vnd.google-apps.folder" };

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 10;
        listRequest.Fields = "nextPageToken, files(id, name)";

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        if (files != null && files.Count > 0)
        {
            foreach (Google.Apis.Drive.v3.Data.File file in files)
            {
                if (file.Name == newFile.Name)
                {
                    Console.WriteLine("File already existing... Skip creation");
                    return file.Id;
                }
            }
        }
        else
        {
            Console.WriteLine("No files found.");
        }

        Console.WriteLine("Creating new file...");

        Google.Apis.Drive.v3.Data.File result = service.Files.Create(newFile).Execute();

        return result.Id;
    }

    /*
    public static void DownloadFile(string filename)
    {
        static DriveService GetDriveService()
        {
            string[] scopes = new string[] { DriveService.Scope.Drive }; // Full access

            GoogleDrive cr = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDrive>(System.IO.File.ReadAllText(@"\\PATH_TO_JSONFILE\GoogleAPI.json"));

            ServiceAccountCredential xCred = new(new ServiceAccountCredential.Initializer(cr.client_email)
            {
                User = "xxxxx@xxxx.xx",
                Scopes = new[] { DriveService.Scope.Drive }
            }.FromPrivateKey(cr.private_key));

            DriveService service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = xCred,
                ApplicationName = "APPLICATION_NAME",
            });

            return service;
        }

        DriveService service = GetDriveService();

        //check if file exists and grab id 
        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.SupportsAllDrives = true;
        listRequest.IncludeItemsFromAllDrives = true;
        listRequest.PageSize = 1000;
        listRequest.Q = "name = '" + filename + ".pdf'";
        Google.Apis.Drive.v3.Data.FileList files = listRequest.Execute();

        if (files.Files.Count > 0) // the file exists, DOWNLOAD
        {
            FilesResource.GetRequest request = service.Files.Get(files.Files[0].Id);
            MemoryStream stream = new();
            request.Download(stream);
        }
    }
    /**/
}
