using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.CommandInterface;
using TypeScriptRequestCommandsGenerator.Tools;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.OneFile
{
    public class OneFileGenerator
    {
        private Type interfaceFilterType;
        private Type returnTypeOfCommands;

        private readonly List<Type> commandTypesToGenerate = [];
        private readonly List<Type> extraTypesToGenerate = [];

        /// <summary>
        /// Set interface name for commands in a target language.
        /// Alle generated commands will implement it.
        /// </summary>
        /// <param name="requestCommandInterfaceName"></param>
        /// <returns></returns>
        public OneFileGenerator SetRequestCommandInterfaceNameForGeneratedCommands(string requestCommandInterfaceName)
        {
            CommandInterface.Settings.RequestCommandInterfaceName = requestCommandInterfaceName?.Trim();
            return this;
        }

        /// <summary>
        /// Set filter of interface.
        /// When interface filter is set. All types will be filtered by interface filter
        /// to get only this types with this interface for generation.
        /// </summary>
        /// <returns></returns>
        public OneFileGenerator SetInterfaceFilter(Type interfaceFilterTypeParameter)
        {
            this.interfaceFilterType = interfaceFilterTypeParameter;
            return this;
        }

        /// <summary>
        /// Command has a return type, so it will be set here.
        /// </summary>
        public OneFileGenerator SetReturnTypeOfCommands(Type returnTypeOfCommandsParameter)
        {
            this.returnTypeOfCommands = returnTypeOfCommandsParameter;
            return this;
        }

        /// <summary>
        /// Add command types to collection that will be generated.
        /// </summary>
        public OneFileGenerator AddRangeOfCommandTypesToGenerate(
            IEnumerable<Type> commandTypesToGenerateParameter)
        {
            this.commandTypesToGenerate.AddRange(commandTypesToGenerateParameter);
            return this;
        }

        /// <summary>
        /// Add extra types to collection that will be generated.
        /// </summary>
        public OneFileGenerator AddRangeOfExtraTypesToGenerate(IEnumerable<Type> extraTypesToGenerateParameter)
        {
            this.extraTypesToGenerate.AddRange(extraTypesToGenerateParameter);
            return this;
        }

        public OneFileGeneratorWithMetaData GenerateMetadata()
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

            // get metadata for all another types
            var extraTypesMetadata = MetadataHelper.GetGeneratorTypesMetadata(this.extraTypesToGenerate,
                null);

            typesMetadata.AddRange(generatedCommands);
            typesMetadata.AddRange(extraTypesMetadata);

            // distinct all generated types
            var distinctMetadata = typesMetadata.GroupBy(x => x.Type).Select(x => x.First()).ToArray();
            return new OneFileGeneratorWithMetaData(distinctMetadata);
        }
    }
}
