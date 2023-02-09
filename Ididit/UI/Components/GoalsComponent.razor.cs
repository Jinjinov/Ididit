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

    [Parameter]
    public GoalModel? EditDetailsGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel?> EditDetailsGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditNameGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditNameGoalChanged { get; set; }

    [Parameter]
    public bool AutoSize { get; set; }

    string MaxWidth => Repository.Settings.MaxWidth.HasValue ? $"max-width:{Repository.Settings.MaxWidth.Value}px" : string.Empty;

    TaskModel? _selectedTask;

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    async Task NewGoal()
    {
        GoalModel goal = SelectedCategory.CreateGoal(Repository.NextGoalId, string.Empty);

        (GoalModel? firstGoal, GoalModel? nextGoal) = SelectedCategory.MoveGoalToStart(goal);

        if (firstGoal is not null)
            await Repository.UpdateGoal(firstGoal.Id);

        if (nextGoal is not null)
            await Repository.UpdateGoal(nextGoal.Id);

        await Repository.AddGoal(goal);

        EditNameGoal = goal;
        await EditNameGoalChanged.InvokeAsync(EditNameGoal);

        EditDetailsGoal = goal;
        await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
    }
}
