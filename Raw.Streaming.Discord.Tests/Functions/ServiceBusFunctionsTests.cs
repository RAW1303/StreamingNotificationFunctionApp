using Raw.Streaming.Discord.Functions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Discord.Tests.Functions;

[TestFixture]
internal class ServiceBusFunctionsTests
{
    private Mock<IDiscordEventService> _discordEventService;
    private Mock<IDiscordMessageService> _discordMessageService;
    private Mock<ILogger<ServiceBusFunctions>> _loggerMock;
    private ServiceBusFunctions _controller;

    [SetUp]
    public void Setup()
    {
        _discordEventService = new Mock<IDiscordEventService>();
        _discordMessageService = new Mock<IDiscordMessageService>();
        _loggerMock = new Mock<ILogger<ServiceBusFunctions>>();
        _controller = new ServiceBusFunctions(_discordEventService.Object, _discordMessageService.Object, _loggerMock.Object);
    }

    [Test, AutoData]
    public void ProcessGoLiveMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(GoLive goLive, string channelId)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DiscordStreamGoLiveChannelId", channelId);
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());
        var queueMessage = new DiscordBotQueueItem<GoLive>(goLive);

        // Act and Assert
        Assert.That(() => _controller.ProcessGoLiveMessageQueue(queueMessage), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(channelId, It.IsAny<Message>()));
    }

    [Test, AutoData]
    public void ProcessGoLiveMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(GoLive goLive)
    {
        // Arrange
        var exception = new Exception("Test message");
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ThrowsAsync(exception);
        var queueMessage = new DiscordBotQueueItem<GoLive>(goLive);

        // Act and Assert
        Assert.That(() => _controller.ProcessGoLiveMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public void ProcessClipMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Clip[] clips, string channelId)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DiscordClipChannelId", channelId);
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());
        var queueMessage = new DiscordBotQueueItem<Clip>(clips);

        // Act and Assert
        Assert.That(() => _controller.ProcessClipMessageQueue(queueMessage), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(channelId, It.IsAny<Message>()));
    }

    [Test, AutoData]
    public void ProcessClipMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Clip[] clips)
    {
        // Arrange
        var exception = new Exception("Test message");
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ThrowsAsync(exception);
        var queueMessage = new DiscordBotQueueItem<Clip>(clips);

        // Act and Assert
        Assert.That(() => _controller.ProcessClipMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public void ProcessVideoMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Video[] videos, string channelId)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DiscordVideoChannelId", channelId);
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());
        var queueMessage = new DiscordBotQueueItem<Video>(videos);

        // Act and Assert
        Assert.That(() => _controller.ProcessVideoMessageQueue(queueMessage), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(channelId, It.IsAny<Message>()));
    }

    [Test, AutoData]
    public void ProcessVideoMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Video[] videos)
    {
        // Arrange
        var exception = new Exception("Test message");
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ThrowsAsync(exception);
        var queueMessage = new DiscordBotQueueItem<Video>(videos);

        // Act and Assert
        Assert.That(() => _controller.ProcessVideoMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public void ProcessEventMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Event[] events)
    {
        // Arrange
        _discordEventService
            .Setup(x => x.SyncScheduledEvents(It.IsAny<string>(), It.IsAny<IEnumerable<Event>>()))
            .ReturnsAsync(new List<GuildScheduledEvent>());
        var queueMessage = new DiscordBotQueueItem<Event>(events);

        // Act and Assert
        Assert.That(() => _controller.ProcessEventMessageQueue(queueMessage), Throws.Nothing);
    }

    [Test, AutoData]
    public void ProcessEventMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Event[] events)
    {
        // Arrange
        var exception = new Exception("Test message");
        _discordEventService
            .Setup(x => x.SyncScheduledEvents(It.IsAny<string>(), It.IsAny<IEnumerable<Event>>()))
            .ThrowsAsync(exception);
        var queueMessage = new DiscordBotQueueItem<Event>(events);

        // Act and Assert
        Assert.That(() => _controller.ProcessEventMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
    }
}
