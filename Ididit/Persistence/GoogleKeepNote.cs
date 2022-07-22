using System.Text.Json.Serialization;

namespace Ididit.Persistence;

internal class GoogleKeepNote
{
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    [JsonPropertyName("isTrashed")]
    public bool IsTrashed { get; set; }

    [JsonPropertyName("isPinned")]
    public bool IsPinned { get; set; }

    [JsonPropertyName("isArchived")]
    public bool IsArchived { get; set; }

    [JsonPropertyName("textContent")]
    public string TextContent { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("userEditedTimestampUsec")]
    public long UserEditedTimestampUsec { get; set; }

    [JsonPropertyName("createdTimestampUsec")]
    public long CreatedTimestampUsec { get; set; }
}
