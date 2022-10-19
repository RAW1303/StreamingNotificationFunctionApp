using Moq;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Discord.Tests.Services;

[TestFixture]
internal class DiscordMessageServiceTests
{
    private Mock<IDiscordApiService> _mockDiscordApiService;
    private Mock<ILogger<DiscordMessageService>> _loggerMock;
    private DiscordMessageService _service;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<DiscordMessageService>>();
        _mockDiscordApiService = new Mock<IDiscordApiService>();
        _service = new DiscordMessageService(_mockDiscordApiService.Object, _loggerMock.Object);
    }

    [Test, AutoData]
    public async Task SendDiscordMessageAsync_WhenApiServiceReturnsSuccessfully_ReturnsMessage(Message message)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<Message>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ReturnsAsync(message);

        //Act
        var result = await _service.SendDiscordMessageAsync("test", new Message());

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(message));
    }

    [Test, AutoData]
    public void SendDiscordMessageAsync_WhenApiServiceThrowsException_Throws(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<Message>(It.IsAny<string>(), It.IsAny<DiscordApiContent>()))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.SendDiscordMessageAsync("test", new Message()), Throws.Exception.EqualTo(exception));
    }

    [Test, AutoData]
    public async Task CrosspostDiscordMessageAsync_WhenApiServiceReturnsSuccessfully_ReturnsMessage(Message message)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<Message>(It.IsAny<string>(), null))
            .ReturnsAsync(message);

        //Act
        var result = await _service.CrosspostDiscordMessageAsync("test", "testId");

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(message));
    }

    [Test, AutoData]
    public void CrosspostDiscordMessageAsync_WhenApiServiceThrowsException_Throws(DiscordApiException exception)
    {
        //Arrange
        _mockDiscordApiService
            .Setup(x => x.SendDiscordApiPostRequestAsync<Message>(It.IsAny<string>(), null))
            .ThrowsAsync(exception);

        //Act and Assert
        Assert.That(async () => await _service.CrosspostDiscordMessageAsync("test", "testId"), Throws.Exception.EqualTo(exception));
    }
}
