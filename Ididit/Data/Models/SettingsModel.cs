using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

internal class SettingsModel
{
    [JsonIgnore]
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Size { get; set; } = "medium";

    public string Theme { get; set; } = string.Empty;

    public Sort Sort { get; set; }

    public long ElapsedToDesiredRatioMin { get; set; }

    public bool ShowElapsedToDesiredRatioOverMin { get; set; }

    public bool ShowOnlyRepeating { get; set; }

    public bool ShowOnlyAsap { get; set; }

    public bool AlsoShowCompletedAsap { get; set; }
}
