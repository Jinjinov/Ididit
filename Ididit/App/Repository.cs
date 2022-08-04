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
    public long MaxSettingsId => _settingsDict.Keys.DefaultIfEmpty().Max();

    public IReadOnlyDictionary<long, CategoryModel> AllCategories => _categoryDict;
    public IReadOnlyDictionary<long, GoalModel> AllGoals => _goalDict;
    public IReadOnlyDictionary<long, TaskModel> AllTasks => _taskDict;
    public IReadOnlyDictionary<long, SettingsModel> AllSettings => _settingsDict;

    public CategoryModel Category { get; set; } = new();
    public SettingsModel Settings { get; set; } = new();

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
        if (_databaseAccess.IsInitialized)
            return;

        await _databaseAccess.Initialize();

        RepositoryData data = await _databaseAccess.GetData();

        CategoryList = data.CategoryList;
        SettingsList = data.SettingsList;

        _categoryDict = data.CategoryDict;
        _goalDict = data.GoalDict;
        _settingsDict = data.SettingsDict;
        _taskDict = data.TaskDict;

        if (!CategoryList.Any())
        {
            CategoryModel category = CreateCategory();

            category.Name = "ididit!";

            await AddCategory(category);
        }

        Category = CategoryList.First();

        if (!SettingsList.Any())
        {
            SettingsModel settings = new()
            {
                Id = MaxSettingsId + 1,
                Name = "ididit!",
                Theme = "default"
            };

            SettingsList.Add(settings);

            await AddSettings(settings);
        }

        Settings = SettingsList.First();
    }

    public async Task AddData(IDataModel data)
    {
        AddCategoryList(data.CategoryList);

        foreach (SettingsModel settings in data.SettingsList)
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
            Id = MaxCategoryId + 1,
            CategoryId = null,
            PreviousId = CategoryList.Any() ? CategoryList.Last().Id : null
        };

        CategoryList.Add(category);

        return category;
    }

    public async Task AddCategory(CategoryModel category)
    {
        _categoryDict[category.Id] = category;

        await _databaseAccess.AddCategory(category);
    }

    public async Task AddGoal(GoalModel goal)
    {
        _goalDict[goal.Id] = goal;

        await _databaseAccess.AddGoal(goal);
    }

    public async Task AddTask(TaskModel task)
    {
        _taskDict[task.Id] = task;

        await _databaseAccess.AddTask(task);
    }

    public async Task AddTime(DateTime time, long taskId)
    {
        await _databaseAccess.AddTime(time, taskId);
    }

    public async Task AddSettings(SettingsModel settings)
    {
        _settingsDict[settings.Id] = settings;

        await _databaseAccess.AddSettings(settings);
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

    public async Task UpdateSettings(long id)
    {
        if (_settingsDict.TryGetValue(id, out SettingsModel? settings))
        {
            await _databaseAccess.UpdateSettings(settings);
        }
        else
        {
            throw new ArgumentException($"Settings {id} doesn't exist!");
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

    public async Task DeleteSettings(long id)
    {
        SettingsList.Remove(_settingsDict[id]);

        _settingsDict.Remove(id);

        await _databaseAccess.DeleteSettings(id);
    }

    public async Task DeleteAll()
    {
        CategoryList.Clear();
        _categoryDict.Clear();
        _goalDict.Clear();
        _taskDict.Clear();

        await _databaseAccess.DeleteAll();

        await Initialize();
    }

    public async Task LoadExamples()
    {
        await AddCategory(new CategoryModel() { Id = 2, CategoryId = 1, Name = "Accomplishments" });
        await AddCategory(new CategoryModel() { Id = 3, CategoryId = 1, Name = "Health" });
        await AddCategory(new CategoryModel() { Id = 4, CategoryId = 1, Name = "Well-being" });

        await AddGoal(new GoalModel() { Id = 5, CategoryId = 2, Name = "Chores" });
        await AddGoal(new GoalModel() { Id = 6, CategoryId = 2, Name = "Hobbies" });
        await AddGoal(new GoalModel() { Id = 7, CategoryId = 2, Name = "Personal growth" });

        await AddGoal(new GoalModel() { Id = 8, CategoryId = 3, Name = "Appearance" });
        await AddGoal(new GoalModel() { Id = 9, CategoryId = 3, Name = "Food" });
        await AddGoal(new GoalModel() { Id = 10, CategoryId = 3, Name = "Sports" });

        await AddGoal(new GoalModel() { Id = 11, CategoryId = 4, Name = "Peace of mind" });
        await AddGoal(new GoalModel() { Id = 12, CategoryId = 4, Name = "Relationships" });
        await AddGoal(new GoalModel() { Id = 13, CategoryId = 4, Name = "Relaxation" });

        await AddTask(new TaskModel() { Id = 1, GoalId = 5, Name = "Clean dust under the bed", DesiredTime = new TimeSpan(14, 0, 0, 0) });
        await AddTask(new TaskModel() { Id = 2, GoalId = 5, Name = "Clean the windows", DesiredTime = new TimeSpan(56, 0, 0, 0) });

        await AddTask(new TaskModel() { Id = 3, GoalId = 6, Name = "Go salsa dancing" });
        await AddTask(new TaskModel() { Id = 4, GoalId = 6, Name = "Play the piano" });

        await AddTask(new TaskModel() { Id = 5, GoalId = 7, Name = "Attend a cooking workshop", DesiredTime = new TimeSpan(182, 0, 0, 0), Priority = Priority.Low });
        await AddTask(new TaskModel() { Id = 6, GoalId = 7, Name = "Learn Spanish", DesiredTime = new TimeSpan(0, 8, 0, 0), Priority = Priority.High });

        await AddTask(new TaskModel() { Id = 7, GoalId = 8, Name = "Go to a hairdresser", DesiredTime = new TimeSpan(21, 0, 0, 0) });
        await AddTask(new TaskModel() { Id = 8, GoalId = 8, Name = "Buy new clothes", DesiredTime = new TimeSpan(56, 0, 0, 0) });

        await AddTask(new TaskModel() { Id = 9, GoalId = 9, Name = "Drink a glass of water", DesiredTime = new TimeSpan(0, 8, 0, 0), Priority = Priority.VeryHigh });
        await AddTask(new TaskModel() { Id = 10, GoalId = 9, Name = "Eat a piece of fruit", DesiredTime = new TimeSpan(0, 12, 0, 0), Priority = Priority.High });

        await AddTask(new TaskModel() { Id = 11, GoalId = 10, Name = "Stretch & workout", Priority = Priority.High });
        await AddTask(new TaskModel() { Id = 12, GoalId = 10, Name = "Go hiking", DesiredTime = new TimeSpan(7, 0, 0, 0) });

        await AddTask(new TaskModel() { Id = 13, GoalId = 11, Name = "Take a walk" });
        await AddTask(new TaskModel() { Id = 14, GoalId = 11, Name = "Meditate", Priority = Priority.Low });

        await AddTask(new TaskModel() { Id = 15, GoalId = 12, Name = "Call parents", Priority = Priority.High });
        await AddTask(new TaskModel() { Id = 16, GoalId = 12, Name = "Do someone a favor", DesiredTime = new TimeSpan(14, 0, 0, 0) });

        await AddTask(new TaskModel() { Id = 17, GoalId = 13, Name = "Read a book", Priority = Priority.High });
        await AddTask(new TaskModel() { Id = 18, GoalId = 13, Name = "Get a massage", DesiredTime = new TimeSpan(28, 0, 0, 0), Priority = Priority.Low });

        await AddTime(DateTime.Now.AddDays(-50), 5);
        await AddTime(DateTime.Now.AddDays(-28), 5);

        await AddTime(DateTime.Now.AddDays(-28), 12);
        await AddTime(DateTime.Now.AddDays(-7), 12);

        await AddTime(DateTime.Now.AddDays(-27), 15);
        await AddTime(DateTime.Now.AddDays(-13), 15);

        await AddTime(DateTime.Now.AddDays(-70), 16);
        await AddTime(DateTime.Now.AddDays(-12), 16);

        await AddTime(DateTime.Now.AddDays(-300), 17);
    }
}
