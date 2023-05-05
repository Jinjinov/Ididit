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

    [Inject]
    JsInterop JsInterop { get; set; } = null!;

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

    private string AdvancedInputText { get => Filters.AdvancedInputText; set => Filters.AdvancedInputText = value; }

    bool IsMoveSelectedTextDisabled => string.IsNullOrEmpty(_selectedAdvancedEditText) || (EditNameGoal is null && EditDetailsGoal is null);

    MemoEdit? _advancedEdit;

    Selection _advancedEditTextSelection = new();

    string _selectedAdvancedEditText = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_advancedEdit is not null)
            await JsInterop.HandleTabKey(_advancedEdit.ElementRef);
    }

    async Task OnDoubleClick()
    {
        if (_advancedEdit is null)
            return;

        Selection selection = await JsInterop.GetSelectionStartEnd(_advancedEdit.ElementRef);

        int end = selection.End;

        while (end > selection.Start && AdvancedInputText[end - 1] == ' ')
        {
            end--;
        }

        if (end != selection.End)
            await JsInterop.SetSelectionStartEnd(_advancedEdit.ElementRef, selection.Start, end);
    }

    public void OnTextChanged(string text)
    {
        AdvancedInputText = text;
    }

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

        if (!Settings.FilterBySelectedText)
        {
            Filters.SearchFilter = string.Empty;
            await FiltersChanged.InvokeAsync(Filters);
        }
    }

    async Task MoveSelectedTextToSelectedGoal()
    {
        if (EditNameGoal is not null)
        {
            string selectedText = string.IsNullOrEmpty(EditNameGoal.Details) ? _selectedAdvancedEditText : Environment.NewLine + _selectedAdvancedEditText;

            if (EditNameGoal.DetailsSelection is not null)
                EditNameGoal.Details = EditNameGoal.Details.Insert(EditNameGoal.DetailsSelection.End, selectedText);
            else
                EditNameGoal.Details += selectedText;

            await OnTextChanged(EditNameGoal);
            await EditNameGoalChanged.InvokeAsync(EditNameGoal);
        }
        else if (EditDetailsGoal is not null)
        {
            string selectedText = string.IsNullOrEmpty(EditDetailsGoal.Details) ? _selectedAdvancedEditText : Environment.NewLine + _selectedAdvancedEditText;

            if (EditDetailsGoal.DetailsSelection is not null)
                EditDetailsGoal.Details = EditDetailsGoal.Details.Insert(EditDetailsGoal.DetailsSelection.End, selectedText);
            else
                EditDetailsGoal.Details += selectedText;

            await OnTextChanged(EditDetailsGoal);
            await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
        }

        AdvancedInputText = AdvancedInputText.Remove(_advancedEditTextSelection.Start, _advancedEditTextSelection.End - _advancedEditTextSelection.Start);

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

        if (Settings.FilterBySelectedText && _selectedAdvancedEditText != "\n")
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

        if (Settings.Screen != Screen.Main)
            return;

        _advancedEditTextSelection = await JsInterop.GetSelectionStartEnd(_advancedEdit.ElementRef);
        string selectionString = AdvancedInputText[_advancedEditTextSelection.Start.._advancedEditTextSelection.End];

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

        if (selection.Start == selection.End && AdvancedInputText.Length > 0)
        {
            int index = Math.Min(selection.Start, AdvancedInputText.Length - 1);

            int afterEnd = AdvancedInputText.IndexOf('\n', index);

            if (afterEnd == 0)
                return;

            if (afterEnd == index)
                index -= 1;

            int beforeStart = AdvancedInputText.LastIndexOf('\n', index);

            if (afterEnd == -1)
                afterEnd = AdvancedInputText.Length;

            await JsInterop.SetSelectionStartEnd(_advancedEdit.ElementRef, beforeStart + 1, afterEnd);
        }
    }
}
