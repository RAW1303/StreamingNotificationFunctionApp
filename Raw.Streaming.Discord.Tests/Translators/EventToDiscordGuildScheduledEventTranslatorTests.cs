using Raw.Streaming.Discord.Translators;
using System.Linq;

namespace Raw.Streaming.Discord.Tests.Translators;

[TestFixture]
internal class EventToDiscordGuildScheduledEventTranslatorTests
{
    [Test, AutoData]
    public void TranslateList_WhenValidList_ReturnsList(IEnumerable<Event> events)
    {
        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.Translate(events);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Exactly(events.Count()).Items);
    }
}
