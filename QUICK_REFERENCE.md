# Quick Reference Guide

Fast lookup for common tasks and concepts in CsharpToTypeScriptConverter.

## Table of Contents
- [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Type Mappings](#type-mappings)
- [Command Examples](#command-examples)
- [Common Patterns](#common-patterns)
- [Troubleshooting](#troubleshooting)
- [Links](#links)

---

## Installation

```bash
dotnet add package CSharpToTypeScriptConverter
```

Or via NuGet Package Manager:
```
Install-Package CSharpToTypeScriptConverter
```

---

## Basic Usage

### Single File Generation

```csharp
using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Tools;

// Get types
var types = typeof(MyType).Assembly.ExportedTypes;

// Get metadata
var metadata = MetadataHelper.GetGeneratorTypesMetadata(types, null);

// Generate
var generator = new Generator();
var output = generator.TypeScript().OneFile().Generate(metadata.ToArray());

// Save
File.WriteAllText("output.ts", output);
```

### Multiple Files Generation

```csharp
var metadata = MetadataHelper.GetGeneratorTypesMetadata(types, null);
var generator = new Generator();
var files = generator.TypeScript().SeparatedFiles().Generate(metadata.ToArray());

foreach (var file in files.BuildFiles)
{
    var path = Path.Combine("output", file.FilePath);
    Directory.CreateDirectory(Path.GetDirectoryName(path));
    File.WriteAllText(path, file.FileContent);
}
```

### Generate Commands

```csharp
var metadata = MetadataHelper.GetMetadataForCommands(
    types,
    interfaceFilterType: typeof(IRequestCommand),
    explicitCommandInterfaceToFilter: typeof(ICommand<>),
    requestCommandInterfaceName: "ICommand"
);
```

---

## Type Mappings

### Primitive Types

| C# Type | TypeScript |
|---------|-----------|
| `int`, `long`, `double`, `float`, `decimal` | `number` |
| `string`, `char` | `string` |
| `bool` | `boolean` |
| `Guid`, `DateTime` | `string` |
| `object` | `any` |
| `int?` (nullable) | `number` |

### Collections

| C# Type | TypeScript |
|---------|-----------|
| `List<T>` | `T[]` |
| `IEnumerable<T>` | `T[]` |
| `IList<T>` | `T[]` |
| `T[]` | `T[]` |

### Generics

| C# Type | TypeScript |
|---------|-----------|
| `Dictionary<string, int>` | `Dictionary<string, number>` |
| `MyClass<T>` | `MyClass<T>` |
| `MyClass<User>` | `MyClass<User>` |

---

## Command Examples

### Build Project
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
dotnet test --filter "TypeNameResolverTests"
dotnet test -v detailed
```

### Run Specific Test
```bash
dotnet test --filter "TypeNameResolverTests.Resolve_DefaultName_Works"
```

---

## Common Patterns

### Pattern 1: Extract Class Metadata

```csharp
var type = typeof(Order);
var metadata = ClassGeneratorType.Get(type, null);

Console.WriteLine($"Name: {metadata.Name}");              // "Order"
Console.WriteLine($"Base: {metadata.BaseTypeName}");     // "Entity"
Console.WriteLine($"Interfaces: {string.Join(", ", metadata.ImplementsInterfaceTypeNames)}");
```

### Pattern 2: Extract Enum Metadata

```csharp
var type = typeof(OrderStatus);
var metadata = EnumGeneratorType.Get(type);

foreach (var member in metadata.Members)
{
    Console.WriteLine($"{member.Name}");  // Pending, Confirmed, etc.
}
```

### Pattern 3: Resolve Type Names

```csharp
var types = new[]
{
    typeof(int),                              // → "number"
    typeof(List<User>),                       // → "User[]"
    typeof(Dictionary<string, List<Order>>),  // → "Dictionary<string, Order[]>"
};

foreach (var type in types)
{
    var tsName = TypeNameResolver.Resolve(type);
    Console.WriteLine(tsName);
}
```

### Pattern 4: Track Dependencies

```csharp
var resolver = new TypeDependencyResolver();
var dependencies = resolver.GetDependencies(typeof(Order));

foreach (var (depType, kind) in dependencies)
{
    Console.WriteLine($"{depType.Name} ({kind})");
}
```

### Pattern 5: Get All Dependencies

```csharp
var resolver = new TypeDependencyResolver();
var allDeps = resolver.GetAllDependencies(typeof(Order));

foreach (var kvp in allDeps)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value.FullName}");
}
```

### Pattern 6: Load and Use XML Documentation

```csharp
var type = typeof(Order);
DocumentationTools.LoadXmlDocumentation(type.Assembly);
var docs = type.GetDocumentation();
var cleanDocs = docs?.OnlyDocumentationText();

foreach (var line in cleanDocs ?? Array.Empty<string>())
{
    Console.WriteLine(line);
}
```

### Pattern 7: Custom Dependency Options

```csharp
var options = new TypeDependencyResolverOptions
{
    ResolveProperties = true,
    ResolveInterfaces = true,
    ResolveInherits = true,
    ResolveFields = false,
    ResolveMethods = false
};

var resolver = new TypeDependencyResolver(options: options);
```

### Pattern 8: Filter Ignored Types

```csharp
var ignoreTypes = new List<Type>
{
    typeof(EntityBase),
    typeof(AuditableBase)
};

var resolver = new TypeDependencyResolver(ignoreTypes: ignoreTypes);
```

---

## Troubleshooting

### Problem: Type name not resolving correctly
**Solution**: Check if it matches special cases (IEnumerable<T>, nullable, generics)
```csharp
// Debug helper
var type = typeof(MyType);
Console.WriteLine($"IsGenericType: {type.IsGenericType}");
Console.WriteLine($"IsGenericTypeDefinition: {type.IsGenericTypeDefinition}");
Console.WriteLine($"IsArray: {type.IsArray}");
```

### Problem: XML Documentation not found
**Solution**: Ensure `GenerateDocumentationFile` is enabled
```xml
<PropertyGroup>
    <GenerateDocumentationFile>True</GenerateDocumentationFile>
</PropertyGroup>
```

### Problem: Generated TypeScript has wrong interfaces
**Solution**: Check `ImplementsInterfaceTypeNames` in metadata
```csharp
var metadata = ClassGeneratorType.Get(typeof(MyClass), null);
Console.WriteLine(string.Join(", ", metadata.ImplementsInterfaceTypeNames));
```

### Problem: Dependencies missing in generated files
**Solution**: Ensure TypeDependencyResolver is configured correctly
```csharp
var resolver = new TypeDependencyResolver();
var allDeps = resolver.GetAllDependencies(typeof(Order));
Console.WriteLine($"Found {allDeps.Count} dependencies");
```

### Problem: Test fails with reflection error
**Solution**: Ensure types are public, not abstract, and properly accessible
```csharp
bool isValid = !type.IsAbstract && type.IsPublic;
Console.WriteLine($"Type valid: {isValid}");
```

---

## Type Name Resolver Quick Ref

### Method Signature
```csharp
public static string Resolve(
    Type type, 
    bool generateNullableTypesAsType = false,
    string replaceTypeNameWithThisName = null
)
```

### Common Calls
```csharp
TypeNameResolver.Resolve(typeof(int))                    // "number"
TypeNameResolver.Resolve(typeof(int?))                   // "number"
TypeNameResolver.Resolve(typeof(List<User>))             // "User[]"
TypeNameResolver.Resolve(typeof(MyClass<T>))             // "MyClass<T>"
TypeNameResolver.Resolve(typeof(MyClass<User>))          // "MyClass<User>"
TypeNameResolver.Resolve(
    type, 
    generateNullableTypesAsType: true
)                                                         // Nullable<T> → "Nullable<T>"
```

---

## MetadataHelper Quick Ref

### Extract All Types Metadata
```csharp
public static List<GeneratorType> GetGeneratorTypesMetadata(
    IEnumerable<Type> types,
    Type returnTypeFilter
)
```

### Extract Command Metadata
```csharp
public static List<GeneratorType> GetMetadataForCommands(
    IEnumerable<Type> types,
    Type interfaceFilterType,
    Type explicitCommandInterfaceToFilter,
    string requestCommandInterfaceName
)
```

---

## TypeDependencyResolver Quick Ref

### Get Direct Dependencies
```csharp
var dependencies = resolver.GetDependencies(type);
// Returns: List<(Type, TypeKind)>
```

### Get All Transitive Dependencies
```csharp
var allDeps = resolver.GetAllDependencies(typeof(Order));
// Returns: Dictionary<string, Type>
```

### Configure Options
```csharp
var options = new TypeDependencyResolverOptions
{
    ResolveProperties = true,      // From property types
    ResolveInterfaces = true,      // Implemented interfaces
    ResolveInherits = true,        // Base classes
    ResolveFields = false,         // Field types
    ResolveMethods = false         // Method signatures
};
```

---

## GeneratorType Properties

```csharp
public class GeneratorType
{
    public string Name { get; set; }                              // Type name
    public string TypeNameForJsonDeserialization { get; set; }    // Full name with assembly
    public string ReturnTypeName { get; set; }                    // Return type
    public GeneratorTypeKind Kind { get; set; }                   // Class/Enum/Interface/CommandClass
    public IEnumerable<GeneratorMember> Members { get; set; }     // Properties/fields
    public string[] ImplementsInterfaceTypeNames { get; set; }    // Interfaces
    public string BaseTypeName { get; set; }                      // Base class
    public string[] Documentation { get; set; }                   // XML docs
    public string GeneratedCode { get; set; }                     // Generated TypeScript
    public Type Type { get; set; }                                // Original C# type
}
```

---

## Links

**Documentation**:
- 📖 [README.md](README.md) - Main documentation
- 🏗️ [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture deep dive
- 👨‍💻 [CONTRIBUTING.md](CONTRIBUTING.md) - Development guide
- 🗂️ [DOCUMENTATION_INDEX.md](DOCUMENTATION_INDEX.md) - Documentation index

**Repository**:
- 🔗 [GitHub Repository](https://github.com/a-t-k/CsharpToTypeScriptConverter)
- 📦 [NuGet Package](https://www.nuget.org/packages/TypeScriptRequestCommandsGenerator)

**Related**:
- [.NET Reflection API](https://docs.microsoft.com/en-us/dotnet/fundamentals/reflection/reflection)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [C# Generics](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/generics/)

---

## Common Questions

**Q: How do I generate TypeScript from C# types?**
A: Use `new Generator().TypeScript().OneFile()` or `.SeparatedFiles()` with metadata from `MetadataHelper`

**Q: What types are supported?**
A: Classes, enums, interfaces, generics, arrays, primitives, nullable types

**Q: How are imports handled?**
A: `TypeDependencyResolver` tracks dependencies; templates generate imports for SeparatedFiles mode

**Q: Can I customize type name mapping?**
A: Yes, extend `TypeNameResolver` or preprocess types before passing to generator

**Q: How do I test my changes?**
A: Run `dotnet test` and add tests to appropriate test class

**Q: Where do I report issues?**
A: Open an Issue on GitHub with reproduction steps

---

*Last Updated: 2024*
*Version: 0.9.1*
*Target: .NET 9.0*

