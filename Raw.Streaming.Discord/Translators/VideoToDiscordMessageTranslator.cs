using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Translators
{
    internal static class VideoToDiscordMessageTranslator
    {
        public static Message Translate(Video video)
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
