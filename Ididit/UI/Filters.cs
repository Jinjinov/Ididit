using Ididit.Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ididit.UI;

public class Filters
{
    public string SearchFilter { get; set; } = string.Empty;

    public DateTime? DateFilter { get; set; }

    public IList<TaskModel> FilterAndSortTasks(IEnumerable<TaskModel> tasks, SettingsModel settings)
    {
        IEnumerable<TaskModel> filteredTasks = tasks.Where(task =>
        {
            bool isRatioOk = task.ElapsedToDesiredRatio >= settings.ElapsedToDesiredRatioMin;

            bool isNameOk = string.IsNullOrEmpty(SearchFilter) || task.Name.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase);

            bool isDateOk = DateFilter is null || task.TimeList.Any(time => time.Date == DateFilter?.Date);

            bool isPriorityOk = settings.ShowPriority[task.Priority];

            bool isTaskKindOk = settings.ShowTaskKind[task.TaskKind];

            return isNameOk && isDateOk && isPriorityOk && isTaskKindOk &&
                (isRatioOk || !settings.ShowElapsedToDesiredRatioOverMin) &&
                (!task.IsCompletedTask || !settings.HideCompletedTasks);
        });

        return SortTasks(filteredTasks, settings).ToList();
    }

    private IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks, SettingsModel settings)
    {
        return settings.Sort switch
        {
            Sort.None => tasks,
            Sort.Name => tasks.OrderBy(task => task.Name),
            Sort.Priority => tasks.OrderByDescending(task => task.Priority),
            Sort.ElapsedTime => tasks.OrderByDescending(task => task.ElapsedTime),
            Sort.ElapsedToAverageRatio => tasks.OrderByDescending(task => task.ElapsedToAverageRatio),
            Sort.ElapsedToDesiredRatio => tasks.OrderByDescending(task => task.ElapsedToDesiredRatio),
            Sort.AverageToDesiredRatio => tasks.OrderByDescending(task => task.AverageToDesiredRatio),
            _ => throw new ArgumentException("Invalid argument: " + nameof(Sort))
        };
    }
}
