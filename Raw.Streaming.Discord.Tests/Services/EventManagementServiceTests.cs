using Moq;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;
using System.IO;
using System.Linq;

namespace Raw.Streaming.Discord.Tests.Services;

[TestFixture]
internal class EventManagementServiceTests
{
    private Mock<ILogger<EventManagementService>> _loggerMock;
    private Mock<IDiscordEventService> _discordEventServiceMock;
    private EventManagementService _service;

    private Fixture _fixture;

    private readonly string _discordBotApplicationId = "discordBotApplicationId";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Environment.SetEnvironmentVariable("DiscordBotApplicationId", _discordBotApplicationId);
    }

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<EventManagementService>>();
        _discordEventServiceMock = new Mock<IDiscordEventService>();
        _service = new EventManagementService(_discordEventServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Test, AutoData]
    public void SyncScheduledEventsAsync_WhenGetScheduledEventsAsync_ThrowsException(Event[] events)
    {
        // Arrange
        var guildId = "testGuildId";

        var exception = new Exception("Test message");
        _discordEventServiceMock
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act and Assert
        Assert.That(async () => await _service.SyncScheduledEventsAsync(guildId, events), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task SyncScheduledEventsAsync_WhenOnlyNew_CallsCreateOnly(string[] locations)
    {
        // Arrange
        var guildId = "testGuildId";
        var input = locations.Select(
            x => _fixture.Build<Event>()
                .With(x => x.Url, x)
                .Create());

        var existing = Enumerable.Empty<GuildScheduledEvent>();

        var exception = new Exception("Test message");

        _discordEventServiceMock
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(existing);

        // Act
        await _service.SyncScheduledEventsAsync(guildId, input);

        // Assert
        _discordEventServiceMock.Verify(x => x.GetScheduledEventsAsync(guildId), Times.Once);
        _discordEventServiceMock.Verify(x => x.UpdateScheduledEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GuildScheduledEvent>()), Times.Never);
        _discordEventServiceMock.Verify(x => x.DeleteScheduledEventAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        Assert.Multiple(() =>
        {
            foreach (var newEvent in input)
            {
                _discordEventServiceMock.Verify(x => x.CreateScheduledEventAsync(guildId, It.Is<GuildScheduledEvent>(y => y.EntityMetadata.Location == newEvent.Url)), Times.Once);
            }
        });
    }

    [Test, AutoData]
    public async Task SyncScheduledEventsAsync_WhenOnlyExisting_CallsCreateOnly(string[] locations)
    {
        // Arrange
        var guildId = "testGuildId";
        var input = locations.Select(
            x => _fixture.Build<Event>()
                .With(x => x.Url, x)
                .With(x => x.Title, "input")
                .Create()).ToList();

        var existing = locations.Select(
            x => _fixture.Build<GuildScheduledEvent>()
                .With(x => x.EntityMetadata, new GuildScheduledEventEntityMetadata { Location = x })
                .With(x => x.Name, "existing")
                .With(x => x.CreatorId, _discordBotApplicationId)
                .Create()).ToList();

        _discordEventServiceMock
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(existing);

        // Act
        await _service.SyncScheduledEventsAsync(guildId, input);

        // Assert
        _discordEventServiceMock.Verify(x => x.GetScheduledEventsAsync(guildId), Times.Once);
        _discordEventServiceMock.Verify(x => x.CreateScheduledEventAsync(It.IsAny<string>(), It.IsAny<GuildScheduledEvent>()), Times.Never);
        _discordEventServiceMock.Verify(x => x.DeleteScheduledEventAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        Assert.Multiple(() =>
        {
            foreach (var existingEvent in existing)
            {
                _discordEventServiceMock.Verify(x => x.UpdateScheduledEventAsync(guildId, existingEvent.Id, It.Is<GuildScheduledEvent>(y => y.Name == "input" && y.EntityMetadata.Location == existingEvent.EntityMetadata.Location)), Times.Once);
            }
        });
    }

    [Test, AutoData]
    public async Task SyncScheduledEventsAsync_WhenExistingButNoInput_CallsDeleteOnly(string[] locations)
    {
        // Arrange
        var guildId = "testGuildId";
        var input = Enumerable.Empty<Event>();

        var existing = locations.Select(
            x => _fixture.Build<GuildScheduledEvent>()
                .With(x => x.EntityMetadata, new GuildScheduledEventEntityMetadata { Location = x })
                .With(x => x.Name, "existing")
                .With(x => x.CreatorId, _discordBotApplicationId)
                .Create()).ToList();

        _discordEventServiceMock
            .Setup(x => x.GetScheduledEventsAsync(It.IsAny<string>()))
            .ReturnsAsync(existing);

        // Act
        await _service.SyncScheduledEventsAsync(guildId, input);

        // Assert
        _discordEventServiceMock.Verify(x => x.GetScheduledEventsAsync(guildId), Times.Once);
        _discordEventServiceMock.Verify(x => x.CreateScheduledEventAsync(It.IsAny<string>(), It.IsAny<GuildScheduledEvent>()), Times.Never);
        _discordEventServiceMock.Verify(x => x.UpdateScheduledEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GuildScheduledEvent>()), Times.Never);

        Assert.Multiple(() =>
        {
            foreach (var existingEvent in existing)
            {
                _discordEventServiceMock.Verify(x => x.DeleteScheduledEventAsync(guildId, existingEvent.Id), Times.Once);
            }
        });
    }
}
