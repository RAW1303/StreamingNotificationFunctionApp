using Video = Google.Apis.YouTube.v3.Data.Video;

namespace Raw.Streaming.Webhook.Tests.Services;

[TestFixture]
internal class ScheduleServiceTests
{
    private Mock<ITwitchApiService> _twitchApiServiceMock;
    private Mock<IYoutubeScheduleService> _youtubeScheduleServiceMock;
    private Mock<IMapper> _mapperMock;
    private ScheduleService _service;

    [SetUp]
    public void SetUp()
    {
        _twitchApiServiceMock = new Mock<ITwitchApiService>();
        _youtubeScheduleServiceMock = new Mock<IYoutubeScheduleService>();
        _mapperMock = new Mock<IMapper>();
        _service = new ScheduleService(_twitchApiServiceMock.Object, _youtubeScheduleServiceMock.Object, _mapperMock.Object);
    }

    [Test, AutoData]
    public async Task GetScheduleAsync_WhenAllSourcesReturnValues_ReturnsEventList(DateTimeOffset from, DateTimeOffset to, TwitchSchedule twitchSchedule, IEnumerable<Video> youtubeVideos, IEnumerable<Event> twitchEvents, IEnumerable<Event> youtubeEvents)
    {
        //Arrange
        _twitchApiServiceMock
            .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(twitchSchedule);
        _youtubeScheduleServiceMock
            .Setup(x => x.GetUpcomingBroadcastsAsync())
            .ReturnsAsync(youtubeVideos);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<Event>>(It.IsAny<TwitchSchedule>()))
            .Returns(twitchEvents);
        _mapperMock
            .Setup(x => x.Map<IEnumerable<Event>>(It.IsAny<IEnumerable<Video>>()))
            .Returns(youtubeEvents);

        //Act
        var result = await _service.GetScheduleAsync(from, to);

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.All.With.Property("Start").GreaterThan(from));
        Assert.That(result, Has.All.With.Property("Start").LessThan(to));
    }

    [Test, AutoData]
    public void GetScheduleAsync_WhenTwitchApiServiceThrowsException_ThrowsException(DateTimeOffset from, DateTimeOffset to)
    {
        //Arrange
        _twitchApiServiceMock
            .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            .ThrowsAsync(new TwitchApiException("Test"));

        //Act & Assert
        Assert.That(async () => await _service.GetScheduleAsync(from, to), Throws.InstanceOf<TwitchApiException>());
    }

    [Test, AutoData]
    public void GetScheduleAsync_WhenYoutubeScheduleServiceThrowsException_ThrowsException(DateTimeOffset from, DateTimeOffset to)
    {
        //Arrange
        _twitchApiServiceMock
            .Setup(x => x.GetScheduleByBroadcasterIdAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
            .ThrowsAsync(new Exception("Test"));

        //Act & Assert
        Assert.That(async () => await _service.GetScheduleAsync(from, to), Throws.InstanceOf<Exception>());
    }
}
