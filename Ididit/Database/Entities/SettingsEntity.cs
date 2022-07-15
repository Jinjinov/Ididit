using DnetIndexedDb;

namespace Ididit.Database.Entities;

internal class SettingsEntity
{
    [IndexDbKey]
    public long Id { get; set; }

    [IndexDbIndex]
    public string Name { get; set; } = string.Empty;

    [IndexDbIndex]
    public Blazorise.Size Size { get; set; }

    [IndexDbIndex]
    public string Theme { get; set; } = string.Empty;

    [IndexDbIndex]
    public Sort Sort { get; set; }

    [IndexDbIndex]
    public long ElapsedToDesiredRatioMin { get; set; }

    [IndexDbIndex]
    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    [IndexDbIndex]
    public bool HideEmptyGoals { get; set; }

    [IndexDbIndex]
    public bool ShowCategoriesInGoalList { get; set; }

    [IndexDbIndex]
    public bool AlsoShowCompletedAsap { get; set; }
}
