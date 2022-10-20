using Raw.Streaming.Discord.Model.DiscordApi;
using System.Text.Json;

namespace Raw.Streaming.Discord.Tests.Model;

[TestFixture]
internal class DiscordApiContentTests
{
    [Test, AutoData]
    public async Task GuildScheduledEvent_ToStringContent_ReturnsValidJson(GuildScheduledEvent guildScheduledEvent)
    {
        // Act
        var stringContent = guildScheduledEvent.ToStringContent();

        // Assert
        var json = await stringContent.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GuildScheduledEvent>(json);
        result.Should().BeEquivalentTo(guildScheduledEvent);
    }

    [Test, AutoData]
    public async Task Message_ToStringContent_ReturnsValidJson(Message message)
    {
        // Act
        var stringContent = message.ToStringContent();

        // Assert
        var json = await stringContent.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Message>(json);
        result.Should().BeEquivalentTo(message);
    }
}
