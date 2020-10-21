using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchStreamChangeToDiscordNotificationTranslator
    {
        public DiscordNotification Translate(TwitchStreamChange twitchStreamChange, TwitchGame game)
        {
            return new DiscordNotification()
            {
                Content = $"<@&766814146330624011>",
                Embeds = new DiscordEmbed[]
                {
                    new DiscordEmbed()
                    {
                        Author = new DiscordEmbedAuthor()
                        {
                            Name = $"{twitchStreamChange.UserName} is now streaming"
                        },
                        Title = $"{twitchStreamChange.Title}",
                        Url = $"https://twitch.tv/{twitchStreamChange.UserName}",
                        Color = 6570404,
                        Fields = new DiscordEmbedField[]
                        {
                            new DiscordEmbedField()
                            {
                                Name = "Playing",
                                Value = game.Name,
                                Inline = true
                            }
                        },
                        Image = new DiscordEmbedImage()
                        {
                            Url = game.BoxArtUrl.Replace("{width}x{height}", AppSettings.GameBoxSize)
                        },
                        Footer = new DiscordEmbedFooter()
                        {
                            Text = "Stream started"
                        },
                        Timestamp = twitchStreamChange.StartedAt
                    }
                }
            };
        }
    }
}
