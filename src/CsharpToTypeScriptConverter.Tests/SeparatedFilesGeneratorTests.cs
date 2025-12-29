using CollisionFreeNamespaceWithCollisionTypeNames;
using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Models;

namespace CsharpToTypeScriptConverter.Tests
{
    public class SeparatedFilesGeneratorTests
    {
        [Fact]
        public void GenerateWithSeparatedFilesGeneratorWorks()
        {
            string outputDirectory = "test";
            var generator = new Generator()
                .TypeScript()
                .SeparatedFiles()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(typeof(IRequestCommand))
                .SetReturnTypeOfCommands(typeof(ICommand<>))
                .AddRangeOfCommandTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .GenerateMetadata()
                .Generate([typeof(ICommand<>), typeof(IRequestCommand)])
                .Build(outputDirectory);

            int countGeneratedFiles = generator.BuildFiles.Count;
            int countApiFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("api.ts"));
            int countIndexFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("index.ts"));

            Assert.True(countGeneratedFiles == 16);
            Assert.True(countApiFiles == 4);
            Assert.True(countIndexFiles == 1);
        }

        [Fact]
        public void GenerateWithSeparatedFilesGeneratorWorksAndGenerateCollisionFreeNames()
        {
            string outputDirectory = "test";
            var generator = new Generator()
                .TypeScript()
                .SeparatedFiles()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(typeof(IRequestCommand))
                .SetReturnTypeOfCommands(typeof(ICommand<>))
                .AddRangeOfCommandTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .AddRangeOfExtraTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .AddRangeOfExtraTypesToGenerate(typeof(NameSpaceForNameCollision.UserRoles).Assembly.ExportedTypes)
                .AddRangeOfExtraTypesToGenerate(typeof(UserRoles_1).Assembly.ExportedTypes)
                .AddRangeOfExtraTypesToGenerate([typeof(ClassWithoutReferences.ClassWithoutReferences)])
                .GenerateMetadata()
                .Generate([typeof(ICommand<>), typeof(IRequestCommand)])
                .Build(outputDirectory);

            int countIndexFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("index.ts"));
            int countCollisionFreeNameInApiContent = generator.BuildFiles.Count(f =>
                f.BuildFiLeType == BuildFiLeType.ApiFile && f.Content.Contains("UserRoles_4"));

            Assert.True(countIndexFiles == 1);
            Assert.True(countCollisionFreeNameInApiContent == 1);
        }


        [Fact]
        public void Generating_ChangeUserRoleRequestCommand_Works()
        {
            string outputDirectory = "test";
            var interfaceFilterType = typeof(IRequestCommand);
            var interfaceCommandType = typeof(ICommand<>);
            var generator = new Generator()
                .TypeScript()
                .SeparatedFiles()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(interfaceFilterType)
                .SetReturnTypeOfCommands(interfaceCommandType)
                .AddRangeOfCommandTypesToGenerate([typeof(ChangeUserRoleRequestCommand)])
                .GenerateMetadata()
                .Generate([interfaceCommandType, interfaceFilterType])
                .Build(outputDirectory);

            int countIndexFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("index.ts"));

            Assert.True(countIndexFiles == 1);
        }

        [Fact]
        public void Generating_GetUsersRequestCommand_Works()
        {
            string outputDirectory = "test";
            var interfaceFilterType = typeof(IRequestCommand);
            var interfaceCommandType = typeof(ICommand<>);
            var generator = new Generator()
                .TypeScript()
                .SeparatedFiles()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(interfaceFilterType)
                .SetReturnTypeOfCommands(interfaceCommandType)
                .AddRangeOfCommandTypesToGenerate([typeof(GetUsersRequestCommand)])
                .GenerateMetadata()
                .Generate([interfaceCommandType, interfaceFilterType])
                .Build(outputDirectory);

            int countIndexFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("index.ts"));

            Assert.True(countIndexFiles == 1);
        }
    }
}
