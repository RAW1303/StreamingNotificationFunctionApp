using Raw.Streaming.Webhook.Model;
using System;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchVideoToDiscordNotificationTranslator
    {
        public DiscordNotification Translate(TwitchVideo twitchVideo)
        {
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
                        Fields = new DiscordEmbedField[]
                        {
                            new DiscordEmbedField()
                            {
                                Name = "Duration",
                                Value = TimeSpan.ParseExact(twitchVideo.Duration, "h`hm`ms`s", null).ToString("c"),
                                Inline = true
                            }
                        },
                        Image = new DiscordEmbedImage()
                        {
                            Url = twitchVideo.ThumbnailUrl
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
