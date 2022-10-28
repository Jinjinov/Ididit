using Ididit.App.Data;
using Ididit.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class JsonBackup
{
    private readonly JsonSerializerOptions _options = new() { IncludeFields = true, WriteIndented = true };

    public bool UnsavedChanges { get; private set; }

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public JsonBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;

        _repository.DataChanged += (sender, e) => UnsavedChanges = true;
    }

    public async Task ImportData(Stream stream)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        DataModel data = JsonSerializer.Deserialize<DataModel>(text, _options) ?? throw new InvalidDataException("Can't deserialize JSON");

        await _repository.AddData(data);
    }

    public async Task ExportData(IDataModel data)
    {
        string jsonString = JsonSerializer.Serialize(data, _options);

        await _jsInterop.SaveAsUTF8("ididit.json", jsonString);

        UnsavedChanges = false;
    }
}
