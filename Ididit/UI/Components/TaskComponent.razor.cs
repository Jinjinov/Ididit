using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class TaskComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Inject]
    Theme Theme { get; set; } = null!;

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

    async Task OnDone()
    {
        (DateTime time, long taskId) = Task.AddTime();

        await _repository.AddTime(time, taskId);
    }
}
