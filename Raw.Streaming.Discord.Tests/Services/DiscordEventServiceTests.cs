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
    public async Task GetScheduledEventsAsync_WhenApiServiceReturnsSuccessfully_ReturnsList(IEnumerable<GuildScheduledEvent> events)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(It.IsAny<string>()))
            .ReturnsAsync(events);

        //Act
        var result = await _service.GetScheduledEventsAsync("test");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(events));
    }

    [Test, AutoData]
    public void GetScheduledEventsAsync_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(It.IsAny<string>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.GetScheduledEventsAsync("test"), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task CreateScheduledEventAsync_WhenApiServiceReturnsSuccessfully_ReturnsList(GuildScheduledEvent guildEvent)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ReturnsAsync(guildEvent);

        //Act
        var result = await _service.CreateScheduledEventAsync("test", new GuildScheduledEvent());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(guildEvent));
    }

    [Test, AutoData]
    public void CreateScheduledEventAsync_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.CreateScheduledEventAsync("test", new GuildScheduledEvent()), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task UpdateScheduledEventAsync_WhenApiServiceReturnsSuccessfully_ReturnsList(GuildScheduledEvent guildEvent)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPatchRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ReturnsAsync(guildEvent);

        //Act
        var result = await _service.UpdateScheduledEventAsync("test", "testId", new GuildScheduledEvent());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(guildEvent));
    }

    [Test, AutoData]
    public void UpdateScheduledEventAsync_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPatchRequestAsync<GuildScheduledEvent>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.UpdateScheduledEventAsync("test", "testId", new GuildScheduledEvent()), Throws.Exception.EqualTo(exception));
    }


    [Test, AutoData]
    public async Task DeleteScheduledEventAsync_WhenApiServiceReturnsSuccessfully_ReturnsList()
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiDeleteRequestAsync(It.IsAny<string>()));

        //Act
        await _service.DeleteScheduledEventAsync("test", "testId");

        //Assert
        _mockDiscordApiService.Verify(x => x.SendDiscordApiDeleteRequestAsync(It.IsAny<string>()));
    }

    [Test, AutoData]
    public void DeleteScheduledEventAsync_WhenApiServiceThrowsException_LogsErrorAndThrows(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiDeleteRequestAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.DeleteScheduledEventAsync("test", "testId"), Throws.Exception.EqualTo(exception));
    }
}
