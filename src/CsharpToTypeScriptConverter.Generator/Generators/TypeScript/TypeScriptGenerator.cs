using TypeScriptRequestCommandsGenerator.Generators.TypeScript.OneFile;
using TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript
{
    public class TypeScriptGenerator
    {
        public OneFileGenerator OneFile() => new();

        public SeparatedFilesGenerator SeparatedFiles() => new();
    }
}
