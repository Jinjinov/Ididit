using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Pages;

public partial class Index
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    CategoryModel? _selectedCategory;

    Filters _filters = new();

    protected override void OnInitialized()
    {
        _filters = new()
        {
            Sort = Repository.Settings.Sort,
            ElapsedToDesiredRatioMin = Repository.Settings.ElapsedToDesiredRatioMin,
            ShowElapsedToDesiredRatioOverMin = Repository.Settings.ShowElapsedToDesiredRatioOverMin,
            HideEmptyGoals = Repository.Settings.HideEmptyGoals,
            ShowCategoriesInGoalList = Repository.Settings.ShowCategoriesInGoalList,
            AlsoShowCompletedAsap = Repository.Settings.AlsoShowCompletedAsap,
        };
    }
}
