using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Reflectify.Specs;

public class ParameterInfoExtensionsSpecs
{
    [Fact]
    public void Can_determine_a_parameter_has_an_attribute()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttribute<CustomParameterAttribute>().Should().BeTrue();
    }

    [Fact]
    public void Can_determine_a_parameter_does_not_have_an_attribute()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttribute<CLSCompliantAttribute>().Should().BeFalse();
    }

    [Fact]
    public void Can_determine_a_parameter_has_an_attribute_using_a_specific_predicate()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttribute<CustomParameterAttribute>(attribute =>
            attribute.Reason.StartsWith("Specific")).Should().BeTrue();
    }

    [Fact]
    public void Can_determine_a_parameter_has_an_attribute_that_does_not_meet_a_predicate()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttribute<CustomParameterAttribute>(predicate =>
            predicate.Reason.Contains("Other")).Should().BeFalse();
    }

    [Fact]
    public void The_predicate_must_not_be_null()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act
        var act = () => parameter.HasAttribute<CustomParameterAttribute>(null);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class CustomParameterAttribute : Attribute
    {
        public CustomParameterAttribute(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }

    private class ClassWithAttributedParameter
    {
        public void Method([CustomParameter("Specific reason")] string value)
        {
        }
    }
}
