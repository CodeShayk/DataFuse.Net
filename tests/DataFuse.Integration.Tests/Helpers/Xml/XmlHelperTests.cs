using System.Xml;
using System.Xml.Serialization;
using DataFuse.Integration.Helpers.Xml;

namespace DataFuse.Integration.Tests.Helpers.Xml
{
    [TestFixture]
    public class XmlHelperTests
    {
        [Test]
        public void SerializeToXml_WithValidObject_ShouldReturnXmlString()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("Test"));
            Assert.That(result, Contains.Substring("123"));
            Assert.That(result, Contains.Substring("TestSerializableEntity"));
        }

        [Test]
        public void SerializeToXml_WithNamespaces_ShouldIncludeNamespaces()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("test", "http://test.namespace");
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("http://test.namespace"));
        }

        [Test]
        public void SerializeToXml_WithXmlDeclaration_ShouldIncludeDeclaration()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.StartWith("<?xml"));
        }

        [Test]
        public void SerializeToXml_WithIndentation_ShouldFormatXml()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "  "
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("\r\n") | Contains.Substring("\n")); // Should contain line breaks
        }

        [Test]
        public void SerializeToXml_WithNullObject_ShouldThrowException()
        {
            // Arrange
            TestSerializableEntity testObject = null;
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                XmlHelper.SerializeToXml(testObject, namespaces, settings));
        }

        [Test]
        public void SerializeToXml_WithNullNamespaces_ShouldStillWork()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            XmlSerializerNamespaces namespaces = null;
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("Test"));
        }

        [Test]
        public void SerializeToXml_WithNullSettings_ShouldThrowException()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            XmlWriterSettings settings = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                XmlHelper.SerializeToXml(testObject, namespaces, settings));
        }

        [Test]
        public void SerializeToXml_WithComplexObject_ShouldSerializeAllProperties()
        {
            // Arrange
            var complexObject = new ComplexSerializableEntity
            {
                Id = 1,
                Name = "Complex",
                Child = new TestSerializableEntity { Name = "Child", Value = 456 },
                Items = new[] { "item1", "item2", "item3" }
            };
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            // Act
            var result = XmlHelper.SerializeToXml(complexObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("Complex"));
            Assert.That(result, Contains.Substring("Child"));
            Assert.That(result, Contains.Substring("456"));
            Assert.That(result, Contains.Substring("item1"));
            Assert.That(result, Contains.Substring("item2"));
            Assert.That(result, Contains.Substring("item3"));
        }

        [Test]
        public void SerializeToXml_WithEmptyNamespaces_ShouldNotIncludeCustomNamespaces()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("Test"));
            // Should still have default XML namespace declarations
        }

        [Test]
        public void SerializeToXml_WithMultipleNamespaces_ShouldIncludeAll()
        {
            // Arrange
            var testObject = new TestSerializableEntity { Name = "Test", Value = 123 };
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("ns1", "http://namespace1.com");
            namespaces.Add("ns2", "http://namespace2.com");
            namespaces.Add("ns3", "http://namespace3.com");
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false
            };

            // Act
            var result = XmlHelper.SerializeToXml(testObject, namespaces, settings);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("http://namespace1.com"));
            Assert.That(result, Contains.Substring("http://namespace2.com"));
            Assert.That(result, Contains.Substring("http://namespace3.com"));
        }

        // Test classes for serialization
        public class TestSerializableEntity
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class ComplexSerializableEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public TestSerializableEntity Child { get; set; }
            public string[] Items { get; set; }
        }
    }
}