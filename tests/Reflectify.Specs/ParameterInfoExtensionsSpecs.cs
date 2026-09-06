using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
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

    [Fact]
    public void Can_determine_a_parameter_has_an_attribute_in_hierarchy()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttributeInHierarchy<CustomParameterAttribute>().Should().BeTrue();
    }

    [Fact]
    public void Can_determine_a_parameter_does_not_have_an_attribute_in_hierarchy()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttributeInHierarchy<CLSCompliantAttribute>().Should().BeFalse();
    }

    [Fact]
    public void Can_determine_a_parameter_has_an_attribute_in_hierarchy_using_a_specific_predicate()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttributeInHierarchy<CustomParameterAttribute>(attribute =>
            attribute.Reason.StartsWith("Specific")).Should().BeTrue();
    }

    [Fact]
    public void Finds_an_overridden_parameter_attribute_in_hierarchy_using_a_specific_predicate()
    {
        // Arrange
        ParameterInfo parameter = typeof(DerivedClass).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttributeInHierarchy<CustomParameterAttribute>(attribute =>
            attribute.Reason.StartsWith("Inherited")).Should().BeTrue();
    }

    [Fact]
    public void Ignores_the_attribute_in_hierarchy_if_the_predicate_for_a_parameter_does_not_match()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act / Assert
        parameter.HasAttributeInHierarchy<CustomParameterAttribute>(attribute =>
            attribute.Reason.Contains("Other")).Should().BeFalse();
    }

    [Fact]
    public void The_predicate_for_a_parameter_in_hierarchy_must_not_be_null()
    {
        // Arrange
        ParameterInfo parameter = typeof(ClassWithAttributedParameter).GetMethod("Method")!.GetParameters()[0];

        // Act
        var act = () => parameter.HasAttributeInHierarchy<CustomParameterAttribute>(null);

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

    private class BaseClass
    {
        public virtual void Method([CustomParameter("Inherited reason")] string value)
        {
        }
    }

    private class DerivedClass : BaseClass
    {
        public override void Method(string value)
        {
        }
    }

    public class GetNullability
    {
        [Fact]
        public void A_non_nullable_reference_parameter_is_not_null()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("Method")!.GetParameters()[0];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_reference_parameter_is_nullable()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("Method")!.GetParameters()[1];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_value_type_parameter_is_not_null()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("Method")!.GetParameters()[2];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_value_type_parameter_is_nullable()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("OtherMethod")!.GetParameters()[0];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_non_nullable_generic_parameter_is_not_null()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("OtherMethod")!.GetParameters()[1];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_generic_parameter_is_nullable()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("OtherMethod")!.GetParameters()[2];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_parameter_compiled_without_a_nullable_context_is_unknown()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithoutNullableContext).GetMethod("Method")!.GetParameters()[0];

            // Act / Assert
            parameter.GetNullability().Should().Be(Nullability.Unknown);
        }
    }

    public class IsNullableReference
    {
        [Fact]
        public void A_nullable_reference_parameter_is_a_nullable_reference()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("Method")!.GetParameters()[1];

            // Act / Assert
            parameter.IsNullableReference().Should().BeTrue();
        }

        [Fact]
        public void A_non_nullable_reference_parameter_is_not_a_nullable_reference()
        {
            // Arrange
            ParameterInfo parameter = typeof(ClassWithNullableParameters).GetMethod("Method")!.GetParameters()[0];

            // Act / Assert
            parameter.IsNullableReference().Should().BeFalse();
        }
    }

#nullable enable
    private class ClassWithNullableParameters
    {
        [UsedImplicitly]
        public void Method(string nonNullableString, string? nullableString, int nonNullableInt)
        {
        }

        [UsedImplicitly]
        public void OtherMethod(int? nullableInt, List<string> nonNullableList, List<string>? nullableList)
        {
        }
    }
#nullable disable

    private class ClassWithoutNullableContext
    {
        [UsedImplicitly]
        public void Method(string someString)
        {
        }
    }
}
