using TypeScriptRequestCommandsGenerator;

namespace CsharpToTypeScriptConverter.Tests
{
    public class OneFileGeneratorTests
    {
        [Fact]
        public void GenerateWithOneFileGeneratorWorks()
        {
            var generator = new Generator()
                .TypeScript()
                .OneFile()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(typeof(IRequestCommand))
                .SetReturnTypeOfCommands(typeof(ICommand<>))
                .AddRangeOfCommandTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .AddRangeOfExtraTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .GenerateMetadata()
                .GenerateString();

            string? transformedText = generator.TransformedText;

            bool enumExists = transformedText.Contains("export enum UserRoles");
            bool classExists =
                transformedText.Contains("export class ChangeUserRoleRequestCommand implements ICommand<boolean>");
            bool requestCommandInterfaceExists = transformedText.Contains("export interface ICommand<T>{ _?: T}");
            bool genericTypePropertyHasFullName = transformedText.Contains("users: PaginationResponse<User>;");
            bool documentationOfGetUserRequestCommand =
                transformedText.Contains("Simple command to get users. Can be filtered and paged.");

            Assert.True(enumExists);
            Assert.True(classExists);
            Assert.True(requestCommandInterfaceExists);
            Assert.True(genericTypePropertyHasFullName);
            Assert.True(documentationOfGetUserRequestCommand);
        }
    }
}
