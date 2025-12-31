
# CsharpToTypeScriptConverter - Solution Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Project Structure](#project-structure)
4. [Core Components](#core-components)
5. [Key Models](#key-models)
6. [Type Name Resolution](#type-name-resolution)
7. [Type Generation Strategies](#type-generation-strategies)
8. [Type Dependencies](#type-dependencies)
9. [Generation Modes](#generation-modes)
10. [Usage Guide](#usage-guide)
11. [Development Guide](#development-guide)
12. [Testing](#testing)

---

## Project Overview

**CsharpToTypeScriptConverter** is a .NET 9.0 code generation library that automatically converts C# types (classes, interfaces, enums) to equivalent TypeScript code. This tool enables seamless synchronization of type definitions between a C# backend API and a TypeScript frontend client.

### Key Features
- Converts C# classes, interfaces, and enums to TypeScript with full type information
- Supports complex generic types including nested generics and arrays
- Handles class inheritance and interface implementation
- Generates either a single file or multiple organized separated files
- Preserves XML documentation from C# code as JSDoc comments
- Supports command patterns with `ICommand<T>` and `IRequestCommand` interfaces
- Generates type metadata for JSON deserialization
- Intelligent type name resolution for generics and type parameters
- Framework: .NET 9.0
- Package Version: 0.9.1

### Use Case
This library is designed for communication between an ASP.NET API and a TypeScript client using a command-based architecture. It ensures type safety and consistency across language boundaries while maintaining domain language clarity. The library automatically tracks type dependencies and generates proper imports for modular TypeScript code.

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
                ├── Commands/
                ├── ComplexTypes/
                ├── Enumerations/
                ├── Interfaces/
                └── Imports/

    TypeNameResolver (Type Analysis)
        └── Handles generics, arrays, nullable types
    
    MetadataHelper (Metadata Extraction)
        └── Orchestrates GeneratorTypes strategies
        
    GeneratorTypes (Type-Specific Generation)
        ├── ClassGeneratorType
        ├── EnumGeneratorType
        └── InterfaceGeneratorType
    
    TypeDependencyResolver (Dependency Analysis)
        └── Tracks type dependencies with configurable options
```

### Design Patterns Used
1. **Strategy Pattern**: Separate generators for each type kind (Class, Enum, Interface)
2. **Static Factory Pattern**: `ClassGeneratorType.Get()`, `EnumGeneratorType.Get()` for type metadata creation
3. **Builder Pattern**: Generator classes build output step-by-step
4. **Template Method Pattern**: T4 templates (`.tt` files) define code generation templates
5. **Facade Pattern**: `MetadataHelper` orchestrates type generation strategies
6. **Reflection**: Uses .NET Reflection to analyze C# types and extract metadata
7. **Visitor-like Pattern**: `TypeDependencyResolver` traverses type hierarchies

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
│   │   │   ├── TypeDependencyResolverOptions.cs
│   │   │   ├── TypeFileGenerator.cs
│   │   │   ├── TypeNameResolver.cs
│   │   │   └── GeneratorTypes\
│   │   │       ├── ClassGeneratorType.cs
│   │   │       ├── EnumGeneratorType.cs
│   │   │       └── InterfaceGeneratorType.cs
│   │   └── TypeScriptRequestCommandsGenerator.csproj
│   │
│   └── CsharpToTypeScriptConverter.Tests\
│       ├── ClassGeneratorTypeTests.cs
│       ├── InheritanceTests.cs
│       ├── TypeNameResolverTests.cs
│       ├── OneFileGeneratorTests.cs
│       ├── SeparatedFilesGeneratorTests.cs
│       ├── TestDefinitionsData.cs
│       └── CsharpToTypeScriptConverter.Tests.csproj
│
├── README.md
├── SOLUTION_DOCUMENTATION.md
├── LICENSE
└── (Other solution files)
```

### Directory Structure Details

**Generators/**
- Contains all code generation logic organized by output strategy (OneFile vs SeparatedFiles)

**Templates/**
- T4 text templates (`.tt` files) that define TypeScript code generation templates
- Auto-generated `.cs` files from templates using TextTemplatingFilePreprocessor
- Organized by generation strategy and type category

**Models/**
- Data transfer objects representing C# and TypeScript metadata
- `GeneratorType`: Core metadata for any C# type (class, enum, interface)
- `GeneratorTypeKind`: Enum distinguishing type categories
- `GeneratorMember`: Represents properties/fields with type information

**Tools/**
- Utility classes for reflection, documentation parsing, and type resolution
- `TypeNameResolver`: Converts C# types to TypeScript type names (handles generics, arrays, primitives)
- `DocumentationTools`: Extracts XML documentation from .NET assemblies
- `MetadataHelper`: Orchestrates type metadata extraction using GeneratorTypes strategies
- `TypeDependencyResolver`: Analyzes type dependencies with configurable resolution options
- `TypeFileGenerator`: Creates individual TypeScript files
- **GeneratorTypes/** folder: Strategy implementations for extracting metadata from different type kinds

**Tests/**
- Unit tests validating type generation, inheritance, generics, and resolver functionality

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

### 2. TypeScriptGenerator

**File**: `Generators/TypeScript/TypeScriptGenerator.cs`

**Purpose**: Facade class providing access to different generation strategies.

**Methods**:
- `OneFile()`: Returns `OneFileGenerator` for single-file output
- `SeparatedFiles()`: Returns `SeparatedFilesGenerator` for multi-file output

### 3. TypeNameResolver

**File**: `Tools/TypeNameResolver.cs`

**Purpose**: Resolves C# types to their TypeScript type name equivalents

**Key Features**:
- Converts primitive types: `int` → `number`, `string` → `string`, `bool` → `boolean`
- Handles nullables: `int?` → `number`
- Resolves generic types: `IEnumerable<User>` → `User[]`
- Supports nested generics: `Dictionary<string, List<User>>` → `Dictionary<string, User[]>`
- Generates generic type definitions: `MyClass<T>` → `MyClass<T>`
- Handles arrays: `User[]` → `User[]`

**Key Methods**:
- `Resolve(Type, bool, string?)`: Main method that converts C# type to TypeScript name
- `IsIEnumerableOfT(Type)`: Detects if type is `IEnumerable<T>`

**Type Mappings**:
```csharp
Number types (int, double, etc.) → "number"
String types (string, char, Guid, DateTime) → "string"
bool → "boolean"
object → "any"
Nullable<T> → T (unwrapped)
IEnumerable<T> → T[] (converted to array)
Generic types → Preserve generic structure
```

### 4. MetadataHelper

**File**: `Tools/MetadataHelper.cs`

**Purpose**: Orchestrates type metadata extraction for multiple types

**Key Methods**:
- `GetGeneratorTypesMetadata(types, returnTypeFilter)`: Extracts metadata for classes, enums, and interfaces
- `GetMetadataForCommands(types, interfaceFilter, commandInterface, replacementName)`: Extracts command-specific metadata with interface customization

**Workflow**:
1. Filters types (excludes abstract, filters by interfaces)
2. Delegates to appropriate `GeneratorType` strategy (Class, Enum, Interface)
3. Returns list of fully populated `GeneratorType` objects

### 5. GeneratorType Strategies

**Location**: `Tools/GeneratorTypes/`

#### ClassGeneratorType
**File**: `Tools/GeneratorTypes/ClassGeneratorType.cs`

**Purpose**: Extracts metadata from C# class types

**Key Methods**:
- `Get(Type, returnTypeFilter)`: Standard class metadata
- `GetCommand(Type, interfaceFilter?, replacementInterfaceName?)`: Command class metadata

**Metadata Extracted**:
- Name (resolved via `TypeNameResolver`)
- Base type name (inheritance support)
- Interface implementations
- Properties (only declared on this type, not inherited)
- JSON deserialization type
- XML documentation

#### EnumGeneratorType
**File**: `Tools/GeneratorTypes/EnumGeneratorType.cs`

**Purpose**: Extracts metadata from C# enum types

**Key Methods**:
- `Get(Type)`: Enum metadata extraction

**Metadata Extracted**:
- Enum name
- Enum members with their values
- XML documentation

#### InterfaceGeneratorType
**File**: `Tools/GeneratorTypes/InterfaceGeneratorType.cs`

**Purpose**: Extracts metadata from C# interface types

**Key Methods**:
- `Get(Type, returnTypeFilter)`: Interface metadata extraction

**Implementation**: Reuses `ClassGeneratorType` logic but sets `Kind = GeneratorTypeKind.Interface`

### 6. TypeDependencyResolver

**File**: `Tools/TypeDependencyResolver.cs`

**Purpose**: Analyzes and resolves type dependencies for import generation

**Key Features**:
- Recursive dependency resolution
- Configurable resolution options
- Filters built-in .NET types automatically
- Supports custom type ignoring

**Configuration** (via `TypeDependencyResolverOptions`):
- `ResolveProperties` (default: true): Resolve types from properties
- `ResolveInterfaces` (default: true): Resolve implemented interfaces
- `ResolveInherits` (default: true): Resolve base types
- `ResolveFields` (default: false): Resolve types from fields
- `ResolveMethods` (default: false): Resolve types from method signatures

**Key Methods**:
- `GetDependencies(Type, bool)`: Returns direct dependencies
- `GetAllDependencies(params Type[])`: Returns all transitive dependencies

**Type Classification**:
```csharp
public enum TypeKind
{
    Unknown,
    Class,
    Interface,
    Enum,
    ValueType
}
```

**Ignored Types**:
- Primitive types: `bool`, `int`, `long`, `string`, `char`
- Collection interfaces: `IList`, `IEnumerable`, `ICollection`, `IQueryable`
- Generic collection interfaces: `IList<>`, `IEnumerable<>`, etc.

### 7. Documentation Tools

**File**: `Tools/DocumentationTools.cs`

**Purpose**: Parses XML documentation from .NET assemblies and extracts developer-written documentation

**Key Methods**:
- `LoadXmlDocumentation(Assembly)`: Loads XML docs from assembly's `.xml` file
- `LoadXmlDocumentation(string)`: Parses raw XML documentation content
- `GetDocumentation(Type|PropertyInfo|ParameterInfo)`: Retrieves documentation for specific members
- `OnlyDocumentationText(string)`: Extracts clean text from XML doc markup

**Key Members**:
- `LoadedXmlDocumentation`: Dictionary caching parsed XML docs by member key
- `LoadedAssemblies`: HashSet tracking loaded assemblies (prevents reloading)

### 8. OneFileGenerator

**File**: `Generators/TypeScript/OneFile/OneFileGenerator.cs`

**Purpose**: Generates all TypeScript types into a single `.ts` file

**Key Methods**:
- Configures generator settings
- Manages type metadata collection
- Transforms C# types to TypeScript using templates

**Related Classes**:
- `OneFileGeneratorWithMetaData`: Extended version with metadata support

### 9. SeparatedFilesGenerator

**File**: `Generators/TypeScript/SeparatedFiles/SeparatedFilesGenerator.cs`

**Purpose**: Generates each TypeScript type into separate `.ts` files

**Key Features**:
- Organizes output by type category (commands, enums, complex types, interfaces)
- Manages import dependencies between files
- Generates warning comments in generated files
- Creates barrel exports (index.ts files)

**Related Classes**:
- `SeparatedFilesGeneratorWithMetaData`: Metadata version
- `SeparatedFilesGeneratorWithRenderedTypes`: Handles pre-rendered types
- `BuildedSeparatedFiles`: Container for built file structure

---

## Key Models

### GeneratorType

**File**: `Models/GeneratorType.cs`

Represents a C# type converted to TypeScript metadata.

```csharp
public class GeneratorType
{
    public string Name { get; set; }                              // TypeScript type name
    public string TypeNameForJsonDeserialization { get; set; }    // Type reference for JSON
    public string ReturnTypeName { get; set; }                    // For commands: return type
    public GeneratorTypeKind Kind { get; set; }                   // Class, Enum, Interface, CommandClass
    public IEnumerable<GeneratorMember> Members { get; set; }     // Properties/fields with types
    public string[] ImplementsInterfaceTypeNames { get; set; }    // Implemented interfaces
    public string BaseTypeName { get; set; }                      // Base class name (inheritance)
    public string[] Documentation { get; set; }                   // XML docs as text array
    public string GeneratedCode { get; set; }                     // Generated TypeScript code
    public Type Type { get; set; }                                // Original C# type
    
    // Computed property
    public string CommandReturnTypeName { get; }                  // Extracts return type from ICommand<T>
}
```

**New Properties** (Recent Additions):
- `BaseTypeName`: Supports class inheritance in TypeScript output
- `CommandReturnTypeName`: Regex-based extraction of generic type from `ICommand<T>`

### GeneratorMember

**File**: `Models/GeneratorMember.cs`

Represents a member (property/field) of a type.

**Properties**:
- `Name`: Member name
- `Type`: C# Type reference
- `GenericName`: TypeScript-resolved type name
- `IsDeclaredAsGeneric`: Whether member is a generic parameter
- `Documentation`: XML documentation

### GeneratorTypeKind

**File**: `Models/GeneratorTypeKind.cs`

Enum distinguishing different type categories:
```csharp
public enum GeneratorTypeKind
{
    Interface,
    Enum,
    Class,
    CommandClass  // Class implementing ICommand<T>
}
```

### BuildFile

**File**: `Models/BuildFile.cs`

Represents a generated TypeScript file ready for output.

**Properties**:
- `FileName`: Output filename
- `FileType`: Category of file (Command, Enum, ComplexType, Interface, etc.)
- `FileContent`: Generated TypeScript code
- `FilePath`: Full path where file should be written

### TypeScriptImportDependency

**File**: `Models/TypeScriptImportDependency.cs`

Represents an import statement needed between TypeScript files.

---

## Type Name Resolution

The `TypeNameResolver` is the heart of accurate type conversion. It handles complex scenarios:

### Example Resolutions

```csharp
// Primitives
typeof(int)                          → "number"
typeof(string)                       → "string"
typeof(bool)                         → "boolean"

// Generics
typeof(IEnumerable<User>)           → "User[]"
typeof(Dictionary<string, int>)     → "Dictionary<string, number>"
typeof(MyClass<T1, T2>)             → "MyClass<T1, T2>"

// Nested Generics
typeof(List<List<User>>)            → "List<User[]>[]"
typeof(Dictionary<string, List<User>>) → "Dictionary<string, User[]>"

// Arrays
typeof(int[])                       → "number[]"
typeof(User[])                      → "User[]"
typeof(List<User>[])                → "List<User>[]"

// Nullables
typeof(int?)                        → "number"
typeof(string?)                     → "string"

// Generic Type Definitions
typeof(MyClass<>)                   → "MyClass<T>"
typeof(MyClass<,>)                  → "MyClass<T1, T2>"
```

---

## Type Generation Strategies

### Class Generation

**Input**: A C# class with properties and base type

```csharp
public class User : BasePerson
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

**Metadata Extraction**:
1. Resolve class name via `TypeNameResolver`
2. Extract base type using `TypeNameResolver`
3. Collect interface implementations
4. Extract declared properties only (not inherited)
5. Resolve each property type via `TypeNameResolver`
6. Load XML documentation via `DocumentationTools`

**Output**:
```typescript
export class User extends BasePerson {
    public id?: number;
    public name?: string;
}
```

### Enum Generation

**Input**: A C# enum

```csharp
public enum Status
{
    Active = 0,
    Inactive = 1
}
```

**Metadata Extraction**:
1. Resolve enum name
2. Extract enum members (exclude `value__` synthetic field)
3. Load XML documentation

**Output**:
```typescript
export enum Status {
    Active = 0,
    Inactive = 1,
}
```

### Interface Generation

**Input**: A C# interface

```csharp
public interface ICommand<T>
{
    // members
}
```

**Metadata Extraction**:
1. Uses `ClassGeneratorType` logic
2. Sets `Kind = GeneratorTypeKind.Interface`
3. Extracts interface members

---

## Type Dependencies

The `TypeDependencyResolver` enables intelligent import generation:

### Dependency Resolution Example

```csharp
public class Order
{
    public List<OrderItem> Items { get; set; }  // Depends on OrderItem
    public User Customer { get; set; }          // Depends on User
    public OrderStatus Status { get; set; }     // Depends on OrderStatus (Enum)
}
```

**Configuration**:
```csharp
var resolver = new TypeDependencyResolver(
    ignoreTypes: new List<Type> { typeof(SomeFrameworkType) },
    options: new TypeDependencyResolverOptions
    {
        ResolveProperties = true,
        ResolveInterfaces = true,
        ResolveInherits = true,
        ResolveMethods = false
    }
);
```

**Result**:
```csharp
var deps = resolver.GetDependencies(typeof(Order));
// Returns: [(OrderItem, Class), (User, Class), (OrderStatus, Enum)]
```

**Transitive Dependencies**:
```csharp
var allDeps = resolver.GetAllDependencies(typeof(Order));
// Recursively gets dependencies of Order, OrderItem, User, OrderStatus, etc.
```

---

## Generation Modes

### Mode 1: One File Generation

**Use Case**: When you want all TypeScript types in a single `.ts` file

**Entry Point**: `TypeScriptGenerator.OneFile()`

**Workflow**:
1. Collect all C# types to convert
2. Extract metadata for each type
3. Apply OneFileGenerator template to all types at once
4. Include all imports at the top of the file
5. Write single output file

**Output Structure**:
```typescript
// Generated with all types in one file
export interface ICommand<T> { _?: T }
export class Order { items?: OrderItem[] }
export class OrderItem { ... }
export enum OrderStatus { ... }
```

**Advantages**:
- Simple structure
- Minimal file management
- Good for smaller projects
- No import management needed

### Mode 2: Separated Files Generation

**Use Case**: When you want organized, modular TypeScript files

**Entry Point**: `TypeScriptGenerator.SeparatedFiles()`

**Workflow**:
1. Collect all C# types to convert
2. Extract metadata for each type
3. Categorize types by kind (commands, enums, complex types, interfaces)
4. Calculate import dependencies using `TypeDependencyResolver`
5. Generate individual files for each type category or type
6. Generate import statements for each file
7. Create barrel exports (index.ts) for each category
8. Write individual files to organized directory structure

**Output Structure**:
```
output/
├── commands/
│   ├── index.ts
│   ├── createOrder.ts
│   └── updateOrder.ts
├── enums/
│   ├── index.ts
│   └── orderStatus.ts
├── models/
│   ├── index.ts
│   ├── order.ts
│   └── orderItem.ts
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
- Automatic import management

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
public class CreateOrderCommand : IRequestCommand, ICommand<Order>
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
}

// Example model with inheritance
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
}

// Example enum
public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped
}
```

#### 3. Generate TypeScript (One File)

```csharp
using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Tools;

// Get all exported types from assembly
var exportedTypes = typeof(OrderStatus).Assembly.ExportedTypes;

// Define interfaces to search for
var requestCommandType = typeof(IRequestCommand);

// Get metadata for types
var typesMetadata = MetadataHelper.GetGeneratorTypesMetadata(
    exportedTypes,
    requestCommandType
);

// Generate TypeScript
var generator = new Generator();
var oneFileGen = generator.TypeScript().OneFile();
var transformedText = oneFileGen.Generate(typesMetadata.ToArray());

// Write to file
File.WriteAllText("./models.ts", transformedText);
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
/// <summary>Order information</summary>
public class Order
{
    /// <summary>Order identifier</summary>
    public int Id { get; set; }
    
    /// <summary>Order items list</summary>
    public List<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public enum OrderStatus { Pending = 0, Shipped = 1 }
```

**Output TypeScript** (from separated files):

`models/order.ts`:
```typescript
import { OrderItem } from './orderItem';

/**
 * Order information
 */
export class Order {
    /**
     * Order identifier
     */
    public id?: number;
    
    /**
     * Order items list
     */
    public items?: OrderItem[];
}
```

`models/orderItem.ts`:
```typescript
export class OrderItem {
    public productId?: number;
    public quantity?: number;
}
```

`enums/orderStatus.ts`:
```typescript
export enum OrderStatus {
    Pending = 0,
    Shipped = 1,
}
```

---

## Development Guide

### Adding Support for New Type Kinds

To support generating a new type category:

1. **Create strategy class** (e.g., `Tools/GeneratorTypes/RecordGeneratorType.cs`)
2. **Implement metadata extraction** using reflection
3. **Update `MetadataHelper`** to use new strategy
4. **Add `GeneratorTypeKind` enum value** if needed
5. **Create template** (e.g., `Templates/SeparatedFiles/Records/RecordTypeScriptGenerator.tt`)
6. **Add tests** validating metadata extraction and output

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

### Extending TypeNameResolver

To add custom type mappings:

1. Add custom type to the appropriate HashSet (`numberTypes`, `stringTypes`, etc.)
2. Or modify the resolution logic for special cases
3. Add test cases to `TypeNameResolverTests`

### Adding Custom Dependency Filtering

To ignore specific types during dependency resolution:

```csharp
var resolver = new TypeDependencyResolver(
    ignoreTypes: new List<Type> 
    { 
        typeof(SpecialFrameworkType),
        typeof(AnotherTypeToIgnore)
    }
);
```

### Testing Generated Output

Use unit tests to validate generation:

**Key Test Classes**:
- `TypeNameResolverTests.cs`: Tests type name resolution (generics, arrays, primitives)
- `ClassGeneratorTypeTests.cs`: Tests class metadata extraction
- `InheritanceTests.cs`: Tests inheritance and generic base class handling
- `OneFileGeneratorTests.cs`: Tests single-file generation
- `SeparatedFilesGeneratorTests.cs`: Tests multi-file generation

**Testing Pattern**:
1. Define test C# types (classes, enums, interfaces)
2. Call appropriate `GeneratorType` strategy
3. Assert metadata is correct
4. Optionally generate TypeScript and verify output

---

## Testing

### Running Tests

```bash
dotnet test
```

### Test Coverage

**Type Resolution Tests** (`TypeNameResolverTests.cs`):
- Simple type names
- Single and multiple generic parameters
- Nested generics
- Generic arrays
- Generic properties

**Type Generation Tests** (`ClassGeneratorTypeTests.cs`):
- Simple class generation
- Generic class generation
- Interface detection
- Command class detection

**Inheritance Tests** (`InheritanceTests.cs`):
- Base type name resolution
- Generic base class support
- Member inheritance

**Integration Tests**:
- `OneFileGeneratorTests.cs`: Full single-file generation
- `SeparatedFilesGeneratorTests.cs`: Full multi-file generation

---

## Building and Publishing

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

### 2. Strategy Pattern for Type Generation
- **Decision**: Separate generators for Class, Enum, Interface types
- **Rationale**: Each type kind has unique metadata extraction needs
- **Trade-off**: More classes to maintain, but clearer separation of concerns

### 3. TypeNameResolver Utility
- **Decision**: Centralized type-to-TypeScript-name resolution
- **Rationale**: Complex logic for generics, arrays, and type parameters
- **Trade-off**: Need to keep mappings in sync with TypeScript support

### 4. T4 Templates for Code Generation
- **Decision**: Use T4 text templates instead of string builders
- **Rationale**: Separate concerns (template design vs logic), cleaner code
- **Trade-off**: Need template preprocessing in build

### 5. Two Generation Strategies
- **Decision**: Support both one-file and separated-file modes
- **Rationale**: Different projects have different needs
- **Trade-off**: More code to maintain

### 6. Configurable Dependency Resolution
- **Decision**: Options class to control which dependencies to resolve
- **Rationale**: Flexibility for different use cases (properties only vs methods too)
- **Trade-off**: More complex configuration

### 7. Generic Interface Support
- **Decision**: Special handling for `ICommand<T>` and `IRequestCommand`
- **Rationale**: Command pattern is primary use case
- **Trade-off**: Less flexible than generic interface support

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
2. Check `TypeDependencyResolverOptions` matches your needs
3. Verify import path calculation in templates

### Issue: Generic Type Name Incorrect
**Cause**: `TypeNameResolver` not recognizing the pattern
**Solution**:
1. Check if type is `IEnumerable<T>` (converted to `T[]`)
2. Verify generic parameters are properly formed
3. Add test case to `TypeNameResolverTests`

### Issue: Base Type Not Generated in TypeScript
**Cause**: Inheritance or base type tracking not working
**Solution**:
1. Ensure C# class explicitly declares base type (not `object`)
2. Check `GeneratorType.BaseTypeName` is populated
3. Verify template includes `extends` keyword for non-null base types

---

## Performance Considerations

- **Reflection Impact**: Type analysis uses reflection; minimize assembly loads
- **Caching**: XML documentation is cached in `LoadedXmlDocumentation`
- **Dependency Resolution**: `GetAllDependencies()` is recursive; use options to limit scope
- **Memory**: Large projects may benefit from separated files generation
- **File I/O**: Batch file writes for performance

---

## Future Enhancements

Potential improvements for future versions:

1. Support for record types (C# 9+)
2. Support for init-only properties
3. Custom attribute-based generation control
4. TypeScript strict mode optimizations
5. Angular/React specific output modes
6. JSON schema integration
7. Performance optimizations for large codebases
8. Support for nullable reference types annotations
9. Generic constraint support
10. Method signature generation for interfaces
