using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public GoalModel Goal { get; set; } = null!;

    [Parameter]
    public GoalModel? SelectedGoal { get; set; }

    [Parameter]
    public EventCallback<GoalModel?> SelectedGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditGoalChanged { get; set; }

    [Parameter]
    public string SearchFilter { get; set; } = string.Empty;

    [Parameter]
    public DateTime? DateFilter { get; set; }

    [Parameter]
    public Priority? PriorityFilter { get; set; }

    TaskModel? _selectedTask;

    Blazorise.MemoEdit? _memoEdit;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (SelectedGoal == Goal && _memoEdit != null)
        {
            await _memoEdit.Focus();
        }
    }

    async Task SelectGoal()
    {
        if (SelectedGoal != Goal)
            SelectedGoal = Goal;
        else
            SelectedGoal = null;

        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    IEnumerable<TaskModel> GetTasks()
    {
        IEnumerable<TaskModel> tasks = Goal.TaskList.Where(task =>
        {
            bool isRatioOk = task.ElapsedToDesiredRatio >= _repository.Settings.ElapsedToDesiredRatioMin;

            bool isNameOk = string.IsNullOrEmpty(SearchFilter) || task.Name.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase);

            bool isDateOk = DateFilter == null || task.TimeList.Any(time => time.Date == DateFilter?.Date);

            bool isPriorityOk = PriorityFilter == null || task.Priority == PriorityFilter;

            return isNameOk && isDateOk && isPriorityOk && 
                (isRatioOk || !_repository.Settings.ShowElapsedToDesiredRatioOverMin) &&
                (task.IsRepeating || !_repository.Settings.ShowOnlyRepeating) &&
                (!task.IsRepeating || !_repository.Settings.ShowOnlyAsap) &&
                (!task.IsCompleted || _repository.Settings.AlsoShowCompletedAsap);
        });

        return GetSorted(tasks);
    }

    IEnumerable<TaskModel> GetSorted(IEnumerable<TaskModel> tasks)
    {
        return _repository.Settings.Sort switch
        {
            Sort.None => tasks,
            Sort.Name => tasks.OrderBy(task => task.Name),
            Sort.Priority => tasks.OrderByDescending(task => task.Priority),
            Sort.ElapsedTime => tasks.OrderByDescending(task => task.ElapsedTime),
            Sort.ElapsedToAverageRatio => tasks.OrderByDescending(task => task.ElapsedToAverageRatio),
            Sort.ElapsedToDesiredRatio => tasks.OrderByDescending(task => task.ElapsedToDesiredRatio),
            Sort.AverageToDesiredRatio => tasks.OrderByDescending(task => task.AverageToDesiredRatio),
            _ => throw new ArgumentException("Invalid argument: " + nameof(_repository.Settings.Sort))
        };
    }

    class DoneLine
    {
        public string Line = null!;
        public bool IsDone;
    }

    class DoneTask
    {
        public TaskModel Task = null!;
        public bool IsDone;
    }

    async Task OnTextChanged(string text)
    {
        List<DoneTask> oldLines = Goal.TaskList.Select(task => new DoneTask { Task = task }).ToList();
        List<DoneLine> newLines = text.Split('\n').Select(line => new DoneLine { Line = line }).ToList();

        Goal.Details = text;
        await _repository.UpdateGoal(Goal.Id);

        // reordering will be done with drag & drop, don't check the order of tasks here

        while (newLines.Any(p => !p.IsDone) || oldLines.Any(p => !p.IsDone))
        {
            while (newLines.Any(p => !p.IsDone) && oldLines.Any(p => !p.IsDone))
            {
                var newLine = newLines.First(p => !p.IsDone);
                var oldLine = oldLines.First(p => !p.IsDone);

                if (newLine.Line == oldLine.Task.Name)
                {
                    newLine.IsDone = true;
                    oldLine.IsDone = true;
                }
                else
                {
                    break;
                }
            }

            int newLinesCount = newLines.Count(p => !p.IsDone);
            int oldLinesCount = oldLines.Count(p => !p.IsDone);

            if (oldLinesCount == newLinesCount && newLinesCount > 0) // changed
            {
                var newLine = newLines.First(p => !p.IsDone);
                var oldLine = oldLines.First(p => !p.IsDone);

                await UpdateTask(oldLine.Task, newLine.Line);

                newLine.IsDone = true;
                oldLine.IsDone = true;
            }
            else if (oldLinesCount < newLinesCount) // added
            {
                var newLine = newLines.First(p => !p.IsDone);
                int idx = oldLines.FindLastIndex(p => p.IsDone) + 1;

                await AddTaskAt(idx, newLine.Line);

                newLine.IsDone = true;
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                var oldLine = oldLines.First(p => !p.IsDone);

                await DeleteTask(oldLine.Task);

                oldLine.IsDone = true;
            }
        }
    }

    // TODO: settings with small and large UI: https://bootstrapdemo.blazorise.com/tests/misc-forms

    // TODO: UI - add options page: size & theme

    // TODO: UI - search filter

    // TODO: UI - date filter
    // TODO: UI - today filter

    // TODO: UI - show only selected priority

    // TODO: UI - show only repeating tasks
    // TODO: UI - show only ASAP tasks
    // TODO: UI - also show completed ASAP tasks

    // TODO: UI - show only ratio over % - checkbox
    // TODO: UI - show only ratio over % - slider

    // TODO: UI - sort combo box

    // TODO: UI - move backup from footer to options
    // TODO: UI - move Login from header to options

    // TODO: GoogleDriveBackup

    private async Task UpdateTask(TaskModel task, string line)
    {
        task.Name = line;
        await _repository.UpdateTask(task.Id);
    }

    private async Task AddTaskAt(int idx, string line)
    {
        (TaskModel task, TaskModel? changedTask) = Goal.CreateTaskAt(_repository.MaxTaskId + 1, idx);
        task.Name = line;

        if (changedTask is not null)
            await _repository.UpdateTask(changedTask.Id);

        await _repository.AddTask(task);
    }

    private async Task DeleteTask(TaskModel task)
    {
        TaskModel? changedTask = Goal.RemoveTask(task);

        if (changedTask is not null)
            await _repository.UpdateTask(changedTask.Id);

        await _repository.DeleteTask(task.Id);
    }

    // TODO: mobile: single column, minimized tree view
    // TODO: mobile: use Breadcrumb to show Category/Subcategory in Goal header
    // TODO: mobile: show sub-categories in Goal list

    // TODO: task - "times list" should load on demand - on Task done - on show Task details

    //
    // HIGH PRIORITY:
    //

    // TODO: new settings
    // TODO: load settings
    // TODO: help + GitHub link
    // TODO: load examples
    // TODO: delete all

    // TODO: every line that StartsWith("- ") is a task detail

    // TODO: task obstacle: weak point -> Habit / Task -> reason for not doing it -> solution
    // TODO: (class Solution) --> (class Task.Details) - when, where
    // name, address, phone number, working hours, website, email
    // possible to do the task:
    // - anytime
    // - free time
    // - during work week open hours
    // - during weekend
    // - when opportunity arises

    // TODO: remove focused borders

    // TODO: "save" (exit edit) on Enter key

    //
    // MEDIUM PRIORITY:
    //

    // TODO: user friendly "edit" "save" - remove Edit name buttons, remove Toggle buttons (click on Goal to toggle edit mode), start edit on click (except on URL link click)

    // TODO: don't add Category/Goal until (name is set) / (Save button is clicked) - no need to undo adding empty objects = easy discard

    // TODO: desired task duration - set (i want to exercise 15 min) / countdown timer + alarm
    // TODO: average task duration - start / stop timer (how long does it take to clean the floor)

    // TODO: see all - or collapse to titles

    // TODO: use Drag & Drop to move Subcategory into another Category
    // TODO: use Drag & Drop to move Goal into another Category
    // TODO: use Drag & Drop to reorder Goals
    // TODO: use Drag & Drop to reorder Tasks

    // TODO: arrow down can change focus to next textarea

    // TODO: backup - Dropbox / OneDrive / iCloud

    //
    // LOW PRIORITY:
    //

    // TODO: task priority (must, should, can) - importance / urgency - (scale 1-10) - (low / med / high)
    // TODO: sort by priority (must, should, can) - importance / urgency

    // TODO: weekly category goal (do X tasks from this category)
    // TODO: statistics (did X tasks from this category)
    // TODO: graphs (num of tasks over time)

    // TODO: show Times in a Calendar/Scheduler, not a List
}

// TODO: use blazor layouts?
// @inherits LayoutComponentBase
// @page "/users"
// @layout MainLayout
// @page "/admin"
// @layout AdminLayout

// TODO: use route navigation for help, options, settings?

// https://blazorise.com/docs/components/repeater
// The repeater component is a helper component that repeats the child content for each element in a collection.
// One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
// Meaning you can do custom actions whenever a data-source changes.