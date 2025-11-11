using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace TypeScriptRequestCommandsGenerator.Tools
{
    /// <summary>
    /// Tolls for creating documentation.
    /// </summary>
    internal static class DocumentationTools
    {
        internal static Dictionary<string, string> LoadedXmlDocumentation = new();

        public static string GetDirectoryPath(this Assembly assembly)
        {
            string codeBase = assembly.Location;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        internal static HashSet<Assembly> LoadedAssemblies = [];

        internal static void LoadXmlDocumentation(Assembly assembly)
        {
            if (LoadedAssemblies.Contains(assembly))
            {
                return;
            }

            string directoryPath = assembly.GetDirectoryPath();
            string xmlFilePath = Path.Combine(directoryPath, assembly.GetName().Name + ".xml");
            if (File.Exists(xmlFilePath))
            {
                LoadXmlDocumentation(File.ReadAllText(xmlFilePath));
                LoadedAssemblies.Add(assembly);
            }
        }

        public static void LoadXmlDocumentation(string xmlDocumentation)
        {
            using var xmlReader = XmlReader.Create(new StringReader(xmlDocumentation));
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "member")
                {
                    continue;
                }

                string rawName = xmlReader["name"];
                if (rawName != null)
                {
                    LoadedXmlDocumentation[rawName] = xmlReader.ReadInnerXml();
                }
            }
        }

        public static string GetDocumentation(this MemberInfo memberInfo)
        {
            if (memberInfo.MemberType.HasFlag(MemberTypes.Field))
            {
                return ((FieldInfo)memberInfo).GetDocumentation();
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Property))
            {
                return ((PropertyInfo)memberInfo).GetDocumentation();
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Event))
            {
                return ((EventInfo)memberInfo).GetDocumentation();
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Constructor))
            {
                return ((ConstructorInfo)memberInfo).GetDocumentation();
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.Method))
            {
                return ((MethodInfo)memberInfo).GetDocumentation();
            }
            else if (memberInfo.MemberType.HasFlag(MemberTypes.TypeInfo) ||
                     memberInfo.MemberType.HasFlag(MemberTypes.NestedType))
            {
                return ((TypeInfo)memberInfo).GetDocumentation();
            }
            else
            {
                return null;
            }
        }

        // Helper method to format the key strings
        private static string XmlDocumentationKeyHelper(string typeFullNameString, string memberNameString)
        {
            string key = Regex.Replace(typeFullNameString, @"\[.*\]", string.Empty).Replace('+', '.');
            if (memberNameString != null)
            {
                key += "." + memberNameString;
            }

            return key;
        }

        public static string[] OnlyDocumentationText(this string fullDocumentation)
        {
            return fullDocumentation?.Split("\n").Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "<summary>" && x != "</summary>").ToArray();
        }

        public static string GetDocumentation(this Type type)
        {
            LoadXmlDocumentation(type.Assembly);
            string key = "T:" + XmlDocumentationKeyHelper(type.FullName, null);
            LoadedXmlDocumentation.TryGetValue(key, out string documentation);
            return documentation;
        }

        public static string GetDocumentation(this PropertyInfo propertyInfo)
        {
            string documentation = null;
            if (propertyInfo.DeclaringType != null)
            {
                string key = "P:" + XmlDocumentationKeyHelper(propertyInfo.DeclaringType.FullName, propertyInfo.Name);
                LoadedXmlDocumentation.TryGetValue(key, out documentation);
            }

            return documentation;
        }

        public static string GetDocumentation(this ParameterInfo parameterInfo)
        {
            string memberDocumentation = parameterInfo.Member.GetDocumentation();
            if (memberDocumentation != null)
            {
                string regexPattern =
                    Regex.Escape(@"<param name=" + "\"" + parameterInfo.Name + "\"" + @">") +
                    ".*?" +
                    Regex.Escape(@"</param>");
                var match = Regex.Match(memberDocumentation, regexPattern);
                if (match.Success)
                {
                    return match.Value;
                }
            }

            return null;
        }
    }
}
