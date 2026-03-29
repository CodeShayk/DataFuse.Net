using DataFuse.Adapters.Abstraction;
using MongoDB.Driver;
using Moq;

namespace DataFuse.Adapters.MongoDB.Tests.Unit;

[TestFixture]
public class QueryEngineTests
{
    private Mock<IMongoDatabase> _mockDatabase;
    private QueryEngine _queryEngine;

    [SetUp]
    public void Setup()
    {
        _mockDatabase = new Mock<IMongoDatabase>();
        _queryEngine = new QueryEngine(_mockDatabase.Object);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullException_WhenDatabaseIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryEngine(null!));
    }

    [Test]
    public void CanExecute_ReturnsTrue_WhenQueryIsIMongoQuery()
    {
        var mockQuery = new Mock<IMongoQuery>();
        Assert.That(_queryEngine.CanExecute(mockQuery.Object), Is.True);
    }

    [Test]
    public void CanExecute_ReturnsFalse_WhenQueryIsNotIMongoQuery()
    {
        var mockQuery = new Mock<IQuery>();
        Assert.That(_queryEngine.CanExecute(mockQuery.Object), Is.False);
    }

    [Test]
    public async Task Execute_CallsRunOnMongoQuery()
    {
        var expectedResult = new Mock<IQueryResult>();
        var mockQuery = new Mock<IMongoQuery>();
        mockQuery.Setup(q => q.Run(_mockDatabase.Object))
            .ReturnsAsync(expectedResult.Object);

        var result = await _queryEngine.Execute(mockQuery.Object);

        Assert.That(result, Is.EqualTo(expectedResult.Object));
        mockQuery.Verify(q => q.Run(_mockDatabase.Object), Times.Once);
    }

    [Test]
    public void Execute_ThrowsInvalidCastException_WhenQueryIsNotIMongoQuery()
    {
        var mockQuery = new Mock<IQuery>();
        Assert.ThrowsAsync<InvalidCastException>(() => _queryEngine.Execute(mockQuery.Object));
    }
}
