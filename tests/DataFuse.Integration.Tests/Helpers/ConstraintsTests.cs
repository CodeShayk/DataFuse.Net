using DataFuse.Integration.Helpers;
using DataFuse.Integration.Tests;

namespace DataFuse.Integration.Tests.Helpers
{
    [TestFixture]
    public class ConstraintsTests
    {
        [Test]
        public void NotNull_WithNonNullValue_ShouldNotThrow()
        {
            // Arrange
            var value = "not null";

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithNullString_ShouldThrowArgumentNullException()
        {
            // Arrange
            string value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Is.EqualTo("String"));
        }

        [Test]
        public void NotNull_WithNullObject_ShouldThrowArgumentNullException()
        {
            // Arrange
            object value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Is.EqualTo("Object"));
        }

        [Test]
        public void NotNull_WithNullCustomType_ShouldThrowArgumentNullExceptionWithCorrectTypeName()
        {
            // Arrange
            TestEntity value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Is.EqualTo("TestEntity"));
        }

        [Test]
        public void NotNull_WithValidCustomType_ShouldNotThrow()
        {
            // Arrange
            var value = new TestEntity { Name = "test", Value = 123 };

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithNullInt_ShouldThrowArgumentNullException()
        {
            // Arrange
            int? value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Is.EqualTo("Nullable`1"));
        }

        [Test]
        public void NotNull_WithValidInt_ShouldNotThrow()
        {
            // Arrange
            int? value = 42;

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithZeroValue_ShouldNotThrow()
        {
            // Arrange
            int value = 0;

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithEmptyString_ShouldNotThrow()
        {
            // Arrange
            var value = string.Empty;

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithEmptyArray_ShouldNotThrow()
        {
            // Arrange
            var value = new string[0];

            // Act & Assert
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithNullArray_ShouldThrowArgumentNullException()
        {
            // Arrange
            string[] value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Is.EqualTo("String[]"));
        }

        [Test]
        public void NotNull_ChainedCalls_ShouldWorkCorrectly()
        {
            // Arrange
            var value1 = "first";
            var value2 = "second";
            var value3 = "third";

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                value1.NotNull();
                value2.NotNull();
                value3.NotNull();
            });
        }

        [Test]
        public void NotNull_InMethodChain_ShouldNotBreakFlow()
        {
            // Arrange
            var value = "test value";

            // Assert - The extension method should not return anything (void)
            // This test mainly checks that the method exists and can be called
            Assert.DoesNotThrow(() => value.NotNull());
        }

        [Test]
        public void NotNull_WithGenericType_ShouldUseCorrectTypeName()
        {
            // Arrange
            List<string> value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Contains.Substring("List"));
        }

        [Test]
        public void NotNull_WithInterface_ShouldUseInterfaceName()
        {
            // Arrange
            System.Collections.Generic.IEnumerable<int> value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.ParamName, Contains.Substring("IEnumerable"));
        }

        [Test]
        public void NotNull_ExceptionMessage_ShouldBeCorrect()
        {
            // Arrange
            string value = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull());
            Assert.That(ex.Message, Contains.Substring("Value cannot be null"));
            Assert.That(ex.ParamName, Is.EqualTo("String"));
        }
    }
}