using Raw.Streaming.Webhook.Functions;

namespace Raw.Streaming.Webhook.Tests.Functions
{
    [TestFixture]
    internal class TwitchClipControllerTests
    {
        private Mock<ITwitchApiService> _twitchApiService;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<TwitchClipController>> _loggerMock;
        private TwitchClipController _controller;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _twitchApiService = new Mock<ITwitchApiService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TwitchClipController>>();
            _controller = new TwitchClipController(_twitchApiService.Object, _mapperMock.Object, _loggerMock.Object);
            _fixture = new Fixture();
        }

        [Test, AutoData]
        public async Task NotifyTwitchClips_WhenTwitchApiServiceReturnsClips_ReturnsServiceBusMessage(DateTimeOffset last, DateTimeOffset next)
        {
            // Arrange
            var gameId = "testGameId";
            var clip = _fixture.Build<TwitchClip>().With(x => x.GameId, gameId).Create();
            var game = _fixture.Build<TwitchGame>().With(x => x.Id, gameId).Create();

            _twitchApiService
                .Setup(x => x.GetClipsByBroadcasterAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<TwitchClip> { clip });
            _twitchApiService
                .Setup(x => x.GetGamesAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new List<TwitchGame> { game });
            _mapperMock
                .Setup(x => x.Map<Clip>(It.IsAny<TwitchClip>()))
                .Returns(new Clip());

            // Act
            var result = await _controller.NotifyTwitchClips(last, next);

            // Assert test
            Assert.That(result, Is.Not.Null);
        }

        [Test, AutoData]
        public async Task NotifyTwitchClips_WhenTwitchApiServiceReturnsEmptyList_ReturnsNull(DateTimeOffset last, DateTimeOffset next)
        {
            // Arrange
            _twitchApiService
                .Setup(x => x.GetClipsByBroadcasterAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<TwitchClip>());
            _twitchApiService
                .Setup(x => x.GetGamesAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new List<TwitchGame>());
            _mapperMock
                .Setup(x => x.Map<Clip>(It.IsAny<TwitchClip>()))
                .Returns(new Clip());

            // Act
            var result = await _controller.NotifyTwitchClips(last, next);

            // Assert test
            Assert.That(result, Is.Null);
        }

        [TestCase("2022-04-19T22:00", "2022-04-19T22:05", "2022-04-19T22:00")]
        [TestCase("2022-04-19T22:00", "2022-04-19T23:00", "2022-04-19T22:50")]
        public async Task NotifyTwitchClips_WhenTwitchApiServiceSuccessful_CallsGetClipsByBroadcasterAsyncWithCorrectDateTimes(string lastString, string nextString, string expectedString)
        {
            // Arrange
            var last = DateTimeOffset.Parse(lastString);
            var next = DateTimeOffset.Parse(nextString);
            var expected = DateTimeOffset.Parse(expectedString);
            _twitchApiService
                .Setup(x => x.GetClipsByBroadcasterAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<TwitchClip>());
            _twitchApiService
                .Setup(x => x.GetGamesAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new List<TwitchGame>());
            _mapperMock
                .Setup(x => x.Map<Clip>(It.IsAny<TwitchClip>()))
                .Returns(new Clip());

            // Act
            var result = await _controller.NotifyTwitchClips(last, next);

            // Assert
            _twitchApiService.Verify(mock => mock.GetClipsByBroadcasterAsync(It.IsAny<string>(), expected, next));
        }

        [Test, AutoData]
        public void NotifyTwitchClips_WhenGetClipsByBroadcasterAsyncThrowsException_ThrowsException(DateTimeOffset last, DateTimeOffset next)
        {
            // Arrange
            var exception = new Exception("Test message");
            _twitchApiService
                .Setup(x => x.GetClipsByBroadcasterAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .ThrowsAsync(exception);
            _twitchApiService
                .Setup(x => x.GetGamesAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new List<TwitchGame>());
            _mapperMock
                .Setup(x => x.Map<Clip>(It.IsAny<TwitchClip>()))
                .Returns(new Clip());

            // Act & Assert
            Assert.That(() => _controller.NotifyTwitchClips(last, next), Throws.Exception.EqualTo(exception));
        }

        [Test, AutoData]
        public void NotifyTwitchClips_WhenGetGamesAsyncThrowsException_ThrowsException(DateTimeOffset last, DateTimeOffset next)
        {
            // Arrange
            var exception = new Exception("Test message");
            _twitchApiService
                .Setup(x => x.GetClipsByBroadcasterAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<TwitchClip>());
            _twitchApiService
                .Setup(x => x.GetGamesAsync(It.IsAny<string[]>()))
                .ThrowsAsync(exception);
            _mapperMock
                .Setup(x => x.Map<Clip>(It.IsAny<TwitchClip>()))
                .Returns(new Clip());

            // Act & Assert
            Assert.That(() => _controller.NotifyTwitchClips(last, next), Throws.Exception.EqualTo(exception));
        }
    }
}
