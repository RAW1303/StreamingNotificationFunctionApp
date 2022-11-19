using Raw.Streaming.Discord.Extensions;

namespace Raw.Streaming.Discord.Tests.Extensions
{
    [TestFixture]
    internal class DateTimeOffsetExtensionsTests
    {
        [Test]
        public void ToDiscordShortTime_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordShortTime();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:t>"));
        }

    }
}
