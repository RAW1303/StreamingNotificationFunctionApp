using Raw.Streaming.Webhook.Functions;

namespace Raw.Streaming.Webhook.Tests.Functions;

[TestFixture]
internal class TwitchHighlightsControllerTests
{
    private Mock<ITwitchApiService> _twitchApiService;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<TwitchHighlightsController>> _loggerMock;
    private TwitchHighlightsController _controller;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _twitchApiService = new Mock<ITwitchApiService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TwitchHighlightsController>>();
        _controller = new TwitchHighlightsController(_twitchApiService.Object, _mapperMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Test, AutoData]
    public async Task NotifyTwitchHighlights_WhenTwitchApiServiceReturnsVideos_ReturnsServiceBusMessage(DateTimeOffset startedAt)
    {
        // Arrange
        var video = _fixture.Build<TwitchVideo>()
            .With(x => x.PublishedAt, startedAt.AddHours(1))
            .With(x => x.Viewable, "public")
            .Create();

        _twitchApiService
            .Setup(x => x.GetHighlightsByBroadcasterAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<TwitchVideo> { video });
        _mapperMock
            .Setup(x => x.Map<Video>(It.IsAny<TwitchVideo>()))
            .Returns(_fixture.Create<Video>());

        // Act
        var result = await _controller.NotifyTwitchHighlights(startedAt);

        // Assert test
        Assert.That(result, Is.Not.Null);
    }

    [Test, AutoData]
    public async Task NotifyTwitchHighlights_WhenTwitchApiServiceReturnsNoVideos_ReturnsNull(DateTimeOffset startedAt)
    {
        // Arrange
        _twitchApiService
            .Setup(x => x.GetHighlightsByBroadcasterAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<TwitchVideo>());
        _mapperMock
            .Setup(x => x.Map<Video>(It.IsAny<TwitchVideo>()))
            .Returns(_fixture.Create<Video>());

        // Act
        var result = await _controller.NotifyTwitchHighlights(startedAt);

        // Assert test
        Assert.That(result, Is.Null);
    }


    [Test, AutoData]
    public async Task NotifyTwitchHighlights_WhenTwitchApiServiceReturnsOldVideos_ReturnsServiceBusMessage(DateTimeOffset startedAt)
    {
        // Arrange
        var video = _fixture.Build<TwitchVideo>()
            .With(x => x.PublishedAt, startedAt.AddHours(-1))
            .With(x => x.Viewable, "public")
            .Create();

        _twitchApiService
            .Setup(x => x.GetHighlightsByBroadcasterAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<TwitchVideo> { video });
        _mapperMock
            .Setup(x => x.Map<Video>(It.IsAny<TwitchVideo>()))
            .Returns(_fixture.Create<Video>());

        // Act
        var result = await _controller.NotifyTwitchHighlights(startedAt);

        // Assert test
        Assert.That(result, Is.Null);
    }


    [Test, AutoData]
    public async Task NotifyTwitchHighlights_WhenTwitchApiServiceReturnsPrivateVideos_ReturnsServiceBusMessage(DateTimeOffset startedAt)
    {
        // Arrange
        var video = _fixture.Build<TwitchVideo>()
            .With(x => x.PublishedAt, startedAt.AddHours(1))
            .With(x => x.Viewable, "private")
            .Create();

        _twitchApiService
            .Setup(x => x.GetHighlightsByBroadcasterAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<TwitchVideo> { video });
        _mapperMock
            .Setup(x => x.Map<Video>(It.IsAny<TwitchVideo>()))
            .Returns(_fixture.Create<Video>());

        // Act
        var result = await _controller.NotifyTwitchHighlights(startedAt);

        // Assert test
        Assert.That(result, Is.Null);
    }

    [Test, AutoData]
    public void NotifyTwitchHighlights_WhenApiThrowsException_ThrowsException(DateTimeOffset startedAt)
    {
        // Arrange
        var exception = new Exception("Test message");
        _twitchApiService
            .Setup(x => x.GetHighlightsByBroadcasterAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);
        _mapperMock
            .Setup(x => x.Map<Video>(It.IsAny<TwitchVideo>()))
            .Returns(_fixture.Create<Video>());

        // Act & Assert
        Assert.That(() => _controller.NotifyTwitchHighlights(startedAt), Throws.Exception.EqualTo(exception));
    }
}
