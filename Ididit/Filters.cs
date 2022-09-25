using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ididit;

public class Filters
{
    public string SearchFilter { get; set; } = string.Empty;

    public DateTime? DateFilter { get; set; }

    public Dictionary<Priority, bool> ShowPriority { get; set; } = new()
    {
        { Priority.None, true },
        { Priority.VeryLow, true },
        { Priority.Low, true },
        { Priority.Medium, true },
        { Priority.High, true },
        { Priority.VeryHigh, true }
    };

    public Dictionary<TaskKind, bool> ShowTaskKind { get; set; } = new()
    {
        { TaskKind.Note, true },
        { TaskKind.Task, true },
        { TaskKind.RepeatingTask, true }
    };

    public Sort Sort { get; set; }

    public long ElapsedToDesiredRatioMin { get; set; }

    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    public bool HideEmptyGoals { get; set; }

    public bool ShowCategoriesInGoalList { get; set; }

    public bool HideCompletedTasks { get; set; }

    public IList<TaskModel> FilterTasks(IEnumerable<TaskModel> tasks)
    {
        IEnumerable<TaskModel> filteredTasks = tasks.Where(task =>
        {
            bool isRatioOk = task.ElapsedToDesiredRatio >= ElapsedToDesiredRatioMin;

            bool isNameOk = string.IsNullOrEmpty(SearchFilter) || task.Name.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase);

            bool isDateOk = DateFilter == null || task.TimeList.Any(time => time.Date == DateFilter?.Date);

            bool isPriorityOk = ShowPriority[task.Priority];

            bool isTaskKindOk = ShowTaskKind[task.TaskKind];

            return isNameOk && isDateOk && isPriorityOk && isTaskKindOk &&
                (isRatioOk || !ShowElapsedToDesiredRatioOverMin) &&
                (!task.IsCompletedTask || !HideCompletedTasks);
        });

        return SortTasks(filteredTasks).ToList();
    }

    private IEnumerable<TaskModel> SortTasks(IEnumerable<TaskModel> tasks)
    {
        return Sort switch
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
