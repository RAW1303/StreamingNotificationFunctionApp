using Moq.Protected;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Tests.Functions;

[TestFixture]
internal class DiscordBotEventServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private Mock<ILogger<DiscordEventService>> _loggerMock;
    private DiscordEventService _service;

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
        _loggerMock = new Mock<ILogger<DiscordEventService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new DiscordEventService(_loggerMock.Object, httpClient);
    }

    [Test]
    public async Task GetScheduledEvents_WhenApiReturnsSuccessfully_ReturnsValidEventsList()
    {
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/GetScheduledEventsResponse.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);
        var result = await _service.GetScheduledEvents("testid");

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
        Assert.That(async () => await _service.GetScheduledEvents("testid"), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void CreateScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, string guildId, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.CreateScheduledEvent(guildId, guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void UpdateScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, string guildId, string eventId, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.UpdateScheduledEvent(guildId, eventId, guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void DeleteScheduledEvent_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, string guildId, string eventId)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.DeleteScheduledEvent(guildId, eventId), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
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
