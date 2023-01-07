using Ididit.Data.Model;
using Ididit.Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.Data;

public interface IRepository : IDataModel
{
    long NextCategoryId { get; }
    long NextGoalId { get; }
    long NextTaskId { get; }
    long NextSettingsId { get; }

    IReadOnlyDictionary<long, CategoryModel> AllCategories { get; }
    IReadOnlyDictionary<long, GoalModel> AllGoals { get; }
    IReadOnlyDictionary<long, TaskModel> AllTasks { get; }
    IReadOnlyDictionary<long, SettingsModel> AllSettings { get; }

    CategoryModel Category { get; }
    SettingsModel Settings { get; }

    event EventHandler? DataChanged;

    Task Initialize();
    Task AddData(IDataModel data);

    CategoryModel CreateCategory(string name);
    Task AddCategory(CategoryModel category);
    Task AddGoal(GoalModel goal);
    Task AddTask(TaskModel task);
    Task AddTime(DateTime time, long taskId);
    Task AddSettings(SettingsModel settings);

    Task UpdateCategory(long id);
    Task UpdateGoal(long id);
    Task UpdateTask(long id);
    Task UpdateTime(long id, DateTime time, long taskId);
    Task UpdateSettings(long id);

    Task DeleteCategory(long id);
    Task DeleteGoal(long id);
    Task DeleteTask(long id);
    Task DeleteTime(long id);
    Task DeleteSettings(long id);

    Task DeleteAll();
    Task ResetSettings();

    Task LoadExamples();
}
