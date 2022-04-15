using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class TaskComponent
{
    [Parameter]
    [EditorRequired]
    public TaskModel Task { get; set; } = null!;

    [Parameter]
    public TaskModel? SelectedTask { get; set; }

    [Parameter]
    public EventCallback<TaskModel?> SelectedTaskChanged { get; set; }

    async Task SelectTask()
    {
        SelectedTask = Task;

        await SelectedTaskChanged.InvokeAsync(SelectedTask);
    }
}
