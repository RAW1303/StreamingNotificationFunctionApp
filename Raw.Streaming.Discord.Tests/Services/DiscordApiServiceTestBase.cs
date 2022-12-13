using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Raw.Streaming.Discord.Tests.Services;

internal class DiscordApiServiceTestBase
{
    protected Mock<HttpMessageHandler> _mockHttpMessageHandler;

    private readonly string _discordApiUrl = "https://test.com";
    private readonly string _discordBotToken = "t3stt0k3n";

    public virtual void OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("DiscordBotToken", _discordBotToken);
        Environment.SetEnvironmentVariable("DiscordApiUrl", _discordApiUrl);
    }

    protected void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string content = null)
    {
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = statusCode
        };

        if (content is not null)
            mockResponse.Content = new StringContent(content);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);
    }
}