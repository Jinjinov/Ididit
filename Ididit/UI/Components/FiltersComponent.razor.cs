using Blazorise.Localization;
using Ididit.Data;
using Ididit.Data.Model.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class FiltersComponent
{
    [Inject]
    ITextLocalizer<Translations> Localizer { get; set; } = null!;

    public static bool IsApple => OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst();

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

    string _searchFilter = string.Empty;

    async Task SearchFilterChanged(string searchFilter)
    {
        _searchFilter = searchFilter;
        Filters.SearchFilter = searchFilter;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearSearchFilter()
    {
        _searchFilter = string.Empty;
        Filters.SearchFilter = string.Empty;
        await FiltersChanged.InvokeAsync(Filters);
    }

    DateTime? _dateFilter;

    bool? IsTodayChecked => _dateFilter == DateTime.Now.Date;

    async Task TodayCheckedChanged(bool? isToday)
    {
        _dateFilter = isToday == true ? DateTime.Now.Date : null;
        Filters.DateFilter = isToday == true ? DateTime.Now.Date : null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task OnDateChanged(DateTime? dateTime)
    {
        _dateFilter = dateTime;
        Filters.DateFilter = dateTime;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearDateFilter()
    {
        _dateFilter = null;
        Filters.DateFilter = null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    bool GetShowPriority(Priority priority) => Settings.ShowPriority[priority];

    async Task OnShowPriorityChanged(Priority priority, bool show)
    {
        Settings.ShowPriority[priority] = show;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    bool GetShowTaskKind(TaskKind taskKind) => Settings.ShowTaskKind[taskKind];

    async Task OnShowTaskKindChanged(TaskKind taskKind, bool show)
    {
        Settings.ShowTaskKind[taskKind] = show;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnSortChangeEvent(ChangeEventArgs e)
    {
        if (e.Value is string value)
            await OnSortChanged(Enum.Parse<Sort>(value));
    }

    async Task OnSortChanged(Sort sort)
    {
        Settings.Sort = sort;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnHideCompletedTasksChanged(bool? val)
    {
        Settings.HideCompletedTasks = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnShowElapsedToDesiredRatioOverMinChanged(bool? val)
    {
        Settings.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }

    async Task OnElapsedToDesiredRatioMinChanged(long val)
    {
        Settings.ElapsedToDesiredRatioMin = val;
        await Repository.UpdateSettings(Settings.Id);

        await SettingsChanged.InvokeAsync(Settings);
    }
}
