using YamlDotNet.Serialization;

namespace ProfileList2.Lib.Manifest
{
    public class ScriptPath
    {
        [YamlMember(Alias = "os")]
        public OSType OS { get; set; }

        [YamlMember(Alias = "path")]
        public string Path { get; set; }
    }
}
