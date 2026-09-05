using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete

namespace Reflectify.Specs;

public class TypeMetaDataExtensionsSpecs
{
    public class GetNonGenericName
    {
        [Theory]
        [InlineData(typeof(string), "String")]
        [InlineData(typeof(List<string>), "List")]
        [InlineData(typeof(Dictionary<int, string>), "Dictionary")]
        [InlineData(typeof(int?), "Nullable")]
        public void Can_get_non_generic_name(Type type, string expectedName)
        {
            // Act
            string result = type.GetNonGenericName();

            // Assert
            result.Should().Be(expectedName);
        }
    }

    public class IsDerivedFromOpenGeneric
    {
        [Fact]
        public void Can_detect_a_type_derived_from_an_open_generic_type()
        {
            // Act
            bool result = typeof(DerivedFromOpenGeneric).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_open_generic_type_cannot_derive_from_itself()
        {
            // Act
            bool result = typeof(OpenGenericClass<>).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_unrelated_class_is_not_going_to_match_an_open_generic_type()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsDerivedFromOpenGeneric(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }
    }

    public class GetClosedGenericInterfaces
    {
        [Fact]
        public void Returns_nothing_if_the_class_does_not_implement_the_open_generic_interface()
        {
            // Act
            Type[] results = typeof(DerivedFromOpenGeneric).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Returns_nothing_if_an_interface_does_not_inherit_any_closed_generic_interface()
        {
            // Act
            Type[] results = typeof(ISomeOtherInterface).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Returns_nothing_if_a_class_does_not_implement_any_closed_generic_interface()
        {
            // Act
            Type[] results = typeof(SomeOtherClass).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Can_find_closed_generic_interfaces()
        {
            // Act
            Type[] results =
                typeof(TypeImplementingClosedGenericInterface).GetClosedGenericInterfaces(typeof(IOpenGenericInterface<>));

            // Assert
            results.Should().BeEquivalentTo([typeof(IClosedGenericInterface), typeof(IAnotherClosedGenericInterface)]);
        }
    }

    public class HasAttribute
    {
        [Fact]
        public void Can_determine_an_attribute_exists_on_a_specific_type()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<InheritableAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_derived_attribute_exists_on_a_specific_type()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<Attribute>().Should().BeTrue();
        }

        [Fact]
        public void The_attribute_must_be_applied_directly_to_the_type()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute).HasAttribute<CollectionDefinitionAttribute>()
                .Should().BeFalse();
        }

        [Fact]
        public void Can_determine_that_the_attribute_does_not_exist()
        {
            // Act / Assert
            typeof(ClassWithAttribute).HasAttribute<ObsoleteAttribute>().Should().BeFalse();
        }

        [Fact]
        public void Can_check_that_an_attribute_has_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<InheritableAttribute>(a => a.Message!.Contains("First"));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_check_that_a_derived_attribute_has_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<Attribute>(a => !a.IsDefaultAttribute());

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_attribute_with_a_property_must_be_applied_directly_to_the_type()
        {
            // Act
            bool result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(a => a.DisableParallelization);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Can_check_that_an_attribute_doesnt_have_a_specific_property()
        {
            // Act
            bool result = typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(a => !a.DisableParallelization);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_attribute_predicate_cannot_be_null()
        {
            // Act
            Action act = () => typeof(ClassWithInheritableAndParameterizedAttribute)
                .HasAttribute<CollectionDefinitionAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class HasAttributeInHierarchy
    {
        [Fact]
        public void Can_find_an_attribute_in_a_base_class()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_find_an_attribute_with_a_specific_property()
        {
            // Act
            var result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>(a => a.Message == "FirstAttribute");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Ignores_the_attribute_if_the_predicate_does_not_match()
        {
            // Act
            var result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<InheritableAttribute>(a => a.Message == "Other Message");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Cannot_find_an_attribute_on_a_base_class_if_its_not_inheritable()
        {
            // Act / Assert
            typeof(ClassDerivedFromOneWithNonInheritableAndParameterizedAttribute)
                .HasAttributeInHierarchy<NonInheritableAttribute>().Should().BeFalse();
        }

        [Fact]
        public void Cannot_find_an_attribute_if_none_exist()
        {
            // Act / Assert
            typeof(SomeOtherClass).HasAttributeInHierarchy<InheritableAttribute>().Should().BeFalse();
        }

        [Fact]
        public void A_predicate_cannot_be_null()
        {
            // Act
            Action act = () => typeof(ClassWithAttribute).HasAttributeInHierarchy<InheritableAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class IsObsolete
    {
        [Fact]
        public void An_obsolete_type_is_obsolete()
        {
            // Act
            bool result = typeof(ObsoleteType).IsObsolete();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_normal_type_is_not_obsolete()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsObsolete();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_the_obsolescence_message()
        {
            // Act
            string message = typeof(ObsoleteType).GetObsoleteMessage();

            // Assert
            message.Should().Be("Use another type");
        }

        [Fact]
        public void Returns_no_message_when_the_obsolete_type_has_no_message()
        {
            // Act
            string message = typeof(ObsoleteTypeWithoutMessage).GetObsoleteMessage();

            // Assert
            message.Should().BeNull();
        }

        [Obsolete("Use another type")]
        private class ObsoleteType
        {
        }

        [Obsolete]
        private class ObsoleteTypeWithoutMessage
        {
        }
    }

    public class GetAttribute
    {
        [Fact]
        public void Returns_null_when_no_matching_attribute_exists()
        {
            // Act
            var result = typeof(SomeOtherClass).GetAttribute<InheritableAttribute>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Returns_the_single_matching_attribute()
        {
            // Act
            var result = typeof(ClassWithAttribute).GetAttribute<InheritableAttribute>();

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("SomeMessage");
        }

        [Fact]
        public void Returns_the_first_attribute_when_multiple_are_present()
        {
            // Act
            var result = typeof(ClassWithInheritableAndParameterizedAttribute).GetAttribute<InheritableAttribute>();

            // Assert
            result.Should().BeEquivalentTo(new { Message = "FirstAttribute" });
        }

        [Fact]
        public void Considers_attributes_declared_on_a_base_class()
        {
            // Act
            var result = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetAttribute<InheritableAttribute>();
            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeNull();
        }
    }

    public class GetMatchingAttributes
    {
        [Fact]
        public void Can_find_all_attributes_of_a_specific_type()
        {
            // Act
            InheritableAttribute[] results = typeof(ClassWithAttribute).GetMatchingAttributes<InheritableAttribute>();

            // Assert
            results.Should().ContainSingle().Which.Message.Should().Be("SomeMessage");
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_in_a_derived_class()
        {
            // Act
            var results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>();

            // Assert
            results.Should().BeEquivalentTo([
                new { Message = "FirstAttribute", },
                new { Message = "SecondAttribute" }
            ]);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property()
        {
            // Act
            var results = typeof(ClassWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property_in_a_derived_class()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Can_find_all_attributes_of_a_specific_type_with_a_specific_property_in_a_derived_class_and_base_class()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("First"));

            // Assert
            results.Should().HaveCount(1);
        }

        [Fact]
        public void Will_apply_the_predicate_when_finding_attributes()
        {
            // Act
            Attribute[] results = typeof(ClassDerivedFromOneWithInheritableAndParameterizedAttribute)
                .GetMatchingAttributes<InheritableAttribute>(a => a.Message.Contains("WrongValue"));

            // Assert
            results.Should().HaveCount(0);
        }

        [Fact]
        public void A_predicate_must_be_valid()
        {
            // Act
            var act = () => typeof(ClassWithAttribute).HasAttributeInHierarchy<InheritableAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class OverridesEquals
    {
        [Fact]
        public void Can_detect_if_a_type_implements_equality()
        {
            // Act
            bool result = typeof(string).OverridesEquals();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_that_a_type_implements_reference_equality()
        {
            // Act
            bool result = typeof(SomeOtherClass).OverridesEquals();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsSameOrInheritsGeneric
    {
        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another()
        {
            // Act
            bool result = typeof(string).IsSameOrInherits<string>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another()
        {
            // Act
            bool result = typeof(DerivedFromOpenGeneric).IsSameOrInherits<OpenGenericClass<string>>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsSameOrInherits<OpenGenericClass<string>>();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another_interface()
        {
            // Act
            bool result = typeof(IOpenGenericInterface<string>).IsSameOrInherits<IOpenGenericInterface<string>>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another_interface()
        {
            // Act
            bool result = typeof(IClosedGenericInterface).IsSameOrInherits<IOpenGenericInterface<string>>();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another_interface()
        {
            // Act
            bool result = typeof(ISomeOtherInterface).IsSameOrInherits<IOpenGenericInterface<string>>();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsSameOrInherits
    {
        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another()
        {
            // Act
            bool result = typeof(string).IsSameOrInherits(typeof(string));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another()
        {
            // Act
            bool result = typeof(DerivedFromOpenGeneric).IsSameOrInherits(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsSameOrInherits(typeof(OpenGenericClass<>));

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Can_detect_if_a_type_is_the_same_as_another_interface()
        {
            // Act
            bool result = typeof(IOpenGenericInterface<string>).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_derived_from_another_interface()
        {
            // Act
            bool result = typeof(IClosedGenericInterface).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Can_detect_if_a_type_is_not_derived_from_another_interface()
        {
            // Act
            bool result = typeof(ISomeOtherInterface).IsSameOrInherits(typeof(IOpenGenericInterface<string>));

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsCompilerGenerated
    {
        [Fact]
        public void An_anonymous_type_is_compiler_generated()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_tuple_is_compiler_generated()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_record_is_compiler_generated()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_custom_type_is_never_compiler_generated()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsCompilerGenerated();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_type_with_compiler_generated_attribute_is_compiler_generated()
        {
            // Act
            bool result = typeof(TypeWithCompilerGeneratedAttribute).IsCompilerGenerated();

            // Assert
            result.Should().BeTrue();
        }

        [System.Runtime.CompilerServices.CompilerGenerated]
        private class TypeWithCompilerGeneratedAttribute
        {
        }

        private record SomeRecord(string SomeProperty);
    }

    public class HasFriendlyName
    {
        [Fact]
        public void A_normal_class_has_a_friendly_name()
        {
            // Act
            typeof(SomeOtherClass).HasFriendlyName().Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_does_not_have_a_friendly_name()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act / Assert
            subject.GetType().HasFriendlyName().Should().BeFalse();
        }

        [Fact]
        public void A_tuple_does_not_have_a_friendly_name()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act / Assert
            subject.GetType().HasFriendlyName().Should().BeFalse();
        }
    }

    public class IsTuple
    {
        [Fact]
        public void A_normal_class_is_not_a_tuple()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsTuple();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_anonymous_type_is_not_a_tuple()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsTuple();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_tuple_is_a_tuple()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsTuple();

            // Assert
            result.Should().BeTrue();
        }
    }

    public class IsAnonymous
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsAnonymous();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_anonymous_type_is()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsAnonymous();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_tuple_is_not()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            bool result = subject.GetType().IsAnonymous();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsRecord
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecord();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_record_is()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsRecord();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecord();

            // Assert
            result.Should().BeFalse();
        }

        private record SomeRecord(string SomeProperty);
    }

    public class IsRecordClass
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecordClass();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_record_is()
        {
            // Act
            bool result = new SomeRecord("PropertyValue").GetType().IsRecordClass();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecordClass();

            // Assert
            result.Should().BeFalse();
        }

        private record SomeRecord(string SomeProperty);
    }

    public class IsRecordStruct
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_class_record_is_not()
        {
            // Act
            bool result = new SomeClassRecord("PropertyValue").GetType().IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_struct_record_is()
        {
            // Act
            bool result = new SomeStructRecord("PropertyValue").GetType().IsRecordStruct();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsRecordStruct();

            // Assert
            result.Should().BeFalse();
        }

        private record struct SomeStructRecord(string SomeProperty);

        private record SomeClassRecord(string SomeProperty);
    }

    public class IsDelegate
    {
        [Fact]
        public void A_delegate_is_a_delegate()
        {
            // Act
            bool result = typeof(Action).IsDelegate();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_custom_delegate_is_a_delegate()
        {
            // Act
            bool result = typeof(CustomDelegate).IsDelegate();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_class_is_not_a_delegate()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsDelegate();

            // Assert
            result.Should().BeFalse();
        }

        private delegate void CustomDelegate();
    }

    public class IsStruct
    {
        [Fact]
        public void A_struct_is()
        {
            // Act
            bool result = typeof(SomeStruct).IsStruct();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_enum_is_not()
        {
            // Act
            bool result = typeof(SomeEnum).IsStruct();

            // Assert
            result.Should().BeFalse();
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [Fact]
        public void A_memory_of_T_is()
        {
            // Act
            bool result = typeof(Memory<int>).IsStruct();

            // Assert
            result.Should().BeTrue();
        }
#endif

        private struct SomeStruct
        {
        }

        private enum SomeEnum
        {
            Value
        }
    }

    public class IsRefStruct
    {
        [Fact]
        public void A_regular_struct_is_not()
        {
            // Act
            bool result = typeof(SomeStruct).IsRefStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsRefStruct();

            // Assert
            result.Should().BeFalse();
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [Fact]
        public void A_ref_struct_is()
        {
            // Act
            bool result = typeof(SomeRefStruct).IsRefStruct();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_span_of_T_is()
        {
            // Act
            bool result = typeof(Span<int>).IsRefStruct();

            // Assert
            result.Should().BeTrue();
        }

        private ref struct SomeRefStruct
        {
        }
#endif

        private struct SomeStruct
        {
        }
    }

    public class IsReadOnlyStruct
    {
        [Fact]
        public void A_readonly_struct_is()
        {
            // Act
            bool result = typeof(SomeReadOnlyStruct).IsReadOnlyStruct();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_normal_struct_is_not()
        {
            // Act
            bool result = typeof(SomeStruct).IsReadOnlyStruct();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsReadOnlyStruct();

            // Assert
            result.Should().BeFalse();
        }

        private readonly struct SomeReadOnlyStruct
        {
        }

        private struct SomeStruct
        {
        }
    }

    public class IsFileLocal
    {
        [Fact]
        public void A_file_local_type_is()
        {
            // Act
            bool result = typeof(FileLocalTestType).IsFileLocal();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsFileLocal();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsFileLocal();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsKeyValuePair
    {
        [Fact]
        public void A_normal_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsKeyValuePair();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_key_value_pair_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_string_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_int_is()
        {
            // Act
            bool result = typeof(KeyValuePair<int, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_string_and_int_is()
        {
            // Act
            bool result = typeof(KeyValuePair<string, int>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_key_value_pair_of_int_and_string_is()
        {
            // Act
            bool result = typeof(KeyValuePair<int, string>).IsKeyValuePair();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_anonymous_type_is_not()
        {
            // Arrange
            var subject = new { SomeProperty = "SomeValue" };

            // Act
            bool result = subject.GetType().IsKeyValuePair();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class NullableOrActualType
    {
        [Fact]
        public void Returns_underlying_type_for_nullable_type()
        {
            // Act
            var result = typeof(int?).NullableOrActualType();

            // Assert
            result.Should().Be<int>();
        }

        [Fact]
        public void Returns_same_type_for_non_nullable_type()
        {
            // Act
            var result = typeof(int).NullableOrActualType();

            // Assert
            result.Should().Be<int>();
        }

        [Fact]
        public void Returns_same_type_for_reference_type()
        {
            // Act
            var result = typeof(string).NullableOrActualType();

            // Assert
            result.Should().Be<string>();
        }
    }

    public class GetFriendlyName
    {
        [Theory]
        [InlineData(typeof(int), "int")]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(bool), "bool")]
        [InlineData(typeof(double), "double")]
        [InlineData(typeof(float), "float")]
        [InlineData(typeof(decimal), "decimal")]
        [InlineData(typeof(long), "long")]
        [InlineData(typeof(short), "short")]
        [InlineData(typeof(byte), "byte")]
        [InlineData(typeof(sbyte), "sbyte")]
        [InlineData(typeof(uint), "uint")]
        [InlineData(typeof(ulong), "ulong")]
        [InlineData(typeof(ushort), "ushort")]
        [InlineData(typeof(char), "char")]
        [InlineData(typeof(object), "object")]
        [InlineData(typeof(void), "void")]
        public void Maps_primitive_types_to_their_csharp_keyword_alias(Type type, string expectedName)
        {
            // Act
            string result = type.GetFriendlyName();

            // Assert
            result.Should().Be(expectedName);
        }

        [Fact]
        public void Renders_a_simple_class_by_its_name()
        {
            // Act
            string result = typeof(SomeOtherClass).GetFriendlyName();

            // Assert
            result.Should().Be("TypeMetaDataExtensionsSpecs.SomeOtherClass");
        }

        [Fact]
        public void Renders_a_nullable_value_type_with_a_question_mark()
        {
            // Act
            string result = typeof(int?).GetFriendlyName();

            // Assert
            result.Should().Be("int?");
        }

        [Fact]
        public void Renders_a_single_dimensional_array_with_square_brackets()
        {
            // Act
            string result = typeof(int[]).GetFriendlyName();

            // Assert
            result.Should().Be("int[]");
        }

        [Fact]
        public void Renders_a_multi_dimensional_array_respecting_its_rank()
        {
            // Act
            string result = typeof(int[,]).GetFriendlyName();

            // Assert
            result.Should().Be("int[,]");
        }

        [Fact]
        public void Renders_an_array_of_a_reference_type()
        {
            // Act
            string result = typeof(SomeOtherClass[]).GetFriendlyName();

            // Assert
            result.Should().Be("TypeMetaDataExtensionsSpecs.SomeOtherClass[]");
        }

        [Fact]
        public void Renders_a_nested_type_using_a_dot_instead_of_a_plus()
        {
            // Act
            string result = typeof(OuterType.InnerType).GetFriendlyName();

            // Assert
            result.Should().Be("TypeMetaDataExtensionsSpecs.OuterType.InnerType");
        }

        [Fact]
        public void Renders_a_generic_type_with_a_single_type_argument()
        {
            // Act
            string result = typeof(List<int>).GetFriendlyName();

            // Assert
            result.Should().Be("List<int>");
        }

        [Fact]
        public void Renders_a_generic_type_with_multiple_type_arguments()
        {
            // Act
            string result = typeof(Dictionary<string, int>).GetFriendlyName();

            // Assert
            result.Should().Be("Dictionary<string, int>");
        }

        [Fact]
        public void Renders_nested_generic_types_recursively()
        {
            // Act
            string result = typeof(Dictionary<string, List<int>>).GetFriendlyName();

            // Assert
            result.Should().Be("Dictionary<string, List<int>>");
        }

        [Fact]
        public void Renders_an_open_generic_type_definition_using_the_generic_parameter_name()
        {
            // Act
            string result = typeof(List<>).GetFriendlyName();

            // Assert
            result.Should().Be("List<T>");
        }

        [Fact]
        public void Renders_a_tuple_structurally_using_the_friendly_names_of_its_elements()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            string result = subject.GetType().GetFriendlyName();

            // Assert
            result.Should().Be("(string, int)");
        }

        [Fact]
        public void Renders_an_anonymous_type_structurally_using_its_property_names()
        {
            // Arrange
            var subject = new { Name = "SomeValue", Age = 42 };

            // Act
            string result = subject.GetType().GetFriendlyName();

            // Assert
            result.Should().Be("{ Name, Age }");
        }
    }

    public class GetFullFriendlyName
    {
        [Fact]
        public void Renders_a_simple_class_using_its_full_namespace()
        {
            // Act
            string result = typeof(SomeOtherClass).GetFullFriendlyName();

            // Assert
            result.Should().Be("Reflectify.Specs.TypeMetaDataExtensionsSpecs+SomeOtherClass".Replace('+', '.'));
        }

        [Fact]
        public void Renders_a_nested_type_using_the_full_namespace_and_dots()
        {
            // Act
            string result = typeof(OuterType.InnerType).GetFullFriendlyName();

            // Assert
            result.Should().Be("Reflectify.Specs.TypeMetaDataExtensionsSpecs+OuterType+InnerType".Replace('+', '.'));
        }

        [Fact]
        public void Renders_generic_type_arguments_with_their_full_namespace_too()
        {
            // Act
            string result = typeof(Dictionary<string, List<int>>).GetFullFriendlyName();

            // Assert
            result.Should().Be("System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>");
        }

        [Fact]
        public void Still_maps_primitive_types_to_their_csharp_keyword_alias()
        {
            // Act
            string result = typeof(int).GetFullFriendlyName();

            // Assert
            result.Should().Be("int");
        }

        [Fact]
        public void Renders_a_nullable_value_type_with_a_question_mark()
        {
            // Act
            string result = typeof(int?).GetFullFriendlyName();

            // Assert
            result.Should().Be("int?");
        }

        [Fact]
        public void Renders_an_array_respecting_its_rank()
        {
            // Act
            string result = typeof(int[,]).GetFullFriendlyName();

            // Assert
            result.Should().Be("int[,]");
        }

        [Fact]
        public void Renders_a_tuple_structurally_using_the_full_friendly_names_of_its_elements()
        {
            // Arrange
            var subject = (SomeProperty: "SomeValue", SomeOtherProperty: 42);

            // Act
            string result = subject.GetType().GetFullFriendlyName();

            // Assert
            result.Should().Be("(string, int)");
        }

        [Fact]
        public void Renders_an_anonymous_type_structurally_using_its_property_names()
        {
            // Arrange
            var subject = new { Name = "SomeValue", Age = 42 };

            // Act
            string result = subject.GetType().GetFullFriendlyName();

            // Assert
            result.Should().Be("{ Name, Age }");
        }
    }

    private class OuterType
    {
        public class InnerType
        {
        }
    }

    public class IsNullable
    {
        [Fact]
        public void A_nullable_value_type_is()
        {
            // Act
            bool result = typeof(int?).IsNullable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_non_nullable_value_type_is_not()
        {
            // Act
            bool result = typeof(int).IsNullable();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_reference_type_is_not()
        {
            // Act
            bool result = typeof(string).IsNullable();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsEnumerable
    {
        [Fact]
        public void A_list_is()
        {
            // Act
            bool result = typeof(List<int>).IsEnumerable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void An_array_is()
        {
            // Act
            bool result = typeof(int[]).IsEnumerable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_plain_non_generic_enumerable_is()
        {
            // Act
            bool result = typeof(PlainEnumerable).IsEnumerable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_string_is_explicitly_excluded()
        {
            // Act
            bool result = typeof(string).IsEnumerable();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_plain_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsEnumerable();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class GetElementTypeOfEnumerable
    {
        [Fact]
        public void Returns_the_element_type_of_a_list()
        {
            // Act
            Type result = typeof(List<string>).GetElementTypeOfEnumerable();

            // Assert
            result.Should().Be(typeof(string));
        }

        [Fact]
        public void Returns_the_element_type_of_an_array()
        {
            // Act
            Type result = typeof(int[]).GetElementTypeOfEnumerable();

            // Assert
            result.Should().Be(typeof(int));
        }

        [Fact]
        public void Returns_object_for_a_type_implementing_ienumerable_of_t_more_than_once()
        {
            // Act
            Type result = typeof(EnumerableOfIntAndString).GetElementTypeOfEnumerable();

            // Assert
            result.Should().Be(typeof(object));
        }

        [Fact]
        public void Returns_object_for_a_plain_non_generic_enumerable()
        {
            // Act
            Type result = typeof(PlainEnumerable).GetElementTypeOfEnumerable();

            // Assert
            result.Should().Be(typeof(object));
        }

        [Fact]
        public void Returns_null_for_a_string()
        {
            // Act
            Type result = typeof(string).GetElementTypeOfEnumerable();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Returns_null_for_a_non_enumerable_type()
        {
            // Act
            Type result = typeof(SomeOtherClass).GetElementTypeOfEnumerable();

            // Assert
            result.Should().BeNull();
        }
    }

    public class IsDictionary
    {
        [Fact]
        public void A_dictionary_is()
        {
            // Act
            bool result = typeof(Dictionary<string, int>).IsDictionary();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_read_only_dictionary_is()
        {
            // Act
            bool result = typeof(IReadOnlyDictionary<string, int>).IsDictionary();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_list_is_not()
        {
            // Act
            bool result = typeof(List<string>).IsDictionary();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_plain_class_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsDictionary();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class TryGetDictionaryTypes
    {
        [Fact]
        public void Can_get_the_key_and_value_types_of_a_dictionary()
        {
            // Act
            bool result = typeof(Dictionary<string, int>).TryGetDictionaryTypes(out Type keyType, out Type valueType);

            // Assert
            result.Should().BeTrue();
            keyType.Should().Be(typeof(string));
            valueType.Should().Be(typeof(int));
        }

        [Fact]
        public void Can_get_the_key_and_value_types_of_a_read_only_dictionary()
        {
            // Act
            bool result = typeof(IReadOnlyDictionary<string, int>).TryGetDictionaryTypes(out Type keyType, out Type valueType);

            // Assert
            result.Should().BeTrue();
            keyType.Should().Be(typeof(string));
            valueType.Should().Be(typeof(int));
        }

        [Fact]
        public void Returns_false_for_a_non_dictionary_type()
        {
            // Act
            bool result = typeof(List<string>).TryGetDictionaryTypes(out Type keyType, out Type valueType);

            // Assert
            result.Should().BeFalse();
            keyType.Should().BeNull();
            valueType.Should().BeNull();
        }
    }

    public class IsAwaitable
    {
        [Fact]
        public void A_task_is()
        {
            // Act
            bool result = typeof(Task).IsAwaitable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_task_of_t_is()
        {
            // Act
            bool result = typeof(Task<int>).IsAwaitable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_value_task_is()
        {
            // Act
            bool result = typeof(ValueTask).IsAwaitable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_value_task_of_t_is()
        {
            // Act
            bool result = typeof(ValueTask<int>).IsAwaitable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_custom_duck_typed_awaitable_is()
        {
            // Act
            bool result = typeof(CustomAwaitable).IsAwaitable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_non_awaitable_type_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsAwaitable();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsTaskLike
    {
        [Fact]
        public void A_task_is()
        {
            // Act
            bool result = typeof(Task).IsTaskLike();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_task_of_t_is()
        {
            // Act
            bool result = typeof(Task<int>).IsTaskLike();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_value_task_is()
        {
            // Act
            bool result = typeof(ValueTask).IsTaskLike();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_value_task_of_t_is()
        {
            // Act
            bool result = typeof(ValueTask<int>).IsTaskLike();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_custom_duck_typed_awaitable_is_not()
        {
            // Act
            bool result = typeof(CustomAwaitable).IsTaskLike();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_non_awaitable_type_is_not()
        {
            // Act
            bool result = typeof(SomeOtherClass).IsTaskLike();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsNumeric
    {
        [Theory]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        public void A_numeric_type_is(Type type)
        {
            // Act
            bool result = type.IsNumeric();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(string))]
        [InlineData(typeof(SomeOtherClass))]
        [InlineData(typeof(int?))]
        public void A_non_numeric_type_is_not(Type type)
        {
            // Act
            bool result = type.IsNumeric();

            // Assert
            result.Should().BeFalse();
        }
    }

    public class IsPrimitiveOrString
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(double))]
        [InlineData(typeof(string))]
        public void A_primitive_or_string_type_is(Type type)
        {
            // Act
            bool result = type.IsPrimitiveOrString();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(SomeOtherClass))]
        [InlineData(typeof(int?))]
        public void A_non_primitive_and_non_string_type_is_not(Type type)
        {
            // Act
            bool result = type.IsPrimitiveOrString();

            // Assert
            result.Should().BeFalse();
        }
    }

    private class PlainEnumerable : IEnumerable
    {
        public IEnumerator GetEnumerator() => throw new NotSupportedException();
    }

    private class EnumerableOfIntAndString : IEnumerable<int>, IEnumerable<string>
    {
        IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotSupportedException();

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }

    private class CustomAwaitable
    {
        public CustomAwaiter GetAwaiter() => new();
    }

    private class CustomAwaiter : INotifyCompletion
    {
        public bool IsCompleted => true;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation) => continuation();
    }

    private class OpenGenericClass<T>
    {
        private T Property { get; set; }
    }

    private class DerivedFromOpenGeneric : OpenGenericClass<string>
    {
    }

    private class SomeOtherClass
    {
    }

    private interface IOpenGenericInterface<T>
    {
    }

    private interface IClosedGenericInterface : IOpenGenericInterface<string>
    {
    }

    private interface IAnotherClosedGenericInterface : IOpenGenericInterface<string>
    {
    }

    private class TypeImplementingClosedGenericInterface : IClosedGenericInterface, IAnotherClosedGenericInterface
    {
    }

    private interface ISomeOtherInterface
    {
    }

    [Inheritable("SomeMessage")]
    private class ClassWithAttribute
    {
    }

    [Inheritable("FirstAttribute")]
    [Inheritable("SecondAttribute")]
    private class ClassWithInheritableAndParameterizedAttribute
    {
    }

    private class ClassDerivedFromOneWithInheritableAndParameterizedAttribute : ClassWithInheritableAndParameterizedAttribute
    {
    }

    [NonInheritable("SomeMessage")]
    private class ClassWithNonInheritableAndParameterizedAttribute
    {
    }

    private class ClassDerivedFromOneWithNonInheritableAndParameterizedAttribute : ClassWithInheritableAndParameterizedAttribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    private sealed class InheritableAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
    }

    [AttributeUsage(AttributeTargets.All)]
    private sealed class NonInheritableAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
    }
}

// C# `file` classes can only be tested via a fixture type declared with the `file` modifier at file (top-level)
// scope in the same source file, so it lives here rather than nested inside TypeMetaDataExtensionsSpecs.
file class FileLocalTestType
{
}
