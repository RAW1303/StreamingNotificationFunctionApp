using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Logging;

namespace Raw.Streaming.Common.Tests.Logging;

[TestFixture]
public class FunctionLoggerTests
{
    private Mock<ILoggerFactory> _mockILoggerFactory;
    private Mock<ILogger> _mockILogger;
    private FunctionLogger<string> _logger;

    [SetUp]
    public void Setup()
    {
        _mockILogger = new Mock<ILogger>();
        _mockILoggerFactory = new Mock<ILoggerFactory>();
        _mockILoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockILogger.Object);
        _logger = new FunctionLogger<string>(_mockILoggerFactory.Object);
    }

    [Test, AutoData]
    public void BeginScope_WhenCalled_ReturnsCorrectValue(string state)
    {
        //Arrange
        var _mockDisposable = new Mock<IDisposable>();
        _mockILogger.Setup(x => x.BeginScope(It.IsAny<string>())).Returns(_mockDisposable.Object);

        //Act
        var result = _logger.BeginScope(state);

        //Assert
        Assert.That(result, Is.EqualTo(_mockDisposable.Object));
    }

    [Test, AutoData]
    public void IsEnabled_WhenCalled_ReturnsCorrectValue(LogLevel logLevel, bool isEnabled)
    {
        //Arrange
        _mockILogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(!isEnabled);
        _mockILogger.Setup(x => x.IsEnabled(logLevel)).Returns(isEnabled);

        //Act
        var result = _logger.IsEnabled(logLevel);

        //Assert
        Assert.That(result, Is.EqualTo(isEnabled));
    }
}