using DataFuse.Integration.Helpers;

namespace DataFuse.Integration.Tests.Helpers
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void IsNotNullOrEmpty_WithValidString_ShouldReturnTrue()
        {
            // Arrange
            var testString = "Hello World";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithEmptyString_ShouldReturnFalse()
        {
            // Arrange
            var testString = string.Empty;

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNotNullOrEmpty_WithNullString_ShouldReturnFalse()
        {
            // Arrange
            string testString = null;

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNotNullOrEmpty_WithWhitespaceString_ShouldReturnTrue()
        {
            // Arrange
            var testString = "   ";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True); // Whitespace is not considered empty by string.IsNullOrEmpty
        }

        [Test]
        public void IsNotNullOrEmpty_WithSingleCharacter_ShouldReturnTrue()
        {
            // Arrange
            var testString = "a";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithLongString_ShouldReturnTrue()
        {
            // Arrange
            var testString = new string('a', 1000);

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithSpecialCharacters_ShouldReturnTrue()
        {
            // Arrange
            var testString = "!@#$%^&*()";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithUnicodeCharacters_ShouldReturnTrue()
        {
            // Arrange
            var testString = "Hello 世界";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithNewlineCharacters_ShouldReturnTrue()
        {
            // Arrange
            var testString = "\n\r\t";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithZeroWidthSpace_ShouldReturnTrue()
        {
            // Arrange
            var testString = "\u200B"; // Zero-width space

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_MultipleCallsOnSameString_ShouldReturnSameResult()
        {
            // Arrange
            var testString = "consistent";

            // Act
            var result1 = testString.IsNotNullOrEmpty();
            var result2 = testString.IsNotNullOrEmpty();
            var result3 = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
            Assert.That(result3, Is.True);
            Assert.That(result1, Is.EqualTo(result2));
            Assert.That(result2, Is.EqualTo(result3));
        }

        [Test]
        public void IsNotNullOrEmpty_BehaviorConsistentWithStringIsNullOrEmpty()
        {
            // Arrange
            var testCases = new[]
            {
                "normal string",
                "",
                null,
                " ",
                "\t\n",
                "a",
                new string('x', 100)
            };

            foreach (var testCase in testCases)
            {
                // Act
                var extensionResult = testCase.IsNotNullOrEmpty();
                var expectedResult = !string.IsNullOrEmpty(testCase);

                // Assert
                Assert.That(extensionResult, Is.EqualTo(expectedResult),
                    $"Failed for test case: '{testCase}'");
            }
        }

        [Test]
        public void IsNotNullOrEmpty_CanBeChainedWithOtherStringMethods()
        {
            // Arrange
            var testString = "  Hello World  ";

            // Act
            var result = testString.IsNotNullOrEmpty() && testString.Trim().Length > 0;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithInterpolatedString_ShouldReturnTrue()
        {
            // Arrange
            var name = "John";
            var testString = $"Hello {name}";

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotNullOrEmpty_WithStringBuilder_ShouldReturnTrue()
        {
            // Arrange
            var sb = new System.Text.StringBuilder();
            sb.Append("Hello");
            sb.Append(" World");
            var testString = sb.ToString();

            // Act
            var result = testString.IsNotNullOrEmpty();

            // Assert
            Assert.That(result, Is.True);
        }
    }
}