using Ididit.Data;
using Ididit.Data.Models;
using Ididit.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.App;

internal class Repository : DataModel, IRepository
{
    public long MaxCategoryId => _categoryDict.Keys.DefaultIfEmpty().Max();
    public long MaxGoalId => _goalDict.Keys.DefaultIfEmpty().Max();
    public long MaxTaskId => _taskDict.Keys.DefaultIfEmpty().Max();

    public IReadOnlyDictionary<long, CategoryModel> AllCategories => _categoryDict;
    public IReadOnlyDictionary<long, GoalModel> AllGoals => _goalDict;
    public IReadOnlyDictionary<long, TaskModel> AllTasks => _taskDict;

    private Dictionary<long, CategoryModel> _categoryDict = new();
    private Dictionary<long, GoalModel> _goalDict = new();
    private Dictionary<long, TaskModel> _taskDict = new();
    private Dictionary<long, SettingsModel> _settingsDict = new();

    private readonly IDatabaseAccess _databaseAccess;

    public Repository(IDatabaseAccess databaseAccess)
    {
        _databaseAccess = databaseAccess;
    }

    /// <summary>
    /// Should be called from SetParametersAsync()
    /// Calling this from OnInitializedAsync() or OnParametersSetAsync() doesn't work
    /// </summary>
    public async Task Initialize()
    {
        // This is called 2 times. If isInitialized is used to return after first call, then it doesn't work.

        await _databaseAccess.Initialize();

        RepositoryData data = await _databaseAccess.GetData();

        CategoryList = data.CategoryList;
        SettingsList = data.SettingsList;

        _categoryDict = data.CategoryDict;
        _goalDict = data.GoalDict;
        _taskDict = data.TaskDict;
    }

    public async Task AddData(IDataModel data)
    {
        AddCategoryList(data.CategoryList);

        foreach (var settings in data.SettingsList)
        {
            if (!_settingsDict.ContainsKey(settings.Id))
                SettingsList.Add(settings);

            _settingsDict[settings.Id] = settings;
        }

        await _databaseAccess.AddData(data);
    }

    private void AddCategoryList(List<CategoryModel> categoryList)
    {
        foreach (CategoryModel category in categoryList)
        {
            if (!_categoryDict.ContainsKey(category.Id))
                CategoryList.Add(category);

            _categoryDict[category.Id] = category;

            foreach (GoalModel goal in category.GoalList)
            {
                _goalDict[goal.Id] = goal;

                foreach (TaskModel task in goal.TaskList)
                {
                    _taskDict[task.Id] = task;
                }
            }

            if (category.CategoryList.Any())
            {
                AddCategoryList(category.CategoryList);
            }
        }
    }

    public CategoryModel CreateCategory()
    {
        CategoryModel category = new()
        {
            CategoryId = null,
            Name = "Category " + CategoryList.Count
        };

        CategoryList.Add(category);

        return category;
    }

    public async Task AddCategory(CategoryModel category)
    {
        category.Id = MaxCategoryId + 1;

        _categoryDict[category.Id] = category;

        await _databaseAccess.AddCategory(category);
    }

    public async Task AddGoal(GoalModel goal)
    {
        goal.Id = MaxGoalId + 1;

        _goalDict[goal.Id] = goal;

        await _databaseAccess.AddGoal(goal);
    }

    public async Task AddTask(TaskModel task)
    {
        task.Id = MaxTaskId + 1;

        _taskDict[task.Id] = task;

        await _databaseAccess.AddTask(task);
    }

    public async Task AddTime(long ticks, long taskId)
    {
        await _databaseAccess.AddTime(ticks, taskId);
    }

    public async Task UpdateCategoryName(long id, string name)
    {
        if (_categoryDict.TryGetValue(id, out CategoryModel? category))
        {
            category.Name = name;

            await _databaseAccess.UpdateCategoryName(id, name);
        }
        else
        {
            throw new ArgumentException($"Category {id} doesn't exist!");
        }
    }

    public async Task UpdateGoalName(long id, string name)
    {
        if (_goalDict.TryGetValue(id, out GoalModel? goal))
        {
            goal.Name = name;

            await _databaseAccess.UpdateGoalName(id, name);
        }
        else
        {
            throw new ArgumentException($"Goal {id} doesn't exist!");
        }
    }

    public async Task UpdateTaskName(long id, string name)
    {
        if (_taskDict.TryGetValue(id, out TaskModel? task))
        {
            task.Name = name;

            await _databaseAccess.UpdateTaskName(id, name);
        }
        else
        {
            throw new ArgumentException($"Task {id} doesn't exist!");
        }
    }

    public async Task UpdateGoalDetails(long id, string details)
    {
        if (_goalDict.TryGetValue(id, out GoalModel? goal))
        {
            goal.Details = details;

            await _databaseAccess.UpdateGoalDetails(id, details);
        }
        else
        {
            throw new ArgumentException($"Goal {id} doesn't exist!");
        }
    }

    public async Task DeleteCategory(long id)
    {
        foreach (CategoryModel category in _categoryDict[id].CategoryList)
        {
            await DeleteCategory(category.Id);
        }

        foreach (GoalModel goal in _categoryDict[id].GoalList)
        {
            await DeleteGoal(goal.Id);
        }

        CategoryList.Remove(_categoryDict[id]);

        _categoryDict.Remove(id);

        await _databaseAccess.DeleteCategory(id);
    }

    public async Task DeleteGoal(long id)
    {
        foreach (TaskModel task in _goalDict[id].TaskList)
        {
            await DeleteTask(task.Id);
        }

        _goalDict.Remove(id);

        await _databaseAccess.DeleteGoal(id);
    }

    public async Task DeleteTask(long id)
    {
        foreach (long time in _taskDict[id].TimeList)
        {
            await DeleteTime(time);
        }

        _taskDict.Remove(id);

        await _databaseAccess.DeleteTask(id);
    }

    public async Task DeleteTime(long id)
    {
        await _databaseAccess.DeleteTime(id);
    }
}
