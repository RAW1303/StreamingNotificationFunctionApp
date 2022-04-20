using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            Assert.That(host, Is.Not.Null);
            Assert.That(host.Services.GetService(typeof(IDiscordBotMessageService)), Is.Not.Null);
        }
    }
}
