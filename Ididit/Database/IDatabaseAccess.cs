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

    Task UpdateCategoryName(long id, string name);
    Task UpdateGoalName(long id, string name);
    Task UpdateTaskName(long id, string name);

    Task UpdateGoalDetails(long id, string details);

    Task DeleteCategory(long id);
    Task DeleteGoal(long id);
    Task DeleteTask(long id);
    Task DeleteTime(long id);
}
