using Ididit.Data;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class JsonBackup
{
    private readonly JsonSerializerOptions _options = new() { IncludeFields = true, WriteIndented = true };

    private readonly JsInterop _jsInterop;

    public JsonBackup(JsInterop jsInterop)
    {
        _jsInterop = jsInterop;
    }

    public async Task<DataModel> ImportData(Stream stream)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        DataModel? data = JsonSerializer.Deserialize<DataModel>(text, _options);

        return data ?? throw new InvalidDataException("Can't deserialize JSON");
    }

    public async Task ExportData(IDataModel data)
    {
        string jsonString = JsonSerializer.Serialize(data, _options);

        await _jsInterop.SaveAsUTF8("ididit.json", jsonString);
    }
}
