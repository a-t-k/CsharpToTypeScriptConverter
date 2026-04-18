using System;

namespace ATK.Command.CsToTsGenerator.Models;

public class GeneratorMember
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public string PropertyTypeName { get; set; }

    public override string ToString() =>
        $"{this.Name}-{this.PropertyTypeName}";
}
