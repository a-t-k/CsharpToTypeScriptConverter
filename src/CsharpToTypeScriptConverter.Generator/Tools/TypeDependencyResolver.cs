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

        private readonly Type objectType = typeof(object);

        private readonly List<Type> ignoredCSharpValueType =
        [
            typeof(bool),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(byte),
            typeof(sbyte),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(string),
            typeof(char)
        ];

        private readonly List<Type> ignoredCSharpComplexTypes =
        [
            typeof(List<>),
            typeof(IList),
            typeof(IList<>),
            typeof(IEnumerable),
            typeof(IEnumerable<>),
            typeof(ICollection),
            typeof(ICollection<>),
            typeof(IQueryable),
            typeof(IQueryable<>),
            typeof(Array),
            typeof(Guid),
            typeof(DateTime),
            typeof(Nullable<>),
            typeof(Nullable)
        ];

        private readonly HashSet<Type> nullableTypes =
        [
            typeof(Nullable), typeof(Nullable<>)
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

            // it is nullable?
            if (type.IsGenericType && this.nullableTypes.Contains(type.GetGenericTypeDefinition()))
            {
                var nullableType = Nullable.GetUnderlyingType(type);
                return this.GetDependencies(nullableType);
            }

            if (this.ignoredCSharpValueType.Contains(type))
            {
                return this.DistinctAndFilterTypes(type, includeSelf, dependencies);
            }

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                foreach (var argument in type.GetGenericArguments())
                {
                    dependencies.AddRange(this.GetDependencies(argument));
                }
            }

            if (includeSelf)
            {
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    dependencies.Add((type.GetGenericTypeDefinition(), this.GetTypeKind(type)));
                }
                else
                {
                    dependencies.Add((type, this.GetTypeKind(type)));
                }
            }

            if (type.IsClass || type.IsInterface)
            {
                if (this.options.ResolveInherits)
                {
                    var baseType = type.BaseType;
                    if (baseType != null && baseType != typeof(object))
                    {
                        this.ResolveGenericsFromType([baseType], dependencies);
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

            return this.DistinctAndFilterTypes(type, includeSelf, dependencies);
        }

        /// <summary>
        /// remove doublets and ignored types
        /// </summary>
        /// <param name="type">collection filter this out, when includeSelf is true</param>
        /// <param name="includeSelf">filter or not type</param>
        /// <param name="dependencies">all found dependency types</param>
        /// <returns></returns>
        private List<(Type type, TypeKind typeKind)> DistinctAndFilterTypes(Type type, bool includeSelf,
            List<(Type type, TypeKind typeKind)> dependencies) =>
            dependencies.Distinct()
                .Where(x => this.IsNotIgnoredType(x.type))
                .Where(x => includeSelf || x.type != type)
                .ToList();

        private bool IsNotIgnoredType(Type type)
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
                if (type == this.objectType)
                {
                    continue;
                }

                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    if (elementType != null)
                    {
                        this.ResolveGenericsFromType([elementType], dependencies);
                        continue;
                    }
                }

                // its "T" as Class with empty GUID and empty FullName. Only for generic definition
                if (type.IsGenericTypeParameter)
                {
                    continue;
                }

                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    this.ResolveGenericsFromType(type.GetGenericArguments(), dependencies);
                    dependencies.Add((type.GetGenericTypeDefinition(), this.GetTypeKind(type)));
                    continue;
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

        public Dictionary<string, Type> GetAllDependencies(params Type[] types)
        {
            var allDependencies = new Dictionary<string, Type>();
            var typesToProcess = new Queue<Type>(types);
            while (typesToProcess.Count > 0)
            {
                var currentType = typesToProcess.Dequeue();
                var dependencies = this.GetDependencies(currentType);
                foreach (var (type, _) in dependencies)
                {
                    if (allDependencies.TryAdd(type.FullName, type))
                    {
                        typesToProcess.Enqueue(type);
                    }
                }
            }

            return allDependencies;
        }
    }

    public class TypeDependencyResolverOptions
    {
        public bool ResolveFields { get; set; } = false;
        public bool ResolveProperties { get; set; } = true;
        public bool ResolveMethods { get; set; } = false;
        public bool ResolveInterfaces { get; set; } = true;
        public bool ResolveInherits { get; set; } = true;
    }
}
