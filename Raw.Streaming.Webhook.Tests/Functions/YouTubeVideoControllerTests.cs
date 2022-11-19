using Azure.Messaging.ServiceBus.Administration;
using Microsoft.AspNetCore.Http;
using Raw.Streaming.Webhook.Functions;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Raw.Streaming.Webhook.Tests.Functions;

[TestFixture]
internal class YouTubeVideoControllerTests
{
    private Mock<IYoutubeSubscriptionService> _youtubeSubscriptionService;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<YouTubeVideoController>> _loggerMock;
    private YouTubeVideoController _controller;
    private readonly Fixture _fixture = new();

    private const string CHANNEL_ID = "channel-id";
    private const string VIDEO_TOPIC = "video-topic";
    private const string WEBSITE_HOSTNAME = "website-hostname";

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("YoutubeChannelId", CHANNEL_ID);
        Environment.SetEnvironmentVariable("YoutubeVideoTopic", VIDEO_TOPIC);
        Environment.SetEnvironmentVariable("WEBSITE_HOSTNAME", WEBSITE_HOSTNAME);

        _youtubeSubscriptionService = new Mock<IYoutubeSubscriptionService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<YouTubeVideoController>>();
        _controller = new YouTubeVideoController(_youtubeSubscriptionService.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Test]
    public void YoutubeVideoSubscribe_SubscribeAsyncSuccessful_DoesNotThrowException()
    {
        // Act and Assert
        Assert.DoesNotThrowAsync(() => _controller.YoutubeVideoSubscribe());
        _youtubeSubscriptionService.Verify(x => x.SubscribeAsync($"{VIDEO_TOPIC}{CHANNEL_ID}", $"https://{WEBSITE_HOSTNAME}/api/webhook/youtube/video-update"));
    }

    [Test, AutoData]
    public void YoutubeVideoSubscribe_SubscribeAsyncThrowsException_ThrowsException(Exception exception)
    {
        //Arrange
        _youtubeSubscriptionService
            .Setup(x => x.SubscribeAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act and Assert
        Assert.That(() => _controller.YoutubeVideoSubscribe(), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public void YoutubeVideoWebhook(Video video)
    {
        //Arrange
        var videoFeedStream = new FileStream("TestData/YouTubePushRequest/YouTubeVideoFeed.xml", FileMode.Open); 
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Body).Returns(videoFeedStream);
        _mapperMock.Setup(x => x.Map<Video>(It.IsAny<YoutubeFeed>())).Returns(video);

        // Act
        var result = _controller.YoutubeVideoWebhook(mockRequest.Object);

        //Assert
        Assert.That(result, Is.Not.Null);
    }
}
