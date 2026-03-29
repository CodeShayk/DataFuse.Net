using Microsoft.Extensions.DependencyInjection;
using DataFuse.Integration;
using Moq;
using DataFuse.Integration.Impl;
using DataFuse.Integration.PathMatchers;

namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class ServicesExtensionsTests
    {
        private ServiceCollection _services;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
        }

        [Test]
        public void UseDataFuse_ShouldRegisterCoreServices()
        {
            var mockConfiguration = new Mock<IEntityConfiguration<TestEntity>>();
            Func<IServiceProvider, IEntityConfiguration<TestEntity>> factory = provider => mockConfiguration.Object;

            // Act
            _services.UseDataFuse(config =>
            {
                config.WithEntityConfiguration(factory);
                config.WithEngine<TestQueryEngine>();
                config.WithPathMatcher(provider => new XPathMatcher());
            });

            // Assert
            var serviceProvider = _services.BuildServiceProvider();

            // Verify core services are registered
            Assert.That(serviceProvider.GetService(typeof(IQueryBuilder<TestEntity>)), Is.Not.Null);
            Assert.That(serviceProvider.GetService(typeof(IEntityBuilder<TestEntity>)), Is.Not.Null);
            Assert.That(serviceProvider.GetService(typeof(IDataProvider<TestEntity>)), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IQueryExecutor>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<ISchemaPathMatcher>(), Is.InstanceOf<XPathMatcher>());
        }

        [Test]
        public void UseDataFuse_ShouldReturnDataFuseOptionsBuilder()
        {
            // Act
            DataFuseOptionsBuilder result = null;
            _services.UseDataFuse(c =>
            {
                c.InSilentMode();
                result = (DataFuseOptionsBuilder)c;
            });

            // Assert
            Assert.That(result, Is.InstanceOf<IDataFuseOptions>());
            Assert.That(result.Services, Is.SameAs(_services));
        }

        [Test]
        public void WithEngine_WithFactory_ShouldRegisterQueryEngine()
        {
            // Arrange
            var mockEngine = new Mock<IQueryEngine>();
            Func<IServiceProvider, IQueryEngine> factory = provider => mockEngine.Object;

            // Act
            _services.UseDataFuse(c => c
            .InSilentMode()
            .WithEngine(factory));

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var engine = serviceProvider.GetService<IQueryEngine>();
            Assert.That(engine, Is.SameAs(mockEngine.Object));
        }

        [Test]
        public void WithEngine_WithNullFactory_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _services.UseDataFuse(c => c
                                                    .InSilentMode()
                                                    .WithEngine((Func<IServiceProvider, IQueryEngine>)null)));
        }

        [Test]
        public void WithEngine_Generic_ShouldRegisterQueryEngine()
        {
            // Act
            _services.UseDataFuse(c => c
            .InSilentMode()
            .WithEngine<TestQueryEngine>());

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var engine = serviceProvider.GetService<IQueryEngine>();
            Assert.That(engine, Is.InstanceOf<TestQueryEngine>());
        }

        [Test]
        public void WithEngines_WithFactory_ShouldRegisterQueryEngineArray()
        {
            // Arrange
            var mockEngine1 = new Mock<IQueryEngine>();
            var mockEngine2 = new Mock<IQueryEngine>();
            var engines = new[] { mockEngine1.Object, mockEngine2.Object };
            Func<IServiceProvider, IQueryEngine[]> factory = provider => engines;

            // Act
            _services.UseDataFuse(c => c
            .InSilentMode()
            .WithEngines(factory));

            var serviceProvider = _services.BuildServiceProvider();
            var registeredEngines = serviceProvider.GetService<IQueryEngine[]>();
            Assert.That(registeredEngines, Is.SameAs(engines));
        }

        [Test]
        public void WithEngines_WithNullFactory_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _services.UseDataFuse(c => c
            .InSilentMode()
            .WithEngines(null)));
        }

        [Test]
        public void WithPathMatcher_ShouldReplaceDefaultPathMatcher()
        {
            // Arrange
            var mockPathMatcher = new Mock<ISchemaPathMatcher>();
            Func<IServiceProvider, ISchemaPathMatcher> factory = provider => mockPathMatcher.Object;

            // Act
            _services.UseDataFuse(c => c
                         .InSilentMode()
                         .WithPathMatcher(factory));

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var pathMatcher = serviceProvider.GetService<ISchemaPathMatcher>();
            Assert.That(pathMatcher, Is.SameAs(mockPathMatcher.Object));
        }

        [Test]
        public void WithPathMatcher_WithNullFactory_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _services.UseDataFuse(c => c
                                                    .InSilentMode()
                                                    .WithPathMatcher(null)));
        }

        [Test]
        public void WithNullSchemioConfig_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _services.UseDataFuse(null));
        }

        [Test]
        public void WithNullServices_ShouldThrow()
        {
            // Arrange
            IServiceCollection nullServices = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => nullServices.UseDataFuse(c => c.WithPathMatcher(null)));
        }

        [Test]
        public void WithNoEntityConfiguration_ShouldThrow()
        {
            // Arrange
            var mockConfiguration = new Mock<IEntityConfiguration<TestEntity>>();
            Func<IServiceProvider, IEntityConfiguration<TestEntity>> factory = provider => mockConfiguration.Object;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _services.UseDataFuse(c => c.WithEngine<TestQueryEngine>()));
        }

        [Test]
        public void WithEntityConfiguration_WithNoEngines_ShouldThrow()
        {
            // Arrange
            var mockConfiguration = new Mock<IEntityConfiguration<TestEntity>>();
            Func<IServiceProvider, IEntityConfiguration<TestEntity>> factory = provider => mockConfiguration.Object;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _services.UseDataFuse(c => c.WithEntityConfiguration(factory)));
        }

        [Test]
        public void WithEntityConfiguration_ShouldRegisterEntityConfiguration()
        {
            // Arrange
            var mockConfiguration = new Mock<IEntityConfiguration<TestEntity>>();
            Func<IServiceProvider, IEntityConfiguration<TestEntity>> factory = provider => mockConfiguration.Object;

            // Act
            _services.UseDataFuse(c => c
                            .InSilentMode()
                            .WithEntityConfiguration(factory));

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IEntityConfiguration<TestEntity>>();
            Assert.That(configuration, Is.SameAs(mockConfiguration.Object));
        }

        [Test]
        public void WithEntityConfiguration_WithNullFactory_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _services.UseDataFuse(c => c
                                                    .InSilentMode()
                                                    .WithEntityConfiguration<TestEntity>(null)));
        }

        [Test]
        public void ChainedConfiguration_ShouldWorkCorrectly()
        {
            // Arrange
            var mockEngine = new Mock<IQueryEngine>();
            var mockPathMatcher = new Mock<ISchemaPathMatcher>();
            var mockConfiguration = new Mock<IEntityConfiguration<TestEntity>>();

            // Act
            _services.UseDataFuse(c => c
                .WithEngine(provider => mockEngine.Object)
                .WithPathMatcher(provider => mockPathMatcher.Object)
                .WithEntityConfiguration<TestEntity>(provider => mockConfiguration.Object));

            // Assert
            var serviceProvider = _services.BuildServiceProvider();

            var engine = serviceProvider.GetService<IQueryEngine>();
            var pathMatcher = serviceProvider.GetService<ISchemaPathMatcher>();
            var configuration = serviceProvider.GetService<IEntityConfiguration<TestEntity>>();

            Assert.That(engine, Is.SameAs(mockEngine.Object));
            Assert.That(pathMatcher, Is.SameAs(mockPathMatcher.Object));
            Assert.That(configuration, Is.SameAs(mockConfiguration.Object));
        }

        [Test]
        public void MultipleEntityConfigurations_ShouldRegisterSeparately()
        {
            // Arrange
            var mockConfiguration1 = new Mock<IEntityConfiguration<TestEntity>>();
            var mockConfiguration2 = new Mock<IEntityConfiguration<TestChildEntity>>();

            // Act
            _services.UseDataFuse(c => c
                .InSilentMode()
                .WithEntityConfiguration<TestEntity>(provider => mockConfiguration1.Object)
                .WithEntityConfiguration<TestChildEntity>(provider => mockConfiguration2.Object));

            // Assert
            var serviceProvider = _services.BuildServiceProvider();

            var configuration1 = serviceProvider.GetService<IEntityConfiguration<TestEntity>>();
            var configuration2 = serviceProvider.GetService<IEntityConfiguration<TestChildEntity>>();

            Assert.That(configuration1, Is.SameAs(mockConfiguration1.Object));
            Assert.That(configuration2, Is.SameAs(mockConfiguration2.Object));
        }

        [Test]
        public void DataFuseOptionsBuilder_Constructor_ShouldSetServices()
        {
            // Act
            var optionsBuilder = new DataFuseOptionsBuilder(_services);

            // Assert
            Assert.That(optionsBuilder.Services, Is.SameAs(_services));
        }

        [Test]
        public void WithEngine_MultipleCallsWithGeneric_ShouldRegisterMultiple()
        {
            // Act
            _services.UseDataFuse(c => c
                .InSilentMode()
                .WithEngine<TestQueryEngine>()
                .WithEngine<TestQueryEngine>());

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var engines = serviceProvider.GetServices<IQueryEngine>();

            // Should have multiple registrations
            var engineList = System.Linq.Enumerable.ToList(engines);
            Assert.That(engineList.Count, Is.GreaterThan(1));
        }

        [Test]
        public void WithPathMatcher_ShouldRemoveExistingRegistrations()
        {
            // Arrange
            var mockPathMatcher = new Mock<ISchemaPathMatcher>();

            // Act
            _services.UseDataFuse(c => c
                            .InSilentMode()
                            .WithPathMatcher(provider => mockPathMatcher.Object));

            // Assert
            var pathMatcherDescriptors = System.Linq.Enumerable.Where(_services, s => s.ServiceType == typeof(ISchemaPathMatcher));
            var descriptorList = System.Linq.Enumerable.ToList(pathMatcherDescriptors);

            // Should only have one registration after replacement
            Assert.That(descriptorList.Count, Is.EqualTo(1));

            var serviceProvider = _services.BuildServiceProvider();
            var pathMatcher = serviceProvider.GetService<ISchemaPathMatcher>();
            Assert.That(pathMatcher, Is.SameAs(mockPathMatcher.Object));
        }

        [Test]
        public void ComplexConfiguration_ShouldResolveAllDependencies()
        {
            // Arrange
            var configuration = new TestEntityConfiguration();
            var engine = new TestQueryEngine();

            // Act
            _services.UseDataFuse(config => config
                .WithEngine<TestQueryEngine>()
                .WithEntityConfiguration<TestEntity>(provider => configuration)
                );

            // Assert
            var serviceProvider = _services.BuildServiceProvider();

            // Should be able to resolve DataProvider with all dependencies
            var dataProvider = serviceProvider.GetService<IDataProvider<TestEntity>>();
            Assert.That(dataProvider, Is.Not.Null);
            Assert.That(dataProvider, Is.InstanceOf<DataProvider<TestEntity>>());
        }
    }
}