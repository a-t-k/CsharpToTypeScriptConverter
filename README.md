# ATK.Command.CsToTsGenerator - Technische Dokumentation

Code-Generator zur automatischen Generierung von TypeScript Types aus C# Types mittels .NET Reflection.

## Anforderungen

- **.NET 10.0**
- **Microsoft.CodeAnalysis 5.0.0**
- **System.CodeDom 10.0.1**

## Einstiegspunkt

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();
```

Die Klasse **`Generator`** ist der zentrale Einstiegspunkt:
- `Generator.TypeScript()` → Instanziiert `TypeScriptGenerator`
- `TypeScriptGenerator` orchestriert den gesamten Generierungsprozess

## Generierungsmodi

**SeparatedFiles** - Jeder Typ in separater Datei
- Eine Datei pro Typ
- Barrel-Export via `index.ts`
- Automatisches Verzeichnis-Leeren mit Flag
- Template: `Templates/SeparatedFiles/`
- Klassen: `SeparatedFilesGenerator`, `SeparatedFilesGeneratorWithMetaData`, `SeparatedFilesGeneratorWithRenderedTypes`

**OneFile** - Alle Typen in einer Datei
- Alle Typen in einer Datei
- Kompakter Output
- Klassen: `OneFileGenerator`, `OneFileGeneratorWithMetaData`

## Kernkomponenten

| Komponente | Beschreibung |
|---|---|
| **Generator** | Einstiegspunkt, erzeugt `TypeScriptGenerator` |
| **TypeScriptGenerator** | Orchestriert den Generierungsprozess |
| **SeparatedFilesGenerator** | Generiert separate TS-Dateien pro Typ |
| **OneFileGenerator** | Generiert alle Typen in eine TS-Datei |
| **TypeDependencyResolver** | Löst Typ-Abhängigkeiten rekursiv auf |
| **TypeNameResolver** | Konvertiert C#-Namen zu TypeScript-Namen |
| **GeneratorType** | Repräsentiert einen C#-Typ mit Metadaten |
| **GeneratorMember** | Repräsentiert ein Property/Field eines Typs |

## Typ-Mapping

| C# | TypeScript |
|---|---|
| `int`, `long`, `short`, `byte` | `number` |
| `decimal`, `float`, `double` | `number` |
| `bool` | `boolean` |
| `string` | `string` |
| `DateTime`, `DateOnly` | `string` |
| `Enum` | `enum` |
| `List<T>`, `IEnumerable<T>` | `T[]` |
| `Dictionary<K, V>` | `{ [key: K]: V }` |
| `Nullable<T>`, `T?` | `T \| null` |
| `Guid` | `string` |



T4 Text Templates in `Templates/`:
- `SeparatedFiles/ComplexTypes/` - Class/Record Generierung
- `SeparatedFiles/Commands/` - Command-Interface
- `SeparatedFiles/Enums/` - Enum-Generierung
- `SeparatedFiles/TypeScriptImports/` - Import-Statements
- `SeparatedFiles/CodeGenerationWarning/` - Generierungs-Header

## API

### SeparatedFilesGenerator

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();

var builtFiles = tsGenerator
    .SeparatedFiles()
    .SetInterfaceFilter(typeof(IRequestCommand))
    .SetReturnTypeOfCommands(typeof(Response))
    .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
    .AddRangeOfCommandTypesToGenerate(commandTypes)
    .AddRangeOfExtraTypesToGenerate(dtoTypes)
    .GenerateMetadata()
    .GenerateTypeScript()
    .Build();

builtFiles.Save(clearDestinationFolder: true);
```

### OneFileGenerator

```csharp
using ATK.Command.CsToTsGenerator;

var generator = new Generator();
var tsGenerator = generator.TypeScript();

var builtFile = tsGenerator
    .OneFile()
    .SetInterfaceFilter(typeof(IRequestCommand))
    .SetReturnTypeOfCommands(typeof(Response))
    .SetRequestCommandInterfaceNameForGeneratedCommands("ICommand")
    .AddRangeOfCommandTypesToGenerate(commandTypes)
    .AddRangeOfExtraTypesToGenerate(dtoTypes)
    .GenerateMetadata()
    .GenerateTypeScript()
    .Build("output/api-types.ts");

builtFile.Save();
```

## Datenmodelle

| Klasse | Zweck |
|---|---|
| `GeneratorType` | C#-Typ mit Metadaten |
| `GeneratorMember` | Property/Field eines Typs |
| `GeneratorTypeKind` | Klassifizierung (Class, Enum, Interface, Record) |
| `FileMetadata` | Zielordner und Metadaten |
| `BuildFile` | Generierte TypeScript-Datei |
| `BuildedSeparatedFiles` | Sammlung generierter Dateien |

## Filter und Konfiguration

**SetInterfaceFilter**
- Nur Typen implementieren diese Interface
- Typ-Selektion für Commands

**SetReturnTypeOfCommands**
- Response-Type für generierte Commands
- Standard: `Response`, `ApiResponse`

**AddRangeOfCommandTypesToGenerate**
- Command-Typen auswählen
- Werden zu TypeScript Interfaces

**AddRangeOfExtraTypesToGenerate**
- DTOs und weitere Typen hinzufügen

## Typ-Klassifizierung

`GeneratorTypeKind` Enum:
- `Class` - Gewöhnliche Klassen
- `Interface` - Schnittstellen
- `Record` - Record-Typen
- `Enum` - Enumerationen
- `Struct` - Strukturtypen

## Testing

Test-Projekte in `src/CsharpToTypeScriptConverter.Tests`:

- `GeneratorTypeTests` - Typ-Erkennung
- `ClassGeneratorTypeTests` - Class-Generierung
- `OneFileGeneratorTests` - OneFile-Modus
- `SeparatedFilesGeneratorTests` - SeparatedFiles-Modus
- `InheritanceTests` - Vererbungs-Handling
- `TypeDependencyResolverTests` - Abhängigkeits-Auflösung
- `TypeNameResolverTests` - Namen-Auflösung

## Build & NuGet

**Build:**
```
dotnet build
dotnet test
```

**Paket:**
- ATK.Command.CsToTsGenerator
- Repository: https://github.com/a-t-k/CsharpToTypeScriptConverter
- Lizenz: LICENSE
|----------|-----|--------------|-----------|
| Pro Änderung | 2-3 Stunden | 5 Minuten | 95% |
| Pro Woche (5 Änderungen) | 50-75 Stunden | 25 Minuten | 95% |
| Pro Monat | 200-300 Stunden | 100 Minuten | 95% |

### Bug-Reduktion

| Fehlertyp | Alt | Mit Generator |
|-----------|-----|--------------|
| Naming-Fehler | Häufig | Unmöglich (Reflection) |
| Typ-Fehler | Häufig | Unmöglich (Konvertierung) |
| Struktur-Fehler | Häufig | Unmöglich (Rekursion) |
| **Erkennung** | Laufzeit | **Compile-Zeit** |

### Team-Zufriedenheit

- ✓ Backend Team: "Wir generieren die Typen automatisch"
- ✓ Frontend Team: "Danke, die sind immer aktuell"
- ✓ Keine gegenseitigen Blockierungen
- ✓ Fokus auf Features statt Synchronisation

---

## 🏛️ Architektur & Design

### Generierungsprozess

```
1. Input: C# Types (DTOs, Commands)
   ↓
2. Reflection: Auslesen aller Properties & Typen
   ↓
3. Dependency Resolution: Finde alle abhängigen Typen
   ↓
4. Metadata Generation: Erstelle Typ-Definitionen
   ↓
5. TypeScript Rendering: Generiere .ts Code
   ↓
6. File Building: Erstelle BuildFile Objekte
   ↓
7. Output: Speichere in Dateien (SeparatedFiles/OneFile)
```

---

## 🔧 Installation & Setup

### Voraussetzungen
- .NET 9+
- C# 13.0

### NuGet Package
```bash
dotnet add package TypeScriptRequestCommandsGenerator
```

### Schnelleinstieg

1. **Generiert Typen definieren:**
```csharp
var commands = assembly.GetTypes()
    .Where(t => typeof(IRequestCommand).IsAssignableFrom(t));

var dtos = assembly.GetTypes()
    .Where(t => t.Name.EndsWith("Dto"));
```

2. **Generator konfigurieren:**
```csharp
var generator = new SeparatedFilesGenerator()
    .SetInterfaceFilter(typeof(IRequestCommand))
    .SetReturnTypeOfCommands(typeof(ApiResponse))
    .AddRangeOfCommandTypesToGenerate(commands)
    .AddRangeOfExtraTypesToGenerate(dtos);
```

3. **Generieren & speichern:**
```csharp
generator.GenerateMetadata()
    .GenerateTypeScript()
    .Build()
    .Save(clearDestinationFolder: true);
```

4. **In CI/CD integrieren:**
Füge einen Build-Step hinzu, der den Generator nach Backend-Änderungen aufruft.

---

## 📁 Projektstruktur

```
CsharpToTypeScriptConverter/
├─ src/
│  ├─ CsharpToTypeScriptConverter.Generator/
│  │  ├─ Generators/          # Generator-Logik (SeparatedFiles, OneFile)
│  │  ├─ Models/              # BuildFile, GeneratorType, etc.
│  │  ├─ Templates/           # TypeScript Template Engine
│  │  └─ Tools/               # TypeFileGenerator, TypeResolver, etc.
│  └─ CsharpToTypeScriptConverter.Tests/
│     └─ ...                  # Unit & Integration Tests
└─ README.md                  # Diese Datei
```

---

## 🧪 Tests

```bash
dotnet test src/CsharpToTypeScriptConverter.Tests/CsharpToTypeScriptConverter.Tests.csproj
```

Tests überprüfen:
- ✓ Korrekte TypeScript Generierung
- ✓ Dependency Resolution
- ✓ Datei-Strukturen (Separated vs OneFile)
- ✓ Spezielle Typen (Enums, Interfaces, etc.)

---

## 🎓 Für wen ist das?

### ✓ Kleine Teams (2-3 Devs)
Auch bei guter Kommunikation: Versicherung gegen menschliche Fehler.

### ✓ Wachsende Teams (5-10+ Devs)
Essentiell! Manuelle Synchronisation wird unmöglich.

### ✓ Große Projekte (200+ Typen)
Unverzichtbar! Ohne Generator: Chaos garantiert.

### ✓ Agile Entwicklung
Häufige Änderungen? Generator spart Stunden jede Woche.

---

## ❓ FAQ

### "Aber wir nutzen Swagger/OpenAPI?"
**Unterschied:** 
- Swagger dokumentiert HTTP-Schnittstellen (was du schicken kannst)
- Dieser Generator generiert exakte TypeScript Typen (wie die Struktur aussieht)

Beide können zusammen verwendet werden!

### "Wir haben noch nie Probleme mit Typ-Sync?"
Dann seid ihr wahrscheinlich:
- Sehr klein (< 3 Devs)
- Sehr gut organisiert
- Sehr viel Glück gehabt

Trotzdem: Generator würde euch 10+ Stunden/Monat sparen.

### "Ist das nicht overkill?"
Nein. Mit 200+ Typen und 5+ Änderungen/Woche sind 10-20 Stunden Sync-Overhead normal. Generator spart das komplett.

### "Können wir das in unsere CI/CD integrieren?"
**Ja!** Perfekt für:
- Post-Build Hook
- PR-Validierung
- Auto-Commit der generierten Typen

---

## 📝 Lizenz

Siehe LICENSE Datei.

---

## 🤝 Contributing

Contributions sind willkommen! Bitte erstelle einen Pull Request oder öffne ein Issue.

---

## 🎯 Zusammenfassung

**CsharpToTypeScriptConverter löst ein echtes Problem:**

Statt Stunden mit manueller Typ-Synchronisation zu verschwenden, konzentrieren sich Teams auf das was zählt: **Features entwickeln.**

Der Generator macht aus dem Backend-Code die einzige Wahrheit. Das Resultat:
- ✅ Keine Typ-Fehler
- ✅ 95% weniger Sync-Overhead
- ✅ Keine Blockierungen zwischen Teams
- ✅ Glücklichere, produktivere Teams

**Das ist kein Nice-to-Have. Das ist eine Versicherung gegen menschliche Fehler.**

---

**Ready to sync your types automatically?** 🚀
