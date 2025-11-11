using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptRequestCommandsGenerator.Tools
{
    public class TypeDependencyResolver
    {
        private readonly TypeDependencyResolverOptions options = new();
        private readonly List<Type> ignoreCustomerTypes = [];

        private readonly List<Type> ignoredCSharpValueType =
        [
            typeof(bool),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(long),
            typeof(string),
            typeof(char)
        ];

        private readonly List<Type> ignoredCSharpComplexTypes =
        [
            typeof(List<>),
            typeof(IList),
            typeof(IEnumerable),
            typeof(ICollection),
            typeof(IQueryable),
            typeof(Array)
        ];

        public TypeDependencyResolver(List<Type> ignoreTypes = null, TypeDependencyResolverOptions options = null)
        {
            if (ignoreTypes != null)
            {
                this.ignoreCustomerTypes.AddRange(ignoreTypes);
            }

            if (options != null)
            {
                this.options = options;
            }
        }

        public List<(Type Type, TypeKind Kind)> GetDependencies(Type type, bool includeSelf = true)
        {
            var dependencies = new List<(Type type, TypeKind typeKind)>();
            if (this.ignoredCSharpValueType.Contains(type))
            {
                return dependencies.Distinct().Where(x => this.IsNotIgnoredCSharpType(x.type)).ToList();
            }

            if (type.IsGenericType)
            {
                foreach (var argument in type.GetGenericArguments())
                {
                    dependencies.AddRange(this.GetDependencies(argument));
                }
            }

            if (includeSelf)
            {
                dependencies.Add((type, this.GetTypeKind(type)));
            }

            if (type.IsClass || type.IsInterface)
            {
                if (this.options.ResolveInherits)
                {
                    var baseType = type.BaseType;
                    if (baseType != null && baseType != typeof(object))
                    {
                        dependencies.Add((baseType, this.GetTypeKind(baseType)));
                    }
                }

                if (this.options.ResolveInterfaces)
                {
                    var interfaces = type.GetInterfaces();
                    this.ResolveGenericsFromType(interfaces, dependencies);
                }

                if (this.options.ResolveFields)
                {
                    var fieldTypes = type.GetFields().Select(x => x.FieldType);
                    this.ResolveGenericsFromType(fieldTypes, dependencies);
                }

                if (this.options.ResolveProperties)
                {
                    var propertyTypes = type.GetProperties().Select(x => x.PropertyType);
                    this.ResolveGenericsFromType(propertyTypes, dependencies);
                }

                if (this.options.ResolveMethods)
                {
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        // parameter types
                        var parameterTypes = method.GetParameters().Select(x => x.ParameterType);
                        this.ResolveGenericsFromType(parameterTypes, dependencies);

                        // return type
                        var returnType = method.ReturnType;
                        this.ResolveGenericsFromType([returnType], dependencies);
                    }
                }
            }

            // remove doublets and ignored
            return dependencies.Distinct().Where(x => this.IsNotIgnoredCSharpType(x.type)).ToList();
        }

        private bool IsNotIgnoredCSharpType(Type type)
        {
            bool result = !this.ignoredCSharpComplexTypes.Any(it => it.IsAssignableFrom(type))
                          && !this.ignoredCSharpValueType.Contains(type)
                          && !this.ignoreCustomerTypes.Any(it => it.IsAssignableFrom(type));
            result = type.IsGenericType
                ? result && this.ignoreCustomerTypes.All(it => type.GetGenericTypeDefinition() != it)
                : result;
            return result;
        }

        private void ResolveGenericsFromType(IEnumerable<Type> types, List<(Type type, TypeKind typeKind)> dependencies)
        {
            foreach (var type in types)
            {
                if (type.IsGenericType)
                {
                    this.ResolveGenericsFromType(type.GetGenericArguments(), dependencies);
                }

                dependencies.Add((type, this.GetTypeKind(type)));
            }
        }

        private TypeKind GetTypeKind(Type type)
        {
            if (type.IsEnum)
            {
                return TypeKind.Enum;
            }
            else if (type.IsInterface)
            {
                return TypeKind.Interface;
            }
            else if (type.IsClass)
            {
                return TypeKind.Class;
            }
            else if (type.IsValueType)
            {
                return TypeKind.ValueType;
            }

            return TypeKind.Unknown;
        }

        public enum TypeKind
        {
            Unknown,
            Class,
            Interface,
            Enum,
            ValueType
        }
    }

    public class TypeDependencyResolverOptions
    {
        public bool ResolveFields { get; set; } = false;
        public bool ResolveProperties { get; set; } = true;
        public bool ResolveMethods { get; set; } = false;
        public bool ResolveInterfaces { get; set; } = true;
        public bool ResolveInherits { get; set; } = false;
    }
}
