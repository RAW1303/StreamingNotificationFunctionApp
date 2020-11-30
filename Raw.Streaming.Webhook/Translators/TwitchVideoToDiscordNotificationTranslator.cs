using System;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchVideoToDiscordNotificationTranslator
    {
        private ILogger<TwitchVideoToDiscordNotificationTranslator> _logger;

        public TwitchVideoToDiscordNotificationTranslator(ILogger<TwitchVideoToDiscordNotificationTranslator> logger)
        {
            _logger = logger;
        }

        public DiscordNotification Translate(TwitchVideo twitchVideo)
        {
            if(!TimeSpan.TryParseExact(twitchVideo.Duration, @"%h\h%m\m%s\s", null, out var duration))
            {
                _logger.LogWarning($"Could not parse duration string: {twitchVideo.Duration}");
            }

            return new DiscordNotification()
            {
                Embeds = new DiscordEmbed[]
                {
                    new DiscordEmbed()
                    {
                        Author = new DiscordEmbedAuthor()
                        {
                            Name = $"New Highlight from {twitchVideo.UserName}"
                        },
                        Title = twitchVideo.Title,
                        Url = twitchVideo.Url,
                        Description = twitchVideo.Description,
                        Color = 6570404,
                        Fields = duration == TimeSpan.Zero ? null : new DiscordEmbedField[]
                        {
                            new DiscordEmbedField()
                            {
                                Name = "Duration",
                                Value = duration.ToString("c"),
                                Inline = true
                            }
                        },
                        Image = new DiscordEmbedImage()
                        {
                            Url = new Uri(twitchVideo.ThumbnailUrl.Replace("%{width}x%{height}", AppSettings.VideoThumbnailSize)).AbsoluteUri
                        },
                        Footer = new DiscordEmbedFooter()
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
