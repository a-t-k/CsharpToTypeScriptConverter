# Contributing to CsharpToTypeScriptConverter

Thank you for your interest in contributing to CsharpToTypeScriptConverter! This document provides guidance for developers who want to extend or improve the project.

## Table of Contents

1. [Development Setup](#development-setup)
2. [Project Structure](#project-structure)
3. [Building and Testing](#building-and-testing)
4. [Common Development Tasks](#common-development-tasks)
5. [Testing Guidelines](#testing-guidelines)
6. [Code Style](#code-style)
7. [Pull Request Process](#pull-request-process)

---

## Development Setup

### Prerequisites

- **.NET SDK 9.0** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for version control

### Clone and Setup

```bash
git clone https://github.com/a-t-k/CsharpToTypeScriptConverter.git
cd CsharpToTypeScriptConverter

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

### Project Dependencies

**Generator Project** (`TypeScriptRequestCommandsGenerator.csproj`):
- `Microsoft.CodeAnalysis` (v4.14.0)
- `Microsoft.CodeAnalysis.CSharp` (v4.14.0)
- `System.CodeDom` (v9.0.0)

**Test Project** (`CsharpToTypeScriptConverter.Tests.csproj`):
- `xunit` (v2.9.2)
- `Microsoft.NET.Test.Sdk` (v17.12.0)

---

## Project Structure

### Directory Organization

```
src/
├── CsharpToTypeScriptConverter.Generator/
│   ├── Models/
│   │   ├── GeneratorType.cs
│   │   ├── GeneratorMember.cs
│   │   ├── GeneratorTypeKind.cs
│   │   ├── BuildFile.cs
│   │   └── ...
│   │
│   ├── Tools/
│   │   ├── DocumentationTools.cs
│   │   ├── MetadataHelper.cs
│   │   ├── TypeDependencyResolver.cs
│   │   ├── TypeDependencyResolverOptions.cs
│   │   ├── TypeNameResolver.cs
│   │   ├── TypeFileGenerator.cs
│   │   └── GeneratorTypes/
│   │       ├── ClassGeneratorType.cs
│   │       ├── EnumGeneratorType.cs
│   │       └── InterfaceGeneratorType.cs
│   │
│   ├── Generators/
│   │   └── TypeScript/
│   │       ├── TypeScriptGenerator.cs
│   │       ├── OneFile/
│   │       │   ├── OneFileGenerator.cs
│   │       │   └── OneFileGeneratorWithMetaData.cs
│   │       └── SeparatedFiles/
│   │           ├── SeparatedFilesGenerator.cs
│   │           ├── SeparatedFilesGeneratorWithMetaData.cs
│   │           ├── SeparatedFilesGeneratorWithRenderedTypes.cs
│   │           └── BuildedSeparatedFiles.cs
│   │
│   ├── Templates/
│   │   ├── SeparatedFiles/
│   │   │   ├── Commands/
│   │   │   │   ├── CommandTypeScriptGenerator.tt
│   │   │   │   └── CommandTypeScriptGeneratorExt.cs
│   │   │   ├── ComplexTypes/
│   │   │   │   ├── ComplexTypeScriptGenerator.tt
│   │   │   │   └── ComplexTypesScriptGeneratorExt.cs
│   │   │   ├── Enumerations/
│   │   │   │   ├── EnumTypeScriptGenerator.tt
│   │   │   │   └── EnumTypeScriptGeneratorExt.cs
│   │   │   ├── CommandInterface/
│   │   │   ├── CodeGenerationWarning/
│   │   │   └── TypeScriptImports/
│   │   └── OneFIle/
│   │       ├── TypesScriptGenerator.tt
│   │       └── TypesScriptGeneratorExt.cs
│   │
│   ├── Generator.cs
│   └── TypeScriptRequestCommandsGenerator.csproj
│
└── CsharpToTypeScriptConverter.Tests/
    ├── ClassGeneratorTypeTests.cs
    ├── InheritanceTests.cs
    ├── TypeNameResolverTests.cs
    ├── OneFileGeneratorTests.cs
    ├── SeparatedFilesGeneratorTests.cs
    ├── TestDefinitionsData.cs
    └── CsharpToTypeScriptConverter.Tests.csproj
```

### Key File Purposes

| File | Purpose |
|------|---------|
| `GeneratorType.cs` | Core metadata model for C# types |
| `TypeNameResolver.cs` | Converts C# types to TypeScript names |
| `MetadataHelper.cs` | Orchestrates metadata extraction |
| `ClassGeneratorType.cs` | Extracts class metadata |
| `EnumGeneratorType.cs` | Extracts enum metadata |
| `InterfaceGeneratorType.cs` | Extracts interface metadata |
| `TypeDependencyResolver.cs` | Analyzes type dependencies |
| `DocumentationTools.cs` | Extracts XML documentation |
| `*.tt` files | T4 templates for code generation |
| `*Ext.cs` files | Helper methods for templates |

---

## Building and Testing

### Build Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/CsharpToTypeScriptConverter.Generator

# Build in Release mode
dotnet build -c Release

# Clean before building
dotnet clean && dotnet build
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test -v detailed

# Run specific test class
dotnet test --filter "TypeNameResolverTests"

# Run single test method
dotnet test --filter "TypeNameResolverTests.Resolve_DefaultName_Works"

# Run tests with code coverage
dotnet test /p:CollectCoverage=true
```

### Test Organization

**Test Classes by Component**:

| Test Class | Tests Component | Focus |
|---|---|---|
| `TypeNameResolverTests` | `TypeNameResolver` | Type name resolution for all scenarios |
| `ClassGeneratorTypeTests` | `ClassGeneratorType` | Class metadata extraction |
| `InheritanceTests` | Class inheritance & generics | Handling base types and inheritance |
| `OneFileGeneratorTests` | `OneFileGenerator` | Single-file generation |
| `SeparatedFilesGeneratorTests` | `SeparatedFilesGenerator` | Multi-file generation |

---

## Common Development Tasks

### Adding a Test for a New Scenario

**Example: Test generic class with constraints**

```csharp
[Fact]
public void Resolve_Generic_WithConstraint_Works()
{
    var type = typeof(ConstrainedClass<>);
    string? name = TypeNameResolver.Resolve(type);
    True(name == "ConstrainedClass<T>");
}

private class ConstrainedClass<T> where T : class
{
    public T Value { get; set; }
}
```

**Steps**:
1. Add test method to appropriate test class
2. Define test type (usually as nested class in test)
3. Call component method
4. Assert results
5. Run test: `dotnet test`

### Adding Support for a New Type Mapping

**Example: Map `decimal` to `number` in TypeScript**

```csharp
// In TypeNameResolver.cs
private static readonly HashSet<Type> numberTypes =
[
    // ...existing types...
    typeof(decimal),  // Add this
];

// Add test in TypeNameResolverTests.cs
[Fact]
public void Resolve_Decimal_ReturnsNumber()
{
    string? name = TypeNameResolver.Resolve(typeof(decimal));
    True(name == "number");
}
```

### Adding a New Template Type

**Example: Support for readonly properties**

1. **Identify the need**: Determine if new template is needed
   - If it's just filtering existing types: update existing template
   - If it's a new category: create new template

2. **Create template file**:
   ```
   Templates/SeparatedFiles/Properties/ReadOnlyPropertyGenerator.tt
   ```

3. **Update .csproj**:
   ```xml
   <None Update="Templates/SeparatedFiles/Properties/ReadOnlyPropertyGenerator.tt">
     <LastGenOutput>ReadOnlyPropertyGenerator.cs</LastGenOutput>
     <Generator>TextTemplatingFilePreprocessor</Generator>
   </None>
   ```

4. **Create extension class**:
   ```csharp
   // Tools/GeneratorTypes/ReadOnlyPropertyGeneratorType.cs
   public static class ReadOnlyPropertyGeneratorType
   {
       public static GeneratorType Get(Type type)
       {
           // Implementation
       }
   }
   ```

5. **Update MetadataHelper** to use new strategy

6. **Add tests**

7. **Update generator** to apply template

### Extending TypeDependencyResolver

**Example: Add option to resolve static members**

1. **Update options class**:
   ```csharp
   public class TypeDependencyResolverOptions
   {
       // ...existing options...
       public bool ResolveStaticMembers { get; set; } = false;  // New
   }
   ```

2. **Implement resolution logic**:
   ```csharp
   if (this.options.ResolveStaticMembers)
   {
       var staticMembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public);
       // ... process static members ...
   }
   ```

3. **Add tests**:
   ```csharp
   [Fact]
   public void GetDependencies_WithStaticMembers_IncludesStaticTypes()
   {
       var options = new TypeDependencyResolverOptions 
       { 
           ResolveStaticMembers = true 
       };
       var resolver = new TypeDependencyResolver(options: options);
       
       var deps = resolver.GetDependencies(typeof(ClassWithStatic));
       
       True(deps.Any(d => d.type == typeof(StaticType)));
   }
   ```

---

## Testing Guidelines

### Test Naming Convention

```
// Pattern: Method_Scenario_Expected
[Fact]
public void Resolve_Generic_WithArray_ReturnsArraySyntax()
{
    // Test code
}

[Theory]
[InlineData(typeof(int), "number")]
[InlineData(typeof(string), "string")]
public void Resolve_PrimitiveTypes_ReturnsTypeScriptType(Type input, string expected)
{
    // Test code
}
```

### Test Structure

```csharp
[Fact]
public void DescriptiveTestName()
{
    // Arrange: Set up test data
    var input = new MyClass { Property = "value" };
    
    // Act: Execute the method being tested
    var result = ComponentUnderTest.MethodName(input);
    
    // Assert: Verify the result
    NotNull(result);
    True(result.Property == "expected");
}
```

### Using Xunit Assertions

```csharp
using static Xunit.Assert;

// Common assertions
True(condition);              // Assert.True
False(condition);             // Assert.False
NotNull(obj);                 // Assert.NotNull
Null(obj);                    // Assert.Null
Equal(expected, actual);      // Assert.Equal
NotEqual(unexpected, actual); // Assert.NotEqual
Contains(item, collection);   // Assert.Contains
Empty(collection);            // Assert.Empty
Single(collection);           // Assert.Single
Throws<Exception>(() => ...); // Assert.Throws
```

### Testing Metadata Extraction

```csharp
[Fact]
public void ClassGeneratorType_ExtractsAllMetadata()
{
    // Arrange
    var testClass = typeof(SampleClass);
    
    // Act
    var meta = ClassGeneratorType.Get(testClass, null);
    
    // Assert
    NotNull(meta);
    Equal("SampleClass", meta.Name);
    Equal(GeneratorTypeKind.Class, meta.Kind);
    NotEmpty(meta.Members);
    
    // Specific member assertions
    var idMember = meta.Members.FirstOrDefault(m => m.Name == "Id");
    NotNull(idMember);
    Equal("number", idMember.GenericName);
}
```

### Testing Type Dependencies

```csharp
[Fact]
public void TypeDependencyResolver_ResolvesTransitiveDependencies()
{
    // Arrange
    var resolver = new TypeDependencyResolver();
    var type = typeof(Order);
    
    // Act
    var allDeps = resolver.GetAllDependencies(type);
    
    // Assert
    NotNull(allDeps);
    True(allDeps.Count > 0);
    True(allDeps.ContainsKey(nameof(OrderItem)));
    True(allDeps.ContainsKey(nameof(User)));
}
```

---

## Code Style

### C# Conventions

**Naming**:
- Classes: `PascalCase` (e.g., `ClassGeneratorType`)
- Methods: `PascalCase` (e.g., `GetDocumentation`)
- Properties: `PascalCase` (e.g., `Name`)
- Private fields: `camelCase` (e.g., `_cache`)
- Local variables: `camelCase` (e.g., `resultList`)
- Constants: `PascalCase` (e.g., `DefaultBufferSize`)

**Formatting**:
```csharp
// Opening braces on same line
public class MyClass
{
    public void MyMethod()
    {
        // Code
    }
}

// Spacing
var result = value + 5;  // Space around operators
if (condition)           // Space after keywords
{
    // Code
}

// Null checks
if (obj != null)
{
    // Code
}

// Modern C# features (target is C# 13.0)
var result = value switch
{
    > 0 => "positive",
    0 => "zero",
    _ => "negative"
};
```

### Documentation

```csharp
/// <summary>
/// Brief description of what the method does.
/// </summary>
/// <param name="parameter1">Description of parameter1</param>
/// <param name="parameter2">Description of parameter2</param>
/// <returns>Description of return value</returns>
/// <remarks>Additional notes if needed</remarks>
public string MyMethod(Type parameter1, string parameter2)
{
    // Implementation
}
```

### Comments

```csharp
// Use comments to explain WHY, not WHAT
// Good: This approach avoids circular dependency resolution
// Bad: Loop through dependencies

// Multiline comments for complex logic
/*
 * Explanation of complex algorithm or logic that spans
 * multiple lines and needs detailed explanation
 */

// Avoid obvious comments
int count = 0;  // DO NOT: Set count to zero
// DO: Leave code self-documenting where obvious
```

---

## Pull Request Process

### Before Creating a PR

1. **Ensure tests pass**:
   ```bash
   dotnet test
   ```

2. **Check code builds**:
   ```bash
   dotnet build
   ```

3. **Add/update tests** for new functionality

4. **Update documentation** if API changes

5. **Follow code style** guidelines above

### PR Checklist

- [ ] Tests pass locally
- [ ] New tests added for new functionality
- [ ] Code follows style guidelines
- [ ] Documentation updated (if applicable)
- [ ] No debug code or console output
- [ ] Changes are focused (one feature per PR)
- [ ] Commit messages are clear and descriptive

### PR Description Template

```markdown
## Description
Brief description of the changes.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Enhancement
- [ ] Documentation update

## Related Issues
Closes #(issue number)

## Changes Made
- Change 1
- Change 2
- Change 3

## Testing
- [ ] Added unit tests
- [ ] Verified existing tests pass
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] No breaking changes
```

### Review Process

1. **Maintainer review**: Code review and feedback
2. **CI/CD checks**: Automated build and test verification
3. **Approval**: PR approved after addressing feedback
4. **Merge**: PR merged to main branch

---

## Troubleshooting Common Issues

### Build Errors

**Error**: `CS0246: The type or namespace name 'TypeScriptRequestCommandsGenerator' could not be found`

**Solution**: Run `dotnet restore` to restore dependencies

**Error**: T4 template compilation error

**Solution**: 
1. Check template syntax
2. Verify .csproj configuration for template
3. Rebuild templates: `dotnet clean && dotnet build`

### Test Failures

**Failure**: `Test failed: Method not found`

**Solution**: Rebuild to ensure latest code is compiled

**Failure**: `Assertion failed with null reference`

**Solution**: Check test data setup; verify mock objects are properly initialized

### TypeNameResolver Issues

**Problem**: Generic type name not resolving correctly

**Solution**:
1. Add test case to understand expected behavior
2. Debug with step-through debugging
3. Check if type matches special cases (IEnumerable<T>, nullable, etc.)

---

## Documentation

### Updating README.md

If API changes affect usage:
1. Update usage examples
2. Update method signatures
3. Update descriptions

### Updating ARCHITECTURE.md

If architecture changes:
1. Update component diagrams
2. Update flow descriptions
3. Update design pattern explanations

### Creating New Documentation

For significant features, consider:
- FAQ document for common questions
- Tutorial document for complex scenarios
- API reference for new public methods

---

## Performance Optimization

### Profiling

To identify performance bottlenecks:

```bash
# Run tests with timing
dotnet test --logger:"console;verbosity=detailed"
```

### Common Optimization Areas

1. **TypeNameResolver**: Cache compiled regex patterns
2. **MetadataHelper**: Cache type analysis results
3. **TypeDependencyResolver**: Implement memoization for recursive calls
4. **XML Documentation**: Lazy loading of documentation

### Performance Testing

Add benchmark tests using BenchmarkDotNet:

```csharp
[SimpleJob]
public class TypeNameResolverBenchmarks
{
    [Benchmark]
    public void Resolve_SimpleType()
    {
        TypeNameResolver.Resolve(typeof(int));
    }
    
    [Benchmark]
    public void Resolve_ComplexGeneric()
    {
        TypeNameResolver.Resolve(typeof(Dictionary<string, List<User>>));
    }
}
```

---

## Reporting Issues

When reporting bugs:

1. **Title**: Clear, specific description
   - Good: "TypeNameResolver fails on nullable generic types"
   - Bad: "Bug in type resolution"

2. **Description**: Include context
   - What were you trying to do?
   - What happened instead?
   - What did you expect to happen?

3. **Reproduction**: Minimal example
   ```csharp
   var type = typeof(Dictionary<string, int?>);
   var result = TypeNameResolver.Resolve(type);
   // Expected: "Dictionary<string, number>"
   // Actual: [error or incorrect output]
   ```

4. **Environment**:
   - .NET version
   - OS
   - Project version

---

## Questions?

For questions about contributing:
- Open an Issue on GitHub
- Check existing documentation
- Review similar PRs or code sections

Thank you for contributing!
