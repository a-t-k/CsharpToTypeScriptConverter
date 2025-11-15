using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class SeparatedFilesGeneratorWithRenderedTypes
    {
        private readonly List<FileMetadata> allGeneration;

        public SeparatedFilesGeneratorWithRenderedTypes(List<FileMetadata> allGeneration) =>
            this.allGeneration = allGeneration;

        public BuildedSeparatedFiles Build(string outputDirectory)
        {
            var buildFiles = new List<BuildFile>();
            var groups =
                this.allGeneration.GroupBy(f => Path.Combine(outputDirectory, Path.Combine(f.FilePath.Split("/"))))
                    .ToArray();

            foreach (var group in groups)
            {
                string directoryPath = group.Key;
                string apiFilePath = Path.Combine(directoryPath, "api.ts");
                string apiContent = this.CreateApiContent(group.ToList());
                buildFiles.Add(new BuildFile { Path = apiFilePath, Content = apiContent });
                foreach (var fileMetadata in group)
                {
                    buildFiles.Add(new BuildFile
                    {
                        Path = Path.Combine(directoryPath, fileMetadata.FileName),
                        Content = fileMetadata.TransformedText
                    });
                }
            }

            string indexContent = this.CreateIndexContent(groups);
            buildFiles.Add(new BuildFile { Path = Path.Combine(outputDirectory, "index.ts"), Content = indexContent });

            return new BuildedSeparatedFiles(buildFiles, outputDirectory, groups);
        }

        private string CreateApiContent(List<FileMetadata> filesMetadata)
        {
            var exports = new List<string>();

            foreach (var fileMetadata in filesMetadata)
            {
                string fileName = Path.GetFileNameWithoutExtension(fileMetadata.FileName);
                string path = string.IsNullOrWhiteSpace(fileMetadata.FilePath)
                    ? fileName
                    : fileMetadata.FilePath + "/" + fileName;

                exports.Add($"""
                             export * from "./{path}";
                             """);
            }

            return string.Join("\n", exports);
        }

        private string CreateIndexContent(IEnumerable<IGrouping<string, FileMetadata>> groups)
        {
            var exports = new List<string>();
            foreach (var group in groups)
            {
                string fileName = "api";
                string path = string.IsNullOrWhiteSpace(group.First().FilePath)
                    ? fileName
                    : group.First().FilePath + "/" + fileName;

                exports.Add($"""
                             export * from "./{path}";
                             """);
            }

            return string.Join("\n", exports);
        }
    }
}
