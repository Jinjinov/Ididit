using Ididit.App.Data;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class MarkdownBackup
{
    public bool UnsavedChanges { get; private set; }

    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public MarkdownBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;

        _repository.DataChanged += (sender, e) => UnsavedChanges = true;
    }

    private static int GetStartHashCount(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != '#')
                return i;
        }

        return line.Length;
    }

    public async Task ImportData(Stream stream)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        using StringReader stringReader = new(text);

        await ImportToCategory(stringReader, null, null, 1);
    }

    private async Task<CategoryModel> GetChildCategory(CategoryModel? parent, string name)
    {
        if (parent is null)
        {
            if (_repository.CategoryList.FirstOrDefault(c => c.Name == name) is not CategoryModel child)
            {
                child = _repository.CreateCategory(name);
                await _repository.AddCategory(child);
            }

            return child;
        }
        else
        {
            if (parent.CategoryList.FirstOrDefault(c => c.Name == name) is not CategoryModel child)
            {
                child = parent.CreateCategory(_repository.NextCategoryId, name);
                await _repository.AddCategory(child);
            }

            return child;
        }
    }

    private async Task<string> ImportToCategory(StringReader stringReader, CategoryModel? parent, CategoryModel? category, int level)
    {
        GoalModel? goal = null;
        TaskModel? task = null;

        string? line;
        while ((line = stringReader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            int hashCount = GetStartHashCount(line);

            if (hashCount > 0 && hashCount < line.Length - 1 && line[hashCount] == ' ')
            {
                string name = line[(hashCount + 1)..];

                if (hashCount == level)
                {
                    category = await GetChildCategory(parent, name);
                }
                else if (hashCount > level)
                {
                    CategoryModel child = await GetChildCategory(category, name);

                    string nextName = await ImportToCategory(stringReader, category, child, hashCount);

                    if (!string.IsNullOrEmpty(nextName))
                    {
                        category = await GetChildCategory(parent, nextName);
                    }
                }
                else if (hashCount < level)
                {
                    return name;
                }
            }
            else if (line.StartsWith("**") && line.EndsWith("**") && line.Trim('*').Length > 0)
            {
                if (category is not null)
                {
                    goal = category.CreateGoal(_repository.NextGoalId, line.Trim('*'));
                    await _repository.AddGoal(goal);
                }
            }
            else if (goal is not null)
            {
                line = line.Trim();

                goal.Details += string.IsNullOrEmpty(goal.Details) ? line : Environment.NewLine + line;

                if (task != null && line.StartsWith("- "))
                {
                    bool add = task.AddDetail(line);

                    if (add)
                        task.DetailsText += string.IsNullOrEmpty(task.DetailsText) ? line : Environment.NewLine + line;

                    await _repository.UpdateTask(task.Id);
                }
                else
                {
                    task = goal.CreateTask(_repository.NextTaskId, line);

                    await _repository.AddTask(task);
                }
            }
        }

        return string.Empty;
    }

    public async Task ExportData()
    {
        IDataModel data = _repository;

        StringBuilder stringBuilder = new();

        foreach (CategoryModel category in data.CategoryList)
        {
            await SaveCategory(category, stringBuilder, level: 1);
        }

        string md = stringBuilder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.md", md);

        UnsavedChanges = false;
    }

    private async Task SaveCategory(CategoryModel category, StringBuilder stringBuilder, int level)
    {
        stringBuilder.AppendLine($"{new string('#', Math.Min(level, 6))} {category.Name}");
        stringBuilder.AppendLine();

        foreach (GoalModel goal in category.GoalList)
        {
            stringBuilder.AppendLine($"**{goal.Name}**");
            stringBuilder.AppendLine();
            //stringBuilder.AppendLine(goal.Details.Replace(Environment.NewLine, $"  {Environment.NewLine}"));

            foreach (TaskModel task in goal.TaskList)
            {
                stringBuilder.AppendLine($"{task.Name}  ");
                stringBuilder.AppendLine($"- Priority: {task.Priority}  ");

                if (task.IsTask)
                {
                    string interval = task.DesiredInterval.TotalDays > 0.0 ? task.DesiredInterval.TotalDays.ToString(CultureInfo.InvariantCulture) : "ASAP";
                    stringBuilder.AppendLine($"- Interval: {interval}  ");
                }

                if (task.DesiredDuration.HasValue && task.DesiredDuration.Value.TotalMinutes > 0.0)
                {
                    string duration = task.DesiredDuration.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture);
                    stringBuilder.AppendLine($"- Duration: {duration}  ");
                }

                task.Details?.AppendToStringBuilder(stringBuilder);

                stringBuilder.AppendLine();
            }

            //stringBuilder.AppendLine();
        }

        foreach (CategoryModel item in category.CategoryList)
        {
            await SaveCategory(item, stringBuilder, level + 1);
        }
    }
}
