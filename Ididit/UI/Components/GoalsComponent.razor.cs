using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalsComponent
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
    public Filters Filters { get; set; } = null!;

    TaskModel? _selectedTask;

    GoalModel? _selectedGoal;

    GoalModel? _editGoal;

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    async Task NewGoal()
    {
        GoalModel goal = SelectedCategory.CreateGoal(Repository.NextGoalId, string.Empty);

        await Repository.AddGoal(goal);

        _selectedGoal = goal;
        _editGoal = goal;
    }
}
