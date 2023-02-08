using Blazorise;
using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class AdvancedInputComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public SettingsModel Settings { get; set; } = null!;

    [Parameter]
    public EventCallback<SettingsModel> SettingsChanged { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public EventCallback<Filters> FiltersChanged { get; set; }

    [Parameter]
    public GoalModel? EditNameGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel> EditNameGoalChanged { get; set; }

    [Parameter]
    public GoalModel? EditDetailsGoal { get; set; } = null!;

    [Parameter]
    public EventCallback<GoalModel?> EditDetailsGoalChanged { get; set; }

    bool IsMoveSelectedTextDisabled => string.IsNullOrEmpty(_selectedAdvancedEditText) || (EditNameGoal is null && EditDetailsGoal is null);

    MemoEdit? _advancedEdit;

    string _advancedEditText = string.Empty;

    Selection _advancedEditTextSelection = new();

    string _selectedAdvancedEditText = string.Empty;

    [Inject]
    JsInterop JsInterop { get; set; } = null!;

    async Task OnSelectLineWithCaretChanged(bool val)
    {
        Settings.SelectLineWithCaret = val;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnFilterBySelectedTextChanged(bool val)
    {
        Settings.FilterBySelectedText = val;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task MoveSelectedTextToSelectedGoal()
    {
        if (EditNameGoal is not null)
        {
            EditNameGoal.Details += string.IsNullOrEmpty(EditNameGoal.Details) ? _selectedAdvancedEditText : Environment.NewLine + _selectedAdvancedEditText;
            await OnTextChanged(EditNameGoal);
            await EditNameGoalChanged.InvokeAsync(EditNameGoal);
        }
        else if (EditDetailsGoal is not null)
        {
            EditDetailsGoal.Details += string.IsNullOrEmpty(EditDetailsGoal.Details) ? _selectedAdvancedEditText : Environment.NewLine + _selectedAdvancedEditText;
            await OnTextChanged(EditDetailsGoal);
            await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
        }

        _advancedEditText = _advancedEditText.Remove(_advancedEditTextSelection.Start, _advancedEditTextSelection.End - _advancedEditTextSelection.Start);

        _advancedEditTextSelection.Start = 0;
        _advancedEditTextSelection.End = 0;
        await SetSelectedAdvancedEditText(string.Empty);

        async Task OnTextChanged(GoalModel goal)
        {
            await Repository.UpdateGoal(goal.Id);

            if (!goal.CreateTaskFromEachLine)
                return;

            await Repository.UpdateGoalTasks(goal);
        }
    }

    async Task SetSelectedAdvancedEditText(string text)
    {
        _selectedAdvancedEditText = text;

        if (Settings.FilterBySelectedText)
        {
            Filters.SearchFilter = _selectedAdvancedEditText;
            await FiltersChanged.InvokeAsync(Filters);
        }
    }

    async Task OnSelect(EventArgs e)
    {
        await GetSelectionString();
    }

    async Task GetSelectionString()
    {
        if (_advancedEdit is null)
            return;

        _advancedEditTextSelection = await JsInterop.GetSelectionStartEnd(_advancedEdit.ElementRef);
        string selectionString = _advancedEditText[_advancedEditTextSelection.Start.._advancedEditTextSelection.End];

        //string selectionString = await JsInterop.GetSelectionString(_advancedEdit.ElementRef);

        await SetSelectedAdvancedEditText(selectionString);
    }

    async Task OnFocusOut()
    {
        // TODO: https://github.com/Megabit/Blazorise/issues/4554
        // https://developer.mozilla.org/en-US/docs/Web/API/FocusEvent/relatedTarget
        //await SetSelectedAdvancedEditText(string.Empty);
    }

    async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (Settings.SelectLineWithCaret)
        {
            if (e.Code == "ArrowLeft" || e.Code == "ArrowUp" || e.Code == "ArrowRight" || e.Code == "ArrowDown")
                await SelectCurrentLine();
        }
        else if (e.ShiftKey || e.Key == "Shift")
        {
            await GetSelectionString();
        }
        else
        {
            await SetSelectedAdvancedEditText(string.Empty);
        }
    }

    async Task OnMouseUp(MouseEventArgs e)
    {
        if (Settings.SelectLineWithCaret)
            await SelectCurrentLine();
        else
            await SetSelectedAdvancedEditText(string.Empty);
    }

    private async Task SelectCurrentLine()
    {
        if (_advancedEdit is null)
            return;

        Selection selection = await JsInterop.GetSelectionStartEnd(_advancedEdit.ElementRef);

        if (selection.Start == selection.End && _advancedEditText.Length > 0)
        {
            int index = Math.Min(selection.Start, _advancedEditText.Length - 1);

            int afterEnd = _advancedEditText.IndexOf('\n', index);

            if (afterEnd == 0)
                return;

            if (afterEnd == index)
                index -= 1;

            int beforeStart = _advancedEditText.LastIndexOf('\n', index);

            if (afterEnd == -1)
                afterEnd = _advancedEditText.Length;

            await JsInterop.SetSelectionStartEnd(_advancedEdit.ElementRef, beforeStart + 1, afterEnd);
        }
    }
}
