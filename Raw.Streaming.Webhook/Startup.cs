using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Logging;
using Raw.Streaming.Webhook.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Webhook.Startup))]
[assembly: InternalsVisibleTo("Raw.Streaming.Webhook.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Raw.Streaming.Webhook;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApplicationName = AppSettings.WebSiteUrl,
            ApiKey = AppSettings.YoutubeApiKey,
        });

        var logger = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(ILogger<>));
        if (logger != null)
            builder.Services.Remove(logger);

        builder.Services.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(FunctionLogger<>), ServiceLifetime.Transient));
        builder.Services.AddHttpClient();
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddSingleton(youtubeService);
        builder.Services.AddSingleton<ITwitchTokenService, TwitchTokenService>();
        builder.Services.AddSingleton<ITwitchApiService, TwitchApiService>();
        builder.Services.AddSingleton<ITwitchSubscriptionService, TwitchSubscriptionService>();
        builder.Services.AddSingleton<IYoutubeScheduleService, YoutubeScheduleService>();
        builder.Services.AddSingleton<IYoutubeSubscriptionService, YoutubePubSubHubbubSubscriptionService>();
        builder.Services.AddSingleton<IScheduleService, ScheduleService>();
    }
}
