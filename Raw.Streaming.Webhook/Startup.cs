using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Webhook.Services;

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
            builder.Services.AddSingleton<ITwitchApiService, TwitchApiService>();
            builder.Services.AddSingleton<ITwitchSubscriptionService, TwitchSubscriptionService>();
            builder.Services.AddSingleton<IYoutubeSubscriptionService, YoutubeSubscriptionService>();
            builder.Services.AddSingleton<IScheduleService, GoogleCalendarScheduleService>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}
