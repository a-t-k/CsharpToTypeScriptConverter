this file is generated with copilot

# CsharpToTypeScriptConverter - Solution Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Project Structure](#project-structure)
4. [Core Components](#core-components)
5. [Key Models](#key-models)
6. [Generation Modes](#generation-modes)
7. [Usage Guide](#usage-guide)
8. [Development Guide](#development-guide)

---

## Project Overview

**CsharpToTypeScriptConverter** is a .NET 9.0 code generation library that automatically converts C# types (classes, interfaces, enums) to equivalent TypeScript code. This tool enables seamless synchronization of type definitions between a C# backend API and a TypeScript frontend client.

### Key Features
- Converts C# classes, interfaces, and enums to TypeScript
- Supports generic types and nested generics
- Generates either a single file or multiple separated files
- Preserves XML documentation from C# code
- Supports command patterns with `ICommand<T>` and `IRequestCommand` interfaces
- Generates type metadata for JSON deserialization
- Framework: .NET 9.0
- Package Version: 0.9.1

### Use Case
This library is designed for communication between an ASP.NET API and a TypeScript client using a command-based architecture. It ensures type safety and consistency across language boundaries while maintaining domain language clarity.

---

## Architecture

### High-Level Architecture

```
CsharpToTypeScriptConverter
    └── TypeScriptGenerator (Entry Point)
        ├── OneFileGenerator (Single TypeScript file output)
        │   ├── OneFileGeneratorWithMetaData
        │   └── Templates/OneFile/TypesScriptGenerator
        │
        └── SeparatedFilesGenerator (Multiple TypeScript files)
            ├── SeparatedFilesGeneratorWithMetaData
            ├── SeparatedFilesGeneratorWithRenderedTypes
            ├── BuildedSeparatedFiles
            └── Templates/SeparatedFiles/*
```

### Design Patterns Used
1. **Builder Pattern**: Generator classes build the output step-by-step
2. **Template Method Pattern**: T4 templates (`.tt` files) define code generation templates
3. **Strategy Pattern**: Different generation strategies (OneFile vs SeparatedFiles)
4. **Reflection**: Uses .NET Reflection to analyze C# types and extract metadata
5. **Visitor Pattern**: Processes type hierarchies to collect and transform data

---

## Project Structure

### Solution Layout
```
D:\GIT\CsharpToTypeScriptConverter\
├── src\
│   ├── CsharpToTypeScriptConverter.Generator\
│   │   ├── Generator.cs                           (Main entry point)
│   │   ├── Generators\
│   │   │   └── TypeScript\
│   │   │       ├── TypeScriptGenerator.cs
│   │   │       ├── OneFile\
│   │   │       │   ├── OneFileGenerator.cs
│   │   │       │   └── OneFileGeneratorWithMetaData.cs
│   │   │       └── SeparatedFiles\
│   │   │           ├── SeparatedFilesGenerator.cs
│   │   │           ├── SeparatedFilesGeneratorWithMetaData.cs
│   │   │           ├── SeparatedFilesGeneratorWithRenderedTypes.cs
│   │   │           └── BuildedSeparatedFiles.cs
│   │   ├── Models\
│   │   │   ├── GeneratorType.cs
│   │   │   ├── GeneratorMember.cs
│   │   │   ├── GeneratorTypeKind.cs
│   │   │   ├── BuildFile.cs
│   │   │   ├── BuildFileType.cs
│   │   │   ├── FileMetadata.cs
│   │   │   ├── FileMetadataType.cs
│   │   │   └── TypeScriptImportDependency.cs
│   │   ├── Templates\
│   │   │   ├── SeparatedFiles\
│   │   │   │   ├── Commands\
│   │   │   │   ├── ComplexTypes\
│   │   │   │   ├── CommandInterface\
│   │   │   │   ├── Enumerations\
│   │   │   │   ├── CodeGenerationWarning\
│   │   │   │   └── TypeScriptImports\
│   │   │   └── OneFIle\
│   │   ├── Tools\
│   │   │   ├── DocumentationTools.cs
│   │   │   ├── MetadataHelper.cs
│   │   │   ├── TypeDependencyResolver.cs
│   │   │   └── TypeFileGenerator.cs
│   │   └── TypeScriptRequestCommandsGenerator.csproj
│   │
│   └── CsharpToTypeScriptConverter.Tests\
│       ├── OneFileGeneratorTests.cs
│       ├── SeparatedFilesGeneratorTests.cs
│       ├── TestDefinitionsData.cs
│       └── CsharpToTypeScriptConverter.Tests.csproj
│
├── README.md
├── LICENSE
└── (Other solution files)
```

### Directory Structure Details

**Generators/**
- Contains all code generation logic organized by output strategy

**Templates/**
- T4 text templates (`.tt` files) that define TypeScript code generation templates
- Auto-generated `.cs` files from templates using TextTemplatingFilePreprocessor
- Organized by generation strategy and type category

**Models/**
- Data transfer objects representing C# and TypeScript metadata
- Used internally by generators to transform type information

**Tools/**
- Utility classes for reflection, documentation parsing, and type resolution
- `DocumentationTools`: Extracts XML documentation from .NET assemblies
- `MetadataHelper`: Converts C# types to GeneratorType metadata
- `TypeDependencyResolver`: Resolves type dependencies for import generation
- `TypeFileGenerator`: Creates individual TypeScript files

**Tests/**
- Unit tests validating generator output for both strategies

---

## Core Components

### 1. Generator (Entry Point)

**File**: `Generator.cs`

```csharp
public class Generator
{
    public TypeScriptGenerator TypeScript()
    {
        return new TypeScriptGenerator();
    }
}
```

**Purpose**: Main entry point for the library. Provides fluent API access to TypeScript generation capabilities.

**Usage**:
```csharp
var generator = new Generator();
var tsGenerator = generator.TypeScript();
```

### 2. TypeScriptGenerator

**File**: `Generators/TypeScript/TypeScriptGenerator.cs`

**Purpose**: Facade class providing access to different generation strategies.

**Methods**:
- `OneFile()`: Returns `OneFileGenerator` for single-file output
- `SeparatedFiles()`: Returns `SeparatedFilesGenerator` for multi-file output

### 3. OneFileGenerator

**File**: `Generators/TypeScript/OneFile/OneFileGenerator.cs`

**Purpose**: Generates all TypeScript types into a single `.ts` file

**Key Methods**:
- Configures generator settings
- Manages type metadata collection
- Transforms C# types to TypeScript using templates

**Related Classes**:
- `OneFileGeneratorWithMetaData`: Extended version with metadata support

### 4. SeparatedFilesGenerator

**File**: `Generators/TypeScript/SeparatedFiles/SeparatedFilesGenerator.cs`

**Purpose**: Generates each TypeScript type into separate `.ts` files

**Key Features**:
- Organizes output by type category (commands, enums, complex types, interfaces)
- Manages import dependencies between files
- Generates warning comments in generated files

**Related Classes**:
- `SeparatedFilesGeneratorWithMetaData`: Metadata version
- `SeparatedFilesGeneratorWithRenderedTypes`: Handles pre-rendered types
- `BuildedSeparatedFiles`: Container for built file structure

### 5. Documentation Tools

**File**: `Tools/DocumentationTools.cs`

**Purpose**: Parses XML documentation from .NET assemblies and extracts developer-written documentation

**Key Members**:
- `LoadedXmlDocumentation`: Dictionary storing parsed XML docs by member key
- `LoadedAssemblies`: Set of assemblies with loaded documentation
- `LoadXmlDocumentation(Assembly)`: Loads XML docs from assembly
- `GetDocumentation(Type|PropertyInfo|ParameterInfo)`: Retrieves documentation for specific members
- `OnlyDocumentationText()`: Extracts clean text from XML doc markup

**Key Methods**:
- `GetDocumentation(this MemberInfo)`: Route to appropriate documentation getter based on member type
- `XmlDocumentationKeyHelper()`: Formats XML documentation keys for lookup
- `LoadXmlDocumentation(string)`: Parses raw XML documentation content

### 6. MetadataHelper

**File**: `Tools/MetadataHelper.cs`

**Purpose**: Converts C# reflection data to GeneratorType metadata used by templates

**Key Responsibilities**:
- Extract type information from C# types
- Identify command types and return types
- Resolve generic type parameters
- Build member metadata for properties and fields
- Track used types for dependency resolution

### 7. TypeDependencyResolver

**File**: `Tools/TypeDependencyResolver.cs`

**Purpose**: Analyzes type dependencies and generates import statements for TypeScript

**Key Responsibilities**:
- Track which types depend on other types
- Generate proper import paths between files
- Handle circular dependency detection
- Resolve type names to file locations

### 8. TypeFileGenerator

**File**: `Tools/TypeFileGenerator.cs`

**Purpose**: Creates individual TypeScript files with proper content

**Key Responsibilities**:
- Apply templates to generate file content
- Handle file naming conventions
- Manage file system operations
- Organize files into directory structure

---

## Key Models

### GeneratorType

**File**: `Models/GeneratorType.cs`

Represents a C# type converted to TypeScript metadata.

```csharp
public class GeneratorType
{
    public string Name { get; set; }                                    // TypeScript type name
    public string TypeNameForJsonDeserialization { get; set; }          // Type reference for JSON
    public string ReturnTypeName { get; set; }                          // For commands: return type
    public GeneratorTypeKind Kind { get; set; }                         // Class, Enum, Interface, etc.
    public IEnumerable<GeneratorMember> Members { get; set; }           // Properties/fields
    public string[] ImplementsInterfaceTypeNames { get; set; }          // Implemented interfaces
    public string[] Documentation { get; set; }                         // XML docs converted to text
    public string GeneratedCode { get; set; }                           // Generated TypeScript code
    public Type Type { get; set; }                                      // Original C# type
}
```

### GeneratorMember

**File**: `Models/GeneratorMember.cs`

Represents a member (property/field) of a type.

**Properties**:
- `Name`: Member name
- `TypeName`: TypeScript type name
- `Kind`: Member kind (property, field, etc.)
- `IsOptional`: Whether member is nullable/optional
- `Documentation`: XML documentation

### GeneratorTypeKind

**File**: `Models/GeneratorTypeKind.cs`

Enum distinguishing different type categories:
- `Class`: Regular C# class
- `Enum`: Enumeration
- `Interface`: Interface definition
- `CommandWithGenericReturnType`: Command with `ICommand<T>`

### BuildFile

**File**: `Models/BuildFile.cs`

Represents a generated TypeScript file ready for output.

**Properties**:
- `FileName`: Output filename
- `FileType`: Category of file (Command, Enum, ComplexType, Interface, etc.)
- `FileContent`: Generated TypeScript code
- `FilePath`: Full path where file should be written

### FileMetadata

**File**: `Models/FileMetadata.cs`

Metadata about a TypeScript file for tracking and organization.

### TypeScriptImportDependency

**File**: `Models/TypeScriptImportDependency.cs`

Represents an import statement needed between TypeScript files.

**Properties**:
- Source type
- Target type
- Import path
- Import statements to generate

---

## Generation Modes

### Mode 1: One File Generation

**Use Case**: When you want all TypeScript types in a single `.ts` file

**Entry Point**: `TypeScriptGenerator.OneFile()`

**Workflow**:
1. Collect all C# types to convert
2. Apply OneFileGenerator template to all types at once
3. Include all imports at the top of the file
4. Write single output file

**Output Structure**:
```typescript
// Single file with all interfaces, classes, and enums
export interface ICommand<T> { _?: T }
export class MyCommand implements ICommand<boolean> { ... }
export enum MyEnum { ... }
```

**Advantages**:
- Simple structure
- Minimal file management
- Good for smaller projects

### Mode 2: Separated Files Generation

**Use Case**: When you want organized, modular TypeScript files

**Entry Point**: `TypeScriptGenerator.SeparatedFiles()`

**Workflow**:
1. Collect all C# types to convert
2. Categorize types by kind (commands, enums, complex types, interfaces)
3. Generate individual files for each type category or type
4. Calculate import dependencies
5. Generate import statements for each file
6. Write individual files to organized directory structure

**Output Structure**:
```
output/
├── commands/
│   ├── index.ts (barrel export)
│   ├── myCommand.ts
│   └── anotherCommand.ts
├── enums/
│   ├── index.ts
│   └── myEnum.ts
├── types/
│   ├── index.ts
│   └── complexType.ts
├── interfaces/
│   ├── index.ts
│   └── iCommand.ts
└── generationWarning.ts
```

**Advantages**:
- Clear organization by type
- Easier to maintain and navigate
- Better for large projects
- Barrel exports for cleaner imports

---

## Usage Guide

### Basic Setup

#### 1. Install NuGet Package
```bash
Install-Package CSharpToTypeScriptConverter
```

#### 2. Define C# Types

```csharp
// Command interface (generic return type)
public interface ICommand<T>;

// Request command marker interface
public interface IRequestCommand;

// Example command
public class ChangeUserRoleRequestCommand : IRequestCommand, ICommand<bool>
{
    public string UserId { get; set; }
    public UserRoles NewRole { get; set; }
}

// Example enum
public enum UserRoles
{
    User,
    Admin
}
```

#### 3. Generate TypeScript (One File)

```csharp
using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Tools;

// Get all exported types from assembly
var exportedTypes = typeof(UserRoles).Assembly.ExportedTypes;

// Define interfaces to search for
var requestCommandType = typeof(IRequestCommand);
var commandReturnType = typeof(ICommand<>);

// Create settings
var usedTypes = new Dictionary<string, Type>();
TypesScriptGenerator.Settings.RequestCommandInterfaceName = "ICommand";

// Get metadata for types
var typesMetadata = MetadataHelper.GetGeneratorTypesMetadata(
    exportedTypes,
    requestCommandType,
    commandReturnType,
    usedTypes
);

// Handle types that weren't automatically included
var notGeneratedTypes = usedTypes
    .Where(ut => typesMetadata.All(tm => tm.Name != ut.Key))
    .ToDictionary();

if (notGeneratedTypes.Count > 0)
{
    typesMetadata.AddRange(
        MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes)
    );
}

// Generate TypeScript
var generator = new Generator();
var oneFileGen = generator.TypeScript().OneFile();
var transformedText = oneFileGen.Generate(typesMetadata.ToArray());

// Write to file
File.WriteAllText("./typeScriptTypes.ts", transformedText);
```

#### 4. Generate TypeScript (Separated Files)

```csharp
// ... (setup same as above until generation)

// Generate separated files
var generator = new Generator();
var separatedGen = generator.TypeScript().SeparatedFiles();
var builtFiles = separatedGen.Generate(typesMetadata.ToArray());

// Write files
foreach (var file in builtFiles.BuildFiles)
{
    var fullPath = Path.Combine("./output", file.FilePath);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
    File.WriteAllText(fullPath, file.FileContent);
}
```

### Generated TypeScript Output Example

**Input C# Code**:
```csharp
public class ChangeUserRoleRequestCommand : IRequestCommand, ICommand<bool>
{
    /// <summary>User identifier to change role for</summary>
    public string UserId { get; set; }
    
    /// <summary>New role to assign</summary>
    public UserRoles NewRole { get; set; }
}

public enum UserRoles { User = 0, Admin = 1 }

public interface ICommand<T> { _?: T }
```

**Output TypeScript**:
```typescript
/**
 * User identifier to change role for
 */
export class ChangeUserRoleRequestCommand implements ICommand<boolean> {
    private readonly $type? = ".ChangeUserRole, CsharpToTypeScriptConverter.Tests";
    public _?: boolean;
    public userId?: string;
    public newRole?: UserRoles;
}

export enum UserRoles {
    User = 0,
    Admin = 1,
}

export interface ICommand<T> {
    _?: T;
}
```

---

## Development Guide

### Adding a New Template

T4 templates control code generation. To add support for a new type of output:

1. **Create template file** (e.g., `Templates/SeparatedFiles/MyType/MyTypeGenerator.tt`)
2. **Update project file** (`.csproj`) to register the template:
   ```xml
   <None Update="Templates/SeparatedFiles/MyType/MyTypeGenerator.tt">
     <LastGenOutput>MyTypeGenerator.cs</LastGenOutput>
     <Generator>TextTemplatingFilePreprocessor</Generator>
   </None>
   ```
3. **Implement template logic** in the `.tt` file
4. **Create extension class** (e.g., `MyTypeGeneratorExt.cs`) for helper methods
5. **Integrate into generator** by calling template from generator class

### Extending MetadataHelper

To customize type metadata extraction:

1. Add new static method to `MetadataHelper` class
2. Implement reflection logic to extract desired metadata
3. Return `GeneratorType[]` array with custom data
4. Call from generator before template processing

### Adding Documentation Support

To add documentation to generated TypeScript:

1. Ensure C# types have XML documentation (`/// <summary>...`)
2. Use `DocumentationTools.LoadXmlDocumentation(assembly)` to load docs
3. Extract docs via `DocumentationTools.GetDocumentation(memberInfo)`
4. Pass documentation strings to template
5. Template includes docs in generated code

### Testing Generated Output

Use unit tests to validate generation:

**Key Test Classes**:
- `OneFileGeneratorTests.cs`: Tests single-file generation
- `SeparatedFilesGeneratorTests.cs`: Tests multi-file generation
- `TestDefinitionsData.cs`: Test data and expected outputs

**Testing Pattern**:
1. Define test C# types
2. Generate TypeScript
3. Assert generated code matches expected output
4. Verify imports and dependencies

### Building and Publishing

**Build**:
```bash
dotnet build
```

**Run Tests**:
```bash
dotnet test
```

**Create NuGet Package**:
- Project is configured with `<GeneratePackageOnBuild>True</GeneratePackageOnBuild>`
- Build creates `.nupkg` file automatically
- Package metadata in `.csproj`:
  - Version: 0.9.1
  - Description: Generates classes, enums, interfaces from C# to TypeScript
  - Repository: https://github.com/a-t-k/CsharpToTypeScriptConverter

---

## Key Architectural Decisions

### 1. Reflection-Based Type Analysis
- **Decision**: Use .NET Reflection instead of parsing source code
- **Rationale**: Works with compiled assemblies, handles complex scenarios
- **Trade-off**: Requires compiled types, not source-only

### 2. T4 Templates for Code Generation
- **Decision**: Use T4 text templates instead of string builders
- **Rationale**: Separate concerns (template design vs logic), cleaner code
- **Trade-off**: Need template preprocessing in build

### 3. Two Generation Strategies
- **Decision**: Support both one-file and separated-file modes
- **Rationale**: Different projects have different needs
- **Trade-off**: More code to maintain

### 4. Generic Interface Support
- **Decision**: Special handling for `ICommand<T>` and `IRequestCommand`
- **Rationale**: Command pattern is primary use case
- **Trade-off**: Less flexible than generic interface support

### 5. Lazy Type Resolution
- **Decision**: Track used types and resolve them on demand
- **Rationale**: Handle transitive dependencies efficiently
- **Trade-off**: Multiple passes required sometimes

---

## Common Issues and Solutions

### Issue: XML Documentation Not Found
**Cause**: XML documentation file (`.xml`) not found in assembly directory
**Solution**: 
1. Ensure C# project has `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
2. Copy `.xml` file to same directory as assembly
3. Load documentation before generating types

### Issue: Type Not Generated
**Cause**: Type doesn't implement required interface (IRequestCommand/ICommand<T>)
**Solution**: 
1. Check type implements required interfaces
2. Verify assembly is passed to generator
3. Check type visibility (must be public)

### Issue: Missing Imports in Generated TypeScript
**Cause**: Dependency not tracked by TypeDependencyResolver
**Solution**:
1. Ensure all used types are in generated metadata
2. Call `GetGeneratorTypesForUsedTypes()` for transitive dependencies
3. Verify import path calculation in TypeDependencyResolver

---

## Performance Considerations

- **Reflection Impact**: Type analysis uses reflection; minimize assembly loads
- **Caching**: XML documentation is cached in `LoadedXmlDocumentation`
- **Memory**: Large projects may benefit from separated files generation
- **File I/O**: Batch file writes for performance

---

## Future Enhancements

Potential improvements for future versions:

1. Support more generic interface patterns
2. Custom attribute-based generation control
3. TypeScript strict mode optimizations
4. Angular/React specific output modes
5. JSON schema integration
6. Performance optimizations for large codebases

---

## References

- **Repository**: https://github.com/a-t-k/CsharpToTypeScriptConverter
- **NuGet Package**: https://www.nuget.org/packages/TypeScriptRequestCommandsGenerator
- **.NET Framework**: .NET 9.0
- **Related Patterns**: CQRS, Event Sourcing, Domain-Driven Design
