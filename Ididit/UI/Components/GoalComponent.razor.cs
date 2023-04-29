using Blazorise;
using Blazorise.Localization;
using HtmlAgilityPack;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    [Inject]
    JsInterop JsInterop { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    [EditorRequired]
    public GoalModel Goal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> GoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditDetailsGoal { get; set; }

    [Parameter]
    public EventCallback<GoalModel?> EditDetailsGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditNameGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditNameGoalChanged { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public SettingsModel Settings { get; set; } = null!;

    bool EditEnabled => EditDetailsGoal == Goal || EditNameGoal == Goal;

    IFluentBorderWithAll CardBorder => EditEnabled ? Border.Is1.RoundedZero : Border.Is0.RoundedZero;

    TaskModel? _selectedTask;

    TextEdit? _nameEdit;

    MemoEdit? _detailsEdit;

    string _goalName = string.Empty;

    bool _shouldFocus;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_detailsEdit is not null)
            await JsInterop.HandleTabKey(_detailsEdit.ElementRef);

        if (_shouldFocus && _nameEdit is not null)
        {
            _shouldFocus = false;

            await _nameEdit.Focus();
        }
    }

    public void UpdateDetailsHeight()
    {
        if (_detailsEdit is null)
            return;

        var e = _detailsEdit.ElementRef;

        // TODO: https://github.com/Megabit/Blazorise/issues/4555
        /*
        function calculateAutoHeight(e)
        {
            if (e && e.target)
            {
                e.target.style.height = 'auto';
                e.target.style.height = this.scrollHeight + 'px';
                e.target.style.overflowY = 'hidden';
            }
        }
        /**/
    }

    async Task OnEditNameFocusIn()
    {
        await SetEditGoal(Goal, null);
    }

    async Task OnEditNameFocusOut()
    {
        await UpdateGoalName();

        if (Settings.ShowAdvancedInput)
            return;

        EditNameGoal = null;
        await EditNameGoalChanged.InvokeAsync(EditNameGoal);
    }

    private async Task UpdateGoalName()
    {
        if (Goal.Name != _goalName)
        {
            Goal.Name = _goalName;
            await Repository.UpdateGoal(Goal.Id);

            await GoalChanged.InvokeAsync(Goal);
        }
    }

    async Task OnEditDetailsFocusIn()
    {
        await SetEditGoal(null, Goal);
    }

    async Task OnEditDetailsFocusOut()
    {
        if (Settings.ShowAdvancedInput)
            return;

        EditDetailsGoal = null;
        await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
    }

    async Task OnClick(MouseEventArgs args)
    {
        await SelectAndEditNameGoal();
    }

    async Task SelectAndEditNameGoal()
    {
        _goalName = Goal.Name;

        _shouldFocus = true;

        await SetEditGoal(Goal, null);
    }

    async Task KeyUp(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Code == "Escape")
        {
            await CancelEdit();
        }
        else if (eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter")
        {
            await SaveNameAndEndEdit();
        }
    }

    async Task CancelEdit()
    {
        _goalName = Goal.Name;

        await SetEditGoal(null, null);
    }

    private async Task SetEditGoal(GoalModel? nameGoal, GoalModel? detailsGoal)
    {
        EditNameGoal = nameGoal;
        await EditNameGoalChanged.InvokeAsync(EditNameGoal);

        EditDetailsGoal = detailsGoal;
        await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
    }

    async Task SaveNameAndEndEdit()
    {
        await SetEditGoal(null, null);

        await UpdateGoalName();
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

    async Task OnTextChanged(string text)
    {
        Goal.Details = text;
        await Repository.UpdateGoal(Goal.Id);

        if (!Goal.CreateTaskFromEachLine)
            return;

        await Repository.UpdateGoalTasks(Goal);
    }

    async Task ToggleCreateTaskFromEachLine()
    {
        Goal.CreateTaskFromEachLine = !Goal.CreateTaskFromEachLine;

        if (!Goal.CreateTaskFromEachLine)
        {
            Goal.UpdateDetailsMarkdownHtml();
        }

        if (!string.IsNullOrEmpty(Goal.Details) && Goal.CreateTaskFromEachLine)
        {
            await Repository.UpdateGoalTasks(Goal);
        }

        await Repository.UpdateGoal(Goal.Id);

        await GoalChanged.InvokeAsync(Goal);
    }

    string MarkCurrentLine(string text)
    {
        if (Goal.DetailsSelection is null)
            return text;

        if (text.Length < Goal.DetailsSelection.End)
            return text;

        return text.Insert(Goal.DetailsSelection.End, "</mark>").Insert(Goal.DetailsSelection.Start, "<mark class='hwt-mark'>");
    }

    string MarkSearchResults(string text)
    {
        if (Filters.IgnoreSearchCase)
            return MarkSearchResults(text, "<mark class='hwt-mark'>", "</mark>");
        else
            return text.Replace(Filters.SearchFilter, $"<mark class='hwt-mark'>{Filters.SearchFilter}</mark>");
    }

    string MarkSearchResults(string input, string before, string after)
    {
        // Create a pattern to match the search term with case-insensitivity
        string pattern = "(?i)" + Regex.Escape(Filters.SearchFilter);

        // Replace the search term with the marked version
        return Regex.Replace(input, pattern, match =>
        {
            // Get the matched search term with its original case
            string matchedTerm = match.Value;

            // Add mark tag to the start and end of the matched term
            return $"{before}{matchedTerm}{after}";
        });
    }

    string MarkSearchResultsInHtml(string text)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(text);

        HtmlNodeCollection coll = htmlDoc.DocumentNode.SelectNodes("//text()");

        if (Filters.IgnoreSearchCase)
        {
            foreach (HtmlTextNode node in coll.Cast<HtmlTextNode>())
            {
                node.Text = MarkSearchResults(node.Text, "<mark>", "</mark>");
            }
        }
        else
        {
            foreach (HtmlTextNode node in coll.Cast<HtmlTextNode>())
            {
                node.Text = node.Text.Replace(Filters.SearchFilter, $"<mark>{Filters.SearchFilter}</mark>");
            }
        }

        return htmlDoc.DocumentNode.OuterHtml;
    }

    async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (Settings.SelectLineWithCaret)
            await SelectCurrentLine();
        else if (Goal.DetailsSelection is not null)
            Goal.DetailsSelection = null;
    }

    async Task OnMouseUp(MouseEventArgs e)
    {
        if (Settings.SelectLineWithCaret)
            await SelectCurrentLine();
        else if (Goal.DetailsSelection is not null)
            Goal.DetailsSelection = null;
    }

    private async Task SelectCurrentLine()
    {
        if (_detailsEdit is null)
            return;

        Selection selection = await JsInterop.GetSelectionStartEnd(_detailsEdit.ElementRef);

        if (selection.Start == selection.End && Goal.Details.Length > 0)
        {
            int index = Math.Min(selection.Start, Goal.Details.Length - 1);

            int afterEnd = Goal.Details.IndexOf('\n', index);

            if (afterEnd == 0)
                return;

            if (afterEnd == index)
                index -= 1;

            int beforeStart = Goal.Details.LastIndexOf('\n', index);

            if (afterEnd == -1)
                afterEnd = Goal.Details.Length;

            if (Goal.DetailsSelection is null)
                Goal.DetailsSelection = new();

            Goal.DetailsSelection.Start = beforeStart + 1;
            Goal.DetailsSelection.End = afterEnd;
        }
    }
}
