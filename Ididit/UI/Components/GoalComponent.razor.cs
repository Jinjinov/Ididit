using Blazorise;
using Blazorise.Localization;
using HtmlAgilityPack;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    string MarkSearchResults(string text)
    {
        //return text.Replace(Filters.SearchFilter, $"<mark>{Filters.SearchFilter}</mark>");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(text);

        HtmlNodeCollection coll = htmlDoc.DocumentNode.SelectNodes("//text()");

        foreach (HtmlTextNode node in coll.Cast<HtmlTextNode>())
        {
            node.Text = node.Text.Replace(Filters.SearchFilter, $"<mark>{Filters.SearchFilter}</mark>");
        }

        return htmlDoc.DocumentNode.OuterHtml;
    }
}
