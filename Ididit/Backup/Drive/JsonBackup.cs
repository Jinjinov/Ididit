using Ididit.App.Data;
using Ididit.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Backup.Drive;

internal class JsonBackup : IDataExport, IFileImport
{
    private readonly JsonSerializerOptions _options = new() { IncludeFields = true, WriteIndented = true };

    public bool UnsavedChanges { get; private set; }

    public DataFormat DataFormat => DataFormat.Json;

    public string FileExtension => ".json";

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public JsonBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;

        _repository.DataChanged += (sender, e) => UnsavedChanges = true;

        _repository.AddDataExport(this);
        _repository.AddFileImport(this);
    }

    public async Task ImportData(Stream stream)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        DataModel data = JsonSerializer.Deserialize<DataModel>(text, _options) ?? throw new InvalidDataException("Can't deserialize JSON");

        await _repository.AddData(data);
    }

    public async Task ExportData()
    {
        IDataModel data = _repository;

        string jsonString = JsonSerializer.Serialize(data, _options);

        await _jsInterop.SaveAsUTF8("ididit.json", jsonString);

        UnsavedChanges = false;
    }
}
