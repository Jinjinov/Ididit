using Ididit.Data.Models;
using System.Collections.Generic;

namespace Ididit.App;

internal class RepositoryData
{
    public List<CategoryModel> CategoryList { get; set; } = new();
    public List<SettingsModel> SettingsList { get; set; } = new();

    public Dictionary<long, CategoryModel> CategoryDict { get; set; } = new();
    public Dictionary<long, GoalModel> GoalDict { get; set; } = new();
    public Dictionary<long, TaskModel> TaskDict { get; set; } = new();
}
