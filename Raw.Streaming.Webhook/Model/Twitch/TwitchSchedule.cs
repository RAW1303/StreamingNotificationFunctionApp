using Raw.Streaming.Webhook.JsonConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        public IEnumerable<TwitchScheduleSegment> Segments { get; set; } = new List<TwitchScheduleSegment>();

        public IEnumerable<TwitchScheduleSegment> SegmentsExcludingVaction
        { 
            get
            {
                if(Vacation?.StartTime == null)
                    return Segments;

                return Segments.Where(x => x.EndTime < Vacation.StartTime || x.StartTime > Vacation.EndTime);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    internal class TwitchScheduleSegment
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(Base64ObjectJsonConverter<TwitchScheduleId>))]
        public TwitchScheduleId Id { get; set; }
        [JsonPropertyName("start_time")]
        public DateTimeOffset StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTimeOffset? EndTime { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("canceled_until")]
        public DateTimeOffset? CancelledUntil { get; set; }
        [JsonPropertyName("category")]
        public TwitchGame Category { get; set; }
        [JsonPropertyName("is_recurring")]
        public bool IsRecurring { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class TwitchScheduleId
    {
        public string SegmentId { get; set; }
        public int IsoYear { get; set; }
        public int IsoWeek { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class TwitchScheduleVacation
    {
        [JsonPropertyName("start_time")]
        public DateTimeOffset StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTimeOffset EndTime { get; set; }
    }
}
