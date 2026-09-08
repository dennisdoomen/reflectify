# Agent instructions for Reflectify

Guidance for any AI coding agent (Claude, GitHub Copilot, etc.) working in this repository.

## Source-embedding visibility convention

Reflectify is distributed both as a compiled NuGet package and as embedded source
(via source-only packaging) that gets compiled directly into a consumer's project.
The `REFLECTIFY_COMPILE` conditional compilation symbol (defined in
`src/Reflectify/Reflectify.csproj`) distinguishes the two:

- When `REFLECTIFY_COMPILE` is defined (building the Reflectify package itself),
  types are `public`.
- When it is *not* defined (the type is compiled as embedded source into a
  consumer), the same type must be `internal` and annotated with
  `[global::Microsoft.CodeAnalysis.Embedded]` (classes also get
  `[global::System.Diagnostics.DebuggerNonUserCode]`) so it does not leak into
  the consumer's public API.

**Every public type declared in `src/Reflectify` (classes, enums, interfaces,
etc.) must follow this pattern.** See `src/Reflectify/MemberKind.cs` or
`src/Reflectify/MemberInfoExtensions.cs` for the canonical shape:

```csharp
#if REFLECTIFY_COMPILE
public enum SomeEnum
#else
[global::Microsoft.CodeAnalysis.Embedded]
internal enum SomeEnum
#endif
{
    ...
}
```

When adding a new public type to this project, check that it uses this
conditional visibility pattern before considering the change complete.
