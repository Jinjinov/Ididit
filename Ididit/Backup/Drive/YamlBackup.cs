using Ididit.Data;
using Ididit.Data.Model;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ididit.Backup.Drive;

// https://github.com/aaubry/YamlDotNet/wiki/Serialization.Serializer#withtyperesolverityperesolver

// https://github.com/aaubry/YamlDotNet/wiki/Serialization.Serializer#disablealiases

internal class YamlBackup : IDataExport, IFileImport
{
    private readonly ISerializer _serializer = new Serializer();
    //private readonly ISerializer _serializer = new SerializerBuilder().DisableAliases().Build();
    //private readonly ISerializer _serializer = new SerializerBuilder().WithTypeResolver(new StaticTypeResolver()).Build();
    //private readonly ISerializer _serializer = new SerializerBuilder().DisableAliases().WithTypeResolver(new StaticTypeResolver()).Build();

    private readonly IDeserializer _deserializer = new Deserializer();

    public bool UnsavedChanges { get; private set; }

    public DataFormat DataFormat => DataFormat.Yaml;

    public string FileExtension => ".yaml";

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public YamlBackup(JsInterop jsInterop, IRepository repository)
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

        DataModel data = _deserializer.Deserialize<DataModel>(text);

        await _repository.AddData(data);
    }

    public async Task ExportData()
    {
        IDataModel data = _repository;

        string yamlString = string.Empty;

        using (StringWriter stringWriter = new())
        {
            _serializer.Serialize(stringWriter, data, typeof(IDataModel));

            yamlString = stringWriter.ToString();
        }

        await _jsInterop.SaveAsUTF8("ididit.yaml", yamlString);

        UnsavedChanges = false;
    }
}
