using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class CategoriesComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [Parameter]
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    IList<CategoryModel?> _expandedNodes = new List<CategoryModel?>();

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

    static IList<CategoryModel>? GetChildNodes(CategoryModel? item) => item?.CategoryList;

    static bool HasChildNodes(CategoryModel? item) => item?.CategoryList?.Any() == true;

    async Task OnSelectedCategoryChanged(CategoryModel? category)
    {
        SelectedCategory = category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    static void NodeStyling(CategoryModel? item, Blazorise.TreeView.NodeStyling style)
    {
        style.TextColor = (item?.CategoryList?.Any() == true) ? Blazorise.TextColor.Primary : Blazorise.TextColor.Default;
        style.Style = "font-weight:bold";
    }

    static void SelectedNodeStyling(CategoryModel? item, Blazorise.TreeView.NodeStyling style)
    {
        style.Style = "padding:0!important";
    }

    async Task ShowAllGoals()
    {
        SelectedCategory = null;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    void ToggleAll()
    {
        if (_expandedNodes.Any())
        {
            _expandedNodes.Clear();
        }
        else
        {
            _expandedNodes = new List<CategoryModel?>(Repository.CategoryList);
        }
    }

    async Task NewCategory()
    {
        CategoryModel category = SelectedCategory != null ? SelectedCategory.CreateCategory(Repository.MaxCategoryId + 1) : Repository.CreateCategory();

        await Repository.AddCategory(category);

        if (SelectedCategory != null)
        {
            if (!_expandedNodes.Contains(SelectedCategory))
            {
                _expandedNodes.Add(SelectedCategory);
            }
        }

        _editCategory = category;

        await OnSelectedCategoryChanged(category);
    }
}
