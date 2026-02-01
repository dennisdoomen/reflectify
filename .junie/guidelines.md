### Project Overview
Reflectify is a .NET library providing reflection extension methods without external dependencies. It uses [PolySharp](https://github.com/Sergio0694/PolySharp) to provide polyfills for modern C# features while targeting older frameworks.

### Build and Configuration
The project targets multiple frameworks: `net47`, `net6.0`, `netstandard2.0`, and `netstandard2.1`.

#### Prerequisites
- .NET SDK (supporting .NET 8.0 or newer for development).
- C# 12.0 or newer.

#### Build Instructions
You can build the solution using the standard `dotnet` CLI or the provided build scripts:
- **CLI**: `dotnet build`
- **PowerShell**: `.\build.ps1`
- **Bash**: `./build.sh`

#### Analyzers
Analyzers are enabled specifically for `net6.0` to optimize build times. These include:
- `StyleCop.Analyzers`
- `CSharpGuidelinesAnalyzer`
- `Roslynator.Analyzers`
- `Meziantou.Analyzer`

### Testing
Tests are located in the `tests/Reflectify.Specs` project and use [xUnit](https://xunit.net/) and [FluentAssertions](https://fluentassertions.com/).

#### Running Tests
- **All Tests**: `dotnet test`
- **Specific Framework**: `dotnet test -f net6.0`
- **Filtered Tests**: `dotnet test --filter Name~SomeTestName`

#### Adding New Tests
- Follow the **Arrange-Act-Assert** pattern.
- Use **FluentAssertions** for validations.
- Tests are typically organized into nested classes within a file, corresponding to the method being tested (e.g., `TypeMetaDataExtensionsSpecs.HasAttribute`).
- Internal members of the `Reflectify` assembly are visible to `Reflectify.Specs` via `InternalsVisibleTo`.

#### Demonstration Test
Below is a simple test demonstrating how to use Reflectify's `GetProperties` extension:

```csharp
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Reflectify.Specs;

public class SampleSpecs
{
    [Fact]
    public void Should_be_able_to_get_public_properties()
    {
        // Arrange
        var type = typeof(SampleClass);

        // Act
        // MemberKind is an enum provided by Reflectify to filter members
        var properties = type.GetProperties(MemberKind.Public);

        // Assert
        properties.Should().ContainSingle(p => p.Name == "PublicProperty");
        properties.Should().NotContain(p => p.Name == "InternalProperty");
    }

    private class SampleClass
    {
        public string PublicProperty { get; set; } = string.Empty;

        internal string InternalProperty { get; set; } = string.Empty;
    }
}
```

### Development Guidelines
- **Code Style**: Follow the [C# Coding Guidelines](https://csharpcodingguidelines.com/).
- **EditorConfig**: An `.editorconfig` file is present in the root to enforce formatting (e.g., `PascalCase` for constants, `outside_namespace` for usings).
- **PolySharp**: Be aware that many modern C# features are available on older targets via PolySharp.
- **Internals**: Use `internal` for logic that should not be part of the public API but needs to be shared between classes. These are tested via the `InternalsVisibleTo` attribute.

### Git Commit Instructions
- Title: Max 50 characters, imperative mood, starts with an appropriate gitmoji.
- Body: Blank line after title, wrapped at 72 characters.
- Content: Focus on user-facing value and architectural intent rather than implementation details.
- Markdown: Use lists for consequential changes (architecture/requirements) only.
- Automation: Pre-fill commit message in the chat response for easy copy-pasting.
