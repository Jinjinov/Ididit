using Ididit.Data;
using Ididit.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.App;

internal interface IRepository : IDataModel
{
    long MaxCategoryId { get; }
    long MaxGoalId { get; }
    long MaxTaskId { get; }

    IReadOnlyDictionary<long, CategoryModel> AllCategories { get; }
    IReadOnlyDictionary<long, GoalModel> AllGoals { get; }
    IReadOnlyDictionary<long, TaskModel> AllTasks { get; }

    Task Initialize();
    Task AddData(IDataModel data);

    CategoryModel CreateCategory();
    Task AddCategory(CategoryModel category);
    Task AddGoal(GoalModel goal);
    Task AddTask(TaskModel task);
    Task AddTime(long ticks, long taskId);

    Task UpdateCategoryName(long id, string name);
    Task UpdateGoalName(long id, string name);
    Task UpdateTaskName(long id, string name);

    Task UpdateGoalDetails(long id, string details);

    Task DeleteCategory(long id);
    Task DeleteGoal(long id);
    Task DeleteTask(long id);
    Task DeleteTime(long id);
}
