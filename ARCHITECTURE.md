# CsharpToTypeScriptConverter - Architecture Deep Dive

## Overview

This document provides an in-depth look at the architectural decisions, component interactions, and data flow in the CsharpToTypeScriptConverter project.

## Table of Contents

1. [Component Interaction Flow](#component-interaction-flow)
2. [Type Resolution Pipeline](#type-resolution-pipeline)
3. [Metadata Extraction Strategy](#metadata-extraction-strategy)
4. [Dependency Resolution System](#dependency-resolution-system)
5. [Code Generation Pipeline](#code-generation-pipeline)
6. [Design Patterns](#design-patterns)
7. [Extension Points](#extension-points)

---

## Component Interaction Flow

### High-Level Flow Diagram

```
User Code
    ↓
Generator (Entry Point)
    ↓
TypeScriptGenerator
    ├─→ OneFileGenerator
    │       ├→ MetadataHelper.GetGeneratorTypesMetadata()
    │       ├→ TypeNameResolver.Resolve() [for each type]
    │       └→ T4 OneFile Template
    │
    └─→ SeparatedFilesGenerator
            ├→ MetadataHelper.GetGeneratorTypesMetadata()
            ├→ TypeDependencyResolver.GetAllDependencies()
            ├→ TypeDependencyResolver.GetDependencies() [for imports]
            ├→ T4 SeparatedFiles Templates [by category]
            └→ Output: Multiple .ts files
```

### Complete Flow: From C# to TypeScript

```
1. User collects C# types (assembly.ExportedTypes)
   
2. Call MetadataHelper.GetGeneratorTypesMetadata(types)
   ├─ Filter abstract types
   ├─ For each type:
   │   ├─ If Enum → EnumGeneratorType.Get(type)
   │   ├─ If Interface → InterfaceGeneratorType.Get(type)
   │   └─ If Class → ClassGeneratorType.Get(type)
   └─ Return List<GeneratorType>

3. For each GeneratorType:
   ├─ Name = TypeNameResolver.Resolve(type)
   ├─ BaseTypeName = TypeNameResolver.Resolve(type.BaseType)
   ├─ Members.GenericName = TypeNameResolver.Resolve(memberType)
   ├─ Documentation = DocumentationTools.GetDocumentation(type)
   └─ ImplementsInterfaceTypeNames = [resolved interface names]

4. Optional: Resolve dependencies
   ├─ resolver = new TypeDependencyResolver()
   └─ allDeps = resolver.GetAllDependencies(types)

5. Generate TypeScript
   ├─ OneFile mode: Apply single template to all types
   └─ SeparatedFiles mode: 
       ├─ Group types by GeneratorTypeKind
       ├─ Apply type-specific template to each type
       ├─ Generate import statements
       └─ Create barrel exports

6. Write output
   ├─ OneFile: Single .ts file
   └─ SeparatedFiles: Directory structure with multiple .ts files
```

---

## Type Resolution Pipeline

### TypeNameResolver Architecture

The `TypeNameResolver` is the core of type name conversion. It handles the complexity of mapping C# types to TypeScript equivalents.

```
TypeNameResolver.Resolve(Type)
    ↓
Check nullability
    ├─ If Nullable<T> → Resolve(underlying T)
    └─ Else → Continue
    ↓
Check if generic
    ├─ Is Generic Type Definition (e.g., MyClass<T>)
    │   ├─ Extract base name: "MyClass" from "MyClass`1"
    │   ├─ Get generic arguments: [T]
    │   └─ Return "MyClass<T>"
    │
    ├─ Is IEnumerable<T> (generic)
    │   ├─ Extract T
    │   ├─ Resolve(T)
    │   └─ Return "T[]"
    │
    └─ Is constructed generic (e.g., MyClass<User>)
        ├─ Get base name: "MyClass"
        ├─ For each generic argument:
        │   └─ Resolve(argument)
        └─ Return "MyClass<ResolvedArg1, ResolvedArg2>"
    ↓
Check if array
    ├─ Get element type
    ├─ Resolve(elementType)
    └─ Return "ResolvedType[]"
    ↓
Check primitive mapping
    ├─ Number types → "number"
    ├─ String types → "string"
    ├─ bool → "boolean"
    ├─ object → "any"
    └─ Custom types → type.Name
```

### Type Categories

```csharp
// Primitive Types (mapped to TypeScript)
numberTypes = { sbyte, byte, short, ushort, int, uint, long, ulong, float, double, decimal }
stringTypes = { char, string, Guid, DateTime }
boolTypes = { bool }
objectTypes = { object }

// Generic Types (require special handling)
nullableTypes = { Nullable, Nullable<> }
collectionTypes = { List<T>, IEnumerable<T>, IList<T>, ... }

// Custom Types (preserve name, track dependencies)
userDefinedTypes = { YourClass, YourEnum, YourInterface, ... }
```

### Resolution Examples with Call Stack

```
Resolve(typeof(List<Order>))
├─ IsGenericType? Yes
├─ IsIEnumerableOfT()? Yes
├─ GetGenericArguments()[0] = typeof(Order)
├─ Resolve(typeof(Order))
│   ├─ IsGenericType? No
│   ├─ IsArray? No
│   ├─ numberTypes.Contains(Order)? No
│   ├─ stringTypes.Contains(Order)? No
│   ├─ custom type → "Order"
│   └─ Return "Order"
├─ Append "[]"
└─ Return "Order[]"

Resolve(typeof(Dictionary<string, List<User>>))
├─ IsGenericType? Yes
├─ IsIEnumerableOfT()? No
├─ GetNameFromGenericName("Dictionary`2") = "Dictionary"
├─ GetGenericArguments() = [typeof(string), typeof(List<User>)]
├─ For each argument:
│   ├─ Resolve(typeof(string)) → "string"
│   └─ Resolve(typeof(List<User>))
│       ├─ IsGenericType? Yes
│       ├─ IsIEnumerableOfT()? Yes
│       ├─ Resolve(typeof(User)) → "User"
│       ├─ Append "[]"
│       └─ Return "User[]"
├─ Aggregate args: "string, User[]"
└─ Return "Dictionary<string, User[]>"
```

---

## Metadata Extraction Strategy

### Strategy Pattern Implementation

The codebase uses the Strategy pattern to extract metadata differently for each type kind.

```csharp
// Strategy Interface (implicit)
public interface IGeneratorTypeStrategy
{
    GeneratorType Get(Type type, Type? returnTypeFilter);
}

// Implementations
public static class ClassGeneratorType : IGeneratorTypeStrategy { }
public static class EnumGeneratorType : IGeneratorTypeStrategy { }
public static class InterfaceGeneratorType : IGeneratorTypeStrategy { }
```

### MetadataHelper - Strategy Orchestrator

```
MetadataHelper.GetGeneratorTypesMetadata(types)
    ↓
Filter (excluding abstract types)
    ↓
For each type:
    ├─ If type.IsEnum
    │   └─ EnumGeneratorType.Get(type)
    │       ├─ Get enum name via TypeNameResolver
    │       ├─ Extract enum members (skip value__)
    │       ├─ Get documentation
    │       └─ Return GeneratorType { Kind = Enum, Members = [...] }
    │
    ├─ Else if type.IsInterface
    │   └─ InterfaceGeneratorType.Get(type, returnTypeFilter)
    │       ├─ Use ClassGeneratorType.Get()
    │       ├─ Override Kind = Interface
    │       └─ Return GeneratorType { Kind = Interface, ... }
    │
    └─ Else if type.IsClass
        └─ ClassGeneratorType.Get(type, returnTypeFilter)
            ├─ Get class name via TypeNameResolver
            ├─ Get base type via TypeNameResolver
            ├─ GetInterfaceNames()
            │   ├─ Filter by implementOnlyThisInterfaceWhenExists
            │   ├─ Resolve generic interfaces
            │   └─ Return interface names
            ├─ GetMember() - Extract properties
            │   ├─ Get only DeclaredOnly properties
            │   ├─ For each property:
            │   │   ├─ Resolve property type
            │   │   ├─ Check if generic parameter
            │   │   └─ Add GenericName
            │   └─ Return List<GeneratorMember>
            ├─ Get documentation
            ├─ Get JSON deserialization type
            └─ Return GeneratorType { Kind = Class, Members = [...] }
    ↓
Return List<GeneratorType>
```

### Class Metadata Extraction Details

```csharp
ClassGeneratorType.Get(typeof(Order))
    ├─ name = TypeNameResolver.Resolve(typeof(Order))
    │   └─ Result: "Order"
    │
    ├─ baseTypeName = TypeNameResolver.Resolve(typeof(Order).BaseType)
    │   ├─ BaseType = typeof(Entity)
    │   └─ Result: "Entity"
    │
    ├─ interfaces = GetInterfaceNames(typeof(Order))
    │   ├─ Implements IComparable
    │   ├─ Implements IEquatable<Order>
    │   ├─ Resolve generics: "IEquatable<Order>"
    │   └─ Result: ["IComparable", "IEquatable<Order>"]
    │
    ├─ members = GetMember(typeof(Order))
    │   ├─ Get properties(BindingFlags.DeclaredOnly)
    │   ├─ For property "Id" : int
    │   │   ├─ type = typeof(int)
    │   │   ├─ genericName = TypeNameResolver.Resolve(typeof(int))
    │   │   └─ GeneratorMember { Name = "Id", GenericName = "number" }
    │   │
    │   └─ For property "Items" : List<OrderItem>
    │       ├─ type = typeof(List<OrderItem>)
    │       ├─ genericName = TypeNameResolver.Resolve(...)
    │       └─ GeneratorMember { Name = "Items", GenericName = "OrderItem[]" }
    │
    └─ Return GeneratorType {
        Name = "Order",
        Kind = Class,
        BaseTypeName = "Entity",
        ImplementsInterfaceTypeNames = ["IComparable", "IEquatable<Order>"],
        Members = [Id, Items]
    }
```

### Enum Metadata Extraction Details

```csharp
EnumGeneratorType.Get(typeof(OrderStatus))
    ├─ name = typeof(OrderStatus).Name
    │   └─ Result: "OrderStatus"
    │
    ├─ members = Extract enum members
    │   ├─ GetFields() of enum type
    │   ├─ Exclude "value__" (synthesized field)
    │   └─ For each field:
    │       └─ GeneratorMember { Name = "Pending", Type = FieldType }
    │
    └─ Return GeneratorType {
        Name = "OrderStatus",
        Kind = Enum,
        Members = [
            { Name = "Pending", Type = int },
            { Name = "Confirmed", Type = int },
            { Name = "Shipped", Type = int }
        ]
    }
```

---

## Dependency Resolution System

### TypeDependencyResolver Architecture

```
TypeDependencyResolver(ignoreTypes?, options?)
    ├─ ignoreCustomerTypes: List<Type> (user-specified ignore list)
    ├─ ignoredCSharpValueType: List<Type> (primitives)
    ├─ ignoredCSharpComplexTypes: List<Type> (collections, interfaces)
    └─ options: TypeDependencyResolverOptions
        ├─ ResolveProperties: bool = true
        ├─ ResolveInterfaces: bool = true
        ├─ ResolveInherits: bool = true
        ├─ ResolveFields: bool = false
        └─ ResolveMethods: bool = false
```

### Dependency Resolution Flow

```
GetDependencies(Type order, includeSelf=true)
    ↓
Check if primitive type
    ├─ If yes → Filter and return empty
    └─ Else → Continue
    ↓
Check if generic type (not definition)
    ├─ Get generic arguments
    ├─ Recursively resolve each argument
    └─ Add to dependencies
    ↓
If includeSelf
    ├─ Add type itself to dependencies
    └─ Set correct TypeKind (Class, Enum, Interface, etc.)
    ↓
If IsClass or IsInterface
    ├─ If options.ResolveInherits
    │   └─ Add base type (excluding object)
    │
    ├─ If options.ResolveInterfaces
    │   └─ ResolveGenericsFromType(interfaces)
    │       ├─ For each interface:
    │       │   ├─ If generic type definition
    │       │   │   └─ Resolve generic arguments
    │       │   └─ Add interface itself
    │
    ├─ If options.ResolveFields
    │   └─ ResolveGenericsFromType(fieldTypes)
    │
    ├─ If options.ResolveProperties
    │   └─ ResolveGenericsFromType(propertyTypes)
    │       ├─ For each property type:
    │       │   ├─ If generic type definition
    │       │   │   └─ Recursively resolve
    │       │   └─ Add to dependencies
    │
    └─ If options.ResolveMethods
        └─ For each method:
            ├─ ResolveGenericsFromType(parameterTypes)
            └─ ResolveGenericsFromType(returnType)
    ↓
Filter out ignored types
    ├─ Remove duplicates
    ├─ Remove built-in .NET types
    └─ Remove custom ignored types
    ↓
Return List<(Type, TypeKind)>
```

### Transitive Dependency Resolution

```
GetAllDependencies(params Type[] types)
    ├─ allDependencies = Dictionary<string, Type>
    ├─ typesToProcess = Queue<Type>(types)
    │
    └─ While typesToProcess.Count > 0
        ├─ currentType = typesToProcess.Dequeue()
        ├─ dependencies = GetDependencies(currentType)
        │
        └─ For each dependency:
            ├─ If not already in allDependencies
            │   ├─ Add to allDependencies
            │   └─ Enqueue for processing
            └─ Skip if already seen
    ↓
Return Dictionary<string, Type> (all unique dependencies)
```

### Example: Resolving Order Dependencies

```
Input: Type = Order class with:
- List<OrderItem> Items
- OrderStatus Status
- User CreatedBy (implements IAuditable)

TypeDependencyResolver with default options:
├─ ResolveProperties: true
├─ ResolveInterfaces: true
├─ ResolveInherits: true
└─ Others: false

GetDependencies(typeof(Order), includeSelf=true)
├─ Add Order itself
├─ ResolveInherits: BaseType = EntityBase
│   └─ Add EntityBase
├─ ResolveInterfaces: 
│   ├─ Implements IEntity<int>
│   ├─ Resolve IEntity<int>
│   │   ├─ Get generic args [int]
│   │   └─ Add IEntity<>
│   └─ Add IEntity<int>
└─ ResolveProperties:
    ├─ Items: List<OrderItem>
    │   ├─ Is IEnumerable<T>
    │   ├─ Get argument OrderItem
    │   └─ Add OrderItem
    ├─ Status: OrderStatus (enum)
    │   └─ Add OrderStatus
    └─ CreatedBy: User
        └─ Add User

Return: [
    (Order, Class),
    (EntityBase, Class),
    (IEntity<>, Interface),
    (IEntity<int>, Interface),
    (OrderItem, Class),
    (OrderStatus, Enum),
    (User, Class)
]
```

---

## Code Generation Pipeline

### OneFile Generation Flow

```
OneFileGenerator.Generate(GeneratorType[] types)
    ├─ Initialize T4 template: TypesScriptGenerator
    ├─ For each GeneratorType:
    │   └─ Pass to template
    └─ Return single .ts file content

Template Processing (TypesScriptGenerator.tt):
├─ Generate auto-generated header
├─ Generate ICommand interface
├─ For each command class:
│   ├─ Generate class signature
│   ├─ Include members
│   └─ Generate internal type property
├─ For each enum:
│   ├─ Generate enum signature
│   └─ Generate enum members
└─ Generate output string
```

### SeparatedFiles Generation Flow

```
SeparatedFilesGenerator.Generate(GeneratorType[] types)
    ├─ Group by GeneratorTypeKind:
    │   ├─ commands = types where Kind == CommandClass
    │   ├─ enums = types where Kind == Enum
    │   ├─ complexTypes = types where Kind == Class
    │   └─ interfaces = types where Kind == Interface
    │
    ├─ For commands:
    │   ├─ Template: CommandTypeScriptGenerator.tt
    │   ├─ Output: commands/xxx.ts
    │   └─ Create: commands/index.ts (barrel export)
    │
    ├─ For enums:
    │   ├─ Template: EnumTypeScriptGenerator.tt
    │   ├─ Output: enums/xxx.ts
    │   └─ Create: enums/index.ts (barrel export)
    │
    ├─ For complexTypes:
    │   ├─ Template: ComplexTypeScriptGenerator.tt
    │   ├─ Generate imports based on dependencies
    │   ├─ Output: types/xxx.ts
    │   └─ Create: types/index.ts (barrel export)
    │
    ├─ For interfaces:
    │   ├─ Template: CommandInterface.tt
    │   ├─ Output: interfaces/xxx.ts
    │   └─ Create: interfaces/index.ts (barrel export)
    │
    ├─ Generate CodeGenerationWarning.ts
    ├─ Generate TypeScriptImports.ts (with all imports)
    │
    └─ Return BuildedSeparatedFiles
        └─ BuildFiles: List<BuildFile>
            ├─ FileName, FileType, FilePath, FileContent
            └─ Ready for file system writing
```

### T4 Template Execution

```
Template.TransformText()
    ├─ Initialize output buffer
    ├─ Write header comments
    ├─ Process template directives
    ├─ Execute template logic
    │   ├─ Conditional generation based on GeneratorType
    │   ├─ Loop through Members
    │   ├─ Generate documentation comments
    │   └─ Output TypeScript syntax
    └─ Return complete output string
```

---

## Design Patterns

### 1. Strategy Pattern (Type Generation)

**Problem**: Different metadata extraction logic for different type kinds

**Solution**:
```csharp
// Each type kind has its own strategy
ClassGeneratorType.Get(type)
EnumGeneratorType.Get(type)
InterfaceGeneratorType.Get(type)

// MetadataHelper routes to correct strategy
public List<GeneratorType> GetGeneratorTypesMetadata(types)
{
    foreach (var type in types)
    {
        if (type.IsEnum) 
            yield return EnumGeneratorType.Get(type);
        // ... etc
    }
}
```

**Benefits**:
- Each strategy is independent and testable
- Easy to add new type kinds
- Separation of concerns

### 2. Static Factory Pattern

**Problem**: Creating `GeneratorType` objects with complex initialization

**Solution**:
```csharp
public static class ClassGeneratorType
{
    public static GeneratorType Get(Type type, Type? returnTypeFilter)
    {
        // Complex initialization logic
        return new GeneratorType { ... };
    }
    
    public static GeneratorType GetCommand(Type type, ...)
    {
        // Different initialization for commands
        return new GeneratorType { ... };
    }
}
```

**Benefits**:
- Encapsulates complex creation logic
- Allows overloading for different scenarios
- Testable in isolation

### 3. Template Method Pattern (T4 Templates)

**Problem**: Generating different TypeScript for different type categories

**Solution**:
- Base template structure defined in `.tt` files
- Each template type has its own file
- Templates override parts while keeping structure consistent

### 4. Facade Pattern (MetadataHelper)

**Problem**: Complex orchestration of type extraction across multiple strategies

**Solution**:
```csharp
public static class MetadataHelper
{
    public static List<GeneratorType> GetGeneratorTypesMetadata(...)
    {
        // Coordinates between ClassGeneratorType, EnumGeneratorType, etc.
    }
}
```

**Benefits**:
- Simple public interface
- Hides complexity of strategy selection
- Central place for filtering and processing

### 5. Builder Pattern (Generators)

**Problem**: Constructing complex output with multiple stages

**Solution**:
- Generator classes build output step-by-step
- Each generator configures its aspects
- Final output assembled at the end

### 6. Visitor-like Pattern (TypeDependencyResolver)

**Problem**: Traversing type hierarchy to collect dependencies

**Solution**:
```csharp
// Recursive traversal of type structure
GetDependencies()
├─ Visits properties
├─ Visits interfaces
├─ Visits base type
└─ Recursively visits dependencies
```

---

## Extension Points

### Adding Support for New Type Categories

**Scenario**: Support for C# record types

**Steps**:

1. **Create Strategy Class**:
```csharp
// Tools/GeneratorTypes/RecordGeneratorType.cs
public static class RecordGeneratorType
{
    public static GeneratorType Get(Type type, Type? returnTypeFilter)
    {
        // Similar to ClassGeneratorType.Get()
        // But Kind = GeneratorTypeKind.Record
        return new GeneratorType { ... };
    }
}
```

2. **Add Enum Value**:
```csharp
public enum GeneratorTypeKind
{
    Interface,
    Enum,
    Class,
    CommandClass,
    Record  // New
}
```

3. **Update MetadataHelper**:
```csharp
public static List<GeneratorType> GetGeneratorTypesMetadata(...)
{
    // Add check for record types
    if (type.IsRecord)
        yield return RecordGeneratorType.Get(type);
}
```

4. **Add Template**:
```
Templates/SeparatedFiles/Records/RecordTypeScriptGenerator.tt
```

5. **Update Generator**:
```csharp
// In SeparatedFilesGenerator.Generate()
var records = types.Where(t => t.Kind == GeneratorTypeKind.Record);
foreach (var record in records)
{
    // Generate record files using template
}
```

### Customizing Type Name Resolution

**Scenario**: Map custom types to specific TypeScript types

**Option 1: Extend TypeNameResolver**:
```csharp
public static class TypeNameResolver
{
    // Add special handling
    if (type == typeof(CustomType))
        return "CustomTypeScript";
}
```

**Option 2: Preprocess Types**:
```csharp
// Map custom types before passing to generator
var customMappings = new Dictionary<Type, string>
{
    { typeof(CustomType), "CustomTypeScript" }
};
// Use during resolution...
```

### Custom Dependency Filtering

**Scenario**: Exclude certain types from dependency resolution

```csharp
var resolver = new TypeDependencyResolver(
    ignoreTypes: new List<Type>
    {
        typeof(EntityBase),
        typeof(BaseRepository)
    },
    options: new TypeDependencyResolverOptions
    {
        ResolveProperties = true,
        ResolveMethods = false  // Skip method return types
    }
);
```

### Custom Documentation Processing

**Scenario**: Transform documentation format for TypeScript

```csharp
// Load docs
DocumentationTools.LoadXmlDocumentation(assembly);

// Get docs and transform
var docs = type.GetDocumentation();
var customDocs = TransformToCustomFormat(docs);

// Pass to template...
```

---

## Data Flow: From Types to TypeScript Files

### Complete End-to-End Example

```
Input: C# Assembly with types
    └─ Order class (with List<OrderItem>, User, OrderStatus)
    └─ OrderItem class
    └─ OrderStatus enum
    └─ User class
    └─ OrderCommand : ICommand<Order>

↓ Step 1: Gather Types
var types = assembly.ExportedTypes;
    └─ [Order, OrderItem, OrderStatus, User, OrderCommand, ...]

↓ Step 2: Extract Metadata
MetadataHelper.GetGeneratorTypesMetadata(types)
    ├─ EnumGeneratorType.Get(OrderStatus)
    │   └─ GeneratorType { Kind = Enum, Name = "OrderStatus", ... }
    │
    ├─ ClassGeneratorType.Get(Order)
    │   └─ GeneratorType {
    │       Kind = Class,
    │       Name = "Order",
    │       Members = [
    │           { Name = "Items", GenericName = "OrderItem[]" },
    │           { Name = "CreatedBy", GenericName = "User" }
    │       ]
    │   }
    │
    ├─ ClassGeneratorType.Get(OrderItem)
    │   └─ GeneratorType { Kind = Class, ... }
    │
    ├─ ClassGeneratorType.Get(User)
    │   └─ GeneratorType { Kind = Class, ... }
    │
    └─ ClassGeneratorType.GetCommand(OrderCommand)
        └─ GeneratorType {
            Kind = CommandClass,
            Name = "OrderCommand",
            ImplementsInterfaceTypeNames = ["ICommand<Order>"]
        }

↓ Step 3: Resolve Dependencies
resolver.GetAllDependencies(typeof(Order))
    └─ { Order, OrderItem, User, OrderStatus, ... }

↓ Step 4: Generate (Separated Files Mode)
SeparatedFilesGenerator.Generate(generatorTypes)
    ├─ Group types by kind
    ├─ Generate imports for each file
    │   ├─ Order imports OrderItem and User
    │   ├─ OrderItem imports nothing
    │   ├─ User imports nothing
    │   └─ OrderCommand imports nothing (or ICommand)
    ├─ Apply templates
    │   ├─ ComplexTypeScriptGenerator for Order, OrderItem, User
    │   ├─ EnumTypeScriptGenerator for OrderStatus
    │   ├─ CommandTypeScriptGenerator for OrderCommand
    │   └─ CommandInterface for ICommand<T>
    └─ Return BuildedSeparatedFiles

↓ Step 5: Output Files
models/order.ts:
```typescript
import { OrderItem } from './orderItem';
import { User } from './user';

export class Order {
    public items?: OrderItem[];
    public createdBy?: User;
}
```

models/orderItem.ts:
```typescript
export class OrderItem {
    // ...
}
```

models/user.ts:
```typescript
export class User {
    // ...
}
```

enums/orderStatus.ts:
```typescript
export enum OrderStatus {
    Pending = 0,
    // ...
}
```

commands/orderCommand.ts:
```typescript
import { Order } from '../models/order';

export class OrderCommand implements ICommand<Order> {
    // ...
}
```

interfaces/iCommand.ts:
```typescript
export interface ICommand<T> {
    _?: T;
}
```
```

---

## Performance Characteristics

### Time Complexity

| Operation | Complexity | Notes |
|-----------|-----------|-------|
| TypeNameResolver.Resolve() | O(d) | d = depth of generic nesting |
| MetadataHelper.GetGeneratorTypesMetadata() | O(n × m) | n = types, m = avg properties |
| TypeDependencyResolver.GetDependencies() | O(p + i + f) | p = properties, i = interfaces, f = fields |
| TypeDependencyResolver.GetAllDependencies() | O((n + e) × log n) | n = nodes, e = edges (BFS) |
| Template.TransformText() | O(n) | n = size of output |

### Space Complexity

| Data Structure | Complexity | Notes |
|---|---|---|
| LoadedXmlDocumentation | O(m) | m = members with docs |
| LoadedAssemblies | O(a) | a = unique assemblies |
| GeneratorType list | O(n) | n = total types |
| Dependency graph | O(n + e) | n = types, e = dependencies |

---

## Concurrency Considerations

**Current Status**: NOT thread-safe

**Shared State**:
- `LoadedXmlDocumentation`: Static dictionary
- `LoadedAssemblies`: Static HashSet

**Implications**:
- Use locks if calling from multiple threads
- Consider ThreadLocal<T> for XML doc caching
- TypeDependencyResolver is stateless (thread-safe)

**Future**: Consider adding thread-safe variants for ASP.NET Core scenarios

