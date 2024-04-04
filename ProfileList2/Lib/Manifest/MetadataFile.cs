using System.Text;
using YamlDotNet.Serialization;

namespace ProfileList2.Lib.Manifest
{
    public class MetadataFile
    {
        [YamlMember(Alias = "metadata")]
        public Metadata Metadata { get; set; }

        public static MetadataFile Load(string path)
        {
            MetadataFile mdFile = null;
            try
            {
                var yml = File.ReadAllText(path, Encoding.UTF8);
                mdFile = new Deserializer().Deserialize<MetadataFile>(yml);
            }
            catch
            {
                var data = new MetadataFile()
                {
                    Metadata = new Metadata()
                    {
                        Name = "Unknown",
                        Description = "Unknown",
                        Script = new ScriptPath[]
                        {
                            new ScriptPath()
                            {
                                OS = OSType.Windows,
                                Path = "Unknown",
                            },
                            new ScriptPath()
                            {
                                OS = OSType.Linux,
                                Path = "Unknown",
                            },
                            new ScriptPath()
                            {
                                OS = OSType.Mac,
                                Path = "Unknown",
                            },
                        },
                        OutputType = OutputType.Console,
                        OutputFilePath = "Unknown",
                    }
                };
                string content = new Serializer().Serialize(data);
                File.WriteAllText(path, content, Encoding.UTF8);
            }
            mdFile ??= new();
            return mdFile;
        }
    }
}
