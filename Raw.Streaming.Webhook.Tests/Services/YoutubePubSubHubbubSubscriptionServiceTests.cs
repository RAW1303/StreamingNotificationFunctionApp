using Microsoft.AspNetCore.Http;
using Moq.Protected;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Raw.Streaming.Webhook.Tests.Services;

[TestFixture]
internal class YoutubePubSubHubbubSubscriptionServiceTests : ApiTestsBase
{
    private Mock<ILogger<YoutubePubSubHubbubSubscriptionService>> _loggerMock;
    private YoutubePubSubHubbubSubscriptionService _service;

    private readonly string _youtubeSubscriptionUrl = "https://test.com/";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("YoutubeSubscriptionUrl", _youtubeSubscriptionUrl);
    }

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<YoutubePubSubHubbubSubscriptionService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new YoutubePubSubHubbubSubscriptionService(_loggerMock.Object, httpClient);
    }

    [Test]
    public void SubscribeAsync_WhenOKStatusCode_CallsHttpClientSendAsync()
    {
        //Arrange
        SetupMockHttpMessageHandler(HttpStatusCode.OK);

        //Act and Assert
        Assert.DoesNotThrowAsync(async () => await _service.SubscribeAsync("https://topic.com", "https://callback.com"));
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri.AbsoluteUri == _youtubeSubscriptionUrl
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.InternalServerError)]
    public void SubscribeAsync_WhenNotOKStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        //Arrange
        SetupMockHttpMessageHandler(statusCode);

        //Act and Assert
        Assert.That(async () => await _service.SubscribeAsync("https://topic.com", "https://callback.com"), Throws.InstanceOf<YouTubeApiException>());
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri.AbsoluteUri == _youtubeSubscriptionUrl
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public void UnsubscribeAsync_WhenOKStatusCode_CallsHttpClientSendAsync()
    {
        //Arrange
        SetupMockHttpMessageHandler(HttpStatusCode.OK);

        //Act and Assert
        Assert.DoesNotThrowAsync(async () => await _service.UnsubscribeAsync("https://topic.com", "https://callback.com"));
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri.AbsoluteUri == _youtubeSubscriptionUrl
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.InternalServerError)]
    public void UnsubscribeAsync_WhenNotOKStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        //Arrange
        SetupMockHttpMessageHandler(statusCode);

        //Act and Assert
        Assert.That(async () => await _service.UnsubscribeAsync("https://topic.com", "https://callback.com"), Throws.InstanceOf<YouTubeApiException>());
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri.AbsoluteUri == _youtubeSubscriptionUrl
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public void ProcessRequest_WhenValidStream_ReturnsYoutubeFeed()
    {
        //Arrange
        var videoFeedStream = new FileStream("TestData/YouTubePushRequest/YouTubeVideoFeed.xml", FileMode.Open);

        //Act and Assert
        var result = _service.ProcessRequest(videoFeedStream);

        //Assert
        Assert.That(result.VideoId, Is.EqualTo("VIDEO_ID"));
        Assert.That(result.ChannelId, Is.EqualTo("CHANNEL_ID"));
        Assert.That(result.Title, Is.EqualTo("Video title"));
        Assert.That(result.Link, Is.EqualTo("http://www.youtube.com/watch?v=VIDEO_ID"));
        Assert.That(result.Published, Is.EqualTo(DateTimeOffset.Parse("2022-11-19T21:40:57+00:00")));
        Assert.That(result.Updated, Is.EqualTo(DateTimeOffset.Parse("2022-11-19T21:45:57+00:00")));
    }
}
