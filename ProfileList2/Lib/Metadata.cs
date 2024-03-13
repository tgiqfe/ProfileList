using YamlDotNet.Serialization;

namespace ProfileList2.Lib
{
    public class Metadata
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        [YamlMember(Alias = "scriptPath")]
        public string ScriptPath { get; set; }

        [YamlMember(Alias = "outputType")]
        public OutputType OutputType { get; set; }

        [YamlMember(Alias = "outputFilePath")]
        public string OutputFilePath { get; set; }
    }
}
