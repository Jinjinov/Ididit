using Ididit.App.Data;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class MarkdownBackup
{
    private readonly JsInterop _jsInterop;
    private readonly IRepository _repository;

    public MarkdownBackup(JsInterop jsInterop, IRepository repository)
    {
        _jsInterop = jsInterop;
        _repository = repository;
    }

    public async Task ImportData(CategoryModel category, Stream stream, string name)
    {
        using StreamReader streamReader = new(stream);

        string text = await streamReader.ReadToEndAsync();

        GoalModel goal = category.CreateGoal(_repository.NextGoalId, name);
        goal.Details = text;

        await _repository.AddGoal(goal);

        TaskModel? task = null;

        foreach (string line in text.Split(Environment.NewLine))
        {
            if (task != null && line.StartsWith("- "))
            {
                task.DetailsText += line;

                task.AddDetail(line);

                await _repository.UpdateTask(task.Id);
            }
            else
            {
                task = goal.CreateTask(_repository.NextTaskId, line);

                await _repository.AddTask(task);
            }
        }
    }

    public async Task ExportData(IDataModel data)
    {
        StringBuilder stringBuilder = new();

        await SaveCategoryList(data.CategoryList, stringBuilder, level: 1);

        string md = stringBuilder.ToString();

        await _jsInterop.SaveAsUTF8("ididit.md", md);
    }

    private async Task SaveCategoryList(List<CategoryModel> categoryList, StringBuilder stringBuilder, int level)
    {
        foreach (CategoryModel category in categoryList)
        {
            stringBuilder.AppendLine($"{new string('#', Math.Min(level, 6))} {category.Name}");
            stringBuilder.AppendLine();

            foreach (GoalModel goal in category.GoalList)
            {
                stringBuilder.AppendLine($"**{goal.Name}**");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(goal.Details.Replace(Environment.NewLine, $"  {Environment.NewLine}"));
                stringBuilder.AppendLine();
            }

            if (category.CategoryList.Any())
            {
                await SaveCategoryList(category.CategoryList, stringBuilder, level + 1);
            }
        }
    }
}
