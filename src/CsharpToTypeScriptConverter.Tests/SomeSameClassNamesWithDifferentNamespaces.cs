using TypeScriptRequestCommandsGenerator;
using TypeScriptRequestCommandsGenerator.Tools;

namespace CsharpToTypeScriptConverter.Tests
{
    public class SomeSameClassNamesWithDifferentNamespaces
    {
        [Fact]
        public void Resolve_Works()
        {
            var resolver = new TypeDependencyResolver();
            var result = resolver.GetAllDependencies(typeof(BenutzerkontoForBetrieb), typeof(MyNamespace.Benutzer));
            Assert.NotNull(result);
            Assert.True(result.Count == 4);
        }

        [Fact]
        public void Generation__Works()
        {
            var generated = new Generator()
                .TypeScript()
                .SeparatedFiles()
                .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
                .SetInterfaceFilter(typeof(IRequestCommand))
                .SetReturnTypeOfCommands(typeof(ICommand<>))
                .AddRangeOfExtraTypesToGenerate([typeof(MyNamespace.Benutzer), typeof(BenutzerkontoForBetrieb)])
                .GenerateMetadata();

            var result = generated
                .Generate([typeof(ICommand<>), typeof(IRequestCommand)])
                .Build("");

            Assert.NotNull(result);
            Assert.True(result.BuildFiles.Count == 9);
        }

        public class BenutzerkontoForBetrieb
        {
            public bool SiFa { get; set; }
            public DateTime? LetzteAnmeldungAm { get; set; }
            public DateTime ErstelltAm { get; set; }
            public bool Gesperrt { get; set; }
            public Benutzer Benutzer { get; set; }
            public int AnzahlDerZugewiesenerBetriebe { get; set; }
            public long BenutzerkontoId { get; set; }
            public bool AutomatischeEMails { get; set; }
            public List<StrukturbaumElementData> ZugewieseneStrukturbausteineData { get; set; }
        }

        public class StrukturbaumElementData
        {
            public long StrukturbaumZuweisungId { get; set; }
            public long StrukturbausteinId { get; set; }
            public long BetriebId { get; set; }
            public string Name { get; set; }
            public int Ebene { get; set; }
            public string Hinweis { get; set; }
        }

        public class Benutzer
        {
            public string Nachname { get; set; }
            public string Vorname { get; set; }
            public string Anmeldename { get; set; }
            public string Email { get; set; }
            public string Telefon { get; set; }
            public string Titel { get; set; }
            public string Plz { get; set; }
            public string Strasse { get; set; }
            public string Stadt { get; set; }
            public string Mobil { get; set; }
        }
    }
}

namespace MyNamespace
{
    public class Benutzer
    {
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string Anmeldename { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Titel { get; set; }
        public string Plz { get; set; }
        public string Strasse { get; set; }
        public string Stadt { get; set; }
        public string Mobil { get; set; }
    }
}
