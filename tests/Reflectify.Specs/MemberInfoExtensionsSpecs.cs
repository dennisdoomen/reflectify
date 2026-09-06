using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace Reflectify.Specs;

public class MemberInfoExtensionsSpecs
{
    public class HasAttribute
    {
        [Fact]
        public void Can_determine_a_method_has_an_attribute()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act / Assert
            member.HasAttribute<ObsoleteAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_method_has_an_attribute_using_a_specific_predicate()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act / Assert
            member.HasAttribute<ObsoleteAttribute>(attribute =>
                attribute.Message!.StartsWith("Specific")).Should().BeTrue();
        }

        [Fact]
        public void The_predicate_must_not_be_null()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var act = () => member.HasAttribute<ObsoleteAttribute>(null).Should().BeTrue();

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
        }

        [Fact]
        public void Can_determine_a_method_has_an_attribute_that_does_not_meet_a_predicate()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act / Assert
            member.HasAttribute<ObsoleteAttribute>(predicate =>
                predicate.Message.Contains("*Other*")).Should().BeFalse();
        }

        [Fact]
        public void Does_not_find_an_inheritable_attribute_on_an_overridden_method_using_a_predicate()
        {
            // Arrange
            var member = typeof(DerivedClass).GetMethod("Method");

            // Act / Assert
            member.HasAttribute<InheritableAttribute>(_ => true).Should().BeFalse();
        }

        [Fact]
        public void Does_not_find_an_inheritable_attribute_on_an_overridden_property_using_a_predicate()
        {
            // Arrange
            var member = typeof(DerivedClass).GetProperty("Property");

            // Act / Assert
            member.HasAttribute<InheritableAttribute>(_ => true).Should().BeFalse();
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
        private sealed class InheritableAttribute : Attribute
        {
        }

        private class BaseClass
        {
            [Inheritable]
            public virtual void Method()
            {
            }

            [Inheritable]
            public virtual string Property => "";
        }

        private class DerivedClass : BaseClass
        {
            public override void Method()
            {
            }

            public override string Property => "";
        }
    }

    public class GetAttribute
    {
        [Fact]
        public void Returns_null_when_the_member_has_no_matching_attribute()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var result = member.GetAttribute<CLSCompliantAttribute>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Returns_the_single_matching_attribute()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var result = member.GetAttribute<ObsoleteAttribute>();

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Specific reason");
        }

        [Fact]
        public void Returns_the_first_attribute_when_multiple_are_present()
        {
            // Arrange
            var member = typeof(ClassWithMultipleAttributesOnMember).GetMethod("Method");

            // Act
            var result = member.GetAttribute<MultiValuedAttribute>();

            // Assert
            result.Should().BeEquivalentTo(new { Tag = "First" });
        }

        [Fact]
        public void Does_not_consider_attributes_that_are_only_defined_in_a_base_class()
        {
            // Arrange
            var member = typeof(DerivedClassWithoutOwnAttribute).GetMethod("Method");

            // Act
            var result = member.GetAttribute<MarkerAttribute>();

            // Assert
            result.Should().BeNull();
        }

        private class ClassWithMultipleAttributesOnMember
        {
            [MultiValued("First")]
            [MultiValued("Second")]
            public void Method()
            {
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private sealed class MultiValuedAttribute(string tag) : Attribute
        {
            public string Tag { get; } = tag;
        }

        [AttributeUsage(AttributeTargets.Method)]
        private sealed class MarkerAttribute : Attribute
        {
        }

        private class BaseClassWithAttribute
        {
            [Marker]
            public virtual void Method()
            {
            }
        }

        private class DerivedClassWithoutOwnAttribute : BaseClassWithAttribute
        {
            public override void Method()
            {
            }
        }
    }

    public class GetMatchingAttributes
    {
        [Fact]
        public void Returns_an_empty_array_when_the_member_has_no_matching_attribute()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var result = member.GetMatchingAttributes<CLSCompliantAttribute>();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Returns_the_single_matching_attribute()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var result = member.GetMatchingAttributes<ObsoleteAttribute>();

            // Assert
            result.Should().ContainSingle().Which.Message.Should().Be("Specific reason");
        }

        [Fact]
        public void Returns_all_attributes_when_multiple_are_present()
        {
            // Arrange
            var member = typeof(ClassWithMultipleRepeatableAttributes).GetMethod("Method");

            // Act
            var result = member.GetMatchingAttributes<RepeatableAttribute>();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Can_filter_the_matching_attributes_using_a_predicate()
        {
            // Arrange
            var member = typeof(ClassWithMultipleRepeatableAttributes).GetMethod("Method");

            // Act
            var result = member.GetMatchingAttributes<RepeatableAttribute>(a => a.Tag == "First");

            // Assert
            result.Should().ContainSingle().Which.Tag.Should().Be("First");
        }

        [Fact]
        public void The_predicate_must_not_be_null()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var act = () => member.GetMatchingAttributes<ObsoleteAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
        }

        [Fact]
        public void Does_not_consider_attributes_that_are_only_defined_in_a_base_class()
        {
            // Arrange
            var member = typeof(DerivedClassWithoutOwnMatchingAttribute).GetMethod("Method");

            // Act
            var result = member.GetMatchingAttributes<MarkerAttribute>();

            // Assert
            result.Should().BeEmpty();
        }

        private class ClassWithMultipleRepeatableAttributes
        {
            [Repeatable("First")]
            [Repeatable("Second")]
            public void Method()
            {
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private sealed class RepeatableAttribute(string tag) : Attribute
        {
            public string Tag { get; } = tag;
        }

        [AttributeUsage(AttributeTargets.Method)]
        private sealed class MarkerAttribute : Attribute
        {
        }

        private class BaseClassWithMatchingAttribute
        {
            [Marker]
            public virtual void Method()
            {
            }
        }

        private class DerivedClassWithoutOwnMatchingAttribute : BaseClassWithMatchingAttribute
        {
            public override void Method()
            {
            }
        }
    }

    public class HasAttributeInHierarchy
    {
        [Fact]
        public void Can_find_an_inheritable_attribute_on_an_overridden_method()
        {
            // Arrange
            var member = typeof(DerivedClass).GetMethod("Method");

            // Act / Assert
            member.HasAttributeInHierarchy<InheritableAttribute>().Should().BeTrue();
        }

        [Fact]
        public void Can_determine_a_member_does_not_have_an_attribute_in_hierarchy()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act / Assert
            member.HasAttributeInHierarchy<CLSCompliantAttribute>().Should().BeFalse();
        }

        [Fact]
        public void Can_find_an_inheritable_attribute_on_an_overridden_method_using_a_specific_predicate()
        {
            // Arrange
            var member = typeof(DerivedClass).GetMethod("Method");

            // Act / Assert
            member.HasAttributeInHierarchy<InheritableAttribute>(_ => true).Should().BeTrue();
        }

        [Fact]
        public void Finds_an_attribute_on_a_member_using_a_specific_predicate()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act / Assert
            member.HasAttributeInHierarchy<ObsoleteAttribute>(attribute =>
                attribute.Message!.StartsWith("Specific")).Should().BeTrue();
        }

        [Fact]
        public void Ignores_the_attribute_in_hierarchy_if_the_predicate_does_not_match()
        {
            // Arrange
            var member = typeof(DerivedClass).GetMethod("Method");

            // Act / Assert
            member.HasAttributeInHierarchy<InheritableAttribute>(_ => false).Should().BeFalse();
        }

        [Fact]
        public void The_predicate_for_a_member_in_hierarchy_must_not_be_null()
        {
            // Arrange
            var member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            var act = () => member.HasAttributeInHierarchy<ObsoleteAttribute>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = true)]
        private sealed class InheritableAttribute : Attribute
        {
        }

        private class BaseClass
        {
            [Inheritable]
            public virtual void Method()
            {
            }
        }

        private class DerivedClass : BaseClass
        {
            public override void Method()
            {
            }
        }
    }

    public class IsObsolete
    {
        [Fact]
        public void An_obsolete_member_is_obsolete()
        {
            // Arrange
            MemberInfo member = typeof(ClassWithAttributedMember).GetMethod("Method");

            // Act
            bool result = member.IsObsolete();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_member_on_an_obsolete_type_is_obsolete_by_default()
        {
            // Arrange
            MemberInfo member = typeof(ObsoleteClass).GetMethod("Method");

            // Act
            bool result = member.IsObsolete();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void A_member_on_an_obsolete_type_can_ignore_the_declaring_type()
        {
            // Arrange
            MemberInfo member = typeof(ObsoleteClass).GetMethod("Method");

            // Act
            bool result = member.IsObsolete(ObsoleteMemberFilter.MemberOnly);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void A_member_on_a_normal_type_is_not_obsolete()
        {
            // Arrange
            MemberInfo member = typeof(NormalClass).GetMethod("Method");

            // Act
            bool result = member.IsObsolete();

            // Assert
            result.Should().BeFalse();
        }

        [Obsolete]
        private class ObsoleteClass
        {
            public void Method()
            {
            }
        }

        private class NormalClass
        {
            public void Method()
            {
            }
        }
    }

    private class ClassWithAttributedMember
    {
        [Obsolete("Specific reason")]
        public void Method()
        {
        }
    }

    public class IsRequired
    {
        [Fact]
        public void A_required_property_member_is_required()
        {
            // Arrange
            MemberInfo member = typeof(ClassWithRequiredMembers).GetProperty("RequiredProperty");

            // Act / Assert
            member.IsRequired().Should().BeTrue();
        }

        [Fact]
        public void A_required_field_member_is_required()
        {
            // Arrange
            MemberInfo member = typeof(ClassWithRequiredMembers).GetField("RequiredField");

            // Act / Assert
            member.IsRequired().Should().BeTrue();
        }

        [Fact]
        public void A_normal_property_member_is_not_required()
        {
            // Arrange
            MemberInfo member = typeof(ClassWithRequiredMembers).GetProperty("NormalProperty");

            // Act / Assert
            member.IsRequired().Should().BeFalse();
        }

        private class ClassWithRequiredMembers
        {
            public required string RequiredProperty { get; set; }

            public required string RequiredField;

            public string NormalProperty { get; set; }
        }
    }
}
