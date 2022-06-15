using CsvHelper;
using CsvHelper.Configuration;
using Ididit.Data;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class TsvBackup
{
    // https://stackoverflow.com/questions/66166584/csvhelper-custom-delimiter

    CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        DetectDelimiter = true
    };

    private readonly JsInterop _jsInterop;

    public TsvBackup(JsInterop jsInterop)
    {
        _jsInterop = jsInterop;
    }

    public async Task<DataModel> ImportData(Stream stream)
    {
        // https://joshclose.github.io/CsvHelper/examples/reading/get-anonymous-type-records/

        using (StreamReader streamReader = new(stream))
        {
            using (CsvReader csv = new CsvReader(streamReader, config))
            {
                while (csv.Read())
                {
                    switch (csv.GetField(0))
                    {
                    }
                }
            }
        }

        DataModel data = new();

        return data ?? throw new InvalidDataException("Can't deserialize TSV");
    }

    public async Task ExportData(IDataModel data)
    {
        // https://joshclose.github.io/CsvHelper/examples/writing/write-anonymous-type-objects/

        var records = new List<object>
        {
            new { Id = 1, Name = "one" },
        };

        StringBuilder builder = new StringBuilder();

        using (StringWriter writer = new StringWriter(builder))
        {
            using (CsvWriter csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(records);
            }
        }

        string tsv = builder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.tsv", tsv);
    }
}
