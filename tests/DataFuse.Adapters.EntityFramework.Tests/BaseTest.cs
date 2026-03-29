using Microsoft.EntityFrameworkCore;
using DataFuse.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataFuse.Adapters.Abstraction;
using DataFuse.Integration.Helpers;
using DataFuse.Integration.PathMatchers;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.Entities;
using DataFuse.Adapters.EntityFramework.Tests.EntitySetup.EntitySchemas;

namespace DataFuse.Adapters.EntityFramework.Tests
{
    public class BaseTest
    {
        protected ServiceProvider _serviceProvider;

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

            var connectionString = $"DataSource={Environment.CurrentDirectory}//Customer.db;mode=readonly;cache=shared";

            services.AddDbContextFactory<CustomerDbContext>(options =>
                    options.UseSqlite(connectionString));

            services.AddLogging();

            services.UseDataFuse(configuration => configuration
                .WithEngine(c => new QueryEngine<CustomerDbContext>(c.GetService<IDbContextFactory<CustomerDbContext>>()))
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