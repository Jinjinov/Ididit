using CsvHelper;
using CsvHelper.Configuration;
using Ididit.Data;
using Ididit.Data.Model;
using Ididit.Data.Model.Models;
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
    }

    class CsvRow
    {
        public string Goal { get; set; } = string.Empty;
        public string Task { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public List<string> Category { get; set; } = new();
    };

    private sealed class CategoryLastIndexMap : ClassMap<CsvRow>
    {
        public CategoryLastIndexMap()
        {
            Map(m => m.Goal).Index(0);
            Map(m => m.Task).Index(1);
            Map(m => m.Interval).Index(2);
            Map(m => m.Duration).Index(3);
            Map(m => m.Priority).Index(4);
            Map(m => m.Category).Index(5);
        }
    }

    private sealed class CategoryFirstIndexMap : ClassMap<CsvRow>
    {
        public CategoryFirstIndexMap(int categoryColumns)
        {
            Map(m => m.Category).Index(0, categoryColumns - 1);
            Map(m => m.Goal).Index(categoryColumns);
            Map(m => m.Task).Index(categoryColumns + 1);
            Map(m => m.Interval).Index(categoryColumns + 2);
            Map(m => m.Duration).Index(categoryColumns + 3);
            Map(m => m.Priority).Index(categoryColumns + 4);
        }
    }

    // https://joshclose.github.io/CsvHelper/examples/reading/get-anonymous-type-records/

    // https://github.com/JoshClose/CsvHelper/blob/master/tests/CsvHelper.Tests/TypeConversion/IEnumerableConverterTests.cs

    public async Task ImportData(Stream stream)
    {
        using MemoryStream memoryStream = new();

        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using StreamReader streamReader = new(memoryStream);

        int categoryColumns = 0;

        if (streamReader.ReadLine() is string headerRow)
        {
            string[] headers = headerRow.Split('\t');

            if (headers.Length > 0 && headers[0] == "Category")
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == "Goal")
                    {
                        categoryColumns = i;
                        break;
                    }
                }
            }
        }

        memoryStream.Position = 0;
        streamReader.DiscardBufferedData();

        using CsvReader csv = new(streamReader, _importConfig);

        if (categoryColumns > 0)
        {
            csv.Context.RegisterClassMap(new CategoryFirstIndexMap(categoryColumns));
        }
        else
        {
            csv.Context.RegisterClassMap<CategoryLastIndexMap>();
        }

        IAsyncEnumerable<CsvRow> records = csv.GetRecordsAsync<CsvRow>();

        await foreach (CsvRow record in records)
        {
            if (!record.Category.Any())
                continue;

            string firstCategory = record.Category.First();

            if (_repository.CategoryList.FirstOrDefault(c => c.Name == firstCategory) is not CategoryModel root)
            {
                root = _repository.CreateCategory(firstCategory);
                await _repository.AddCategory(root);
            }

            CategoryModel category = root;

            if (record.Category.Count > 1)
            {
                for (int i = 1; i < record.Category.Count; i++)
                {
                    string name = record.Category[i];

                    if (string.IsNullOrEmpty(name))
                        break;

                    if (category.CategoryList.FirstOrDefault(c => c.Name == name) is not CategoryModel child)
                    {
                        child = category.CreateCategory(_repository.NextCategoryId, name);
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

            if (!string.IsNullOrEmpty(record.Priority))
            {
                TimeSpan desiredInterval = TimeSpan.Zero;
                TimeSpan? desiredDuration = null;
                Priority priority = Priority.None;
                TaskKind taskKind = TaskKind.Note;

                if (double.TryParse(record.Interval, NumberStyles.Any, CultureInfo.InvariantCulture, out double days))
                {
                    desiredInterval = TimeSpan.FromDays(days);
                    taskKind = TaskKind.RepeatingTask;
                }
                else if (!string.IsNullOrEmpty(record.Interval))
                {
                    desiredInterval = TimeSpan.Zero;
                    taskKind = TaskKind.Task;
                }

                if (Enum.TryParse(record.Priority, out Priority taskPriority))
                {
                    priority = taskPriority;
                }

                if (double.TryParse(record.Duration, NumberStyles.Any, CultureInfo.InvariantCulture, out double minutes))
                {
                    desiredDuration = TimeSpan.FromMinutes(minutes);
                }

                TaskModel task = goal.CreateTask(_repository.NextTaskId, record.Task, desiredInterval, priority, taskKind, desiredDuration);

                await _repository.AddTask(task);
            }
        }
    }

    // https://joshclose.github.io/CsvHelper/examples/writing/write-anonymous-type-objects/

    public async Task ExportData()
    {
        IDataModel data = _repository;

        List<CsvRow> records = new();

        foreach (CategoryModel category in data.CategoryList)
        {
            AddCategory(records, category, new());
        }

        StringBuilder builder = new();

        using (StringWriter writer = new(builder))
        {
            using CsvWriter csv = new(writer, _exportConfig);

            csv.Context.RegisterClassMap<CategoryLastIndexMap>();

            csv.WriteRecords(records);
        }

        string tsv = builder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.tsv", tsv);

        UnsavedChanges = false;
    }

    private static void AddCategory(List<CsvRow> records, CategoryModel category, List<string> parents)
    {
        List<string> categories = new(parents) { category.Name };

        foreach (GoalModel goal in category.GoalList)
        {
            if (goal.TaskList.Any())
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
                        Priority = task.Priority.ToString(),
                        Interval = interval,
                        Duration = duration
                    });
                }
            }
            else if(!string.IsNullOrEmpty(goal.Details))
            {
                foreach (string line in goal.Details.Split('\n'))
                {
                    records.Add(new CsvRow
                    {
                        Category = categories,
                        Goal = goal.Name,
                        Task = line,
                        Priority = string.Empty,
                        Interval = string.Empty,
                        Duration = string.Empty
                    });
                }
            }
        }

        foreach (CategoryModel item in category.CategoryList)
        {
            AddCategory(records, item, categories);
        }
    }
}
