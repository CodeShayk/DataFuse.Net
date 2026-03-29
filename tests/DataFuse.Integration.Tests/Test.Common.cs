namespace DataFuse.Integration.Tests
{
    // Test entities
    public class TestEntity : IEntity
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class TestChildEntity : IEntity
    {
        public string Description { get; set; }
    }

    // Test query results
    public class TestQueryResult : IQueryResult
    {
        public string Data { get; set; }
    }

    public class TestChildQueryResult : IQueryResult
    {
        public string ChildData { get; set; }
    }

    public class TestPolymorphicResult : IPolymorphicResult
    {
        public string PolymorphicData { get; set; }
    }

    // Test queries
    public class TestQuery : BaseQuery<TestQueryResult>
    {
        private bool _isContextResolved;
        public string TestData { get; set; }

        public override bool IsContextResolved()
        {
            return _isContextResolved;
        }

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            _isContextResolved = true;
            TestData = "resolved";
        }
    }

    public class TestChildQuery : BaseQuery<TestChildQueryResult>
    {
        private bool _isContextResolved;
        public string ChildTestData { get; set; }

        public override bool IsContextResolved()
        {
            return _isContextResolved;
        }

        public override void ResolveQuery(IDataContext context, IQueryResult parentQueryResult)
        {
            _isContextResolved = true;
            ChildTestData = parentQueryResult != null ? "resolved_with_parent" : "resolved_without_parent";
        }
    }

    // Test transformers
    public class TestTransformer : BaseTransformer<TestQueryResult, TestEntity>
    {
        public override void Transform(TestQueryResult queryResult, TestEntity entity)
        {
            entity.Name = queryResult.Data;
            entity.Value = queryResult.Data?.Length ?? 0;
        }
    }

    public class TestChildTransformer : BaseTransformer<TestChildQueryResult, TestEntity>
    {
        public override void Transform(TestChildQueryResult queryResult, TestEntity entity)
        {
            entity.Name = entity.Name + "_" + queryResult.ChildData;
        }
    }

    // Test query engine
    public class TestQueryEngine : IQueryEngine
    {
        public bool CanExecute(IQuery query)
        {
            return query is TestQuery || query is TestChildQuery;
        }

        public async Task<IQueryResult> Execute(IQuery query)
        {
            await Task.Delay(1); // Simulate async operation

            if (query is TestQuery)
                return new TestQueryResult { Data = "test_data" };

            if (query is TestChildQuery)
                return new TestChildQueryResult { ChildData = "child_data" };

            return null;
        }
    }

    // Test entity configuration
    public class TestEntityConfiguration : EntityConfiguration<TestEntity>
    {
        public override IEnumerable<Mapping<TestEntity, IQueryResult>> GetSchema()
        {
            return CreateSchema.For<TestEntity>()
                .Map<TestQuery, TestTransformer>(For.Paths("/root/test"))
                .Map<TestChildQuery, TestChildTransformer>(
                    For.Paths("/root/test/child"),
                    deps => deps.Dependents
                        .Map<TestChildQuery, TestChildTransformer>(For.Paths("/root/test/child/nested"))
                )
                .End();
        }
    }

    // Test entity request
    public class TestEntityRequest : IEntityRequest
    {
        public string[] SchemaPaths { get; set; }
    }

    // Test entity context validator
    public class TestEntityContextValidator : IEntityContextValidator
    {
        public void Validate(IEntityRequest context)
        {
            if (context?.SchemaPaths == null || context.SchemaPaths.Length == 0)
                throw new ArgumentException("SchemaPaths cannot be null or empty");
        }
    }
}