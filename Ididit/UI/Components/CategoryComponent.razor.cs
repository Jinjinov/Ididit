using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class CategoryComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    //[CascadingParameter]
    //Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel? Category { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> CategoryChanged { get; set; }

    [Parameter]
    public CategoryModel? EditCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> EditCategoryChanged { get; set; }

    Blazorise.TextEdit? _textEdit;

    string _categoryName = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EditCategory == Category && _textEdit is not null)
        {
            await _textEdit.Focus();
        }
    }

    async Task EditName()
    {
        if (Category is not null)
        {
            _categoryName = Category.Name;

            EditCategory = Category;
            await EditCategoryChanged.InvokeAsync(EditCategory);
        }
    }

    async Task KeyUp(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Code == "Escape")
        {
            await CancelEdit();
        }
        else if (eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter")
        {
            await SaveName();
        }
    }

    async Task FocusOut(FocusEventArgs eventArgs)
    {
        await SaveName();
    }

    async Task CancelEdit()
    {
        _categoryName = Category?.Name ?? string.Empty;

        EditCategory = null;
        await EditCategoryChanged.InvokeAsync(EditCategory);
    }

    async Task SaveName()
    {
        EditCategory = null;
        await EditCategoryChanged.InvokeAsync(EditCategory);

        if (_categoryName != Category?.Name)
        {
            if (Category is not null)
            {
                Category.Name = _categoryName;
                await Repository.UpdateCategory(Category.Id);
            }

            await CategoryChanged.InvokeAsync(Category);
        }
    }

    async Task DeleteCategory()
    {
        if (Category is null)
            return;

        if (Category.CategoryId.HasValue && Repository.AllCategories.TryGetValue(Category.CategoryId.Value, out CategoryModel? parent))
        {
            CategoryModel? changedCategory = parent.RemoveCategory(Category);

            if (changedCategory is not null)
                await Repository.UpdateCategory(changedCategory.Id);
        }

        await Repository.DeleteCategory(Category.Id);

        Category = null;
        await CategoryChanged.InvokeAsync(Category);
    }
}
