using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalsComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    public GoalModel? _selectedGoal { get; set; }

    public GoalModel? _editGoal { get; set; }

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;

        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    async Task NewGoal()
    {
        if (SelectedCategory != null)
        {
            GoalModel goal = SelectedCategory.CreateGoal(_repository.MaxGoalId + 1);

            await _repository.AddGoal(goal);

            _selectedGoal = goal;

            _editGoal = goal;
        }
    }
}
