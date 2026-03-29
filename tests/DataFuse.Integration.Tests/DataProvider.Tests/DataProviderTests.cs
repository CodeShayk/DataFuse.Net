using Microsoft.Extensions.Logging;
using DataFuse.Integration;
using Moq;
using DataFuse.Integration.Impl;
using DataFuse.Integration.Tests.EntitySetup;
using DataFuse.Integration.Tests.EntitySetup.Entities;

namespace DataFuse.Integration.Tests.DataProvider.Tests;

[TestFixture]
internal class DataProviderTests
{
    private DataProvider<Customer> _provider;
    private Mock<ILogger<DataProvider<Customer>>> _logger;
    private Mock<IQueryBuilder<Customer>> _queryBuilder;
    private Mock<IQueryExecutor> _queryExecutor;
    private Mock<IEntityBuilder<Customer>> _entityBuilder;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<DataProvider<Customer>>>();
        _queryBuilder = new Mock<IQueryBuilder<Customer>>();
        _queryExecutor = new Mock<IQueryExecutor>();
        _entityBuilder = new Mock<IEntityBuilder<Customer>>();

        _provider = new DataProvider<Customer>(_logger.Object, _queryBuilder.Object, _queryExecutor.Object, _entityBuilder.Object);
    }

    [Test]
    public async Task TestDataProvider()
    {
        var context = new CustomerContext { CustomerId = 1 };

        await _provider.GetDataAsync(context);

        _queryBuilder.Verify(x => x.Build(It.IsAny<IDataContext>()), Times.Once);
        _queryExecutor.Verify(x => x.ExecuteAsync(It.IsAny<IDataContext>(), It.IsAny<IQueryList>()), Times.Once);
        _entityBuilder.Verify(x => x.Build(It.IsAny<IDataContext>(), It.IsAny<List<IQueryResult>>()), Times.Once);
    }
}
