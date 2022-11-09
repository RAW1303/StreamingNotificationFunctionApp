using FluentAssertions;
using Moq.Protected;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace Raw.Streaming.Webhook.Tests.Services
{
    [TestFixture]
    internal class TwitchApiServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<ITwitchTokenService> _mockTwitchTokenService;
        private Mock<ILogger<TwitchApiService>> _loggerMock;
        private TwitchApiService _service;

        private readonly string _twitchApiUrl = "https://test.com";
        private readonly string _twitchApiChannelEndpoint ="channel";
        private readonly string _twitchApiClipEndpoint = "clip";
        private readonly string _twitchApiGameEndpoint = "game";
        private readonly string _twitchApiScheduleEndpoint = "schedule";
        private readonly string _twitchApiVideoEndpoint = "video";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Environment.SetEnvironmentVariable("TwitchApiUrl", _twitchApiUrl);
            Environment.SetEnvironmentVariable("TwitchApiChannelEndpoint", _twitchApiChannelEndpoint);
            Environment.SetEnvironmentVariable("TwitchApiClipEndpoint", _twitchApiClipEndpoint);
            Environment.SetEnvironmentVariable("TwitchApiGameEndpoint", _twitchApiGameEndpoint);
            Environment.SetEnvironmentVariable("TwitchApiScheduleEndpoint", _twitchApiScheduleEndpoint);
            Environment.SetEnvironmentVariable("TwitchApiVideoEndpoint", _twitchApiVideoEndpoint);
        }

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<TwitchApiService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockTwitchTokenService = new Mock<ITwitchTokenService>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _service = new TwitchApiService(_loggerMock.Object, _mockTwitchTokenService.Object, httpClient);
        }

        [Test, AutoData]
        public async Task GetChannelInfoAsync_WhenHttpClientReturnsChannelInfo_ReturnsSuccessfully(TwitchChannel channel)
        {
            // Arrange
            var channelId = "TestId";
            var content = new TwitchApiResponse<IList<TwitchChannel>> { Data = new List<TwitchChannel> { channel } };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetChannelInfoAsync(channelId);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(channel);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiChannelEndpoint}?broadcaster_id={channelId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetGamesAsync_WhenHttpClientReturnsGames_ReturnsSuccessfully(IList<TwitchGame> games)
        {
            // Arrange
            var gameId = "TestId";
            var content = new TwitchApiResponse<IList<TwitchGame>> { Data = games };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetGamesAsync(gameId);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(games);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiGameEndpoint}?id={gameId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetClipsByBroadcasterAsync_WhenHttpClientReturnsClips_ReturnsSuccessfully(IList<TwitchClip> clips)
        {
            // Arrange
            var channelId = "TestId";
            var content = new TwitchApiResponse<IList<TwitchClip>> { Data = clips };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetClipsByBroadcasterAsync(channelId);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(clips);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiClipEndpoint}?broadcaster_id={channelId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetClipsByBroadcasterAsync_WhenStartTimePassed_StartTimeAddedToUrl(IList<TwitchClip> clips)
        {
            // Arrange
            var channelId = "TestId";
            var startTimeString = "2022-04-23T23:11:45Z";
            var startTime = DateTimeOffset.Parse(startTimeString);
            var content = new TwitchApiResponse<IList<TwitchClip>> { Data = clips };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetClipsByBroadcasterAsync(channelId, startTime);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(clips);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == 
                    $"{_twitchApiUrl}/{_twitchApiClipEndpoint}?broadcaster_id={channelId}&started_at=2022-04-23T23%3a11%3a45Z"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetClipsByBroadcasterAsync_WhenStartAndEndTimePassed_StartAndEndTimeAddedToUrl(IList<TwitchClip> clips)
        {
            // Arrange
            var channelId = "TestId";
            var startTimeString = "2022-04-23T23:11:45Z";
            var startTime = DateTimeOffset.Parse(startTimeString);
            var endTimeString = "2022-04-24T02:16:45Z";
            var endTime = DateTimeOffset.Parse(endTimeString);
            var content = new TwitchApiResponse<IList<TwitchClip>> { Data = clips };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetClipsByBroadcasterAsync(channelId, startTime, endTime);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(clips);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == 
                    $"{_twitchApiUrl}/{_twitchApiClipEndpoint}?broadcaster_id={channelId}&started_at=2022-04-23T23%3a11%3a45Z&ended_at=2022-04-24T02%3a16%3a45Z"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetHighlightsByBroadcasterAsync_WhenHttpClientReturnsHighlights_ReturnsSuccessfully(IList<TwitchVideo> videos)
        {
            // Arrange
            var channelId = "TestId";
            var content = new TwitchApiResponse<IList<TwitchVideo>> { Data = videos };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetHighlightsByBroadcasterAsync(channelId);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(videos);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiVideoEndpoint}?type=highlight&user_id={channelId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task GetScheduleByBroadcasterIdAsync_WhenHttpClientReturnsSchedule_ReturnsSuccessfully()
        {
            // Arrange
            var channelId = "TestId";
            var content = File.ReadAllText("TestData/TwitchApiResponse/GetScheduleSuccess.json");
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetScheduleByBroadcasterIdAsync(channelId);

            // Assert
            Assert.That(result, Is.Not.Null);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiScheduleEndpoint}?broadcaster_id={channelId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetScheduleByBroadcasterIdAsync_WhenHttpClientReturnsSchedule_ReturnsSuccessfully(TwitchSchedule schedule)
        {
            // Arrange
            var channelId = "TestId";
            var content = new TwitchApiResponse<TwitchSchedule> { Data = schedule };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetScheduleByBroadcasterIdAsync(channelId);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(schedule);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri == $"{_twitchApiUrl}/{_twitchApiScheduleEndpoint}?broadcaster_id={channelId}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test, AutoData]
        public async Task GetScheduleByBroadcasterIdAsync_WhenStartTimePassed_StartTimeAddedToUrl(TwitchSchedule schedule)
        {
            // Arrange
            var channelId = "TestId";
            var startTimeString = "2022-04-23T23:11:45Z";
            var startTime = DateTimeOffset.Parse(startTimeString);
            var content = new TwitchApiResponse<TwitchSchedule> { Data = schedule };
            SetupMockHttpMessageHandler(HttpStatusCode.OK, content);

            // Act
            var result = await _service.GetScheduleByBroadcasterIdAsync(channelId, startTime);

            // Assert
            Assert.That(result, Is.Not.Null);
            result.Should().BeEquivalentTo(schedule);
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
                  && req.RequestUri.AbsoluteUri ==
                    $"{_twitchApiUrl}/{_twitchApiScheduleEndpoint}?broadcaster_id={channelId}&start_time=2022-04-23T23%3a11%3a45Z"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetChannelInfoAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var channelId = "TestId";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetChannelInfoAsync(channelId), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
        }

        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetGamesAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var gameId = "TestId";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetGamesAsync(gameId), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
        }

        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetClipsByBroadcasterAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var channelId = "TestId";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetClipsByBroadcasterAsync(channelId), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
        }

        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetHighlightsByBroadcasterAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var channelId = "TestId";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetHighlightsByBroadcasterAsync(channelId), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
        }

        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetScheduleByBroadcasterIdAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var channelId = "TestId";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetScheduleByBroadcasterIdAsync(channelId), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
        }

        private void SetupMockHttpMessageHandler<T>(HttpStatusCode statusCode, T content)
        {
            var jsonContent = JsonSerializer.Serialize(content);

            var mockResponse = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(mockResponse);
        }

        private void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string jsonContent)
        {
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(jsonContent)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(mockResponse);
        }
    }
}
