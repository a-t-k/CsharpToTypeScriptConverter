using TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes;
using static Xunit.Assert;

namespace CsharpToTypeScriptConverter.Tests
{
    public class ClassGeneratorTypeTests
    {
        [Fact]
        public void Generate_Works()
        {
            var meta = ClassGeneratorType.Get(typeof(GenerateTestClass), null);
            NotNull(meta);
            meta = ClassGeneratorType.Get(typeof(PaginationResponse<>), null);
            NotNull(meta);
            meta = ClassGeneratorType.Get(typeof(GetUsersRequestCommand), null);
            NotNull(meta);

            // test that interface detection works
            meta = ClassGeneratorType.Get(typeof(GenerateTestClassWithInterface), null);
            NotNull(meta);
            True(meta.ImplementsInterfaceTypeNames.Any());
        }

        private class GenerateTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class GenerateTestClassWithInterface : IGenerateTestInterface
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private interface IGenerateTestInterface
        {
        }
    }
}
