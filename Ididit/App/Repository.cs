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

    public async Task Initialize()
    {
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

    public async Task AddTime(DateTime time, long taskId)
    {
        await _databaseAccess.AddTime(time, taskId);
    }

    public async Task UpdateCategory(long id)
    {
        if (_categoryDict.TryGetValue(id, out CategoryModel? category))
        {
            await _databaseAccess.UpdateCategory(category);
        }
        else
        {
            throw new ArgumentException($"Category {id} doesn't exist!");
        }
    }

    public async Task UpdateGoal(long id)
    {
        if (_goalDict.TryGetValue(id, out GoalModel? goal))
        {
            await _databaseAccess.UpdateGoal(goal);
        }
        else
        {
            throw new ArgumentException($"Goal {id} doesn't exist!");
        }
    }

    public async Task UpdateTask(long id)
    {
        if (_taskDict.TryGetValue(id, out TaskModel? task))
        {
            await _databaseAccess.UpdateTask(task);
        }
        else
        {
            throw new ArgumentException($"Task {id} doesn't exist!");
        }
    }

    public async Task UpdateTime(long id, DateTime time, long taskId)
    {
        await _databaseAccess.UpdateTime(id, time, taskId);
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
        foreach (DateTime time in _taskDict[id].TimeList)
        {
            await DeleteTime(time.Ticks);
        }

        _taskDict.Remove(id);

        await _databaseAccess.DeleteTask(id);
    }

    public async Task DeleteTime(long id)
    {
        await _databaseAccess.DeleteTime(id);
    }
}
