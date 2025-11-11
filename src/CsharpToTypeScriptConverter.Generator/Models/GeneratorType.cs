using System;
using System.Collections.Generic;

namespace TypeScriptRequestCommandsGenerator.Models;

public class GeneratorType
{
    public string Name { get; set; }
    public string TypeNameForJsonDeserialization { get; set; }
    public string ReturnTypeName { get; set; }
    public GeneratorTypeKind Kind { get; set; }
    public IEnumerable<GeneratorMember> Members { get; set; }
    public string[] ImplementsInterfaceTypeNames { get; set; } = [];
    public string[] Documentation { get; set; }
    public string GeneratedCode { get; set; }
    public Type Type { get; set; }
}
