using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Webhook.Startup))]

namespace Raw.Streaming.Webhook
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ISubscriptionService, TwitchSubscriptionService>();
            builder.Services.AddSingleton<IDiscordNotificationService, DiscordNotificationService>();
            builder.Services.AddSingleton<ITwitchApiService, TwitchApiService>();
            builder.Services.AddSingleton<TwitchClipToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<TwitchStreamChangeToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<TwitchVideoToDiscordNotificationTranslator>();
        }
    }
}
