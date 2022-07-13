using Ididit.App;
using Ididit.Data.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class GoalsComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public EventCallback<CategoryModel> SelectedCategoryChanged { get; set; }

    public GoalModel? _selectedGoal { get; set; }

    public GoalModel? _editGoal { get; set; }

    public string SearchFilter { get; set; } = string.Empty;

    public DateTime? DateFilter { get; set; }

    public Priority? PriorityFilter { get; set; }

    bool? IsTodayChecked => DateFilter == DateTime.Now.Date;

    void TodayCheckedChanged(bool? isToday) => DateFilter = isToday == true ? DateTime.Now.Date : null;

    void ClearSearchFilter() => SearchFilter = string.Empty;

    void OnDateChanged(DateTime? dateTime) => DateFilter = dateTime;

    void ClearDateFilter() => DateFilter = null;

    void OnPriorityChanged(Priority? priority) => PriorityFilter = priority;

    Sort Sort => _repository.Settings.Sort;

    long ElapsedToDesiredRatioMin => _repository.Settings.ElapsedToDesiredRatioMin;

    bool ShowElapsedToDesiredRatioOverMin => _repository.Settings.ShowElapsedToDesiredRatioOverMin;

    bool ShowOnlyRepeating => _repository.Settings.ShowOnlyRepeating;

    bool ShowOnlyAsap => _repository.Settings.ShowOnlyAsap;

    bool AlsoShowCompletedAsap => _repository.Settings.AlsoShowCompletedAsap;

    async Task OnSortChanged(Sort sort)
    {
        _repository.Settings.Sort = sort;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowOnlyRepeatingChanged(bool? val)
    {
        _repository.Settings.ShowOnlyRepeating = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowOnlyAsapChanged(bool? val)
    {
        _repository.Settings.ShowOnlyAsap = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnAlsoShowCompletedAsapChanged(bool? val)
    {
        _repository.Settings.AlsoShowCompletedAsap = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowElapsedToDesiredRatioOverMinChanged(bool? val)
    {
        _repository.Settings.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnElapsedToDesiredRatioMinChanged(long val)
    {
        _repository.Settings.ElapsedToDesiredRatioMin = val;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnSelectedCategoryChanged(CategoryModel category)
    {
        SelectedCategory = category;
        await SelectedCategoryChanged.InvokeAsync(SelectedCategory);
    }

    async Task NewGoal()
    {
        if (SelectedCategory != null)
        {
            GoalModel goal = SelectedCategory.CreateGoal(_repository.MaxGoalId + 1);

            await _repository.AddGoal(goal);

            _selectedGoal = goal;
            _editGoal = goal;
        }
    }
}
