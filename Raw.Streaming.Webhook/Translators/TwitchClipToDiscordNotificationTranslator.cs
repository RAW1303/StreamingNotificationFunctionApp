using Raw.Streaming.Webhook.Model;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchClipToDiscordNotificationTranslator
    {
        public DiscordNotification Translate(TwitchClip twitchClip, TwitchGame game)
        {
            return new DiscordNotification()
            {
                Embeds = new DiscordEmbed[]
                {
                    new DiscordEmbed()
                    {
                        Author = new DiscordEmbedAuthor()
                        {
                            Name = $"New Clip of {twitchClip.BroadcasterName}"
                        },
                        Title = twitchClip.Title,
                        Url = twitchClip.Url,
                        Color = 6570404,
                        Fields = new DiscordEmbedField[]
                        {
                            new DiscordEmbedField()
                            {
                                Name = "Game",
                                Value = game.Name,
                                Inline = true
                            },
                            new DiscordEmbedField()
                            {
                                Name = "Created By",
                                Value = twitchClip.CreatorName,
                                Inline = true
                            }
                        },
                        Image = new DiscordEmbedImage()
                        {
                            Url = twitchClip.ThumbnailUrl
                        },
                        Footer = new DiscordEmbedFooter()
                        {
                            Text = "Clip created"
                        },
                        Timestamp = twitchClip.CreatedAt
                    }
                }
            };
        }
    }
}
