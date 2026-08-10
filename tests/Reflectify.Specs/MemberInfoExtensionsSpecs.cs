using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

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
