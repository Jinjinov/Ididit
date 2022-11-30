using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ididit.WebView.Online;

public partial class GoogleDriveService : IGoogleDriveService
{
    // If modifying these scopes, delete your previously saved "Ididit" folder
    private readonly string[] _scopes = { DriveService.Scope.DriveFile };

    private const string _applicationName = "ididit";

    private static GoogleClientSecrets? GetGoogleClientSecrets()
    {
        // D:\Jinjinov\Ididit\Ididit.WebView.Maui\bin\Debug\net6.0-windows10.0.19041.0\win10-x64\AppX\
        // D:\Jinjinov\Ididit\Ididit.WebView.Wpf\bin\Debug\net6.0-windows\
        // "/media/sf_Jinjinov/Ididit/Ididit.WebView.Photino/bin/Debug/net6.0/"
        // "/Users/Urban/Projects/Ididit/Ididit.WebView.Maui/bin/Debug/net6.0-maccatalyst/maccatalyst-x64/Ididit.WebView.Maui.app/Contents/MonoBundle"
        string baseDirectory = AppContext.BaseDirectory;

        /*
        baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        These days (.NET Core, .NET Standard 1.3+ or .NET Framework 4.6+) it's better to use AppContext.BaseDirectory rather than AppDomain.CurrentDomain.BaseDirectory. 
        Both are equivalent, but multiple AppDomains are no longer supported. https://learn.microsoft.com/en-us/dotnet/core/porting/net-framework-tech-unavailable

        // C:\WINDOWS\system32
        // D:\Jinjinov\Ididit\Ididit.WebView.Wpf\bin\Debug\net6.0-windows
        // "/media/sf_Jinjinov/Ididit"
        // "/Users/Urban/Projects/Ididit/Ididit.WebView.Maui/bin/Debug/net6.0-maccatalyst/maccatalyst-x64/Ididit.WebView.Maui.app"
        baseDirectory = Environment.CurrentDirectory;
        baseDirectory = Directory.GetCurrentDirectory(); // public static string GetCurrentDirectory() => Environment.CurrentDirectory;
        /**/

        while (!File.Exists(Path.Combine(baseDirectory, "credentials.json")))
        {
            string? parent = Directory.GetParent(baseDirectory)?.FullName; // macOS work-around

            if (parent == null)
                return null;
            else
                baseDirectory = parent;
        }

        string path = Path.Combine(baseDirectory, "credentials.json");

        GoogleClientSecrets googleClientSecrets;

        using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
        {
            googleClientSecrets = GoogleClientSecrets.FromStream(stream);
        }

        return googleClientSecrets;
    }

    public async Task<DriveService?> GetDriveService()
    {
        GoogleClientSecrets? googleClientSecrets = GetGoogleClientSecrets() ?? JsonSerializer.Deserialize<GoogleClientSecrets>(CredentialsJson);

        if (googleClientSecrets == null)
            return null;

        // The folder "Ididit" stores the user's access and refresh tokens, and is created automatically when the authorization flow completes for the first time
        string credPath = "Ididit";

        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            googleClientSecrets.Secrets,
            _scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath)); // Defines whether the folder parameter is absolute or relative to
                                            // Environment.SpecialFolder.ApplicationData on Windows,
                                            // C:\Users\<user>\AppData\Roaming\
                                            // or $HOME on Linux and MacOS.

        DriveService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });

        return service;
    }
}
