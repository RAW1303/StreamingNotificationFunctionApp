using Raw.Streaming.Webhook.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Syndication;

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

        public bool IsNewVideo()
        {
            return Updated >= Published 
                && Updated <= Published.AddMinutes(15);
        }

        public static YoutubeFeed Create(SyndicationFeed feed)
        {
            if (!feed?.Items?.Any() ?? false)
                return default;

            var item = feed.Items.First();
            return new YoutubeFeed()
            {
                ChannelId = item.GetElementExtensionValueByOuterName("channelId"),
                VideoId = item.GetElementExtensionValueByOuterName("videoId"),
                Title = item.Title?.Text,
                Link = item.Links[0].Uri.ToString(),
                Published = item.PublishDate,
                Updated = item.LastUpdatedTime,
                Author = new Author
                {
                    Name = item.Authors[0].Name,
                    Uri = item.Authors[0].Uri
                }
            };
        }
    }

    [ExcludeFromCodeCoverage]
    public class Author
    {
        public string Name { get; set; }
        public string Uri { get; set; }
    }
}
