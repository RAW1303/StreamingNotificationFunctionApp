using Moq.Protected;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Raw.Streaming.Discord.Tests.Services;

[TestFixture]
internal class DiscordMessageApiServiceTests : DiscordApiServiceTests
{
    private Mock<ILogger<DiscordMessageApiService>> _loggerMock;
    private DiscordMessageApiService _service;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<DiscordMessageApiService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new DiscordMessageApiService(httpClient, _loggerMock.Object);
    }

    [Test, AutoData]
    public async Task GetScheduledEventsAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEventsList(Message message)
    {
        // Arrange
        var channelId = "testChannelId";
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/DiscordMessage.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);

        // Act
        var result = await _service.SendDiscordMessageAsync(channelId, message);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsoluteUri.Contains($"/channels/{channelId}/messages")
                    && x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("Id").EqualTo("334385199974967042"));
        Assert.That(result, Has.Property("Content").EqualTo("Supa Hot"));
        Assert.That(result, Has.Property("Author").With.Property("Username").EqualTo("Mason"));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void GetScheduledEventsAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, Message message)
    {
        // Arrange
        var channelId = "testChannelId";
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.SendDiscordMessageAsync(channelId, message), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [Test, AutoData]
    public async Task CrosspostDiscordMessageAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEventsList(Message message)
    {
        // Arrange
        var channelId = "testChannelId";
        var messageId = "testMessageId";
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/DiscordMessage.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);

        // Act
        var result = await _service.CrosspostDiscordMessageAsync(channelId, messageId);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsoluteUri.Contains($"/channels/{channelId}/messages/{messageId}/crosspost")
                    && x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("Id").EqualTo("334385199974967042"));
        Assert.That(result, Has.Property("Content").EqualTo("Supa Hot"));
        Assert.That(result, Has.Property("Author").With.Property("Username").EqualTo("Mason"));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void CrosspostDiscordMessageAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, Message message)
    {
        // Arrange
        var channelId = "testChannelId";
        var messageId = "testMessageId";
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.CrosspostDiscordMessageAsync(channelId, messageId), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }
}
