using Ididit.Backup;
using Ididit.UI;
using System.Collections.Generic;

namespace Ididit.Data.Model.Models;

public class SettingsModel
{
    public long Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public DataFormat SelectedBackupFormat { get; set; } = DataFormat.Markdown;

    public Blazorise.Size Size { get; set; }

    public string Culture { get; set; } = "en";

    public string Theme { get; set; } = "default";

    public string Background { get; set; } = "Default";

    public bool ShowAllGoals { get; set; } = true;

    public bool ShowAllTasks { get; set; }

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

    public Screen Screen { get; set; } = Screen.Main;

    public long ElapsedToDesiredRatioMin { get; set; }

    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    public bool HideEmptyGoals { get; set; }

    public bool HideGoalsWithSimpleText { get; set; }

    public bool ShowCategoriesInGoalList { get; set; }

    public bool HideCompletedTasks { get; set; }
}
