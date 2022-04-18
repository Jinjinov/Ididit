using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Threading.Tasks;

namespace Ididit.Database;

internal interface IDatabaseAccess
{
    Task Initialize();

    Task<RepositoryData> GetData();
    Task AddData(IDataModel data);

    Task AddCategory(CategoryModel category);
    Task AddGoal(GoalModel goal);
    Task AddTask(TaskModel task);
    Task AddTime(DateTime time, long taskId);

    Task UpdateCategory(CategoryModel category);
    Task UpdateGoal(GoalModel goal);
    Task UpdateTask(TaskModel task);
    Task UpdateTime(long id, DateTime time, long taskId);

    Task DeleteCategory(long id);
    Task DeleteGoal(long id);
    Task DeleteTask(long id);
    Task DeleteTime(long id);
}
