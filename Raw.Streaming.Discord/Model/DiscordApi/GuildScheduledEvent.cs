using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Discord.Model.DiscordApi
{
    [ExcludeFromCodeCoverage]
    internal class GuildScheduledEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("guild_id")]
        public int GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public int ChannelId { get; set; }
        [JsonPropertyName("creator_id")]
        public int CreatorId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
        [JsonPropertyName("scheduled_start_time")]
        public DateTime ScheduledStartTime { get; set; }
        [JsonPropertyName("scheduled_end_time")]
        public DateTime ScheduledEndTime { get; set; }
        [JsonPropertyName("privacy_level")]
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; }
        [JsonPropertyName("status")]
        public GuildScheduledEventStatus Status { get; set; }
        [JsonPropertyName("entity_type")]
        public GuildScheduledEventEntityType EntityType { get; set; }
        [JsonPropertyName("entity_id")]
        public int? EntityId { get; set; }
        [JsonPropertyName("entity_metadata")]
        public GuildScheduledEventEntityMetadata EntityMetadata { get; set; }

    }

    [ExcludeFromCodeCoverage]
    internal class GuildScheduledEventEntityMetadata
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }
    }

    internal enum GuildScheduledEventPrivacyLevel
    {
        GUILD_ONLY = 2
    }

    internal enum GuildScheduledEventStatus
    {
        SCHEDULED = 1,
        ACTIVE = 2,
        COMPLETED = 3,
        CANCELED = 4
    }

    internal enum GuildScheduledEventEntityType
    {
        STAGE_INSTANCE = 1,
        VOICE = 2,
        EXTERNAL = 3
    }
}
