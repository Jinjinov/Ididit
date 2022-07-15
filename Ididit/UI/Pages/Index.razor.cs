using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Pages;

public partial class Index
{
    //[Inject]
    //IRepository Repository { get; set; } = null!;

    CategoryModel? _selectedCategory;

    Filters _filters = new();

    //protected override async Task OnInitializedAsync()
    //{
    //    await Repository.Initialize();

    //    StateHasChanged(); // refresh components with _repository.Settings
    //}
}
