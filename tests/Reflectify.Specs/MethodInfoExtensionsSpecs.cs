using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class MethodInfoExtensionsSpecs
{
    public class IsExplicitlyImplemented
    {
        [Fact]
        public void A_method_that_is_only_reachable_through_the_interface_is_explicitly_implemented()
        {
            // Arrange
            MethodInfo method = typeof(ClassWithExplicitAndNormalMethod).GetMethod(
                $"{typeof(IInterfaceWithSingleMethod).FullName!.Replace("+", ".")}.InterfaceMethod",
                BindingFlags.Instance | BindingFlags.NonPublic);

            // Act
            bool isExplicitlyImplemented = method.IsExplicitlyImplemented();

            // Assert
            isExplicitlyImplemented.Should().BeTrue();
        }

        [Fact]
        public void A_method_that_implements_the_interface_implicitly_is_not_explicitly_implemented()
        {
            // Arrange
            MethodInfo method = typeof(ClassWithImplicitlyImplementedMethod).GetMethod("InterfaceMethod");

            // Act
            bool isExplicitlyImplemented = method.IsExplicitlyImplemented();

            // Assert
            isExplicitlyImplemented.Should().BeFalse();
        }

        [Fact]
        public void A_method_that_has_nothing_to_do_with_an_interface_is_not_explicitly_implemented()
        {
            // Arrange
            MethodInfo method = typeof(ClassWithExplicitAndNormalMethod).GetMethod("NormalMethod");

            // Act
            bool isExplicitlyImplemented = method.IsExplicitlyImplemented();

            // Assert
            isExplicitlyImplemented.Should().BeFalse();
        }

        [Fact]
        public void The_declaration_on_the_interface_itself_is_not_explicitly_implemented()
        {
            // Arrange
            MethodInfo method = typeof(IInterfaceWithSingleMethod).GetMethod("InterfaceMethod");

            // Act
            bool isExplicitlyImplemented = method.IsExplicitlyImplemented();

            // Assert
            isExplicitlyImplemented.Should().BeFalse();
        }

        private interface IInterfaceWithSingleMethod
        {
            [UsedImplicitly]
            string InterfaceMethod();
        }

        private sealed class ClassWithExplicitAndNormalMethod : IInterfaceWithSingleMethod
        {
            string IInterfaceWithSingleMethod.InterfaceMethod() => "explicit";

            [UsedImplicitly]
            public string NormalMethod() => "normal";
        }

        private sealed class ClassWithImplicitlyImplementedMethod : IInterfaceWithSingleMethod
        {
            public string InterfaceMethod() => "implicit";
        }
    }
}
