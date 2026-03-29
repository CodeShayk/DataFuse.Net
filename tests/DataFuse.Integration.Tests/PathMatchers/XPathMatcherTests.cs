using DataFuse.Integration.PathMatchers;

namespace DataFuse.Integration.Tests.PathMatchers
{
    [TestFixture]
    public class XPathMatcherTests
    {
        private XPathMatcher _matcher;

        [SetUp]
        public void Setup()
        {
            _matcher = new XPathMatcher();
        }

        [Test]
        public void IsMatch_WithNullInputPath_ShouldReturnFalse()
        {
            // Arrange
            string inputPath = null;
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithExactMatch_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "/root/test/element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithCaseInsensitiveMatch_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "/ROOT/TEST/element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithNoMatch_ShouldReturnFalse()
        {
            // Arrange
            var inputPath = "/different/path/element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithMultipleConfiguredPaths_ShouldMatchAny()
        {
            // Arrange
            var inputPath = "/root/second/element";
            var configuredPaths = new SchemaPaths
            {
                Paths = new[] { "/root/first", "/root/second", "/root/third" }
            };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithAncestorAxis_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "/root/element=ancestor::parent/@attribute";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/parent" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithAncestorAxisAndPredicate_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "/root/element=ancestor::parent[position()=1]/@attribute";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/parent" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithAncestorAxisNoMatch_ShouldReturnFalse()
        {
            // Arrange
            var inputPath = "/root/element=ancestor::different/@attribute";
            var configuredPaths = new SchemaPaths { Paths = new[] { "/root/parent" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithPartialPath_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "/root/test/child/grandchild";
            var configuredPaths = new SchemaPaths { Paths = new[] { "test/child" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithEmptyConfiguredPaths_ShouldReturnFalse()
        {
            // Arrange
            var inputPath = "/root/test";
            var configuredPaths = new SchemaPaths { Paths = new string[0] };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithNullConfiguredPaths_ShouldThrow()
        {
            // Arrange
            var inputPath = "/root/test";
            var configuredPaths = new SchemaPaths { Paths = null };

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() =>
                _matcher.IsMatch(inputPath, configuredPaths));
        }
    }
}