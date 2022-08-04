using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.App;

internal interface IRepository : IDataModel
{
    long MaxCategoryId { get; }
    long MaxGoalId { get; }
    long MaxTaskId { get; }
    long MaxSettingsId { get; }

    IReadOnlyDictionary<long, CategoryModel> AllCategories { get; }
    IReadOnlyDictionary<long, GoalModel> AllGoals { get; }
    IReadOnlyDictionary<long, TaskModel> AllTasks { get; }
    IReadOnlyDictionary<long, SettingsModel> AllSettings { get; }

    CategoryModel Category { get; }
    SettingsModel Settings { get; }

    Task Initialize();
    Task AddData(IDataModel data);

    CategoryModel CreateCategory();
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
}
