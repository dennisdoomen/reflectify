using System;
using FluentAssertions;
using Xunit;

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
}
