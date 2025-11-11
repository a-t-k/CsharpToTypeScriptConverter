using System;
using System.IO;
using System.Linq;

namespace TypeScriptRequestCommandsGenerator.Tools
{
    public class TypeFileGenerator
    {
        private readonly TypeFileGeneratorOptions options = new();

        public TypeFileGenerator(TypeFileGeneratorOptions options = null)
        {
            if (options is not null)
            {
                this.options = options;
            }
        }

        public void CreateDirectoryIfNotExist(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        public string BuildFilePath(Type type, string outputDirectory)
        {
            string namespacePath = this.NamespacePath(type);
            return Path.Combine(outputDirectory, namespacePath);
        }

        public string GetFileNameFromType(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            // name is generic. we get generic types 
            string genericName = $"{type.Name.Substring(0, type.Name.IndexOf($"`", StringComparison.Ordinal))}";
            return genericName;
        }

        public string NamespacePath(Type type) => type.Namespace?.Replace('.', '/') ?? "";

        public string GenerateRelativePath(string namespaceStr)
        {
            string relativePath = "";
            if (string.IsNullOrEmpty(namespaceStr))
            {
                return relativePath;
            }

            int levels = this.GetLevelsByDotsFromNamespace(namespaceStr);
            relativePath = this.GetRelativePath(levels);
            return relativePath;
        }

        private string GetRelativePath(int levels) =>
            Enumerable.Repeat(0, levels).Aggregate("", (current, x) => current + "../");

        private int GetLevelsByDotsFromNamespace(string namespaceStr) => namespaceStr.Split('.').Length;
    }

    public class TypeFileGeneratorOptions
    {
        public string RootFolder { get; set; } = string.Empty;
    }
}
