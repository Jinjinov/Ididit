using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        GoalModel goal = category.CreateGoal(_repository.MaxGoalId + 1);
        goal.Name = name;
        goal.Details = text;

        await _repository.AddGoal(goal);

        TaskModel? task = null;

        foreach (string line in text.Split(Environment.NewLine))
        {
            if (task != null && line.StartsWith("- "))
            {
                task.Details += line;

                await _repository.UpdateTask(task.Id);
            }
            else
            {
                task = goal.CreateTask(_repository.MaxTaskId + 1);
                task.Name = line;

                await _repository.AddTask(task);
            }
        }
    }

    public async Task ExportData(IDataModel data)
    {
        await SaveCategoryList(data.CategoryList);
    }

    private async Task SaveCategoryList(List<CategoryModel> categoryList)
    {
        foreach (CategoryModel category in categoryList)
        {
            foreach (GoalModel goal in category.GoalList)
            {
                await _jsInterop.SaveAsUTF8(goal.Name + ".md", goal.Details);
            }

            if (category.CategoryList.Any())
            {
                await SaveCategoryList(category.CategoryList);
            }
        }
    }
}
