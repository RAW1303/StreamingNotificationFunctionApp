using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Raw.Streaming.Discord.Model.DiscordApi;

internal abstract class DiscordApiContent
{
    public StringContent ToStringContent()
    {
        var notificationRequestJson = JsonSerializer.Serialize(this, GetType());
        return new StringContent(notificationRequestJson, Encoding.UTF8, "application/json");
    }
}