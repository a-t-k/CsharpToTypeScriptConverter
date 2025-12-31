# CsharpToTypeScriptConverter Documentation Index

Welcome to the CsharpToTypeScriptConverter project documentation! This index will help you navigate all available documentation.

## 📚 Documentation Files

### Getting Started
- **[README.md](README.md)** - Main project documentation
  - Project overview and key features
  - Quick start guide
  - Usage examples
  - Basic architecture overview
  - Development guide basics
  - **Start here for**: Quick understanding of what the project does

### In-Depth Resources

#### [ARCHITECTURE.md](ARCHITECTURE.md)
Detailed architectural documentation covering:
- Component interaction flow
- Type resolution pipeline
- Metadata extraction strategy
- Dependency resolution system
- Code generation pipeline
- Design patterns used
- Extension points
- Performance characteristics
- Data flow examples
- **Read this for**: Understanding how components work together, design decisions, and extending the system

#### [CONTRIBUTING.md](CONTRIBUTING.md)
Developer guide for contributors:
- Development setup instructions
- Project structure overview
- Building and testing
- Common development tasks
- Testing guidelines
- Code style conventions
- Pull request process
- Troubleshooting
- **Read this for**: Contributing to the project, setting up your development environment, and following project conventions

#### [SOLUTION_DOCUMENTATION.md](SOLUTION_DOCUMENTATION.md)
Original comprehensive solution documentation:
- Complete feature list
- Detailed component descriptions
- Key models and their properties
- Generation modes explained
- **Read this for**: Comprehensive reference of all features and components (legacy, see README for latest)

## 🎯 Quick Navigation by Topic

### I want to...

#### **Use the library**
1. Read: [README.md - Usage Guide](README.md#usage-guide)
2. Check: [README.md - Type Name Resolution](README.md#type-name-resolution)
3. Run: Code examples from README

#### **Contribute code**
1. Start: [CONTRIBUTING.md - Development Setup](CONTRIBUTING.md#development-setup)
2. Follow: [CONTRIBUTING.md - Code Style](CONTRIBUTING.md#code-style)
3. Test: [CONTRIBUTING.md - Testing Guidelines](CONTRIBUTING.md#testing-guidelines)
4. Submit: [CONTRIBUTING.md - Pull Request Process](CONTRIBUTING.md#pull-request-process)

#### **Understand how types are resolved**
1. Read: [README.md - Type Name Resolution](README.md#type-name-resolution)
2. Deep dive: [ARCHITECTURE.md - Type Resolution Pipeline](ARCHITECTURE.md#type-resolution-pipeline)
3. Code: `src/CsharpToTypeScriptConverter.Generator/Tools/TypeNameResolver.cs`

#### **Add a new feature**
1. Review: [ARCHITECTURE.md - Extension Points](ARCHITECTURE.md#extension-points)
2. Follow: [CONTRIBUTING.md - Common Development Tasks](CONTRIBUTING.md#common-development-tasks)
3. Test: [CONTRIBUTING.md - Testing Guidelines](CONTRIBUTING.md#testing-guidelines)

#### **Understand the type generation process**
1. Start: [README.md - Type Generation Strategies](README.md#type-generation-strategies)
2. Details: [ARCHITECTURE.md - Metadata Extraction Strategy](ARCHITECTURE.md#metadata-extraction-strategy)
3. Code: `src/CsharpToTypeScriptConverter.Generator/Tools/GeneratorTypes/`

#### **Work with dependencies and imports**
1. Overview: [README.md - Type Dependencies](README.md#type-dependencies)
2. Deep dive: [ARCHITECTURE.md - Dependency Resolution System](ARCHITECTURE.md#dependency-resolution-system)
3. Code: `src/CsharpToTypeScriptConverter.Generator/Tools/TypeDependencyResolver.cs`

#### **Learn about design patterns**
Read: [ARCHITECTURE.md - Design Patterns](ARCHITECTURE.md#design-patterns)

#### **Troubleshoot an issue**
1. Check: [README.md - Common Issues and Solutions](README.md#common-issues-and-solutions)
2. More: [CONTRIBUTING.md - Troubleshooting](CONTRIBUTING.md#troubleshooting-common-issues)

## 📋 Document Summaries

### README.md
- **Length**: ~1000 lines
- **Audience**: Users, developers, contributors
- **Content**: Features, architecture overview, models, usage, development guide, testing
- **Updates**: Latest (includes recent refactoring with TypeNameResolver, GeneratorTypes strategies)

### ARCHITECTURE.md
- **Length**: ~1200 lines
- **Audience**: Architects, advanced developers, contributors
- **Content**: Component flows, resolution pipelines, data structures, design patterns, extension points, performance
- **Focus**: How and why components interact the way they do

### CONTRIBUTING.md
- **Length**: ~800 lines
- **Audience**: Contributors and developers
- **Content**: Setup, building, testing, code style, PR process, troubleshooting
- **Focus**: Practical guidance for contributing and extending the project

### SOLUTION_DOCUMENTATION.md
- **Length**: ~600 lines
- **Audience**: Users and developers
- **Content**: Comprehensive feature overview, models, usage examples
- **Status**: Original documentation (kept for reference, newer content in README)

## 🔍 Key Concepts Cross-Reference

### Type Name Resolution
- **What**: Converting C# types to TypeScript type names
- **How**: `TypeNameResolver` class
- **Where**: README [Type Name Resolution](README.md#type-name-resolution)
- **Details**: ARCHITECTURE [Type Resolution Pipeline](ARCHITECTURE.md#type-resolution-pipeline)
- **Code**: `Tools/TypeNameResolver.cs`
- **Tests**: `TypeNameResolverTests.cs`

### Metadata Extraction
- **What**: Extracting information from C# types (properties, base types, interfaces)
- **How**: Strategy classes: `ClassGeneratorType`, `EnumGeneratorType`, `InterfaceGeneratorType`
- **Where**: README [Type Generation Strategies](README.md#type-generation-strategies)
- **Details**: ARCHITECTURE [Metadata Extraction Strategy](ARCHITECTURE.md#metadata-extraction-strategy)
- **Code**: `Tools/GeneratorTypes/` folder
- **Tests**: `ClassGeneratorTypeTests.cs`, `InheritanceTests.cs`

### Dependency Resolution
- **What**: Finding which types depend on which other types
- **How**: `TypeDependencyResolver` with configurable options
- **Where**: README [Type Dependencies](README.md#type-dependencies)
- **Details**: ARCHITECTURE [Dependency Resolution System](ARCHITECTURE.md#dependency-resolution-system)
- **Code**: `Tools/TypeDependencyResolver.cs`
- **Tests**: Integration tests in generator tests

### Code Generation
- **What**: Converting metadata to TypeScript code
- **How**: T4 templates in `Templates/` folder
- **Where**: README [Generation Modes](README.md#generation-modes)
- **Details**: ARCHITECTURE [Code Generation Pipeline](ARCHITECTURE.md#code-generation-pipeline)
- **Code**: `Generators/` and `Templates/` folders

## 📁 File Organization

```
Documentation:
├── README.md                    (Main documentation - START HERE)
├── ARCHITECTURE.md              (Deep technical details)
├── CONTRIBUTING.md              (Developer guide)
├── SOLUTION_DOCUMENTATION.md    (Original comprehensive docs - reference)
└── DOCUMENTATION_INDEX.md       (This file)

Source Code:
├── Generator.cs                 (Entry point)
├── Models/                      (Data structures)
├── Tools/                       (Core logic)
│   ├── TypeNameResolver.cs
│   ├── MetadataHelper.cs
│   ├── TypeDependencyResolver.cs
│   ├── DocumentationTools.cs
│   └── GeneratorTypes/
│       ├── ClassGeneratorType.cs
│       ├── EnumGeneratorType.cs
│       └── InterfaceGeneratorType.cs
├── Generators/                  (Generation strategies)
└── Templates/                   (T4 code generation templates)

Tests:
├── TypeNameResolverTests.cs
├── ClassGeneratorTypeTests.cs
├── InheritanceTests.cs
├── OneFileGeneratorTests.cs
└── SeparatedFilesGeneratorTests.cs
```

## 🚀 Getting Started Paths

### Path 1: Quick Start (15 minutes)
1. Read: [README.md - Project Overview](README.md#project-overview) (2 min)
2. Read: [README.md - Key Features](README.md#project-overview) (1 min)
3. Read: [README.md - Usage Guide](README.md#usage-guide) (10 min)
4. Try: Run the examples

### Path 2: Understanding Architecture (1 hour)
1. Read: [README.md](README.md) (20 min)
2. Read: [ARCHITECTURE.md - Component Interaction Flow](ARCHITECTURE.md#component-interaction-flow) (10 min)
3. Read: [ARCHITECTURE.md - Design Patterns](ARCHITECTURE.md#design-patterns) (10 min)
4. Browse: Source code structure (20 min)

### Path 3: Contributing Setup (2 hours)
1. Read: [CONTRIBUTING.md - Development Setup](CONTRIBUTING.md#development-setup) (15 min)
2. Follow: Setup instructions (30 min)
3. Read: [CONTRIBUTING.md - Code Style](CONTRIBUTING.md#code-style) (15 min)
4. Read: [CONTRIBUTING.md - Testing Guidelines](CONTRIBUTING.md#testing-guidelines) (20 min)
5. Run: `dotnet test` (20 min)

### Path 4: Advanced Development (3 hours)
1. Complete: Path 3 above (2 hours)
2. Read: [ARCHITECTURE.md](ARCHITECTURE.md) (45 min)
3. Study: Relevant source files (15 min)

## 📚 Related Resources

### External Documentation
- [.NET Reflection API](https://docs.microsoft.com/en-us/dotnet/fundamentals/reflection/reflection)
- [C# Generics](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/generics/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [T4 Text Templates](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates)

### Repository
- **GitHub**: https://github.com/a-t-k/CsharpToTypeScriptConverter
- **NuGet**: https://www.nuget.org/packages/TypeScriptRequestCommandsGenerator
- **Issues**: GitHub Issues page
- **Discussions**: GitHub Discussions page

## 📝 Document Maintenance

### Last Updated
- README.md: Latest (includes recent refactoring)
- ARCHITECTURE.md: Created with latest architecture
- CONTRIBUTING.md: Created with contribution guidelines
- SOLUTION_DOCUMENTATION.md: Original documentation (kept for reference)

### How to Update Documentation
1. Make changes to relevant markdown file(s)
2. Test that examples still work
3. Update table of contents if structure changes
4. Check links are still valid
5. Commit with clear message: "docs: update [topic]"

## 🔗 Quick Links by Role

### 👨‍💻 As a User
- Quick example: [README.md - Usage Guide](README.md#usage-guide)
- Type mappings: [README.md - Type Name Resolution](README.md#type-name-resolution)
- Troubleshooting: [README.md - Common Issues](README.md#common-issues-and-solutions)

### 👨‍🔧 As a Contributor
- Setup: [CONTRIBUTING.md - Development Setup](CONTRIBUTING.md#development-setup)
- Conventions: [CONTRIBUTING.md - Code Style](CONTRIBUTING.md#code-style)
- Testing: [CONTRIBUTING.md - Testing Guidelines](CONTRIBUTING.md#testing-guidelines)
- Adding features: [CONTRIBUTING.md - Common Development Tasks](CONTRIBUTING.md#common-development-tasks)

### 🏗️ As an Architect
- Architecture: [ARCHITECTURE.md](ARCHITECTURE.md)
- Design patterns: [ARCHITECTURE.md - Design Patterns](ARCHITECTURE.md#design-patterns)
- Extension points: [ARCHITECTURE.md - Extension Points](ARCHITECTURE.md#extension-points)
- Performance: [ARCHITECTURE.md - Performance Characteristics](ARCHITECTURE.md#performance-characteristics)

### 📚 As a Learner
- Overview: [README.md - Project Overview](README.md#project-overview)
- Architecture overview: [README.md - Architecture](README.md#architecture)
- Design patterns: [ARCHITECTURE.md - Design Patterns](ARCHITECTURE.md#design-patterns)
- Data flow: [ARCHITECTURE.md - Data Flow](ARCHITECTURE.md#data-flow-from-types-to-typescript-files)

## ❓ FAQ

**Q: Where do I start?**
A: Start with [README.md](README.md) for a quick overview, then choose a path above based on your needs.

**Q: How do I set up development?**
A: Follow [CONTRIBUTING.md - Development Setup](CONTRIBUTING.md#development-setup).

**Q: How do types get converted to TypeScript?**
A: Read [README.md - Type Name Resolution](README.md#type-name-resolution) for overview, then [ARCHITECTURE.md - Type Resolution Pipeline](ARCHITECTURE.md#type-resolution-pipeline) for details.

**Q: Can I add support for new type categories?**
A: Yes! See [ARCHITECTURE.md - Extension Points](ARCHITECTURE.md#extension-points) for guidance.

**Q: What's the difference between the documentation files?**
A: README is for users/quick reference, ARCHITECTURE is for deep understanding, CONTRIBUTING is for developers, SOLUTION_DOCUMENTATION is legacy reference.

**Q: How do I run tests?**
A: See [CONTRIBUTING.md - Running Tests](CONTRIBUTING.md#running-tests).

**Q: Where's the code style guide?**
A: [CONTRIBUTING.md - Code Style](CONTRIBUTING.md#code-style).

## 🎓 Learning Outcomes

After reading these documents, you should understand:

### From README
- ✅ What the project does and why
- ✅ How to use it as a library
- ✅ Key architectural components
- ✅ How types are resolved to TypeScript names
- ✅ How metadata is extracted
- ✅ How dependencies are resolved
- ✅ How code is generated

### From ARCHITECTURE
- ✅ Detailed component interactions
- ✅ Type resolution algorithms
- ✅ Metadata extraction strategies
- ✅ Dependency resolution implementation
- ✅ Design patterns used throughout
- ✅ How to extend the system
- ✅ Performance characteristics

### From CONTRIBUTING
- ✅ How to set up development environment
- ✅ How to run tests
- ✅ Code style and conventions
- ✅ How to contribute changes
- ✅ Testing approach and practices
- ✅ How to debug issues

---

Happy learning! 📖

