using System;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Webhook.Translators
{
    public static class TwitchClipToDiscordNotificationTranslator
    {
        public static Message Translate(Clip clip)
        {
            return new Message()
            {
                Embeds = new Embed[]
                {
                    new Embed()
                    {
                        Author = new EmbedAuthor()
                        {
                            Name = $"New Clip of {clip.BroadcasterName}"
                        },
                        Title = clip.Title,
                        Url = clip.Url,
                        Color = 6570404,
                        Fields = new EmbedField[]
                        {
                            new EmbedField()
                            {
                                Name = "Game",
                                Value = clip.GameName,
                                Inline = true
                            },
                            new EmbedField()
                            {
                                Name = "Created By",
                                Value = clip.CreatorName,
                                Inline = true
                            }
                        },
                        Image = new EmbedImage()
                        {
                            Url = new Uri(clip.ThumbnailUrl).AbsoluteUri
                        },
                        Footer = new EmbedFooter()
                        {
                            Text = "Clip created"
                        },
                        Timestamp = clip.CreatedAt
                    }
                }
            };
        }
    }
}
