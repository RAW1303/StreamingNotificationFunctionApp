using Moq.Protected;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Raw.Streaming.Webhook.Tests.Functions;

#nullable disable

[TestFixture]
internal class DiscordApiServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private Mock<ILogger<DiscordApiService>> _loggerMock;
    private DiscordApiService _service;

    private readonly string _discordApiUrl = "https://test.com";
    private readonly string _discordBotToken = "t3stt0k3n";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("DiscordBotToken", _discordBotToken);
        Environment.SetEnvironmentVariable("DiscordApiUrl", _discordApiUrl);
    }

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<DiscordApiService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new DiscordApiService(httpClient, _loggerMock.Object);
    }

    [Test]
    public async Task GetScheduledEvents_WhenApiReturnsSuccessfully_ReturnsValidEventsList()
    {
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/GetScheduledEventsResponse.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);
        var result = await _service.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>("testid");

        Assert.That(result, Has.One.Items);
        Assert.That(result, Has.One.With.Property("Id").EqualTo("1026468392015769610"));
        Assert.That(result, Has.One.With.Property("Description").EqualTo("sdgdfgffdsgfdsg"));
        Assert.That(result, Has.One.With.Property("ScheduledStartTime").EqualTo(new DateTimeOffset(2022, 10, 03, 13, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.One.With.Property("ScheduledEndTime").EqualTo(new DateTimeOffset(2022, 10, 03, 15, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.One.With.Property("EntityMetadata").With.Property("Location").EqualTo("https://twitch.tv/royweller"));
    }

    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.InternalServerError)]
    public void GetScheduledEvents_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.SendDiscordApiGetRequestAsync<GuildScheduledEvent>("testUrl"), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void CreateScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.SendDiscordApiPostRequestAsync<GuildScheduledEvent>("testUrl", guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void UpdateScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.SendDiscordApiPatchRequestAsync<GuildScheduledEvent>("testUrl", guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void DeleteScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.SendDiscordApiDeleteRequestAsync("testUrl"), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    private void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
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
