namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class CollectionResultTests
    {
        [Test]
        public void Constructor_WithEmptyEnumerable_ShouldCreateEmptyCollection()
        {
            // Arrange
            var items = new List<string>();

            // Act
            var collection = new CollectionResult<string>(items);

            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Count, Is.EqualTo(0));
            Assert.That(collection, Is.InstanceOf<IQueryResult>());
        }

        [Test]
        public void Constructor_WithEnumerable_ShouldCreateCollectionWithItems()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };

            // Act
            var collection = new CollectionResult<string>(items);

            // Assert
            Assert.That(collection.Count, Is.EqualTo(3));
            Assert.That(collection[0], Is.EqualTo("item1"));
            Assert.That(collection[1], Is.EqualTo("item2"));
            Assert.That(collection[2], Is.EqualTo("item3"));
        }

        [Test]
        public void Constructor_WithNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() =>
                new CollectionResult<string>(null));
        }

        [Test]
        public void DefaultConstructor_ShouldCreateEmptyCollection()
        {
            // Act
            var collection = new CollectionResult<string>();

            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Count, Is.EqualTo(0));
            Assert.That(collection, Is.InstanceOf<IQueryResult>());
        }

        [Test]
        public void Add_ShouldAddItemToCollection()
        {
            // Arrange
            var collection = new CollectionResult<string>();

            // Act
            collection.Add("new_item");

            // Assert
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.EqualTo("new_item"));
        }

        [Test]
        public void AddRange_ShouldAddMultipleItemsToCollection()
        {
            // Arrange
            var collection = new CollectionResult<string>();
            var newItems = new List<string> { "item1", "item2" };

            // Act
            collection.AddRange(newItems);

            // Assert
            Assert.That(collection.Count, Is.EqualTo(2));
            Assert.That(collection[0], Is.EqualTo("item1"));
            Assert.That(collection[1], Is.EqualTo("item2"));
        }

        [Test]
        public void Remove_ShouldRemoveItemFromCollection()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act
            var removed = collection.Remove("item2");

            // Assert
            Assert.That(removed, Is.True);
            Assert.That(collection.Count, Is.EqualTo(2));
            Assert.That(collection.Contains("item2"), Is.False);
            Assert.That(collection[0], Is.EqualTo("item1"));
            Assert.That(collection[1], Is.EqualTo("item3"));
        }

        [Test]
        public void Clear_ShouldRemoveAllItems()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act
            collection.Clear();

            // Assert
            Assert.That(collection.Count, Is.EqualTo(0));
        }

        [Test]
        public void Contains_ShouldReturnTrueForExistingItem()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act & Assert
            Assert.That(collection.Contains("item2"), Is.True);
            Assert.That(collection.Contains("item4"), Is.False);
        }

        [Test]
        public void IndexOf_ShouldReturnCorrectIndex()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act & Assert
            Assert.That(collection.IndexOf("item2"), Is.EqualTo(1));
            Assert.That(collection.IndexOf("item4"), Is.EqualTo(-1));
        }

        [Test]
        public void Insert_ShouldInsertItemAtSpecifiedIndex()
        {
            // Arrange
            var items = new List<string> { "item1", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act
            collection.Insert(1, "item2");

            // Assert
            Assert.That(collection.Count, Is.EqualTo(3));
            Assert.That(collection[0], Is.EqualTo("item1"));
            Assert.That(collection[1], Is.EqualTo("item2"));
            Assert.That(collection[2], Is.EqualTo("item3"));
        }

        [Test]
        public void RemoveAt_ShouldRemoveItemAtSpecifiedIndex()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act
            collection.RemoveAt(1);

            // Assert
            Assert.That(collection.Count, Is.EqualTo(2));
            Assert.That(collection[0], Is.EqualTo("item1"));
            Assert.That(collection[1], Is.EqualTo("item3"));
        }

        [Test]
        public void Enumeration_ShouldWorkCorrectly()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new CollectionResult<string>(items);

            // Act
            var enumeratedItems = collection.ToList();

            // Assert
            Assert.That(enumeratedItems.Count, Is.EqualTo(3));
            Assert.That(enumeratedItems, Is.EqualTo(items));
        }

        [Test]
        public void GenericType_ShouldWorkWithDifferentTypes()
        {
            // Arrange
            var numbers = new List<int> { 1, 2, 3 };
            var entities = new List<TestEntity>
            {
                new TestEntity { Name = "entity1", Value = 1 },
                new TestEntity { Name = "entity2", Value = 2 }
            };

            // Act
            var numberCollection = new CollectionResult<int>(numbers);
            var entityCollection = new CollectionResult<TestEntity>(entities);

            // Assert
            Assert.That(numberCollection.Count, Is.EqualTo(3));
            Assert.That(numberCollection[0], Is.EqualTo(1));
            Assert.That(entityCollection.Count, Is.EqualTo(2));
            Assert.That(entityCollection[0].Name, Is.EqualTo("entity1"));
        }
    }
}