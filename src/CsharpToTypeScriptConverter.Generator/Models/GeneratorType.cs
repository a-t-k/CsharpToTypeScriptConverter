using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TypeScriptRequestCommandsGenerator.Models
{
    public class GeneratorType
    {
        public string Name { get; set; }
        public string TypeNameForJsonDeserialization { get; set; }
        public string ReturnTypeName { get; set; }

        public string CommandReturnTypeName
        {
            get
            {
                var regex = new Regex(@"ICommand<([^>]+)>");
                if (this.ImplementsInterfaceTypeNames.Length == 1)
                {
                    string interfaceDefinition = this.GetImplementsInterfaceTypeNames;
                    var match = regex.Match(interfaceDefinition);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }

                return string.Empty;
            }
        }

        public GeneratorTypeKind Kind { get; set; }
        public IEnumerable<GeneratorMember> Members { get; set; }
        public string[] ImplementsInterfaceTypeNames { get; set; } = [];

        public string GetImplementsInterfaceTypeNames =>
            this.ImplementsInterfaceTypeNames.Aggregate((i1, i2) => $"{i1}, {i2}");

        public string BaseTypeName { get; set; }

        public string[] Documentation { get; set; }
        public string GeneratedCode { get; set; }
        public Type Type { get; set; }
        public override string ToString() => $"{this.Kind}-{this.Name}";
    }
}
