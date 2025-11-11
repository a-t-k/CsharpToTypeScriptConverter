using System;

namespace TypeScriptRequestCommandsGenerator.Models
{
    public class FileMetadata
    {
        public string TransformedText { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Type Type { get; set; }
    }
}
