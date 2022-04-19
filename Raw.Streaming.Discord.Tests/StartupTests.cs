using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Raw.Streaming.Discord.Services;

namespace Raw.Streaming.Discord.Tests
{   
    [TestFixture]
    internal class StartupTests
    {
        [Test]
        public void TestStartup()
        {
            // Arrange and Act
            var startUp = new Startup();
            var host = new HostBuilder()
                .ConfigureWebJobs(startUp.Configure)
                .Build();

            // Assert
            Assert.IsNotNull(host);
            Assert.IsNotNull(host.Services.GetService(typeof(IDiscordBotMessageService)));
        }
    }
}
