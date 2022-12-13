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
internal class DiscordEventApiServiceTests : DiscordApiServiceTests
{
    private Mock<ILogger<DiscordEventApiService>> _loggerMock;
    private DiscordEventApiService _service;

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();
    }

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<DiscordEventApiService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new DiscordEventApiService(httpClient, _loggerMock.Object);
    }

    [Test]
    public async Task GetScheduledEventsAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEventsList()
    {
        // Arrange
        var guildId = "testGuildId";
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/ScheduledEventsList.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);

        // Act
        var result = await _service.GetScheduledEventsAsync(guildId);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync", 
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => 
                    x.RequestUri.AbsoluteUri.Contains($"guilds/{guildId}/scheduled-events")
                    && x.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            );

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
    public void GetScheduledEventsAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.GetScheduledEventsAsync("testId"), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [Test, AutoData]
    public async Task CreateScheduledEventAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEvent(GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var guildId = "testGuildId";
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/ScheduledEvent.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);

        // Act
        var result = await _service.CreateScheduledEventAsync(guildId, guildScheduledEvent);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => 
                    x.RequestUri.AbsoluteUri.Contains($"guilds/{guildId}/scheduled-events")
                    && x.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("Id").EqualTo("1026468392015769610"));
        Assert.That(result, Has.Property("Description").EqualTo("sdgdfgffdsgfdsg"));
        Assert.That(result, Has.Property("ScheduledStartTime").EqualTo(new DateTimeOffset(2022, 10, 03, 13, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.Property("ScheduledEndTime").EqualTo(new DateTimeOffset(2022, 10, 03, 15, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.Property("EntityMetadata").With.Property("Location").EqualTo("https://twitch.tv/royweller"));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void CreateScheduledEventAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.CreateScheduledEventAsync("testId", guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [Test, AutoData]
    public async Task UpdateScheduledEventAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEventsList(GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var guildId = "testGuildId";
        var eventId = "testEventId";
        var jsonContent = await File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}/TestData/ScheduledEvent.json");
        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonContent);

        // Act
        var result = await _service.UpdateScheduledEventAsync("testGuildId", "testEventId", guildScheduledEvent);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x => 
                    x.RequestUri.AbsoluteUri.Contains($"guilds/{guildId}/scheduled-events/{eventId}")
                    && x.Method == HttpMethod.Patch
                ),
                ItExpr.IsAny<CancellationToken>()
            );

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Property("Id").EqualTo("1026468392015769610"));
        Assert.That(result, Has.Property("Description").EqualTo("sdgdfgffdsgfdsg"));
        Assert.That(result, Has.Property("ScheduledStartTime").EqualTo(new DateTimeOffset(2022, 10, 03, 13, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.Property("ScheduledEndTime").EqualTo(new DateTimeOffset(2022, 10, 03, 15, 00, 00, 520, TimeSpan.FromHours(0))));
        Assert.That(result, Has.Property("EntityMetadata").With.Property("Location").EqualTo("https://twitch.tv/royweller"));
    }

    [InlineAutoData(HttpStatusCode.BadRequest)]
    [InlineAutoData(HttpStatusCode.Forbidden)]
    [InlineAutoData(HttpStatusCode.Unauthorized)]
    [InlineAutoData(HttpStatusCode.InternalServerError)]
    public void UpdateScheduledEventAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode, GuildScheduledEvent guildScheduledEvent)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.UpdateScheduledEventAsync("testGuildId", "testEventId", guildScheduledEvent), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }

    [Test]
    public async Task DeleteScheduledEventAsync_WhenHttpClientReturnsSuccessfully_ReturnsValidEventsList()
    {
        // Arrange
        var guildId = "testGuildId";
        var eventId = "testEventId";
        SetupMockHttpMessageHandler(HttpStatusCode.OK);

        // Act
        await _service.DeleteScheduledEventAsync(guildId, eventId);

        // Assert
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsoluteUri.Contains($"guilds/{guildId}/scheduled-events/{eventId}")
                    && x.Method == HttpMethod.Delete
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.Forbidden)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.InternalServerError)]
    public void DeleteScheduledEventAsync_WhenHttpClientReturnsNonSuccessStatusCode_ThrowsException(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = $"Test Error Message {statusCode}";
        SetupMockHttpMessageHandler(statusCode, errorMessage);

        // Act and Assert
        Assert.That(async () => await _service.DeleteScheduledEventAsync("testGuildId", "testEventId"), Throws.InstanceOf<DiscordApiException>().With.Property("Message").Contains(errorMessage));
    }
}
