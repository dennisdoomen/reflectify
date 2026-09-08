# Agent instructions for Reflectify

## Public/internal visibility convention

Reflectify ships both as a compiled package and as embedded source. Every
public type in `src/Reflectify` must be conditionally `internal` when
`REFLECTIFY_COMPILE` is not defined, so it doesn't leak into a consumer's
public API when embedded as source:

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

See `MemberKind.cs` or `MemberInfoExtensions.cs` for the canonical shape.
Apply this to any new public type before considering a change complete.
