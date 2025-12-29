using System;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;

namespace TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes;

public static class EnumGeneratorType
{
    public static GeneratorType Get(Type type)
    {
        var generatorType = new GeneratorType
        {
            Name = type.Name,
            Type = type,
            Kind = GeneratorTypeKind.Enum,
            Members = type.GetFields()
                .Where(f => f.Name != "value__")
                .Select(f => new GeneratorMember { Name = f.Name, Type = f.FieldType }),
            Documentation = type.GetDocumentation()?.OnlyDocumentationText()
        };
        return generatorType;
    }
}
