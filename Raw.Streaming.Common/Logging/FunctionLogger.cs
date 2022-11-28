using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace Raw.Streaming.Common.Logging;
public class FunctionLogger<T> : ILogger<T>
{
    readonly ILogger logger;

    public FunctionLogger(ILoggerFactory factory)
    {
        logger = factory.CreateLogger(LogCategories.CreateFunctionUserCategory(typeof(T).FullName));
    }

    public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);
    public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => logger.Log(logLevel, eventId, state, exception, formatter);
}
