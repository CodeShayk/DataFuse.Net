using DataFuse.Integration.Helpers.Xml;

namespace DataFuse.Integration.Tests.Helpers.Xml
{
    [TestFixture]
    public class XmlSanitizerTests
    {
        [Test]
        public void Sanitize_WithNullInput_ShouldReturnNull()
        {
            // Act
            var result = XmlSanitizer.Sanitize(null);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Sanitize_WithEmptyString_ShouldReturnEmptyString()
        {
            // Act
            var result = XmlSanitizer.Sanitize(string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Sanitize_WithValidXml_ShouldReturnUnchanged()
        {
            // Arrange
            var validXml = "<root><child>Valid content</child></root>";

            // Act
            var result = XmlSanitizer.Sanitize(validXml);

            // Assert
            Assert.That(result, Is.EqualTo(validXml));
        }

        [Test]
        public void Sanitize_WithValidNumericEntity_ShouldReturnUnchanged()
        {
            // Arrange
            var xmlWithEntity = "<root>&#65;&#66;&#67;</root>"; // ABC

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithEntity);

            // Assert
            Assert.That(result, Is.EqualTo(xmlWithEntity));
        }

        [Test]
        public void Sanitize_WithValidHexEntity_ShouldReturnUnchanged()
        {
            // Arrange
            var xmlWithHexEntity = "<root>&#x41;&#x42;&#x43;</root>"; // ABC

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithHexEntity);

            // Assert
            Assert.That(result, Is.EqualTo(xmlWithHexEntity));
        }

        [Test]
        public void Sanitize_WithInvalidControlCharacterEntity_ShouldRemoveEntity()
        {
            // Arrange
            var xmlWithInvalidEntity = "<root>&#0;</root>"; // NULL character

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithInvalidEntity);

            // Assert
            Assert.That(result, Is.EqualTo("<root></root>"));
        }

        [Test]
        public void Sanitize_WithMultipleInvalidEntities_ShouldRemoveAllInvalid()
        {
            // Arrange
            var xmlWithInvalidEntities = "<root>&#0;Valid&#1;Content&#2;</root>";

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithInvalidEntities);

            // Assert
            Assert.That(result, Is.EqualTo("<root>ValidContent</root>"));
        }

        [Test]
        public void Sanitize_WithMixedValidAndInvalidEntities_ShouldKeepValidRemoveInvalid()
        {
            // Arrange
            var xmlWithMixedEntities = "<root>&#65;&#0;&#66;&#1;&#67;</root>"; // A[invalid]B[invalid]C

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithMixedEntities);

            // Assert
            Assert.That(result, Is.EqualTo("<root>&#65;&#66;&#67;</root>"));
        }

        [Test]
        public void Sanitize_WithNamedEntities_ShouldKeepNamedEntities()
        {
            // Arrange
            var xmlWithNamedEntities = "<root>&amp;&lt;&gt;&quot;&apos;</root>";

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithNamedEntities);

            // Assert
            Assert.That(result, Is.EqualTo(xmlWithNamedEntities));
        }

        [Test]
        public void Sanitize_WithInvalidNumericEntity_ShouldReturnOriginalEntity()
        {
            // Arrange
            var xmlWithInvalidNumber = "<root>&#ABC;</root>"; // Invalid number

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithInvalidNumber);

            // Assert
            Assert.That(result, Is.EqualTo(xmlWithInvalidNumber)); // Should remain unchanged
        }

        [Test]
        public void Sanitize_WithHexEntityWithoutXPrefix_ShouldRemoveInvalidCharacter()
        {
            // Arrange
            var xmlWithInvalidHex = "<root>&#x0;</root>"; // NULL character in hex

            // Act
            var result = XmlSanitizer.Sanitize(xmlWithInvalidHex);

            // Assert
            Assert.That(result, Is.EqualTo("<root></root>"));
        }

        [Test]
        public void Sanitize_WithComplexXmlContainingInvalidEntities_ShouldSanitizeCorrectly()
        {
            // Arrange
            var complexXml = @"<root attr=""value&#1;more"">
                <child>Valid content &#65; here</child>
                <other>&#0;Bad&#2;Content&#65;</other>
            </root>";

            // Act
            var result = XmlSanitizer.Sanitize(complexXml);

            // Assert
            Assert.That(result, Contains.Substring("Valid content &#65; here"));
            Assert.That(result, Contains.Substring("BadContent&#65;"));
            Assert.That(result, Does.Not.Contain("&#0;"));
            Assert.That(result, Does.Not.Contain("&#1;"));
            Assert.That(result, Does.Not.Contain("&#2;"));
        }

        [Test]
        public void Sanitize_WithLargeDocument_ShouldProcessEfficiently()
        {
            // Arrange
            var largeXml = "<root>";
            for (var i = 0; i < 1000; i++)
                largeXml += $"<item>Content {i} &#{i % 32}; more content</item>";
            largeXml += "</root>";

            // Act & Assert
            Assert.DoesNotThrow(() => XmlSanitizer.Sanitize(largeXml));

            var result = XmlSanitizer.Sanitize(largeXml);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.LessThanOrEqualTo(largeXml.Length)); // Should be same or smaller after sanitization
        }
    }
}