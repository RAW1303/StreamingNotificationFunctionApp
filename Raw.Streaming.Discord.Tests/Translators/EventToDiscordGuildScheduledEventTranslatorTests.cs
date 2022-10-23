using AutoFixture;
using Raw.Streaming.Discord.Translators;
using System.Linq;

namespace Raw.Streaming.Discord.Tests.Translators;

[TestFixture]
internal class EventToDiscordGuildScheduledEventTranslatorTests
{
    private readonly Fixture _fixture = new();

    [Test, AutoData]
    public void TranslateList_WhenValidList_ReturnsList(IEnumerable<Event> events)
    {
        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.Translate(events);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Exactly(events.Count()).Items);
    }

    [Test]
    public void TranslateEvent_WhenEndDateIsNull_ReturnsGuildScheduledEventWithNotNullEndDate()
    {
        //Arrange
        var input = _fixture.Build<Event>().Without(x => x.End).Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.Translate(input);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("ScheduledEndTime").Not.Null);
        Assert.That(result.ScheduledEndTime, Is.GreaterThan(result.ScheduledStartTime));
    }
}
