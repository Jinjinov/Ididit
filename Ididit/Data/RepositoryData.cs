using Ididit.Data.Model.Models;
using System.Collections.Generic;

namespace Ididit.Data;

internal class RepositoryData
{
    public List<CategoryModel> CategoryList { get; set; } = new();
    public List<SettingsModel> SettingsList { get; set; } = new();

    public Dictionary<long, CategoryModel> CategoryDict { get; set; } = new();
    public Dictionary<long, GoalModel> GoalDict { get; set; } = new();
    public Dictionary<long, SettingsModel> SettingsDict { get; set; } = new();
    public Dictionary<long, TaskModel> TaskDict { get; set; } = new();
}
