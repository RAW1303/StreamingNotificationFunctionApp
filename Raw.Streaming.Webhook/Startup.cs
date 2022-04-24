using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Webhook.Services;
using System.Runtime.CompilerServices;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Webhook.Startup))]
[assembly: InternalsVisibleTo("Raw.Streaming.Webhook.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Raw.Streaming.Webhook
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSingleton<ITwitchTokenService, TwitchTokenService>();
            builder.Services.AddSingleton<ITwitchApiService, TwitchApiService>();
            builder.Services.AddSingleton<ITwitchSubscriptionService, TwitchSubscriptionService>();
            builder.Services.AddSingleton<IYoutubeSubscriptionService, YoutubeSubscriptionService>();
        }
    }
}
