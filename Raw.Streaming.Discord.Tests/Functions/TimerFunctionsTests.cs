using AutoFixture;
using Raw.Streaming.Discord.Functions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Discord.Tests.Functions;

[TestFixture]
internal class TimerFunctionsTests
{
    private Mock<IDiscordEventService> _discordEventService;
    private Mock<IDiscordMessageService> _discordMessageService;
    private Mock<ILogger<TimerFunctions>> _loggerMock;
    private TimerFunctions _controller;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _discordEventService = new Mock<IDiscordEventService>();
        _discordMessageService = new Mock<IDiscordMessageService>();
        _loggerMock = new Mock<ILogger<TimerFunctions>>();
        _controller = new TimerFunctions(_discordEventService.Object, _discordMessageService.Object, _loggerMock.Object);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_WhenEvents_SendDiscordMessageAsyncIsCalled(DateTimeOffset triggerTime, string channelId)
    {
        // Arrange
        var events = new List<GuildScheduledEvent>
        {
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date.AddHours(1)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date.AddHours(2)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.AddHours(3)).Create()
        };

        Environment.SetEnvironmentVariable("DiscordScheduleChannelId", channelId);
        _discordEventService
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(events);
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());

        // Act and Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(channelId, It.IsAny<Message>()), Times.Once);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_WhenNoEvents_DoesNotCallMessageService(DateTimeOffset triggerTime, string channelId)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DiscordScheduleChannelId", channelId);
        _discordEventService
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<GuildScheduledEvent>());
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());

        // Act and Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()), Times.Never);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_WhenNoEventsOnTriggerDay_DoesNotCallMessageService(DateTimeOffset triggerTime, string channelId)
    {
        // Arrange
        var events = new List<GuildScheduledEvent>
        {
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date.AddDays(1)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.AddDays(1)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.AddDays(2)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.AddDays(3)).Create()
        };

        Environment.SetEnvironmentVariable("DiscordScheduleChannelId", channelId);
        _discordEventService
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<GuildScheduledEvent>());
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());

        // Act and Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Nothing);
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()), Times.Never);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_GetScheduledEventsThrowsException_DoesNotCallMessageServiceAndThrows(DateTimeOffset triggerTime, string channelId, string exceptionMessage)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DiscordScheduleChannelId", channelId);
        _discordEventService
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ReturnsAsync(new Message());

        // Act and Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Exception.With.Message.EqualTo(exceptionMessage));
        _discordMessageService.Verify(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()), Times.Never);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_SendDiscordMessageAsyncThrowsException_Throws(DateTimeOffset triggerTime, string channelId, string exceptionMessage)
    {
        // Arrange
        var events = new List<GuildScheduledEvent>
        {
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date.AddHours(1)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.Date.AddHours(2)).Create(),
            _fixture.Build<GuildScheduledEvent>().With(x => x.ScheduledStartTime, triggerTime.AddHours(3)).Create()
        };

        Environment.SetEnvironmentVariable("DiscordScheduleChannelId", channelId);
        _discordEventService
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(events);
        _discordMessageService
            .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act and Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Exception.With.Message.EqualTo(exceptionMessage));
    }
}
