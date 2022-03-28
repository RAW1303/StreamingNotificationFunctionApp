using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Discord.Services;

[assembly: FunctionsStartup(typeof(Raw.Streaming.Discord.Startup))]

namespace Raw.Streaming.Discord
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IDiscordBotMessageService, DiscordBotMessageService>();
        }
    }
}
