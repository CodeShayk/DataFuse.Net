using DataFuse.Integration.PathMatchers;

namespace DataFuse.Integration.Tests.PathMatchers
{
    [TestFixture]
    public class JPathMatcherTests
    {
        private JPathMatcher _matcher;

        [SetUp]
        public void Setup()
        {
            _matcher = new JPathMatcher();
        }

        [Test]
        public void IsMatch_WithNullOrEmptyInputPath_ShouldReturnFalse()
        {
            // Arrange
            var configuredPaths = new SchemaPaths { Paths = new[] { "$.root.test" } };

            // Act
            var result1 = _matcher.IsMatch(null, configuredPaths);
            var result2 = _matcher.IsMatch("", configuredPaths);

            // Assert
            Assert.That(result1, Is.False);
            Assert.That(result2, Is.False);
        }

        [Test]
        public void IsMatch_WithExactMatch_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.root.test.element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "$.root.test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithCaseInsensitiveMatch_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.ROOT.TEST.element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "$.root.test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithNoMatch_ShouldReturnFalse()
        {
            // Arrange
            var inputPath = "$.different.path.element";
            var configuredPaths = new SchemaPaths { Paths = new[] { "$.root.test" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMatch_WithMultipleConfiguredPaths_ShouldMatchAny()
        {
            // Arrange
            var inputPath = "$.root.second.element";
            var configuredPaths = new SchemaPaths 
            { 
                Paths = new[] { "$.root.first", "$.root.second", "$.root.third" } 
            };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithPartialPath_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.root.test.child.grandchild";
            var configuredPaths = new SchemaPaths { Paths = new[] { "test.child" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithArrayNotation_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.root.items[0].name";
            var configuredPaths = new SchemaPaths { Paths = new[] { "items[0]" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithEmptyConfiguredPaths_ShouldReturnFalse()
        {
            // Arrange
            var inputPath = "$.root.test";
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
            var inputPath = "$.root.test";
            var configuredPaths = new SchemaPaths { Paths = null };

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => 
                _matcher.IsMatch(inputPath, configuredPaths));
        }

        [Test]
        public void IsMatch_WithComplexJsonPath_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.store.book[*].author";
            var configuredPaths = new SchemaPaths { Paths = new[] { "book[*].author" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMatch_WithWildcardInConfigured_ShouldReturnTrue()
        {
            // Arrange
            var inputPath = "$.root.items.specific.value";
            var configuredPaths = new SchemaPaths { Paths = new[] { "items.specific" } };

            // Act
            var result = _matcher.IsMatch(inputPath, configuredPaths);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}