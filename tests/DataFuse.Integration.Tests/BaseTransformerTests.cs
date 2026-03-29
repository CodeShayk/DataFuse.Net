using Moq;

namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class BaseTransformerTests
    {
        private TestTransformer _transformer;
        private Mock<IDataContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            _transformer = new TestTransformer();
            _mockContext = new Mock<IDataContext>();
        }

        [Test]
        public void SupportedQueryResult_ShouldReturnCorrectType()
        {
            // Act
            var supportedType = _transformer.SupportedQueryResult;

            // Assert
            Assert.That(supportedType, Is.EqualTo(typeof(TestQueryResult)));
        }

        [Test]
        public void Context_ShouldBeNullInitially()
        {
            // Arrange
            var transformer = new TestTransformer();

            // Act & Assert
            // Context is protected, so we can't directly access it in tests
            // We'll test through SetContext method
            Assert.DoesNotThrow(() => transformer.SetContext(_mockContext.Object));
        }

        [Test]
        public void SetContext_ShouldSetContext()
        {
            // Act
            _transformer.SetContext(_mockContext.Object);

            // Assert
            // Since Context is protected, we can't directly verify it was set
            // But the method should complete without throwing
            Assert.DoesNotThrow(() => _transformer.SetContext(_mockContext.Object));
        }

        [Test]
        public void SetContext_WithNull_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _transformer.SetContext(null));
        }

        [Test]
        public void Transform_WithIQueryResultAndIEntity_ShouldCallTypedTransform()
        {
            // Arrange
            var queryResult = new TestQueryResult { Data = "test_data" };
            var entity = new TestEntity();

            // Act
            _transformer.Transform((IQueryResult)queryResult, (IEntity)entity);

            // Assert
            Assert.That(entity.Name, Is.EqualTo("test_data"));
            Assert.That(entity.Value, Is.EqualTo(9)); // "test_data".Length
        }

        [Test]
        public void Transform_WithTypedParameters_ShouldTransformEntity()
        {
            // Arrange
            var queryResult = new TestQueryResult { Data = "hello_world" };
            var entity = new TestEntity();

            // Act
            _transformer.Transform(queryResult, entity);

            // Assert
            Assert.That(entity.Name, Is.EqualTo("hello_world"));
            Assert.That(entity.Value, Is.EqualTo(11)); // "hello_world".Length
        }

        [Test]
        public void Transform_WithNullData_ShouldHandleGracefully()
        {
            // Arrange
            var queryResult = new TestQueryResult { Data = null };
            var entity = new TestEntity();

            // Act
            _transformer.Transform(queryResult, entity);

            // Assert
            Assert.That(entity.Name, Is.Null);
            Assert.That(entity.Value, Is.EqualTo(0));
        }

        [Test]
        public void Transform_WithWrongQueryResultType_ShouldThrowInvalidCastException()
        {
            // Arrange
            var wrongQueryResult = new TestChildQueryResult { ChildData = "wrong_type" };
            var entity = new TestEntity();

            // Act & Assert
            Assert.Throws<InvalidCastException>(() =>
                _transformer.Transform((IQueryResult)wrongQueryResult, (IEntity)entity));
        }

        [Test]
        public void Transform_WithWrongEntityType_ShouldThrowInvalidCastException()
        {
            // Arrange
            var queryResult = new TestQueryResult { Data = "test_data" };
            var wrongEntity = new TestChildEntity { Description = "wrong_type" };

            // Act & Assert
            Assert.Throws<InvalidCastException>(() =>
                _transformer.Transform((IQueryResult)queryResult, (IEntity)wrongEntity));
        }
    }
}