using Microsoft.Extensions.Logging;
using DataFuse.Integration;
using Moq;
using DataFuse.Integration.Impl;

namespace DataFuse.Integration.Tests.Impl;

[TestFixture]
public class DataProviderTests
{
    private Mock<ILogger<IDataProvider<TestEntity>>> _mockLogger;
    private Mock<IQueryBuilder<TestEntity>> _mockQueryBuilder;
    private Mock<IQueryExecutor> _mockQueryExecutor;
    private Mock<IEntityBuilder<TestEntity>> _mockEntityBuilder;
    private Mock<IEntityConfiguration<TestEntity>> _mockEntityConfiguration;
    private Mock<ISchemaPathMatcher> _mockPathMatcher;
    private Mock<IQueryEngine> _mockQueryEngine;
    private DataProvider<TestEntity> _dataProvider;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<IDataProvider<TestEntity>>>();
        _mockQueryBuilder = new Mock<IQueryBuilder<TestEntity>>();
        _mockQueryExecutor = new Mock<IQueryExecutor>();
        _mockEntityBuilder = new Mock<IEntityBuilder<TestEntity>>();
        _mockEntityConfiguration = new Mock<IEntityConfiguration<TestEntity>>();
        _mockPathMatcher = new Mock<ISchemaPathMatcher>();
        _mockQueryEngine = new Mock<IQueryEngine>();

        _dataProvider = new DataProvider<TestEntity>(
            _mockLogger.Object,
            _mockQueryBuilder.Object,
            _mockQueryExecutor.Object,
            _mockEntityBuilder.Object);
    }

    [Test]
    public void Constructor_WithEntityConfiguration_ShouldCreateDataProvider()
    {
        // Act
        var provider = new DataProvider<TestEntity>(_mockEntityConfiguration.Object, _mockQueryEngine.Object);

        // Assert
        Assert.That(provider, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithEntityConfigurationAndPathMatcher_ShouldCreateDataProvider()
    {
        // Act
        var provider = new DataProvider<TestEntity>(
            _mockLogger.Object,
            _mockEntityConfiguration.Object,
            _mockPathMatcher.Object,
            _mockQueryEngine.Object);

        // Assert
        Assert.That(provider, Is.Not.Null);
    }

    [Test]
    public async Task GetData_WithValidRequest_ShouldReturnEntity()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult> { new TestQueryResult { Data = "test" } };
        var expectedEntity = new TestEntity { Name = "test", Value = 4 };

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), queries.Object)).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), results)).Returns(expectedEntity);

        // Act
        var result = await _dataProvider.GetDataAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedEntity));
        _mockQueryBuilder.Verify(x => x.Build(It.IsAny<IDataContext>()), Times.Once);
        _mockQueryExecutor.Verify(x => x.ExecuteAsync(It.IsAny<IDataContext>(), queries.Object), Times.Once);
        _mockEntityBuilder.Verify(x => x.Build(It.IsAny<IDataContext>(), results), Times.Once);
    }

    [Test]
    public async Task GetData_ShouldCreateDataContextWithRequest()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult>();
        var expectedEntity = new TestEntity();

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>())).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), It.IsAny<IList<IQueryResult>>())).Returns(expectedEntity);

        // Act
        await _dataProvider.GetDataAsync(request);

        // Assert
        _mockQueryBuilder.Verify(x => x.Build(It.Is<IDataContext>(ctx => ctx.Request == request)), Times.Once);
    }

    [Test]
    public async Task GetData_WithNullRequest_ShouldStillWork()
    {
        // Arrange
        IEntityRequest request = null;
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult>();
        var expectedEntity = new TestEntity();

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>())).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), It.IsAny<IList<IQueryResult>>())).Returns(expectedEntity);

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _dataProvider.GetDataAsync(request));
    }

    [Test]
    public async Task GetData_ShouldLogExecutionTimes()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult>();
        var expectedEntity = new TestEntity();

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>())).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), It.IsAny<IList<IQueryResult>>())).Returns(expectedEntity);

        // Act
        await _dataProvider.GetDataAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Query builder executed")),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Query executor executed")),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transform executor executed")),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);
    }

    [Test]
    public void GetData_WhenQueryBuilderThrows_ShouldPropagateException()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var expectedException = new System.Exception("Query builder failed");

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Throws(expectedException);

        // Act & Assert
        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _dataProvider.GetDataAsync(request));
        Assert.That(ex, Is.EqualTo(expectedException));
    }

    [Test]
    public void GetData_WhenQueryExecutorThrows_ShouldPropagateException()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var expectedException = new System.Exception("Query executor failed");

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>())).ThrowsAsync(expectedException);

        // Act & Assert
        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _dataProvider.GetDataAsync(request));
        Assert.That(ex, Is.EqualTo(expectedException));
    }

    [Test]
    public void GetData_WhenEntityBuilderThrows_ShouldPropagateException()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult>();
        var expectedException = new System.Exception("Entity builder failed");

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>())).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), It.IsAny<IList<IQueryResult>>())).Throws(expectedException);

        // Act & Assert
        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _dataProvider.GetDataAsync(request));
        Assert.That(ex, Is.EqualTo(expectedException));
    }

    [Test]
    public async Task GetData_WithComplexScenario_ShouldWorkCorrectly()
    {
        // Arrange
        var request = new TestEntityRequest
        {
            SchemaPaths = new[] { "/root/test", "/root/child" }
        };
        var queries = new Mock<IQueryList>();
        var results = new List<IQueryResult>
        {
            new TestQueryResult { Data = "parent_data" },
            new TestChildQueryResult { ChildData = "child_data" }
        };
        var expectedEntity = new TestEntity { Name = "combined", Value = 123 };

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), queries.Object)).ReturnsAsync(results);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), results)).Returns(expectedEntity);

        // Act
        var result = await _dataProvider.GetDataAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedEntity));
        Assert.That(result.Name, Is.EqualTo("combined"));
        Assert.That(result.Value, Is.EqualTo(123));
    }

    [Test]
    public void DataProvider_ShouldImplementIDataProvider()
    {
        // Assert
        Assert.That(_dataProvider, Is.InstanceOf<IDataProvider<TestEntity>>());
    }

    [Test]
    public async Task GetData_WithEmptyResults_ShouldReturnEntityFromBuilder()
    {
        // Arrange
        var request = new TestEntityRequest { SchemaPaths = new[] { "/root/test" } };
        var queries = new Mock<IQueryList>();
        var emptyResults = new List<IQueryResult>();
        var expectedEntity = new TestEntity { Name = "empty_result", Value = 0 };

        _mockQueryBuilder.Setup(x => x.Build(It.IsAny<IDataContext>())).Returns(queries.Object);
        _mockQueryExecutor.Setup(x => x.ExecuteAsync(It.IsAny<IDataContext>(), queries.Object)).ReturnsAsync(emptyResults);
        _mockEntityBuilder.Setup(x => x.Build(It.IsAny<IDataContext>(), emptyResults)).Returns(expectedEntity);

        // Act
        var result = await _dataProvider.GetDataAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedEntity));
        _mockEntityBuilder.Verify(x => x.Build(It.IsAny<IDataContext>(), emptyResults), Times.Once);
    }
}
