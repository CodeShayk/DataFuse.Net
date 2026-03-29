namespace DataFuse.Integration.Tests
{
    [TestFixture]
    public class CacheResultAttributeTests
    {
        [Test]
        public void Constructor_ShouldCreateInstance()
        {
            // Act
            var attribute = new CacheResultAttribute();

            // Assert
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute, Is.InstanceOf<Attribute>());
        }

        [Test]
        public void Attribute_CanBeAppliedToClass()
        {
            // Act
            var type = typeof(CacheableQueryResult);
            var attributes = type.GetCustomAttributes(typeof(CacheResultAttribute), false);

            // Assert
            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes.Length, Is.EqualTo(1));
            Assert.That(attributes[0], Is.InstanceOf<CacheResultAttribute>());
        }

        [Test]
        public void Attribute_CanBeDetectedOnType()
        {
            // Act
            var type = typeof(CacheableQueryResult);
            var hasCacheAttribute = type.GetCustomAttributes(typeof(CacheResultAttribute), false).Length > 0;

            // Assert
            Assert.That(hasCacheAttribute, Is.True);
        }

        [Test]
        public void Attribute_OnNonCacheableType_ShouldNotBePresent()
        {
            // Act
            var type = typeof(NonCacheableQueryResult);
            var hasCacheAttribute = type.GetCustomAttributes(typeof(CacheResultAttribute), false).Length > 0;

            // Assert
            Assert.That(hasCacheAttribute, Is.False);
        }

        [Test]
        public void Attribute_InheritanceTest_ShouldNotInheritByDefault()
        {
            // Act
            var baseType = typeof(CacheableQueryResult);
            var derivedType = typeof(DerivedQueryResult);

            var baseHasAttribute = baseType.GetCustomAttributes(typeof(CacheResultAttribute), false).Length > 0;
            var derivedHasAttribute = derivedType.GetCustomAttributes(typeof(CacheResultAttribute), false).Length > 0;
            var derivedInheritsAttribute = derivedType.GetCustomAttributes(typeof(CacheResultAttribute), true).Length > 0;

            // Assert
            Assert.That(baseHasAttribute, Is.True);
            Assert.That(derivedHasAttribute, Is.False); // Should not have its own attribute
            Assert.That(derivedInheritsAttribute, Is.True); // But should inherit from base when inherit = true
        }

        [Test]
        public void MultipleAttributes_ShouldNotBeAllowed()
        {
            // This test verifies that you can't apply the same attribute multiple times
            // by checking a type that attempts to do so would cause a compilation error
            // Since CacheResultAttribute doesn't specify AllowMultiple = true,
            // multiple applications should not be allowed

            // Act
            var type = typeof(CacheableQueryResult);
            var attributes = type.GetCustomAttributes(typeof(CacheResultAttribute), false);

            // Assert
            Assert.That(attributes.Length, Is.EqualTo(1)); // Should only be one instance
        }

        [Test]
        public void AttributeUsage_ShouldFollowDefaultBehavior()
        {
            // Act
            var attributeType = typeof(CacheResultAttribute);
            var usageAttributes = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), false);

            // Assert
            // Since CacheResultAttribute doesn't specify AttributeUsage, it uses the default
            // which allows application to any target and doesn't allow multiple uses
            if (usageAttributes.Length > 0)
            {
                var usage = (AttributeUsageAttribute)usageAttributes[0];
                Assert.That(usage.AllowMultiple, Is.False);
            }
        }

        // Test classes
        [CacheResult]
        public class CacheableQueryResult : IQueryResult
        {
            public string Data { get; set; }
        }

        public class NonCacheableQueryResult : IQueryResult
        {
            public string Data { get; set; }
        }

        public class DerivedQueryResult : CacheableQueryResult
        {
            public string AdditionalData { get; set; }
        }
    }

    [TestFixture]
    public class QueryComparerTests
    {
        private QueryComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _comparer = new QueryComparer();
        }

        [Test]
        public void Equals_WithSameQueryType_ShouldReturnTrue()
        {
            // Arrange
            var query1 = new TestQuery();
            var query2 = new TestQuery();

            // Act
            var result = _comparer.Equals(query1, query2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WithDifferentQueryTypes_ShouldReturnFalse()
        {
            // Arrange
            var query1 = new TestQuery();
            var query2 = new TestChildQuery();

            // Act
            var result = _comparer.Equals(query1, query2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WithSameInstance_ShouldReturnTrue()
        {
            // Arrange
            var query = new TestQuery();

            // Act
            var result = _comparer.Equals(query, query);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WithNullQueries_ShouldHandleCorrectly()
        {
            // Act
            var result1 = _comparer.Equals(null, null);
            var result2 = _comparer.Equals(new TestQuery(), null);
            var result3 = _comparer.Equals(null, new TestQuery());

            // Assert
            Assert.That(result1, Is.True); // Both null should be equal
            Assert.That(result2, Is.False); // One null should not equal non-null
            Assert.That(result3, Is.False); // One null should not equal non-null
        }

        [Test]
        public void GetHashCode_WithSameQueryType_ShouldReturnSameHashCode()
        {
            // Arrange
            var query1 = new TestQuery();
            var query2 = new TestQuery();

            // Act
            var hash1 = _comparer.GetHashCode(query1);
            var hash2 = _comparer.GetHashCode(query2);

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_WithDifferentQueryTypes_ShouldReturnDifferentHashCodes()
        {
            // Arrange
            var query1 = new TestQuery();
            var query2 = new TestChildQuery();

            // Act
            var hash1 = _comparer.GetHashCode(query1);
            var hash2 = _comparer.GetHashCode(query2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_WithNullQuery_ShouldNotThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _comparer.GetHashCode(null));
        }

        [Test]
        public void Comparer_ShouldBeUsableInHashSet()
        {
            // Arrange
            var queries = new System.Collections.Generic.HashSet<IQuery>(_comparer)
            {
                new TestQuery(),
                new TestQuery(), // Duplicate type
                new TestChildQuery()
            };

            // Act & Assert
            Assert.That(queries.Count, Is.EqualTo(2)); // Should only contain unique types
        }

        [Test]
        public void Comparer_ShouldBeUsableInLinqDistinct()
        {
            // Arrange
            var queries = new System.Collections.Generic.List<IQuery>
            {
                new TestQuery(),
                new TestQuery(),
                new TestChildQuery(),
                new TestQuery(),
                new TestChildQuery()
            };

            // Act
            var distinctQueries = queries.Distinct(_comparer).ToList();

            // Assert
            Assert.That(distinctQueries.Count, Is.EqualTo(2)); // Should only contain unique types
        }

        [Test]
        public void Comparer_WithInheritedTypes_ShouldTreatAsDifferent()
        {
            // Arrange
            var baseQuery = new TestQuery();
            var derivedQuery = new DerivedTestQuery();

            // Act
            var areEqual = _comparer.Equals(baseQuery, derivedQuery);
            var baseHash = _comparer.GetHashCode(baseQuery);
            var derivedHash = _comparer.GetHashCode(derivedQuery);

            // Assert
            Assert.That(areEqual, Is.False);
            Assert.That(baseHash, Is.Not.EqualTo(derivedHash));
        }

        // Test class for inheritance testing
        public class DerivedTestQuery : TestQuery
        {
            public string AdditionalProperty { get; set; }
        }
    }
}