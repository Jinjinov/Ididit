using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class CategoriesComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Parameter]
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    IList<CategoryModel?> _expandedNodes = new List<CategoryModel?>();

    CategoryModel? _editCategory;

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    void ShowAllGoals()
    {
        SelectedCategory = null;
    }

    async Task NewCategory()
    {
        CategoryModel category = SelectedCategory != null ? SelectedCategory.CreateCategory() : _repository.CreateCategory();

        await _repository.AddCategory(category);

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
