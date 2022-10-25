using Ididit.Data;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ididit.Persistence;

// https://github.com/aaubry/YamlDotNet/wiki/Serialization.Serializer#withtyperesolverityperesolver

// https://github.com/aaubry/YamlDotNet/wiki/Serialization.Serializer#disablealiases

internal class YamlBackup
{
    private readonly ISerializer _serializer = new Serializer();
    //private readonly ISerializer _serializer = new SerializerBuilder().DisableAliases().Build();
    //private readonly ISerializer _serializer = new SerializerBuilder().WithTypeResolver(new StaticTypeResolver()).Build();
    //private readonly ISerializer _serializer = new SerializerBuilder().DisableAliases().WithTypeResolver(new StaticTypeResolver()).Build();

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
        string yamlString = string.Empty;

        using (StringWriter stringWriter = new())
        {
            _serializer.Serialize(stringWriter, data, typeof(IDataModel));

            yamlString = stringWriter.ToString();
        }

        await _jsInterop.SaveAsUTF8("ididit.yaml", yamlString);
    }
}
