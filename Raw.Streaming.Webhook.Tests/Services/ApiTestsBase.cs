using Moq.Protected;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Text.Json;

namespace Raw.Streaming.Webhook.Tests.Services;
internal abstract class ApiTestsBase
{
    protected Mock<HttpMessageHandler> _mockHttpMessageHandler;

    protected void SetupMockHttpMessageHandler(HttpStatusCode statusCode)
    {
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = statusCode
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(mockResponse);
    }

    protected void SetupMockHttpMessageHandler<T>(HttpStatusCode statusCode, T content)
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

    protected void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string jsonContent)
    {
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
