<h1 align="center">
  <br>
  Reflectify
  <br>
</h1>

<h4 align="center">Reflection extensions without causing dependency pains</h4>

<div align="center">

[![](https://img.shields.io/github/actions/workflow/status/dennisdoomen/reflectify/build.yml?branch=main)](https://github.com/dennisdoomen/reflectify/actions?query=branch%3amain)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/dennisdoomen/reflectify?branch=main)](https://coveralls.io/github/dennisdoomen/reflectify?branch=main)
[![](https://img.shields.io/github/release/DennisDoomen/Reflectify.svg?label=latest%20release&color=007edf)](https://github.com/dennisdoomen/reflectify/releases/latest)
[![](https://img.shields.io/nuget/dt/Reflectify.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/Reflectify)
[![](https://img.shields.io/librariesio/dependents/nuget/Reflectify.svg?label=dependent%20libraries)](https://libraries.io/nuget/Reflectify)
![GitHub Repo stars](https://img.shields.io/github/stars/dennisdoomen/reflectify?style=flat)
[![GitHub contributors](https://img.shields.io/github/contributors/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/dennisdoomen/reflectify)](https://github.com/dennisdoomen/reflectify/issues)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://makeapullrequest.com)
![](https://img.shields.io/badge/release%20strategy-githubflow-orange.svg)
![Static Badge](https://img.shields.io/badge/4.7%2C_6.0%2C_netstandard2.0%2C_netstandard2.1-dummy?label=dotnet&color=%235027d5)



<a href="#about">About</a> •
<a href="#how-to-use-it">How To Use</a> •
<a href="#download">Download</a> •
<a href="#contributors">Contributors</a> •
<a href="#versioning">Versioning</a> •
<a href="#credits">Credits</a> •
<a href="#related">Related</a> •
<a href="#license">License</a> 

</div>

## About

### What's this?

Reflectify offers a bunch of extension methods to provide information such as the properties or fields a type exposes
and metadata about those members, and many other details about classes, records and interfaces. It supports all major
.NET versions and even understands explicitly implemented properties or properties coming from default interface
implementations, a C# 8 feature.

### What's so special about that?

Nothing really, but it offers that functionality through a content-only NuGet package that relies on C# 12. In other words, you can use this
package in your own packages, without the need to tie yourself to the Reflectify package. Oh, and it's used
by [an open-source project](https://fluentassertions.com/) with over 400 million downloads.

### Who created this?
My name is Dennis Doomen and I'm a Microsoft MVP and Principal Consultant at [Aviva Solutions](https://avivasolutions.nl/) with 28 years of experience under my belt. As a software architect and/or lead developer, I specialize in designing full-stack enterprise solutions based on .NET as well as providing coaching on all aspects of designing, building, deploying and maintaining software systems. I'm the author of [Fluent Assertions](https://www.fluentassertions.com), a popular .NET assertion library, [Liquid Projections](https://www.liquidprojections.net), a set of libraries for building Event Sourcing projections and I've been maintaining [coding guidelines for C#](https://www.csharpcodingguidelines.com) since 2001. 

Contact me through [Email](mailto:dennis.doomen@avivasolutions.nl), [BlueSky](https://bsky.app/profile/ddoomen.bsky.social), [Twitter/X](https://twitter.com/ddoomen) or [Mastadon](https://mastodon.social/@ddoomen)

## How do I use it?

Simple, to get the properties of a type, add the `Reflectify` NuGet package to a project that targets at least C# 12 and use

```csharp
using Reflectify;

var properties = typeof(SuperClass).GetProperties(
    MemberKind.Public | MemberKind.ExplicitlyImplemented | MemberKind.DefaultInterfaceProperties);
```

You can take any of the options `Public`, `Internal`, `Static`, `ExplictlyImplemented` and `DefaultInterfaceProperties`.

If you need the fields, use `GetFields` (which obviously cannot be explicitly implemented, nor be part of interfaces),
and if you need the members, use `GetMembers`. You can also request individual members by name, like
`GetProperty("Name", MemberKind.Public)` or `GetField("Name", MemberKind.Internal)`.

To get more metadata from a `PropertyInfo`, you can use extensions methods like:

* `IsExplictlyImplemented`
* `IsIndexer`
* `HasAttribute` and `HasAttributeInHierarchy`
* `IsPublic`, `IsInternal` or `IsAbstract` to check either the getter or setters matches the criteria

Similarly, you can find indexers using `FindIndexers`, conversion operators through `FindImplicitConversionOperators`
and `FindExplicitConversionOperators`, and methods via `FindMethod`, `FindParameterlessMethod` and `HasMethod`.

Other extension methods act on `Type` directly and include:

* `IsDerivedFromOpenGeneric` and `GetClosedGenericInterfaces`
* Various `HasAttribute`, `HasAttributeInHierarchy` and `GetMatchingAttributes` overloads, with and without additional
  filtering.
* `OverridesEquals` to determine if a type implements value semantics
* `IsSameOrInherits` to see if a type is the same as or derives from another (open-generic) type
* `IsCompilerGenerated` and `HasFriendlyName` to see if a type is (partially) generated by the compiler, like records,
  tuples, etc.
* `IsAnonymous`, `IsTuple`, `IsRecord`, `IsRecordClass`,`IsRecordStruct`, `IsKeyValuePair` to find these types.

Additionally, Reflectify offers some helpers such as

* `NullableOrActualType` to get the actual type of a nullable type or the type itself if it's not nullable.

## Download

This library is available as [a NuGet package](https://www.nuget.org/packages/Reflectify) on https://nuget.org. To install it, use the following command-line:

  `dotnet add package Reflectify`

## Contributors

Your contributions are always welcome! Please have a look at the [contribution guidelines](CONTRIBUTING.md) first. 

<a href="https://github.com/dennisdoomen/Reflectify/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=dennisdoomen/Reflectify" alt="contrib.rocks image" />
</a>

(Made with [contrib.rocks](https://contrib.rocks))

## Versioning
This library uses [Semantic Versioning](https://semver.org/) to give meaning to the version numbers. For the versions available, see the [tags](/releases) on this repository.

## Credits
This library wouldn't have been possible without the following tools, packages and companies:

* [Nuke](https://nuke.build/) - Smart automation for DevOps teams and CI/CD pipelines
* [Rider](https://www.jetbrains.com/rider/) - The world's most loved .NET and game dev IDE
* [xUnit](https://xunit.net/) - Community-focused unit testing tool for .NET.
* [Coverlet](https://github.com/coverlet-coverage/coverlet) - Cross platform code coverage for .NET
* [Polysharp](https://github.com/Sergio0694/PolySharp) - Generated, source-only polyfills for C# language features
* [GitVersion](https://gitversion.net/) - From git log to SemVer in no time
* [ReportGenerator](https://reportgenerator.io/) - Powerful code coverage visualization

## Support the project
* [Github Sponsors](https://github.com/sponsors/dennisdoomen)
* [Tip Me](https://paypal.me/fluentassertions)
* [Buy me a Coffee](https://ko-fi.com/dennisdoomen)
* [Sponsor Me](https://www.patreon.com/bePatron?u=9250052&redirect_uri=http%3A%2F%2Ffluentassertions.com%2F&utm_medium=widget)

## You may also like

* [My Blog](https://www.dennisdoomen.com)
* [FluentAssertions](https://github.com/fluentassertions/fluentassertions) - Extension methods to fluently assert the outcome of .NET tests
* [C# Coding Guidelines](https://csharpcodingguidelines.com/) - Forkable coding guidelines for all C# versions

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.