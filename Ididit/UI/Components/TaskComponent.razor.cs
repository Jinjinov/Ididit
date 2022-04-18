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

    public async Task SetDesiredIntervalDays(int? days)
    {
        Task.DesiredTime = new TimeSpan(days ?? 0, Task.DesiredTime.Hours, Task.DesiredTime.Minutes, Task.DesiredTime.Seconds);

        await _repository.UpdateTaskInterval(Task.Id, Task.DesiredInterval);
    }

    public async Task SetDesiredIntervalHours(int? hours)
    {
        Task.DesiredTime = new TimeSpan(Task.DesiredTime.Days, hours ?? 0, Task.DesiredTime.Minutes, Task.DesiredTime.Seconds);

        await _repository.UpdateTaskInterval(Task.Id, Task.DesiredInterval);
    }

    public static string ToReadableString(TimeSpan span)
    {
        return span.TotalMinutes >= 1.0 ? (
            (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
            (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
            (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
            ) : "0 minutes";
    }
}
