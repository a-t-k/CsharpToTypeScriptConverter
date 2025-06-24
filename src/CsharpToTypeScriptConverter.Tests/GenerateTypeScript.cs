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
        var notGeneratedTypes = usedTypes.Where(ut => typesMetadata.ToArray().All(tm => tm.Name != ut.Key)).ToDictionary();
        var newMetaData = MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes);

        // generate it
        typesMetadata.AddRange(newMetaData);

        // distinct all generated types
        var distinctGeneratorTypes = typesMetadata.GroupBy(x => x.Name).Select(x => x.First()).ToArray();

        // and transform it to type script
        var typesGenerator = new TypesScriptGenerator { GeneratorTypes = distinctGeneratorTypes };
        var transformedText = typesGenerator.TransformText().Trim();

        var enumExists = transformedText.Contains("export enum UserRoles");
        var classExists = transformedText.Contains("export class ChangeUserRoleRequestCommand implements ICommand<boolean>");
        var requestCommandInterfaceExists = transformedText.Contains("export interface ICommand<T>{ _?: T}");
        var genericTypePropertyHasFullName = transformedText.Contains("users: PaginationResponse<User>;");

        Assert.True(enumExists);
        Assert.True(classExists);
        Assert.True(requestCommandInterfaceExists);
        Assert.True(genericTypePropertyHasFullName);
    }
}