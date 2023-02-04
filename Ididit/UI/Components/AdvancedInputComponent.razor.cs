using Blazorise;
using Blazorise.Localization;
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

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

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

    MemoEdit? _advancedEdit;

    string _advancedEditText = string.Empty;

    string _selectedAdvancedEditText = string.Empty;

    bool _selectLineWithCaret;

    bool _filterBySelectedText;

    [Inject]
    JsInterop JsInterop { get; set; } = null!;

    async Task MoveSelectedTextToSelectedGoal()
    {
        if (EditNameGoal is not null)
        {
            EditNameGoal.Details += _selectedAdvancedEditText;
            await EditNameGoalChanged.InvokeAsync(EditNameGoal);
        }
        else if (EditDetailsGoal is not null)
        {
            EditDetailsGoal.Details += _selectedAdvancedEditText;
            await EditDetailsGoalChanged.InvokeAsync(EditDetailsGoal);
        }
    }

    async Task SetSelectedAdvancedEditText(string text)
    {
        _selectedAdvancedEditText = text;

        if (_filterBySelectedText)
        {
            Filters.SearchFilter = _selectedAdvancedEditText;
            await FiltersChanged.InvokeAsync(Filters);
        }
    }

    async Task OnSelect(EventArgs e)
    {
        if (_advancedEdit is null)
            return;

        string selectionString = await JsInterop.GetSelectionString(_advancedEdit.ElementRef);
        await SetSelectedAdvancedEditText(selectionString);
    }

    async Task OnFocusOut()
    {
        await SetSelectedAdvancedEditText(string.Empty);
    }

    async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (_selectLineWithCaret)
        {
            if (e.Code == "ArrowLeft" || e.Code == "ArrowUp" || e.Code == "ArrowRight" || e.Code == "ArrowDown")
                await SelectCurrentLine();
        }
        else if (e.ShiftKey || e.Key == "Shift")
        {
            if (_advancedEdit is not null)
            {
                string selectionString = await JsInterop.GetSelectionString(_advancedEdit.ElementRef);
                await SetSelectedAdvancedEditText(selectionString);
            }
        }
        else
        {
            await SetSelectedAdvancedEditText(string.Empty);
        }
    }

    async Task OnMouseUp(MouseEventArgs e)
    {
        if (_selectLineWithCaret)
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
