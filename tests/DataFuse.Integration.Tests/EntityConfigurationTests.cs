namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class EntityConfigurationTests
    {
        private TestEntityConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new TestEntityConfiguration();
        }

        [Test]
        public void Constructor_ShouldInitializeMappings()
        {
            // Act
            var configuration = new TestEntityConfiguration();

            // Assert
            Assert.That(configuration.Mappings, Is.Not.Null);
        }

        [Test]
        public void Mappings_ShouldReturnConfiguredMappings()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Mappings_ShouldContainCorrectQueryTypes()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            var hasTestQuery = mappings.Any(m => m.Query is TestQuery);
            var hasTestChildQuery = mappings.Any(m => m.Query is TestChildQuery);

            Assert.That(hasTestQuery, Is.True);
            Assert.That(hasTestChildQuery, Is.True);
        }

        [Test]
        public void Mappings_ShouldContainCorrectTransformerTypes()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            var hasTestTransformer = mappings.Any(m => m.Transformer is TestTransformer);
            var hasTestChildTransformer = mappings.Any(m => m.Transformer is TestChildTransformer);

            Assert.That(hasTestTransformer, Is.True);
            Assert.That(hasTestChildTransformer, Is.True);
        }

        [Test]
        public void Mappings_ShouldHaveCorrectSchemaPaths()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            var rootMapping = mappings.FirstOrDefault(m => m.Query is TestQuery);
            var childMapping = mappings.FirstOrDefault(m => m.Query is TestChildQuery);

            Assert.That(rootMapping, Is.Not.Null);
            Assert.That(rootMapping.SchemaPaths, Is.Not.Null);
            Assert.That(rootMapping.SchemaPaths.Paths, Contains.Item("/root/test"));

            Assert.That(childMapping, Is.Not.Null);
            Assert.That(childMapping.SchemaPaths, Is.Not.Null);
        }

        [Test]
        public void Mappings_ShouldHaveCorrectDependencyStructure()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            var rootMapping = mappings.FirstOrDefault(m => m.Query is TestQuery && m.DependentOn == null);
            var dependentMappings = mappings.Where(m => m.DependentOn != null).ToList();

            Assert.That(rootMapping, Is.Not.Null);
            Assert.That(dependentMappings, Is.Not.Empty);

            foreach (var dependent in dependentMappings)
            {
                Assert.That(dependent.DependentOn, Is.Not.Null);
            }
        }

        [Test]
        public void Mappings_ShouldHaveCorrectOrdering()
        {
            // Act
            var mappings = _configuration.Mappings.ToList();

            // Assert
            var orders = mappings.Select(m => m.Order).Distinct().OrderBy(o => o).ToList();

            Assert.That(orders, Is.Not.Empty);
            Assert.That(orders[0], Is.EqualTo(1)); // Should start with order 1

            // Orders should be sequential (allowing for gaps, but generally increasing)
            foreach (var order in orders)
            {
                Assert.That(order, Is.GreaterThan(0));
            }
        }

        [Test]
        public void GetSchema_ShouldReturnSameMappingsAsProperty()
        {
            // Act
            var schemaFromMethod = _configuration.GetSchema().ToList();
            var schemaFromProperty = _configuration.Mappings.ToList();

            // Assert
            Assert.That(schemaFromMethod.Count, Is.EqualTo(schemaFromProperty.Count));

            for (int i = 0; i < schemaFromMethod.Count; i++)
            {
                Assert.That(schemaFromMethod[i].Query.GetType(), Is.EqualTo(schemaFromProperty[i].Query.GetType()));
                Assert.That(schemaFromMethod[i].Transformer.GetType(), Is.EqualTo(schemaFromProperty[i].Transformer.GetType()));
                Assert.That(schemaFromMethod[i].Order, Is.EqualTo(schemaFromProperty[i].Order));
            }
        }

        [Test]
        public void EntityConfiguration_ShouldImplementInterface()
        {
            // Assert
            Assert.That(_configuration, Is.InstanceOf<IEntityConfiguration<TestEntity>>());
        }

        [Test]
        public void GetSchema_ShouldBeCalledOnlyOnce()
        {
            // Arrange
            var configurationSpy = new TestEntityConfigurationSpy();

            // Act
            var mappings1 = configurationSpy.Mappings.ToList();
            var mappings2 = configurationSpy.Mappings.ToList();

            // Assert
            Assert.That(configurationSpy.GetSchemaCallCount, Is.EqualTo(1));
        }

        // Test implementation that tracks method calls
        private class TestEntityConfigurationSpy : EntityConfiguration<TestEntity>
        {
            public int GetSchemaCallCount { get; private set; }

            public override System.Collections.Generic.IEnumerable<Mapping<TestEntity, IQueryResult>> GetSchema()
            {
                GetSchemaCallCount++;
                return CreateSchema.For<TestEntity>()
                    .Map<TestQuery, TestTransformer>(For.Paths("/root/spy"))
                    .End();
            }
        }

        [Test]
        public void EmptyEntityConfiguration_ShouldReturnEmptyMappings()
        {
            // Arrange
            var emptyConfiguration = new EmptyEntityConfiguration();

            // Act
            var mappings = emptyConfiguration.Mappings.ToList();

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Count, Is.EqualTo(0));
        }

        private class EmptyEntityConfiguration : EntityConfiguration<TestEntity>
        {
            public override System.Collections.Generic.IEnumerable<Mapping<TestEntity, IQueryResult>> GetSchema()
            {
                return System.Linq.Enumerable.Empty<Mapping<TestEntity, IQueryResult>>();
            }
        }
    }
}