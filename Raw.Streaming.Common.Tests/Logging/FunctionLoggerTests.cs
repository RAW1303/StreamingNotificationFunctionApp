using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Logging;

namespace Raw.Streaming.Common.Tests.Logging;

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

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}