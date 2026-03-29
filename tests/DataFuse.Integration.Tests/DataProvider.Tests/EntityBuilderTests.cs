using DataFuse.Integration.Impl;
using DataFuse.Integration;
using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;
using DataFuse.Integration.Tests.EntitySetup.Entities;

namespace DataFuse.Integration.Tests.DataProvider.Tests
{
    [TestFixture]
    internal class EntityBuilderTests
    {
        private EntityBuilder<Customer> _entityBuilder;
        private IEntityConfiguration<Customer> _entitySchema;

        public class Invocations
        {
            public Invocations(Type result, int invocationCount, int preInvocationCount, int postInvocationCount)
            {
                Result = result;
                InvocationCount = invocationCount;
                PreInvocationCount = preInvocationCount;
                PostInvocationCount = postInvocationCount;
            }

            public Type Result { get; set; }
            public int InvocationCount { get; set; }
            public int PreInvocationCount { get; set; }
            public int PostInvocationCount { get; set; }
        }

        private static List<Invocations> TransformerInvocations;

        [SetUp]
        public void Setup()
        {
            _entitySchema = new MockCustomerSchema();
            _entityBuilder = new EntityBuilder<Customer>(_entitySchema);
            TransformerInvocations = new List<Invocations>();
        }

        [Test]
        public void TestTransformExecutorForCorrectExecutionOfConfiguredTransforms()
        {
            var queryList = new List<IQueryResult>
            {
                new CustomerRecord{Id = 123, CustomerCode= "ABC", CustomerName="Ninja Labs"},
                new CommunicationRecord{Id = 123, Email = "ninja@labs.com", Telephone = "0212345689"},
                new CollectionResult<OrderRecord>(),
                new CollectionResult<OrderItemRecord>()
            };

            var entity = _entityBuilder.Build(new DataContext(new EntityContext()), queryList);

            var customerTransforms = TransformerInvocations.Where(x => x.Result == typeof(CustomerRecord));
            Assert.That(customerTransforms.Count() == 1);
            Assert.That(customerTransforms.ElementAt(0).PreInvocationCount == 1);
            Assert.That(customerTransforms.ElementAt(0).InvocationCount == 1);
            Assert.That(customerTransforms.ElementAt(0).PostInvocationCount == 1);

            // Assert that the communication transform is invoked twice due to the repeat in PostTransform
            var communicationTransforms = TransformerInvocations.Where(x => x.Result == typeof(CommunicationRecord));
            Assert.That(communicationTransforms.Count() == 1);
            Assert.That(communicationTransforms.ElementAt(0).PreInvocationCount == 2);
            Assert.That(communicationTransforms.ElementAt(0).InvocationCount == 2);
            Assert.That(communicationTransforms.ElementAt(0).PostInvocationCount == 2);

            var orderCollectionTransforms = TransformerInvocations.Where(x => x.Result == typeof(CollectionResult<OrderRecord>));
            Assert.That(orderCollectionTransforms.Count() == 1);
            Assert.That(orderCollectionTransforms.ElementAt(0).PreInvocationCount == 1);
            Assert.That(orderCollectionTransforms.ElementAt(0).InvocationCount == 1);
            Assert.That(orderCollectionTransforms.ElementAt(0).PostInvocationCount == 1);

            // Assert that the order items transform is not invoked due to cancellation in PreTransform
            var orderItemsCollectionTransforms = TransformerInvocations.Where(x => x.Result == typeof(CollectionResult<OrderItemRecord>));
            Assert.That(orderItemsCollectionTransforms.Count() == 1);
            Assert.That(orderItemsCollectionTransforms.ElementAt(0).PreInvocationCount == 1);
            Assert.That(orderItemsCollectionTransforms.ElementAt(0).InvocationCount == 0);
            Assert.That(orderItemsCollectionTransforms.ElementAt(0).PostInvocationCount == 0);

            Assert.That(entity, Is.Not.Null);
        }

        public class MockTransform<TQueryResult, TEntity> : BaseTransformer<TQueryResult, TEntity>
        where TEntity : IEntity
        where TQueryResult : IQueryResult
        {
            public override void Transform(TQueryResult queryResult, TEntity entity)
            {
                var invocations = TransformerInvocations.Where(x => x.Result == queryResult.GetType());

                if (!invocations.Any())
                    return;

                var transformInvocations = invocations
                        .Select(x => new Invocations(x.Result, x.InvocationCount + 1, x.PreInvocationCount, x.PostInvocationCount)).ToList();

                TransformerInvocations.RemoveAll(x => x.Result == queryResult.GetType());
                TransformerInvocations.AddRange(transformInvocations);
            }

            public override void PreTransform(PreTransformContext context)
            {
                var invocation = TransformerInvocations.FirstOrDefault(x => x.Result == context.QueryResult.GetType());

                if (invocation != null)
                {
                    TransformerInvocations.Remove(invocation);
                    TransformerInvocations.Add(new Invocations(invocation.Result, invocation.InvocationCount, invocation.PreInvocationCount + 1, invocation.PostInvocationCount));
                }
                else
                {
                    TransformerInvocations.Add(new Invocations(context.QueryResult.GetType(), 0, 1, 0));
                }

                if (context.QueryResult is CollectionResult<OrderItemRecord> orderItems)
                {
                    context.Cancel();
                }
            }

            public override void PostTransform(PostTransformContext context)
            {
                var invocations = TransformerInvocations.Where(x => x.Result == context.QueryResult.GetType());

                if (!invocations.Any())
                    return;

                var transformInvocations = invocations
                        .Select(x => new Invocations(x.Result, x.InvocationCount, x.PreInvocationCount, x.PostInvocationCount + 1)).ToList();

                TransformerInvocations.RemoveAll(x => x.Result == context.QueryResult.GetType());
                TransformerInvocations.AddRange(transformInvocations);

                if (context.QueryResult is CommunicationRecord communication)
                {
                    context.Repeat();
                }
            }
        }

        internal class MockCustomerSchema : EntityConfiguration<Customer>
        {
            public override IEnumerable<Mapping<Customer, IQueryResult>> GetSchema()
            {
                return CreateSchema.For<Customer>()
                    .Map<CustomerQuery, MockTransform<CustomerRecord, Customer>>(For.Paths("customer/id", "customer/customercode", "customer/customername"),
                     customer => customer.Dependents
                        .Map<CommunicationQuery, MockTransform<CommunicationRecord, Customer>>(For.Paths("customer/communication"))
                        .Map<OrdersQuery, MockTransform<CollectionResult<OrderRecord>, Customer>>(For.Paths("customer/orders"),
                            customerOrders => customerOrders.Dependents
                                .Map<OrderItemsQuery, MockTransform<CollectionResult<OrderItemRecord>, Customer>>(For.Paths("customer/orders/order/items")))
                    ).End();
            }
        }
    }

    internal class EntityContext : IEntityRequest
    {
        public int CustomerId { get; set; }
        public string[] SchemaPaths { get; set; }
    }
}