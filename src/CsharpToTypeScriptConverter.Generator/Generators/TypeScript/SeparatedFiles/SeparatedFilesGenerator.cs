using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Templates.SeparatedFiles.CommandInterface;
using TypeScriptRequestCommandsGenerator.Tools;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class SeparatedFilesGenerator
    {
        private readonly Dictionary<string, Type> usedTypes = new();
        private readonly List<Type> typesToGenerate = [];
        private readonly TypeFileGenerator typeFileGenerator;

        private Type interfaceFilterType;
        private Type returnTypeOfCommands;

        public SeparatedFilesGenerator()
        {
            this.typeFileGenerator = new TypeFileGenerator();
        }

        /// <summary>
        /// Set interface name for commands in a target language.
        /// Alle generated commands will implement it.
        /// </summary>
        /// <param name="requestCommandInterfaceName"></param>
        /// <returns></returns>
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
        /// <param name="interfaceFilterType"></param>
        /// <returns></returns>
        public SeparatedFilesGenerator SetInterfaceFilter(Type interfaceFilterType)
        {
            this.interfaceFilterType = interfaceFilterType;
            return this;
        }

        /// <summary>
        /// Command has a return type, so it will be set here.
        /// </summary>
        /// <param name="returnTypeOfCommands"></param>
        /// <returns></returns>
        public SeparatedFilesGenerator SetReturnTypeOfCommands(Type returnTypeOfCommands)
        {
            this.returnTypeOfCommands = returnTypeOfCommands;
            return this;
        }

        /// <summary>
        /// Add Types to collection that will be generated.
        /// </summary>
        /// <param name="typesToGenerate"></param>
        /// <returns></returns>
        public SeparatedFilesGenerator AddRangeOfTypesToGenerate(IEnumerable<Type> typesToGenerate)
        {
            this.typesToGenerate.AddRange(typesToGenerate);
            return this;
        }

        public SeparatedFilesGeneratorWithMetaData GenerateMetadata()
        {
            var typesMetadata = MetadataHelper.GetGeneratorTypesMetadata(this.typesToGenerate, this.interfaceFilterType,
                this.returnTypeOfCommands, this.usedTypes);
            // types that generator not generated, because they are deeper in definition
            var notGeneratedTypes = this.usedTypes.Where(ut => typesMetadata.ToArray().All(tm => tm.Name != ut.Key))
                .ToDictionary();
            var newMetaData = MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes);

            // generate it
            typesMetadata.AddRange(newMetaData);

            // distinct all generated types
            var distinctMetadata = typesMetadata.GroupBy(x => x.Type).Select(x => x.First()).ToArray();
            return new SeparatedFilesGeneratorWithMetaData(distinctMetadata, this.typeFileGenerator);
        }
    }
}
