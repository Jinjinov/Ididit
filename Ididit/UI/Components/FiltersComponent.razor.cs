using Ididit.App;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class FiltersComponent
{
    [Inject]
    IRepository _repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public EventCallback<Filters> FiltersChanged { get; set; }

    string SearchFilter = string.Empty;

    async Task SearchFilterChanged(string searchFilter)
    {
        SearchFilter = searchFilter;
        Filters.SearchFilter = searchFilter;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearSearchFilter()
    {
        SearchFilter = string.Empty;
        Filters.SearchFilter = string.Empty;
        await FiltersChanged.InvokeAsync(Filters);
    }

    DateTime? DateFilter;

    bool? IsTodayChecked => DateFilter == DateTime.Now.Date;

    async Task TodayCheckedChanged(bool? isToday)
    {
        DateFilter = isToday == true ? DateTime.Now.Date : null;
        Filters.DateFilter = isToday == true ? DateTime.Now.Date : null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task OnDateChanged(DateTime? dateTime)
    {
        DateFilter = dateTime;
        Filters.DateFilter = dateTime;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearDateFilter()
    {
        DateFilter = null;
        Filters.DateFilter = null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    Priority? PriorityFilter;

    async Task OnPriorityChanged(Priority? priority)
    {
        PriorityFilter = priority;
        Filters.PriorityFilter = priority;
        await FiltersChanged.InvokeAsync(Filters);
    }

    Sort Sort
    {
        get
        {
            Filters.Sort = _repository.Settings.Sort;
            return _repository.Settings.Sort;
        }
    }

    long ElapsedToDesiredRatioMin
    {
        get
        {
            Filters.ElapsedToDesiredRatioMin = _repository.Settings.ElapsedToDesiredRatioMin;
            return _repository.Settings.ElapsedToDesiredRatioMin;
        }
    }

    bool ShowElapsedToDesiredRatioOverMin
    {
        get
        {
            Filters.ShowElapsedToDesiredRatioOverMin = _repository.Settings.ShowElapsedToDesiredRatioOverMin;
            return _repository.Settings.ShowElapsedToDesiredRatioOverMin;
        }
    }

    bool ShowOnlyRepeating
    {
        get
        {
            Filters.ShowOnlyRepeating = _repository.Settings.ShowOnlyRepeating;
            return _repository.Settings.ShowOnlyRepeating;
        }
    }

    bool ShowOnlyAsap
    {
        get
        {
            Filters.ShowOnlyAsap = _repository.Settings.ShowOnlyAsap;
            return _repository.Settings.ShowOnlyAsap;
        }
    }

    bool AlsoShowCompletedAsap
    {
        get
        {
            Filters.AlsoShowCompletedAsap = _repository.Settings.AlsoShowCompletedAsap;
            return _repository.Settings.AlsoShowCompletedAsap;
        }
    }

    async Task OnSortChanged(Sort sort)
    {
        Filters.Sort = sort;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.Sort = sort;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowOnlyRepeatingChanged(bool? val)
    {
        Filters.ShowOnlyRepeating = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.ShowOnlyRepeating = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowOnlyAsapChanged(bool? val)
    {
        Filters.ShowOnlyAsap = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.ShowOnlyAsap = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnAlsoShowCompletedAsapChanged(bool? val)
    {
        Filters.AlsoShowCompletedAsap = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.AlsoShowCompletedAsap = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnShowElapsedToDesiredRatioOverMinChanged(bool? val)
    {
        Filters.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }

    async Task OnElapsedToDesiredRatioMinChanged(long val)
    {
        Filters.ElapsedToDesiredRatioMin = val;
        await FiltersChanged.InvokeAsync(Filters);

        _repository.Settings.ElapsedToDesiredRatioMin = val;
        await _repository.UpdateSettings(_repository.Settings.Id);
    }
}
