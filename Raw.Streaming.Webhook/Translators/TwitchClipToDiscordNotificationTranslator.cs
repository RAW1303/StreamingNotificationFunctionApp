using System;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Twitch;

namespace Raw.Streaming.Webhook.Translators
{
    public class TwitchClipToDiscordNotificationTranslator
    {
        public Notification Translate(Clip twitchClip, Game game)
        {
            return new Notification()
            {
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = $"New Clip of {twitchClip.BroadcasterName}"
                        },
                        Title = twitchClip.Title,
                        Url = twitchClip.Url,
                        Color = 6570404,
                        Fields = new EmbedField[]
                        {
                            new EmbedField()
                            {
                                Name = "Game",
                                Value = game.Name,
                                Inline = true
                            },
                            new EmbedField()
                            {
                                Name = "Created By",
                                Value = twitchClip.CreatorName,
                                Inline = true
                            }
                        },
                        Image = new EmbedImage()
                        {
                            Url = new Uri(twitchClip.ThumbnailUrl).AbsoluteUri
                        },
                        Footer = new EmbedFooter()
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
