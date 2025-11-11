using TypeScriptRequestCommandsGenerator.Generators.TypeScript.OneFile;
using TypeScriptRequestCommandsGenerator.Generators.TypeScript.SplitedFiles;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript
{
    public class TypeScriptGenerator
    {
        public OneFileGenerator OneFile()
        {
            return new OneFileGenerator();
        }

        public SeparatedFilesGenerator SeparatedFiles()
        {
            return new SeparatedFilesGenerator();
        }
    }
}
