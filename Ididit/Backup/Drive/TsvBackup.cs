using CsvHelper;
using CsvHelper.Configuration;
using Ididit.App.Data;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Backup.Drive;

internal class TsvBackup : IDataExport, IFileImport
{
    // https://stackoverflow.com/questions/66166584/csvhelper-custom-delimiter

    readonly CsvConfiguration _importConfig = new(CultureInfo.InvariantCulture)
    {
        //DetectDelimiter = true
        Delimiter = "\t",
        Quote = (char)1,
        Mode = CsvMode.NoEscape
    };

    readonly CsvConfiguration _exportConfig = new(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        Quote = (char)1,
        Mode = CsvMode.NoEscape
    };

    public bool UnsavedChanges { get; private set; }

    public DataFormat DataFormat => DataFormat.Tsv;

    public string FileExtension => ".tsv";

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public TsvBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;

        _repository.DataChanged += (sender, e) => UnsavedChanges = true;

        _repository.AddDataExport(this);
        _repository.AddFileImport(this);
    }

    class CsvRow
    {
        public string Goal { get; set; } = string.Empty;
        public string Task { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.None;
        public Priority Success { get; set; } = Priority.None;
        public Priority Benefit { get; set; } = Priority.None;
        public List<string> Category { get; set; } = new();
    };

    private sealed class CsvRowIndexMap : ClassMap<CsvRow>
    {
        public CsvRowIndexMap()
        {
            Map(m => m.Goal).Index(0);
            Map(m => m.Task).Index(1);
            Map(m => m.Interval).Index(2);
            Map(m => m.Duration).Index(3);
            Map(m => m.Priority).Index(4);
            Map(m => m.Success).Index(5);
            Map(m => m.Benefit).Index(6);
            Map(m => m.Category).Index(7);
        }
    }

    public async Task ImportData(Stream stream)
    {
        // https://joshclose.github.io/CsvHelper/examples/reading/get-anonymous-type-records/

        using StreamReader streamReader = new(stream);

        using CsvReader csv = new(streamReader, _importConfig);

        // https://github.com/JoshClose/CsvHelper/blob/master/tests/CsvHelper.Tests/TypeConversion/IEnumerableConverterTests.cs

        csv.Context.RegisterClassMap<CsvRowIndexMap>();

        IAsyncEnumerable<CsvRow> records = csv.GetRecordsAsync<CsvRow>();

        await foreach (CsvRow record in records)
        {
            if (!record.Category.Any())
                continue;

            if (_repository.CategoryList.FirstOrDefault(c => c.Name == record.Category.First()) is not CategoryModel root)
            {
                root = _repository.CreateCategory(record.Category.First());
                await _repository.AddCategory(root);
            }

            CategoryModel category = root;

            if (record.Category.Count > 1)
            {
                for (int i = 1; i < record.Category.Count; i++)
                {
                    string name = record.Category[i];

                    if (category.CategoryList.FirstOrDefault(c => c.Name == name) is not CategoryModel child)
                    {
                        child = root.CreateCategory(_repository.NextCategoryId, name);
                        await _repository.AddCategory(child);
                    }

                    category = child;
                }
            }

            if (category.GoalList.FirstOrDefault(g => g.Name == record.Goal) is not GoalModel goal)
            {
                goal = category.CreateGoal(_repository.NextGoalId, record.Goal);
                await _repository.AddGoal(goal);
            }

            goal.Details += string.IsNullOrEmpty(goal.Details) ? record.Task : Environment.NewLine + record.Task;
            await _repository.UpdateGoal(goal.Id);

            TimeSpan desiredInterval = TimeSpan.Zero;
            TimeSpan? desiredDuration = null;
            TaskKind taskKind = TaskKind.Note;

            if (double.TryParse(record.Interval, NumberStyles.Any, CultureInfo.InvariantCulture, out double days))
            {
                desiredInterval = TimeSpan.FromDays(days);
                taskKind = TaskKind.RepeatingTask;
            }
            else if (string.Equals(record.Interval, "ASAP", StringComparison.OrdinalIgnoreCase))
            {
                desiredInterval = TimeSpan.Zero;
                taskKind = TaskKind.Task;
            }

            if (double.TryParse(record.Duration, NumberStyles.Any, CultureInfo.InvariantCulture, out double minutes))
            {
                desiredDuration = TimeSpan.FromMinutes(minutes);
            }

            TaskModel task = goal.CreateTask(_repository.NextTaskId, record.Task, desiredInterval, record.Priority, taskKind, desiredDuration);

            await _repository.AddTask(task);
        }
    }

    public async Task ExportData()
    {
        IDataModel data = _repository;

        // https://joshclose.github.io/CsvHelper/examples/writing/write-anonymous-type-objects/

        List<CsvRow> records = new();

        foreach (CategoryModel category in data.CategoryList)
        {
            AddCategory(records, category, new());
        }

        StringBuilder builder = new();

        using (StringWriter writer = new(builder))
        {
            using CsvWriter csv = new(writer, _exportConfig);

            csv.Context.RegisterClassMap<CsvRowIndexMap>();

            csv.WriteRecords(records);
        }

        string tsv = builder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.tsv", tsv);

        UnsavedChanges = false;
    }

    private static void AddCategory(List<CsvRow> records, CategoryModel category, List<string> parents)
    {
        List<string> categories = new(parents);
        categories.Add(category.Name);

        foreach (GoalModel goal in category.GoalList)
        {
            foreach (TaskModel task in goal.TaskList)
            {
                string interval = string.Empty;

                if (task.IsTask)
                {
                    interval = task.DesiredInterval.TotalDays > 0.0 ? task.DesiredInterval.TotalDays.ToString(CultureInfo.InvariantCulture) : "ASAP";
                }

                string duration = task.DesiredDuration.HasValue && task.DesiredDuration.Value.TotalMinutes > 0.0 ? task.DesiredDuration.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture) : "";

                records.Add(new CsvRow
                {
                    Category = categories,
                    Goal = goal.Name,
                    Task = task.Name,
                    Priority = task.Priority,
                    Interval = interval,
                    Duration = duration
                });
            }
        }

        foreach (CategoryModel item in category.CategoryList)
        {
            AddCategory(records, item, categories);
        }
    }
}
