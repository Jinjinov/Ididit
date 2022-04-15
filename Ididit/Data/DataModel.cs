using Ididit.Data.Models;
using System.Collections.Generic;

namespace Ididit.Data;

internal class DataModel : IDataModel
{
    public List<CategoryModel> CategoryList { get; set; } = new();
    public List<SettingsModel> SettingsList { get; set; } = new();
}
