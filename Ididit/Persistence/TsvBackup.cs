using CsvHelper;
using CsvHelper.Configuration;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class TsvBackup
{
    // https://stackoverflow.com/questions/66166584/csvhelper-custom-delimiter

    CsvConfiguration importConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        //DetectDelimiter = true
        Delimiter = "\t",
        Quote = (char)1,
        Mode = CsvMode.NoEscape
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
        DataModel data = new();

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
                    Interval = string.Empty
                };

                var records = csv.GetRecordsAsync(anonymousTypeDefinition);

                CategoryModel category = new();
                GoalModel goal = new();
                TaskModel task = new();

                await foreach (var record in records)
                {
                    if (data.CategoryList.Any(c => c.Name == record.Category))
                    {
                        category = data.CategoryList.First(c => c.Name == record.Category);
                    }
                    else
                    {
                        category = new() { Name = record.Category };
                        data.CategoryList.Add(category);
                    }

                    if (category.GoalList.Any(g => g.Name == record.Goal))
                    {
                        goal = category.GoalList.First(g => g.Name == record.Goal);
                    }
                    else
                    {
                        goal = new() { Name = record.Goal };
                        category.GoalList.Add(goal);
                    }

                    task = new() { Name = record.Task, Priority = record.Priority };

                    string[] time = record.Interval.Split(' ');

                    if (time.Length == 2 && int.TryParse(time[0], out int interval))
                    {
                        task.DesiredTime = time[1].TrimEnd('s') switch
                        {
                            "day" => TimeSpan.FromDays(interval),
                            "week" => TimeSpan.FromDays(interval * 7),
                            "month" => TimeSpan.FromDays(interval * 30),
                            "year" => TimeSpan.FromDays(interval * 365),
                            _ => TimeSpan.Zero
                        };
                    }

                    goal.TaskList.Add(task);
                }
            }
        }

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
