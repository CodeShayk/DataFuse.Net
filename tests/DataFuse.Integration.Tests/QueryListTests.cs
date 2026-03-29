namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class QueryListTests
    {
        [Test]
        public void DefaultConstructor_ShouldCreateEmptyQueryList()
        {
            // Act
            var queryList = new QueryList();

            // Assert
            Assert.That(queryList.Queries, Is.Not.Null);
            Assert.That(queryList.Count(), Is.EqualTo(0));
            Assert.That(queryList.IsEmpty(), Is.True);
        }

        [Test]
        public void Constructor_WithCollection_ShouldInitializeWithQueries()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery(), new TestChildQuery() };

            // Act
            var queryList = new QueryList(queries);

            // Assert
            Assert.That(queryList.Count(), Is.EqualTo(2));
            Assert.That(queryList.IsEmpty(), Is.False);
            Assert.That(queryList.Queries.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => new QueryList(null));
        }

        [Test]
        public void Constructor_WithEmptyCollection_ShouldCreateEmptyQueryList()
        {
            // Arrange
            var emptyQueries = new List<IQuery>();

            // Act
            var queryList = new QueryList(emptyQueries);

            // Assert
            Assert.That(queryList.Count(), Is.EqualTo(0));
            Assert.That(queryList.IsEmpty(), Is.True);
        }

        [Test]
        public void QueryDependencyDepth_ShouldBeSettableAndGettable()
        {
            // Arrange
            var queryList = new QueryList();

            // Act
            queryList.QueryDependencyDepth = 5;

            // Assert
            Assert.That(queryList.QueryDependencyDepth, Is.EqualTo(5));
        }

        [Test]
        public void Queries_ShouldReturnReadOnlyEnumerable()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery() };
            var queryList = new QueryList(queries);

            // Act
            var result = queryList.Queries;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetByType_ShouldReturnQueriesOfSpecificType()
        {
            // Arrange
            var queries = new List<IQuery>
            {
                new TestQuery(),
                new TestChildQuery(),
                new TestQuery()
            };
            var queryList = new QueryList(queries);

            // Act
            var testQueries = queryList.GetByType<TestQuery>();

            // Assert
            Assert.That(testQueries.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetByType_WithNoMatchingType_ShouldReturnEmptyQueryList()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery() };
            var queryList = new QueryList(queries);

            // Act
            var result = queryList.GetByType<TestChildQuery>();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
            Assert.That(result.IsEmpty(), Is.True);
        }

        [Test]
        public void As_ShouldReturnListOfSpecificType()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery(), new TestChildQuery() };
            var queryList = new QueryList(queries);

            // Act
            var result = queryList.As<IQuery>();

            // Assert
            Assert.That(result, Is.InstanceOf<List<IQuery>>());
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetChildrenQueries_WithQueriesHavingChildren_ShouldReturnChildrenQueries()
        {
            // Arrange
            var childQuery1 = new TestChildQuery();
            var childQuery2 = new TestChildQuery();

            var parentQuery = new TestQuery
            {
                Children = new List<IQuery> { childQuery1, childQuery2 }
            };

            var queryList = new QueryList(new List<IQuery> { parentQuery });

            // Act
            var childrenQueries = queryList.GetChildrenQueries();

            // Assert
            Assert.That(childrenQueries, Is.Not.Null);
            Assert.That(childrenQueries.Count, Is.EqualTo(1));
            Assert.That(childrenQueries[0].ParentQueryResultType, Is.EqualTo(typeof(TestQueryResult)));
            Assert.That(childrenQueries[0].Queries.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetChildrenQueries_WithNoChildren_ShouldReturnEmptyList()
        {
            // Arrange
            var parentQuery = new TestQuery { Children = null };
            var queryList = new QueryList(new List<IQuery> { parentQuery });

            // Act
            var childrenQueries = queryList.GetChildrenQueries();

            // Assert
            Assert.That(childrenQueries, Is.Not.Null);
            Assert.That(childrenQueries.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetChildrenQueries_WithEmptyChildren_ShouldReturnEmptyList()
        {
            // Arrange
            var parentQuery = new TestQuery { Children = new List<IQuery>() };
            var queryList = new QueryList(new List<IQuery> { parentQuery });

            // Act
            var childrenQueries = queryList.GetChildrenQueries();

            // Assert
            Assert.That(childrenQueries, Is.Not.Null);
            Assert.That(childrenQueries.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetChildrenQueries_ShouldGroupByParentResultType()
        {
            // Arrange
            var child1 = new TestChildQuery();
            var child2 = new TestChildQuery();

            var parent1 = new TestQuery { Children = new List<IQuery> { child1 } };
            var parent2 = new TestQuery { Children = new List<IQuery> { child2 } };

            var queryList = new QueryList(new List<IQuery> { parent1, parent2 });

            // Act
            var childrenQueries = queryList.GetChildrenQueries();

            // Assert
            Assert.That(childrenQueries.Count, Is.EqualTo(2)); // Should be grouped by result type
            Assert.That(childrenQueries[0].ParentQueryResultType, Is.EqualTo(typeof(TestQueryResult)));
            Assert.That(childrenQueries[0].Queries.Count, Is.EqualTo(1)); // Should contain both children
        }

        [Test]
        public void GetChildrenQueries_ShouldRemoveDuplicateQueries()
        {
            // Arrange
            var duplicateChild = new TestChildQuery();

            var parent = new TestQuery
            {
                Children = new List<IQuery> { duplicateChild, duplicateChild }
            };

            var queryList = new QueryList(new List<IQuery> { parent });

            // Act
            var childrenQueries = queryList.GetChildrenQueries();

            // Assert
            Assert.That(childrenQueries.Count, Is.EqualTo(1));
            Assert.That(childrenQueries[0].Queries.Count, Is.EqualTo(1)); // Duplicates should be removed
        }

        [Test]
        public void Count_ShouldReturnCorrectCount()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery(), new TestChildQuery(), new TestQuery() };
            var queryList = new QueryList(queries);

            // Act
            var count = queryList.Count();

            // Assert
            Assert.That(count, Is.EqualTo(3));
        }

        [Test]
        public void IsEmpty_WithQueries_ShouldReturnFalse()
        {
            // Arrange
            var queries = new List<IQuery> { new TestQuery() };
            var queryList = new QueryList(queries);

            // Act
            var isEmpty = queryList.IsEmpty();

            // Assert
            Assert.That(isEmpty, Is.False);
        }

        [Test]
        public void IsEmpty_WithoutQueries_ShouldReturnTrue()
        {
            // Arrange
            var queryList = new QueryList();

            // Act
            var isEmpty = queryList.IsEmpty();

            // Assert
            Assert.That(isEmpty, Is.True);
        }

        [Test]
        public void AddRange_ShouldAddQueriesToList()
        {
            // Arrange
            var queryList = new QueryList();
            var newQueries = new List<IQuery> { new TestQuery(), new TestChildQuery() };

            // Act
            queryList.AddRange(newQueries);

            // Assert
            Assert.That(queryList.Count(), Is.EqualTo(2));
            Assert.That(queryList.IsEmpty(), Is.False);
        }

        [Test]
        public void AddRange_WithExistingQueries_ShouldAppendToList()
        {
            // Arrange
            var initialQueries = new List<IQuery> { new TestQuery() };
            var queryList = new QueryList(initialQueries);
            var additionalQueries = new List<IQuery> { new TestChildQuery() };

            // Act
            queryList.AddRange(additionalQueries);

            // Assert
            Assert.That(queryList.Count(), Is.EqualTo(2));
        }

        [Test]
        public void AddRange_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            var queryList = new QueryList();

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => queryList.AddRange(null));
        }

        [Test]
        public void AddRange_WithEmptyCollection_ShouldNotChangeCount()
        {
            // Arrange
            var queryList = new QueryList(new List<IQuery> { new TestQuery() });
            var emptyQueries = new List<IQuery>();

            // Act
            queryList.AddRange(emptyQueries);

            // Assert
            Assert.That(queryList.Count(), Is.EqualTo(1));
        }

        [Test]
        public void QueryList_ShouldImplementIQueryList()
        {
            // Arrange
            var queryList = new QueryList();

            // Assert
            Assert.That(queryList, Is.InstanceOf<IQueryList>());
        }
    }
}