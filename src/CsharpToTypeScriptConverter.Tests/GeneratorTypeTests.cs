using TypeScriptRequestCommandsGenerator.Models;
using static Xunit.Assert;

namespace CsharpToTypeScriptConverter.Tests
{
    public class GeneratorTypeTests
    {
        [Fact]
        public void GetInterfaceTypeNames_Works()
        {
            var generatorType = new GeneratorType { ImplementsInterfaceTypeNames = ["ICommand<User<Test>>"] };
            string? genericInterface = generatorType.GetImplementsInterfaceTypeNames;
            True(genericInterface == "ICommand<User<Test>>");
        }

        [Fact]
        public void CommandReturnTypeName_Works()
        {
            var generatorType =
                new GeneratorType { ImplementsInterfaceTypeNames = ["ICommand<Pagination<List<User>>>"] };
            string? commandReturnTypeName = generatorType.CommandReturnTypeName;
            True(commandReturnTypeName == "Pagination<List<User>>");
        }
    }
}
