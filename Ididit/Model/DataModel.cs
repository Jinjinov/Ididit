using Ididit.Model.Models;
using System.Collections.Generic;

namespace Ididit.Model;

public class DataModel : IDataModel
{
    public List<CategoryModel> CategoryList { get; set; } = new();
    public List<SettingsModel> SettingsList { get; set; } = new();
}
