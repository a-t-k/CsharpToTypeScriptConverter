using System;
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
                    var generatorType = GetEnumGeneratorType(t);
                    generatorType.Documentation = t.GetDocumentation()?.OnlyDocumentationText();
                    return generatorType;
                }

                if (t.IsValueType)
                {
                    var valueGeneratorType = GetInterfaceGeneratorType(t, usedTypes, returnTypeFilter);
                    valueGeneratorType.Documentation = t.GetDocumentation()?.OnlyDocumentationText();
                    return valueGeneratorType;
                }

                var classGeneratorType = GetClassGeneratorType(t, usedTypes, returnTypeFilter);
                classGeneratorType.Documentation = t.GetDocumentation()?.OnlyDocumentationText();
                return classGeneratorType;
            })
            .ToList();

        return generatorTypes;
    }

    public static List<GeneratorType> GetGeneratorTypesForUsedTypes(Dictionary<string, Type> usedTypes)
    {
        var foundTypes = new Dictionary<string, Type>();
        var returnGeneratorTypes = usedTypes.Select(
            x =>
            {
                if (x.Value.IsEnum)
                {
                    var generatorType = GetEnumGeneratorType(x.Value);
                    generatorType.Documentation = x.Value.GetDocumentation()?.OnlyDocumentationText();
                    return generatorType;
                }
                var resultGeneratorType = new GeneratorType
                {
                    Kind = GeneratorTypeKind.UsedReturnType,
                    Name = x.Key,
                    ImplementsInterfaceTypeNames = x.Value.GetInterfaces().Select(i =>
                    {
                        if (!i.IsGenericType) return i.Name;

                        // name is generic. we get generic types 
                        var interfacePrefixName = $"{i.Name.Substring(0, i.Name.IndexOf("`"))}";
                        var genericNameResult = GetReturnTypeName(x.Value, usedTypes, i.GetGenericTypeDefinition());

                        // name: IAnimal
                        // genericNameResult: Dog<Black>
                        // result is then IAnimal<Dog<Black>>
                        var genericInterfaceName = $"{interfacePrefixName}<{genericNameResult}>";
                        return genericInterfaceName;
                    }).ToArray(),
                    Members = x.Value.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(
                            p =>
                            {
                                var generatorMember = new GeneratorMember
                                {
                                    Name = p.Name,
                                    Type = p.PropertyType,
                                    IsDeclaredAsGeneric = p.DeclaringType is not null &&
                                                          p.DeclaringType.IsGenericType &&
                                                          p.DeclaringType.GetGenericTypeDefinition()
                                                              .GetProperty(p.Name)!.PropertyType.IsGenericParameter,
                                    GenericName = "T"
                                };

                                // all new types in properties will be added to generation collection. 
                                TypesScriptGenerator.GetTypeScriptFieldTypeName(p.PropertyType, foundTypes,
                                    itIsForReturnTypeName: false, itIsForPropertyName: true);

                                // IEnumerable<>, IList<>, List<>.... List<T> ist then T[].
                                if (p.PropertyType.IsIEnumerableOfT() && p.PropertyType.IsGenericType &&
                                    p.PropertyType.GetGenericArguments().Length > 0)
                                {
                                    var checkType = p.PropertyType.GetGenericArguments()[0];

                                    // Only when Class (declaringType) is generic?
                                    if (p.DeclaringType is not null &&
                                        p.DeclaringType.GenericTypeArguments.Any(a => a.Name == checkType.Name))
                                    {
                                        generatorMember.IsDeclaredAsGeneric = true;
                                        generatorMember.GenericName = "T[]";
                                    }
                                }

                                return generatorMember;
                            }).ToArray()
                };
                resultGeneratorType.Documentation = x.Value.GetDocumentation()?.OnlyDocumentationText();
                return resultGeneratorType;
            }).ToList();

        if (foundTypes.Any())
        {
            // check for not generated types.
            var notGeneratedTypes = foundTypes.Where(ft => usedTypes.All(ut => ft.Key != ut.Key)).ToDictionary();
            if (notGeneratedTypes.Any())
            {
                // generate it
                returnGeneratorTypes.AddRange(MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes));
                return returnGeneratorTypes;
            }
        }

        return returnGeneratorTypes.ToList();
    }

    private static GeneratorType GetClassGeneratorType(Type type, Dictionary<string, Type> returnTypes, Type returnTypeFilter)
    {
        return new GeneratorType
        {
            Name = type.Name,
            Kind = GeneratorTypeKind.Class,
            ImplementsInterfaceTypeNames = type.GetInterfaces().Select(i =>
            {
                if (!i.IsGenericType) return i.Name;

                // name is generic. we get generic types 
                var interfacePrefixName = $"{i.Name.Substring(0, i.Name.IndexOf("`"))}";
                var genericNameResult = GetReturnTypeName(type, returnTypes, i.GetGenericTypeDefinition());

                // name: IAnimal
                // genericNameResult: Dog<Black>
                // result is then IAnimal<Dog<Black>>
                var genericInterfaceName = $"{interfacePrefixName}<{genericNameResult}>";
                return genericInterfaceName;
            }).ToArray(),
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