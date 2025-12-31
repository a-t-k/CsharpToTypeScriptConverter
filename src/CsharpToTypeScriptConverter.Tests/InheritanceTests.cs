using TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes;
using static Xunit.Assert;

namespace CsharpToTypeScriptConverter.Tests
{
    public class InheritanceTests
    {
        [Fact]
        public void Generate_Works()
        {
            var meta = ClassGeneratorType.Get(typeof(MyDerivedTestClass), null);
            NotNull(meta);
            True(meta.BaseTypeName == "MyBaseClass<MyGenericType>");
            True(meta.Name == "MyDerivedTestClass");
            True(meta.Members.Count() == 3);

            meta = ClassGeneratorType.Get(typeof(MyBaseClass<>), null);
            NotNull(meta);
            True(meta.BaseTypeName == string.Empty);
            True(meta.Name == "MyBaseClass<T>");
            True(meta.Members.Count() == 1);
        }

        private class MyDerivedTestClass : MyBaseClass<MyGenericType>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class MyBaseClass<T>
        {
            public T Data { get; set; }
        }

        private class MyGenericType
        {
        }
    }
}
