using DataFuse.Adapters.MongoDB.Tests.EntitySetup.Entities;
using DataFuse.Adapters.MongoDB.Tests.EntitySetup.EntityConfiguration;
using DataFuse.Integration;
using DataFuse.Integration.Helpers;
using DataFuse.Integration.PathMatchers;
using Microsoft.Extensions.DependencyInjection;

namespace DataFuse.Adapters.MongoDB.Tests.Integration;

public abstract class IntegrationBaseTest
{
    protected ServiceProvider _serviceProvider;

    protected void AssertAreEqual(Customer expected, Customer actual)
    {
        var actualJson = actual.ToJson();
        var expectedJson = expected.ToJson();

        Console.WriteLine("expected:");
        Console.WriteLine(expectedJson);

        Console.WriteLine("actual:");
        Console.WriteLine(actualJson);

        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }

    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.UseDataFuse(configuration => configuration
            .WithEngine(c => new QueryEngine(MongoContainerFixture.Database))
            .WithPathMatcher(c => new XPathMatcher())
            .WithEntityConfiguration<Customer>(c => new CustomerConfiguration()));

        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (_serviceProvider is IDisposable disposable)
            disposable.Dispose();
    }
}
