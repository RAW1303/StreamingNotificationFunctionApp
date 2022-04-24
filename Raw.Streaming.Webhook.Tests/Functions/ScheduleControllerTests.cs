using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Tests.Functions
{
    [TestFixture]
    internal class ScheduleControllerTests
    {
        private Mock<ITwitchApiService> _twitchApiServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<ScheduleController>> _loggerMock;
        private ScheduleController _controller;

        [SetUp]
        public void Setup()
        {
            _twitchApiServiceMock = new Mock<ITwitchApiService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ScheduleController>>();
            _controller = new ScheduleController(_twitchApiServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test, AutoData]
        public async Task NotifyDailySchedule_WhenGetScheduledStreamsAsyncReturnsEmptyList_ReturnsNull(DateTime triggerTime)
        {
            // Arrange
            _twitchApiServiceMock
                .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TwitchSchedule());
            _mapperMock
                .Setup(x => x.Map<Event>(It.IsAny<StreamEvent>()))
                .Returns(new Event());

            // Act
            var result = await _controller.NotifyDailySchedule(triggerTime);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test, AutoData]
        public async Task NotifyDailySchedule_WhenGetScheduledStreamsAsyncReturnsItems_ReturnsServiceBusMessage(DateTime triggerTime, TwitchScheduleSegment segment)
        {
            // Arrange
            segment.StartTime = triggerTime;
            _twitchApiServiceMock
                .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TwitchSchedule{ Segments = new List<TwitchScheduleSegment>() { segment } });
            _mapperMock
                .Setup(x => x.Map<Event>(It.IsAny<StreamEvent>()))
                .Returns(new Event());

            // Act
            var result = await _controller.NotifyDailySchedule(triggerTime);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, AutoData]
        public void NotifyDailySchedule_WhenGetScheduledStreamsAsyncThrowsException_ThrowsException(DateTime triggerTime)
        {
            // Arrange
            var exception = new Exception("Test message");
            _twitchApiServiceMock
                .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ThrowsAsync(exception);
            _mapperMock
                .Setup(x => x.Map<Event>(It.IsAny<StreamEvent>()))
                .Returns(new Event());

            // Act & Assert
            Assert.That(() => _controller.NotifyDailySchedule(triggerTime), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public async Task NotifyWeeklySchedule_WhenGetScheduledStreamsAsyncWorks_ReturnsServiceBusMessage(DateTime triggerTime)
        {
            // Arrange
            _twitchApiServiceMock
                .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new TwitchSchedule());
            _mapperMock
                .Setup(x => x.Map<Event>(It.IsAny<StreamEvent>()))
                .Returns(new Event());

            // Act
            var result = await _controller.NotifyWeeklySchedule(triggerTime);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test, AutoData]
        public void NotifyWeeklySchedule_WhenGetScheduledStreamsAsyncThrowsException_ThrowsException(DateTime triggerTime)
        {
            // Arrange
            var exception = new Exception("Test message");
            _twitchApiServiceMock
                .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ThrowsAsync(exception);
            _mapperMock
                .Setup(x => x.Map<Event>(It.IsAny<StreamEvent>()))
                .Returns(new Event());

            // Act & Assert
            Assert.That(() => _controller.NotifyWeeklySchedule(triggerTime), Throws.Exception.EqualTo(exception));
        }


    }
}
