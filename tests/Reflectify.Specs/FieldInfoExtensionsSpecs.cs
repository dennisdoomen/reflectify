using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace Reflectify.Specs;

public class FieldInfoExtensionsSpecs
{
    public class IsRequired
    {
        [Fact]
        public void A_required_field_is_required()
        {
            // Act
            FieldInfo field = typeof(ClassWithFields).GetField("RequiredField");

            // Assert
            field.IsRequired().Should().BeTrue();
        }

        [Fact]
        public void A_normal_field_is_not_required()
        {
            // Act
            FieldInfo field = typeof(ClassWithFields).GetField("NormalField");

            // Assert
            field.IsRequired().Should().BeFalse();
        }

        private class ClassWithFields
        {
            public required string RequiredField;

            [UsedImplicitly]
            public string NormalField;
        }
    }
}
