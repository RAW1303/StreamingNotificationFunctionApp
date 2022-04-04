using System;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Translators
{
    public class VideoToDiscordMessageTranslator
    {
        private readonly ILogger<VideoToDiscordMessageTranslator> _logger;

        public VideoToDiscordMessageTranslator(ILogger<VideoToDiscordMessageTranslator> logger)
        {
            _logger = logger;
        }

        public Message Translate(Video video)
        {
            var lines = new string[] {
                $"{video.AuthorName} has published a new video!",
                $":pencil2: {video.Title}",
                $":tv: {video.Url}"
            };

            return new Message()
            {
                Content = string.Join("\n", lines)
            };
        }
    }
}
