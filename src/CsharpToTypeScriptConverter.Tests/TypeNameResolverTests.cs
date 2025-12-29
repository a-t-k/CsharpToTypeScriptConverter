using TypeScriptRequestCommandsGenerator.Tools;
using static Xunit.Assert;

namespace CsharpToTypeScriptConverter.Tests
{
    public class TypeNameResolverTests
    {
        [Fact]
        public void Resolve_DefaultName_Works()
        {
            string? name = TypeNameResolver.Resolve(typeof(DefaultNameTestClass));
            True(name == nameof(DefaultNameTestClass));
        }

        private class DefaultNameTestClass
        {
        }

        [Fact]
        public void Resolve_WithOneGenericParameter_AsGenericTypeDefinition_Works()
        {
            string? name = TypeNameResolver.Resolve(typeof(NameWithOneGenericParameterTestClass<>));
            True(name == "NameWithOneGenericParameterTestClass<T1>");
        }

        private class NameWithOneGenericParameterTestClass<T1>
        {
        }

        [Fact]
        public void Resolve_WithTwoGenericParameter_AsGenericTypeDefinition_Works()
        {
            string? name = TypeNameResolver.Resolve(typeof(NameWithTwoGenericParameterTestClass<,>));
            True(name == "NameWithTwoGenericParameterTestClass<T1, T2>");
        }

        private class NameWithTwoGenericParameterTestClass<T1, T2>
        {
        }

        [Fact]
        public void Resolve_Generic_OneGenericParameter_Works()
        {
            var type = typeof(MyClass).BaseType;
            string? name = TypeNameResolver.Resolve(type);
            True(name == "NameWithOneGenericParameterTestClass<MyClass>");
        }

        private class MyClass : NameWithOneGenericParameterTestClass<MyClass>
        {
        }

        [Fact]
        public void Resolve_Generic_OneGenericParameter_AndDeep1_Works()
        {
            var type = typeof(MyClassGenericDeep).BaseType;
            string? name = TypeNameResolver.Resolve(type);
            True(name == "NameWithOneGenericParameterTestClass<NameWithOneGenericParameterTestClass<MyClass>>");
        }

        private class
            MyClassGenericDeep : NameWithOneGenericParameterTestClass<NameWithOneGenericParameterTestClass<MyClass>>
        {
        }

        [Fact]
        public void Resolve_Generic_OneGenericParameter_AsArray_Works()
        {
            var type = typeof(MyClassGenericArray).BaseType;
            string? name = TypeNameResolver.Resolve(type);
            True(name == "NameWithOneGenericParameterTestClass<MyClass[]>");
        }

        private class MyClassGenericArray : NameWithOneGenericParameterTestClass<MyClass[]>
        {
        }

        [Fact]
        public void Resolve_Generic_TwoGenericParameter_AsArray_Works()
        {
            var type = typeof(MyClassTwoParameterAndGenericArray).BaseType;
            string? name = TypeNameResolver.Resolve(type);
            True(name ==
                 "NameWithTwoGenericParameterTestClass<MyClass[], NameWithOneGenericParameterTestClass<MyClass[]>[]>");
        }

        private class
            MyClassTwoParameterAndGenericArray : NameWithTwoGenericParameterTestClass<MyClass[],
            NameWithOneGenericParameterTestClass<MyClass[]>[]>

        {
        }

        [Fact]
        public void Resolve_Generic_Property_AsGenericTypeDefinition_Works()
        {
            var type = typeof(MyClassWithGenericProperty<,>).GetProperties()[0].PropertyType;
            string? name = TypeNameResolver.Resolve(type);
            True(name == "T1");

            type = typeof(MyClassWithGenericProperty<,>).GetProperties()[1].PropertyType;
            name = TypeNameResolver.Resolve(type);
            True(name == "T2");

            type = typeof(MyClassWithGenericProperty<,>).GetProperties()[2].PropertyType;
            name = TypeNameResolver.Resolve(type);
            True(name == "T1[]");

            type = typeof(MyClassWithGenericProperty<,>).GetProperties()[3].PropertyType;
            name = TypeNameResolver.Resolve(type);
            True(name == "T2[]");
        }

        private class MyClassWithGenericProperty<T1, T2>
        {
            public T1 GenericPropertyT1 { get; set; }
            public T2 GenericPropertyT2 { get; set; }
            public T1[] GenericPropertyT1Array { get; set; }
            public T2[] GenericPropertyT2Array { get; set; }
        }


        [Fact]
        public void Resolve_Generic_Property__Works()
        {
            var type = typeof(MyClassWithProperties).GetProperties()[0].PropertyType;
            string? name = TypeNameResolver.Resolve(type);
            True(name == "MyClass[]");
        }

        private class MyClassWithProperties
        {
            public IEnumerable<MyClass> GenericProperty1 { get; set; }
        }
    }
}
