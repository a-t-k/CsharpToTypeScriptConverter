using System.IO;
using System.Text;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Templates;
using TypeScriptRequestCommandsGenerator.Templates.OneFIle;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.OneFile
{
    public class OneFileGeneratorWithMetaData(GeneratorType[] metadata)
    {
        public string TransformedText { get; set; }

        public OneFileGeneratorWithMetaData GenerateString()
        {
            // and transform it to type script
            var typesGenerator = new TypesScriptGenerator { GeneratorTypes = metadata };
            this.TransformedText = typesGenerator.TransformText().Trim();
            return this;
        }

        public OneFileGeneratorWithMetaData Save(DirectoryInfo directory, string fileName)
        {
            if (directory.Exists)
            {
                string path = Path.Combine(directory.FullName, fileName);
                File.AppendAllText(path, this.TransformedText, Encoding.UTF8);
            }

            return this;
        }
    }
}
