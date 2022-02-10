using System;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Model.Twitch.EventSub;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchStreamChangeToDiscordNotificationTranslator
    {
        public static Notification Translate(StreamOnlineEvent message, Channel channel, Game game)
        {
            return new Notification()
            {
                Content = AppSettings.DiscordNotificationGroupIds,
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = $"{channel.BroadcasterName} is now streaming on twitch"
                        },
                        Title = $"{channel.Title}",
                        Url = $"https://twitch.tv/{channel.BroadcasterName}",
                        Color = 6570404,
                        Fields = new EmbedField[]
                        {
                            new EmbedField()
                            {
                                Name = "Playing",
                                Value = game.Name,
                                Inline = true
                            }
                        },
                        Image = new EmbedImage()
                        {
                            Url = new Uri(game.BoxArtUrl.Replace("{width}x{height}", AppSettings.DiscordGameBoxSize)).AbsoluteUri
                        },
                        Footer = new EmbedFooter()
                        {
                            Text = "Stream started"
                        },
                        Timestamp = DateTime.Parse(message.StartedAt)
                    }
                }
            };
        }
    }
}
