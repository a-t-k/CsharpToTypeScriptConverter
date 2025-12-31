using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeScriptRequestCommandsGenerator.Models;

namespace TypeScriptRequestCommandsGenerator.Tools.GeneratorTypes
{
    public static class ClassGeneratorType
    {
        public static GeneratorType Get(Type type, Type returnTypeFilter)
        {
            string name = TypeNameResolver.Resolve(type);
            var generatorType = new GeneratorType
            {
                Name = name,
                Type = type,
                Kind = GeneratorTypeKind.Class,
                ImplementsInterfaceTypeNames = GetInterfaceNames(type),
                BaseTypeName = GetBaseName(type),
                Members = GetMember(type),
                TypeNameForJsonDeserialization = GetTypeForJsonDeserialization(type),
                ReturnTypeName = name,
                Documentation = type.GetDocumentation()?.OnlyDocumentationText()
            };
            return generatorType;
        }

        public static GeneratorType GetCommand(Type type, Type implementOnlyThisInterfaceWhenExists = null,
            string replaceInterfaceNameWithThisOne = null)
        {
            string name = TypeNameResolver.Resolve(type);
            var generatorType = new GeneratorType
            {
                Name = name,
                Type = type,
                Kind = GeneratorTypeKind.CommandClass,
                ImplementsInterfaceTypeNames =
                    GetInterfaceNames(type, implementOnlyThisInterfaceWhenExists, replaceInterfaceNameWithThisOne),
                BaseTypeName = GetBaseName(type),
                Members = GetMember(type),
                TypeNameForJsonDeserialization = GetTypeForJsonDeserialization(type),
                ReturnTypeName = name,
                Documentation = type.GetDocumentation()?.OnlyDocumentationText()
            };

            return generatorType;
        }

        private static string[] GetInterfaceNames(Type type, Type implementOnlyThisInterfaceWhenExists = null,
            string replaceInterfaceNameWithThisName = null)
        {
            if (implementOnlyThisInterfaceWhenExists is not null)
            {
                var explicitType = type.GetInterfaces().FirstOrDefault(i =>
                {
                    if (i.IsGenericType)
                    {
                        return i.GetGenericTypeDefinition() == implementOnlyThisInterfaceWhenExists;
                    }

                    return i == implementOnlyThisInterfaceWhenExists;
                });

                if (explicitType is not null)
                {
                    string genericNameResult =
                        TypeNameResolver.Resolve(explicitType, false, replaceInterfaceNameWithThisName);
                    return [genericNameResult];
                }
            }

            return type.GetInterfaces().Select(i =>
            {
                if (!i.IsGenericType)
                {
                    return i.Name;
                }

                // name is generic. we get generic types 
                string interfacePrefixName =
                    $"{i.Name.Substring(0, i.Name.IndexOf("`", StringComparison.Ordinal))}";
                string genericNameResult = TypeNameResolver.Resolve(type);
                string genericInterfaceName = $"{interfacePrefixName}<{genericNameResult}>";
                return genericInterfaceName;
            }).ToArray();
        }

        private static string GetBaseName(Type type)
        {
            string baseName = string.Empty;
            var baseType = type.BaseType;
            if (baseType != null && baseType != typeof(object))
            {
                baseName = TypeNameResolver.Resolve(baseType);
            }

            return baseName;
        }

        private static List<GeneratorMember> GetMember(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    bool isDeclaredAsGeneric = p.DeclaringType is not null && p.DeclaringType.IsGenericType &&
                                               p.DeclaringType.GetGenericTypeDefinition().GetProperty(p.Name)!
                                                   .PropertyType.IsGenericParameter;
                    var generatorMember = new GeneratorMember
                    {
                        Name = p.Name, Type = p.PropertyType, IsDeclaredAsGeneric = isDeclaredAsGeneric
                    };

                    string propertyGenericTypeName =
                        TypeNameResolver.Resolve(p.PropertyType);
                    generatorMember.GenericName = propertyGenericTypeName;
                    return generatorMember;
                }).ToList();


        private static string GetTypeForJsonDeserialization(Type type)
        {
            string result = $"{type.Namespace}.{type.Name}, {type.Assembly.GetName().Name}";
            return result;
        }
    }
}
