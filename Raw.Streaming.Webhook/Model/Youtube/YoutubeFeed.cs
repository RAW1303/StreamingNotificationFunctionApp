using System;

namespace Raw.Streaming.Webhook.Model.Youtube
{
    public class YoutubeFeed
    {
        public string Id { get; set; }
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public Author Author { get; set; }
        public DateTimeOffset Published { get; set; }
        public DateTimeOffset Updated { get; set; }
        public bool IsNewVideo(DateTimeOffset checkDateTime)
        {
            return Published != null
                && Published <= Updated
                && Published > checkDateTime.AddMinutes(-10)
                && Updated <= Published.AddMinutes(30);
        }
    }

    public class Author
    {
        public string Name { get; set; }
        public string Uri { get; set; }
    }
}
