using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes;

namespace TypeScriptRequestCommandsGenerator.Tools
{
    public static class MetadataHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="interfaceFilterType">
        /// All Types will be filtered for this type to get only this types for generating.
        /// </param>
        /// <param name="explicitCommandInterfaceToFilter">
        /// when resolves interface names, only interfaces will be resolved from this type.
        /// say the type implements 2 interfaces, ICommand and IRequestCommand. Param is set to "ICommand"
        /// so only "ICommand" interface will be return. When nothing is set, all interfaces will be return.
        /// </param>
        /// <param name="requestCommandInterfaceName">
        /// if param is set and param "explicitCommandInterfaceToFilter" is set too, the name will be replaced
        /// with this value.
        /// Say we have "explicitCommandInterfaceToFilter" set to ICommand, and this param is set to "MyCommand"
        /// so result will be "MyCommand". So can ICommand changed in one Name for TypeScript. Generic will stay.
        /// </param>
        /// <returns></returns>
        public static List<GeneratorType> GetMetadataForCommands(IEnumerable<Type> types, Type interfaceFilterType,
            Type explicitCommandInterfaceToFilter, string requestCommandInterfaceName)
        {
            var typesToGenerate = types
                .Where(t => !t.IsAbstract)
                .Where(t => t.GetInterfaces().Contains(interfaceFilterType) && t.IsClass);
            var generatorTypes = typesToGenerate.Select(t =>
                {
                    var commandMetaData = ClassGeneratorType.GetCommand(t, explicitCommandInterfaceToFilter,
                        requestCommandInterfaceName);
                    return commandMetaData;
                }
            );
            return generatorTypes.ToList();
        }

        public static List<GeneratorType> GetGeneratorTypesMetadata(IEnumerable<Type> types,
            Type returnTypeFilter)
        {
            var typesToGenerate = types
                .Where(t => (t.IsClass && !t.IsAbstract) || t.IsEnum || t.IsInterface);

            var generatorTypes = typesToGenerate
                .Select(t =>
                {
                    if (t.IsEnum)
                    {
                        var generatorType = EnumGeneratorType.Get(t);
                        return generatorType;
                    }

                    if (t.IsInterface)
                    {
                        var interfaceGeneratorType = InterfaceGeneratorType.Get(t, returnTypeFilter);
                        return interfaceGeneratorType;
                    }

                    var classGeneratorType = ClassGeneratorType.Get(t, returnTypeFilter);
                    return classGeneratorType;
                })
                .ToList();

            return generatorTypes;
        }
    }
}
