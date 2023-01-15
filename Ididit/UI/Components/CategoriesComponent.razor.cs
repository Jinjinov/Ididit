using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class CategoriesComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    [Parameter]
    public SettingsModel Settings { get; set; } = null!;

    [Parameter]
    public EventCallback<SettingsModel> SettingsChanged { get; set; }

    IList<CategoryModel> _expandedNodes = new List<CategoryModel>();

    CategoryModel? _editCategory;

    bool _isFirstToggle;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!_isFirstToggle && Repository.CategoryList.Any() && !_expandedNodes.Any())
        {
            _isFirstToggle = true;
            ToggleAll();
            StateHasChanged();
        }
    }

    static IList<CategoryModel> GetChildNodes(CategoryModel item) => item.CategoryList;

    static bool HasChildNodes(CategoryModel item) => item.CategoryList.Any();

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        if (SelectedCategory != category)
        {
            SelectedCategory = category;
            await SelectedCategoryChanged.InvokeAsync(SelectedCategory);

            Settings.ShowAllGoals = false;
            Settings.ShowAllTasks = false;
            await Repository.UpdateSettings(Settings.Id);

            await SettingsChanged.InvokeAsync(Settings);
        }
    }

    async Task OnCategoryChanged(CategoryModel category)
    {
        if (category is null)
        {
            SelectedCategory = Repository.Category;
        }

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    static void NodeStyling(CategoryModel item, Blazorise.TreeView.NodeStyling style)
    {
        style.TextColor = item.CategoryList.Any() ? Blazorise.TextColor.Primary : Blazorise.TextColor.Default;
        style.Style = "font-weight:bold";
    }

    static void SelectedNodeStyling(CategoryModel item, Blazorise.TreeView.NodeStyling style)
    {
        style.Style = "padding:0!important";
    }

    async Task OnHideEmptyGoalsChanged(bool? val)
    {
        Settings.HideEmptyGoals = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnHideGoalsWithSimpleText(bool? val)
    {
        Settings.HideGoalsWithSimpleText = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnShowAllGoalsChanged(bool? val)
    {
        bool showAllGoals = val ?? false;

        if (Settings.ShowAllGoals != showAllGoals)
        {
            Settings.ShowAllGoals = showAllGoals;
            await Repository.UpdateSettings(Settings.Id);

            await SettingsChanged.InvokeAsync(Settings);

            SelectedCategory = Repository.Category;
            await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
        }
    }

    async Task OnShowCategoriesInGoalListChanged(bool? val)
    {
        Settings.ShowCategoriesInGoalList = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnShowAllTasksChanged(bool? val)
    {
        bool showAllTasks = val ?? false;

        if (Settings.ShowAllTasks != showAllTasks)
        {
            Settings.ShowAllTasks = showAllTasks;
            await Repository.UpdateSettings(Settings.Id);

            await SettingsChanged.InvokeAsync(Settings);

            SelectedCategory = Repository.Category;
            await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
        }
    }

    void ToggleAll()
    {
        if (_expandedNodes.Any())
        {
            _expandedNodes.Clear();
        }
        else
        {
            _expandedNodes = new List<CategoryModel>(Repository.CategoryList);
        }
    }

    async Task NewCategory()
    {
        CategoryModel category = SelectedCategory.CreateCategory(Repository.NextCategoryId, string.Empty);

        await Repository.AddCategory(category);

        if (!_expandedNodes.Contains(SelectedCategory))
        {
            _expandedNodes.Add(SelectedCategory);
        }

        _editCategory = category;

        await OnSelectedCategoryChanged(category);
    }
}
