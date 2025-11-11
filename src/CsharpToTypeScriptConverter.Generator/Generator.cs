using TypeScriptRequestCommandsGenerator.Generators.TypeScript;

namespace TypeScriptRequestCommandsGenerator;

public class Generator
{
    public TypeScriptGenerator TypeScript()
    {
        return new TypeScriptGenerator();
    }
}
