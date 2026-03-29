using System.Data.Common;
using DataFuse.Integration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using DataFuse.Integration.PathMatchers;
using DataFuse.Adapters.SQL;
using DataFuse.Adapters.SQL.Tests.EntitySetup.Entities;
using DataFuse.Adapters.SQL.Tests.EntitySetup.EntitySchemas;

namespace DataFuse.Adapters.SQL.Tests
{
    public class BaseTest
    {
        protected ServiceProvider _serviceProvider;
        private const string DbProviderName = "System.Data.SQLite";

        protected void AssertAreEqual(Customer expected, Customer actual)
        {
            var actualCustomer = actual.ToJson();
            var expectedCustomer = expected.ToJson();

            Console.WriteLine("expected:");
            Console.WriteLine(expectedCustomer);

            Console.WriteLine("actual:");
            Console.WriteLine(actualCustomer);

            Assert.That(actualCustomer, Is.EqualTo(expectedCustomer));
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            DbProviderFactories.RegisterFactory(DbProviderName, SqliteFactory.Instance);
            var connectionString = $"DataSource={Environment.CurrentDirectory}//Customer.db;mode=readonly;cache=shared";
            var configuration = new SQLConfiguration { ConnectionSettings = new ConnectionSettings { ConnectionString = connectionString, ProviderName = DbProviderName } };

            Console.WriteLine(connectionString);

            services.AddLogging();

            services.UseDataFuse(config => config
                .WithEngine(c => new QueryEngine(configuration))
                .WithPathMatcher(c => new XPathMatcher())
                   .WithEntityConfiguration<Customer>(c => new CustomerConfiguration()));

            // 4. Build the service provider
            _serviceProvider = services.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}