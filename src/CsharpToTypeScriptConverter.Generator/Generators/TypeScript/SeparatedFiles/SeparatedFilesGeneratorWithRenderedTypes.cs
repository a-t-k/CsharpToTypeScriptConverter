using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class SeparatedFilesGeneratorWithRenderedTypes
    {
        private readonly List<FileMetadata> fileMetadata;

        public SeparatedFilesGeneratorWithRenderedTypes(List<FileMetadata> fileMetadata) =>
            this.fileMetadata = fileMetadata;

        public BuildedSeparatedFiles Build(string outputDirectory)
        {
            this.PreventNameCollision();

            var buildFiles = new List<BuildFile>();
            var groups =
                this.fileMetadata.GroupBy(f => Path.Combine(outputDirectory, Path.Combine(f.FilePath.Split("/"))))
                    .ToArray();

            foreach (var group in groups)
            {
                string directoryPath = group.Key;
                string apiFilePath = Path.Combine(directoryPath, "api.ts");
                string apiContent = this.CreateApiContent(group.ToList());
                buildFiles.Add(new BuildFile
                {
                    BuildFiLeType = BuildFiLeType.ApiFile, Path = apiFilePath, Content = apiContent
                });
                foreach (var fileMetadataFromGroup in group)
                {
                    buildFiles.Add(new BuildFile
                    {
                        BuildFiLeType = BuildFiLeType.GeneratedCSharpSource,
                        Path = Path.Combine(directoryPath, fileMetadataFromGroup.FileName),
                        Content = fileMetadataFromGroup.TransformedText
                    });
                }
            }

            string indexContent = this.CreateIndexContent(groups);
            buildFiles.Add(new BuildFile
            {
                BuildFiLeType = BuildFiLeType.IndexFile,
                Path = Path.Combine(outputDirectory, "index.ts"),
                Content = indexContent
            });

            return new BuildedSeparatedFiles(buildFiles, outputDirectory, groups);
        }

        /// <summary>
        /// Prevent name collisions by appending a counter to files with the same name.
        /// </summary>
        private void PreventNameCollision()
        {
            // TODO: Generate better unique names, e.g., based on namespace or folder structure.
            // TODO: Logging can be added to inform about name changes.
            var sameNameGroups = this.fileMetadata.GroupBy(x => x.Name);
            foreach (var sameNameGroup in sameNameGroups)
            {
                if (sameNameGroup.Count() > 1)
                {
                    int counter = 0;
                    foreach (var fileMetadataFromGroup in sameNameGroup)
                    {
                        if (counter > 0)
                        {
                            string newName = $"{fileMetadataFromGroup.Name}_{counter}";
                            while (this.fileMetadata.Any(x => x.Name == newName))
                            {
                                counter++;
                                newName = $"{fileMetadataFromGroup.Name}_{counter}";
                            }

                            fileMetadataFromGroup.Name = newName;
                        }

                        counter++;
                    }
                }
            }
        }

        private string CreateApiContent(List<FileMetadata> fileMetadataCollection)
        {
            var exports = new List<string>();

            foreach (var fileMetaData in fileMetadataCollection)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileMetaData.FileName);
                string exportType = fileMetaData.Name == fileNameWithoutExtension
                    ? "*"
                    : $"{{ {fileNameWithoutExtension} as {fileMetaData.Name} }}";

                exports.Add($"""
                             export {exportType} from "./{fileNameWithoutExtension}";
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
