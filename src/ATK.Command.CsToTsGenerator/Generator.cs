using ATK.Command.CsToTsGenerator.Generators.TypeScript;

namespace ATK.Command.CsToTsGenerator;

public class Generator
{
    public TypeScriptGenerator TypeScript()
    {
        return new TypeScriptGenerator();
    }
}
