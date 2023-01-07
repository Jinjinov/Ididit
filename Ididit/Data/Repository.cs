using Ididit.Data.Database;
using Ididit.Data.Model;
using Ididit.Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.Data;

internal class Repository : DataModel, IRepository
{
    public long NextCategoryId => _categoryDict.Keys.DefaultIfEmpty().Max() + 1;
    public long NextGoalId => _goalDict.Keys.DefaultIfEmpty().Max() + 1;
    public long NextTaskId => _taskDict.Keys.DefaultIfEmpty().Max() + 1;
    public long NextSettingsId => _settingsDict.Keys.DefaultIfEmpty().Max() + 1;

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

    public event EventHandler? DataChanged;

    private readonly IDatabaseAccess _databaseAccess;

    public Repository(IDatabaseAccess databaseAccess)
    {
        _databaseAccess = databaseAccess;

        _databaseAccess.DataChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, EventArgs e)
    {
        DataChanged?.Invoke(sender, e);
    }

    public async Task Initialize()
    {
        if (_databaseAccess.IsInitialized)
            return;

        await _databaseAccess.Initialize();

        await GetData();
    }

    private async Task GetData()
    {
        RepositoryData data = await _databaseAccess.GetData();

        CategoryList = data.CategoryList;
        SettingsList = data.SettingsList;

        _categoryDict = data.CategoryDict;
        _goalDict = data.GoalDict;
        _settingsDict = data.SettingsDict;
        _taskDict = data.TaskDict;

        if (!CategoryList.Any())
        {
            CategoryModel category = CreateCategory("ididit!");

            await AddCategory(category);
        }

        Category = CategoryList.First();

        if (!SettingsList.Any())
        {
            await AddDefaultSettings();
        }

        Settings = SettingsList.First();
    }

    private async Task AddDefaultSettings()
    {
        SettingsModel settings = new()
        {
            Id = NextSettingsId,
            Name = "ididit!",
            Theme = "default"
        };

        SettingsList.Add(settings);

        await AddSettings(settings);
    }

    public async Task ResetSettings()
    {
        await DeleteSettings(Settings.Id);

        await AddDefaultSettings();

        Settings = SettingsList.First();
    }

    public async Task AddData(IDataModel data)
    {
        AddCategoryList(data.CategoryList);

        foreach (SettingsModel settings in data.SettingsList)
        {
            SettingsList.RemoveAll(s => s.Id == settings.Id);
            SettingsList.Add(settings);

            _settingsDict[settings.Id] = settings;
        }

        await _databaseAccess.AddData(data);
    }

    private void AddCategoryList(List<CategoryModel> categoryList)
    {
        foreach (CategoryModel category in categoryList)
        {
            if (category.CategoryId is null)
            {
                CategoryList.RemoveAll(c => c.Id == category.Id);
                CategoryList.Add(category);

                Category = CategoryList.First();
            }

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

    public CategoryModel CreateCategory(string name)
    {
        CategoryModel category = new()
        {
            Id = NextCategoryId,
            CategoryId = null,
            PreviousId = CategoryList.Any() ? CategoryList.Last().Id : null,
            Name = name
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
        long nextCategoryId = NextCategoryId;

        List<CategoryModel> categories = new()
        {
            { Category.CreateCategory(nextCategoryId++, "Accomplishments") },
            { Category.CreateCategory(nextCategoryId++, "Health") },
            { Category.CreateCategory(nextCategoryId++, "Well-being") }
        };

        foreach (CategoryModel category in categories)
            await AddCategory(category);

        long nextGoalId = NextGoalId;

        List<GoalModel> goals = new()
        {
            { categories[0].CreateGoal(nextGoalId++, "Chores") },
            { categories[0].CreateGoal(nextGoalId++, "Hobbies") },
            { categories[0].CreateGoal(nextGoalId++, "Personal growth") },

            { categories[1].CreateGoal(nextGoalId++, "Appearance") },
            { categories[1].CreateGoal(nextGoalId++, "Food") },
            { categories[1].CreateGoal(nextGoalId++, "Sports") },

            { categories[2].CreateGoal(nextGoalId++, "Peace of mind") },
            { categories[2].CreateGoal(nextGoalId++, "Relationships") },
            { categories[2].CreateGoal(nextGoalId++, "Relaxation") }
        };

        long nextTaskId = NextTaskId;

        List<TaskModel> tasks = new()
        {
            { goals[0].CreateTask(nextTaskId++, "Clean dust", TimeSpan.FromDays(7), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },
            { goals[0].CreateTask(nextTaskId++, "Clean the windows", TimeSpan.FromDays(90), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(20)) },

            { goals[1].CreateTask(nextTaskId++, "Go salsa dancing") },
            { goals[1].CreateTask(nextTaskId++, "Play the piano", TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },

            { goals[2].CreateTask(nextTaskId++, "Attend a workshop") },
            { goals[2].CreateTask(nextTaskId++, "Learn Spanish", TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },

            { goals[3].CreateTask(nextTaskId++, "Get a haircut", TimeSpan.FromDays(21), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },
            { goals[3].CreateTask(nextTaskId++, "Buy new clothes") },

            { goals[4].CreateTask(nextTaskId++, "Drink water", TimeSpan.FromHours(8), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(1)) },
            { goals[4].CreateTask(nextTaskId++, "Eat fruit", TimeSpan.FromHours(12), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(3)) },

            { goals[5].CreateTask(nextTaskId++, "Stretch & workout", TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(5)) },
            { goals[5].CreateTask(nextTaskId++, "Go hiking", TimeSpan.FromDays(7), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(90)) },

            { goals[6].CreateTask(nextTaskId++, "Take a walk", TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(30)) },
            { goals[6].CreateTask(nextTaskId++, "Meditate", TimeSpan.FromDays(7), Priority.Medium, TaskKind.RepeatingTask, TimeSpan.FromMinutes(15)) },

            { goals[7].CreateTask(nextTaskId++, "Call parents", TimeSpan.FromDays(7), Priority.VeryHigh, TaskKind.RepeatingTask, TimeSpan.FromMinutes(10)) },
            { goals[7].CreateTask(nextTaskId++, "Do someone a favor") },

            { goals[8].CreateTask(nextTaskId++, "Read a book", TimeSpan.FromDays(1), Priority.High, TaskKind.RepeatingTask, TimeSpan.FromMinutes(60)) },
            { goals[8].CreateTask(nextTaskId++, "Get a massage", TimeSpan.FromDays(28), Priority.Low, TaskKind.RepeatingTask, TimeSpan.FromMinutes(45)) }
        };

        foreach (GoalModel goal in goals)
        {
            foreach (TaskModel task in goal.TaskList)
            {
                goal.Details += string.IsNullOrEmpty(goal.Details) ? task.Name : Environment.NewLine + task.Name;
            }

            goal.CreateTaskFromEachLine = true;

            await AddGoal(goal);
        }

        foreach (TaskModel task in tasks)
            await AddTask(task);

        List<(DateTime time, long taskId)> times = new()
        {
            { tasks[0].AddTime(DateTime.Now.AddDays(-20)) },
            { tasks[0].AddTime(DateTime.Now.AddDays(-10)) },

            { tasks[3].AddTime(DateTime.Now.AddDays(-28)) },
            { tasks[3].AddTime(DateTime.Now.AddDays(-8)) },

            { tasks[6].AddTime(DateTime.Now.AddDays(-27)) },
            { tasks[6].AddTime(DateTime.Now.AddDays(-13)) },

            { tasks[11].AddTime(DateTime.Now.AddDays(-70)) },
            { tasks[11].AddTime(DateTime.Now.AddDays(-12)) },

            { tasks[17].AddTime(DateTime.Now.AddDays(-300)) }
        };

        foreach ((DateTime time, long taskId) in times)
        {
            await AddTime(time, taskId);
            await UpdateTask(taskId);
        }
    }
}
