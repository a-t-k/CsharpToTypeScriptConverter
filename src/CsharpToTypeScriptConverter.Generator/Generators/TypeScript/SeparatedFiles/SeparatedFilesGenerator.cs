using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.CommandInterface;
using TypeScriptRequestCommandsGenerator.Tools;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class SeparatedFilesGenerator
    {
        private readonly List<Type> commandTypesToGenerate = [];
        private readonly List<Type> extraTypesToGenerate = [];
        private readonly TypeFileGenerator typeFileGenerator;

        private Type interfaceFilterType;
        private Type returnTypeOfCommands;

        public SeparatedFilesGenerator() => this.typeFileGenerator = new TypeFileGenerator();

        /// <summary>
        /// Set interface name for commands in a target language.
        /// After setting this name, all generated commands uses this interface name.
        /// </summary>
        public SeparatedFilesGenerator SetRequestCommandInterfaceNameForGeneratedCommands(
            string requestCommandInterfaceName)
        {
            CommandInterface.Settings.RequestCommandInterfaceName = requestCommandInterfaceName?.Trim();
            return this;
        }

        /// <summary>
        /// Set filter of interface.
        /// When interface filter is set. All types will be filtered by interface filter
        /// to get only this types with this interface for generation.
        /// </summary>
        public SeparatedFilesGenerator SetInterfaceFilter(Type interfaceFilterTypeParameter)
        {
            this.interfaceFilterType = interfaceFilterTypeParameter;
            return this;
        }

        /// <summary>
        /// Command has a return type, so it will be set here.
        /// </summary>
        public SeparatedFilesGenerator SetReturnTypeOfCommands(Type returnTypeOfCommandsParameter)
        {
            this.returnTypeOfCommands = returnTypeOfCommandsParameter;
            return this;
        }

        /// <summary>
        /// Add command types to collection that will be generated.
        /// </summary>
        public SeparatedFilesGenerator AddRangeOfCommandTypesToGenerate(
            IEnumerable<Type> commandTypesToGenerateParameter)
        {
            this.commandTypesToGenerate.AddRange(commandTypesToGenerateParameter);
            return this;
        }

        /// <summary>
        /// Add extra types to collection that will be generated.
        /// </summary>
        public SeparatedFilesGenerator AddRangeOfExtraTypesToGenerate(IEnumerable<Type> extraTypesToGenerateParameter)
        {
            this.extraTypesToGenerate.AddRange(extraTypesToGenerateParameter);
            return this;
        }

        /// <summary>
        /// Create metadata for all types that will be generated.
        /// </summary>
        public SeparatedFilesGeneratorWithMetaData GenerateMetadata()
        {
            var typesMetadata = new List<GeneratorType>();
            var dependencyResolver = new TypeDependencyResolver([this.returnTypeOfCommands, this.interfaceFilterType]);
            var allDependencies =
                dependencyResolver.GetAllDependencies(this.commandTypesToGenerate.Concat(this.extraTypesToGenerate)
                    .ToArray());

            this.extraTypesToGenerate.AddRange(allDependencies.Select(x => x.Value));

            // get metadata for commands
            var generatedCommands = MetadataHelper.GetMetadataForCommands(this.commandTypesToGenerate,
                this.interfaceFilterType,
                this.returnTypeOfCommands,
                CommandInterface.Settings.RequestCommandInterfaceName);

            // get metadata for extra types
            var extraTypesMetadata = MetadataHelper.GetGeneratorTypesMetadata(this.extraTypesToGenerate,
                this.returnTypeOfCommands);

            typesMetadata.AddRange(generatedCommands);
            typesMetadata.AddRange(extraTypesMetadata);

            // distinct all generated types
            var distinctMetadata = typesMetadata.GroupBy(x => x.Type).Select(x => x.First()).ToArray();
            return new SeparatedFilesGeneratorWithMetaData(distinctMetadata, this.typeFileGenerator);
        }
    }
}
