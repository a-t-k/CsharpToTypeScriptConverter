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
                .AddRangeOfTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
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
                .AddRangeOfTypesToGenerate(typeof(UserRoles).Assembly.ExportedTypes)
                .AddRangeOfTypesToGenerate(typeof(NameSpaceForNameCollision.UserRoles).Assembly.ExportedTypes)
                .AddRangeOfTypesToGenerate(typeof(UserRoles_1).Assembly.ExportedTypes)
                .GenerateMetadata()
                .Generate([typeof(ICommand<>), typeof(IRequestCommand)])
                .Build(outputDirectory);

            int countIndexFiles = generator.BuildFiles.Count(f => f.Path.EndsWith("index.ts"));
            int countCollisionFreeNameInApiContent = generator.BuildFiles.Count(f =>
                f.BuildFiLeType == BuildFiLeType.ApiFile && f.Content.Contains("UserRoles_4"));

            Assert.True(countIndexFiles == 1);
            Assert.True(countCollisionFreeNameInApiContent == 1);
        }
    }
}
