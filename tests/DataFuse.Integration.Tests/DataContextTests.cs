using Moq;

namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class DataContextTests
    {
        private Mock<IEntityRequest> _mockRequest;

        [SetUp]
        public void Setup()
        {
            _mockRequest = new Mock<IEntityRequest>();
        }

        [Test]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Act
            var dataContext = new DataContext(_mockRequest.Object);

            // Assert
            Assert.That(dataContext.Request, Is.EqualTo(_mockRequest.Object));
            Assert.That(dataContext.Cache, Is.Not.Null);
            Assert.That(dataContext.Cache, Is.InstanceOf<Dictionary<string, object>>());
            Assert.That(dataContext.Cache.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithNullRequest_ShouldAllowNullRequest()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new DataContext(null));
        }

        [Test]
        public void Request_ShouldReturnSetRequest()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);

            // Act
            var request = dataContext.Request;

            // Assert
            Assert.That(request, Is.SameAs(_mockRequest.Object));
        }

        [Test]
        public void Cache_ShouldBeSettableAndGettable()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);
            var newCache = new Dictionary<string, object> { { "key1", "value1" } };

            // Act
            dataContext.Cache = newCache;

            // Assert
            Assert.That(dataContext.Cache, Is.SameAs(newCache));
            Assert.That(dataContext.Cache["key1"], Is.EqualTo("value1"));
        }

        [Test]
        public void Cache_CanAddItems()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);

            // Act
            dataContext.Cache.Add("test_key", "test_value");
            dataContext.Cache["another_key"] = 42;

            // Assert
            Assert.That(dataContext.Cache.Count, Is.EqualTo(2));
            Assert.That(dataContext.Cache["test_key"], Is.EqualTo("test_value"));
            Assert.That(dataContext.Cache["another_key"], Is.EqualTo(42));
        }

        [Test]
        public void Cache_CanRemoveItems()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);
            dataContext.Cache.Add("key_to_remove", "value");

            // Act
            var removed = dataContext.Cache.Remove("key_to_remove");

            // Assert
            Assert.That(removed, Is.True);
            Assert.That(dataContext.Cache.Count, Is.EqualTo(0));
            Assert.That(dataContext.Cache.ContainsKey("key_to_remove"), Is.False);
        }

        [Test]
        public void Cache_CanCheckContainsKey()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);
            dataContext.Cache.Add("existing_key", "value");

            // Act & Assert
            Assert.That(dataContext.Cache.ContainsKey("existing_key"), Is.True);
            Assert.That(dataContext.Cache.ContainsKey("non_existing_key"), Is.False);
        }

        [Test]
        public void Cache_CanClear()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);
            dataContext.Cache.Add("key1", "value1");
            dataContext.Cache.Add("key2", "value2");

            // Act
            dataContext.Cache.Clear();

            // Assert
            Assert.That(dataContext.Cache.Count, Is.EqualTo(0));
        }

        [Test]
        public void Cache_ShouldImplementIEntityContextCache()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);

            // Act & Assert
            Assert.That(dataContext, Is.InstanceOf<IEntityContextCache>());
        }

        [Test]
        public void DataContext_ShouldImplementIDataContext()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);

            // Act & Assert
            Assert.That(dataContext, Is.InstanceOf<IDataContext>());
        }

        [Test]
        public void Cache_WithComplexObjects_ShouldStoreAndRetrieveCorrectly()
        {
            // Arrange
            var dataContext = new DataContext(_mockRequest.Object);
            var complexObject = new TestEntity { Name = "test", Value = 123 };
            var queryResult = new TestQueryResult { Data = "query_data" };

            // Act
            dataContext.Cache["entity"] = complexObject;
            dataContext.Cache["result"] = queryResult;

            // Assert
            Assert.That(dataContext.Cache["entity"], Is.SameAs(complexObject));
            Assert.That(dataContext.Cache["result"], Is.SameAs(queryResult));
            Assert.That(((TestEntity)dataContext.Cache["entity"]).Name, Is.EqualTo("test"));
            Assert.That(((TestQueryResult)dataContext.Cache["result"]).Data, Is.EqualTo("query_data"));
        }
    }
}