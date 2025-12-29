using System;

namespace TypeScriptRequestCommandsGenerator.Models;

public class GeneratorMember
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public bool IsDeclaredAsGeneric { get; set; }
    public string GenericName { get; set; }

    public override string ToString() =>
        $"{this.Name}" + (this.IsDeclaredAsGeneric ? $"-GenericName: {this.GenericName}" : string.Empty);
}
