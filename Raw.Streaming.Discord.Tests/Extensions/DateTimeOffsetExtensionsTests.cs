using Raw.Streaming.Discord.Extensions;

namespace Raw.Streaming.Discord.Tests.Extensions
{
    [TestFixture]
    internal class DateTimeOffsetExtensionsTests
    {
        [Test]
        public void ToDiscordLongDate_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordLongDate();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:D>"));
        }

        [Test]
        public void ToDiscordLongDateTime_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordLongDateTime();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:F>"));
        }

        [Test]
        public void ToDiscordLongTime_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordLongTime();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:T>"));
        }

        [Test]
        public void ToDiscordRelaitveDateTime_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordRelaitveDateTime();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:R>"));
        }

        [Test]
        public void ToDiscordShortDate_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordShortDate();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:d>"));
        }

        [Test]
        public void ToDiscordShortDateTime_WhenNotNull_ReturnsStringValue()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2022, 6, 7, 21, 17, 26, TimeSpan.Zero);

            // Act
            var result = dateTime.ToDiscordShortDateTime();

            // Assert
            Assert.That(result, Is.EqualTo("<t:1654636646:f>"));
        }

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
