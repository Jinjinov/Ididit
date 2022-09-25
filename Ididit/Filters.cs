using System;
using System.Collections.Generic;

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
}
