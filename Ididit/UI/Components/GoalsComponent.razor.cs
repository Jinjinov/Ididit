using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalsComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    [Parameter]
    public bool ShowAllGoals { get; set; }

    [Parameter]
    public bool ShowAllTasks { get; set; }

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
