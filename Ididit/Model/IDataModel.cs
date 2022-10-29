using Ididit.Model.Models;
using System.Collections.Generic;

namespace Ididit.Model;

public interface IDataModel
{
    List<CategoryModel> CategoryList { get; set; }
    List<SettingsModel> SettingsList { get; set; }
}
