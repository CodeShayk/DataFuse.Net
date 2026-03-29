using DataFuse.Integration.Impl;
using DataFuse.Integration;
using DataFuse.Integration.PathMatchers;
using DataFuse.Integration.Tests.EntitySetup;
using DataFuse.Integration.Tests.EntitySetup.Entities;
using DataFuse.Integration.Tests.EntitySetup.Configuration;
using DataFuse.Integration.Tests.EntitySetup.Configuration.Queries;

namespace DataFuse.Integration.Tests.DataProvider.Tests
{
    [TestFixture]
    internal class QueryBuilderTests
    {
        private QueryBuilder<Customer> _queryBuilder;

        private IEntityConfiguration<Customer> _entitySchema;
        private ISchemaPathMatcher _schemaPathMatcher;

        [SetUp]
        public void Setup()
        {
            _entitySchema = new CustomerConfiguration();

            /*-----------------------------------------             *
             *
             * CreateSchema.For<Customer>()
                .Map<CustomerQuery, CustomerTransform>(For.Paths("customer/id", "customer/customercode", "customer/customername"),
                 customer => customer.Dependents
                    .Map<CustomerCommunicationQuery, CustomerCommunicationTransform>(For.Paths("customer/communication"))
                    .Map<CustomerOrdersQuery, CustomerOrdersTransform>(For.Paths("customer/orders"),
                        customerOrders => customerOrders.Dependents
                            .Map<CustomerOrderItemsQuery, CustomerOrderItemsTransform>(For.Paths("customer/orders/order/items")))
                )
             *
             * --------------------------------------- */

            _schemaPathMatcher = new XPathMatcher();
            _queryBuilder = new QueryBuilder<Customer>(_entitySchema, _schemaPathMatcher);
        }

        [Test]
        public void TestQueryBuilderForCorrectParentQueryList()
        {
            var context = new DataContext(new CustomerContext { SchemaPaths = new[] { "customer/customercode" }, CustomerId = 1 });

            var result = _queryBuilder.Build(context);

            Assert.That(result, Is.Not.Null);

            // returns parent query with filtered out child communication query.

            Assert.That(result.QueryDependencyDepth == 0);
            Assert.That(result.Queries.Count, Is.EqualTo(1));
            Assert.That(result.Queries.ElementAt(0).Children.Count, Is.EqualTo(0));

            var parentQuery = result.Queries.First();
            Assert.That(parentQuery.GetType() == typeof(CustomerQuery));
        }

        [Test]
        public void TestQueryBuilderForCorrectParentQueryListWithOneChildren()
        {
            var context = new DataContext(new CustomerContext { SchemaPaths = new[] { "customer/customercode", "customer/communication" } });

            var result = _queryBuilder.Build(context);

            Assert.That(result, Is.Not.Null);

            // returns parent query with filtered out child communication query.

            Assert.That(result.QueryDependencyDepth == 0);
            Assert.That(result.Queries.Count, Is.EqualTo(1));
            Assert.That(result.Queries.ElementAt(0).Children.Count, Is.EqualTo(1));

            var parentQuery = result.Queries.First();
            Assert.That(parentQuery.GetType() == typeof(CustomerQuery));

            var childQuery = parentQuery.Children.First();
            Assert.That(childQuery.GetType() == typeof(CommunicationQuery));
        }

        [Test]
        public void TestQueryBuilderForCorrectParentQueryListWithTwoChildren()
        {
            var context = new DataContext(new CustomerContext { SchemaPaths = new[] { "customer/customercode", "customer/communication", "customer/orders" } });

            var result = _queryBuilder.Build(context);

            Assert.That(result, Is.Not.Null);

            // returns parent query with filtered out children - communication & orders query.

            Assert.That(result.QueryDependencyDepth == 0);
            Assert.That(result.Queries.Count, Is.EqualTo(1));
            Assert.That(result.Queries.ElementAt(0).Children.Count, Is.EqualTo(2));

            var parentQuery = result.Queries.First();
            Assert.That(parentQuery.GetType() == typeof(CustomerQuery));

            var communicationChildQuery = parentQuery.Children.FirstOrDefault(x => x.GetType() == typeof(CommunicationQuery));
            var ordersChildQuery = parentQuery.Children.FirstOrDefault(x => x.GetType() == typeof(OrdersQuery));

            Assert.That(communicationChildQuery, Is.Not.Null);
            Assert.That(ordersChildQuery, Is.Not.Null);

            // nested child query for order item not included as order items are excluded from paths
            Assert.That(ordersChildQuery.Children.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestQueryBuilderForCorrectParentQueryListWithTwoChildrenAndOneChildFurtherNestedChildQuery()
        {
            var context = new DataContext(new CustomerContext { SchemaPaths = new[] { "customer/customercode", "customer/communication", "customer/orders", "customer/orders/order/items" } });

            var result = _queryBuilder.Build(context);

            Assert.That(result, Is.Not.Null);

            // returns parent query with filtered out children - communication & orders query.

            Assert.That(result.QueryDependencyDepth == 0);
            Assert.That(result.Queries.Count, Is.EqualTo(1));
            Assert.That(result.Queries.ElementAt(0).Children.Count, Is.EqualTo(2));

            var parentQuery = result.Queries.First();
            Assert.That(parentQuery.GetType() == typeof(CustomerQuery));

            var communicationChildQuery = parentQuery.Children.FirstOrDefault(x => x.GetType() == typeof(CommunicationQuery));
            var ordersChildQuery = parentQuery.Children.FirstOrDefault(x => x.GetType() == typeof(OrdersQuery));

            Assert.That(communicationChildQuery, Is.Not.Null);
            Assert.That(ordersChildQuery, Is.Not.Null);

            // nested child query for order item in order query children as order items are included in paths
            Assert.That(ordersChildQuery.Children.Count, Is.EqualTo(1));

            var orderItemsChildQuery = ordersChildQuery.Children.FirstOrDefault(x => x.GetType() == typeof(OrderItemsQuery));
            Assert.That(orderItemsChildQuery, Is.Not.Null);
        }
    }
}