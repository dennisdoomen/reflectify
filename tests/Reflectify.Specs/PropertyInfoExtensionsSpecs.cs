using System.Globalization;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class PropertyInfoExtensionsSpecs
{
    public class IsIndexer
    {
        [Fact]
        public void Can_determine_a_property_is_an_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).GetProperty("Item");

            // Assert
            indexer.IsIndexer().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_property_is_not_an_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).GetProperty("Foo");

            // Assert
            indexer.IsIndexer().Should().BeFalse();
        }

        private sealed class ClassWithIndexer
        {
            [UsedImplicitly]
            public object Foo { get; set; }

            public string this[int n] => n.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class IsExplicitlyImplemented
    {
        [Fact]
        public void An_explicitly_implemented_property_is_explicitly_implemented()
        {
            // Act
            PropertyInfo property = typeof(ClassWithExplicitProperty)
                .GetProperty($"{typeof(IWithProperty).FullName!.Replace("+", ".")}.Value",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            property.IsExplicitlyImplemented().Should().BeTrue();
        }

        [Fact]
        public void A_normal_property_is_not_explicitly_implemented()
        {
            // Act
            PropertyInfo property = typeof(ClassWithExplicitProperty).GetProperty("NormalProperty");

            // Assert
            property.IsExplicitlyImplemented().Should().BeFalse();
        }

        private interface IWithProperty
        {
            [UsedImplicitly]
            string Value { get; }
        }

        private class ClassWithExplicitProperty : IWithProperty
        {
            [UsedImplicitly]
            public string NormalProperty { get; set; }

            string IWithProperty.Value => "explicit";
        }
    }

    public class IsPublic
    {
        [Fact]
        public void A_property_with_a_public_getter_is_public()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "PublicProperty", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            property.IsPublic().Should().BeTrue();
        }

        [Fact]
        public void A_property_with_only_a_public_setter_is_public()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "SetterOnlyProperty", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            property.IsPublic().Should().BeTrue();
        }

        [Fact]
        public void An_internal_property_is_not_public()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "InternalProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            property.IsPublic().Should().BeFalse();
        }
    }

    public class IsInternal
    {
        [Fact]
        public void An_internal_property_is_internal()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "InternalProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            property.IsInternal().Should().BeTrue();
        }

        [Fact]
        public void A_protected_internal_property_is_internal()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "ProtectedInternalProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            property.IsInternal().Should().BeTrue();
        }

        [Fact]
        public void A_public_property_is_not_internal()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "PublicProperty", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            property.IsInternal().Should().BeFalse();
        }
    }

    public class IsAbstract
    {
        [Fact]
        public void An_abstract_property_is_abstract()
        {
            // Act
            PropertyInfo property = typeof(AbstractClassWithProperty).GetProperty("AbstractProperty");

            // Assert
            property.IsAbstract().Should().BeTrue();
        }

        [Fact]
        public void A_concrete_property_is_not_abstract()
        {
            // Act
            PropertyInfo property = typeof(ClassWithVariousProperties).GetProperty(
                "PublicProperty", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            property.IsAbstract().Should().BeFalse();
        }

        [Fact]
        public void An_overridden_property_is_not_abstract()
        {
            // Act
            PropertyInfo property = typeof(ConcreteClassWithProperty).GetProperty("AbstractProperty");

            // Assert
            property.IsAbstract().Should().BeFalse();
        }

        private abstract class AbstractClassWithProperty
        {
            [UsedImplicitly]
            public abstract string AbstractProperty { get; set; }
        }

        private sealed class ConcreteClassWithProperty : AbstractClassWithProperty
        {
            public override string AbstractProperty { get; set; }
        }
    }

    private class ClassWithVariousProperties
    {
        [UsedImplicitly]
        public string PublicProperty { get; set; }

        [UsedImplicitly]
        public string SetterOnlyProperty { private get; set; }

        [UsedImplicitly]
        internal string InternalProperty { get; set; }

        [UsedImplicitly]
        protected internal string ProtectedInternalProperty { get; set; }
    }
}
