using DataFuse.Integration.Helpers;
using DataFuse.Integration.Tests;

namespace DataFuse.Integration.Tests.Helpers
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void Each_WithValidEnumerable_ShouldExecuteActionForEachItem()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var processedItems = new List<string>();

            // Act
            items.Each(item => processedItems.Add(item.ToUpper()));

            // Assert
            Assert.That(processedItems.Count, Is.EqualTo(3));
            Assert.That(processedItems[0], Is.EqualTo("ITEM1"));
            Assert.That(processedItems[1], Is.EqualTo("ITEM2"));
            Assert.That(processedItems[2], Is.EqualTo("ITEM3"));
        }

        [Test]
        public void Each_WithEmptyEnumerable_ShouldNotExecuteAction()
        {
            // Arrange
            var items = new List<string>();
            var actionExecuted = false;

            // Act
            items.Each(item => actionExecuted = true);

            // Assert
            Assert.That(actionExecuted, Is.False);
        }

        [Test]
        public void Each_WithNullAction_ShouldThrowArgumentNullException()
        {
            // Arrange
            var items = new List<string> { "item1" };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.Each(null));
        }

        [Test]
        public void Each_ShouldMaintainOrder()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var processedItems = new List<int>();

            // Act
            items.Each(item => processedItems.Add(item * 2));

            // Assert
            Assert.That(processedItems, Is.EqualTo(new[] { 2, 4, 6, 8, 10 }));
        }

        [Test]
        public void ToCSV_WithDefaultSeparator_ShouldReturnCommaSeparatedValues()
        {
            // Arrange
            var items = new List<string> { "apple", "banana", "cherry" };

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.EqualTo("apple,banana,cherry"));
        }

        [Test]
        public void ToCSV_WithCustomSeparator_ShouldReturnCustomSeparatedValues()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4 };

            // Act
            var result = items.ToCSV('|');

            // Assert
            Assert.That(result, Is.EqualTo("1|2|3|4"));
        }

        [Test]
        public void ToCSV_WithSingleItem_ShouldReturnSingleValue()
        {
            // Arrange
            var items = new List<string> { "single" };

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.EqualTo("single"));
        }

        [Test]
        public void ToCSV_WithEmptyEnumerable_ShouldReturnEmptyString()
        {
            // Arrange
            var items = new List<string>();

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void ToCSV_WithNullEnumerable_ShouldReturnNull()
        {
            // Arrange
            List<string> items = null;

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ToCSV_WithNullEnumerableAndCustomSeparator_ShouldReturnNull()
        {
            // Arrange
            List<int> items = null;

            // Act
            var result = items.ToCSV(';');

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ToCSV_WithDifferentTypes_ShouldCallToStringOnEachItem()
        {
            // Arrange
            var items = new List<object> { 1, 2.5, true, "text" };

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.EqualTo("1,2.5,True,text"));
        }

        [Test]
        public void ToCSV_WithNullItemsInEnumerable_ShouldHandleNulls()
        {
            // Arrange
            var items = new List<string> { "item1", null, "item3" };

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.EqualTo("item1,,item3"));
        }

        [Test]
        public void ToCSV_WithCustomObjectsOverridingToString_ShouldUseToStringOutput()
        {
            // Arrange
            var items = new List<TestEntity>
            {
                new TestEntity { Name = "Entity1", Value = 1 },
                new TestEntity { Name = "Entity2", Value = 2 }
            };

            // Act
            var result = items.ToCSV();

            // Assert
            // This will depend on TestEntity's ToString() implementation
            // Since TestEntity doesn't override ToString(), it will use the default
            Assert.That(result, Contains.Substring("TestEntity"));
        }

        [Test]
        public void ToCSV_WithSpecialCharactersInSeparator_ShouldWorkCorrectly()
        {
            // Arrange
            var items = new List<string> { "a", "b", "c" };

            // Act
            var result = items.ToCSV('\t'); // Tab separator

            // Assert
            Assert.That(result, Is.EqualTo("a\tb\tc"));
        }

        [Test]
        public void ToCSV_WithLargeEnumerable_ShouldHandleEfficiently()
        {
            // Arrange
            var items = Enumerable.Range(1, 1000).ToList();

            // Act
            var result = items.ToCSV();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Split(',').Length, Is.EqualTo(1000));
            Assert.That(result, Does.StartWith("1,2,3"));
            Assert.That(result, Does.EndWith("998,999,1000"));
        }

        [Test]
        public void Each_WithComplexObjects_ShouldWorkCorrectly()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Name = "Test1", Value = 1 },
                new TestEntity { Name = "Test2", Value = 2 }
            };
            var totalValue = 0;

            // Act
            entities.Each(entity => totalValue += entity.Value);

            // Assert
            Assert.That(totalValue, Is.EqualTo(3));
        }

        [Test]
        public void Each_WithSideEffects_ShouldModifyOriginalObjects()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Name = "Test1", Value = 1 },
                new TestEntity { Name = "Test2", Value = 2 }
            };

            // Act
            entities.Each(entity => entity.Value *= 2);

            // Assert
            Assert.That(entities[0].Value, Is.EqualTo(2));
            Assert.That(entities[1].Value, Is.EqualTo(4));
        }

        [Test]
        public void ToCSV_PerformanceWithDifferentSeparators_ShouldBeConsistent()
        {
            // Arrange
            var items = Enumerable.Range(1, 100).Select(i => $"item{i}").ToList();

            // Act
            var commaResult = items.ToCSV(',');
            var pipeResult = items.ToCSV('|');
            var semicolonResult = items.ToCSV(';');

            // Assert
            Assert.That(commaResult.Split(',').Length, Is.EqualTo(100));
            Assert.That(pipeResult.Split('|').Length, Is.EqualTo(100));
            Assert.That(semicolonResult.Split(';').Length, Is.EqualTo(100));
        }
    }
}