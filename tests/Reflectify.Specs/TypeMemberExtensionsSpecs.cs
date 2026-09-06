using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class TypeMemberExtensionsSpecs
{
    public class MemberKindExtensions
    {
        [Theory]
        [InlineData(MemberKind.Internal)]
        [InlineData(MemberKind.Protected)]
        [InlineData(MemberKind.Private)]
        public void Non_public_visibility_kinds_map_to_non_public_binding_flags(MemberKind kind)
        {
            // Act
            BindingFlags flags = kind.ToBindingFlags();

            // Assert
            flags.Should().Be(BindingFlags.NonPublic);
        }
    }

    public class GetPropertiesAndFields
    {
        [Fact]
        public void Can_get_all_public_explicit_and_default_instance_interface_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.ExplicitlyImplemented | MemberKind.DefaultInterfaceProperties);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new
                {
                    Name =
                        $"{typeof(IInterfaceWithSingleProperty).FullName!.Replace("+", ".")}.ExplicitlyImplementedProperty",
                    PropertyType = typeof(string)
                },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultProperty", PropertyType = typeof(string) }
#endif
            });
        }

        [Fact]
        public void Can_get_all_public_static_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.Static);

            // Assert
            properties.Should().BeEquivalentTo([
                new { Name = "StaticProperty", PropertyType = typeof(bool) }
            ]);
        }

        [Fact]
        public void Can_get_all_properties_from_an_interface()
        {
            // Act
            var properties = typeof(IInterfaceWithDefaultProperty).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new { Name = "ExplicitlyImplementedProperty", PropertyType = typeof(string) },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultProperty", PropertyType = typeof(string) },
#endif
            });
        }

        [Fact]
        public void Can_get_normal_public_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) }
            });
        }

        [Fact]
        public void Can_get_explicit_properties_only()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Name =
                        $"{typeof(IInterfaceWithSingleProperty).FullName!.Replace("+", ".")}.ExplicitlyImplementedProperty",
                    PropertyType = typeof(string)
                }
            });
        }

        [Fact]
        public void Prefers_normal_property_over_explicitly_implemented_one()
        {
            // Act
            var properties = typeof(ClassWithExplicitAndNormalProperty).GetProperties(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "ExplicitlyImplementedProperty", PropertyType = typeof(int) }
            });
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_get_default_interface_properties_only()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.DefaultInterfaceProperties);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "DefaultProperty", PropertyType = typeof(string) }
            });
        }
#endif

        [Fact]
        public void Can_get_internal_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Internal);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "InternalProperty", PropertyType = typeof(bool) },
                new { Name = "InternalProtectedProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_protected_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Protected);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "ProtectedProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_private_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Private);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "PrivateProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_combine_protected_and_private_properties_with_public()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.Protected | MemberKind.Private);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new { Name = "ProtectedProperty", PropertyType = typeof(bool) },
                new { Name = "PrivateProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_protected_and_private_properties_without_public_ones()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(MemberKind.Protected | MemberKind.Private);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "ProtectedProperty", PropertyType = typeof(bool) },
                new { Name = "PrivateProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_properties_for_every_visibility()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Public | MemberKind.Internal | MemberKind.Protected | MemberKind.Private);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalProperty", PropertyType = typeof(string) },
                new { Name = "NewProperty", PropertyType = typeof(int) },
                new { Name = "InterfaceProperty", PropertyType = typeof(string) },
                new { Name = "InternalProperty", PropertyType = typeof(bool) },
                new { Name = "InternalProtectedProperty", PropertyType = typeof(bool) },
                new { Name = "ProtectedProperty", PropertyType = typeof(bool) },
                new { Name = "PrivateProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_protected_and_private_static_properties()
        {
            // Act
            var properties = typeof(SuperClass).GetProperties(
                MemberKind.Protected | MemberKind.Private | MemberKind.Static);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "ProtectedStaticProperty", PropertyType = typeof(bool) },
                new { Name = "PrivateStaticProperty", PropertyType = typeof(bool) }
            });
        }

        [Fact]
        public void Can_get_write_only_properties()
        {
            // Act
            var properties = typeof(ClassImplementingSetterOnlyInterface)
                .GetProperties(MemberKind.Public | MemberKind.DefaultInterfaceProperties | MemberKind.ExplicitlyImplemented);

            // Assert
            properties.Should().BeEquivalentTo([
                new { Name = "WriteOnlyProperty", PropertyType = typeof(string) }
            ]);
        }

        [Fact]
        public void Will_ignore_indexers()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetProperties(MemberKind.Public);

            // Assert
            properties.Should().BeEquivalentTo(new[]
            {
                new { Name = "Foo", PropertyType = typeof(object) }
            });
        }

        [Fact]
        public void Supports_returning_no_properties_if_asked_for()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetProperties(MemberKind.None);

            // Assert
            properties.Should().BeEmpty();
        }

        [Fact]
        public void Can_find_public_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Public);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_internal_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Internal);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "InternalField", FieldType = typeof(string) },
                new { Name = "ProtectedInternalField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_protected_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Protected);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "protectedField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_private_fields()
        {
            // Act
            var fields = typeof(ClassWithFieldsOfEveryVisibility).GetFields(MemberKind.Private);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "privateField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_combine_protected_and_private_fields_with_public()
        {
            // Act
            var fields = typeof(ClassWithFieldsOfEveryVisibility).GetFields(
                MemberKind.Public | MemberKind.Protected | MemberKind.Private);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "PublicField", FieldType = typeof(string) },
                new { Name = "protectedField", FieldType = typeof(string) },
                new { Name = "privateField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Can_get_protected_and_private_static_fields()
        {
            // Act
            var fields = typeof(ClassWithFieldsOfEveryVisibility).GetFields(
                MemberKind.Protected | MemberKind.Private | MemberKind.Static);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "protectedStaticField", FieldType = typeof(string) },
                new { Name = "privateStaticField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Requesting_private_fields_also_surfaces_compiler_generated_property_backing_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Private);

            // Assert
            fields.Select(f => f.Name).Should().Contain(name => name.Contains("k__BackingField"),
                "auto-implemented properties store their state in a compiler-generated private field");
        }

        [Fact]
        public void Can_find_all_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(
                MemberKind.Internal | MemberKind.Public);

            // Assert
            fields.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalField", FieldType = typeof(string) },
                new { Name = "InternalField", FieldType = typeof(string) },
                new { Name = "ProtectedInternalField", FieldType = typeof(string) }
            });
        }

        [Fact]
        public void Supports_returning_no_fields_if_asked_for()
        {
            // Act
            var properties = typeof(ClassWithIndexer).GetFields(MemberKind.None);

            // Assert
            properties.Should().BeEmpty();
        }

        [Fact]
        public void Can_find_public_static_fields()
        {
            // Act
            var fields = typeof(SuperClass).GetFields(MemberKind.Public | MemberKind.Static);

            // Assert
            fields.Should().BeEquivalentTo([
                new { Name = "StaticField", FieldType = typeof(bool) }
            ]);
        }

        [Fact]
        public void Can_find_all_members()
        {
            // Act
            var members = typeof(SuperClass).GetMembers(MemberKind.Public);

            // Assert
            members.Should().BeEquivalentTo([
                new { Name = "NormalProperty" },
                new { Name = "NewProperty" },
                new { Name = "InterfaceProperty" },
                new { Name = "NormalField" },
            ]);
        }

        [Fact]
        public void Can_find_all_internal_members()
        {
            // Act
            var members = typeof(SuperClass).GetMembers(MemberKind.Internal);

            // Assert
            members.Should().BeEquivalentTo([
                new { Name = "InternalProperty" },
                new { Name = "InternalProtectedProperty" },
                new { Name = "InternalField" },
                new { Name = "ProtectedInternalField" },
            ]);
        }

        [Fact]
        public void Can_find_all_protected_and_private_members()
        {
            // Act
            var members = typeof(ClassWithFieldsOfEveryVisibility).GetMembers(MemberKind.Protected | MemberKind.Private);

            // Assert
            members.Should().BeEquivalentTo([
                new { Name = "protectedField" },
                new { Name = "privateField" },
            ]);
        }

        [Fact]
        public void Can_find_every_member_regardless_of_visibility()
        {
            // Act
            var members = typeof(ClassWithFieldsOfEveryVisibility).GetMembers(
                MemberKind.Public | MemberKind.Internal | MemberKind.Protected | MemberKind.Private);

            // Assert
            members.Should().BeEquivalentTo([
                new { Name = "PublicField" },
                new { Name = "InternalField" },
                new { Name = "ProtectedInternalField" },
                new { Name = "protectedField" },
                new { Name = "privateField" },
            ]);
        }

        private interface ICollectionInterface
        {
            [UsedImplicitly]
            IReadOnlyCollection<int> Items { get; }
        }

        private abstract class BaseCollection
        {
            public List<int> Items { get; } = new();
        }

        private sealed class CollectionImplementation : BaseCollection, ICollectionInterface
        {
            IReadOnlyCollection<int> ICollectionInterface.Items => Items;
        }

        [Fact]
        public void Normal_properties_are_always_preferred_over_explicit_properties()
        {
            var properties = typeof(CollectionImplementation).GetProperties(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            properties.Should().BeEquivalentTo([
                new { Name = "Items", PropertyType = typeof(List<int>) }
            ]);
        }
    }

    public class FindProperty
    {
        [Fact]
        public void Can_find_a_normal_property()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NormalProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_property_if_it_does_not_exist()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NonExistingProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void A_property_name_is_required(string propertyName)
        {
            // Act
            var act = () => typeof(SuperClass).FindProperty(propertyName, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*property name*");
        }

        [Fact]
        public void Can_find_a_property_that_hides_its_base_class_name_sake()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("NewProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<int>();
        }

        [Fact]
        public void Can_find_a_property_that_is_hidden_by_a_superclass_provided_you_refer_to_the_baseclass()
        {
            // Act
            var property = typeof(BaseClass).FindProperty("NewProperty", MemberKind.Public);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_an_internal_property_if_you_ask_for_public_ones()
        {
            // Act
            var property = typeof(BaseClass).FindProperty("InternalProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("InternalProperty", MemberKind.Internal);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Can_find_an_internal_protected_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("InternalProtectedProperty", MemberKind.Internal);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Cannot_find_a_protected_property_if_you_ask_for_public_ones()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ProtectedProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_protected_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ProtectedProperty", MemberKind.Protected);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Cannot_find_a_private_property_if_you_ask_for_public_ones()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("PrivateProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_private_property_if_you_ask_for_them()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("PrivateProperty", MemberKind.Private);

            // Assert
            property.Should().NotBeNull().And.Return<bool>();
        }

        [Fact]
        public void Cannot_find_a_protected_property_if_you_only_ask_for_private_ones()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ProtectedProperty", MemberKind.Private);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Cannot_find_an_explicitly_implemented_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_explicitly_implemented_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.ExplicitlyImplemented);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_default_interface_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_find_a_default_interface_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.DefaultInterfaceProperties);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }
#endif

        [Fact]
        public void Can_find_a_public_indexer()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Public, typeof(int));

            // Assert
            indexer.Should().NotBeNull();
            indexer.GetIndexParameters().Should().BeEquivalentTo(new[]
            {
                new { ParameterType = typeof(int) }
            });
        }

        [Fact]
        public void Cannot_find_an_internal_indexer_if_you_ask_for_public_ones()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Public, typeof(string), typeof(string));

            // Assert
            indexer.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_indexer_if_you_ask_for_ot()
        {
            // Act
            var indexer = typeof(ClassWithIndexer).FindIndexer(MemberKind.Internal, typeof(string), typeof(string));

            // Assert
            indexer.Should().NotBeNull();
            indexer.GetIndexParameters().Should().BeEquivalentTo(new[]
            {
                new { ParameterType = typeof(string) },
                new { ParameterType = typeof(string) }
            });
        }

        [Fact]
        public void Can_find_an_indexer_on_an_interface()
        {
            // Act
            var indexer = typeof(IWithIndexer).FindIndexer(MemberKind.Public, typeof(int));

            // Assert
            indexer.Should().NotBeNull();
            indexer.GetIndexParameters().Should().BeEquivalentTo(new[]
            {
                new { ParameterType = typeof(int) }
            });
        }

        [Fact]
        public void Cannot_find_a_non_existing_indexer_on_an_interface()
        {
            // Act
            var indexer = typeof(IWithIndexer).FindIndexer(MemberKind.Public, typeof(string));

            // Assert
            indexer.Should().BeNull();
        }

        private interface IWithIndexer
        {
            string this[int n] { get; }
        }
    }

    public class FindField
    {
        [Fact]
        public void Can_find_a_public_instance_field()
        {
            // Act
            var field = typeof(SuperClass).FindField("NormalField", MemberKind.Public);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("NormalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_a_field_if_it_does_not_exist()
        {
            // Act
            var field = typeof(SuperClass).FindField("NonExistingField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Cannot_find_a_static_field_if_you_dont_ask_for_it()
        {
            // Act
            var field = typeof(SuperClass).FindField("StaticField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_static_field_if_you_ask_for_it()
        {
            // Act
            var field = typeof(SuperClass).FindField("StaticField", MemberKind.Public | MemberKind.Static);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("StaticField");
            field.FieldType.Should().Be<bool>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void A_field_name_is_required(string name)
        {
            // Act
            var act = () => typeof(SuperClass).FindField(name, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*field name*");
        }

        [Fact]
        public void Cannot_find_an_internal_field_if_you_ask_for_public_ones()
        {
            // Act
            var field = typeof(BaseClass).FindField("InternalField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_field_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("InternalField", MemberKind.Internal);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("InternalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Can_find_an_internal_protected_property_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("ProtectedInternalField", MemberKind.Internal);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("ProtectedInternalField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_a_protected_field_if_you_ask_for_public_ones()
        {
            // Act
            var field = typeof(SuperClass).FindField("protectedField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_protected_field_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("protectedField", MemberKind.Protected);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("protectedField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_a_private_field_if_you_ask_for_public_ones()
        {
            // Act
            var field = typeof(SuperClass).FindField("privateField", MemberKind.Public);

            // Assert
            field.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_private_field_if_you_ask_for_them()
        {
            // Act
            var field = typeof(SuperClass).FindField("privateField", MemberKind.Private);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("privateField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Can_find_a_private_field_declared_by_a_base_class()
        {
            // Act
            var field = typeof(SuperClass).FindField("privateBaseField", MemberKind.Private);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("privateBaseField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Can_find_a_private_static_field_if_you_ask_for_it()
        {
            // Act
            var field = typeof(ClassWithFieldsOfEveryVisibility)
                .FindField("privateStaticField", MemberKind.Private | MemberKind.Static);

            // Assert
            field.Should().NotBeNull();
            field.Name.Should().Be("privateStaticField");
            field.FieldType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_an_explicitly_implemented_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_explicitly_implemented_property_if_you_ask_for_it()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("ExplicitlyImplementedProperty", MemberKind.ExplicitlyImplemented);

            // Assert
            property.Should().NotBeNull().And.Return<string>();
        }

        [Fact]
        public void Cannot_find_a_default_interface_property_if_you_dont_ask_for_that()
        {
            // Act
            var property = typeof(SuperClass).FindProperty("DefaultProperty", MemberKind.Public);

            // Assert
            property.Should().BeNull();
        }
    }

    public class Methods
    {
        [Fact]
        public void Can_find_a_parameterless_method()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("Method", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(0);
        }

        [Fact]
        public void Can_find_a_parameterless_method_directly()
        {
            // Act
            var method = typeof(ClassWithMethods).FindParameterlessMethod("Method", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(0);
        }

        [Fact]
        public void Cannot_find_an_internal_method_if_you_ask_for_public_ones()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("InternalMethod", MemberKind.Public);

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_method_if_you_ask_for_it()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("InternalMethod", MemberKind.Internal);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_protected_internal_method_if_you_ask_for_it()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("ProtectedInternalMethod", MemberKind.Internal);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_static_method()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("StaticMethod", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void Can_find_a_method_with_any_parameter()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public);

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(3);
        }

        [Fact]
        public void Can_find_a_method_with_specific_parameter()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int), typeof(bool));

            // Assert
            method.Should().NotBeNull();
            method.GetParameters().Should().HaveCount(3);
        }

        [Fact]
        public void Can_detect_a_method_with_specific_parameter()
        {
            // Act / Assert
            typeof(ClassWithMethods).HasMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int), typeof(bool)).Should().BeTrue();
        }

        [Fact]
        public void Cannot_detect_a_non_existing_method()
        {
            // Act / Assert
            typeof(ClassWithMethods).HasMethod("NonExistingMethod", MemberKind.Public).Should().BeFalse();
        }

        [Fact]
        public void A_method_matching_the_predicate_exists()
        {
            // Act / Assert
            typeof(ClassWithMethods).HasMethod(method => method.Name == nameof(ClassWithMethods.StaticMethod)).Should().BeTrue();
        }

        [Fact]
        public void A_method_not_matching_the_predicate_does_not_exist()
        {
            // Act / Assert
            typeof(ClassWithMethods).HasMethod(method => method.Name == "NonExistingMethod").Should().BeFalse();
        }

        [Fact]
        public void A_null_method_predicate_is_rejected()
        {
            // Act
            var act = () => typeof(ClassWithMethods).HasMethod(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void The_name_of_the_method_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("NonExistingName", MemberKind.Public, typeof(string), typeof(int),
                typeof(bool));

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void The_number_of_parameters_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(int));

            // Assert
            method.Should().BeNull();
        }

        [Fact]
        public void The_type_of_the_parameters_must_match()
        {
            // Act
            var method = typeof(ClassWithMethods).FindMethod("MethodWithThreeParameters", MemberKind.Public, typeof(string),
                typeof(object), typeof(bool));

            // Assert
            method.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void The_name_is_required(string methodName)
        {
            // Act
            var act = () => typeof(ClassWithMethods).FindMethod(methodName, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*method name*");
        }

        private class ClassWithMethods
        {
            [UsedImplicitly]
            public void Method()
            {
            }

            [UsedImplicitly]
            public void MethodWithThreeParameters(string text, int number, bool flag)
            {
            }

            [UsedImplicitly]
            internal void InternalMethod()
            {
            }

            [UsedImplicitly]
            public static void StaticMethod()
            {
            }

            [UsedImplicitly]
            protected internal void ProtectedInternalMethod()
            {
            }
        }
    }

    public class GetEvents
    {
        [Fact]
        public void Can_get_all_public_explicit_and_default_instance_interface_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(
                MemberKind.Public | MemberKind.ExplicitlyImplemented | MemberKind.DefaultInterfaceProperties);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalEvent" },
                new
                {
                    Name = $"{typeof(IInterfaceWithSingleEvent).FullName!.Replace("+", ".")}.ExplicitlyImplementedEvent"
                },
#if NETCOREAPP3_0_OR_GREATER
                new { Name = "DefaultEvent" }
#endif
            });
        }

        [Fact]
        public void Can_get_normal_public_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Public);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "NormalEvent" }
            });
        }

        [Fact]
        public void Can_get_internal_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Internal);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "InternalEvent" }
            });
        }

        [Fact]
        public void Can_get_protected_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Protected);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "ProtectedEvent" }
            });
        }

        [Fact]
        public void Can_get_private_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Private);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "PrivateEvent" }
            });
        }

        [Fact]
        public void A_protected_event_is_not_an_internal_one()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Internal);

            // Assert
            events.Should().NotContain(e => e.Name == "ProtectedEvent");
        }

        [Fact]
        public void Can_get_explicit_events_only()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.ExplicitlyImplemented);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new
                {
                    Name = $"{typeof(IInterfaceWithSingleEvent).FullName!.Replace("+", ".")}.ExplicitlyImplementedEvent"
                }
            });
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Can_get_default_interface_events_only()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.DefaultInterfaceProperties);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "DefaultEvent" }
            });
        }
#endif

        [Fact]
        public void Can_get_public_static_events()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.Public | MemberKind.Static);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "StaticEvent" }
            });
        }

        [Fact]
        public void Prefers_normal_event_over_explicitly_implemented_one()
        {
            // Act
            var events = typeof(ClassWithExplicitAndNormalEvent).GetEvents(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "ExplicitlyImplementedEvent" }
            });
        }

        [Fact]
        public void Supports_returning_no_events_if_asked_for()
        {
            // Act
            var events = typeof(ClassWithEvents).GetEvents(MemberKind.None);

            // Assert
            events.Should().BeEmpty();
        }

        [Fact]
        public void Events_declared_on_a_base_class_are_returned_through_the_derived_type()
        {
            // Act
            var events = typeof(DerivedClassWithEvents).GetEvents(MemberKind.Public);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "HiddenEvent" },
                new { Name = "DerivedEvent" },
                new { Name = "BaseEvent" }
            });
        }

        [Fact]
        public void An_event_that_hides_its_base_class_name_sake_is_only_returned_once()
        {
            // Act
            var events = typeof(DerivedClassWithEvents).GetEvents(MemberKind.Public);

            // Assert
            events.Should().ContainSingle(e => e.Name == "HiddenEvent")
                .Which.DeclaringType.Should().Be(typeof(DerivedClassWithEvents));
        }

        [Fact]
        public void Events_declared_on_an_interface_are_returned_when_reflecting_that_interface()
        {
            // Act
            var events = typeof(IInterfaceWithSingleEvent).GetEvents(MemberKind.Public);

            // Assert
            events.Should().BeEquivalentTo(new[]
            {
                new { Name = "ExplicitlyImplementedEvent" }
            });
        }

        [Fact]
        public void Events_inherited_from_a_base_interface_are_returned_when_reflecting_an_interface()
        {
            // Act
            var events = typeof(IInterfaceWithDefaultEvent).GetEvents(MemberKind.Public);

            // Assert
            events.Should().Contain(e => e.Name == "ExplicitlyImplementedEvent");
        }

        [Fact]
        public void A_normal_event_wins_from_an_explicitly_implemented_one_in_a_base_class()
        {
            // Act
            var events = typeof(DerivedClassWithNormalEvent).GetEvents(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            // Assert
            events.Should().ContainSingle()
                .Which.DeclaringType.Should().Be(typeof(DerivedClassWithNormalEvent));
        }

        [Fact]
        public void A_normal_event_in_a_base_class_wins_from_an_explicitly_implemented_one()
        {
            // Act
            var events = typeof(DerivedClassWithExplicitEvent).GetEvents(
                MemberKind.Public | MemberKind.ExplicitlyImplemented);

            // Assert
            events.Should().ContainSingle()
                .Which.DeclaringType.Should().Be(typeof(BaseClassWithNormalEvent));
        }
    }

    public class FindEvent
    {
        [Fact]
        public void Can_find_a_normal_event()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("NormalEvent", MemberKind.Public);

            // Assert
            @event.Should().NotBeNull();
            @event.EventHandlerType.Should().Be<EventHandler>();
        }

        [Fact]
        public void Cannot_find_an_event_if_it_does_not_exist()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("NonExistingEvent", MemberKind.Public);

            // Assert
            @event.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void An_event_name_is_required(string eventName)
        {
            // Act
            var act = () => typeof(ClassWithEvents).FindEvent(eventName, MemberKind.Public);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*event name*");
        }

        [Fact]
        public void Cannot_find_an_internal_event_if_you_ask_for_public_ones()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("InternalEvent", MemberKind.Public);

            // Assert
            @event.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_event_if_you_ask_for_them()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("InternalEvent", MemberKind.Internal);

            // Assert
            @event.Should().NotBeNull();
        }

        [Fact]
        public void Cannot_find_an_explicitly_implemented_event_if_you_dont_ask_for_that()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("ExplicitlyImplementedEvent", MemberKind.Public);

            // Assert
            @event.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_explicitly_implemented_event_if_you_ask_for_it()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("ExplicitlyImplementedEvent", MemberKind.ExplicitlyImplemented);

            // Assert
            @event.Should().NotBeNull();
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void Cannot_find_a_default_interface_event_if_you_dont_ask_for_that()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("DefaultEvent", MemberKind.Public);

            // Assert
            @event.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_default_interface_event_if_you_ask_for_it()
        {
            // Act
            var @event = typeof(ClassWithEvents).FindEvent("DefaultEvent", MemberKind.DefaultInterfaceProperties);

            // Assert
            @event.Should().NotBeNull();
        }
#endif
    }

    public class Constructors
    {
        [Fact]
        public void Can_get_the_public_constructors()
        {
            // Act
            var constructors = typeof(ClassWithConstructors).GetConstructors(MemberKind.Public);

            // Assert
            constructors.Should().HaveCount(2);
            constructors.Should().Contain(c => c.GetParameters().Length == 0);
            constructors.Should().Contain(c => c.GetParameters().Length == 1);
        }

        [Fact]
        public void Can_get_the_internal_constructors()
        {
            // Act
            var constructors = typeof(ClassWithConstructors).GetConstructors(MemberKind.Internal);

            // Assert
            constructors.Should().ContainSingle()
                .Which.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void Excludes_static_constructors_unless_asked_for()
        {
            // Act
            var constructors = typeof(ClassWithConstructors).GetConstructors(MemberKind.Public | MemberKind.Internal);

            // Assert
            constructors.Should().HaveCount(3);
        }

        [Fact]
        public void A_static_constructor_is_not_a_public_constructor()
        {
            // Act
            var constructors = typeof(ClassWithConstructors).GetConstructors(MemberKind.Public);

            // Assert
            constructors.Should().NotContain(c => c.IsStatic);
        }

        [Fact]
        public void A_private_constructor_is_not_a_public_constructor()
        {
            // Act
            var constructors = typeof(ClassWithOnlyPrivateConstructor).GetConstructors(MemberKind.Public);

            // Assert
            constructors.Should().BeEmpty();
        }

        [Fact]
        public void Can_get_a_private_constructor_if_you_ask_for_it()
        {
            // Act
            var constructors = typeof(ClassWithOnlyPrivateConstructor).GetConstructors(MemberKind.Private);

            // Assert
            constructors.Should().ContainSingle().Which.IsPrivate.Should().BeTrue();
        }

        [Fact]
        public void A_static_class_has_no_constructors_to_call()
        {
            // Act
            var constructors = typeof(StaticClass).GetConstructors(MemberKind.Public | MemberKind.Internal);

            // Assert
            constructors.Should().BeEmpty();
        }

        [Fact]
        public void Can_get_the_static_constructor_if_you_ask_for_it()
        {
            // Act
            var constructors = typeof(ClassWithConstructors).GetConstructors(MemberKind.Internal | MemberKind.Static);

            // Assert
            constructors.Should().ContainSingle().Which.IsStatic.Should().BeTrue();
        }

        [Fact]
        public void Can_find_a_parameterless_constructor()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Public);

            // Assert
            constructor.Should().NotBeNull();
            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void Omitting_the_parameter_types_finds_the_parameterless_constructor_rather_than_an_arbitrary_one()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Public);

            // Assert
            constructor.GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void A_type_without_a_parameterless_constructor_does_not_return_one_when_the_parameters_are_omitted()
        {
            // Act
            var constructor = typeof(ClassWithOnlyParameterizedConstructor).FindConstructor(MemberKind.Public);

            // Assert
            constructor.Should().BeNull();
        }

        [Fact]
        public void Can_find_a_constructor_with_specific_parameters()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Public, typeof(string));

            // Assert
            constructor.Should().NotBeNull();
            constructor.GetParameters().Should().ContainSingle().Which.ParameterType.Should().Be<string>();
        }

        [Fact]
        public void Cannot_find_an_internal_constructor_if_you_ask_for_public_ones()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Public, typeof(string), typeof(int));

            // Assert
            constructor.Should().BeNull();
        }

        [Fact]
        public void Can_find_an_internal_constructor_if_you_ask_for_it()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Internal, typeof(string), typeof(int));

            // Assert
            constructor.Should().NotBeNull();
        }

        [Fact]
        public void Returns_null_if_no_constructor_matches_the_requested_parameters()
        {
            // Act
            var constructor = typeof(ClassWithConstructors).FindConstructor(MemberKind.Public, typeof(bool));

            // Assert
            constructor.Should().BeNull();
        }

        [Fact]
        public void A_type_with_a_public_parameterless_constructor_next_to_other_public_ones_has_a_default_constructor()
        {
            // Act / Assert
            typeof(ClassWithConstructors).HasDefaultConstructor().Should().BeTrue();
        }

        [Fact]
        public void A_type_with_only_a_parameterized_public_constructor_has_no_default_constructor()
        {
            // Act / Assert
            typeof(ClassWithOnlyParameterizedConstructor).HasDefaultConstructor().Should().BeFalse();
        }

        [Fact]
        public void A_type_with_only_an_internal_parameterless_constructor_has_no_default_constructor()
        {
            // Act / Assert
            typeof(ClassWithOnlyInternalParameterlessConstructor).HasDefaultConstructor().Should().BeFalse();
        }

        [Fact]
        public void A_type_with_only_a_private_constructor_has_no_default_constructor()
        {
            // Act / Assert
            typeof(ClassWithOnlyPrivateConstructor).HasDefaultConstructor().Should().BeFalse();
        }

        [Fact]
        public void A_static_class_has_no_default_constructor()
        {
            // Act / Assert
            typeof(StaticClass).HasDefaultConstructor().Should().BeFalse();
        }

        [Fact]
        public void A_struct_always_has_a_default_constructor()
        {
            // Act / Assert
            typeof(StructWithoutExplicitConstructor).HasDefaultConstructor().Should().BeTrue();
        }

        private class ClassWithConstructors
        {
            [UsedImplicitly]
            static ClassWithConstructors()
            {
            }

            [UsedImplicitly]
            public ClassWithConstructors()
            {
            }

            [UsedImplicitly]
            public ClassWithConstructors(string text)
            {
            }

            [UsedImplicitly]
            internal ClassWithConstructors(string text, int number)
            {
            }
        }

        private class ClassWithOnlyPrivateConstructor
        {
            private ClassWithOnlyPrivateConstructor()
            {
            }
        }

        private static class StaticClass
        {
        }

        private class ClassWithOnlyParameterizedConstructor
        {
            [UsedImplicitly]
            public ClassWithOnlyParameterizedConstructor(string text)
            {
            }
        }

        private class ClassWithOnlyInternalParameterlessConstructor
        {
            [UsedImplicitly]
            internal ClassWithOnlyInternalParameterlessConstructor()
            {
            }
        }

#pragma warning disable CS0649 // Field is never assigned to - this field exists only for reflection metadata purposes.
        private struct StructWithoutExplicitConstructor
        {
            [UsedImplicitly]
            public int Value;
        }
#pragma warning restore CS0649
    }

    public class ConversionOperators
    {
        [Fact]
        public void Can_find_the_explicit_convertor_for_a_specific_source_and_target_type()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindExplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(int));

            // Assert
            convertor.Should().NotBeNull();
            convertor.ReturnType.Should().Be<int>();
        }

        [Fact]
        public void The_source_type_of_the_explicit_convertor_must_match()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindExplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(bool));

            // Assert
            convertor.Should().BeNull();
        }

        [Fact]
        public void Can_find_the_implicit_convertor_for_a_specific_source_and_target_type()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindImplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(string));

            // Assert
            convertor.Should().NotBeNull();
            convertor.ReturnType.Should().Be<string>();
        }

        [Fact]
        public void The_source_type_of_the_implicit_convertor_must_match()
        {
            // Act
            var convertor = typeof(ClassWithConversionOperators)
                .FindImplicitConversionOperator(typeof(ClassWithConversionOperators), typeof(bool));

            // Assert
            convertor.Should().BeNull();
        }

        private class ClassWithConversionOperators
        {
            [UsedImplicitly]
            public static explicit operator int(ClassWithConversionOperators instance) => 42;

            [UsedImplicitly]
            public static implicit operator string(ClassWithConversionOperators instance) => "42";
        }
    }

    private class SuperClass : BaseClass, IInterfaceWithDefaultProperty
    {
        public string NormalProperty { get; set; }

        public new int NewProperty { get; set; }

        public static bool StaticProperty { get; set; }

        protected static bool ProtectedStaticProperty { get; set; }

        private static bool PrivateStaticProperty { get; set; }

        internal bool InternalProperty { get; set; }

        protected internal bool InternalProtectedProperty { get; set; }

        protected bool ProtectedProperty { get; set; }

        private bool PrivateProperty { get; set; }

        string IInterfaceWithSingleProperty.ExplicitlyImplementedProperty { get; set; }

        public string InterfaceProperty { get; set; }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS0169 // Field is never used
#pragma warning disable CA1823 // Unused field
        public string NormalField;

        internal string InternalField;

        public static bool StaticField;

        protected internal string ProtectedInternalField;

        protected string protectedField;

        private string privateField;

        protected static string protectedStaticField;

        private static string privateStaticField;
#pragma warning restore CA1823 // Unused field
#pragma warning restore CS0169 // Field is never used
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    private class ClassWithFieldsOfEveryVisibility
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS0169 // Field is never used
#pragma warning disable CA1823 // Unused field
        public string PublicField;

        internal string InternalField;

        protected internal string ProtectedInternalField;

        protected string protectedField;

        private string privateField;

        protected static string protectedStaticField;

        private static string privateStaticField;
#pragma warning restore CA1823 // Unused field
#pragma warning restore CS0169 // Field is never used
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    private sealed class ClassWithExplicitAndNormalProperty : IInterfaceWithSingleProperty
    {
        string IInterfaceWithSingleProperty.ExplicitlyImplementedProperty { get; set; }

        [UsedImplicitly]
        public int ExplicitlyImplementedProperty { get; set; }
    }

    private class BaseClass
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS0169 // Field is never used
#pragma warning disable CA1823 // Unused field
        private string privateBaseField;
#pragma warning restore CA1823 // Unused field
#pragma warning restore CS0169 // Field is never used
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

        [UsedImplicitly]
        public string NewProperty { get; set; }
    }

    private interface IInterfaceWithDefaultProperty : IInterfaceWithSingleProperty
    {
        [UsedImplicitly]
        string InterfaceProperty { get; set; }

#if NETCOREAPP3_0_OR_GREATER
        [UsedImplicitly]
        string DefaultProperty => "Default";
#endif
    }

    private interface IInterfaceWithSingleProperty
    {
        [UsedImplicitly]
        string ExplicitlyImplementedProperty { get; set; }
    }

    private class ClassImplementingSetterOnlyInterface : IWithSetterOnlyProperty
    {
        public string WriteOnlyProperty { private get; set; }
    }

    private interface IWithSetterOnlyProperty
    {
        string WriteOnlyProperty { set; }
    }

    private sealed class ClassWithIndexer
    {
        [UsedImplicitly]
        public object Foo { get; set; }

        public string this[int n] => n.ToString(CultureInfo.InvariantCulture);

        internal string this[string s1, string s2] => s1 + "/" + s2;
    }

    private class ClassWithEvents : IInterfaceWithDefaultEvent
    {
#pragma warning disable CS0067 // Event is never used - these events exist only for reflection metadata purposes.
        [UsedImplicitly]
        public event EventHandler NormalEvent;

        [UsedImplicitly]
        public static event EventHandler StaticEvent;

        [UsedImplicitly]
        internal event EventHandler InternalEvent;

        [UsedImplicitly]
        protected event EventHandler ProtectedEvent;

        [UsedImplicitly]
        private event EventHandler PrivateEvent;
#pragma warning restore CS0067

        event EventHandler IInterfaceWithSingleEvent.ExplicitlyImplementedEvent
        {
            add { }
            remove { }
        }
    }

    private sealed class ClassWithExplicitAndNormalEvent : IInterfaceWithSingleEvent
    {
        event EventHandler IInterfaceWithSingleEvent.ExplicitlyImplementedEvent
        {
            add { }
            remove { }
        }

#pragma warning disable CS0067 // Event is never used - this event exists only for reflection metadata purposes.
        [UsedImplicitly]
        public event EventHandler ExplicitlyImplementedEvent;
#pragma warning restore CS0067
    }

    private interface IInterfaceWithDefaultEvent : IInterfaceWithSingleEvent
    {
#if NETCOREAPP3_0_OR_GREATER
        event EventHandler DefaultEvent
        {
            add { }
            remove { }
        }
#endif
    }

    private interface IInterfaceWithSingleEvent
    {
        event EventHandler ExplicitlyImplementedEvent;
    }

#pragma warning disable CS0067 // Event is never used - these events exist only for reflection metadata purposes.
    private class BaseClassWithEvents
    {
        [UsedImplicitly]
        public event EventHandler BaseEvent;

        [UsedImplicitly]
        public event EventHandler HiddenEvent;
    }

    private sealed class DerivedClassWithEvents : BaseClassWithEvents
    {
        [UsedImplicitly]
        public new event EventHandler HiddenEvent;

        [UsedImplicitly]
        public event EventHandler DerivedEvent;
    }

    private class BaseClassWithExplicitEvent : IInterfaceWithSingleEvent
    {
        event EventHandler IInterfaceWithSingleEvent.ExplicitlyImplementedEvent
        {
            add { }
            remove { }
        }
    }

    private sealed class DerivedClassWithNormalEvent : BaseClassWithExplicitEvent
    {
        [UsedImplicitly]
        public event EventHandler ExplicitlyImplementedEvent;
    }

    private class BaseClassWithNormalEvent
    {
        [UsedImplicitly]
        public event EventHandler ExplicitlyImplementedEvent;
    }
#pragma warning restore CS0067

    private sealed class DerivedClassWithExplicitEvent : BaseClassWithNormalEvent, IInterfaceWithSingleEvent
    {
        event EventHandler IInterfaceWithSingleEvent.ExplicitlyImplementedEvent
        {
            add { }
            remove { }
        }
    }
}

