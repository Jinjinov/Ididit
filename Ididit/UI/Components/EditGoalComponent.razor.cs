using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class EditGoalComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public GoalModel? Goal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> GoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditGoalChanged { get; set; }

    Blazorise.TextEdit? _textEdit;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (EditGoal == Goal && _textEdit != null)
        {
            await _textEdit.Focus();
        }
    }

    async Task EditName()
    {
        if (Goal != null)
        {
            EditGoal = Goal;
            await EditGoalChanged.InvokeAsync(EditGoal);
        }
    }

    async Task CancelEdit()
    {
        EditGoal = null;
        await EditGoalChanged.InvokeAsync(EditGoal);
    }

    async Task SaveName()
    {
        EditGoal = null;
        await EditGoalChanged.InvokeAsync(EditGoal);

        if (Goal != null)
            await _repository.UpdateGoal(Goal.Id);

        await GoalChanged.InvokeAsync(Goal);
    }

    async Task DeleteGoal()
    {
        if (Goal == null)
            return;

        if (_repository.AllCategories.TryGetValue(Goal.CategoryId, out CategoryModel? parent))
            parent.GoalList.Remove(Goal);

        await _repository.DeleteGoal(Goal.Id);

        Goal = null;
        await GoalChanged.InvokeAsync(Goal);
    }
}
