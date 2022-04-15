using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

internal class SettingsModel
{
    [JsonIgnore]
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Size { get; set; } = "medium";

    public string Theme { get; set; } = string.Empty;
}
