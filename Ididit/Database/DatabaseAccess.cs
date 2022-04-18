using DnetIndexedDb;
using DnetIndexedDb.Fluent;
using DnetIndexedDb.Models;
using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using Ididit.Database.Entities;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.Database;

internal class IndexedDb : IndexedDbInterop
{
    public IndexedDb(IJSRuntime jsRuntime, IndexedDbOptions<IndexedDb> options) : base(jsRuntime, options)
    {
    }

    public static IndexedDbDatabaseModel GetDatabaseModel()
    {
        IndexedDbDatabaseModel indexedDbDatabaseModel = new()
        {
            Name = "Ididit",
            Version = 1,
            DbModelId = 0,
            //UseKeyGenerator = true - Unable to use AutoIncrement = false and AutoIncrement = true in the same IndexedDbDatabaseModel
        };

        indexedDbDatabaseModel.AddStore<CategoryEntity>();
        indexedDbDatabaseModel.AddStore<GoalEntity>();
        indexedDbDatabaseModel.AddStore<SettingsEntity>();
        indexedDbDatabaseModel.AddStore<TaskEntity>();
        indexedDbDatabaseModel.AddStore<TimeEntity>();

        return indexedDbDatabaseModel;
    }
}

internal class DatabaseAccess : IDatabaseAccess
{
    private List<CategoryEntity> _categoryList = new();
    private List<GoalEntity> _goalList = new();
    private List<SettingsEntity> _settingsList = new();
    private List<TaskEntity> _taskList = new();
    private List<TimeEntity> _timeList = new();

    private readonly Dictionary<long, CategoryEntity> _categoryDict = new();
    private readonly Dictionary<long, GoalEntity> _goalDict = new();
    private readonly Dictionary<long, TaskEntity> _taskDict = new();
    private readonly Dictionary<long, TimeEntity> _timeDict = new();

    private int _dbModelId = -1;

    private readonly IndexedDb _indexedDb;

    public DatabaseAccess(IndexedDb indexedDb)
    {
        _indexedDb = indexedDb;
    }

    public async Task Initialize()
    {
        if (_dbModelId != -1)
            throw new Exception("IndexedDb is already open");

        _dbModelId = await _indexedDb.OpenIndexedDb();
    }

    public async Task<RepositoryData> GetData()
    {
        RepositoryData data = new();

        _categoryList = await _indexedDb.GetAll<CategoryEntity>();

        foreach (CategoryEntity category in _categoryList)
        {
            data.CategoryDict.Add(category.Id, new CategoryModel()
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                Name = category.Name
            });

            _categoryDict[category.Id] = category;
        }

        foreach (CategoryModel category in data.CategoryDict.Values)
        {
            if (category.CategoryId is long id)
            {
                data.CategoryDict[id].CategoryList.Add(category);
            }
            else
            {
                data.CategoryList.Add(category);
            }
        }

        _goalList = await _indexedDb.GetAll<GoalEntity>();

        foreach (GoalEntity goal in _goalList)
        {
            GoalModel goalModel = new()
            {
                Id = goal.Id,
                CategoryId = goal.CategoryId,
                Name = goal.Name,
                Details = goal.Details
            };

            _goalDict[goal.Id] = goal;

            data.GoalDict.Add(goal.Id, goalModel);

            data.CategoryDict[goal.CategoryId].GoalList.Add(goalModel);
        }

        _taskList = await _indexedDb.GetAll<TaskEntity>();

        foreach (TaskEntity task in _taskList)
        {
            TaskModel taskModel = new()
            {
                Id = task.Id,
                GoalId = task.GoalId,
                Name = task.Name,
                CreatedAt = task.CreatedAt,
                AverageInterval = task.AverageInterval,
                DesiredInterval = task.DesiredInterval
            };

            _taskDict[task.Id] = task;

            data.TaskDict.Add(task.Id, taskModel);

            data.GoalDict[task.GoalId].TaskList.Add(taskModel);
        }

        _timeList = await _indexedDb.GetAll<TimeEntity>();

        foreach (TimeEntity time in _timeList)
        {
            _timeDict[time.Time.Ticks] = time;

            data.TaskDict[time.TaskId].TimeList.Add(time.Time);
        }

        return data;
    }

    public async Task AddData(IDataModel data)
    {
        AddCategoryList(data.CategoryList);

        string result = await _indexedDb.UpdateItems(_categoryList);
        result = await _indexedDb.UpdateItems(_goalList);
        result = await _indexedDb.UpdateItems(_taskList);
        result = await _indexedDb.UpdateItems(_timeList);
    }

    private void AddCategoryList(List<CategoryModel> categoryList)
    {
        foreach (CategoryModel category in categoryList)
        {
            CategoryEntity categoryEntity = new()
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                Name = category.Name
            };

            if (!_categoryDict.ContainsKey(categoryEntity.Id))
                _categoryList.Add(categoryEntity);

            _categoryDict[categoryEntity.Id] = categoryEntity;

            foreach (GoalModel goal in category.GoalList)
            {
                GoalEntity goalEntity = new()
                {
                    Id = goal.Id,
                    CategoryId = goal.CategoryId,
                    Name = goal.Name,
                    Details = goal.Details
                };

                if (!_goalDict.ContainsKey(goalEntity.Id))
                    _goalList.Add(goalEntity);

                _goalDict[goalEntity.Id] = goalEntity;

                foreach (TaskModel task in goal.TaskList)
                {
                    TaskEntity taskEntity = new()
                    {
                        Id = task.Id,
                        GoalId = task.GoalId,
                        Name = task.Name,
                        CreatedAt = task.CreatedAt,
                        AverageInterval = task.AverageInterval,
                        DesiredInterval = task.DesiredInterval
                    };

                    if (!_taskDict.ContainsKey(taskEntity.Id))
                        _taskList.Add(taskEntity);

                    _taskDict[taskEntity.Id] = taskEntity;

                    foreach (DateTime time in task.TimeList)
                    {
                        TimeEntity timeEntity = new()
                        {
                            Time = time,
                            TaskId = task.Id
                        };

                        if (!_timeDict.ContainsKey(timeEntity.Time.Ticks))
                            _timeList.Add(timeEntity);

                        _timeDict[timeEntity.Time.Ticks] = timeEntity;
                    }
                }
            }

            if (category.CategoryList.Any())
            {
                AddCategoryList(category.CategoryList);
            }
        }
    }

    public async Task AddCategory(CategoryModel category)
    {
        CategoryEntity categoryEntity = new()
        {
            Id = category.Id,
            CategoryId = category.CategoryId,
            Name = category.Name
        };

        _categoryList.Add(categoryEntity);

        _categoryDict[categoryEntity.Id] = categoryEntity;

        string result = await _indexedDb.AddItems(new List<CategoryEntity>() { categoryEntity });
    }

    public async Task AddGoal(GoalModel goal)
    {
        GoalEntity goalEntity = new()
        {
            Id = goal.Id,
            CategoryId = goal.CategoryId,
            Name = goal.Name,
            Details = goal.Details
        };

        _goalList.Add(goalEntity);

        _goalDict[goalEntity.Id] = goalEntity;

        string result = await _indexedDb.AddItems(new List<GoalEntity>() { goalEntity });
    }

    public async Task AddTask(TaskModel task)
    {
        TaskEntity taskEntity = new()
        {
            Id = task.Id,
            GoalId = task.GoalId,
            Name = task.Name,
            CreatedAt = task.CreatedAt,
            AverageInterval = task.AverageInterval,
            DesiredInterval = task.DesiredInterval
        };

        _taskList.Add(taskEntity);

        _taskDict[taskEntity.Id] = taskEntity;

        string result = await _indexedDb.AddItems(new List<TaskEntity>() { taskEntity });
    }

    public async Task AddTime(DateTime time, long taskId)
    {
        TimeEntity timeEntity = new()
        {
            Time = time,
            TaskId = taskId
        };

        _timeList.Add(timeEntity);

        _timeDict[timeEntity.Time.Ticks] = timeEntity;

        string result = await _indexedDb.AddItems(new List<TimeEntity>() { timeEntity });
    }

    public async Task UpdateCategory(CategoryModel category)
    {
        if (_categoryDict.TryGetValue(category.Id, out CategoryEntity? categoryEntity))
        {
            categoryEntity.CategoryId = category.CategoryId;
            categoryEntity.Name = category.Name;

            await _indexedDb.UpdateItems(new List<CategoryEntity> { categoryEntity });
        }
        else
        {
            throw new ArgumentException($"Category {category.Id} doesn't exist!");
        }
    }

    public async Task UpdateGoal(GoalModel goal)
    {
        if (_goalDict.TryGetValue(goal.Id, out GoalEntity? goalEntity))
        {
            goalEntity.CategoryId = goal.CategoryId;
            goalEntity.Name = goal.Name;
            goalEntity.Details = goal.Details;

            await _indexedDb.UpdateItems(new List<GoalEntity> { goalEntity });
        }
        else
        {
            throw new ArgumentException($"Gaol {goal.Id} doesn't exist!");
        }
    }

    public async Task UpdateTask(TaskModel task)
    {
        if (_taskDict.TryGetValue(task.Id, out TaskEntity? taskEntity))
        {
            taskEntity.GoalId = taskEntity.GoalId;
            taskEntity.Name = taskEntity.Name;
            taskEntity.CreatedAt = taskEntity.CreatedAt;
            taskEntity.AverageInterval = taskEntity.AverageInterval;
            taskEntity.DesiredInterval = taskEntity.DesiredInterval;

            await _indexedDb.UpdateItems(new List<TaskEntity> { taskEntity });
        }
        else
        {
            throw new ArgumentException($"Task {task.Id} doesn't exist!");
        }
    }

    public async Task UpdateTime(long id, DateTime time, long taskId)
    {
        if (_timeDict.ContainsKey(id))
        {
            await DeleteTime(id);

            await AddTime(time, taskId);
        }
        else
        {
            throw new ArgumentException($"Goal {id} doesn't exist!");
        }
    }

    public async Task DeleteCategory(long id)
    {
        _categoryList.Remove(_categoryDict[id]);

        _categoryDict.Remove(id);

        await _indexedDb.DeleteByKey<long, CategoryEntity>(id);
    }

    public async Task DeleteGoal(long id)
    {
        _goalList.Remove(_goalDict[id]);

        _goalDict.Remove(id);

        await _indexedDb.DeleteByKey<long, GoalEntity>(id);
    }

    public async Task DeleteTask(long id)
    {
        _taskList.Remove(_taskDict[id]);

        _taskDict.Remove(id);

        await _indexedDb.DeleteByKey<long, TaskEntity>(id);
    }

    public async Task DeleteTime(long id)
    {
        _timeList.Remove(_timeDict[id]);

        _timeDict.Remove(id);

        await _indexedDb.DeleteByKey<long, TimeEntity>(id);
    }

    async Task Test()
    {
        List<GoalEntity> goalEntities = new List<GoalEntity>();

        int dbModelId = await _indexedDb.OpenIndexedDb();

        string? result2 = await _indexedDb.AddItems<GoalEntity>(goalEntities);

        GoalEntity? result3 = await _indexedDb.GetByKey<long, GoalEntity>(11);
        string? result4 = await _indexedDb.DeleteByKey<long, GoalEntity>(11);

        List<GoalEntity>? result5 = await _indexedDb.GetAll<GoalEntity>();

        // get range by lower / upper key:
        List<GoalEntity>? result6 = await _indexedDb.GetRange<long, GoalEntity>(15, 20);

        // get range by lower / upper field value by the filed (index) name:
        List<GoalEntity>? db1Result8 = await _indexedDb.GetByIndex<int, GoalEntity>(11, 18, "Id", true);
        List<GoalEntity>? db1Result9 = await _indexedDb.GetByIndex<int?, GoalEntity>(11, null, "Id", false);

        // get min / max key value:
        long result8 = await _indexedDb.GetMaxKey<long, GoalEntity>();
        long result9 = await _indexedDb.GetMinKey<long, GoalEntity>();

        // get min / max value of the field by the filed (index) name:
        string? result11 = await _indexedDb.GetMaxIndex<string, GoalEntity>("Title");
        string? result12 = await _indexedDb.GetMinIndex<string, GoalEntity>("Title");

        string? result13 = await _indexedDb.UpdateItems<GoalEntity>(goalEntities);
        string? result132 = await _indexedDb.UpdateItemsByKey<GoalEntity>(goalEntities, new List<int>());

        string? result14 = await _indexedDb.DeleteAll<GoalEntity>();

        string? result15 = await _indexedDb.DeleteIndexedDb();
    }
}
