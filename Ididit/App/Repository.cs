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
            SettingsModel settings = new()
            {
                Id = NextSettingsId,
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
        List<CategoryModel> categories = new()
        {
            { Category.CreateCategory(NextCategoryId, "Accomplishments") },
            { Category.CreateCategory(NextCategoryId, "Health") },
            { Category.CreateCategory(NextCategoryId, "Well-being") }
        };

        foreach (CategoryModel category in categories)
            await AddCategory(category);

        List<GoalModel> goals = new()
        {
            { categories[0].CreateGoal(NextGoalId, "Chores") },
            { categories[0].CreateGoal(NextGoalId, "Hobbies") },
            { categories[0].CreateGoal(NextGoalId, "Personal growth") },

            { categories[1].CreateGoal(NextGoalId, "Appearance") },
            { categories[1].CreateGoal(NextGoalId, "Food") },
            { categories[1].CreateGoal(NextGoalId, "Sports") },

            { categories[2].CreateGoal(NextGoalId, "Peace of mind") },
            { categories[2].CreateGoal(NextGoalId, "Relationships") },
            { categories[2].CreateGoal(NextGoalId, "Relaxation") }
        };

        foreach (GoalModel goal in goals)
            await AddGoal(goal);

        List<TaskModel> tasks = new()
        {
            { goals[0].CreateTask(NextTaskId, "Clean dust under the bed", TimeSpan.FromDays(14), Priority.High) },
            { goals[0].CreateTask(NextTaskId, "Clean the windows", TimeSpan.FromDays(90), Priority.High) },

            { goals[1].CreateTask(NextTaskId, "Go salsa dancing") },
            { goals[1].CreateTask(NextTaskId, "Play the piano", TimeSpan.FromDays(7), Priority.Medium) },

            { goals[2].CreateTask(NextTaskId, "Attend a cooking workshop") },
            { goals[2].CreateTask(NextTaskId, "Learn Spanish", TimeSpan.FromDays(1), Priority.High) },

            { goals[3].CreateTask(NextTaskId, "Go to a hairdresser", TimeSpan.FromDays(21), Priority.High) },
            { goals[3].CreateTask(NextTaskId, "Buy new clothes") },

            { goals[4].CreateTask(NextTaskId, "Drink a glass of water", TimeSpan.FromHours(8), Priority.VeryHigh) },
            { goals[4].CreateTask(NextTaskId, "Eat a piece of fruit", TimeSpan.FromHours(12), Priority.High) },

            { goals[5].CreateTask(NextTaskId, "Stretch & workout", TimeSpan.FromDays(1), Priority.High) },
            { goals[5].CreateTask(NextTaskId, "Go hiking", TimeSpan.FromDays(7), Priority.Medium) },

            { goals[6].CreateTask(NextTaskId, "Take a walk", TimeSpan.FromDays(7), Priority.Medium) },
            { goals[6].CreateTask(NextTaskId, "Meditate", TimeSpan.FromDays(7), Priority.Medium) },

            { goals[7].CreateTask(NextTaskId, "Call parents", TimeSpan.FromDays(7), Priority.High) },
            { goals[7].CreateTask(NextTaskId, "Do someone a favor") },

            { goals[8].CreateTask(NextTaskId, "Read a book", TimeSpan.FromDays(1), Priority.High) },
            { goals[8].CreateTask(NextTaskId, "Get a massage", TimeSpan.FromDays(28), Priority.Low) }
        };

        foreach (TaskModel task in tasks)
            await AddTask(task);

        List<(DateTime time, long taskId)> times = new()
        {
            { tasks[0].AddTime(DateTime.Now.AddDays(-50)) },
            { tasks[0].AddTime(DateTime.Now.AddDays(-28)) },

            { tasks[3].AddTime(DateTime.Now.AddDays(-28)) },
            { tasks[3].AddTime(DateTime.Now.AddDays(-7)) },

            { tasks[6].AddTime(DateTime.Now.AddDays(-27)) },
            { tasks[6].AddTime(DateTime.Now.AddDays(-13)) },

            { tasks[11].AddTime(DateTime.Now.AddDays(-70)) },
            { tasks[11].AddTime(DateTime.Now.AddDays(-12)) },

            { tasks[17].AddTime(DateTime.Now.AddDays(-300)) }
        };

        foreach ((DateTime time, long taskId) in times)
            await AddTime(time, taskId);
    }
}
