using Ididit.Data.Model.Models;
using System.Collections.Generic;

namespace Ididit.Data.Model;

public interface IDataModel
{
    List<CategoryModel> CategoryList { get; set; }
    List<SettingsModel> SettingsList { get; set; }
}
