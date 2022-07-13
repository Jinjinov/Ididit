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

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    [EditorRequired]
    public TaskModel Task { get; set; } = null!;

    [Parameter]
    public TaskModel? SelectedTask { get; set; }

    [Parameter]
    public EventCallback<TaskModel?> SelectedTaskChanged { get; set; }

    bool showTime;
    bool editTime;
    long SelectedTime;
    DateTime EditTime;

    async Task ToggleTask()
    {
        if (SelectedTask != Task)
            SelectedTask = Task;
        else
            SelectedTask = null;

        await SelectedTaskChanged.InvokeAsync(SelectedTask);
    }

    void ToggleShowTime()
    {
        showTime = !showTime;
    }

    async Task PriorityChanged(Priority priority)
    {
        Task.Priority = priority;

        await _repository.UpdateTask(Task.Id);
    }

    async Task SetDesiredInterval()
    {
        Task.DesiredInterval = 864000000000;

        await _repository.UpdateTask(Task.Id);

        if (SelectedTask != Task)
        {
            SelectedTask = Task;

            await SelectedTaskChanged.InvokeAsync(SelectedTask);
        }
    }

    async Task ClearDesiredInterval()
    {
        Task.DesiredInterval = 0;

        await _repository.UpdateTask(Task.Id);
    }

    async Task OnDone()
    {
        (DateTime time, long taskId) = Task.AddTime(DateTime.Now);

        await _repository.AddTime(time, taskId);

        await _repository.UpdateTask(Task.Id);
    }

    async Task SaveTime(DateTime time)
    {
        Task.UpdateTime(time, EditTime);

        await _repository.UpdateTime(time.Ticks, EditTime, Task.Id);

        await _repository.UpdateTask(Task.Id);
    }

    async Task DeleteTime(DateTime time)
    {
        Task.RemoveTime(time);

        await _repository.DeleteTime(time.Ticks);

        await _repository.UpdateTask(Task.Id);
    }

    async Task SetDesiredIntervalDays(int? days)
    {
        Task.DesiredTime = new TimeSpan(days ?? 0, Task.DesiredTime.Hours, Task.DesiredTime.Minutes, Task.DesiredTime.Seconds);

        await _repository.UpdateTask(Task.Id);
    }

    async Task SetDesiredIntervalHours(int? hours)
    {
        Task.DesiredTime = new TimeSpan(Task.DesiredTime.Days, hours ?? 0, Task.DesiredTime.Minutes, Task.DesiredTime.Seconds);

        await _repository.UpdateTask(Task.Id);
    }

    static string ToReadableString(TimeSpan span)
    {
        return span.TotalMinutes >= 1.0 ? (
            (span.Days > 0 ? span.Days + " d" + (span.Hours > 0 || span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
            (span.Hours > 0 ? span.Hours + " h" + (span.Minutes > 0 ? ", " : string.Empty) : string.Empty) +
            (span.Minutes > 0 ? span.Minutes + " m" : string.Empty)
            ) : "0 minutes";
    }

    static string ToHighestValueString(TimeSpan span)
    {
        return span.Days > 0 ? span.Days + " day" + (span.Days == 1 ? string.Empty : "s")
                             : span.Hours > 0 ? span.Hours + " hour" + (span.Hours == 1 ? string.Empty : "s")
                                              : span.Minutes + " minute" + (span.Minutes == 1 ? string.Empty : "s");
    }
}
