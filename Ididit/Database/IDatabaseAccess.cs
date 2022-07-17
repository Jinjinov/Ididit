using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Threading.Tasks;

namespace Ididit.Database;

internal interface IDatabaseAccess
{
    bool IsInitialized { get; }

    Task Initialize();

    Task<RepositoryData> GetData();
    Task AddData(IDataModel data);

    Task AddCategory(CategoryModel category);
    Task AddGoal(GoalModel goal);
    Task AddTask(TaskModel task);
    Task AddTime(DateTime time, long taskId);
    Task AddSettings(SettingsModel settings);

    Task UpdateCategory(CategoryModel category);
    Task UpdateGoal(GoalModel goal);
    Task UpdateTask(TaskModel task);
    Task UpdateTime(long id, DateTime time, long taskId);
    Task UpdateSettings(SettingsModel settings);

    Task DeleteCategory(long id);
    Task DeleteGoal(long id);
    Task DeleteTask(long id);
    Task DeleteTime(long id);
    Task DeleteSettings(long id);
}
