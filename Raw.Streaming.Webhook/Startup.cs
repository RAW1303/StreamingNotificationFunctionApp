using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Webhook.Startup))]

namespace Raw.Streaming.Webhook
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            var googleCalendarService = new CalendarService(new BaseClientService.Initializer()
            {
                ApiKey = AppSettings.GoogleCalendarApiKey
            });

            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(googleCalendarService);
            builder.Services.AddSingleton<IDiscordNotificationService, DiscordNotificationService>();
            builder.Services.AddSingleton<ITwitchApiService, TwitchApiService>();
            builder.Services.AddSingleton<ITwitchSubscriptionService, TwitchSubscriptionService>();
            builder.Services.AddSingleton<IYoutubeSubscriptionService, YoutubeSubscriptionService>();
            builder.Services.AddSingleton<IScheduleService, GoogleCalendarScheduleService>();
            builder.Services.AddSingleton<TwitchClipToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<TwitchStreamChangeToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<TwitchVideoToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<YoutubeFeedToDiscordNotificationTranslator>();
            builder.Services.AddSingleton<ScheduledStreamToDiscordNotificationTranslator>();
        }
    }
}
