using CsvHelper;
using CsvHelper.Configuration;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class TsvBackup
{
    // https://stackoverflow.com/questions/66166584/csvhelper-custom-delimiter

    CsvConfiguration importConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        DetectDelimiter = true
    };

    CsvConfiguration exportConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t"
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
            using (CsvReader csv = new CsvReader(streamReader, importConfig))
            {
                /*
                var records = new List<object>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new
                    {
                        Id = csv.GetField<int>("Id"),
                        Name = csv.GetField("Name"),
                        Tags = new List<string> { csv.GetField(5), csv.GetField(6), csv.GetField(7) }
                    };
                    records.Add(record);
                }
                /**/

                var anonymousTypeDefinition = new
                {
                    Category = string.Empty,
                    Goal = string.Empty,
                    Task = string.Empty,
                    Priority = Priority.None,
                    Interval = TimeSpan.Zero
                };

                var records = csv.GetRecords(anonymousTypeDefinition);
            }
        }

        DataModel data = new();

        return data ?? throw new InvalidDataException("Can't deserialize TSV");
    }

    public async Task ExportData(IDataModel data)
    {
        // https://joshclose.github.io/CsvHelper/examples/writing/write-anonymous-type-objects/

        List<object> records = new List<object>();

        foreach (CategoryModel category in data.CategoryList)
        {
            foreach (GoalModel goal in category.GoalList)
            {
                foreach (TaskModel task in goal.TaskList)
                {
                    records.Add(new { Category = category.Name, Goal = goal.Name, Task = task.Name, Priority = task.Priority, Interval = task.DesiredTime });
                }
            }
        }

        StringBuilder builder = new StringBuilder();

        using (StringWriter writer = new StringWriter(builder))
        {
            using (CsvWriter csv = new CsvWriter(writer, exportConfig))
            {
                csv.WriteRecords(records);
            }
        }

        string tsv = builder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.tsv", tsv);
    }
}
