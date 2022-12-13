using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Logging;
using Raw.Streaming.Discord.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Discord.Startup))]
[assembly: InternalsVisibleTo("Raw.Streaming.Discord.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Raw.Streaming.Discord;

[ExcludeFromCodeCoverage]
internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var logger = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(ILogger<>));
        if (logger != null)
            builder.Services.Remove(logger);

        builder.Services.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(FunctionLogger<>), ServiceLifetime.Transient));
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IDiscordEventService, DiscordEventApiService>();
        builder.Services.AddSingleton<IDiscordMessageService, DiscordMessageApiService>();
    }
}