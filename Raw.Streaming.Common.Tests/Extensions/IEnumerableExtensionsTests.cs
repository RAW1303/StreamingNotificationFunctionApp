using Raw.Streaming.Common.Extensions;

namespace Raw.Streaming.Common.Tests.Extensions;

[TestFixture]
internal class IEnumerableExtensionsTests
{
    [Test]
    public void IsNullOrEmpty_WhenNull_ReturnsTrue()
    {
        // Arrange
        IEnumerable<int> input = null;

        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullOrEmpty_WhenEmpty_ReturnsTrue()
    {
        // Arrange
        var input = Enumerable.Empty<int>();

        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullOrEmpty_WhenPopulated_ReturnsFalse()
    {
        // Arrange
        var input = Enumerable.Range(0, 10);

        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        Assert.That(result, Is.False);
    }
}
