using AutoFixture;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Translators;
using System;
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
    public void TranslateEvent_WhenEndDateIsNull_ReturnsGuildScheduledEventWithNotNullScheduledEndTime()
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

    [Test, AutoData]
    public void MergeEvent_WhenSourceEndDateAndTargetScheduledEndTimeAreNotNull_ReturnsGuildScheduledEventWithSourceScheduledEndTime(DateTimeOffset sourceEnd, DateTimeOffset targetEnd)
    {
        //Arrange
        var source = _fixture.Build<Event>().With(x => x.End, sourceEnd).Create();
        var target = _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledEndTime, targetEnd).Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.Merge(target, source);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("ScheduledEndTime").Not.Null);
        Assert.That(result.ScheduledEndTime, Is.EqualTo(sourceEnd));
    }

    [Test, AutoData]
    public void MergeEvent_WhenSourceEndDateIsNull_ReturnsGuildScheduledEventWithTargetScheduledEndTime(DateTimeOffset dateTime)
    {
        //Arrange
        var source = _fixture.Build<Event>().Without(x => x.End).Create();
        var target = _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledEndTime, dateTime).Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.Merge(target, source);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("ScheduledEndTime").Not.Null);
        Assert.That(result.ScheduledEndTime, Is.EqualTo(dateTime));
    }

    [Test, AutoData]
    public void IsUpdate_WhenDifferentUrl_ReturnsFalse()
    {
        //Arrange
        var source = _fixture.Build<Event>().With(x => x.Url, "Test1").Create();
        var target = _fixture.Build<GuildScheduledEvent>().With(x => x.EntityMetadata, new GuildScheduledEventEntityMetadata { Location = "Test2" }).Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.IsUpdate(target, source);

        //Assert
        Assert.That(result, Is.False);
    }

    [Test, AutoData]
    public void IsUpdate_WhenAllSame_ReturnsFalse(string url, string title, string description, DateTimeOffset start, DateTimeOffset end)
    {
        //Arrange
        var source = new Event
        {
            Url = url,
            Title = title,
            Description = description,
            Start = start,
            End = end,
            IsRecurring = true
        };

        var target = _fixture.Build<GuildScheduledEvent>()
            .With(x => x.EntityMetadata, new GuildScheduledEventEntityMetadata { Location = url })
            .With(x => x.Name, title)
            .With(x => x.Description, description)
            .With(x => x.ScheduledStartTime, start)
            .With(x => x.ScheduledEndTime, end)
            .Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.IsUpdate(target, source);

        //Assert
        Assert.That(result, Is.False);
    }

    [Test, AutoData]
    public void IsUpdate_WhenDifferent_ReturnsTrue()
    {
        //Arrange
        var source = _fixture.Build<Event>().With(x => x.Url, "Test").Create();
        var target = _fixture.Build<GuildScheduledEvent>().With(x => x.EntityMetadata, new GuildScheduledEventEntityMetadata { Location = "Test" }).Create();

        //Act
        var result = EventToDiscordGuildScheduledEventTranslator.IsUpdate(target, source);

        //Assert
        Assert.That(result, Is.True);
    }
}
