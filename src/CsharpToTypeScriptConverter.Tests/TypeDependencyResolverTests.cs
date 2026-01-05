using System.Net.Sockets;
using TypeScriptRequestCommandsGenerator.Tools;
using TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes;
using static Xunit.Assert;

namespace CsharpToTypeScriptConverter.Tests
{
    public class TypeDependencyResolverTests
    {
        [Fact]
        public void Resolve_Works()
        {
            var resolver = new TypeDependencyResolver();
            var result = resolver.GetAllDependencies(typeof(MyDerivedTestClass));
            NotNull(result);
            Contains(result, x => x.Value == typeof(MyGenericType));
            Contains(result, x => x.Value == typeof(MyBaseClass<>));
            Contains(result, x => x.Value == typeof(MyDerivedTestClass));
            Contains(result, x => x.Value == typeof(Person));
            Contains(result, x => x.Value == typeof(ICustomer));
            True(result.Count == 5);
        }

        private class MyDerivedTestClass : MyBaseClass<MyGenericType>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Person Owner { get; set; }
        }

        private class MyBaseClass<T> : object
        {
            public T Data { get; set; }
        }

        private class MyGenericType
        {
        }

        private class Person : ICustomer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public object Value { get; set; }
        }

        private interface ICustomer
        {
        }
    }
}
