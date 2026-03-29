using Moq;
using DataFuse.Integration;
using DataFuse.Integration.Impl;
using DataFuse.Integration.PathMatchers;
using DataFuse.Integration.Tests.EntitySetup;
using DataFuse.Integration.Tests.EntitySetup.Entities;
using DataFuse.Integration.Tests.EntitySetup.Configuration;
using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;

namespace DataFuse.Integration.Tests.DataProvider.Tests;

[TestFixture]
internal class QueryExecutorTests
{
    private QueryExecutor _queryExecutor;
    private Mock<IQueryEngine> _queryEngine;

    [SetUp]
    public void Setup()
    {
        _queryEngine = new Mock<IQueryEngine>();
        _queryEngine.Setup(x => x.CanExecute(It.IsAny<IQuery>())).Returns(true);

        _queryExecutor = new QueryExecutor(new[] { _queryEngine.Object });
    }

    [Test]
    public async Task TestQueryExecutorToReturnWhenNoQueries()
    {
        await _queryExecutor.ExecuteAsync(new DataContext(new EntityContext()), new QueryList());

        _queryEngine.Verify(x => x.Execute(It.IsAny<IQuery>()), Times.Never());
    }

    [Test]
    public async Task TestQueryExecutorToCallEngineWhenQueriesExistForExecution()
    {
        await _queryExecutor.ExecuteAsync(new DataContext(new EntityContext()), new QueryList(new[] { new CustomerQuery() }) { });

        _queryEngine.Verify(x => x.Execute(It.IsAny<IQuery>()), Times.Once());
    }

    [Test]
    public async Task TestQueryExecutorToExecuteConfiguredQueriesInCorrectOrder()
    {
        var querList = new QueryBuilder<Customer>(new CustomerConfiguration(), new XPathMatcher())
            .Build(new DataContext(new CustomerContext()));

        await _queryExecutor.ExecuteAsync(new DataContext(new EntityContext()), querList);

        _queryEngine.Verify(x => x.Execute(It.IsAny<IQuery>()), Times.Once());
    }
}
