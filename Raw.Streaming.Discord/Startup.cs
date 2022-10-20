﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Raw.Streaming.Discord.Services;
using System.Diagnostics.CodeAnalysis;
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
        builder.Services.AddLogging();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IDiscordApiService, DiscordApiService>();
        builder.Services.AddSingleton<IDiscordEventService, DiscordEventService>();
        builder.Services.AddSingleton<IDiscordMessageService, DiscordMessageService>();
    }
}