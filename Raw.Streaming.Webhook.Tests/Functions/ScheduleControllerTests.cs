using Raw.Streaming.Webhook.Functions;

namespace Raw.Streaming.Webhook.Tests.Functions;

[TestFixture]
internal class ScheduleControllerTests
{
    private Mock<IScheduleService> _scheduleService;
    private Mock<ILogger<ScheduleController>> _loggerMock;
    private ScheduleController _controller;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup()
    {
        _scheduleService = new Mock<IScheduleService>();
        _loggerMock = new Mock<ILogger<ScheduleController>>();
        _controller = new ScheduleController(_scheduleService.Object, _loggerMock.Object);
    }

    [Test, AutoData]
    public async Task NotifyDailySchedule_WhenGetScheduledStreamsAsyncReturnsEmptyList_ReturnsNull(DateTimeOffset triggerTime)
    {
        // Arrange
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(new List<Event>());

        // Act
        var result = await _controller.NotifyDailySchedule(triggerTime);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test, AutoData]
    public async Task NotifyDailySchedule_WhenGetScheduledStreamsAsyncReturnsItems_ReturnsServiceBusMessage(DateTimeOffset triggerTime, Event scheduledEvent)
    {
        // Arrange
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(new List<Event> { scheduledEvent });

        // Act
        var result = await _controller.NotifyDailySchedule(triggerTime);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test, AutoData]
    public void NotifyDailySchedule_WhenGetScheduledStreamsAsyncThrowsException_ThrowsException(DateTimeOffset triggerTime)
    {
        // Arrange
        var exception = new Exception("Test message");
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ThrowsAsync(exception);

        // Act & Assert
        Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task NotifyWeeklySchedule_WhenGetScheduledStreamsAsyncWorks_ReturnsServiceBusMessage(DateTimeOffset triggerTime)
    {
        // Arrange
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(new List<Event>());

        // Act
        var result = await _controller.NotifyWeeklySchedule(triggerTime);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test, AutoData]
    public void NotifyWeeklySchedule_WhenGetScheduledStreamsAsyncThrowsException_ThrowsException(DateTimeOffset triggerTime)
    {
        // Arrange
        var exception = new Exception("Test message");
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ThrowsAsync(exception);

        // Act & Assert
        Assert.That(() => _controller.NotifyWeeklySchedule(triggerTime), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task UpdateEventSchedule_WhenGetScheduledStreamsAsyncWorks_ReturnsServiceBusMessage(DateTimeOffset triggerTime)
    {
        // Arrange
        var events = new List<Event>
        {
            _fixture.Build<Event>().With(x => x.IsRecurring, true).Create(),
            _fixture.Build<Event>().With(x => x.IsRecurring, false).Create()
        };

        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), null))
            .ReturnsAsync(events);

        // Act
        var result = await _controller.UpdateEventSchedule(triggerTime);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Body.ToObjectFromJson<DiscordBotQueueItem<Event>>(), Has.Property("Entities").With.One.Items);
    }

    [Test, AutoData]
    public void UpdateEventSchedule_WhenGetScheduledStreamsAsyncThrowsException_ThrowsException(DateTimeOffset triggerTime)
    {
        // Arrange
        var exception = new Exception("Test message");
        _scheduleService
            .Setup(x => x.GetScheduleAsync(It.IsAny<DateTimeOffset>(), null))
            .ThrowsAsync(exception);

        // Act & Assert
        Assert.That(() => _controller.UpdateEventSchedule(triggerTime), Throws.Exception.EqualTo(exception));
    }
}
