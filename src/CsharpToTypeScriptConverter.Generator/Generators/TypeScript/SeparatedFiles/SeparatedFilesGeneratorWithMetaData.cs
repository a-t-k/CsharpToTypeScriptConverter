using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.CodeGenerationWarning;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.CommandInterface;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.Commands;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.ComplexTypes;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.Enumerations;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.TypeScriptImports;
using TypeScriptRequestCommandsGenerator.Tools;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class SeparatedFilesGeneratorWithMetaData(GeneratorType[] metadata, TypeFileGenerator typeFileGenerator)
    {
        public SeparatedFilesGeneratorWithRenderedTypes Generate(List<Type> ignoredCustomerTypes = null)
        {
            // generate generation text warning header
            string warningHeaderGenerated = new CodeGenerationWarning().TransformText();

            // generate interface for commands
            string generatedTypeScriptCommandInterface = new CommandInterface().TransformText().Trim();
            var commandInterfaceFileToSave = new FileMetadata
            {
                TransformedText = generatedTypeScriptCommandInterface,
                FilePath = Path.Combine(""),
                FileName = $"{CommandInterface.Settings.RequestCommandInterfaceName}.ts",
                Type = null
            };

            var enumTypesGenerated = metadata.Where(x => x.Kind == GeneratorTypeKind.Enum).Select(e =>
            {
                string enumGenerated = new EnumTypeScriptGenerator { TypeToGenerate = e }.TransformText().Trim();
                return new FileMetadata
                {
                    TransformedText = warningHeaderGenerated + enumGenerated,
                    FilePath = Path.Combine(typeFileGenerator.NamespacePath(e.Type)),
                    FileName = $"{e.Name}.ts",
                    Type = e.Type
                };
            }).ToArray();

            var generatedCommands = metadata.Where(x => x.Kind == GeneratorTypeKind.Class).Select(c =>
            {
                string commandGenerated = new CommandTypeScriptGenerator { TypeToGenerate = c }.TransformText().Trim();
                var dependencies = new TypeDependencyResolver(ignoredCustomerTypes).GetDependencies(c.Type, false);

                string fileName = $"{typeFileGenerator.GetFileNameFromType(c.Type)}.ts";
                var result = dependencies.Select(d => new TypeScriptImportDependency
                {
                    Name = typeFileGenerator.GetFileNameFromType(d.Type),
                    Path = typeFileGenerator.GenerateRelativePath(c.Type.Namespace)
                           + typeFileGenerator.NamespacePath(d.Type)
                           + "/"
                           + $"{typeFileGenerator.GetFileNameFromType(d.Type)}.ts"
                }).ToList();
                result.Add(new TypeScriptImportDependency
                {
                    Name = CommandInterface.Settings.RequestCommandInterfaceName,
                    Path = "./" + commandInterfaceFileToSave.FileName
                });

                string importsGenerated = new TypeScriptImports { Dependencies = result.ToList() }.TransformText();
                return new FileMetadata
                {
                    TransformedText = warningHeaderGenerated + importsGenerated + commandGenerated,
                    FilePath = Path.Combine(typeFileGenerator.NamespacePath(c.Type)),
                    FileName = fileName,
                    Type = c.Type
                };
            }).ToArray();

            var generatedUsedTypes = metadata.Where(x => x.Kind == GeneratorTypeKind.UsedReturnType).Select(c =>
            {
                string commandGenerated = new ComplexTypeScriptGenerator { TypeToGenerate = c }.TransformText().Trim();
                var dependencies = new TypeDependencyResolver(ignoredCustomerTypes).GetDependencies(c.Type, false);
                var fileGenerator = new TypeFileGenerator();
                string fileName = $"{fileGenerator.GetFileNameFromType(c.Type)}.ts";
                var result = dependencies.Select(d => new TypeScriptImportDependency
                {
                    Name = fileGenerator.GetFileNameFromType(d.Type),
                    Path = fileGenerator.GenerateRelativePath(c.Type.Namespace)
                           + fileGenerator.NamespacePath(d.Type)
                           + "/"
                           + $"{fileGenerator.GetFileNameFromType(d.Type)}.ts"
                });

                string importsGenerated = new TypeScriptImports { Dependencies = result.ToList() }.TransformText();
                return new FileMetadata
                {
                    TransformedText = warningHeaderGenerated + importsGenerated + commandGenerated,
                    FilePath = Path.Combine(fileGenerator.NamespacePath(c.Type)),
                    FileName = fileName,
                    Type = c.Type
                };
            }).ToArray();

            var allGeneration = new List<FileMetadata> { commandInterfaceFileToSave };
            allGeneration.AddRange(enumTypesGenerated);
            allGeneration.AddRange(generatedCommands);
            allGeneration.AddRange(generatedUsedTypes);
            return new SeparatedFilesGeneratorWithRenderedTypes(allGeneration);
        }
    }
}
