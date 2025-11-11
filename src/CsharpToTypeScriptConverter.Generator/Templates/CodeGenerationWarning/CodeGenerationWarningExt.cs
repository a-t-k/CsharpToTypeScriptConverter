using System;
using System.Globalization;

namespace TypeScriptRequestCommandsGenerator.Templates.CodeGenerationWarning;

public partial class CodeGenerationWarning
{
    public static class Settings
    {
        public static string GenerationTime => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        public static string ToolName => "CsharpToTypeScriptConverter";
        public static string Version => Environment.Version.ToString();
    }
}
