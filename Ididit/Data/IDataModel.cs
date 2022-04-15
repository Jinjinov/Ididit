using Ididit.Data.Models;
using System.Collections.Generic;

namespace Ididit.Data;

internal interface IDataModel
{
    List<CategoryModel> CategoryList { get; set; }
    List<SettingsModel> SettingsList { get; set; }
}
