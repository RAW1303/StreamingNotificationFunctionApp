using Raw.Streaming.Discord.Functions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Webhook.Tests.Functions
{
    [TestFixture]
    internal class ServiceBusFunctionsTests
    {
        private Mock<IDiscordBotMessageService> _discordBotMessageService;
        private Mock<ILogger<ServiceBusFunctions>> _loggerMock;
        private ServiceBusFunctions _controller;

        [SetUp]
        public void Setup()
        {
            _discordBotMessageService = new Mock<IDiscordBotMessageService>();
            _loggerMock = new Mock<ILogger<ServiceBusFunctions>>();
            _controller = new ServiceBusFunctions(_discordBotMessageService.Object, _loggerMock.Object);
        }

        [Test, AutoData]
        public void ProcessGoLiveMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(GoLive goLive)
        {
            // Arrange
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ReturnsAsync(new Message());
            var queueMessage = new DiscordBotQueueItem<GoLive>(goLive);

            // Act and Assert
            Assert.That(() => _controller.ProcessGoLiveMessageQueue(queueMessage), Throws.Nothing);
        }

        [Test, AutoData]
        public void ProcessGoLiveMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(GoLive goLive)
        {
            // Arrange
            var exception = new Exception("Test message");
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ThrowsAsync(exception);
            var queueMessage = new DiscordBotQueueItem<GoLive>(goLive);

            // Act and Assert
            Assert.That(() => _controller.ProcessGoLiveMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public void ProcessClipMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Clip[] clips)
        {
            // Arrange
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ReturnsAsync(new Message());
            var queueMessage = new DiscordBotQueueItem<Clip>(clips);

            // Act and Assert
            Assert.That(() => _controller.ProcessClipMessageQueue(queueMessage), Throws.Nothing);
        }

        [Test, AutoData]
        public void ProcessClipMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Clip[] clips)
        {
            // Arrange
            var exception = new Exception("Test message");
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ThrowsAsync(exception);
            var queueMessage = new DiscordBotQueueItem<Clip>(clips);

            // Act and Assert
            Assert.That(() => _controller.ProcessClipMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public void ProcessVideoMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Video[] videos)
        {
            // Arrange
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ReturnsAsync(new Message());
            var queueMessage = new DiscordBotQueueItem<Video>(videos);

            // Act and Assert
            Assert.That(() => _controller.ProcessVideoMessageQueue(queueMessage), Throws.Nothing);
        }

        [Test, AutoData]
        public void ProcessVideoMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Video[] videos)
        {
            // Arrange
            var exception = new Exception("Test message");
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ThrowsAsync(exception);
            var queueMessage = new DiscordBotQueueItem<Video>(videos);

            // Act and Assert
            Assert.That(() => _controller.ProcessVideoMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public void ProcessDailyScheduleMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Event[] events)
        {
            // Arrange
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ReturnsAsync(new Message());
            var queueMessage = new DiscordBotQueueItem<Event>(events);

            // Act and Assert
            Assert.That(() => _controller.ProcessDailyScheduleMessageQueue(queueMessage), Throws.Nothing);
        }

        [Test, AutoData]
        public void ProcessDailyScheduleMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Event[] events)
        {
            // Arrange
            var exception = new Exception("Test message");
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ThrowsAsync(exception);
            var queueMessage = new DiscordBotQueueItem<Event>(events);

            // Act and Assert
            Assert.That(() => _controller.ProcessDailyScheduleMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public void ProcessWeeklyScheduleMessageQueue_WhenSendDiscordMessageAsyncSucceeds_DoesNotThrowException(Event[] events)
        {
            // Arrange
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ReturnsAsync(new Message());
            var queueMessage = new DiscordBotQueueItem<Event>(events);

            // Act and Assert
            Assert.That(() => _controller.ProcessWeeklyScheduleMessageQueue(queueMessage), Throws.Nothing);
        }

        [Test, AutoData]
        public void ProcessWeeklyScheduleMessageQueue_WhenSendDiscordMessageAsyncThrowsException_ThrowsException(Event[] events)
        {
            // Arrange
            var exception = new Exception("Test message");
            _discordBotMessageService
                .Setup(x => x.SendDiscordMessageAsync(It.IsAny<string>(), It.IsAny<Message>()))
                .ThrowsAsync(exception);
            var queueMessage = new DiscordBotQueueItem<Event>(events);

            // Act and Assert
            Assert.That(() => _controller.ProcessWeeklyScheduleMessageQueue(queueMessage), Throws.Exception.EqualTo(exception));
        }
    }
}
