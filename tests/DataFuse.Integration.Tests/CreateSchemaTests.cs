namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class CreateSchemaTests
    {
        [Test]
        public void For_ShouldReturnMappingsWithOrderOne()
        {
            // Act
            var mappings = CreateSchema.For<TestEntity>();

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Order, Is.EqualTo(1));
            Assert.That(mappings, Is.InstanceOf<IMappings<TestEntity, IQueryResult>>());
        }

        [Test]
        public void Paths_ShouldReturnSchemaPathsWithCorrectPaths()
        {
            // Arrange
            var paths = new[] { "/root/test", "/root/child" };

            // Act
            var schemaPaths = For.Paths(paths);

            // Assert
            Assert.That(schemaPaths, Is.Not.Null);
            Assert.That(schemaPaths.Paths, Is.EqualTo(paths));
            Assert.That(schemaPaths, Is.InstanceOf<ISchemaPaths>());
        }

        [Test]
        public void Paths_WithEmptyArray_ShouldReturnEmptyPaths()
        {
            // Arrange
            var paths = new string[0];

            // Act
            var schemaPaths = For.Paths(paths);

            // Assert
            Assert.That(schemaPaths, Is.Not.Null);
            Assert.That(schemaPaths.Paths, Is.EqualTo(paths));
            Assert.That(schemaPaths.Paths.Length, Is.EqualTo(0));
        }

        [Test]
        public void Paths_WithNullArray_ShouldReturnNullPaths()
        {
            // Act
            var schemaPaths = For.Paths(null);

            // Assert
            Assert.That(schemaPaths, Is.Not.Null);
            Assert.That(schemaPaths.Paths, Is.Null);
        }

        [Test]
        public void Paths_WithSinglePath_ShouldReturnSinglePath()
        {
            // Arrange
            var path = "/root/single";

            // Act
            var schemaPaths = For.Paths(path);

            // Assert
            Assert.That(schemaPaths, Is.Not.Null);
            Assert.That(schemaPaths.Paths, Is.Not.Null);
            Assert.That(schemaPaths.Paths.Length, Is.EqualTo(1));
            Assert.That(schemaPaths.Paths[0], Is.EqualTo(path));
        }

        [Test]
        public void Map_WithoutDependents_ShouldCreateMappingCorrectly()
        {
            // Arrange
            var paths = For.Paths("/root/test");

            // Act
            var result = CreateSchema.For<TestEntity>()
                .Map<TestQuery, TestTransformer>(paths);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IMapOrComplete<TestEntity, IQueryResult>>());

            var mappings = result.End().ToList();
            Assert.That(mappings.Count, Is.EqualTo(1));
            Assert.That(mappings[0].Query, Is.InstanceOf<TestQuery>());
            Assert.That(mappings[0].Transformer, Is.InstanceOf<TestTransformer>());
            Assert.That(mappings[0].SchemaPaths, Is.EqualTo(paths));
            Assert.That(mappings[0].Order, Is.EqualTo(1));
        }

        [Test]
        public void Map_WithDependents_ShouldCreateMappingsWithDependencies()
        {
            // Arrange
            var parentPaths = For.Paths("/root/test");
            var childPaths = For.Paths("/root/test/child");

            // Act
            var result = CreateSchema.For<TestEntity>()
                .Map<TestQuery, TestTransformer>(parentPaths, deps =>
                    deps.Dependents
                        .Map<TestChildQuery, TestChildTransformer>(childPaths));

            // Assert
            var mappings = result.End().ToList();
            Assert.That(mappings.Count, Is.EqualTo(2));

            var parentMapping = mappings.FirstOrDefault(m => m.Query is TestQuery);
            var childMapping = mappings.FirstOrDefault(m => m.Query is TestChildQuery);

            Assert.That(parentMapping, Is.Not.Null);
            Assert.That(childMapping, Is.Not.Null);
            Assert.That(childMapping.DependentOn, Is.EqualTo(parentMapping.Query));
            Assert.That(childMapping.Order, Is.EqualTo(2)); // Parent order + 1
        }

        [Test]
        public void Map_MultipleRootMappings_ShouldCreateMultipleMappings()
        {
            // Arrange
            var paths1 = For.Paths("/root/test1");
            var paths2 = For.Paths("/root/test2");

            // Act
            var result = CreateSchema.For<TestEntity>()
                .Map<TestQuery, TestTransformer>(paths1)
                .Map<TestChildQuery, TestChildTransformer>(paths2);

            // Assert
            var mappings = result.End().ToList();
            Assert.That(mappings.Count, Is.EqualTo(2));

            var mapping1 = mappings.FirstOrDefault(m => m.Query is TestQuery);
            var mapping2 = mappings.FirstOrDefault(m => m.Query is TestChildQuery);

            Assert.That(mapping1, Is.Not.Null);
            Assert.That(mapping2, Is.Not.Null);
            Assert.That(mapping1.DependentOn, Is.Null);
            Assert.That(mapping2.DependentOn, Is.Null);
            Assert.That(mapping1.Order, Is.EqualTo(1));
            Assert.That(mapping2.Order, Is.EqualTo(1));
        }

        [Test]
        public void GetMappings_ShouldReturnSelfReference()
        {
            // Arrange
            var mappings = CreateSchema.For<TestEntity>();

            // Act
            var getMappings = mappings.GetMappings;

            // Assert
            Assert.That(getMappings, Is.SameAs(mappings));
        }

        [Test]
        public void Order_ShouldBeSettableAndGettable()
        {
            // Arrange
            var mappings = CreateSchema.For<TestEntity>();

            // Act
            mappings.Order = 5;

            // Assert
            Assert.That(mappings.Order, Is.EqualTo(5));
        }

        [Test]
        public void Dependents_ShouldReturnNewMappingsWithIncrementedOrder()
        {
            // Arrange
            var mapping = new Mapping<TestEntity, IQueryResult>
            {
                Order = 3
            };

            // Act
            var dependents = mapping.Dependents;

            // Assert
            Assert.That(dependents, Is.Not.Null);
            Assert.That(dependents.Order, Is.EqualTo(4)); // Original order + 1
        }

        [Test]
        public void ComplexNestedMapping_ShouldCreateCorrectHierarchy()
        {
            // Act
            var result = CreateSchema.For<TestEntity>()
                .Map<TestQuery, TestTransformer>(For.Paths("/root"), deps =>
                    deps.Dependents
                        .Map<TestChildQuery, TestChildTransformer>(For.Paths("/root/child1"))
                        .Map<TestChildQuery, TestChildTransformer>(For.Paths("/root/child2"), childDeps =>
                            childDeps.Dependents
                                .Map<TestQuery, TestTransformer>(For.Paths("/root/child2/grandchild"))
                        )
                );

            // Assert
            var mappings = result.End().ToList();
            Assert.That(mappings.Count, Is.EqualTo(4));

            var orders = mappings.Select(m => m.Order).Distinct().ToList();
            Assert.That(orders, Contains.Item(1)); // Root level
            Assert.That(orders, Contains.Item(2)); // First level dependents
            Assert.That(orders, Contains.Item(3)); // Second level dependents
        }
    }
}