using System;

namespace ATK.Command.CsToTsGenerator.Models
{
    public class FileMetadata
    {
        public string TransformedText { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public FileMetadataType FileMetadataType { get; set; }
    }
}
