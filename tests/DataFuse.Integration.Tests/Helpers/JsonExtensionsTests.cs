using DataFuse.Integration.Helpers;
using DataFuse.Integration.Tests;

namespace DataFuse.Integration.Tests.Helpers
{
    [TestFixture]
    public class JsonExtensionsTests
    {
        [Test]
        public void ToJson_WithValidObject_ShouldReturnJsonString()
        {
            // Arrange
            var testObject = new { Name = "Test", Value = 123 };

            // Act
            var result = testObject.ToJson();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("Test"));
            Assert.That(result, Contains.Substring("123"));
            Assert.That(result, Does.StartWith("{"));
            Assert.That(result, Does.EndWith("}"));
        }

        [Test]
        public void ToJson_WithNullObject_ShouldReturnNull()
        {
            // Arrange
            object testObject = null;

            // Act
            var result = testObject.ToJson();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ToJson_WithString_ShouldReturnQuotedString()
        {
            // Arrange
            var testString = "Hello World";

            // Act
            var result = testString.ToJson();

            // Assert
            Assert.That(result, Is.EqualTo("\"Hello World\""));
        }

        [Test]
        public void ToJson_WithNumber_ShouldReturnNumberAsString()
        {
            // Arrange
            var number = 42;

            // Act
            var result = number.ToJson();

            // Assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void ToJson_WithBoolean_ShouldReturnBooleanAsString()
        {
            // Arrange
            var boolValue = true;

            // Act
            var result = boolValue.ToJson();

            // Assert
            Assert.That(result, Is.EqualTo("true"));
        }

        [Test]
        public void ToJson_WithArray_ShouldReturnJsonArray()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act
            var result = array.ToJson();

            // Assert
            Assert.That(result, Is.EqualTo("[1,2,3]"));
        }

        [Test]
        public void ToJson_WithList_ShouldReturnJsonArray()
        {
            // Arrange
            var list = new List<string> { "apple", "banana", "cherry" };

            // Act
            var result = list.ToJson();

            // Assert
            Assert.That(result, Contains.Substring("apple"));
            Assert.That(result, Contains.Substring("banana"));
            Assert.That(result, Contains.Substring("cherry"));
            Assert.That(result, Does.StartWith("["));
            Assert.That(result, Does.EndWith("]"));
        }

        [Test]
        public void ToJson_WithComplexObject_ShouldReturnValidJson()
        {
            // Arrange
            var complexObject = new TestEntity { Name = "TestEntity", Value = 456 };

            // Act
            var result = complexObject.ToJson();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("TestEntity"));
            Assert.That(result, Contains.Substring("456"));
        }

        [Test]
        public void ToObject_WithValidJsonString_ShouldReturnObject()
        {
            // Arrange
            var json = "{\"Name\":\"Test\",\"Value\":123}";
            var expectedType = typeof(TestEntity);

            // Act
            var result = json.ToObject(expectedType);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<TestEntity>());
            var entity = (TestEntity)result;
            Assert.That(entity.Name, Is.EqualTo("Test"));
            Assert.That(entity.Value, Is.EqualTo(123));
        }

        [Test]
        public void ToObject_WithNullOrEmptyString_ShouldReturnNull()
        {
            // Arrange
            string json = null;
            var type = typeof(TestEntity);

            // Act
            var result1 = json.ToObject(type);
            var result2 = string.Empty.ToObject(type);

            // Assert
            Assert.That(result1, Is.Null);
            Assert.That(result2, Is.Null);
        }

        [Test]
        public void ToObject_WithInvalidJson_ShouldThrowException()
        {
            // Arrange
            var invalidJson = "{invalid json}";
            var type = typeof(TestEntity);

            // Act & Assert
            Assert.Throws<System.Text.Json.JsonException>(() => invalidJson.ToObject(type));
        }

        [Test]
        public void ToObject_WithPrimitiveType_ShouldReturnPrimitive()
        {
            // Arrange
            var json = "42";
            var type = typeof(int);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void ToObject_WithStringType_ShouldReturnString()
        {
            // Arrange
            var json = "\"Hello World\"";
            var type = typeof(string);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void ToObject_WithBooleanType_ShouldReturnBoolean()
        {
            // Arrange
            var json = "true";
            var type = typeof(bool);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void ToObject_WithArrayType_ShouldReturnArray()
        {
            // Arrange
            var json = "[1,2,3]";
            var type = typeof(int[]);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.InstanceOf<int[]>());
            var array = (int[])result;
            Assert.That(array.Length, Is.EqualTo(3));
            Assert.That(array[0], Is.EqualTo(1));
            Assert.That(array[1], Is.EqualTo(2));
            Assert.That(array[2], Is.EqualTo(3));
        }

        [Test]
        public void ToObject_WithListType_ShouldReturnList()
        {
            // Arrange
            var json = "[\"apple\",\"banana\",\"cherry\"]";
            var type = typeof(List<string>);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.InstanceOf<List<string>>());
            var list = (List<string>)result;
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo("apple"));
            Assert.That(list[1], Is.EqualTo("banana"));
            Assert.That(list[2], Is.EqualTo("cherry"));
        }

        [Test]
        public void RoundTripSerialization_ShouldPreserveData()
        {
            // Arrange
            var originalEntity = new TestEntity { Name = "RoundTrip", Value = 789 };

            // Act
            var json = originalEntity.ToJson();
            var deserializedEntity = (TestEntity)json.ToObject(typeof(TestEntity));

            // Assert
            Assert.That(deserializedEntity, Is.Not.Null);
            Assert.That(deserializedEntity.Name, Is.EqualTo(originalEntity.Name));
            Assert.That(deserializedEntity.Value, Is.EqualTo(originalEntity.Value));
        }

        [Test]
        public void ToJson_WithNestedObject_ShouldSerializeNested()
        {
            // Arrange
            var nestedObject = new
            {
                Parent = "ParentValue",
                Child = new { Name = "ChildName", Value = 100 }
            };

            // Act
            var result = nestedObject.ToJson();

            // Assert
            Assert.That(result, Contains.Substring("ParentValue"));
            Assert.That(result, Contains.Substring("ChildName"));
            Assert.That(result, Contains.Substring("100"));
        }

        [Test]
        public void ToObject_WithNestedJson_ShouldDeserializeNested()
        {
            // Arrange
            var json = "{\"Name\":\"Parent\",\"Child\":{\"Description\":\"ChildDesc\"}}";
            var type = typeof(ParentEntity);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ParentEntity>());
            var parent = (ParentEntity)result;
            Assert.That(parent.Name, Is.EqualTo("Parent"));
            Assert.That(parent.Child, Is.Not.Null);
            Assert.That(parent.Child.Description, Is.EqualTo("ChildDesc"));
        }

        [Test]
        public void ToJson_WithEmptyObject_ShouldReturnEmptyJson()
        {
            // Arrange
            var emptyObject = new { };

            // Act
            var result = emptyObject.ToJson();

            // Assert
            Assert.That(result, Is.EqualTo("{}"));
        }

        [Test]
        public void ToObject_WithEmptyJson_ShouldReturnEmptyObject()
        {
            // Arrange
            var json = "{}";
            var type = typeof(TestEntity);

            // Act
            var result = json.ToObject(type);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<TestEntity>());
            var entity = (TestEntity)result;
            Assert.That(entity.Name, Is.Null);
            Assert.That(entity.Value, Is.EqualTo(0));
        }

        // Helper class for nested object tests
        public class ParentEntity
        {
            public string Name { get; set; }
            public ChildEntity Child { get; set; }
        }

        public class ChildEntity
        {
            public string Description { get; set; }
        }
    }
}