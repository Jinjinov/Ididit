using Ididit.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Persistence;

public abstract class GoogleDriveBase : IGoogleDriveBackup
{
    protected const string _fileName = "ididit.json";
    protected const string _fileDescription = "ididit backup";
    protected const string _fileMimeType = "application/json";

    protected const string _folderName = "ididit";
    protected const string _folderDescription = "ididit backup";
    protected const string _folderMimeType = "application/vnd.google-apps.folder";

    protected readonly JsonSerializerOptions _options = new() { IncludeFields = true, WriteIndented = true };

    public async Task<DataModel> ImportData()
    {
        string text = await LoadFile();

        DataModel? data = JsonSerializer.Deserialize<DataModel>(text, _options);

        return data ?? throw new InvalidDataException("Can't deserialize JSON");
    }

    public async Task ExportData(IDataModel data)
    {
        string jsonString = JsonSerializer.Serialize(data, _options);

        await SaveFile(jsonString);
    }

    protected async Task SaveFile(string content)
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

    protected async Task<string> LoadFile()
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

    protected abstract Task<string> GetFile(string fileId);

    protected abstract Task<string> CreateFolder();

    protected abstract Task<string> CreateFile(string folderId, string content);

    protected abstract Task<string> UpdateFile(string fileId, string content);

    protected abstract Task<string> GetFolderId();

    protected abstract Task<string> GetFileId(string folderId);
}
