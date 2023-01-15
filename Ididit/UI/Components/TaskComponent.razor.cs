using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public sealed partial class TaskComponent : IDisposable
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

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

    bool _taskStarted;
    DateTime _taskStartedTime;

    bool _showTime;
    bool _editTime;
    long _selectedTime;
    DateTime _taskTime;

    readonly System.Timers.Timer _timer = new(1000);
    TimeSpan _elapsedTime;

    public TaskComponent()
    {
        _timer.Elapsed += Timer_Elapsed;
    }

    private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _elapsedTime = DateTime.Now - _taskStartedTime;

        if (_elapsedTime >= Task.DesiredDuration)
        {
            _timer.Stop();

            await OnDone();
        }

        await InvokeAsync(StateHasChanged);
    }

    void OnPlay()
    {
        _taskStarted = true;
        _taskStartedTime = DateTime.Now;

        if (Task.DesiredDuration is not null)
        {
            _timer.Start();
        }
    }

    async Task OnDone()
    {
        if (!Task.IsCompletedTask)
        {
            (DateTime time, long taskId) = Task.AddTime(DateTime.Now);

            if (_taskStarted)
            {
                _taskStarted = false;

                if (_timer.Enabled)
                {
                    _timer.Stop();
                }

                TimeSpan taskDuration = time - _taskStartedTime;

                TimeSpan oldAverage = Task.AverageDuration ?? TimeSpan.Zero;

                TimeSpan newAverage = oldAverage * Task.DurationTimedCount + taskDuration;

                Task.DurationTimedCount++;

                Task.AverageDuration = newAverage / Task.DurationTimedCount;
            }

            await Repository.AddTime(time, taskId);
            await Repository.UpdateTask(Task.Id);
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    async Task ToggleTask(MouseEventArgs args)
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

    async Task PriorityChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await PriorityChanged(Enum.Parse<Priority>(value));
    }

    async Task PriorityChanged(Priority priority)
    {
        Task.Priority = priority;

        await Repository.UpdateTask(Task.Id);
    }

    async Task OnTaskKindChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnTaskKindChanged(Enum.Parse<TaskKind>(value));
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
