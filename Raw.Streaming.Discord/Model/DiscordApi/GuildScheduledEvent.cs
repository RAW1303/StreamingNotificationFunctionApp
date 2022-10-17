using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Discord.Model.DiscordApi
{
    [ExcludeFromCodeCoverage]
    internal class GuildScheduledEvent : DiscordApiContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }
        [JsonPropertyName("creator_id")]
        public string? CreatorId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("image")]
        public string? Image { get; set; }
        [JsonPropertyName("scheduled_start_time")]
        public DateTimeOffset ScheduledStartTime { get; set; }
        [JsonPropertyName("scheduled_end_time")]
        public DateTimeOffset ScheduledEndTime { get; set; }
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

        public bool HasDescriptionParameter(string name)
        {
            return GetDescriptionLines().Any(x => x.StartsWith(name));
        }

        public bool HasDescriptionParameter(string name, string value)
        {
            return HasDescriptionParameter(name) && GetDescriptionParameter(name) == value;
        }

        public string GetDescriptionParameter(string name)
        {
            return GetDescriptionLines()
                .First(x => x.StartsWith(name))
                .Substring(name.Length + 1);
        }

        public void AddDescriptionParameter(string name, string value)
        {
            Description = $"{Description}\n\n{name}:{value}"; 
        }

        private string[] GetDescriptionLines()
        {
            return Description.Split('\n');
        }
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
