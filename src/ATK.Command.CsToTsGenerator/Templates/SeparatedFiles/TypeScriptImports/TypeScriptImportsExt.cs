using System.Collections.Generic;
using ATK.Command.CsToTsGenerator.Models;

namespace ATK.Command.CsToTsGenerator.Templates.SeparatedFiles.TypeScriptImports;

public partial class TypeScriptImports
{
    public List<TypeScriptImportDependency> Dependencies { get; set; }
}
