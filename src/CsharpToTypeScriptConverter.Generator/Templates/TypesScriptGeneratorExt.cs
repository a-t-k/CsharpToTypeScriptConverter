using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeScriptRequestCommandsGenerator.Models;

namespace TypeScriptRequestCommandsGenerator.Templates
{
    public partial class TypesScriptGenerator
    {
        public static class Settings
        {
            public static string RequestCommandInterfaceName { get; set; } = "ICommand";
        }

        public GeneratorType[] GeneratorTypes { get; set; }

        private IEnumerable<GeneratorType> Classes { get { return this.GeneratorTypes.Where(t => t.Kind == GeneratorTypeKind.Class || t.Kind == GeneratorTypeKind.Interface); } }
        private IEnumerable<GeneratorType> Enums { get { return this.GeneratorTypes.Where(t => t.Kind == GeneratorTypeKind.Enum); } }
        private IEnumerable<GeneratorType> ReturnTypes { get { return this.GeneratorTypes.Where(t => t.Kind == GeneratorTypeKind.UsedReturnType); } }

        public static bool GenerateNullableTypesAsType => false;
        private static readonly HashSet<Type> NullableTypes =
        [
            typeof(Nullable), typeof(Nullable<>)
        ];
        private static readonly HashSet<Type> NumberTypes =
        [
            typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double),
            typeof(decimal), typeof(Byte)
        ];
        private static readonly HashSet<Type> StringTypes =
        [
            typeof(char), typeof(string), typeof(Guid), typeof(DateTime)
        ];

        private static readonly HashSet<Type> ObjectType =
        [
            typeof(object)
        ];


        /// <summary>
        /// Returns a corresponding TypeScript type for a given .NET type
        /// </summary>
        public static string GetTypeScriptFieldTypeName(Type type, Dictionary<string, Type> returnTypes, bool itIsForReturnTypeName, bool itIsForPropertyName)
        {
            var result = string.Empty;
            if (type is null)
            {
                return result;
            }

            var isCollectionType = false;

            // it is nullable?
            if (!GenerateNullableTypesAsType && type.IsGenericType && NullableTypes.Contains(type.GetGenericTypeDefinition()))
            {
                var nullableType = Nullable.GetUnderlyingType(type);
                return GetTypeScriptFieldTypeName(nullableType, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
            }

            // Transfer IEnumerable to []
            if (type.IsGenericType && type.IsIEnumerableOfT())
            {
                var name = $"{type.Name.Substring(0, type.Name.IndexOf("`"))}";
                //if (returnTypes != null)
                //{
                //    AddReturnType($"{name}<T>", type, returnTypes);
                //}

                type = type.GetGenericArguments()[0];
                isCollectionType = true;
            }

            // Check if it is a generic. 
            if (type.IsGenericType)
            {
                var name = $"{type.Name.Substring(0, type.Name.IndexOf("`"))}";
                if (returnTypes != null && isCollectionType == false)
                {
                    AddReturnType($"{name}<T>", type, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
                }

                var genericArguments = type.GetGenericArguments()
                    .Select(x => GetTypeScriptFieldTypeName(x, returnTypes, itIsForReturnTypeName, itIsForPropertyName))
                    .Aggregate((x1, x2) => $"{x1}, {x2}");

                var genericResult = $"{name}<{genericArguments}>";

                if (type.IsArray) genericResult += "[]";
                return genericResult;
            }

            if (NumberTypes.Contains(type)) result = "number";
            else if (StringTypes.Contains(type)) result = "string";
            else if (type == typeof(bool)) result = "boolean";
            else if (ObjectType.Contains(type)) result = "any";
            else
            {
                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    if (NumberTypes.Contains(elementType)) return "number[]";
                    if (StringTypes.Contains(elementType)) return "string[]";
                    if (type == typeof(bool)) return "boolean[]";
                    if (ObjectType.Contains(elementType)) return "any";

                    var elementName = GetTypeScriptFieldTypeName(elementType, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
                    if (itIsForPropertyName)
                    {
                        return elementName + "[]";
                    }
                    AddReturnType(elementName, elementType, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
                }
                else
                {
                    AddReturnType(type.Name, type, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
                }

                result = type.Name;
            }

            if (isCollectionType) result += "[]";
            return result;
        }

        private static void AddReturnType(string name, Type type, Dictionary<string, Type> returnTypes, bool itIsForReturnTypeName, bool itIsForPropertyName)
        {
            if (returnTypes == null)
            {
                return;
            }

            var success = returnTypes.TryAdd(name, type);
            if (success)
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.PropertyType.IsEnum) continue;

                    GetTypeScriptFieldTypeName(property.PropertyType, returnTypes, itIsForReturnTypeName, itIsForPropertyName);
                }
            }
        }
    }
}