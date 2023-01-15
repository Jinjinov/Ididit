using Blazorise;
using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    [EditorRequired]
    public GoalModel Goal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> GoalChanged { get; set; }

    [Parameter]
    public GoalModel? SelectedGoal { get; set; }

    [Parameter]
    public EventCallback<GoalModel?> SelectedGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditGoalChanged { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public SettingsModel Settings { get; set; } = null!;

    bool EditEnabled => SelectedGoal == Goal || EditGoal == Goal;

    IFluentBorderWithAll CardBorder => EditEnabled ? Border.Is1.RoundedZero : Border.Is0.RoundedZero;

    TaskModel? _selectedTask;

    Blazorise.TextEdit? _textEdit;

    string _goalName = string.Empty;

    bool _shouldFocus;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocus && _textEdit is not null)
        {
            _shouldFocus = false;

            await _textEdit.Focus();
        }
    }

    async Task FocusIn()
    {
        EditGoal = Goal;
        await EditGoalChanged.InvokeAsync(EditGoal);
    }

    async Task FocusOut()
    {
        EditGoal = null;
        await EditGoalChanged.InvokeAsync(EditGoal);

        if (Goal.Name != _goalName)
        {
            Goal.Name = _goalName;
            await Repository.UpdateGoal(Goal.Id);

            await GoalChanged.InvokeAsync(Goal);
        }
    }

    async Task OnFocusIn()
    {
        SelectedGoal = Goal;
        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task OnFocusOut()
    {
        SelectedGoal = null;
        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task OnClick(MouseEventArgs args)
    {
        await SelectAndEditGoal();
    }

    async Task SelectAndEditGoal()
    {
        _goalName = Goal.Name;

        _shouldFocus = true;

        EditGoal = Goal;
        await EditGoalChanged.InvokeAsync(EditGoal);

        //SelectedGoal = Goal;
        //await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task KeyUp(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Code == "Escape")
        {
            await CancelEdit();
        }
        else if (eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter")
        {
            await SaveName();
        }
    }

    async Task CancelEdit()
    {
        _goalName = Goal.Name;

        EditGoal = null;
        await EditGoalChanged.InvokeAsync(EditGoal);

        SelectedGoal = null;
        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task SaveName()
    {
        EditGoal = null;
        await EditGoalChanged.InvokeAsync(EditGoal);

        SelectedGoal = null;
        await SelectedGoalChanged.InvokeAsync(SelectedGoal);

        if (Goal.Name != _goalName)
        {
            Goal.Name = _goalName;
            await Repository.UpdateGoal(Goal.Id);

            await GoalChanged.InvokeAsync(Goal);
        }
    }

    async Task DeleteGoal()
    {
        if (Repository.AllCategories.TryGetValue(Goal.CategoryId, out CategoryModel? parent))
        {
            GoalModel? changedGoal = parent.RemoveGoal(Goal);

            if (changedGoal is not null)
                await Repository.UpdateGoal(changedGoal.Id);
        }

        await Repository.DeleteGoal(Goal.Id);

        Goal = null; // @if (Goal is not null) in GoalComponent.razor is still called after Delete Goal
        await GoalChanged.InvokeAsync(Goal);
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
        Goal.Details = text;
        await Repository.UpdateGoal(Goal.Id);

        if (!Goal.CreateTaskFromEachLine)
            return;

        await UpdateTasks();
    }

    async Task ToggleCreateTaskFromEachLine()
    {
        Goal.CreateTaskFromEachLine = !Goal.CreateTaskFromEachLine;

        if (!string.IsNullOrEmpty(Goal.Details) && !Goal.TaskList.Any())
        {
            await UpdateTasks();
        }

        await Repository.UpdateGoal(Goal.Id);

        await GoalChanged.InvokeAsync(Goal);
    }

    private async Task UpdateTasks()
    {
        List<DoneTask> oldLines = Goal.TaskList.Select(task => new DoneTask { Task = task }).ToList();
        List<DoneLine> newLines = Goal.Details.Split('\n').Select(line => new DoneLine { Line = line }).ToList();

        // reordering will be done with drag & drop, don't check the order of tasks here

        while (newLines.Any(p => !p.IsDone) || oldLines.Any(p => !p.IsDone))
        {
            while (newLines.Any(p => !p.IsDone) && oldLines.Any(p => !p.IsDone))
            {
                DoneLine newLine = newLines.First(p => !p.IsDone);
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

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
                DoneLine newLine = newLines.First(p => !p.IsDone);
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

                await UpdateTask(oldLine.Task, newLine.Line);

                newLine.IsDone = true;
                oldLine.IsDone = true;
            }
            else if (oldLinesCount < newLinesCount) // added
            {
                DoneLine newLine = newLines.First(p => !p.IsDone);
                int idx = oldLines.FindLastIndex(p => p.IsDone) + 1;

                TaskModel task = await AddTaskAt(idx, newLine.Line);

                newLine.IsDone = true;

                oldLines.Insert(idx, new DoneTask() { Task = task, IsDone = true });
            }
            else if (oldLinesCount > newLinesCount) // deleted
            {
                DoneTask oldLine = oldLines.First(p => !p.IsDone);

                await DeleteTask(oldLine.Task);

                oldLine.IsDone = true;
            }
        }
    }

    private async Task UpdateTask(TaskModel task, string line)
    {
        task.Name = line;
        await Repository.UpdateTask(task.Id);
    }

    private async Task<TaskModel> AddTaskAt(int idx, string line)
    {
        (TaskModel task, TaskModel? changedTask) = Goal.CreateTaskAt(Repository.NextTaskId, idx);
        task.Name = line;

        if (changedTask is not null)
            await Repository.UpdateTask(changedTask.Id);

        await Repository.AddTask(task);

        return task;
    }

    private async Task DeleteTask(TaskModel task)
    {
        TaskModel? changedTask = Goal.RemoveTask(task);

        if (changedTask is not null)
            await Repository.UpdateTask(changedTask.Id);

        await Repository.DeleteTask(task.Id);
    }
}
