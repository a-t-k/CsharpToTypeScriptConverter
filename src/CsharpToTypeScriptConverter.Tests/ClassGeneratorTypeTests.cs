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

        [Fact]
        public void Generate_Command_Works()
        {
            var meta = ClassGeneratorType.Get(typeof(SaveStrukturelementenOrderInOrgaRequestCommand), null);
            NotNull(meta);
            Contains(meta.Members, x => x.PropertyTypeName == "OrderedItem<number>[]");
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

        private class SaveStrukturelementenOrderInOrgaRequestCommand : IRequestCommand, ICommand<ApiResponse<bool>>
        {
            public long ParentStrukturElementId { get; }
            public OrderedItem<long>[] StrukturelementenOrder { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public SaveStrukturelementenOrderInOrgaRequestCommand(long parentStrukturElementId,
                OrderedItem<long>[] strukturelementenOrder)
            {
                this.ParentStrukturElementId = parentStrukturElementId;
                this.StrukturelementenOrder = strukturelementenOrder;
            }
        }

        private class ApiResponse<T>
        {
        }

        private class OrderedItem<T>
        {
            public int Order { get; set; }
            public T Item { get; set; }
        }
    }
}
