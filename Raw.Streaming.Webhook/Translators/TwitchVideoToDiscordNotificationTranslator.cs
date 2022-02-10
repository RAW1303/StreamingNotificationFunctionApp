using System;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Twitch;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchVideoToDiscordNotificationTranslator
    {
        private readonly ILogger<TwitchVideoToDiscordNotificationTranslator> _logger;

        public TwitchVideoToDiscordNotificationTranslator(ILogger<TwitchVideoToDiscordNotificationTranslator> logger)
        {
            _logger = logger;
        }

        public Notification Translate(Video twitchVideo)
        {
            if(!TimeSpan.TryParseExact(twitchVideo.Duration, @"%h\h%m\m%s\s", null, out var duration))
            {
                _logger.LogWarning($"Could not parse duration string: {twitchVideo.Duration}");
            }

            return new Notification()
            {
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = $"New Highlight from {twitchVideo.UserName}"
                        },
                        Title = twitchVideo.Title,
                        Url = twitchVideo.Url,
                        Description = twitchVideo.Description,
                        Color = 6570404,
                        Fields = duration == TimeSpan.Zero ? null : new EmbedField[]
                        {
                            new EmbedField()
                            {
                                Name = "Duration",
                                Value = duration.ToString("c"),
                                Inline = true
                            }
                        },
                        Image = new EmbedImage()
                        {
                            Url = new Uri(twitchVideo.ThumbnailUrl.Replace("%{width}x%{height}", AppSettings.DiscordVideoThumbnailSize)).AbsoluteUri
                        },
                        Footer = new EmbedFooter()
                        {
                            Text = "Highlight Published"
                        },
                        Timestamp = twitchVideo.PublishedAt
                    }
                }
            };
        }
    }
}
