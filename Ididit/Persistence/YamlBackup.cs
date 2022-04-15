using Ididit.Data;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ididit.Persistence;

internal class YamlBackup
{
    private readonly ISerializer _serializer = new Serializer();

    private readonly IDeserializer _deserializer = new Deserializer();

    private readonly JsInterop _jsInterop;

    public YamlBackup(JsInterop jsInterop)
    {
        _jsInterop = jsInterop;
    }

    public async Task<DataModel> ImportData(Stream stream)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        DataModel data = _deserializer.Deserialize<DataModel>(text);

        return data;
    }

    public async Task ExportData(IDataModel data)
    {
        string yamlString = _serializer.Serialize(data);

        await _jsInterop.SaveAsUTF8("ididit.yaml", yamlString);
    }
}
