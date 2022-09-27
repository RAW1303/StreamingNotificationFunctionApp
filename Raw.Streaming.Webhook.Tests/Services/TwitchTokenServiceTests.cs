using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace Raw.Streaming.Webhook.Tests.Services
{
    [TestFixture]
    internal class TwitchTokenServiceTests
    {
        private Mock<ILogger<TwitchTokenService>> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private TwitchTokenService _service;

        private readonly string _twitchClientId = "clientId";
        private readonly string _twitchClientSecret = "clientSecret";
        private readonly string _twitchTokenUrl = "https://test.com";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Environment.SetEnvironmentVariable("TwitchClientId", _twitchClientId);
            Environment.SetEnvironmentVariable("TwitchClientSecret", _twitchClientSecret);
            Environment.SetEnvironmentVariable("TwitchTokenUrl", _twitchTokenUrl);
        }

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<TwitchTokenService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _service = new TwitchTokenService(_mockLogger.Object, httpClient);
        }

        [Test, AutoData]
        public async Task GetTwitchTokenAsync_WhenHttpClientReturnsToken_ReturnsSuccessfully(TokenResponse tokenResponse, string scope)
        {
            // Arrange
            SetupMockHttpMessageHandler(HttpStatusCode.OK, tokenResponse);

            // Act
            var result = await _service.GetTwitchTokenAsync(scope);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(tokenResponse.AccessToken));
            _mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post
                  && req.RequestUri.AbsoluteUri == $"{_twitchTokenUrl}/?grant_type=client_credentials&scope={scope}&client_id={_twitchClientId}&client_secret={_twitchClientSecret}"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }


        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Forbidden)]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public void GetTwitchTokenAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
        {
            // Arrange
            var scope = "TestScope";
            var errorMessage = $"Test Error Message {statusCode}";
            SetupMockHttpMessageHandler(statusCode, errorMessage);

            // Act and Assert
            Assert.That(async () => await _service.GetTwitchTokenAsync(scope), Throws.InstanceOf<TwitchApiException>().With.Property("Message").Contains(errorMessage));
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
    }
}
