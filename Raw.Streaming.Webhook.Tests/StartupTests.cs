using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raw.Streaming.Webhook.Tests
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
            Assert.That(host.Services.GetService(typeof(ITwitchApiService)), Is.Not.Null);
            Assert.That(host.Services.GetService(typeof(ITwitchSubscriptionService)), Is.Not.Null);
            Assert.That(host.Services.GetService(typeof(IYoutubeSubscriptionService)), Is.Not.Null);
            Assert.That(host.Services.GetService(typeof(IScheduleService)), Is.Not.Null);
            Assert.That(host.Services.GetService(typeof(IMapper)), Is.Not.Null);
        }
    }
}
