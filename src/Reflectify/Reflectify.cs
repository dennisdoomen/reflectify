﻿#if !REFLECTIFY_COMPILE
// <autogenerated />
#pragma warning disable
#endif

#nullable disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reflectify;

internal static class TypeMetaDataExtensions
{
    /// <summary>
    /// Returns <see langword="true" /> if the type is derived from an open-generic type, or <see langword="false" /> otherwise.
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
    /// Returns <see langword="true" /> if the type is decorated with the specific <typeparamref name="TAttribute"/>,
    /// or <see langword="false" /> otherwise.
    /// </summary>
    public static bool HasAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return type.GetCustomAttributes<TAttribute>(inherit: false).Any();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type is decorated with the specific <typeparamref name="TAttribute"/> <i>and</i>
    /// that attribute instance matches the predicate, or <see langword="false" /> otherwise.
    /// </summary>
    public static bool HasAttribute<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return type.GetCustomAttributes<TAttribute>(inherit: false).Any(predicate);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type or one its parents are decorated with the
    /// specific <typeparamref name="TAttribute"/>.
    /// </summary>
    public static bool HasAttributeInHierarchy<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return type.GetCustomAttributes<TAttribute>(inherit: true).Any();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type or one its parents are decorated with the
    /// specific <typeparamref name="TAttribute"/> <i>and</i> that attribute instance
    /// matches the predicate. Returns <see langword="false" /> otherwise.
    /// </summary>
    public static bool HasAttributeInHierarchy<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return type.GetCustomAttributes<TAttribute>(inherit: true).Any(predicate);
    }

    /// <summary>
    /// Retrieves all custom attributes of the specified type from a class or its inheritance hierarchy.
    /// </summary>
    public static TAttribute[] GetMatchingAttributes<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return (TAttribute[])type.GetCustomAttributes<TAttribute>(inherit: true);
    }

    /// <summary>
    /// Retrieves an array of attributes of a specified type that match the provided predicate.
    /// </summary>
    public static TAttribute[] GetMatchingAttributes<TAttribute>(this Type type, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return type.GetCustomAttributes<TAttribute>(inherit: true).Where(predicate).ToArray();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type overrides the Equals method, or <see langword="false" /> otherwise.
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
    /// <returns><see langword="true" /> if the actual type is the same as, or inherits from, the expected type; otherwise, <see langword="false" />.</returns>
    public static bool IsSameOrInherits(this Type actualType, Type expectedType)
    {
        return actualType == expectedType ||
               expectedType.IsAssignableFrom(actualType) ||
               (actualType.BaseType is { IsGenericType: true } && actualType.BaseType.GetGenericTypeDefinition() == expectedType);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type is a compiler-generated type, or <see langword="false" /> otherwise.
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
    /// Returns <see langword="true" /> if the type has a readable name, or <see langword="false" />
    /// if it is a compiler-generated name.
    /// </summary>
    public static bool HasFriendlyName(this Type type)
    {
        return !type.IsAnonymous() && !type.IsTuple();
    }

    /// <summary>
    /// Return <see langword="true" /> if the type is a tuple type; otherwise, <see langword="false" />
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
    /// Returns <see langword="true" /> if the type is an anonymous type, or <see langword="false" /> otherwise.
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
    /// Return <see langword="true" /> if the type is a struct or class record type; otherwise, <see langword="false" />.
    /// </summary>
    public static bool IsRecord(this Type type)
    {
        return type.IsRecordClass() || type.IsRecordStruct();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the type is a class record type; otherwise, <see langword="false" />.
    /// </summary>
    public static bool IsRecordClass(this Type type)
    {
        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is { } &&
               type.GetProperty("EqualityContract", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
                   .GetMethod?.HasAttribute<CompilerGeneratedAttribute>() == true;
    }

    /// <summary>
    /// Return <see langword="true" /> if the type is a record struct; otherwise, <see langword="false" />
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
    /// <returns><see langword="true" /> if the type is a KeyValuePair; otherwise, <see langword="false" />.</returns>
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
    /// Returns <see langword="null" /> if no such property exists.
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
    /// Gets the public and/or internal fields of a type hierarchy.
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
    /// Returns <see langword="null" /> if no such field exists.
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
    /// Finds a method by its name, parameter types and visibility. Returns <see langword="null" /> if no such method exists.
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
    /// Finds a parameterless method by its name and visibility. Returns <see langword="null" /> if no such method exists.
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
    /// Returns <see langword="null" /> if no such operator exists.
    /// </summary>
    public static MethodInfo FindExplicitConversionOperator(this Type type, Type sourceType, Type targetType)
    {
        return type
            .GetConversionOperators(sourceType, targetType, name => name is "op_Explicit")
            .SingleOrDefault();
    }

    /// <summary>
    /// Finds an implicit conversion operator from the <paramref name="sourceType"/> to the <paramref name="targetType"/>.
    /// Returns <see langword="null" /> if no such operator exists.
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

internal static class TypeExtensions
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
    public static bool HasAttribute<TAttribute>(this MemberInfo member)
        where TAttribute : Attribute
    {
        // Do not use MemberInfo.IsDefined
        // There is an issue with PropertyInfo and EventInfo preventing the inherit option to work.
        // https://github.com/dotnet/runtime/issues/30219
        return Attribute.IsDefined(member, typeof(TAttribute), inherit: false);
    }

    /// <summary>
    /// Determines whether the member has an attribute of the specified type that matches the predicate.
    /// </summary>
    public static bool HasAttribute<TAttribute>(this MemberInfo member, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return member.GetCustomAttributes<TAttribute>().Any(a => predicate(a));
    }

    public static bool HasAttributeInHierarchy<TAttribute>(this MemberInfo member)
        where TAttribute : Attribute
    {
        // Do not use MemberInfo.IsDefined
        // There is an issue with PropertyInfo and EventInfo preventing the inherit option to work.
        // https://github.com/dotnet/runtime/issues/30219
        return Attribute.IsDefined(member, typeof(TAttribute), inherit: true);
    }
}

internal static class PropertyInfoExtensions
{
    /// <summary>
    /// Returns <see langword="true" /> if the property is an indexer, or <see langword="false" /> otherwise.
    /// </summary>
    public static bool IsIndexer(this PropertyInfo member)
    {
        return member.GetIndexParameters().Length != 0;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the property is explicitly implemented on the
    /// <see cref="MemberInfo.DeclaringType"/>, or <see langword="false" /> otherwise.
    /// </summary>
    public static bool IsExplicitlyImplemented(this PropertyInfo prop)
    {
#if NETCOREAPP3_0_OR_GREATER
        return prop.Name.Contains('.', StringComparison.OrdinalIgnoreCase);
#else
        return prop.Name.Contains('.');
#endif
    }

    /// <summary>
    /// Returns <see langword="true" /> if the property has a public getter or setter, or <see langword="false" /> otherwise.
    /// </summary>
    public static bool IsPublic(this PropertyInfo prop)
    {
        return prop.GetMethod is { IsPublic: true } || prop.SetMethod is { IsPublic: true };
    }

    /// <summary>
    /// Returns <see langword="true" /> if the property is internal, or <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="prop">The property to check.</param>
    public static bool IsInternal(this PropertyInfo prop)
    {
        return prop.GetMethod is { IsAssembly: true } or { IsFamilyOrAssembly: true } ||
               prop.SetMethod is { IsAssembly: true } or { IsFamilyOrAssembly: true };
    }

    /// <summary>
    /// Returns <see langword="true" /> if the property is abstract, or <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="prop">The property to check.</param>
    public static bool IsAbstract(this PropertyInfo prop)
    {
        return prop.GetMethod is { IsAbstract: true } || prop.SetMethod is { IsAbstract: true };
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
        var collectedPropertyNames = new HashSet<string>();

        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
            flags |= kind.HasFlag(MemberKind.Static) ? BindingFlags.Static : BindingFlags.Instance;

            var allProperties = typeToReflect.GetProperties(flags);

            AddNormalProperties(kind, allProperties, collectedPropertyNames);

            AddExplicitlyImplementedProperties(kind, allProperties, collectedPropertyNames);

            AddInterfaceProperties(typeToReflect, kind, flags, collectedPropertyNames);

            // Move to the base type
            typeToReflect = typeToReflect.BaseType;
        }

        selectedProperties = selectedProperties.Where(x => !x.IsIndexer()).ToList();
    }

    private void AddNormalProperties(MemberKind kind, PropertyInfo[] allProperties,
        HashSet<string> collectedPropertyNames)
    {
        if (kind.HasFlag(MemberKind.Public) || kind.HasFlag(MemberKind.Internal) ||
            kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var property in allProperties)
            {
                if (HasVisibility(kind, property) && !property.IsExplicitlyImplemented() &&
                    collectedPropertyNames.Add(property.Name))
                {
                    selectedProperties.Add(property);
                }
            }
        }
    }

    private static bool HasVisibility(MemberKind kind, PropertyInfo prop)
    {
        return (kind.HasFlag(MemberKind.Public) && prop.IsPublic()) ||
               (kind.HasFlag(MemberKind.Internal) && prop.IsInternal());
    }

    private void AddExplicitlyImplementedProperties(MemberKind kind, PropertyInfo[] allProperties,
        HashSet<string> collectedPropertyNames)
    {
        if (kind.HasFlag(MemberKind.ExplicitlyImplemented))
        {
            foreach (var p in allProperties)
            {
                if (p.IsExplicitlyImplemented())
                {
                    var name = p.Name.Split('.').Last();

                    if (collectedPropertyNames.Add(name))
                    {
                        selectedProperties.Add(p);
                    }
                }
            }
        }
    }

#pragma warning disable AV1561
    private void AddInterfaceProperties(Type typeToReflect, MemberKind kind, BindingFlags flags,
        HashSet<string> collectedPropertyNames)
    {
        if (kind.HasFlag(MemberKind.DefaultInterfaceProperties) || typeToReflect.IsInterface)
        {
            var interfaces = typeToReflect.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                foreach (var prop in interfaceType.GetProperties(flags))
                {
                    if ((!prop.IsAbstract() || typeToReflect.IsInterface) &&
                        collectedPropertyNames.Add(prop.Name))
                    {
                        selectedProperties.Add(prop);
                    }
                }
            }
        }
    }
#pragma warning restore AV1561

    private void LoadFields(Type typeToReflect, MemberKind kind)
    {
        var collectedFieldNames = new HashSet<string>();

        while (typeToReflect != null && typeToReflect != typeof(object))
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
            flags |= kind.HasFlag(MemberKind.Static) ? BindingFlags.Static : BindingFlags.Instance;

            var files = typeToReflect.GetFields(flags);

            foreach (var field in files)
            {
                if (HasVisibility(kind, field) && collectedFieldNames.Add(field.Name))
                {
                    selectedFields.Add(field);
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
