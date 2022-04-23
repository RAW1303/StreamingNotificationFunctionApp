﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    [ExcludeFromCodeCoverage]
    internal class TwitchSchedule
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("broadcaster_login")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("vacation")]
        public TwitchScheduleVacation Vacation { get; set; }
        [JsonPropertyName("segments")]
        public IList<TwitchScheduleSegment> Segments { get; set; } = new List<TwitchScheduleSegment>();
    }

    [ExcludeFromCodeCoverage]
    internal class TwitchScheduleSegment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("canceled_until")]
        public DateTime? CancelledUntil { get; set; }
        [JsonPropertyName("category")]
        public TwitchGame Category { get; set; }
        [JsonPropertyName("is_recurring")]
        public bool IsRecurring { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class TwitchScheduleVacation
    {
        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
    }
}