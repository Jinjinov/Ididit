using DnetIndexedDb;
using Ididit.Backup;
using Ididit.UI;
using System.Collections.Generic;

namespace Ididit.Data.Database.Entities;

internal class SettingsEntity
{
    [IndexDbKey]
    public long Id { get; init; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;

    [IndexDbIndex]
    public DataFormat SelectedBackupFormat { get; set; } = DataFormat.Markdown;

    [IndexDbIndex]
    public Blazorise.Size Size { get; set; }

    [IndexDbIndex]
    public string Culture { get; set; } = "en";

    [IndexDbIndex]
    public string Theme { get; set; } = "default";

    [IndexDbIndex]
    public string Background { get; set; } = "Default";

    [IndexDbIndex]
    public bool ShowAllGoals { get; set; } = true;

    [IndexDbIndex]
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

    [IndexDbIndex]
    public Sort Sort { get; set; }

    [IndexDbIndex]
    public Screen Screen { get; set; } = Screen.Main;

    [IndexDbIndex]
    public long ElapsedToDesiredRatioMin { get; set; }

    [IndexDbIndex]
    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    [IndexDbIndex]
    public bool HideEmptyGoals { get; set; }

    [IndexDbIndex]
    public bool HideGoalsWithSimpleText { get; set; }

    [IndexDbIndex]
    public bool ShowCategoriesInGoalList { get; set; }

    [IndexDbIndex]
    public bool HideCompletedTasks { get; set; }
}
