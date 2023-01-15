using DnetIndexedDb;
using DnetIndexedDb.Fluent;
using DnetIndexedDb.Models;
using Ididit.Data.Database.Entities;
using Ididit.Data.Model;
using Ididit.Data.Model.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.Data.Database;

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
    private readonly Dictionary<long, SettingsEntity> _settingsDict = new();
    private readonly Dictionary<long, TaskEntity> _taskDict = new();
    private readonly Dictionary<long, TimeEntity> _timeDict = new();

    private int _dbModelId = -1;

    public bool IsInitialized => _dbModelId != -1;

    public event EventHandler? DataChanged;

    private readonly IndexedDb _indexedDb;

    public DatabaseAccess(IndexedDb indexedDb)
    {
        _indexedDb = indexedDb;
    }

    private async ValueTask<List<TEntity>> GetAll<TEntity>()
    {
        List<TEntity> result = await _indexedDb.GetAll<TEntity>();

        return result;
    }

    private async ValueTask<string> UpdateItems<TEntity>(List<TEntity> items)
    {
        string result = await _indexedDb.UpdateItems(items);

        DataChanged?.Invoke(this, EventArgs.Empty);

        return result; // 'DB_DATA_UPDATED'
    }

    private async ValueTask<string> AddItems<TEntity>(List<TEntity> items)
    {
        string result = await _indexedDb.AddItems(items);

        DataChanged?.Invoke(this, EventArgs.Empty);

        return result; // 'DB_DATA_ADDED'
    }

    private async ValueTask<string> DeleteByKey<TKey, TEntity>(TKey key)
    {
        string result = await _indexedDb.DeleteByKey<TKey, TEntity>(key);

        DataChanged?.Invoke(this, EventArgs.Empty);

        return result; // 'DB_DELETEOBJECT_SUCCESS'
    }

    private async ValueTask<string> DeleteAll<TEntity>()
    {
        string result = await _indexedDb.DeleteAll<TEntity>();

        DataChanged?.Invoke(this, EventArgs.Empty);

        return result; // 'DB_DELETEOBJECT_SUCCESS'
    }

    public async Task Initialize()
    {
        if (_dbModelId != -1)
            throw new InvalidOperationException("IndexedDb is already open");

        _dbModelId = await _indexedDb.OpenIndexedDb();
    }

    public async Task<RepositoryData> GetData()
    {
        RepositoryData data = new();

        _categoryList = await GetAll<CategoryEntity>();

        foreach (CategoryEntity category in _categoryList)
        {
            data.CategoryDict.Add(category.Id, new CategoryModel()
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                PreviousId = category.PreviousId,
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

        _goalList = await GetAll<GoalEntity>();

        foreach (GoalEntity goal in _goalList)
        {
            GoalModel goalModel = new()
            {
                Id = goal.Id,
                CategoryId = goal.CategoryId,
                PreviousId = goal.PreviousId,
                Name = goal.Name,
                Details = goal.Details,
                CreateTaskFromEachLine = goal.CreateTaskFromEachLine
            };

            _goalDict[goal.Id] = goal;

            data.GoalDict.Add(goal.Id, goalModel);

            data.CategoryDict[goal.CategoryId].GoalList.Add(goalModel);
        }

        foreach (CategoryModel category in data.CategoryDict.Values)
        {
            category.OrderCategories();
            category.OrderGoals();
        }

        _taskList = await GetAll<TaskEntity>();

        foreach (TaskEntity task in _taskList)
        {
            TaskModel taskModel = new()
            {
                Id = task.Id,
                GoalId = task.GoalId,
                PreviousId = task.PreviousId,
                Name = task.Name,
                DetailsText = task.DetailsText,
                CreatedAt = task.CreatedAt,
                LastTimeDoneAt = task.LastTimeDoneAt,
                AverageInterval = task.AverageInterval,
                DesiredInterval = task.DesiredInterval,
                Priority = task.Priority,
                TaskKind = task.TaskKind,
                AverageDuration = task.AverageDuration,
                DesiredDuration = task.DesiredDuration,
                DurationTimedCount = task.DurationTimedCount,
                Details = task.Details is null ? null : new DetailsModel
                {
                    Date = task.Details.Date,
                    Address = task.Details.Address,
                    Phone = task.Details.Phone,
                    Email = task.Details.Email,
                    Website = task.Details.Website,
                    OpenFrom = task.Details.OpenFrom,
                    OpenTill = task.Details.OpenTill,
                }
            };

            _taskDict[task.Id] = task;

            data.TaskDict.Add(task.Id, taskModel);

            data.GoalDict[task.GoalId].TaskList.Add(taskModel);
        }

        foreach (GoalModel goal in data.GoalDict.Values)
        {
            goal.OrderTasks();
        }

        _timeList = await GetAll<TimeEntity>();

        foreach (TimeEntity time in _timeList)
        {
            _timeDict[time.Time.Ticks] = time;

            data.TaskDict[time.TaskId].TimeList.Add(time.Time);
        }

        _settingsList = await GetAll<SettingsEntity>();

        foreach (SettingsEntity settings in _settingsList)
        {
            SettingsModel settingsModel = new()
            {
                Id = settings.Id,
                Name = settings.Name,
                SelectedBackupFormat = settings.SelectedBackupFormat,
                Size = settings.Size,
                Culture = settings.Culture,
                Theme = settings.Theme,
                Background = settings.Background,
                ShowAllGoals = settings.ShowAllGoals,
                ShowAllTasks = settings.ShowAllTasks,
                ShowPriority = settings.ShowPriority,
                ShowTaskKind = settings.ShowTaskKind,
                Sort = settings.Sort,
                Screen = settings.Screen,
                ElapsedToDesiredRatioMin = settings.ElapsedToDesiredRatioMin,
                ShowElapsedToDesiredRatioOverMin = settings.ShowElapsedToDesiredRatioOverMin,
                HideEmptyGoals = settings.HideEmptyGoals,
                HideGoalsWithSimpleText = settings.HideGoalsWithSimpleText,
                ShowCategoriesInGoalList = settings.ShowCategoriesInGoalList,
                HideCompletedTasks = settings.HideCompletedTasks
            };

            _settingsDict[settings.Id] = settings;

            data.SettingsList.Add(settingsModel);

            data.SettingsDict.Add(settings.Id, settingsModel);
        }

        return data;
    }

    public async Task AddData(IDataModel data)
    {
        AddCategoryList(data.CategoryList);
        AddSettingsList(data.SettingsList);

        await UpdateItems(_categoryList);
        await UpdateItems(_goalList);
        await UpdateItems(_settingsList);
        await UpdateItems(_taskList);
        await UpdateItems(_timeList);
    }

    private void AddCategoryList(List<CategoryModel> categoryList)
    {
        foreach (CategoryModel category in categoryList)
        {
            CategoryEntity categoryEntity = new()
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                PreviousId = category.PreviousId,
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
                    PreviousId = goal.PreviousId,
                    Name = goal.Name,
                    Details = goal.Details,
                    CreateTaskFromEachLine = goal.CreateTaskFromEachLine
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
                        PreviousId = task.PreviousId,
                        Name = task.Name,
                        DetailsText = task.DetailsText,
                        CreatedAt = task.CreatedAt,
                        LastTimeDoneAt = task.LastTimeDoneAt,
                        AverageInterval = task.AverageInterval,
                        DesiredInterval = task.DesiredInterval,
                        Priority = task.Priority,
                        TaskKind = task.TaskKind,
                        AverageDuration = task.AverageDuration,
                        DesiredDuration = task.DesiredDuration,
                        DurationTimedCount = task.DurationTimedCount,
                        Details = task.Details is null ? null : new DetailsEntity
                        {
                            Date = task.Details.Date,
                            Address = task.Details.Address,
                            Phone = task.Details.Phone,
                            Email = task.Details.Email,
                            Website = task.Details.Website,
                            OpenFrom = task.Details.OpenFrom,
                            OpenTill = task.Details.OpenTill,
                        }
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

    private void AddSettingsList(List<SettingsModel> settingsList)
    {
        foreach (SettingsModel settings in settingsList)
        {
            SettingsEntity settingsEntity = new()
            {
                Id = settings.Id,
                Name = settings.Name,
                SelectedBackupFormat = settings.SelectedBackupFormat,
                Size = settings.Size,
                Culture = settings.Culture,
                Theme = settings.Theme,
                Background = settings.Background,
                ShowAllGoals = settings.ShowAllGoals,
                ShowAllTasks = settings.ShowAllTasks,
                ShowPriority = settings.ShowPriority,
                ShowTaskKind = settings.ShowTaskKind,
                Sort = settings.Sort,
                Screen = settings.Screen,
                ElapsedToDesiredRatioMin = settings.ElapsedToDesiredRatioMin,
                ShowElapsedToDesiredRatioOverMin = settings.ShowElapsedToDesiredRatioOverMin,
                HideEmptyGoals = settings.HideEmptyGoals,
                HideGoalsWithSimpleText = settings.HideGoalsWithSimpleText,
                ShowCategoriesInGoalList = settings.ShowCategoriesInGoalList,
                HideCompletedTasks = settings.HideCompletedTasks
            };

            if (!_settingsDict.ContainsKey(settingsEntity.Id))
                _settingsList.Add(settingsEntity);

            _settingsDict[settingsEntity.Id] = settingsEntity;
        }
    }

    public async Task AddCategory(CategoryModel category)
    {
        CategoryEntity categoryEntity = new()
        {
            Id = category.Id,
            CategoryId = category.CategoryId,
            PreviousId = category.PreviousId,
            Name = category.Name
        };

        _categoryList.Add(categoryEntity);

        _categoryDict[categoryEntity.Id] = categoryEntity;

        await AddItems(new List<CategoryEntity>() { categoryEntity });
    }

    public async Task AddGoal(GoalModel goal)
    {
        GoalEntity goalEntity = new()
        {
            Id = goal.Id,
            CategoryId = goal.CategoryId,
            PreviousId = goal.PreviousId,
            Name = goal.Name,
            Details = goal.Details,
            CreateTaskFromEachLine = goal.CreateTaskFromEachLine
        };

        _goalList.Add(goalEntity);

        _goalDict[goalEntity.Id] = goalEntity;

        await AddItems(new List<GoalEntity>() { goalEntity });
    }

    public async Task AddTask(TaskModel task)
    {
        TaskEntity taskEntity = new()
        {
            Id = task.Id,
            GoalId = task.GoalId,
            PreviousId = task.PreviousId,
            Name = task.Name,
            DetailsText = task.DetailsText,
            CreatedAt = task.CreatedAt,
            LastTimeDoneAt = task.LastTimeDoneAt,
            AverageInterval = task.AverageInterval,
            DesiredInterval = task.DesiredInterval,
            Priority = task.Priority,
            TaskKind = task.TaskKind,
            AverageDuration = task.AverageDuration,
            DesiredDuration = task.DesiredDuration,
            DurationTimedCount = task.DurationTimedCount,
            Details = task.Details is null ? null : new DetailsEntity
            {
                Date = task.Details.Date,
                Address = task.Details.Address,
                Phone = task.Details.Phone,
                Email = task.Details.Email,
                Website = task.Details.Website,
                OpenFrom = task.Details.OpenFrom,
                OpenTill = task.Details.OpenTill,
            }
        };

        _taskList.Add(taskEntity);

        _taskDict[taskEntity.Id] = taskEntity;

        await AddItems(new List<TaskEntity>() { taskEntity });
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

        await AddItems(new List<TimeEntity>() { timeEntity });
    }

    public async Task AddSettings(SettingsModel settings)
    {
        SettingsEntity settingsEntity = new()
        {
            Id = settings.Id,
            Name = settings.Name,
            SelectedBackupFormat = settings.SelectedBackupFormat,
            Size = settings.Size,
            Culture = settings.Culture,
            Theme = settings.Theme,
            Background = settings.Background,
            ShowAllGoals = settings.ShowAllGoals,
            ShowAllTasks = settings.ShowAllTasks,
            ShowPriority = settings.ShowPriority,
            ShowTaskKind = settings.ShowTaskKind,
            Sort = settings.Sort,
            Screen = settings.Screen,
            ElapsedToDesiredRatioMin = settings.ElapsedToDesiredRatioMin,
            ShowElapsedToDesiredRatioOverMin = settings.ShowElapsedToDesiredRatioOverMin,
            HideEmptyGoals = settings.HideEmptyGoals,
            HideGoalsWithSimpleText = settings.HideGoalsWithSimpleText,
            ShowCategoriesInGoalList = settings.ShowCategoriesInGoalList,
            HideCompletedTasks = settings.HideCompletedTasks
        };

        _settingsList.Add(settingsEntity);

        _settingsDict[settingsEntity.Id] = settingsEntity;

        await AddItems(new List<SettingsEntity>() { settingsEntity });
    }

    public async Task UpdateCategory(CategoryModel category)
    {
        if (_categoryDict.TryGetValue(category.Id, out CategoryEntity? categoryEntity))
        {
            categoryEntity.CategoryId = category.CategoryId;
            categoryEntity.PreviousId = category.PreviousId;
            categoryEntity.Name = category.Name;

            await UpdateItems(new List<CategoryEntity> { categoryEntity });
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
            goalEntity.PreviousId = goal.PreviousId;
            goalEntity.Name = goal.Name;
            goalEntity.Details = goal.Details;
            goalEntity.CreateTaskFromEachLine = goal.CreateTaskFromEachLine;

            await UpdateItems(new List<GoalEntity> { goalEntity });
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
            taskEntity.GoalId = task.GoalId;
            taskEntity.PreviousId = task.PreviousId;
            taskEntity.Name = task.Name;
            taskEntity.DetailsText = task.DetailsText;
            taskEntity.CreatedAt = task.CreatedAt;
            taskEntity.LastTimeDoneAt = task.LastTimeDoneAt;
            taskEntity.AverageInterval = task.AverageInterval;
            taskEntity.DesiredInterval = task.DesiredInterval;
            taskEntity.Priority = task.Priority;
            taskEntity.TaskKind = task.TaskKind;
            taskEntity.AverageDuration = task.AverageDuration;
            taskEntity.DesiredDuration = task.DesiredDuration;
            taskEntity.DurationTimedCount = task.DurationTimedCount;

            if (task.Details is null)
            {
                taskEntity.Details = null;
            }
            else if (taskEntity.Details is null)
            {
                taskEntity.Details = new DetailsEntity
                {
                    Date = task.Details.Date,
                    Address = task.Details.Address,
                    Phone = task.Details.Phone,
                    Email = task.Details.Email,
                    Website = task.Details.Website,
                    OpenFrom = task.Details.OpenFrom,
                    OpenTill = task.Details.OpenTill,
                };
            }
            else
            {
                taskEntity.Details.Date = task.Details.Date;
                taskEntity.Details.Address = task.Details.Address;
                taskEntity.Details.Phone = task.Details.Phone;
                taskEntity.Details.Email = task.Details.Email;
                taskEntity.Details.Website = task.Details.Website;
                taskEntity.Details.OpenFrom = task.Details.OpenFrom;
                taskEntity.Details.OpenTill = task.Details.OpenTill;
            }

            await UpdateItems(new List<TaskEntity> { taskEntity });
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

    public async Task UpdateSettings(SettingsModel settings)
    {
        if (_settingsDict.TryGetValue(settings.Id, out SettingsEntity? settingsEntity))
        {
            settingsEntity.Name = settings.Name;
            settingsEntity.SelectedBackupFormat = settings.SelectedBackupFormat;
            settingsEntity.Size = settings.Size;
            settingsEntity.Culture = settings.Culture;
            settingsEntity.Theme = settings.Theme;
            settingsEntity.Background = settings.Background;
            settingsEntity.ShowAllGoals = settings.ShowAllGoals;
            settingsEntity.ShowAllTasks = settings.ShowAllTasks;
            settingsEntity.ShowPriority = settings.ShowPriority;
            settingsEntity.ShowTaskKind = settings.ShowTaskKind;
            settingsEntity.Sort = settings.Sort;
            settingsEntity.Screen = settings.Screen;
            settingsEntity.ElapsedToDesiredRatioMin = settings.ElapsedToDesiredRatioMin;
            settingsEntity.ShowElapsedToDesiredRatioOverMin = settings.ShowElapsedToDesiredRatioOverMin;
            settingsEntity.HideEmptyGoals = settings.HideEmptyGoals;
            settingsEntity.HideGoalsWithSimpleText = settings.HideGoalsWithSimpleText;
            settingsEntity.ShowCategoriesInGoalList = settings.ShowCategoriesInGoalList;
            settingsEntity.HideCompletedTasks = settings.HideCompletedTasks;

            await UpdateItems(new List<SettingsEntity> { settingsEntity });
        }
        else
        {
            throw new ArgumentException($"Settings {settings.Id} doesn't exist!");
        }
    }

    public async Task DeleteCategory(long id)
    {
        _categoryList.Remove(_categoryDict[id]);

        _categoryDict.Remove(id);

        await DeleteByKey<long, CategoryEntity>(id);
    }

    public async Task DeleteGoal(long id)
    {
        _goalList.Remove(_goalDict[id]);

        _goalDict.Remove(id);

        await DeleteByKey<long, GoalEntity>(id);
    }

    public async Task DeleteTask(long id)
    {
        _taskList.Remove(_taskDict[id]);

        _taskDict.Remove(id);

        await DeleteByKey<long, TaskEntity>(id);
    }

    public async Task DeleteTime(long id)
    {
        DateTime time = _timeDict[id].Time;

        _timeList.Remove(_timeDict[id]);

        _timeDict.Remove(id);

        await DeleteByKey<DateTime, TimeEntity>(time);
    }

    public async Task DeleteSettings(long id)
    {
        _settingsList.Remove(_settingsDict[id]);

        _settingsDict.Remove(id);

        await DeleteByKey<long, SettingsEntity>(id);
    }

    public async Task DeleteAll()
    {
        _categoryList.Clear();
        _goalList.Clear();
        _taskList.Clear();
        _timeList.Clear();

        _categoryDict.Clear();
        _goalDict.Clear();
        _taskDict.Clear();
        _timeDict.Clear();

        await DeleteAll<CategoryEntity>();
        await DeleteAll<GoalEntity>();
        await DeleteAll<TaskEntity>();
        await DeleteAll<TimeEntity>();

        _dbModelId = -1;
    }
}
