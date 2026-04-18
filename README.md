# ATK.Command.CsToTsGenerator - Technical Documentation

Code generator for automatic generation of TypeScript types from C# types using .NET Reflection.

## Requirements

- **.NET 10.0**
- **Microsoft.CodeAnalysis 5.0.0**
- **System.CodeDom 10.0.1**

## Entry Point

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();
```

The class **`Generator`** is the central entry point:
- `Generator.TypeScript()` → Instantiates `TypeScriptGenerator`
- `TypeScriptGenerator` orchestrates the entire generation process

## Generation Modes

**SeparatedFiles** - Each type in separate file
- One file per type
- Barrel export via `index.ts`
- Automatic directory clearing with flag
- Template: `Templates/SeparatedFiles/`
- Classes: `SeparatedFilesGenerator`, `SeparatedFilesGeneratorWithMetaData`, `SeparatedFilesGeneratorWithRenderedTypes`

**OneFile** - All types in one file
- All types in a single file
- Compact output
- Classes: `OneFileGenerator`, `OneFileGeneratorWithMetaData`

## Core Components

| Component | Description |
|---|---|
| **Generator** | Entry point, creates `TypeScriptGenerator` |
| **TypeScriptGenerator** | Orchestrates the generation process |
| **SeparatedFilesGenerator** | Generates separate TS files per type |
| **OneFileGenerator** | Generates all types into one TS file |
| **TypeDependencyResolver** | Recursively resolves type dependencies |
| **TypeNameResolver** | Converts C# names to TypeScript names |
| **GeneratorType** | Represents a C# type with metadata |
| **GeneratorMember** | Represents a property/field of a type |

## Type Mapping

| C# | TypeScript |
|---|---|
| `int`, `long`, `short`, `byte` | `number` |
| `decimal`, `float`, `double` | `number` |
| `bool` | `boolean` |
| `string` | `string` |
| `DateTime`, `DateOnly` | `string` |
| `Enum` | `enum` |
| `List<T>`, `IEnumerable<T>` | `T[]` |
| `Dictionary<K, V>` | `{ [key: K]: V }` |
| `Nullable<T>`, `T?` | `T \| null` |
| `Guid` | `string` |

## Template Engine

T4 Text Templates in `Templates/`:
- `SeparatedFiles/ComplexTypes/` - Class/Record generation
- `SeparatedFiles/Commands/` - Command interface
- `SeparatedFiles/Enums/` - Enum generation
- `SeparatedFiles/TypeScriptImports/` - Import statements
- `SeparatedFiles/CodeGenerationWarning/` - Generation header

## API

### SeparatedFilesGenerator

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();

var builtFiles = tsGenerator
    .SeparatedFiles()
    .SetInterfaceFilter(typeof(IRequestCommand))
    .SetReturnTypeOfCommands(typeof(Response))
    .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
    .AddRangeOfCommandTypesToGenerate(commandTypes)
    .AddRangeOfExtraTypesToGenerate(dtoTypes)
    .GenerateMetadata()
    .GenerateTypeScript()
    .Build();

builtFiles.Save(clearDestinationFolder: true);
```

### OneFileGenerator

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();

var builtFile = tsGenerator
    .OneFile()
    .SetInterfaceFilter(typeof(IRequestCommand))
    .SetReturnTypeOfCommands(typeof(Response))
    .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
    .AddRangeOfCommandTypesToGenerate(commandTypes)
    .AddRangeOfExtraTypesToGenerate(dtoTypes)
    .GenerateMetadata()
    .GenerateTypeScript()
    .Build("output/api-types.ts");

builtFile.Save();
```

## Data Models

| Class | Purpose |
|---|---|
| `GeneratorType` | C# type with metadata |
| `GeneratorMember` | Property/field of a type |
| `GeneratorTypeKind` | Classification (Class, Enum, Interface, Record) |
| `FileMetadata` | Output folder and metadata |
| `BuildFile` | Generated TypeScript file |
| `BuildedSeparatedFiles` | Collection of generated files |

## Filter and Configuration

**SetInterfaceFilter**
- Only types implementing this interface
- Type selection for commands

**SetReturnTypeOfCommands**
- Response type for generated commands
- Default: `Response`, `ApiResponse`

**AddRangeOfCommandTypesToGenerate**
- Select command types
- Converted to TypeScript interfaces

**AddRangeOfExtraTypesToGenerate**
- Add DTOs and other types

## Type Classification

`GeneratorTypeKind` Enum:
- `Class` - Regular classes
- `Interface` - Interfaces
- `Record` - Record types
- `Enum` - Enumerations
- `Struct` - Structure types

## Testing

Test projects in `src/ATK.Command.CsToTsGenerator.Tests`:

- `GeneratorTypeTests` - Type recognition
- `ClassGeneratorTypeTests` - Class generation
- `OneFileGeneratorTests` - OneFile mode
- `SeparatedFilesGeneratorTests` - SeparatedFiles mode
- `InheritanceTests` - Inheritance handling
- `TypeDependencyResolverTests` - Dependency resolution
- `TypeNameResolverTests` - Name resolution

## Build & NuGet

**Build:**
```
dotnet build
dotnet test
```

**Package:**
- ATK.Command.CsToTsGenerator
- Repository: https://www.nuget.org/packages/ATK.Command.CsToTsGenerator
- License: LICENSE
