﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ididit.Backup.Online;

internal class Annotation
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

internal class Label
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

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

    [JsonPropertyName("annotations")]
    public List<Annotation> Annotations { get; set; } = new();

    [JsonPropertyName("textContent")]
    public string TextContent { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("userEditedTimestampUsec")]
    public long UserEditedTimestampUsec { get; set; }

    [JsonPropertyName("createdTimestampUsec")]
    public long CreatedTimestampUsec { get; set; }

    [JsonPropertyName("labels")]
    public List<Label> Labels { get; set; } = new();
}