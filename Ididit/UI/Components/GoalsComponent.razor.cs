using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalsComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public CategoryModel? ParentCategory { get; set; } = null!;

    public GoalModel? _selectedGoal { get; set; }

    async Task NewGoal()
    {
        if (ParentCategory != null)
        {
            await _repository.AddGoal(ParentCategory.CreateGoal());
        }
    }

}
