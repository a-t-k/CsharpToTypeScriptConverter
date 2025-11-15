using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptRequestCommandsGenerator.Models;
using TypeScriptRequestCommandsGenerator.Tools;

namespace TypeScriptRequestCommandsGenerator.Generators.TypeScript.SeparatedFiles
{
    public class BuildedSeparatedFiles
    {
        public List<BuildFile> BuildFiles { get; }
        public IGrouping<string, FileMetadata>[] Groups { get; }
        private readonly string outputDirectory;
        private readonly TypeFileGenerator typeFileGenerator;

        public BuildedSeparatedFiles(List<BuildFile> buildFiles, string outputDirectory,
            IGrouping<string, FileMetadata>[] groups)
        {
            this.BuildFiles = buildFiles;
            this.Groups = groups;
            this.outputDirectory = outputDirectory;
            this.typeFileGenerator = new TypeFileGenerator();
        }

        public void Save()
        {
            foreach (var group in this.Groups)
            {
                this.typeFileGenerator.CreateDirectoryIfNotExist(group.Key);
            }

            foreach (var buildFile in this.BuildFiles)
            {
                File.WriteAllText(buildFile.Path, buildFile.Content);
            }
        }
    }
}
