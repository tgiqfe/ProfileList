using System.Text.Json.Serialization;
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

        [YamlMember(Alias = "method")]
        public string Method { get; set; }

        public bool IsMatchMethod(string method)
        {
            return this.Method.
                Split(",").
                Select(x => x.Trim()).
                Any(x => x.Equals(method, StringComparison.OrdinalIgnoreCase));
        }
    }
}
