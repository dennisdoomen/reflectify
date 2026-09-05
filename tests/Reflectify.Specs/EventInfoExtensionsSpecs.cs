using System;
using System.Reflection;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Reflectify.Specs;

public class EventInfoExtensionsSpecs
{
    public class IsPublic
    {
        [Fact]
        public void An_event_with_public_accessors_is_public()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsPublic().Should().BeTrue();
        }

        [Fact]
        public void An_internal_event_is_not_public()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("InternalEvent", NonPublicInstance);

            // Assert
            @event.IsPublic().Should().BeFalse();
        }

        [Fact]
        public void An_explicitly_implemented_event_is_not_public()
        {
            // Act
            EventInfo @event = GetExplicitlyImplementedEvent();

            // Assert
            @event.IsPublic().Should().BeFalse();
        }
    }

    public class IsInternal
    {
        [Fact]
        public void An_internal_event_is_internal()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("InternalEvent", NonPublicInstance);

            // Assert
            @event.IsInternal().Should().BeTrue();
        }

        [Fact]
        public void A_protected_internal_event_is_internal()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("ProtectedInternalEvent", NonPublicInstance);

            // Assert
            @event.IsInternal().Should().BeTrue();
        }

        [Fact]
        public void A_public_event_is_not_internal()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsInternal().Should().BeFalse();
        }

        [Fact]
        public void A_private_event_is_not_internal()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PrivateEvent", NonPublicInstance);

            // Assert
            @event.IsInternal().Should().BeFalse();
        }
    }

    public class IsProtected
    {
        [Fact]
        public void A_protected_event_is_protected()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("ProtectedEvent", NonPublicInstance);

            // Assert
            @event.IsProtected().Should().BeTrue();
        }

        [Fact]
        public void A_protected_internal_event_is_not_protected()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("ProtectedInternalEvent", NonPublicInstance);

            // Assert
            @event.IsProtected().Should().BeFalse();
        }

        [Fact]
        public void A_public_event_is_not_protected()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsProtected().Should().BeFalse();
        }
    }

    public class IsPrivate
    {
        [Fact]
        public void A_private_event_is_private()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PrivateEvent", NonPublicInstance);

            // Assert
            @event.IsPrivate().Should().BeTrue();
        }

        [Fact]
        public void A_protected_event_is_not_private()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("ProtectedEvent", NonPublicInstance);

            // Assert
            @event.IsPrivate().Should().BeFalse();
        }

        [Fact]
        public void A_public_event_is_not_private()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsPrivate().Should().BeFalse();
        }
    }

    public class IsAbstract
    {
        [Fact]
        public void An_event_declared_on_an_interface_is_abstract()
        {
            // Act
            EventInfo @event = typeof(IInterfaceWithEvent).GetEvent("InterfaceEvent");

            // Assert
            @event.IsAbstract().Should().BeTrue();
        }

        [Fact]
        public void An_abstract_event_on_a_class_is_abstract()
        {
            // Act
            EventInfo @event = typeof(AbstractClassWithEvents).GetEvent("AbstractEvent");

            // Assert
            @event.IsAbstract().Should().BeTrue();
        }

        [Fact]
        public void A_virtual_event_is_not_abstract()
        {
            // Act
            EventInfo @event = typeof(AbstractClassWithEvents).GetEvent("VirtualEvent");

            // Assert
            @event.IsAbstract().Should().BeFalse();
        }

        [Fact]
        public void A_normal_event_is_not_abstract()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsAbstract().Should().BeFalse();
        }

#if NETCOREAPP3_0_OR_GREATER
        [Fact]
        public void A_default_interface_event_is_not_abstract()
        {
            // Act
            EventInfo @event = typeof(IInterfaceWithEvent).GetEvent("DefaultEvent");

            // Assert
            @event.IsAbstract().Should().BeFalse();
        }
#endif
    }

    public class IsExplicitlyImplemented
    {
        [Fact]
        public void An_event_implementing_an_interface_member_explicitly_is_explicitly_implemented()
        {
            // Act
            EventInfo @event = GetExplicitlyImplementedEvent();

            // Assert
            @event.IsExplicitlyImplemented().Should().BeTrue();
        }

        [Fact]
        public void A_normal_event_is_not_explicitly_implemented()
        {
            // Act
            EventInfo @event = typeof(ClassWithEvents).GetEvent("PublicEvent");

            // Assert
            @event.IsExplicitlyImplemented().Should().BeFalse();
        }

        [Fact]
        public void An_event_implementing_an_interface_member_implicitly_is_not_explicitly_implemented()
        {
            // Act
            EventInfo @event = typeof(ClassImplementingEventImplicitly).GetEvent("InterfaceEvent");

            // Assert
            @event.IsExplicitlyImplemented().Should().BeFalse();
        }
    }

    private const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    private static EventInfo GetExplicitlyImplementedEvent()
    {
        return typeof(ClassWithExplicitlyImplementedEvent).GetEvents(NonPublicInstance)[0];
    }

#pragma warning disable CS0067 // Event is never used - these events exist only for reflection metadata purposes.
    private class ClassWithEvents
    {
        [UsedImplicitly]
        public event EventHandler PublicEvent;

        [UsedImplicitly]
        internal event EventHandler InternalEvent;

        [UsedImplicitly]
        protected internal event EventHandler ProtectedInternalEvent;

        [UsedImplicitly]
        protected event EventHandler ProtectedEvent;

        [UsedImplicitly]
        private event EventHandler PrivateEvent;
    }

    private abstract class AbstractClassWithEvents
    {
        [UsedImplicitly]
        public abstract event EventHandler AbstractEvent;

        [UsedImplicitly]
        public virtual event EventHandler VirtualEvent;
    }

    private sealed class ClassImplementingEventImplicitly : IInterfaceWithEvent
    {
        [UsedImplicitly]
        public event EventHandler InterfaceEvent;
    }
#pragma warning restore CS0067

    private sealed class ClassWithExplicitlyImplementedEvent : IInterfaceWithEvent
    {
        event EventHandler IInterfaceWithEvent.InterfaceEvent
        {
            add { }
            remove { }
        }
    }

    private interface IInterfaceWithEvent
    {
        event EventHandler InterfaceEvent;

#if NETCOREAPP3_0_OR_GREATER
        event EventHandler DefaultEvent
        {
            add { }
            remove { }
        }
#endif
    }
}
