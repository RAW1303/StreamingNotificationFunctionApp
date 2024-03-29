﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch;

[ExcludeFromCodeCoverage]
public class TwitchVideo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("duration")]
    public string Duration { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("published_at")]
    public DateTimeOffset PublishedAt { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    [JsonPropertyName("viewable")]
    public string Viewable { get; set; }

    [JsonPropertyName("view_count")]
    public int ViewCount { get; set; }
}
