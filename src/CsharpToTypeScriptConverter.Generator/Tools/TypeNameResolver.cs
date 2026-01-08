using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptRequestCommandsGenerator.Tools
{
    public static class TypeNameResolver
    {
        private static readonly HashSet<Type> nullableTypes =
        [
            typeof(Nullable), typeof(Nullable<>)
        ];

        private static readonly HashSet<Type> numberTypes =
        [
            typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double),
            typeof(decimal)
        ];

        private static readonly HashSet<Type> stringTypes =
        [
            typeof(char), typeof(string), typeof(Guid), typeof(DateTime)
        ];

        private static readonly HashSet<Type> objectType =
        [
            typeof(object)
        ];

        public static string Resolve(Type type, bool generateNullableTypesAsType = false,
            string replaceTypeNameWithThisName = null)
        {
            string result = string.Empty;
            if (type is null)
            {
                return result;
            }

            bool isGeneric = type.IsGenericType;
            bool isArray = type.IsArray;
            bool isGenericTypeDefinition = type.IsGenericTypeDefinition;

            // it is nullable?
            if (!generateNullableTypesAsType && isGeneric &&
                nullableTypes.Contains(type.GetGenericTypeDefinition()))
            {
                var nullableType = Nullable.GetUnderlyingType(type);
                return Resolve(nullableType, generateNullableTypesAsType);
            }

            if (isGeneric)
            {
                // Type<T>; Type<T1, T2>
                if (isGenericTypeDefinition)
                {
                    type = type.GetGenericTypeDefinition();
                    string genericTypeDefinitionName =
                        string.IsNullOrWhiteSpace(replaceTypeNameWithThisName)
                            ? GetNameFromGenericName(type)
                            : replaceTypeNameWithThisName;

                    var genericTypeDefinitionGenericArguments = type.GetGenericArguments();
                    string genericChainName = genericTypeDefinitionGenericArguments
                        .Select(x => x.Name)
                        .Aggregate((x1, x2) => $"{x1}, {x2}");
                    return $"{genericTypeDefinitionName}<{genericChainName}>";
                }

                // IEnumerable<User>
                if (type.IsIEnumerableOfT())
                {
                    type = type.GetGenericArguments()[0];
                    string genericParameterName = Resolve(type, generateNullableTypesAsType);
                    return $"{genericParameterName}[]";
                }

                // Type<User>; Type<User, Role>
                string name = string.IsNullOrWhiteSpace(replaceTypeNameWithThisName)
                    ? GetNameFromGenericName(type)
                    : replaceTypeNameWithThisName;

                string genericArguments = type.GetGenericArguments()
                    .Select(x => Resolve(x, generateNullableTypesAsType))
                    .Aggregate((x1, x2) => $"{x1}, {x2}");

                string genericResult = $"{name}<{genericArguments}>";
                if (type.IsArray)
                {
                    genericResult += "[]";
                }

                return genericResult;
            }

            if (numberTypes.Contains(type))
            {
                result = "number";
            }
            else if (stringTypes.Contains(type))
            {
                result = "string";
            }
            else if (type == typeof(bool))
            {
                result = "boolean";
            }
            else if (objectType.Contains(type))
            {
                result = "any";
            }
            else
            {
                if (isArray)
                {
                    var elementType = type.GetElementType();
                    if (numberTypes.Contains(elementType))
                    {
                        return "number[]";
                    }

                    if (stringTypes.Contains(elementType))
                    {
                        return "string[]";
                    }

                    if (type == typeof(bool))
                    {
                        return "boolean[]";
                    }

                    if (objectType.Contains(elementType))
                    {
                        return "any";
                    }

                    string elementName = Resolve(elementType);
                    if (!isGenericTypeDefinition)
                    {
                        return elementName + "[]";
                    }
                }

                result = string.IsNullOrWhiteSpace(replaceTypeNameWithThisName)
                    ? type.Name
                    : replaceTypeNameWithThisName;
            }

            return result;
        }

        private static string GetNameFromGenericName(Type type) =>
            $"{type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal))}";

        public static bool IsIEnumerableOfT(this Type type) =>
            type
                .GetInterfaces()
                .Append(type)
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }
}
