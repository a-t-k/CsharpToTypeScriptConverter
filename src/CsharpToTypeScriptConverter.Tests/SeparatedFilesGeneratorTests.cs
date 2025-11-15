using TypeScriptRequestCommandsGenerator;

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

            Assert.True(countGeneratedFiles == 10);
            Assert.True(countApiFiles == 2);
            Assert.True(countIndexFiles == 1);
        }
    }
}
