using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
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

    TaskModel? _selectedTask;

    async Task SelectGoal()
    {
        SelectedGoal = Goal;

        await SelectedGoalChanged.InvokeAsync(SelectedGoal);
    }

    async Task OnTextChanged(string text)
    {
        if (Goal.Details.Count(c => c.Equals('\n')) < text.Count(c => c.Equals('\n')))
        {
            string[] lines = Goal.Details.Split('\n');

            TaskModel task = Goal.CreateTask();

            task.Name = lines[^1];

            await _repository.AddTask(task);
        }
        else if (Goal.Details.Count(c => c.Equals('\n')) > text.Count(c => c.Equals('\n')) && Goal.TaskList.Any())
        {
            TaskModel task = Goal.TaskList.Last();

            Goal.TaskList.Remove(task);

            await _repository.DeleteTask(task.Id);
        }

        // TODO: edit Category inside the selected Category
        // TODO: edit Goal inside the selected Goal

        // TODO: user friendly "select Goal" - easy to select and edit, but not when selecting one Task
        // TODO: user friendly "edit" "discard" "save"

        // TODO: don't add Category / Goal until name is set / Save button clicked

        // TODO: use Breadcrumb to show Category/Subcategory in Goal header

        // TODO: use Blazorise icons

        // TODO: use only Blazorise, no css?

        // TODO: use Drag & Drop to move Subcategory into another Category
        // TODO: use Drag & Drop to move Goal into another Category
        // TODO: use Drag & Drop to sort Goals
        // TODO: use Drag & Drop to sort Tasks

        // https://blazorise.com/docs/components/repeater
        // The repeater component is a helper component that repeats the child content for each element in a collection.
        // One advantage over using traditional @foreach loop is that repeater have a full support for INotifyCollectionChanged.
        // Meaning you can do custom actions whenever a data-source changes.

        // TODO: task - times list should load on demand - on Task done - on show Task details

        // TODO: move backup from MainLayout to a component

        // TODO: import - # headers
        // TODO: import - ### headers
        // TODO: import - empty lines
        // TODO: import - notes = lines without "-"
        // TODO: import - tasks = lines with "-"
        // TODO: import - nested tasks = lines with "    -"

        // using Markdig;
        // readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();
        // public string MarkdownToHtml(string markdown) => Markdown.ToHtml(markdown, markdownPipeline);
        // @((MarkupString)Goal.NotesMarkdownHtml)

        // TODO: settings - https://bootstrapdemo.blazorise.com/tests/misc-forms
        // TODO: themes
        // TODO: loading intro - https://bootstrapdemo.blazorise.com/tests/spinkit

        // TODO: drag & drop - reorder
        // TODO: drag & drop - move / change parent

        Goal.Details = text;

        await _repository.UpdateGoal(Goal.Id);
    }
}
