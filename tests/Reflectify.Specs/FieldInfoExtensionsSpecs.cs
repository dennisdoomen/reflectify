using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class FieldInfoExtensionsSpecs
{
    public class GetNullability
    {
        [Fact]
        public void A_non_nullable_reference_field_is_not_null()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NonNullableString");

            // Assert
            field.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_reference_field_is_nullable()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NullableString");

            // Assert
            field.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_value_type_field_is_not_null()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NonNullableInt");

            // Assert
            field.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_value_type_field_is_nullable()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NullableInt");

            // Assert
            field.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_non_nullable_generic_field_is_not_null()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NonNullableList");

            // Assert
            field.GetNullability().Should().Be(Nullability.NotNull);
        }

        [Fact]
        public void A_nullable_generic_field_is_nullable()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NullableList");

            // Assert
            field.GetNullability().Should().Be(Nullability.Nullable);
        }

        [Fact]
        public void A_field_compiled_without_a_nullable_context_is_unknown()
        {
            // Act
            FieldInfo field = typeof(ClassWithoutNullableContext).GetField("SomeString");

            // Assert
            field.GetNullability().Should().Be(Nullability.Unknown);
        }
    }

    public class IsNullableReference
    {
        [Fact]
        public void A_nullable_reference_field_is_a_nullable_reference()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NullableString");

            // Assert
            field.IsNullableReference().Should().BeTrue();
        }

        [Fact]
        public void A_non_nullable_reference_field_is_not_a_nullable_reference()
        {
            // Act
            FieldInfo field = typeof(ClassWithNullableFields).GetField("NonNullableString");

            // Assert
            field.IsNullableReference().Should().BeFalse();
        }
    }

#nullable enable
#pragma warning disable CS0649 // Field is never assigned to - these fields exist only for reflection metadata purposes.
    private class ClassWithNullableFields
    {
        [UsedImplicitly]
        public string NonNullableString = "";

        [UsedImplicitly]
        public string? NullableString;

        [UsedImplicitly]
        public int NonNullableInt;

        [UsedImplicitly]
        public int? NullableInt;

        [UsedImplicitly]
        public List<string> NonNullableList = new();

        [UsedImplicitly]
        public List<string>? NullableList;
    }
#pragma warning restore CS0649
#nullable disable

    private class ClassWithoutNullableContext
    {
        [UsedImplicitly]
#pragma warning disable CS0649 // Field is never assigned to - this field exists only for reflection metadata purposes.
        public string SomeString;
#pragma warning restore CS0649
    }
}
