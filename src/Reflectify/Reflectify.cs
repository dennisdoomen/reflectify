﻿#if !REFLECTIFY_COMPILE
// <autogenerated />
#pragma warning disable
#endif

#nullable disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reflectify;

internal static class TypeMetaDataExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the type is derived from an open-generic type, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsDerivedFromOpenGeneric(this Type type, Type openGenericType)
    {
        // do not consider a type to be derived from itself
        if (type == openGenericType)
        {
            return false;
        }

        // check subject and its base types against definition
        for (Type baseType = type;
             baseType is not null;
             baseType = baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openGenericType)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the interfaces that the <paramref name="type"/> implements or inherits from that are concrete
    /// versions of the <paramref name="openGenericType"/>.
    /// </summary>
    public static Type[] GetClosedGenericInterfaces(this Type type, Type openGenericType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
        {
            return [type];
        }

        Type[] interfaces = type.GetInterfaces();

        return interfaces
            .Where(t => t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericType))
            .ToArray();
    }

    /// <summary>
    /// Returns <c>true</c> if the type is decorated with the specific <typeparamref name="TAttribute"/>,
    /// or <c>false</c> otherwise.
    /// </summary>
    public static bool HasAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return type.HasAttribute<TAttribute>(_ => true);
    }

    /// <summary>
    /// Returns <c>true</c> if the type is decorated with the specific <typeparamref name="TAttribute"/> <i>and</i>
    /// that attribute instance matches the predicate, or <c>false</c> otherwise.
    /// </summary>
    public static bool HasAttribute<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        return GetCustomAttributes(type, predicate).Any();
    }

    /// <summary>
    /// Returns <c>true</c> if the type or one its parents are decorated with the
    /// specific <typeparamref name="TAttribute"/>.
    /// </summary>
    public static bool HasAttributeInHierarchy<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return GetCustomAttributes<TAttribute>(type, _ => true, inherit: true).Any();
    }

    /// <summary>
    /// Returns <c>true</c> if the type or one its parents are decorated with the
    /// specific <typeparamref name="TAttribute"/> <i>and</i> that attribute instance
    /// matches the predicate. Returns <c>false</c> otherwise.
    /// </summary>
    public static bool HasAttributeInHierarchy<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        return GetCustomAttributes(type, predicate, inherit: true).Any();
    }

    /// <summary>
    /// Retrieves all custom attributes of the specified type from a class or its inheritance hierarchy.
    /// </summary>
    public static TAttribute[] GetMatchingAttributes<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return GetCustomAttributes<TAttribute>(type, _ => true, true);
    }

    /// <summary>
    /// Retrieves an array of attributes of a specified type that match the provided predicate.
    /// </summary>
    public static TAttribute[] GetMatchingAttributes<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        return GetCustomAttributes(type, predicate, true);
    }

    private static TAttribute[] GetCustomAttributes<TAttribute>(
        Type type, Func<TAttribute, bool> predicate, bool inherit = false)
        where TAttribute : Attribute
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return type.GetCustomAttributes(typeof(TAttribute), inherit)
            .OfType<TAttribute>()
            .Where(predicate).ToArray();
    }

    /// <summary>
    /// Returns <c>true</c> if the type overrides the Equals method, or <c>false</c> otherwise.
    /// </summary>
    public static bool OverridesEquals(this Type type)
    {
        MethodInfo method = type
            .GetMethod("Equals", [typeof(object)]);

        return method is not null
               && method.GetBaseDefinition().DeclaringType != method.DeclaringType;
    }

    /// <summary>
    /// Determines whether the actual type is the same as, or inherits from, the expected type.
    /// </summary>
    /// <remarks>
    /// The expected type can also be an open generic type definition.
    /// </remarks>
    /// <returns><c>true</c> if the actual type is the same as, or inherits from, the expected type; otherwise, <c>false</c>.</returns>
    public static bool IsSameOrInherits(this Type actualType, Type expectedType)
    {
        return actualType == expectedType ||
               expectedType.IsAssignableFrom(actualType) ||
               (actualType.BaseType?.IsGenericType is true && actualType.BaseType?.GetGenericTypeDefinition() == expectedType);
    }

    /// <summary>
    /// Returns <c>true</c> if the type is a compiler-generated type, or <c>false</c> otherwise.
    /// </summary>
    /// <remarks>
    /// Typical examples of compiler-generated types are anonymous types, tuples, and records.
    /// </remarks>
    public static bool IsCompilerGenerated(this Type type)
    {
        return type.IsRecord() ||
               type.IsAnonymous() ||
               type.IsTuple();
    }

    /// <summary>
    /// Returns <c>true</c> if the type has a readable name, or <c>false</c>
    /// if it is a compiler-generated name.
    /// </summary>
    public static bool HasFriendlyName(this Type type)
    {
        return !type.IsAnonymous() && !type.IsTuple();
    }

    /// <summary>
    /// Return <c>true</c> if the type is a tuple type; otherwise, <c>false</c>
    /// </summary>
    public static bool IsTuple(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

#if !(NET47 || NETSTANDARD2_0)
        return typeof(ITuple).IsAssignableFrom(type);
#else
        Type openType = type.GetGenericTypeDefinition();

        return openType == typeof(ValueTuple<>)
               || openType == typeof(ValueTuple<,>)
               || openType == typeof(ValueTuple<,,>)
               || openType == typeof(ValueTuple<,,,>)
               || openType == typeof(ValueTuple<,,,,>)
               || openType == typeof(ValueTuple<,,,,,>)
               || openType == typeof(ValueTuple<,,,,,,>)
               || (openType == typeof(ValueTuple<,,,,,,,>) && IsTuple(type.GetGenericArguments()[7]))
               || openType == typeof(Tuple<>)
               || openType == typeof(Tuple<,>)
               || openType == typeof(Tuple<,,>)
               || openType == typeof(Tuple<,,,>)
               || openType == typeof(Tuple<,,,,>)
               || openType == typeof(Tuple<,,,,,>)
               || openType == typeof(Tuple<,,,,,,>)
               || (openType == typeof(Tuple<,,,,,,,>) && IsTuple(type.GetGenericArguments()[7]));
#endif
    }

    /// <summary>
    /// Returns <c>true</c> if the type is an anonymous type, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsAnonymous(this Type type)
    {
        if (!type.FullName!.Contains("AnonymousType"))
        {
            return false;
        }

        return type.HasAttribute<CompilerGeneratedAttribute>();
    }

    /// <summary>
    /// Return <c>true</c> if the type is a struct or class record type; otherwise, <c>false</c>.
    /// </summary>
    public static bool IsRecord(this Type type)
    {
        return type.IsRecordClass() || type.IsRecordStruct();
    }

    /// <summary>
    /// Returns <c>true</c> if the type is a class record type; otherwise, <c>false</c>.
    /// </summary>
    public static bool IsRecordClass(this Type type)
    {
        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is { } &&
               type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
                   .GetMethod?.HasAttribute<CompilerGeneratedAttribute>() == true;
    }

    /// <summary>
    /// Return <c>true</c> if the type is a record struct; otherwise, <c>false</c>
    /// </summary>
    public static bool IsRecordStruct(this Type type)
    {
        // As noted here: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/record-structs#open-questions
        // recognizing record structs from metadata is an open point. The following check is based on common sense
        // and heuristic testing, apparently giving good results but not supported by official documentation.
        return type.BaseType == typeof(ValueType) &&
               type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
                   [typeof(StringBuilder)], null) is { } &&
               type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null,
                       [type, type], null)?
                   .HasAttribute<CompilerGeneratedAttribute>() == true;
    }

    /// <summary>
    /// Determines whether the specified type is a KeyValuePair.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is a KeyValuePair; otherwise, <c>false</c>.</returns>
    public static bool IsKeyValuePair(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
    }
}

internal static class TypeMemberExtensions
{
    private static readonly ConcurrentDictionary<(Type Type, MemberKind Kind), Reflector> ReflectorCache = new();

    /// <summary>
    /// Gets the public, internal, explicitly implemented and/or default properties of a type hierarchy.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of properties to include in the response.</param>
    public static PropertyInfo[] GetProperties(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Properties;
    }

    /// <summary>
    /// Finds the property by a case-sensitive name and with a certain visibility.
    /// </summary>
    /// <remarks>
    /// Normal property get priority over explicitly implemented properties and default interface properties.
    /// </remarks>
    /// <returns>
    /// Returns <c>null</c> if no such property exists.
    /// </returns>
    public static PropertyInfo FindProperty(this Type type, string propertyName, MemberKind memberVisibility)
    {
        if (propertyName is null or "")
        {
            throw new ArgumentException("The property name cannot be null or empty", nameof(propertyName));
        }

        var properties = type.GetProperties(memberVisibility);

        return Array.Find(properties, p =>
            p.Name == propertyName || p.Name.EndsWith("." + propertyName, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the public and/or internal fieldss of a type hierarchy.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of fields to include in the response.</param>
    public static FieldInfo[] GetFields(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Fields;
    }

    /// <summary>
    /// Finds the field by a case-sensitive name.
    /// </summary>
    /// <returns>
    /// Returns <c>null</c> if no such field exists.
    /// </returns>
    public static FieldInfo FindField(this Type type, string fieldName, MemberKind memberVisibility)
    {
        if (fieldName is null or "")
        {
            throw new ArgumentException("The field name cannot be null or empty", nameof(fieldName));
        }

        var fields = type.GetFields(memberVisibility);

        return Array.Find(fields, p => p.Name == fieldName);
    }

    /// <summary>
    /// Gets a combination of <see cref="GetProperties"/> and <see cref="GetFields"/>.
    /// </summary>
    /// <param name="type">The type to reflect.</param>
    /// <param name="kind">The kind of fields and properties to include in the response.</param>
    public static MemberInfo[] GetMembers(this Type type, MemberKind kind)
    {
        return GetFor(type, kind).Members;
    }

    private static Reflector GetFor(Type typeToReflect, MemberKind kind)
    {
        return ReflectorCache.GetOrAdd((typeToReflect, kind),
            static key => new Reflector(key.Type, key.Kind));
    }

    /// <summary>
    /// Finds a method by its name, parameter types and visiblity. Returns <c>null</c> if no such method exists.
    /// </summary>
    /// <remarks>
    /// If you don't specify any parameter types, the method will be found by its name only.
    /// </remarks>
#pragma warning disable AV1561
    public static MethodInfo FindMethod(this Type type, string methodName, MemberKind kind, params Type[] parameterTypes)
#pragma warning restore AV1561
    {
        if (methodName is null or "")
        {
            throw new ArgumentException("The method name cannot be null or empty", nameof(methodName));
        }

        var flags = kind.ToBindingFlags() | BindingFlags.Instance | BindingFlags.Static;

        return type
            .GetMethods(flags)
            .SingleOrDefault(m => m.Name == methodName && HasSameParameters(parameterTypes, m));
    }

    /// <summary>
    /// Finds a parameterless method by its name and visibility. Returns <c>null</c> if no such method exists.
    /// </summary>
    public static MethodInfo FindParameterlessMethod(this Type type, string methodName, MemberKind memberKind)
    {
        return type.FindMethod(methodName, memberKind);
    }

    private static bool HasSameParameters(Type[] parameterTypes, MethodInfo method)
    {
        if (parameterTypes.Length == 0)
        {
            // If we don't specify any specific parameters, it matches always.
            return true;
        }

        return method.GetParameters()
            .Select(p => p.ParameterType)
            .SequenceEqual(parameterTypes);
    }

    /// <summary>
    /// Determines whether the type has a method with the specified name and visibility.
    /// </summary>
#pragma warning disable AV1561
    public static bool HasMethod(this Type type, string methodName, MemberKind memberKind, params Type[] parameterTypes)
#pragma warning restore AV1561
    {
        return type.FindMethod(methodName, memberKind, parameterTypes) is not null;
    }

    public static PropertyInfo FindIndexer(this Type type, MemberKind memberKind, params Type[] parameterTypes)
    {
        var flags = memberKind.ToBindingFlags() | BindingFlags.Instance | BindingFlags.Static;

        return type.GetProperties(flags)
            .SingleOrDefault(p =>
                p.IsIndexer() && p.GetIndexParameters().Select(i => i.ParameterType).SequenceEqual(parameterTypes));
    }

#pragma warning disable AV1561

    /// <summary>
    /// Finds an explicit conversion operator from the <paramref name="sourceType"/> to the <paramref name="targetType"/>.
    /// Returns <c>null</c> if no such operator exists.
    /// </summary>
    public static MethodInfo FindExplicitConversionOperator(this Type type, Type sourceType, Type targetType)
    {
        return type
            .GetConversionOperators(sourceType, targetType, name => name is "op_Explicit")
            .SingleOrDefault();
    }

    /// <summary>
    /// Finds an implicit conversion operator from the <paramref name="sourceType"/> to the <paramref name="targetType"/>.
    /// Returns <c>null</c> if no such operator exists.
    /// </summary>
    public static MethodInfo FindImplicitConversionOperator(this Type type, Type sourceType, Type targetType)
    {
        return type
            .GetConversionOperators(sourceType, targetType, name => name is "op_Implicit")
            .SingleOrDefault();
    }

    private static IEnumerable<MethodInfo> GetConversionOperators(this Type type, Type sourceType, Type targetType,
#pragma warning restore AV1561
        Func<string, bool> predicate)
    {
        return type
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m =>
                m.IsPublic
                && m.IsStatic
                && m.IsSpecialName
                && m.ReturnType == targetType
                && predicate(m.Name)
                && m.GetParameters() is { Length: 1 } parameters
                && parameters[0].ParameterType == sourceType);
    }
}

public static class TypeExtensions
{
    /// <summary>
    /// If the type provided is a nullable type, gets the underlying type. Returns the type itself otherwise.
    /// </summary>
    public static Type NullableOrActualType(this Type type)
    {
        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = type.GetGenericArguments()[0];
        }

        return type;
    }
}

internal static class MemberInfoExtensions
{
    public static bool HasAttribute<TAttribute>(this MemberInfo type)
        where TAttribute : Attribute
    {
        // Do not use MemberInfo.IsDefined
        // There is an issue with PropertyInfo and EventInfo preventing the inherit option to work.
        // https://github.com/dotnet/runtime/issues/30219
        return Attribute.IsDefined(type, typeof(TAttribute), inherit: false);
    }

    public static bool HasAttribute<TAttribute>(this MemberInfo type,
        Expression<Func<TAttribute, bool>> isMatchingAttributePredicate)
        where TAttribute : Attribute
    {
        return false;
    }

    public static bool HasAttributeInHierarchy<TAttribute>(this MemberInfo type)
        where TAttribute : Attribute
    {
        // Do not use MemberInfo.IsDefined
        // There is an issue with PropertyInfo and EventInfo preventing the inherit option to work.
        // https://github.com/dotnet/runtime/issues/30219
        return Attribute.IsDefined(type, typeof(TAttribute), inherit: true);
    }
}

public static class PropertyInfoExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the property is an indexer, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsIndexer(this PropertyInfo member)
    {
        return member.GetIndexParameters().Length != 0;
    }

    /// <summary>
    /// Returns <c>true</c> if the property is explicitly implemented on the
    /// <see cref="MemberInfo.DeclaringType"/>, or <c>false</c> otherwise.
    /// </summary>
    public static bool IsExplicitlyImplemented(this PropertyInfo prop)
    {
#if NETCOREAPP3_0_OR_GREATER
        return prop.Name.Contains('.', StringComparison.OrdinalIgnoreCase);
#else
        return prop.Name.Contains('.');
#endif
    }
}

/// <summary>
/// Defines the kinds of members you want to get when querying for the fields and properties of a type.
/// </summary>
[Flags]
internal enum MemberKind
{
    None,
    Public = 1,
    Internal = 2,
    ExplicitlyImplemented = 4,
    DefaultInterfaceProperties = 8,
    Static = 16
}

internal static class MemberKindExtensions
{
    public static BindingFlags ToBindingFlags(this MemberKind kind)
    {
        BindingFlags flags = BindingFlags.Default;

        if (kind.HasFlag(MemberKind.Public))
        {
            flags |= BindingFlags.Public;
        }

        if (kind.HasFlag(MemberKind.Internal))
        {
            flags |= BindingFlags.NonPublic;
        }

        return flags;
    }
}

/// <summary>
/// Helper class to get all the public and internal fields and properties from a type.
/// </summary>
internal sealed class Reflector
{
    private readonly HashSet<string> collectedPropertyNames = new();
    private readonly HashSet<string> collectedFieldNames = new();
    private readonly List<FieldInfo> selectedFields = new();
    private List<PropertyInfo> selectedProperties = new();

    public Reflector(Type typeToReflect, MemberKind kind)
    {
        LoadProperties(typeToReflect, kind);
        LoadFields(typeToReflect, kind);

        Members = selectedProperties.Concat<MemberInfo>(selectedFields).ToArray();
    }

    private void LoadProperties(Type typeToReflect, MemberKind kind)
    {
        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
            flags |= kind.HasFlag(MemberKind.Static) ? BindingFlags.Static : BindingFlags.Instance;

            var allProperties = typeToReflect.GetProperties(flags);

            AddNormalProperties(kind, allProperties);

            AddExplicitlyImplementedProperties(kind, allProperties);

            AddInterfaceProperties(typeToReflect, kind, flags);

            // Move to the base type
            typeToReflect = typeToReflect.BaseType;
        }

        selectedProperties = selectedProperties.Where(x => !x.IsIndexer()).ToList();
    }

    private void AddNormalProperties(MemberKind kind, PropertyInfo[] allProperties)
    {
        if (kind.HasFlag(MemberKind.Public) || kind.HasFlag(MemberKind.Internal) ||
            kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var property in allProperties)
            {
                if (!collectedPropertyNames.Contains(property.Name) && !property.IsExplicitlyImplemented() &&
                    HasVisibility(kind, property))
                {
                    selectedProperties.Add(property);
                    collectedPropertyNames.Add(property.Name);
                }
            }
        }
    }

    private static bool HasVisibility(MemberKind kind, PropertyInfo prop)
    {
        return (kind.HasFlag(MemberKind.Public) && prop.GetMethod?.IsPublic is true) ||
               (kind.HasFlag(MemberKind.Internal) &&
                (prop.GetMethod?.IsAssembly is true || prop.GetMethod?.IsFamilyOrAssembly is true));
    }

    private void AddExplicitlyImplementedProperties(MemberKind kind, PropertyInfo[] allProperties)
    {
        if (kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var p in allProperties)
            {
                if (p.IsExplicitlyImplemented())
                {
                    var name = p.Name.Split('.').Last();

                    if (!collectedPropertyNames.Contains(name))
                    {
                        selectedProperties.Add(p);
                        collectedPropertyNames.Add(name);
                    }
                }
            }
        }
    }

    private void AddInterfaceProperties(Type typeToReflect, MemberKind kind, BindingFlags flags)
    {
        if (kind.HasFlag(MemberKind.DefaultInterfaceProperties) || typeToReflect.IsInterface)
        {
            var interfaces = typeToReflect.GetInterfaces();

            foreach (var iface in interfaces)
            {
                foreach (var prop in iface.GetProperties(flags))
                {
                    if (!collectedPropertyNames.Contains(prop.Name) &&
                        (!prop.GetMethod.IsAbstract || typeToReflect.IsInterface))
                    {
                        selectedProperties.Add(prop);
                        collectedPropertyNames.Add(prop.Name);
                    }
                }
            }
        }
    }

    private void LoadFields(Type typeToReflect, MemberKind kind)
    {
        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
            flags |= kind.HasFlag(MemberKind.Static) ? BindingFlags.Static : BindingFlags.Instance;

            var files = typeToReflect.GetFields(flags);

            foreach (var field in files)
            {
                if (!collectedFieldNames.Contains(field.Name) && HasVisibility(kind, field))
                {
                    selectedFields.Add(field);
                    collectedFieldNames.Add(field.Name);
                }
            }

            // Move to the base type
            typeToReflect = typeToReflect.BaseType;
        }
    }

    private static bool HasVisibility(MemberKind kind, FieldInfo field)
    {
        return (kind.HasFlag(MemberKind.Public) && field.IsPublic) ||
               (kind.HasFlag(MemberKind.Internal) && (field.IsAssembly || field.IsFamilyOrAssembly));
    }

    public MemberInfo[] Members { get; }

    public PropertyInfo[] Properties => selectedProperties.ToArray();

    public FieldInfo[] Fields => selectedFields.ToArray();
}
