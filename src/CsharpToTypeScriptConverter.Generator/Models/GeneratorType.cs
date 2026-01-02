using System;
using System.Collections.Generic;
using System.Linq;

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
                if (this.ImplementsInterfaceTypeNames.Length == 1)
                {
                    string result = GetGenericParameter(this.GetImplementsInterfaceTypeNames);
                    return result;
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

        private static string GetGenericParameter(string typeString)
        {
            // Find the index of the opening angle bracket
            int openBracketIndex = typeString.IndexOf('<');
            if (openBracketIndex == -1)
            {
                return null; // Not a generic type
            }

            // Find the matching closing angle bracket
            int closeBracketIndex = FindMatchingCloseBracket(typeString, openBracketIndex);
            if (closeBracketIndex == -1)
            {
                return null; // Unmatched brackets
            }

            string genericParams = typeString.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            return genericParams;
        }

        // Helper function to find the matching closing bracket
        private static int FindMatchingCloseBracket(string str, int openIndex)
        {
            int count = 1;
            for (int i = openIndex + 1; i < str.Length; i++)
            {
                if (str[i] == '<')
                {
                    count++;
                }
                else if (str[i] == '>')
                {
                    count--;
                    if (count == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
