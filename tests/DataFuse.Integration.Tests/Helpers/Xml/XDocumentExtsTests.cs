using DataFuse.Integration.Helpers.Xml;

namespace DataFuse.Integration.Tests.Helpers.Xml
{
    [TestFixture]
    public class XDocumentExtsTests
    {
        [Test]
        public void RemoveNamespaces_WithNamespacedDocument_ShouldRemoveNamespaces()
        {
            // Arrange
            var xmlWithNamespaces = @"<?xml version=""1.0""?>
                <root xmlns:ns1=""http://namespace1.com"" xmlns:ns2=""http://namespace2.com"">
                    <ns1:element1 ns2:attribute=""value"">Content1</ns1:element1>
                    <ns2:element2>Content2</ns2:element2>
                </root>";
            var document = System.Xml.Linq.XDocument.Parse(xmlWithNamespaces);

            // Act
            document.RemoveNamespaces();

            // Assert
            Assert.That(document.Root.Name.LocalName, Is.EqualTo("root"));
            Assert.That(document.Root.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
            
            var element1 = document.Root.Element("element1");
            var element2 = document.Root.Element("element2");
            
            Assert.That(element1, Is.Not.Null);
            Assert.That(element1.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
            Assert.That(element2, Is.Not.Null);
            Assert.That(element2.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
        }

        [Test]
        public void RemoveNamespaces_WithNullRoot_ShouldNotThrow()
        {
            // Arrange
            var document = new System.Xml.Linq.XDocument();

            // Act & Assert
            Assert.DoesNotThrow(() => document.RemoveNamespaces());
        }

        [Test]
        public void RemoveNamespaces_WithSimpleDocument_ShouldNotChangeStructure()
        {
            // Arrange
            var simpleXml = @"<root><child>Content</child></root>";
            var document = System.Xml.Linq.XDocument.Parse(simpleXml);

            // Act
            document.RemoveNamespaces();

            // Assert
            Assert.That(document.Root.Name.LocalName, Is.EqualTo("root"));
            Assert.That(document.Root.Element("child").Value, Is.EqualTo("Content"));
        }

        [Test]
        public void RemoveNamespaces_WithAttributeNamespaces_ShouldRemoveNamespaceAttributes()
        {
            // Arrange
            var xmlWithAttributeNamespaces = @"<root xmlns:ns=""http://namespace.com"" ns:attr=""value"">Content</root>";
            var document = System.Xml.Linq.XDocument.Parse(xmlWithAttributeNamespaces);

            // Act
            document.RemoveNamespaces();

            // Assert
            var rootElement = document.Root;
            Assert.That(rootElement.Attribute("attr"), Is.Not.Null);
            Assert.That(rootElement.Attribute("attr").Value, Is.EqualTo("value"));
            
            // Namespace declaration attributes should be removed
            var namespaceAttrs = rootElement.Attributes().Where(a => a.IsNamespaceDeclaration);
            Assert.That(namespaceAttrs.Count(), Is.EqualTo(0));
        }

        [Test]
        public void RemoveNamespaces_WithNestedNamespaces_ShouldRemoveAllNamespaces()
        {
            // Arrange
            var nestedXml = @"<?xml version=""1.0""?>
                <ns1:root xmlns:ns1=""http://ns1.com"" xmlns:ns2=""http://ns2.com"">
                    <ns2:level1 ns1:attr=""value1"">
                        <ns1:level2 ns2:attr=""value2"">
                            <ns2:level3>Content</ns2:level3>
                        </ns1:level2>
                    </ns2:level1>
                </ns1:root>";
            var document = System.Xml.Linq.XDocument.Parse(nestedXml);

            // Act
            document.RemoveNamespaces();

            // Assert
            Assert.That(document.Root.Name.LocalName, Is.EqualTo("root"));
            
            var level1 = document.Root.Element("level1");
            var level2 = level1?.Element("level2");
            var level3 = level2?.Element("level3");
            
            Assert.That(level1, Is.Not.Null);
            Assert.That(level1.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
            Assert.That(level2, Is.Not.Null);
            Assert.That(level2.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
            Assert.That(level3, Is.Not.Null);
            Assert.That(level3.Name.Namespace, Is.EqualTo(System.Xml.Linq.XNamespace.None));
            Assert.That(level3.Value, Is.EqualTo("Content"));
        }

        [Test]
        public void RemoveNamespaces_ShouldPreserveElementContent()
        {
            // Arrange
            var xmlContent = @"<ns:root xmlns:ns=""http://test.com"">
                <ns:child1>Text Content</ns:child1>
                <ns:child2 attr=""attrValue"">
                    <ns:grandchild>Nested Content</ns:grandchild>
                </ns:child2>
            </ns:root>";
            var document = System.Xml.Linq.XDocument.Parse(xmlContent);

            // Act
            document.RemoveNamespaces();

            // Assert
            Assert.That(document.Root.Element("child1").Value, Is.EqualTo("Text Content"));
            Assert.That(document.Root.Element("child2").Element("grandchild").Value, Is.EqualTo("Nested Content"));
            Assert.That(document.Root.Element("child2").Attribute("attr").Value, Is.EqualTo("attrValue"));
        }
    }
}