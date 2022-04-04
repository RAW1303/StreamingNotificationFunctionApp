using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Translators
{
    public static class TwitchClipToDiscordMessageTranslator
    {
        public static Message Translate(Clip clip)
        {
            var lines = new string[] {
                $"New Clip of {clip.BroadcasterName}",
                $":pencil2: {clip.Title}",
                $":video_game: {clip.GameName}",
                $":clapper: {clip.CreatorName}",
                $":tv: {clip.Url}"
            };

            return new Message()
            {
                Content = string.Join("\n", lines)
            };
        }
    }
}
