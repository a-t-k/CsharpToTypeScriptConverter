using TypeScriptRequestCommandsGenerator.Tools;
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

        [Fact]
        public void Resolve_WhenOnePropertyIsArray_Works()
        {
            var resolver = new TypeDependencyResolver();
            var result = resolver.GetDependencies(typeof(LandingPageFragenCmsBlock));
            NotNull(result);
            True(result.Count == 4);
        }

        [Fact]
        public void Resolve_WhenIncludeSelfIsFalse_Works()
        {
            var resolver = new TypeDependencyResolver();
            var resultWithInclude = resolver.GetDependencies(typeof(IncludeSelf));
            var resultWithoutInclude = resolver.GetDependencies(typeof(IncludeSelf), false);

            NotNull(resultWithInclude);
            NotNull(resultWithoutInclude);
            True(resultWithInclude.Count == 1);
            True(resultWithoutInclude.Count == 0);
        }

        [Fact]
        public void Resolve_WhenPropertyIsNullable_Works()
        {
            var resolver = new TypeDependencyResolver();
            var result = resolver.GetDependencies(typeof(ClassWithNullableProperty));

            NotNull(result);
            True(result.Count == 2);
        }

        private class ClassWithNullableProperty
        {
            public int? IamNullable { get; set; }
            public int IamNotNullable { get; set; }
            public ClassWithNullableProperty? Self { get; set; }
            public Person? SelfPeerson { get; set; }
        }

        private class MyDerivedTestClass : MyBaseClass<MyGenericType>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Person Owner { get; set; }
            public Guid TypeId { get; set; }
            public DateTime CreatedAt { get; set; }
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

        private class LandingPageFragenCmsBlock : ICmsBlock
        {
            public CmsBlockContentTypes ContentType => CmsBlockContentTypes.LandingpageFragen;
            public string? Title { get; set; }
            public string? Frage { get; set; }
            public CmsButton[] Buttons { get; set; } = [];
            public string? BackgroundColor { get; set; }
            public ICmsBlock[] Faqs { get; set; } = [];
        }

        private interface ICmsBlock
        {
            public CmsBlockContentTypes ContentType { get; }
        }

        private enum CmsBlockContentTypes
        {
            Unknown = 0,
            Image = 1,
            Vector = 2,
            Video = 3,
            Button = 4,
            Article = 5,
            Vorteil = 6,
            Infobox = 7,
            Store = 8,
            LandingpageHeader = 9,
            LandingpageHero = 10,
            LandingpageCarousel = 11,
            LandingpageVorteile = 12,
            LandingpageStatus = 13,
            LandingpageMobile = 14,
            LandingpageFragen = 15,
            Faq = 16,
            LandingpageKontakt = 17,
            LandingpageFooter = 18
        }

        private class CmsButton
        {
            public string? Title { get; set; }
            public string? ButtonType { get; set; }
            public string? Text { get; set; }
            public string? Icon { get; set; }
            public CmsUrl? Url { get; set; }
        }

        private class CmsUrl
        {
            public string? Url { get; set; }
            public string? Name { get; set; }
            public string? Target { get; set; }
        }

        private class IncludeSelf
        {
            public int Id { get; set; }
            public IncludeSelf Self { get; set; }
        }
    }
}
