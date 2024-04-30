using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Templates;
namespace CsharpToTypeScriptConverter.Tests;

public class Generate
{
    [Fact]
    public void GenerateWorks()
    {
        // interface type to find all request commands.
        var requestCommandType = typeof(IRequestCommand);
            
        // intern defined command with generic return type. 
        var returnType = typeof(ICommand<>);
        var usedTypes = new Dictionary<string, Type>();

        TypesScriptGenerator.Settings.RequestCommandInterfaceName = "ICommand";
            
        var typesMetadata = MetadataHelper.GetGeneratorTypesMetadata(typeof(UserRoles).Assembly.ExportedTypes, requestCommandType, returnType, usedTypes);

        // types that generator not generated, because they are deeper in definition
        var notGeneratedTypes = usedTypes.Where(ut => typesMetadata.All(tm => tm.Name != ut.Key)).ToDictionary();
        // generate it, when some are there. (this can be looped)
        typesMetadata.AddRange(MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes));

        var typesGenerator = new TypesScriptGenerator { GeneratorTypes = typesMetadata.ToArray() };
        var transformedText = typesGenerator.TransformText().Trim();

        var enumExists = transformedText.Contains("export enum UserRoles");
        var classExists = transformedText.Contains("export class ChangeUserRoleRequestCommand implements ICommand<boolean>");
        var requestCommandInterfaceExists = transformedText.Contains("export interface ICommand<T>{ _?: T}");
            
        Assert.True(enumExists);
        Assert.True(classExists);
        Assert.True(requestCommandInterfaceExists);
    }
}