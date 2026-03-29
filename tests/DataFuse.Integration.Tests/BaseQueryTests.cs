using Moq;

namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class BaseQueryTests
    {
        private TestQuery _testQuery;
        private Mock<IQueryEngine> _mockEngine;

        [SetUp]
        public void Setup()
        {
            _testQuery = new TestQuery();
            _mockEngine = new Mock<IQueryEngine>();
        }

        [Test]
        public void Children_ShouldInitializeAsEmpty()
        {
            // Arrange & Act
            var query = new TestQuery();

            // Assert
            Assert.That(query.Children, Is.Empty);
        }

        [Test]
        public void Children_CanSetAndGetValue()
        {
            // Arrange
            var childQueries = new List<IQuery> { new TestChildQuery() };

            // Act
            _testQuery.Children = childQueries;

            // Assert
            Assert.That(_testQuery.Children, Is.EqualTo(childQueries));
            Assert.That(_testQuery.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void ResultType_ShouldReturnCorrectType()
        {
            // Act
            var resultType = _testQuery.ResultType;

            // Assert
            Assert.That(resultType, Is.EqualTo(typeof(TestQueryResult)));
        }

        [Test]
        public void IsContextResolved_ShouldReturnFalseInitially()
        {
            // Act
            var isResolved = _testQuery.IsContextResolved();

            // Assert
            Assert.That(isResolved, Is.False);
        }

        [Test]
        public void ResolveQuery_ShouldSetContextResolvedToTrue()
        {
            // Arrange
            var mockContext = new Mock<IDataContext>();
            var queryResult = new Mock<IQueryResult>();

            // Act
            _testQuery.ResolveQuery(mockContext.Object, queryResult.Object);

            // Assert
            Assert.That(_testQuery.IsContextResolved(), Is.True);
            Assert.That(_testQuery.TestData, Is.EqualTo("resolved"));
        }

        [Test]
        public void ResolveQuery_WithParentResult_ShouldSetContextResolvedToTrue()
        {
            // Arrange
            var mockContext = new Mock<IDataContext>();
            var parentResult = new TestQueryResult { Data = "parent_data" };

            // Act
            _testQuery.ResolveQuery(mockContext.Object, parentResult);

            // Assert
            Assert.That(_testQuery.IsContextResolved(), Is.True);
            Assert.That(_testQuery.TestData, Is.EqualTo("resolved"));
        }

        [Test]
        public async Task Run_ShouldCallEngineExecute()
        {
            // Arrange
            var expectedResult = new TestQueryResult { Data = "engine_result" };
            _mockEngine.Setup(x => x.Execute(_testQuery))
                      .Returns(Task.FromResult<IQueryResult>(expectedResult));

            // Act
            var result = await _testQuery.Run(_mockEngine.Object);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            _mockEngine.Verify(x => x.Execute(_testQuery), Times.Once);
        }

        [Test]
        public async Task Run_WhenEngineReturnsNull_ShouldReturnNull()
        {
            // Arrange
            _mockEngine.Setup(x => x.Execute(_testQuery))
                      .Returns(Task.FromResult<IQueryResult>(null));

            // Act
            var result = await _testQuery.Run(_mockEngine.Object);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}