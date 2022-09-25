using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class TaskComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    [EditorRequired]
    public TaskModel Task { get; set; } = null!;

    [Parameter]
    public TaskModel? SelectedTask { get; set; }

    [Parameter]
    public EventCallback<TaskModel?> SelectedTaskChanged { get; set; }

    bool _showTime;
    bool _editTime;
    long _selectedTime;
    DateTime _taskTime;

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
        _showTime = !_showTime;
    }

    async Task PriorityChanged(Priority priority)
    {
        Task.Priority = priority;

        await Repository.UpdateTask(Task.Id);
    }

    async Task OnTaskKindChanged(TaskKind taskKind)
    {
        if (taskKind != TaskKind.RepeatingTask)
        {
            _showTime = false;
            Task.DesiredInterval = TimeSpan.Zero;
        }
        else if (Task.TaskKind != TaskKind.RepeatingTask)
        {
            Task.DesiredInterval = new(864000000000);
        }

        Task.TaskKind = taskKind;

        await Repository.UpdateTask(Task.Id);
    }

    async Task ClearDesiredInterval()
    {
        Task.TaskKind = TaskKind.Task;
        Task.DesiredInterval = TimeSpan.Zero;

        await Repository.UpdateTask(Task.Id);
    }

    async Task OnDone()
    {
        if (!Task.IsCompletedTask)
        {
            (DateTime time, long taskId) = Task.AddTime(DateTime.Now);

            await Repository.AddTime(time, taskId);
            await Repository.UpdateTask(Task.Id);
        }
    }

    void DateChanged(DateTime dateTime)
    {
        _taskTime = dateTime.Date + _taskTime.TimeOfDay;
    }

    void TimeChanged(TimeSpan timeSpan)
    {
        _taskTime = _taskTime.Date + timeSpan;
    }

    async Task SaveTime(DateTime time)
    {
        if (time != _taskTime)
        {
            Task.UpdateTime(time, _taskTime);

            await Repository.UpdateTime(time.Ticks, _taskTime, Task.Id);

            await Repository.UpdateTask(Task.Id);
        }

        CancelEditTime();
    }

    void CancelEditTime()
    {
        _selectedTime = 0;
        _editTime = false;
    }

    void EditTime(DateTime time)
    {
        _selectedTime = time.Ticks;
        _taskTime = time;
        _editTime = true;
    }

    async Task DeleteTime(DateTime time)
    {
        Task.RemoveTime(time);

        await Repository.DeleteTime(time.Ticks);

        await Repository.UpdateTask(Task.Id);
    }

    async Task SetDesiredIntervalDays(int? days)
    {
        Task.DesiredInterval = new TimeSpan(days ?? 0, Task.DesiredInterval.Hours, Task.DesiredInterval.Minutes, Task.DesiredInterval.Seconds);

        await Repository.UpdateTask(Task.Id);
    }

    async Task SetDesiredIntervalHours(int? hours)
    {
        Task.DesiredInterval = new TimeSpan(Task.DesiredInterval.Days, hours ?? 0, Task.DesiredInterval.Minutes, Task.DesiredInterval.Seconds);

        await Repository.UpdateTask(Task.Id);
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
