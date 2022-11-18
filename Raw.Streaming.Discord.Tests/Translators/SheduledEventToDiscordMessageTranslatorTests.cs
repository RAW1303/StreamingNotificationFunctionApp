using AutoFixture;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Translators;
using System.Linq;

namespace Raw.Streaming.Discord.Tests.Translators;

[TestFixture]
internal class SheduledEventToDiscordMessageTranslatorTests
{
    private readonly Fixture _fixture = new();
    private const string TEST_URL = "http://test.com/events";

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("DiscordEventsUrl", TEST_URL);
    }

    [Test]
    public void TranslateList_WhenValidList_ReturnsCorrectString()
    {
        //Arrange
        var input = new List<GuildScheduledEvent>
        {
            _fixture.Build<GuildScheduledEvent>().With(x => x.GuildId, "GuildId1").With(x => x.Id, "EventId1").Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.GuildId, "GuildId2").With(x => x.Id, "EventId2").Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.GuildId, "GuildId3").With(x => x.Id, "EventId3").Create(),
        };

        //Act
        var result = SheduledEventToDiscordMessageTranslator.TranslateDailySchedule(input);

        //Assert
        Assert.That(result, Has.Property("Content").EqualTo($"{TEST_URL}/GuildId1/EventId1\n{TEST_URL}/GuildId2/EventId2\n{TEST_URL}/GuildId3/EventId3"));
        Assert.That(result, Has.Property("Embeds").Null);
    }

    [Test]
    public void TranslateEvent_WhenEmptyList_ReturnsEmptyString()
    {
        //Arange
        var input = new List<GuildScheduledEvent>();

        //Act
        var result = SheduledEventToDiscordMessageTranslator.TranslateDailySchedule(input);

        //Assert
        Assert.That(result, Has.Property("Content").Empty);
        Assert.That(result, Has.Property("Embeds").Null);
    }
}
