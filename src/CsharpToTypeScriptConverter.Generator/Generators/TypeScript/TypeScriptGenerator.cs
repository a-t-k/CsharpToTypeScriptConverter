using ATK.Command.CsToTsGenerator.Generators.TypeScript.OneFile;
using ATK.Command.CsToTsGenerator.Generators.TypeScript.SeparatedFiles;

namespace ATK.Command.CsToTsGenerator.Generators.TypeScript
{
    public class TypeScriptGenerator
    {
        public OneFileGenerator OneFile() => new();

        public SeparatedFilesGenerator SeparatedFiles() => new();
    }
}
