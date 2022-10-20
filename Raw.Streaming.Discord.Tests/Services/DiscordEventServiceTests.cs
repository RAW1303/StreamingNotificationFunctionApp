using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Discord.Tests.Services;

[TestFixture]
internal class DiscordEventServiceTests
{
    private Mock<IDiscordApiService> _mockDiscordApiService;
    private Mock<ILogger<DiscordEventService>> _loggerMock;
    private DiscordEventService _service;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<DiscordEventService>>();
        _mockDiscordApiService = new Mock<IDiscordApiService>();
        _service = new DiscordEventService(_mockDiscordApiService.Object, _loggerMock.Object);
    }

    [Test, AutoData]
    public async Task GetScheduledEvents_WhenApiServiceReturnsSuccessfully_ReturnsList(IEnumerable<GuildScheduledEvent> events)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(It.IsAny<string>()))
            .ReturnsAsync(events);

        //Act
        var result = await _service.GetScheduledEvents("test");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(events));
    }

    [Test, AutoData]
    public void GetScheduledEvents_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(It.IsAny<string>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.GetScheduledEvents("test"), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task CreateScheduledEvent_WhenApiServiceReturnsSuccessfully_ReturnsList(GuildScheduledEvent guildEvent)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ReturnsAsync(guildEvent);

        //Act
        var result = await _service.CreateScheduledEvent("test", new GuildScheduledEvent());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(guildEvent));
    }

    [Test, AutoData]
    public void CreateScheduledEvent_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.CreateScheduledEvent("test", new GuildScheduledEvent()), Throws.Exception.EqualTo(exception));
    }
}
