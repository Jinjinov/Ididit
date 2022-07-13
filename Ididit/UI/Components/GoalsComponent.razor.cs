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
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    Filters _filters = new();

    GoalModel? _selectedGoal;

    GoalModel? _editGoal;

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;
        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    async Task NewGoal()
    {
        if (SelectedCategory != null)
        {
            GoalModel goal = SelectedCategory.CreateGoal(Repository.MaxGoalId + 1);

            await Repository.AddGoal(goal);

            _selectedGoal = goal;
            _editGoal = goal;
        }
    }
}
