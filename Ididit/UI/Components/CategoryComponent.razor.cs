using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class CategoryComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Inject]
    Theme Theme { get; set; } = null!;

    [Parameter]
    public CategoryModel? Category { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> CategoryChanged { get; set; }

    public bool editCategory;

    void EditCategory()
    {
        if (Category != null)
            editCategory = true;
    }

    void CancelEdit()
    {
        editCategory = false;
    }

    async Task NewCategory()
    {
        editCategory = true;

        CategoryModel category = Category != null ? Category.CreateCategory() : _repository.CreateCategory();

        await _repository.AddCategory(category);

        Category = category;
        await CategoryChanged.InvokeAsync(Category);
    }

    async Task SaveCategory()
    {
        editCategory = false;

        if (Category != null)
            await _repository.UpdateCategoryName(Category.Id, Category.Name);
    }

    async Task DeleteCategory()
    {
        if (Category != null)
        {
            await _repository.DeleteCategory(Category.Id);

            Category = null;
            await CategoryChanged.InvokeAsync(Category);
        }
    }
}
