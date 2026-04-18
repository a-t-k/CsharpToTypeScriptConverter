using System;
using ATK.Command.CsToTsGenerator.Models;

namespace ATK.Command.CsToTsGenerator.Tools.GeneratorTypes
{
    public static class InterfaceGeneratorType
    {
        public static GeneratorType Get(Type type, Type returnTypeFilter)
        {
            var generatorType = ClassGeneratorType.Get(type, returnTypeFilter);
            generatorType.Kind = GeneratorTypeKind.Interface;
            generatorType.Type = type;
            generatorType.Documentation = type.GetDocumentation()?.OnlyDocumentationText();
            return generatorType;
        }
    }
}
