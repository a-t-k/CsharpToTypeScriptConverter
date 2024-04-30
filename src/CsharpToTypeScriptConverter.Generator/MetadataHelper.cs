﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Templates;
namespace TypeScriptRequestCommandsGenerator;
public static class MetadataHelper
{
    public static void SetRequestCommandsInterfaceName(string requestCommandsInterfaceName)
    {
        TypesScriptGenerator.Settings.RequestCommandInterfaceName = requestCommandsInterfaceName;
    }
    
    public static List<GeneratorType> GetGeneratorTypesMetadata(IEnumerable<Type> types, Type filter, Type returnTypeFilter, Dictionary<string, Type> usedTypes)
    {
        var generatorTypes = types
            .Where(t => !t.IsAbstract)
            .Where(t => t.GetInterfaces().Contains(filter) && t.IsClass || t.IsEnum)
            .Select(t =>
            {
                if (t.IsEnum)
                {
                    return GetEnumGeneratorType(t);
                }

                if (t.IsValueType)
                {
                    return GetInterfaceGeneratorType(t, usedTypes, returnTypeFilter);
                }

                return GetClassGeneratorType(t, usedTypes, returnTypeFilter);
            })
            .ToList();

        return generatorTypes;
    }

    public static List<GeneratorType> GetGeneratorTypesForUsedTypes(Dictionary<string, Type> usedTypes)
    {
        var returnGeneratorTypes = usedTypes.Select(
            x => new GeneratorType
            {
                Kind = GeneratorTypeKind.UsedReturnType,
                Name = x.Key,
                Members = x.Value.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(
                        p =>
                        {
                            var generatorMember = new GeneratorMember
                            {
                                Name = p.Name,
                                Type = p.PropertyType,
                                IsDeclaredAsGeneric = p.DeclaringType is not null && p.DeclaringType.IsGenericType && p.DeclaringType.GetGenericTypeDefinition().GetProperty(p.Name)!.PropertyType.IsGenericParameter,
                                GenericName = "T"
                            };

                            // IEnumerable<>, IList<>, List<>.... List<T> ist then T[].
                            if (p.PropertyType.IsIEnumerableOfT() && p.PropertyType.IsGenericType && p.PropertyType.GetGenericArguments().Length > 0)
                            {
                                var checkType = p.PropertyType.GetGenericArguments()[0];

                                // Only when Class (declaringType) is generic?
                                if (p.DeclaringType is not null && p.DeclaringType.GenericTypeArguments.Any(a => a.Name == checkType.Name))
                                {
                                    generatorMember.IsDeclaredAsGeneric = true;
                                    generatorMember.GenericName = "T[]";
                                }
                            }
                            return generatorMember;
                        })
            });
        return returnGeneratorTypes.ToList();
    }

    private static GeneratorType GetClassGeneratorType(Type type, Dictionary<string, Type> returnTypes, Type returnTypeFilter)
    {
        return new GeneratorType
        {
            Name = type.Name,
            Kind = GeneratorTypeKind.Class,
            Members = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    var generatorMember = new GeneratorMember
                    {
                        Name = p.Name,
                        Type = p.PropertyType,
                        IsDeclaredAsGeneric = p.DeclaringType is not null && p.DeclaringType.IsGenericType && p.DeclaringType.GetGenericTypeDefinition().GetProperty(p.Name)!.PropertyType.IsGenericParameter
                    };

                    var propertyTypeName = TypesScriptGenerator.GetTypeScriptFieldTypeName(p.PropertyType, returnTypes, itIsForReturnTypeName: false, itIsForPropertyName: true);
                    generatorMember.IsDeclaredAsGeneric = true;
                    generatorMember.GenericName = propertyTypeName;
                    return generatorMember;
                }).ToList(),
            TypeNameForJsonDeserialization = GetTypeForJsonDeserialization(type),
            ReturnTypeName = GetReturnTypeName(type, returnTypes, returnTypeFilter)
        };
    }

    private static GeneratorType GetInterfaceGeneratorType(Type type, Dictionary<string, Type> returnTypes, Type returnTypeFilter)
    {
        var generatorType = GetClassGeneratorType(type, returnTypes, returnTypeFilter);
        generatorType.Kind = GeneratorTypeKind.Interface;
        return generatorType;
    }

    private static GeneratorType GetEnumGeneratorType(Type type)
    {
        return new GeneratorType
        {
            Name = type.Name,
            Kind = GeneratorTypeKind.Enum,
            Members = type.GetFields()
                .Where(f => f.Name != "value__")
                .Select(
                    f => new GeneratorMember
                    {
                        Name = f.Name,
                        Type = f.FieldType
                    })
        };
    }

    private static string GetReturnTypeName(Type t, Dictionary<string, Type> returnTypesStorage, Type returnTypeFilter)
    {
        var genericArguments = t
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == returnTypeFilter)
            .SelectMany(i => i.GetGenericArguments());

        var returnTypeName = genericArguments
            .Select(t => TypesScriptGenerator.GetTypeScriptFieldTypeName(t, returnTypesStorage, itIsForReturnTypeName: true, itIsForPropertyName: false))
            .FirstOrDefault();
        return returnTypeName;
    }

    private static string GetTypeForJsonDeserialization(Type type)
    {
        var result = $"{type.Namespace}.{type.Name}, {type.Assembly.GetName().Name}";
        return result;
    }

    public static bool IsIEnumerableOfT(this Type type)
    {
        return type
            .GetInterfaces()
            .Append(type)
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        if (type == interfaceType) return true;
        return type.GetInterfaces().Contains(interfaceType);
    }
}