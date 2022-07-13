using Ididit.Data;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Ididit.Persistence;

internal class GoogleDriveBackup
{
    public async Task<DataModel> ImportData()
    {
        string text = await LoadFile();

        DataModel? data = JsonSerializer.Deserialize<DataModel>(text, _options);

        return data ?? throw new InvalidDataException("Can't deserialize JSON");
    }

    public void ExportData(IDataModel data)
    {
        _data = data;

        _timer.Stop();
        _timer.Start();
    }

    private IDataModel _data = null!;

    private readonly HttpClient _httpClient;

    private readonly IAccessTokenProvider _tokenProvider;

    private readonly Timer _timer = new() { AutoReset = false, Interval = 30 * 1000 };

    private readonly JsonSerializerOptions _options = new() { IncludeFields = true, WriteIndented = true };

    public GoogleDriveBackup(HttpClient httpClient, IAccessTokenProvider tokenProvider)
    {
        _httpClient = httpClient;

        _tokenProvider = tokenProvider;

        _timer.Elapsed += async (object? sender, ElapsedEventArgs e) => await Backup();
    }

    private async Task Backup()
    {
        string jsonString = JsonSerializer.Serialize(_data, _options);

        await SaveFile(jsonString);
    }

    private async Task SaveFile(string content)
    {
        string folderId = await GetFolderId();

        if (string.IsNullOrEmpty(folderId))
        {
            folderId = await CreateFolder();
        }

        string fileId = await GetFileId(folderId);

        if (string.IsNullOrEmpty(fileId))
        {
            fileId = await CreateFile(folderId, content);
        }
        else
        {
            await UpdateFile(fileId, content);
        }
    }

    private async Task<string> LoadFile()
    {
        string folderId = await GetFolderId();

        if (string.IsNullOrEmpty(folderId))
        {
            return string.Empty;
        }

        string fileId = await GetFileId(folderId);

        if (string.IsNullOrEmpty(fileId))
        {
            return string.Empty;
        }
        else
        {
            return await GetFile(fileId);
        }
    }

    private async Task<string> GetFolderId()
    {
        string folderId = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            string q = "name = 'ididit' and mimeType = 'application/vnd.google-apps.folder'";
            string url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();

            JsonElement data = await response.Content.ReadFromJsonAsync<JsonElement>();

            JsonElement.ArrayEnumerator files = data.GetProperty("files").EnumerateArray();

            while (files.MoveNext())
            {
                JsonElement file = files.Current;

                string? name = file.GetProperty("name").GetString();

                if (name == "ididit")
                {
                    folderId = file.GetProperty("id").GetString() ?? string.Empty;
                }
            }
        }

        return folderId;
    }

    private async Task<string> GetFileId(string folderId)
    {
        string fileId = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            string q = $"'{folderId}' in parents";
            string url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();

            JsonElement data = await response.Content.ReadFromJsonAsync<JsonElement>();

            JsonElement.ArrayEnumerator files = data.GetProperty("files").EnumerateArray();

            while (files.MoveNext())
            {
                JsonElement file = files.Current;

                string? fileName = file.GetProperty("name").GetString();

                fileId = file.GetProperty("id").GetString() ?? string.Empty;
            }
        }

        return fileId;
    }

    private async Task<string> GetFile(string fileId)
    {
        string file = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            string url = $"https://www.googleapis.com/drive/v3/files/{fileId}?alt=media";

            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            file = await response.Content.ReadAsStringAsync();
        }

        return file;
    }

    public async Task<string> GetFolders()
    {
        string folders = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            string q = "mimeType = 'application/vnd.google-apps.folder'";
            string url = "https://www.googleapis.com/drive/v3/files?q=" + Uri.EscapeDataString(q);

            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();

            JsonElement data = await response.Content.ReadFromJsonAsync<JsonElement>();

            JsonElement.ArrayEnumerator files = data.GetProperty("files").EnumerateArray();

            folders = string.Empty;

            while (files.MoveNext())
            {
                JsonElement file = files.Current;

                string? name = file.GetProperty("name").GetString();

                folders += name + Environment.NewLine;
            }
        }

        return folders;
    }

    private async Task<string> CreateFolder()
    {
        string folderId = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart"),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            JsonContent metaContent = JsonContent.Create(new { name = "ididit", description = "ididit backup", mimeType = "application/vnd.google-apps.folder" });

            MultipartContent multipart = new() { metaContent };

            requestMessage.Content = multipart;

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();

            JsonElement data = await response.Content.ReadFromJsonAsync<JsonElement>();

            folderId = data.GetProperty("id").GetString() ?? string.Empty;
        }

        return folderId;
    }

    private async Task<string> CreateFile(string folderId, string content)
    {
        string fileId = string.Empty;

        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Post,
                //RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=media"),
                RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart"),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            JsonContent metaContent;

            if (string.IsNullOrEmpty(folderId))
                metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup" });
            else
                metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup", parents = new[] { folderId } });

            //var data = new { Title = "Blazor POST Request Example" };
            //string content = JsonSerializer.Serialize(data);

            StringContent fileContent = new(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            //var fileContent = new StringContent(content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
            //fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            fileContent.Headers.ContentLength = content.Length;

            MultipartContent multipart = new() { metaContent, fileContent };

            requestMessage.Content = multipart;

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();

            JsonElement data = await response.Content.ReadFromJsonAsync<JsonElement>();

            fileId = data.GetProperty("id").GetString() ?? string.Empty;
        }

        return fileId;
    }

    private async Task UpdateFile(string fileId, string content)
    {
        AccessTokenResult tokenResult = await _tokenProvider.RequestAccessToken();

        if (tokenResult.TryGetToken(out AccessToken token))
        {
            HttpRequestMessage requestMessage = new()
            {
                Method = HttpMethod.Patch,
                //RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files?uploadType=media"),
                RequestUri = new Uri("https://www.googleapis.com/upload/drive/v3/files/" + fileId + "?uploadType=multipart"),
            };

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            JsonContent metaContent = JsonContent.Create(new { name = "ididit.json", description = "ididit backup" });

            //var data = new { Title = "Blazor POST Request Example", DateTime = DateTime.Now };
            //string content = JsonSerializer.Serialize(data);

            StringContent fileContent = new(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            //var fileContent = new StringContent(content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
            //fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            fileContent.Headers.ContentLength = content.Length;

            MultipartContent multipart = new() { metaContent, fileContent };

            requestMessage.Content = multipart;

            HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            HttpStatusCode responseStatusCode = response.StatusCode;

            //responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}