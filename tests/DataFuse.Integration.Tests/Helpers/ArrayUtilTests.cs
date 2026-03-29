using DataFuse.Integration.Helpers;

namespace DataFuse.Integration.Tests.Helpers
{
    [TestFixture]
    public class ArrayUtilTests
    {
        [Test]
        public void EnsureAndResizeArray_WithNullArray_ShouldCreateNewArrayWithOneElement()
        {
            // Arrange
            string[] array = null;

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, out int index);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(index, Is.EqualTo(0));
        }

        [Test]
        public void EnsureAndResizeArray_WithExistingArray_ShouldExpandByOne()
        {
            // Arrange
            var array = new[] { "item1", "item2" };

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, out int index);

            // Assert
            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(index, Is.EqualTo(2)); // Should point to the new element
            Assert.That(result[0], Is.EqualTo("item1"));
            Assert.That(result[1], Is.EqualTo("item2"));
        }

        [Test]
        public void EnsureAndResizeArray_WithExtendBy_WithNullArray_ShouldCreateNewArrayWithSpecifiedSize()
        {
            // Arrange
            int[] array = null;
            int extendBy = 5;

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, extendBy, out int index);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(5));
            Assert.That(index, Is.EqualTo(0));
        }

        [Test]
        public void EnsureAndResizeArray_WithExtendBy_WithExistingArray_ShouldExpandBySpecifiedAmount()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };
            int extendBy = 3;

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, extendBy, out int index);

            // Assert
            Assert.That(result.Length, Is.EqualTo(6));
            Assert.That(index, Is.EqualTo(3)); // Should point to the first new element
            Assert.That(result[0], Is.EqualTo(1));
            Assert.That(result[1], Is.EqualTo(2));
            Assert.That(result[2], Is.EqualTo(3));
        }

        [Test]
        public void EnsureAndResizeArray_WithEmptyArray_ShouldExpandCorrectly()
        {
            // Arrange
            var array = new string[0];

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, out int index);

            // Assert
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(index, Is.EqualTo(0));
        }

        [Test]
        public void EnsureAndResizeArray_WithExtendByZero_WithNullArray_ShouldCreateEmptyArray()
        {
            // Arrange
            string[] array = null;
            int extendBy = 0;

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, extendBy, out int index);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(0));
            Assert.That(index, Is.EqualTo(0));
        }

        [Test]
        public void EnsureAndResizeArray_WithExtendByZero_WithExistingArray_ShouldNotChangeSize()
        {
            // Arrange
            var array = new[] { "a", "b" };
            int extendBy = 0;

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, extendBy, out int index);

            // Assert
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(index, Is.EqualTo(2)); // Should point after the last existing element
            Assert.That(result[0], Is.EqualTo("a"));
            Assert.That(result[1], Is.EqualTo("b"));
        }

        [Test]
        public void EnsureAndResizeArray_PreservesExistingData()
        {
            // Arrange
            var array = new[] { 10, 20, 30, 40 };

            // Act
            var result = ArrayUtil.EnsureAndResizeArray(array, 2, out int index);

            // Assert
            Assert.That(result.Length, Is.EqualTo(6));
            Assert.That(index, Is.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(10));
            Assert.That(result[1], Is.EqualTo(20));
            Assert.That(result[2], Is.EqualTo(30));
            Assert.That(result[3], Is.EqualTo(40));
            Assert.That(result[4], Is.EqualTo(0)); // Default value for int
            Assert.That(result[5], Is.EqualTo(0)); // Default value for int
        }

        [Test]
        public void EnsureAndResizeArray_WorksWithDifferentTypes()
        {
            // Arrange
            var boolArray = new[] { true, false };
            var doubleArray = new[] { 1.1, 2.2 };

            // Act
            var boolResult = ArrayUtil.EnsureAndResizeArray(boolArray, out int boolIndex);
            var doubleResult = ArrayUtil.EnsureAndResizeArray(doubleArray, out int doubleIndex);

            // Assert
            Assert.That(boolResult.Length, Is.EqualTo(3));
            Assert.That(boolIndex, Is.EqualTo(2));
            Assert.That(boolResult[0], Is.True);
            Assert.That(boolResult[1], Is.False);
            Assert.That(boolResult[2], Is.False); // Default value for bool

            Assert.That(doubleResult.Length, Is.EqualTo(3));
            Assert.That(doubleIndex, Is.EqualTo(2));
            Assert.That(doubleResult[0], Is.EqualTo(1.1));
            Assert.That(doubleResult[1], Is.EqualTo(2.2));
            Assert.That(doubleResult[2], Is.EqualTo(0.0)); // Default value for double
        }

        [Test]
        public void EnsureAndResizeArray_MultipleConsecutiveCalls_ShouldWorkCorrectly()
        {
            // Arrange
            string[] array = null;

            // Act
            array = ArrayUtil.EnsureAndResizeArray(array, out int index1);
            array[index1] = "first";

            array = ArrayUtil.EnsureAndResizeArray(array, out int index2);
            array[index2] = "second";

            array = ArrayUtil.EnsureAndResizeArray(array, out int index3);
            array[index3] = "third";

            // Assert
            Assert.That(array.Length, Is.EqualTo(3));
            Assert.That(array[0], Is.EqualTo("first"));
            Assert.That(array[1], Is.EqualTo("second"));
            Assert.That(array[2], Is.EqualTo("third"));
            Assert.That(index1, Is.EqualTo(0));
            Assert.That(index2, Is.EqualTo(1));
            Assert.That(index3, Is.EqualTo(2));
        }
    }
}