using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model;

[ExcludeFromCodeCoverage]
public class DiscordBotQueueItem<T> where T : Entity
{
    public IEnumerable<T> Entities { get; set; }

    public DiscordBotQueueItem()
    {
        Entities = new List<T>();
    }

    public DiscordBotQueueItem(params T[] entities)
    {
        Entities = entities;
    }
}
