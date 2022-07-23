using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class SettingsModel
{
    [JsonIgnore]
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Blazorise.Size Size { get; set; }

    public string Theme { get; set; } = string.Empty;

    public Sort Sort { get; set; }

    public long ElapsedToDesiredRatioMin { get; set; }

    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    public bool HideEmptyGoals { get; set; }

    public bool ShowCategoriesInGoalList { get; set; }

    public bool HideCompletedTasks { get; set; }
}
